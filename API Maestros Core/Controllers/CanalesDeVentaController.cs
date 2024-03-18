using API_Maestros_Core.BLL;

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
        public static List<GESI.CORE.API.BO.TipoDeError> lstTipoErrores = GESI.CORE.API.BLL.APIHelper.LlenarTiposDeError();
        public GESI.CORE.API.BO.TipoDeError oTipoError;
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
        [SwaggerResponse(200, "OK", typeof(GESI.CORE.API.BO.ResponseCanalesDeVenta))]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            GESI.CORE.API.BLL.APIHelper.SetearConnectionString();
            #endregion

            GESI.CORE.API.BO.ResponseCanalesDeVenta oRespuesta = new GESI.CORE.API.BO.ResponseCanalesDeVenta();
            try
            {
               
                GESI.GESI.BO.ListaCanalesDeVenta lstCanales = new GESI.GESI.BO.ListaCanalesDeVenta();
                
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoErrores.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CanalesDeVentaGetList);
                    oRespuesta.success = false;                   
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];
                    string Referer = this.HttpContext.Request.Headers["Referer"].ToString();
                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        GESI.CORE.API.BO.APISessionManager MiSessionMgrAPI = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);

                        if (MiSessionMgrAPI.Habilitado)
                        {
                            GESI.CORE.API.BLL.CanalesDeVentaMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                            oRespuesta = GESI.CORE.API.BLL.CanalesDeVentaMgr.GetListCanalesDeVenta(MiSessionMgrAPI.CanalesDeVenta,pageNumber, pageSize,Referer);
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
                            oTipoError = lstTipoErrores.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cTokenInvalido);
                            oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cTokenInvalido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CanalesDeVentaGetList);
                            oRespuesta.success = false;
                            return Unauthorized(oRespuesta);
                        }
                    }
                    else
                    {
                        oTipoError = lstTipoErrores.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                oTipoError = lstTipoErrores.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Descripcion: " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CanalesDeVentaGetList);
                oRespuesta.success = false;               
                return Unauthorized(oRespuesta);
            }

          
        }
       
    }
}
