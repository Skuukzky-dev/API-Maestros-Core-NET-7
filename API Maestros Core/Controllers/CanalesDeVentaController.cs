using API_Maestros_Core.BLL;
using API_Maestros_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanalesDeVentaController : ControllerBase
    {
        public static GESI.CORE.BLL.SessionMgr _SessionMgr;
        public static List<GESI.CORE.BO.Verscom2k.HabilitacionesAPI> moHabilitacionesAPI;
        public static string mostrTipoAPI = "LEER_MAESTROS";
        public static string strUsuarioID = "";
        public static bool HabilitadoPorToken = false;
        // GET: api/<CanalesDeVentaController>
        [HttpGet("GetList")]
        [Authorize]
        [EnableCors("MyCorsPolicy")]
        public IActionResult Get()
        {
            RespuestaConCanalesDeVenta oRespuesta = new RespuestaConCanalesDeVenta();
            try
            {
               
                GESI.GESI.BO.ListaCanalesDeVenta lstCanales = new GESI.GESI.BO.ListaCanalesDeVenta();
                moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                bool Habilitado = false;
                if (!HabilitadoPorToken)
                {
                    oRespuesta.success = false;
                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 401;
                    oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                    Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario");
                    this.StatusCode((int)HttpStatusCode.Unauthorized);
                    return Unauthorized(oRespuesta);
                    //return oRespuesta;
                }
                else
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
                        GESI.GESI.BLL.TablasGeneralesGESIMgr.SessionManager = _SessionMgr;
                        oRespuesta.success = true;
                        oRespuesta.error = new Error();
                        oRespuesta.CanalesDeVenta = new List<GESI.GESI.BO.CanalDeVenta>();
                        oRespuesta.CanalesDeVenta.AddRange(GESI.GESI.BLL.TablasGeneralesGESIMgr.CanalesDeVentaGetList());
                        this.StatusCode((int)HttpStatusCode.OK);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oRespuesta.success = false;
                        oRespuesta.error = new Error();
                        oRespuesta.error.code = 401;
                        oRespuesta.error.message = "No esta autorizado a acceder al recurso";
                        Logger.LoguearErrores("No esta autorizado a acceder al recurso");
                        this.StatusCode((int)HttpStatusCode.Unauthorized);
                        return Unauthorized(oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                oRespuesta.success = false;
                oRespuesta.error = new Error();
                oRespuesta.error.code = 500;
                oRespuesta.error.message = "Error interno de la aplicacion. Descripcion: "+ex.Message;
                Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message);
                this.StatusCode((int)HttpStatusCode.InternalServerError);
                return Unauthorized(oRespuesta);
            }

          
        }
        /*
        // GET api/<CanalesDeVentaController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CanalesDeVentaController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CanalesDeVentaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CanalesDeVentaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}
