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
using System.Configuration;
using System.Data.SqlClient;
using GESI.ERP.Core.DAL;
using System.Text;
using System.Reflection;
using Swashbuckle.AspNetCore.Annotations;
using GESI.GESI.BO;
using Microsoft.OpenApi.Expressions;
using Microsoft.AspNetCore.Http;
using GESI.CORE.DAL;
using GESI.ERP.Core.BO;
using GESI.CORE.DAL.Verscom2k;
using Microsoft.AspNetCore.Identity;

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
        public static List<TipoDeError> lstTiposDeError = APIHelper.LlenarTiposDeError();
        public static TipoDeError oTipo = new TipoDeError();
        
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
        [SwaggerResponse(200, "OK", typeof(RespuestaConProductosHijos))]

        public IActionResult Get(int pageNumber = 1, int pageSize = 10, string costos = "N",string imagenes = "N",string fechamodificaciones = "",string stock = "N",string publicaecommerce = "T")
        {
            #region ConnectionsStrings
            APIHelper.SetearConnectionString();

            RespuestaConProductosHijos oRespuesta = new RespuestaConProductosHijos();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ProductosMgr.lstTipoErrores = lstTiposDeError;
           // SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion


            try
            {
              
                if (!HabilitadoPorToken)
                {

                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError +"Token Recibido: "+TokenEnviado, oTipo.TipoErrorAdvertencia, strUsuarioID,APIHelper.ProductosGetList);                                    
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"]; 

                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiSessionMgrAPI = APIHelper.SetearMgrAPI(strUsuarioID);

                        if (MiSessionMgrAPI.Habilitado)
                        {
                            ProductosMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                            int TamanoPagina = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["TamanoPagina"]);

                            if (pageSize <= TamanoPagina) // Si el tamaño de la pagina enviado es menor a 100.
                            {
                                if (pageSize > 100)
                                { pageSize = 100; }

                                oRespuesta = ProductosMgr.GetList(pageNumber, pageSize, MiSessionMgrAPI.CanalesDeVenta, costos, MiSessionMgrAPI.CostosXProveedor, MiSessionMgrAPI.EstadoProductos, MiSessionMgrAPI.CategoriasIDs, imagenes, fechamodificaciones, stock, MiSessionMgrAPI.Almacenes, publicaecommerce);

                                return Ok(oRespuesta);
                            }
                            else
                            {
                                if (TamanoPagina > 100)
                                    TamanoPagina = 100;

                                oRespuesta = ProductosMgr.GetList(pageNumber, TamanoPagina, MiSessionMgrAPI.CanalesDeVenta, costos, MiSessionMgrAPI.CostosXProveedor, MiSessionMgrAPI.EstadoProductos, MiSessionMgrAPI.CategoriasIDs, imagenes, fechamodificaciones, stock, MiSessionMgrAPI.Almacenes, publicaecommerce);
                                return Ok(oRespuesta);

                            }

                        }
                        else
                        {
                            #region Sin autorizacion de acceder al servicio
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError+" Token enviado: " + TokenEnviado,oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);
                            oRespuesta.success = false;
                            return Unauthorized(oRespuesta);
                            #endregion
                        }
                    }
                    else
                    {
                        #region ERROR PROTOCOLO
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                        #endregion
                    }
                    
                }
            }
            catch (FormatException fex)
            {
                #region Error conversion Fecha 
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorConversionDato);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorConversionDato, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);                
                oRespuesta.success = false;
                return BadRequest(oRespuesta);
                #endregion
            }
            catch (Exception ex)
            {
                #region Error interno de la Aplicacion
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message+" Haciendo: "+ APIHelper.QueEstabaHaciendo, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);               
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
        [SwaggerResponse(200, "OK", typeof(RespuestaConProductosHijos))]
        public IActionResult Get(string productoID = "",int canalDeVentaID = 0,string costos = "N",string imagenes = "N",string stock = "N")
        {

            APIHelper.SetearConnectionString();

            RespuestaProductosGetItem oRespuesta = new RespuestaProductosGetItem();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);



            ProductosMgr.lstTipoErrores = lstTiposDeError;
            if (!ModelState.IsValid)
            {
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cModeloNoValido);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cModeloNoValido, "Modelo no valido", "E", strUsuarioID,APIHelper.ProductosGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.UnsupportedMediaType, oRespuesta);
                
            }
            else
            {
                string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                {
                    APIHelper.QueEstabaHaciendo = "Buscando producto";
                    if (productoID == null)
                    {
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                    else
                    {
                        if (!(productoID.Length > 0))
                        {
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                            oRespuesta.success = false;
                            return BadRequest(oRespuesta);
                        }
                        else
                        {

                            try
                            {
                                if (!HabilitadoPorToken)
                                {
                                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                                    oRespuesta.success = false;
                                    return Unauthorized(oRespuesta);
                                }
                                else
                                {

                                    if (productoID != null)
                                    {
                                        if (productoID.Length > 0)
                                        {
                                            APISessionManager MiSessionMgr = APIHelper.SetearMgrAPI(strUsuarioID);

                                            if (MiSessionMgr.Habilitado)
                                            {

                                                ProductosMgr._SessionMgr = MiSessionMgr.SessionMgr;
                                                oRespuesta = ProductosMgr.GetItem(productoID, MiSessionMgr.CanalesDeVenta, canalDeVentaID, costos, MiSessionMgr.CostosXProveedor, MiSessionMgr.EstadoProductos, MiSessionMgr.CategoriasIDs, imagenes, stock, MiSessionMgr.Almacenes);

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
                                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                                                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                                                oRespuesta.success = false;
                                                return Unauthorized(oRespuesta);
                                            }
                                        }
                                        else
                                        {
                                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                                            oRespuesta.error = new Error();
                                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                                            oRespuesta.success = false;
                                            return StatusCode((int)HttpStatusCode.NoContent, oRespuesta);
                                        }
                                    }
                                    else
                                    {
                                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                                        oRespuesta.success = false;
                                        return StatusCode((int)HttpStatusCode.NoContent, oRespuesta);
                                    }
                                }
                            }
                            catch (AccessViolationException ax)
                            {
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cPermisoDenegadoCostos);
                                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cPermisoDenegadoCostos, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);

                            }
                            catch (Exception ex)
                            {
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message + ". Codigo: " + productoID + " Haciendo: " + APIHelper.QueEstabaHaciendo, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                                oRespuesta.success = false;
                                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                            }
                        }
                    }
                }
                else
                {
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
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
        [SwaggerResponse(200, "OK", typeof(RespuestaProductosGetResult))]
        public IActionResult Get(string expresion = "",int pageNumber = 1 , int pageSize = 10,string costos = "N",string categoriasafiltrar = "",string imagenes = "N",string stock = "N",string publicaecommerce = "T",int canaldeventaID = 0,string orden  = "O")
        {
            #region ConnectionStrings
            APIHelper.SetearConnectionString();

            RespuestaConProductosHijos oRespuesta = new RespuestaConProductosHijos();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ProductosMgr.lstTipoErrores = lstTiposDeError;
            //     SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //   GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion

            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetSearchResult);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        List<int> CategoriasPorParametro = new List<int>();

                        if (expresion?.Length > 0)
                        {
                            APISessionManager MiSessionMgrAPI = APIHelper.SetearMgrAPI(strUsuarioID);

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
                                    ProductosMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                                    int TamanoPagina = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["TamanoPagina"]);

                                    if (pageSize <= TamanoPagina) // Si el tamaño de la pagina enviado es menor a 100.
                                    {
                                        if (pageSize > 100)
                                        { pageSize = 100; }

                                        oRespuesta = ProductosMgr.GetList(expresion, MiSessionMgrAPI.CanalesDeVenta, costos, MiSessionMgrAPI.CostosXProveedor, pageNumber, pageSize, MiSessionMgrAPI.EstadoProductos, MiSessionMgrAPI.CategoriasIDs, imagenes, stock, MiSessionMgrAPI.Almacenes, publicaecommerce,canaldeventaID,orden);

                                        return Ok(oRespuesta);
                                    }
                                    else
                                    {
                                        if (TamanoPagina > 100) // Si el tamaño de la variable configuracion es mayor a 100. Toma los 100
                                            TamanoPagina = 100;
                                        oRespuesta = ProductosMgr.GetList(expresion, MiSessionMgrAPI.CanalesDeVenta, costos, MiSessionMgrAPI.CostosXProveedor, pageNumber, TamanoPagina, MiSessionMgrAPI.EstadoProductos, MiSessionMgrAPI.CategoriasIDs, imagenes, stock, MiSessionMgrAPI.Almacenes, publicaecommerce,canaldeventaID,orden);

                                        return Ok(oRespuesta);
                                    }
                                }
                                else
                                {
                                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cSintaxisIncorrecta);
                                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cSintaxisIncorrecta, oTipo.DescripcionError+" Descripcion " + categoriasafiltrar, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetSearchResult);
                                    oRespuesta.success = false;
                                    return BadRequest(oRespuesta);
                                }

                            }
                            else
                            {
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetSearchResult);
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);
                            }

                        }
                        else
                        {
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetSearchResult);
                            oRespuesta.success = false;
                            return StatusCode(204, oRespuesta);
                        }
                    }
                    else
                    {
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message+" .Expresion: "+expresion, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetSearchResult);
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
        [SwaggerResponse(200, "OK", typeof(RespuestaProductosGetExistencias))]
        public IActionResult Get(string codigos = "", int pageNumber = 1, int pageSize = 10)
        {

            #region ConnectionStrings

            APIHelper.SetearConnectionString();
            RespuestaProductosGetExistencias oRespuesta = new RespuestaProductosGetExistencias();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ProductosMgr.lstTipoErrores = lstTiposDeError;

            //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion


            if (!HabilitadoPorToken)
            {
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID,APIHelper.ProductosGetExistencias);
                oRespuesta.success = false;
                return Unauthorized(oRespuesta);
            }
            else
            {
                string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                 
                try
                {
                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        if (codigos?.Length > 0)
                        {
                            APISessionManager MiSessionMgrAPI = APIHelper.SetearMgrAPI(strUsuarioID);

                            if (MiSessionMgrAPI.Habilitado)
                            {
                                ProductosMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                                oRespuesta = ProductosMgr.GetExistencias(codigos, pageNumber, pageSize, MiSessionMgrAPI.Almacenes);
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
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetExistencias);
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);
                                #endregion
                            }
                        }
                        else
                        {
                            #region No hay codigos definidos
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.DescripcionError, strUsuarioID, APIHelper.ProductosGetExistencias);
                            oRespuesta.success = false;
                            return BadRequest(oRespuesta);
                            #endregion
                        }
                    }
                    else
                    {
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetExistencias);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
                catch (Exception ex)
                {
                    #region Error
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message+" codigos:" +codigos, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetExistencias);
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
        [SwaggerResponse(200, "OK", typeof(RespuestaProductosGetPrecios))]
        public IActionResult GetPrecios(string codigos = "", int pageNumber = 1, int pageSize = 10,string fechamodificaciones = "")
        {
            #region ConnectionStrings

            APIHelper.SetearConnectionString();
            RespuestaProductosGetPrecios oRespuesta = new RespuestaProductosGetPrecios();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ProductosMgr.lstTipoErrores = lstTiposDeError;
            //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //  GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;

            #endregion


            if (!HabilitadoPorToken)
            {
                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID,APIHelper.ProductoGetPrecios);
                oRespuesta.success = false;
                return Unauthorized(oRespuesta);
            }
            else
            {
                try
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {

                        if (codigos?.Length > 0 || fechamodificaciones?.Length > 0)
                        {
                            APISessionManager MiSessionMgrAPI = APIHelper.SetearMgrAPI(strUsuarioID);

                            if (MiSessionMgrAPI.Habilitado)
                            {
                                ProductosMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;

                                oRespuesta = ProductosMgr.GetPrecios(codigos, MiSessionMgrAPI.CanalesDeVenta, pageNumber, pageSize, fechamodificaciones);

                                if (oRespuesta.Precios == null)
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
                                oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);

                                oRespuesta.error = new Error();
                                oRespuesta.error.code = (int)APIHelper.cCodigosError.cNuevoToken;
                                oRespuesta.error.message = oTipo.DescripcionError;
                                oRespuesta.success = false;
                                Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario. Token Enviado: " + TokenEnviado, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductoGetPrecios);
                                return Unauthorized(oRespuesta);
                                #endregion
                            }
                        }
                        else
                        {
                            #region No hay codigos definidos
                            oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud);
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cCodigoNoHalladoEnLaSolicitud, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductoGetPrecios);
                            return BadRequest(oRespuesta);
                            #endregion
                        }
                    }
                    else
                    {
                        oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
                catch (FormatException fex)
                {
                    #region Error
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorConversionDato);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorConversionDato, oTipo.DescripcionError, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductoGetPrecios);                    
                    oRespuesta.success = false;
                    return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                    #endregion

                }
                catch (Exception ex)
                {
                    #region Error
                    oTipo = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipo.DescripcionError+" Descripcion: " + ex.Message+" codigos: "+codigos, oTipo.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductoGetPrecios);
                    oRespuesta.success = false;
                    return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                    #endregion
                }
            }

        }
    }

    public class ResponseGetItem
    {
        private string _ProductoID;
        private int _CanalDeVentaID;
        private string _costos;
        public string ProductoID { get => _ProductoID; set => _ProductoID = value; }
        public int CanalDeVentaID { get => _CanalDeVentaID; set => _CanalDeVentaID = value; }
        public string costos { get => _costos; set => _costos = value; }

        public ResponseGetItem(string ProductoID = "",string costos = "N")
        {
            _ProductoID = ProductoID;
            _costos = costos;
        }

    }

    public class ResponseGetList
    {
      
        private int _pageNumber;
        private int _pageSize;
        private string _costos;
     
        public int pageNumber { get => _pageNumber; set => _pageNumber = value; } 
        public int pageSize { get => _pageSize; set => _pageSize = value; }
        public string costos { get => _costos; set => _costos = value; }

        public ResponseGetList(int pageNumber = 1, int pageSize = 10,string costos = "N")
        {
           
            _pageNumber = pageNumber;
            _pageSize = pageSize;
            _costos = costos;

        }
    }


    public class ResponseGetResult
    {
        private string _expresion;
        private int _pageNumber;
        private int _pageSize;

        public string expresion { get => _expresion; set => _expresion = value; }
        public int pageNumber { get => _pageNumber; set => _pageNumber = value; }
        public int pageSize { get => _pageSize; set => _pageSize = value; }

        public ResponseGetResult(string expresion = "", int pageNumber = 1, int pageSize = 10)
        {
            _expresion = expresion;
            _pageNumber = pageNumber;
            _pageSize = pageSize;

        }
    }

    
    public class HijoProductos : GESI.ERP.Core.BO.cProducto
    {
       /* [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override short RubroID { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override short SubRubroID { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override short SubSubRubroID { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override short FiltroArticulos1ID { get => base.FiltroArticulos1ID; set => base.FiltroArticulos1ID = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override short FiltroArticulos2ID { get => base.FiltroArticulos2ID; set => base.FiltroArticulos2ID = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override short FiltroArticulos3ID { get => base.FiltroArticulos3ID; set => base.FiltroArticulos3ID = value; }
       */
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }
        
        public List<int> ListaDeCategorias { get => _CodigosCategorias; set => _CodigosCategorias = value; }

        private List<int> _CodigosCategorias;

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override List<cCategoriaXProducto> Categorias { get => base.Categorias; set => base.Categorias = value; }

      

        public HijoProductos(GESI.ERP.Core.BO.cProducto padre)
        {
            EmpresaID = padre.EmpresaID;
            ProductoID = padre.ProductoID;
            Descripcion = padre.Descripcion;
            DescripcionExtendida = padre.DescripcionExtendida;
            AlicuotaIVA = padre.AlicuotaIVA;
            RubroID = padre.RubroID;
            SubRubroID = padre.SubRubroID;
            SubSubRubroID = padre.SubSubRubroID;
            FiltroArticulos1ID = padre.FiltroArticulos1ID;
            FiltroArticulos2ID = padre.FiltroArticulos2ID;
            FiltroArticulos3ID = padre.FiltroArticulos3ID;
            Unidad2XUnidad1 = padre.Unidad2XUnidad1;
            Unidad2XUnidad1Confirmar = padre.Unidad2XUnidad1Confirmar;
            CostosProveedores = padre.CostosProveedores;
            Imagenes = padre.Imagenes;
            Precios = padre.Precios;
          //  Categorias = padre.Categorias;
            Estado = padre.Estado;
            GrupoArtID = padre.GrupoArtID;
            ListaDeCategorias = new List<int>();            
            Existencias = padre.Existencias;
            Largo = padre.Largo;
            Ancho = padre.Ancho;
            Alto = padre.Alto;
            Peso = padre.Peso;
            
        }

       


        public HijoProductos()
        { }
            

    }

  

    public class ExistenciaHijas : GESI.ERP.Core.BO.cExistenciaProducto
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override uint EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public ExistenciaHijas(GESI.ERP.Core.BO.cExistenciaProducto padre)
        {
            ProductoID = padre.ProductoID;
            AlmacenID = padre.AlmacenID;
            FechaExistencia = padre.FechaExistencia;
            Unidad1 = padre.Unidad1;
            Unidad2 = padre.Unidad2;
            Unidad1AcopioComprometido = padre.Unidad1AcopioComprometido;
            Unidad2AcopioComprometido = padre.Unidad2AcopioComprometido;
            Unidad1AcopioNoComprometido = padre.Unidad1AcopioNoComprometido;
            Unidad2AcopioNoComprometido = padre.Unidad2AcopioNoComprometido;
            Unidad1NPEPendientes = padre.Unidad1NPEPendientes;
            Unidad2NPEPendientes = padre.Unidad2NPEPendientes;
            Unidad1Disponible = padre.Unidad1Disponible;
            Unidad2Disponible = padre.Unidad2Disponible;
        }

    }


    public class ResultProducts
    {
        private string _productoID;
        private string _descripcion;
        private int _alicuotaIVA;
        private decimal _unidad2XUnidad1;
        private bool _unidad2XUnidad1Confirmar;
        private string _descripcionextendida;

        public string productoID { get => _productoID; set => _productoID = value; }
        public string descripcion { get => _descripcion; set => _descripcion = value; }
        public int alicuotaIVA { get => _alicuotaIVA; set => _alicuotaIVA = value; }
        public decimal unidad2XUnidad1 { get => _unidad2XUnidad1; set => _unidad2XUnidad1 = value; }
        public bool unidad2XUnidad1Confirmar { get => _unidad2XUnidad1Confirmar; set => _unidad2XUnidad1Confirmar = value; }
        public string descripcionExtendida { get => _descripcionextendida; set => _descripcionextendida = value; }
    }

}
