using API_Maestros_Core.BLL;
using GESI.CORE.BLL;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

using Newtonsoft.Json;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Configuration;
using System.Data.SqlClient;

using System.Text;
using System.Reflection;
using Swashbuckle.AspNetCore.Annotations;
using GESI.GESI.BO;
using Microsoft.OpenApi.Expressions;
using Microsoft.AspNetCore.Http;
using GESI.CORE.DAL;
using GESI.CORE.DAL.Verscom2k;
using Microsoft.AspNetCore.Identity;
using GESI.CORE.API.BO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        #region Variables
        public static GESI.CORE.BLL.SessionMgr _SessionMgr;
        public static List<GESI.CORE.BO.Verscom2k.HabilitacionesAPI> moHabilitacionesAPI;
        public static string mostrTipoAPI = "LEER_MAESTROS";
        public static string strUsuarioID = "";
        public static bool HabilitadoPorToken = false;
        public static string TokenEnviado = "";
        public static string strProtocolo = "";
        public static List<GESI.CORE.API.BO.TipoDeError> lstTiposDeError = GESI.CORE.API.BLL.APIHelper.LlenarTiposDeError();
        public static GESI.CORE.API.BO.TipoDeError oTipo = new GESI.CORE.API.BO.TipoDeError();
        
        #endregion

        // GET: api/<ProductosController>
        /// <summary>
        /// Devuelve toda la lista de productos de manera paginada
        /// </summary>
        /// <param name="expresion"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseProductos))]

        public IActionResult Get(int pageNumber = 1, int pageSize = 10, string costos = "N",string imagenes = "N",string fechamodificaciones = "",string stock = "N",string publicaecommerce = "T")
        {
            #region ConnectionsStrings
            GESI.CORE.API.BLL.APIHelper.SetearConnectionString();

            ResponseProductos oRespuesta = new ResponseProductos();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            GESI.CORE.API.BLL.ProductosMgr.lstTipoErrores = lstTiposDeError;
           // SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion


            try
            {
              
                if (!HabilitadoPorToken)
                {

                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError +"Token Recibido: "+TokenEnviado, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);                                    
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"]; 

                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiSessionMgrAPI = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);

                        if (MiSessionMgrAPI.Habilitado)
                        {
                            GESI.CORE.API.BLL.ProductosMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                            int TamanoPagina = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["TamanoPagina"]);

                            if (pageSize <= TamanoPagina) // Si el tamaño de la pagina enviado es menor a 100.
                            {
                                if (pageSize > 100)
                                { pageSize = 100; }

                                oRespuesta = GESI.CORE.API.BLL.ProductosMgr.GetList(pageNumber, pageSize, MiSessionMgrAPI.CanalesDeVenta, costos, MiSessionMgrAPI.CostosXProveedor, MiSessionMgrAPI.EstadoProductos, MiSessionMgrAPI.CategoriasIDs, imagenes, fechamodificaciones, stock, MiSessionMgrAPI.Almacenes, publicaecommerce);

                                return Ok(oRespuesta);
                            }
                            else
                            {
                                if (TamanoPagina > 100)
                                    TamanoPagina = 100;

                                oRespuesta = GESI.CORE.API.BLL.ProductosMgr.GetList(pageNumber, TamanoPagina, MiSessionMgrAPI.CanalesDeVenta, costos, MiSessionMgrAPI.CostosXProveedor, MiSessionMgrAPI.EstadoProductos, MiSessionMgrAPI.CategoriasIDs, imagenes, fechamodificaciones, stock, MiSessionMgrAPI.Almacenes, publicaecommerce);
                                return Ok(oRespuesta);

                            }

                        }
                        else
                        {
                            #region Sin autorizacion de acceder al servicio
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                            oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError+" Token enviado: " + TokenEnviado,oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);
                            oRespuesta.success = false;
                            return Unauthorized(oRespuesta);
                            #endregion
                        }
                    }
                    else
                    {
                        #region ERROR PROTOCOLO
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                        #endregion
                    }
                    
                }
            }
            catch (FormatException fex)
            {
                #region Error conversion Fecha 
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorConversionDato);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorConversionDato, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);                
                oRespuesta.success = false;
                return BadRequest(oRespuesta);
                #endregion
            }
            catch (Exception ex)
            {
                #region Error interno de la Aplicacion
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message+" Haciendo: "+ GESI.CORE.API.BLL.APIHelper.QueEstabaHaciendo, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);               
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError,oRespuesta);
                #endregion
            }

        }

        // GET api/<ProductosController>/5
        /// <summary>
        /// Devuelve un producto con sus precios e imagenes
        /// </summary>
        /// <param name="ProductoID"></param>
        /// <param name="CanalDeVentaID"></param>
        /// <returns></returns>
        [HttpGet("GetItem")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseProducto))]
        public IActionResult Get(string productoID = "",int canalDeVentaID = 0,string costos = "N",string imagenes = "N",string stock = "N")
        {

            GESI.CORE.API.BLL.APIHelper.SetearConnectionString();

            ResponseProducto oRespuesta = new ResponseProducto();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);



            GESI.CORE.API.BLL.ProductosMgr.lstTipoErrores = lstTiposDeError;
            if (!ModelState.IsValid)
            {
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido, "Modelo no valido", "E", strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.UnsupportedMediaType, oRespuesta);
                
            }
            else
            {
                string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                {
                    GESI.CORE.API.BLL.APIHelper.QueEstabaHaciendo = "Buscando producto";
                    if (productoID == null)
                    {
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                    else
                    {
                        if (!(productoID.Length > 0))
                        {
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                            oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                            oRespuesta.success = false;
                            return BadRequest(oRespuesta);
                        }
                        else
                        {

                            try
                            {
                                if (!HabilitadoPorToken)
                                {
                                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                                    oRespuesta.success = false;
                                    return Unauthorized(oRespuesta);
                                }
                                else
                                {

                                    if (productoID != null)
                                    {
                                        if (productoID.Length > 0)
                                        {
                                            APISessionManager MiSessionMgr = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);

                                            if (MiSessionMgr.Habilitado)
                                            {

                                                GESI.CORE.API.BLL.ProductosMgr._SessionMgr = MiSessionMgr.SessionMgr;
                                                oRespuesta = GESI.CORE.API.BLL.ProductosMgr.GetItem(productoID, MiSessionMgr.CanalesDeVenta, canalDeVentaID, costos, MiSessionMgr.CostosXProveedor, MiSessionMgr.EstadoProductos, MiSessionMgr.CategoriasIDs, imagenes, stock, MiSessionMgr.Almacenes);

                                                if (oRespuesta.producto?.ProductoID?.Length > 0)
                                                {
                                                    return Ok(oRespuesta);
                                                }
                                                else
                                                {
                                                    return StatusCode((int)HttpStatusCode.NoContent, oRespuesta);
                                                }
                                            }
                                            else
                                            {
                                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                                                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                                                oRespuesta.success = false;
                                                return Unauthorized(oRespuesta);
                                            }
                                        }
                                        else
                                        {
                                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                                            oRespuesta.error = new GESI.CORE.API.BO.Error();
                                            oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                                            oRespuesta.success = false;
                                            return StatusCode((int)HttpStatusCode.NoContent, oRespuesta);
                                        }
                                    }
                                    else
                                    {
                                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                                        oRespuesta.success = false;
                                        return StatusCode((int)HttpStatusCode.NoContent, oRespuesta);
                                    }
                                }
                            }
                            catch (AccessViolationException ax)
                            {
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cPermisoDenegadoCostos);
                                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cPermisoDenegadoCostos, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);

                            }
                            catch (Exception ex)
                            {
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message + ". Codigo: " + productoID + " Haciendo: " + GESI.CORE.API.BLL.APIHelper.QueEstabaHaciendo, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                                oRespuesta.success = false;
                                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                            }
                        }
                    }
                }
                else
                {
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                    oRespuesta.success = false;
                    return BadRequest(oRespuesta);
                }
            }
           
        }

        /// <summary>
        /// Devuelve la lista de productos por una expresion de busqueda
        /// </summary>
        /// <param name="oRequest"></param>
        /// <returns></returns>
        [HttpGet("GetSearchResult")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseProductos))]
        public IActionResult Get(string expresion = "",int pageNumber = 1 , int pageSize = 10,string costos = "N",string categoriasafiltrar = "",string imagenes = "N",string stock = "N",string publicaecommerce = "T",int canaldeventaID = 0,string orden  = "O")
        {
            #region ConnectionStrings
            GESI.CORE.API.BLL.APIHelper.SetearConnectionString();

            ResponseProductos oRespuesta = new ResponseProductos();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            GESI.CORE.API.BLL.ProductosMgr.lstTipoErrores = lstTiposDeError;
            //     SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //   GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion

            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetSearchResult);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        List<int> CategoriasPorParametro = new List<int>();

                        if (expresion?.Length > 0)
                        {
                            APISessionManager MiSessionMgrAPI = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);

                            if (MiSessionMgrAPI.Habilitado)
                            {
                                #region Control Categorias
                                MiSessionMgrAPI.Habilitado = true;

                                categoriasafiltrar = categoriasafiltrar.Replace(" ", "");

                                if (categoriasafiltrar?.Length == 0)
                                {
                                    MiSessionMgrAPI.Habilitado = true;
                                }
                                else
                                {
                                    string[] partes = categoriasafiltrar.Split(',');

                                    foreach (string parte in partes)
                                    {
                                        if (int.TryParse(parte, out int numero))
                                        {
                                            CategoriasPorParametro.Add(numero);
                                        }
                                        else
                                        {
                                            MiSessionMgrAPI.Habilitado = false;
                                        }
                                    }
                                }

                                #endregion

                                if (MiSessionMgrAPI.Habilitado)
                                {
                                    GESI.CORE.API.BLL.ProductosMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                                    int TamanoPagina = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["TamanoPagina"]);

                                    if (pageSize <= TamanoPagina) // Si el tamaño de la pagina enviado es menor a 100.
                                    {
                                        if (pageSize > 100)
                                        { pageSize = 100; }

                                        oRespuesta = GESI.CORE.API.BLL.ProductosMgr.GetList(expresion, MiSessionMgrAPI.CanalesDeVenta, costos, MiSessionMgrAPI.CostosXProveedor, pageNumber, pageSize, MiSessionMgrAPI.EstadoProductos, MiSessionMgrAPI.CategoriasIDs, imagenes, stock, MiSessionMgrAPI.Almacenes, publicaecommerce,canaldeventaID,orden);

                                        return Ok(oRespuesta);
                                    }
                                    else
                                    {
                                        if (TamanoPagina > 100) // Si el tamaño de la variable configuracion es mayor a 100. Toma los 100
                                            TamanoPagina = 100;
                                        oRespuesta = GESI.CORE.API.BLL.ProductosMgr.GetList(expresion, MiSessionMgrAPI.CanalesDeVenta, costos, MiSessionMgrAPI.CostosXProveedor, pageNumber, TamanoPagina, MiSessionMgrAPI.EstadoProductos, MiSessionMgrAPI.CategoriasIDs, imagenes, stock, MiSessionMgrAPI.Almacenes, publicaecommerce,canaldeventaID,orden);

                                        return Ok(oRespuesta);
                                    }
                                }
                                else
                                {
                                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cSintaxisIncorrecta);
                                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cSintaxisIncorrecta, oTipo.DescripcionError+" Descripcion " + categoriasafiltrar, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetSearchResult);
                                    oRespuesta.success = false;
                                    return BadRequest(oRespuesta);
                                }

                            }
                            else
                            {
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetSearchResult);
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);
                            }

                        }
                        else
                        {
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                            oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetSearchResult);
                            oRespuesta.success = false;
                            return StatusCode(204, oRespuesta);
                        }
                    }
                    else
                    {
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message+" .Expresion: "+expresion, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetSearchResult);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }


        /// <summary>
        /// Devuelve la lista de Existencias por Deposito y Producto
        /// </summary>
        /// <param name="codigos"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetExistencias")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseExistencias))]
        public IActionResult Get(string codigos = "", int pageNumber = 1, int pageSize = 10)
        {

            #region ConnectionStrings

            GESI.CORE.API.BLL.APIHelper.SetearConnectionString();
            ResponseExistencias oRespuesta = new ResponseExistencias();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            GESI.CORE.API.BLL.ProductosMgr.lstTipoErrores = lstTiposDeError;

            //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion


            if (!HabilitadoPorToken)
            {
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetExistencias);
                oRespuesta.success = false;
                return Unauthorized(oRespuesta);
            }
            else
            {
                string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                 
                try
                {
                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        if (codigos?.Length > 0)
                        {
                            APISessionManager MiSessionMgrAPI = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);

                            if (MiSessionMgrAPI.Habilitado)
                            {
                                GESI.CORE.API.BLL.ProductosMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                                oRespuesta = GESI.CORE.API.BLL.ProductosMgr.GetExistencias(codigos, pageNumber, pageSize, MiSessionMgrAPI.Almacenes);
                                oRespuesta.success = true;
                                if (oRespuesta.existencias == null)
                                {
                                    return NoContent();
                                }
                                else
                                {
                                    return Ok(oRespuesta);
                                }
                            }
                            else
                            {
                                #region Autorizacion
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetExistencias);
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);
                                #endregion
                            }
                        }
                        else
                        {
                            #region No hay codigos definidos
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                            oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.DescripcionError, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetExistencias);
                            oRespuesta.success = false;
                            return BadRequest(oRespuesta);
                            #endregion
                        }
                    }
                    else
                    {
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetExistencias);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
                catch (Exception ex)
                {
                    #region Error
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message+" codigos:" +codigos, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetExistencias);
                    oRespuesta.success = false;
                    return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                    #endregion
                }
            }
        }

        /// <summary>
        /// Devuelve la lista de Precios por Canal de Venta y Producto
        /// </summary>
        /// <param name="codigos"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="fechamodificaciones"></param>
        /// <returns></returns>
        [HttpGet("GetPrecios")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponsePrecios))]
        public IActionResult GetPrecios(string codigos = "", int pageNumber = 1, int pageSize = 10,string fechamodificaciones = "")
        {
            #region ConnectionStrings

            GESI.CORE.API.BLL.APIHelper.SetearConnectionString();
            ResponsePrecios oRespuesta = new ResponsePrecios();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            GESI.CORE.API.BLL.ProductosMgr.lstTipoErrores = lstTiposDeError;
            //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //  GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;

            #endregion


            if (!HabilitadoPorToken)
            {
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductoGetPrecios);
                oRespuesta.success = false;
                return Unauthorized(oRespuesta);
            }
            else
            {
                try
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {

                        if (codigos?.Length > 0 || fechamodificaciones?.Length > 0)
                        {
                            APISessionManager MiSessionMgrAPI = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);

                            if (MiSessionMgrAPI.Habilitado)
                            {
                                GESI.CORE.API.BLL.ProductosMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;

                                oRespuesta = GESI.CORE.API.BLL.ProductosMgr.GetPrecios(codigos, MiSessionMgrAPI.CanalesDeVenta, pageNumber, pageSize, fechamodificaciones);

                                if (oRespuesta.precios == null)
                                {
                                    return NoContent();
                                }
                                else
                                {
                                    return Ok(oRespuesta);
                                }

                            }
                            else
                            {
                                #region Autorizacion
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);

                                oRespuesta.error = new GESI.CORE.API.BO.Error();
                                oRespuesta.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken;
                                oRespuesta.error.message = oTipo.DescripcionError;
                                oRespuesta.success = false;
                                GESI.CORE.API.BLL.Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario. Token Enviado: " + TokenEnviado, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductoGetPrecios);
                                return Unauthorized(oRespuesta);
                                #endregion
                            }
                        }
                        else
                        {
                            #region No hay codigos definidos
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                            oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductoGetPrecios);
                            return BadRequest(oRespuesta);
                            #endregion
                        }
                    }
                    else
                    {
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
                catch (FormatException fex)
                {
                    #region Error
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorConversionDato);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorConversionDato, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductoGetPrecios);                    
                    oRespuesta.success = false;
                    return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                    #endregion

                }
                catch (Exception ex)
                {
                    #region Error
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message+" codigos: "+codigos, oTipo.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductoGetPrecios);
                    oRespuesta.success = false;
                    return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                    #endregion
                }
            }

        }
    }

   

    

}
