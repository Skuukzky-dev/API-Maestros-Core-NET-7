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
        public static List<TipoDeError> lstTipoErrores = APIHelper.LlenarTiposDeError();
        public TipoDeError oTipoError;
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

            APIHelper.SetearConnectionString();
            #endregion

            RespuestaConCanalesDeVenta oRespuesta = new RespuestaConCanalesDeVenta();
            try
            {
               
                GESI.GESI.BO.ListaCanalesDeVenta lstCanales = new GESI.GESI.BO.ListaCanalesDeVenta();
                
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoErrores.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CanalesDeVentaGetList);
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
                            oTipoError = lstTipoErrores.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cTokenInvalido);
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cTokenInvalido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CanalesDeVentaGetList);
                            oRespuesta.success = false;
                            return Unauthorized(oRespuesta);
                        }
                    }
                    else
                    {
                        oTipoError = lstTipoErrores.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                oTipoError = lstTipoErrores.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Descripcion: " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CanalesDeVentaGetList);
                oRespuesta.success = false;               
                return Unauthorized(oRespuesta);
            }

          
        }
       
    }
}
