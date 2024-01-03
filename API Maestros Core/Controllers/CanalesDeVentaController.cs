using API_Maestros_Core.BLL;
using API_Maestros_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanalesDeVentaController : ControllerBase
    {
        #region Variables
        public static GESI.CORE.BLL.SessionMgr _SessionMgr;
        public static List<GESI.CORE.BO.Verscom2k.HabilitacionesAPI> moHabilitacionesAPI;
        public static string mostrTipoAPI = "LEER_MAESTROS";
        public static string strUsuarioID = "";
        public static bool HabilitadoPorToken = false;
        public static string TokenEnviado = "";
        public static string strProtocolo = "";
        #endregion

        // GET: api/<CanalesDeVentaController>
        /// <summary>
        /// Devuelve la lista de canales de venta en un CanalesDeVenta/GetList
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        [Authorize]
        [EnableCors("MyCorsPolicy")]
        [SwaggerOperation(Tags = new[] {"Canales de Venta"})]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCanalesDeVenta))]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);            
            
            
         //   SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
         //   GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion

            RespuestaConCanalesDeVenta oRespuesta = new RespuestaConCanalesDeVenta();
            try
            {
               
                GESI.GESI.BO.ListaCanalesDeVenta lstCanales = new GESI.GESI.BO.ListaCanalesDeVenta();
                
                if (!HabilitadoPorToken)
                {
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, "No esta autorizado a acceder al servicio. No se encontro el token del usuario. Token Enviado: " + TokenEnviado, "E", strUsuarioID, APIHelper.CanalesDeVentaGetList);
                    oRespuesta.success = false;                   
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];
                    string Referer = this.HttpContext.Request.Headers["Referer"].ToString();
                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiSessionMgrAPI = APIHelper.SetearMgrAPI(strUsuarioID);

                        if (MiSessionMgrAPI.Habilitado)
                        {
                            CanalesDeVentaMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                            oRespuesta = CanalesDeVentaMgr.GetListCanalesDeVenta(MiSessionMgrAPI.CanalesDeVenta,pageNumber, pageSize,Referer);
                            if (oRespuesta != null)
                            {
                                return Ok(oRespuesta);
                            }
                            else
                            {
                                return NoContent();
                            }
                        }
                        else
                        {
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cTokenInvalido, "No esta autorizado a acceder al recurso", "E", strUsuarioID, APIHelper.CanalesDeVentaGetList);
                            oRespuesta.success = false;
                            return Unauthorized(oRespuesta);
                        }
                    }
                    else
                    {
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, "Protocolo Incorrecto en la solicitud", "E", strUsuarioID, APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, "Error interno de la aplicacion. Descripcion: " + ex.Message, "E", strUsuarioID, APIHelper.CanalesDeVentaGetList);
                oRespuesta.success = false;               
                return Unauthorized(oRespuesta);
            }

          
        }
       
    }
}
