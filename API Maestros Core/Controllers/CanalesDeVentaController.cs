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
            
            
            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion

            RespuestaConCanalesDeVenta oRespuesta = new RespuestaConCanalesDeVenta();
            try
            {
               
                GESI.GESI.BO.ListaCanalesDeVenta lstCanales = new GESI.GESI.BO.ListaCanalesDeVenta();
                string CanalesDeVenta = null;

                if (!HabilitadoPorToken)
                {
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, "No esta autorizado a acceder al servicio. No se encontro el token del usuario. Token Enviado: " + TokenEnviado, "E", strUsuarioID, APIHelper.CanalesDeVentaGetList);
                    oRespuesta.success = false;                   
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    APISessionManager MiSessionMgrAPI = APIHelper.SetearMgrAPI(strUsuarioID);                                      

                    if (MiSessionMgrAPI.Habilitado)
                    {
                        GESI.GESI.BLL.TablasGeneralesGESIMgr.SessionManager = MiSessionMgrAPI.SessionMgr;
                        oRespuesta.success = true;
                        oRespuesta.error = new Error();
                        oRespuesta.CanalesDeVenta = new List<GESI.GESI.BO.CanalDeVenta>();
                        oRespuesta.paginacion = new Paginacion();
                        List<GESI.GESI.BO.CanalDeVenta> lstCanalesDeVenta = new List<GESI.GESI.BO.CanalDeVenta>();
                        lstCanalesDeVenta = GESI.GESI.BLL.TablasGeneralesGESIMgr.CanalesDeVentaGetList(CanalesDeVenta);
                        oRespuesta.CanalesDeVenta = lstCanalesDeVenta.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                        oRespuesta.paginacion.totalElementos = lstCanalesDeVenta.Count;
                        oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);
                        oRespuesta.paginacion.paginaActual = pageNumber;
                        oRespuesta.paginacion.tamañoPagina = pageSize;
                        Logger.LoguearErrores("Respuesta GetList Canales de venta OK", "I", MiSessionMgrAPI.SessionMgr.UsuarioID, APIHelper.CanalesDeVentaGetList);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cTokenInvalido, "No esta autorizado a acceder al recurso", "E", strUsuarioID, APIHelper.CanalesDeVentaGetList);
                        oRespuesta.success = false;                        
                        return Unauthorized(oRespuesta);
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
