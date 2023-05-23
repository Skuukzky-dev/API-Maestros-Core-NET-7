using API_Maestros_Core.BLL;
using GESI.CORE.BLL;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using API_Maestros_Core.Models;
using Newtonsoft.Json;
using Error = API_Maestros_Core.Models.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        public static GESI.CORE.BLL.SessionMgr _SessionMgr;
        public static List<GESI.CORE.BO.Verscom2k.HabilitacionesAPI> moHabilitacionesAPI;
        public static string mostrTipoAPI = "LEER_MAESTROS";
        public static string strUsuarioID = "";
        public static bool HabilitadoPorToken = false;
        // GET: api/<ProductosController>
        /// <summary>
        /// Devuelve la lista de Resultados de Busqueda de una Expresion
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetList")]
        [EnableCors("MyCorsPolicy")]
        
        public IActionResult Get(string id, int pageNumber = 1, int pageSize = 10)
        {
            HttpResponseMessage respuesta = new HttpResponseMessage();
            RespuestaConProductos oRespuesta = new RespuestaConProductos();
            try
            {
                if (!HabilitadoPorToken)
                {
                    respuesta.StatusCode = HttpStatusCode.BadRequest;
                    // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 401;
                    oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                    Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario");                   
                    oRespuesta.success = false;
                    string json = JsonConvert.SerializeObject(oRespuesta);
                    respuesta.Content = new StringContent(json);
                    this.StatusCode((int)HttpStatusCode.Unauthorized);
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                    _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                    bool Habilitado = false;
                    if (id != null)
                    {
                        if (id.Length > 0)
                        {
                            foreach (GESI.CORE.BO.Verscom2k.HabilitacionesAPI oHabilitacionAPI in moHabilitacionesAPI)
                            {
                                if (oHabilitacionAPI.TipoDeAPI.Equals(mostrTipoAPI))
                                {
                                    _SessionMgr.EmpresaID = oHabilitacionAPI.EmpresaID;
                                    _SessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                                    _SessionMgr.SucursalID = oHabilitacionAPI.SucursalID;
                                    _SessionMgr.EntidadID = 1;
                                    Habilitado = true;
                                }
                            }

                            if (Habilitado)
                            {
                                ProductosMgr._SessionMgr = _SessionMgr;
                                List<GESI.ERP.Core.BO.cProducto> lstProductos = ProductosMgr.GetList(id);
                                Paginacion oPaginacion = new Paginacion();
                                oPaginacion.totalElementos = lstProductos.Count;
                                oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                                oPaginacion.paginaActual = pageNumber;
                                oPaginacion.tamañoPagina = pageSize;
                                oRespuesta.paginacion = oPaginacion;
                                lstProductos = lstProductos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                                respuesta.StatusCode = HttpStatusCode.OK;
                                oRespuesta.error = new Error();
                                oRespuesta.success = true;
                                oRespuesta.Productos = lstProductos;
                                string json = JsonConvert.SerializeObject(oRespuesta);
                                respuesta.Content = new StringContent(json);
                                Logger.LoguearErrores("Respuesta exitosa para la expresion "+id);
                                this.StatusCode((int)HttpStatusCode.OK);
                                return Ok(oRespuesta);
                            }
                            else
                            {

                                respuesta.StatusCode = HttpStatusCode.BadRequest;
                                // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                                oRespuesta.error = new Error();
                                oRespuesta.error.code = 401;
                                oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                                Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario");
                                oRespuesta.success = false;
                                string json = JsonConvert.SerializeObject(oRespuesta);
                                respuesta.Content = new StringContent(json);
                                this.StatusCode((int)HttpStatusCode.Unauthorized);
                                return Unauthorized(oRespuesta);
                            }
                        }
                        else
                        {
                            respuesta.StatusCode = HttpStatusCode.BadRequest;
                            // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                            oRespuesta.error = new Error();
                            oRespuesta.error.code = 400;
                            oRespuesta.error.message = "No se encontro expresion a buscar";
                            Logger.LoguearErrores("No se encontro expresion a buscar");
                            oRespuesta.success = false;
                            string json = JsonConvert.SerializeObject(oRespuesta);
                            respuesta.Content = new StringContent(json);
                            this.StatusCode((int)HttpStatusCode.NotFound);
                            return NotFound(oRespuesta);
                        }
                    }
                    else
                    {
                        respuesta.StatusCode = HttpStatusCode.BadRequest;
                        //RespuestaConProductos oRespuesta = new RespuestaConProductos();
                        oRespuesta.error = new Error();
                        oRespuesta.error.code = 400;
                        oRespuesta.error.message = "No se encontro expresion a buscar";
                        Logger.LoguearErrores("No se encontro expresion a buscar");
                        oRespuesta.success = false;
                        string json = JsonConvert.SerializeObject(oRespuesta);
                        respuesta.Content = new StringContent(json);
                        this.StatusCode((int)HttpStatusCode.NotFound);
                        return NotFound(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.StatusCode = HttpStatusCode.BadRequest;
               // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                oRespuesta.error = new Error();
                oRespuesta.error.code = 500;
                oRespuesta.error.message = "error interno de la aplicacion. Descripcion: " + ex.Message;
                Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message);
                oRespuesta.success = false;
                string json = JsonConvert.SerializeObject(oRespuesta);
                respuesta.Content = new StringContent(json);
                this.StatusCode((int)HttpStatusCode.InternalServerError);
                return StatusCode(500,oRespuesta);
            }
         
        }

        // GET api/<ProductosController>/5
        /// <summary>
        /// Devuelve un producto con sus precios e imagenes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CanalDeVentaID"></param>
        /// <returns></returns>
        [HttpGet("GetItem/{id}")]
        [EnableCors("MyCorsPolicy")]
        public IActionResult Get(string id,int CanalDeVentaID = 0)
        {
            HttpResponseMessage respuesta = new HttpResponseMessage();
            RespuestaConProductos oRespuesta = new RespuestaConProductos();
            try
            {
                if (!HabilitadoPorToken)
                {
                    respuesta.StatusCode = HttpStatusCode.BadRequest;
                    // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 401;
                    oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                    Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario");
                    oRespuesta.success = false;
                    string json = JsonConvert.SerializeObject(oRespuesta);
                    respuesta.Content = new StringContent(json);
                    this.StatusCode((int)HttpStatusCode.BadRequest);
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                    string strCanalesDeVenta = null;
                    _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                    bool Habilitado = false;
                    if (id != null)
                    {
                        if (id.Length > 0)
                        {
                            foreach (GESI.CORE.BO.Verscom2k.HabilitacionesAPI oHabilitacionAPI in moHabilitacionesAPI)
                            {
                                if (oHabilitacionAPI.TipoDeAPI.Equals(mostrTipoAPI))
                                {
                                    _SessionMgr.EmpresaID = oHabilitacionAPI.EmpresaID;
                                    _SessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                                    _SessionMgr.SucursalID = oHabilitacionAPI.SucursalID;
                                    _SessionMgr.EntidadID = 1;
                                    strCanalesDeVenta = oHabilitacionAPI.CanalesDeVenta;
                                    Habilitado = true;
                                }
                            }

                            if (Habilitado)
                            {

                                ProductosMgr._SessionMgr = _SessionMgr;
                                GESI.ERP.Core.BO.cProducto lstProductos = ProductosMgr.GetItem(id, strCanalesDeVenta,CanalDeVentaID);

                                Paginacion oPaginacion = new Paginacion();
                                oPaginacion.totalElementos = 1;
                                oPaginacion.totalPaginas = 1;
                                oPaginacion.paginaActual = 1;
                                oPaginacion.tamañoPagina = 1;
                                oRespuesta.paginacion = oPaginacion;

                                respuesta.StatusCode = HttpStatusCode.OK;
                                oRespuesta.error = new Error();
                                oRespuesta.success = true;
                                oRespuesta.Productos = new List<GESI.ERP.Core.BO.cProducto>();
                                oRespuesta.Productos.Add(lstProductos);
                                string json = JsonConvert.SerializeObject(oRespuesta);
                                respuesta.Content = new StringContent(json);
                                Logger.LoguearErrores("Exitoso para el codigo "+id);
                                this.StatusCode((int)HttpStatusCode.OK);
                                return Ok(oRespuesta);
                            }
                            else
                            {
                                respuesta.StatusCode = HttpStatusCode.BadRequest;
                                // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                                oRespuesta.error = new Error();
                                oRespuesta.error.code = 401;
                                oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                                Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario");
                                oRespuesta.success = false;
                                string json = JsonConvert.SerializeObject(oRespuesta);
                                respuesta.Content = new StringContent(json);
                                this.StatusCode((int)HttpStatusCode.BadRequest);
                                return Unauthorized(oRespuesta);
                            }
                        }
                        else
                        {
                            respuesta.StatusCode = HttpStatusCode.BadRequest;
                            //    RespuestaConProductos oRespuesta = new RespuestaConProductos();
                            oRespuesta.error = new Error();
                            oRespuesta.error.code = 400;
                            oRespuesta.error.message = "No se encontro expresion a buscar";
                            Logger.LoguearErrores("No se encontro expresion a buscar");
                            oRespuesta.success = false;
                            string json = JsonConvert.SerializeObject(oRespuesta);
                            respuesta.Content = new StringContent(json);
                            this.StatusCode((int)HttpStatusCode.NotFound);
                            return NotFound(oRespuesta);
                        }
                    }
                    else
                    {
                        respuesta.StatusCode = HttpStatusCode.BadRequest;
                        //RespuestaConProductos oRespuesta = new RespuestaConProductos();
                        oRespuesta.error = new Error();
                        oRespuesta.error.code = 400;
                        oRespuesta.error.message = "No se encontro expresion a buscar";
                        Logger.LoguearErrores("No se encontro expresion a buscar");
                        oRespuesta.success = false;
                        string json = JsonConvert.SerializeObject(oRespuesta);
                        respuesta.Content = new StringContent(json);
                        this.StatusCode((int)HttpStatusCode.NotFound);
                        return NotFound(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.StatusCode = HttpStatusCode.BadRequest;
                //RespuestaConProductos oRespuesta = new RespuestaConProductos();
                oRespuesta.error = new Error();
                oRespuesta.error.code = 500;
                oRespuesta.error.message = "Error interno de la aplicacion. Descripcion: " + ex.Message;
                Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message);
                oRespuesta.success = false;
                string json = JsonConvert.SerializeObject(oRespuesta);
                respuesta.Content = new StringContent(json);
                this.StatusCode((int)HttpStatusCode.InternalServerError);
                return StatusCode(500, oRespuesta);
            }
           
        }

        // POST api/<ProductosController>
     /*   [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProductosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
