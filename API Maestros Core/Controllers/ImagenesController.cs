using API_Maestros_Core.BLL;
using GESI.CORE.BO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagenesController : ControllerBase
    {
        // GET: api/<ImagenesController>
      /*  [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ImagenesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }*/

        // POST api/<ImagenesController>
       /* [HttpPost]
        [Authorize]
        [EnableCors("MyCorsPolicy")]
        public async Task<IActionResult> Post([FromForm] ImagenProducto oImagen)
        {
            List<RespuestaImagen> oRespuestaImagenes = new List<RespuestaImagen>();
            try
            {
               
                if (!ModelState.IsValid) // Si el modelo no es valido
                {
                    var file = Request.Form.Files[0];
                    RespuestaImagen oRespuesta = new RespuestaImagen();
                    oRespuesta.error = new Error();
                    oRespuesta.error.message = "Modelo no valido";
                    oRespuesta.success = false;
                    oRespuestaImagenes.Add(oRespuesta);
                    return BadRequest(oRespuestaImagenes);
                }
                else
                {
                    if (oImagen.productoID == null)
                    {
                        RespuestaImagen oRespuesta = new RespuestaImagen();
                        oRespuesta.error = new Error();
                        oRespuesta.error.message = "No se encontro el productoID en la solicitud";
                        oRespuesta.error.code = 4001;
                        oRespuesta.success = false;
                        oRespuestaImagenes.Add(oRespuesta);
                        return BadRequest(oRespuestaImagenes);
                    }
                    else
                    {
                        if (Request.Form.Files.Count > 0)
                        {

                            for (int i = 0; i < Request.Form.Files.Count; i++)
                            {
                                RespuestaImagen oRespuesta = new RespuestaImagen();
                                var file = Request.Form.Files[i]; // Obtén el archivo de la solicitud
                                if (file.Length > 0)
                                {
                                    if (file.ContentType.Contains("image")) // Si es de tipo Imagen
                                    {
                                        // Accede a los atributos adicionales
                                        using (var memoryStream = new MemoryStream())
                                        {
                                            await file.CopyToAsync(memoryStream);
                                            byte[] imageData = memoryStream.ToArray();
                                            oImagen.imagen = imageData;
                                            oRespuesta = ImagenesMgr.Save(oImagen);
                                            oRespuesta.imagenID = i + 1;
                                            oRespuestaImagenes.Add(oRespuesta);
                                        }

                                    }
                                    else
                                    {
                                        oRespuesta.error = new Error();
                                        oRespuesta.error.message = "El archivo enviado no es de tipo Imagen";
                                        oRespuesta.success = false;
                                        oRespuesta.imagenID = i + 1;
                                        oRespuestaImagenes.Add(oRespuesta);

                                    }
                                }
                                else
                                {
                                    #region No contiene un tipo de Imagen
                                    if (!file.ContentType.Contains("image"))
                                    {
                                        oRespuesta.error = new Error();
                                        oRespuesta.error.message = "El archivo enviado no es de tipo Imagen";
                                        oRespuesta.success = false;
                                        oRespuesta.imagenID = i + 1;
                                        oRespuestaImagenes.Add(oRespuesta);
                                    }
                                    else
                                    {
                                        oRespuesta.error = new Error();
                                        oRespuesta.error.message = "No se encontraron imagenes en la solicitud";
                                        oRespuesta.success = false;
                                        oRespuestaImagenes.Add(oRespuesta);
                                    }
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            RespuestaImagen oRespuesta = new RespuestaImagen();
                            oRespuesta.error = new Error();
                            oRespuesta.error.message = "No se encontraron imagenes en la solicitud";
                            oRespuesta.success = false;
                            oRespuestaImagenes.Add(oRespuesta);
                        }
                    }
                    return Ok(oRespuestaImagenes);
                }
            }
            catch (Exception ex)
            {
                RespuestaImagen oRespuesta = new RespuestaImagen();
                oRespuesta.error = new Error();
                oRespuesta.error.message = "Error interno de la aplicacion. Descripcion: "+ex.Message;
                oRespuesta.success = false;
                oRespuestaImagenes.Add(oRespuesta);
                return StatusCode(500,oRespuestaImagenes);
            }
        }

        // PUT api/<ImagenesController>/5
        [HttpPut("{imagenID}")]
        [Authorize]
        [EnableCors("MyCorsPolicy")]
        public async Task<IActionResult> Put(int imagenID,[FromForm] ImagenProducto oImagen)
        {
            try
            {
                if (!ModelState.IsValid) // Si el modelo no es valido
                {
                    var file = Request.Form.Files[0];
                    return BadRequest("Modelo no valido");
                }
                else
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0]; // Obtén el archivo de la solicitud

                        if (file.Length > 0)
                        {
                            if (file.ContentType.Contains("image")) // Si es de tipo Imagen
                            {
                                // Accede a los atributos adicionales
                                var attribute1 = oImagen.productoID;
                                var attribute2 = oImagen.imagenID;
                                oImagen.imagenID = imagenID;
                                if (file.Length > 256 * 1024) // Supera los 256 kb
                                {
                                    return BadRequest("El archivo no debe superar los 256 kb");
                                }
                                else
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        await file.CopyToAsync(memoryStream);
                                        byte[] imageData = memoryStream.ToArray();
                                        oImagen.imagen = imageData;

                                    }
                                    return Ok("Se recibio correctamente la imagen");
                                }
                            }
                            else
                            {
                                return BadRequest("El archivo enviado no es de tipo Imagen");
                            }
                        }
                        else
                        {
                            if (!file.ContentType.Contains("image"))
                            {
                                return BadRequest("El archivo enviado no es de tipo Imagen");
                            }
                            else
                            {
                                return BadRequest("No se encontro imagen en la solicitud");
                            }
                        }
                    }
                    else
                    {
                        return BadRequest("No se encontro imagen en la solicitud");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }*/

        // DELETE api/<ImagenesController>/5
       /* [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }

    public class ImagenProducto
    {
        private string? _productoID;
        private byte[]? _imagen;
        private int _imagenID;

        public string? productoID { get => _productoID; set => _productoID = value; }
        public byte[]? imagen { get => _imagen; set => _imagen = value; }
        public int imagenID { get => _imagenID; set => _imagenID = value; }
    }
}
