using API_Maestros_Core.BLL;
using API_Maestros_Core.Models;
using GESI.ERP.Core.BLL;
using GESI.ERP.Core.BO;
using GESI.GESI.BO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Net;
using static API_Maestros_Core.BLL.CategoriasMgr;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        #region Variables
        public static GESI.CORE.BLL.SessionMgr _SessionMgr;
        public static List<GESI.CORE.BO.Verscom2k.HabilitacionesAPI> moHabilitacionesAPI;
        public static string mostrTipoAPI = "LEER_MAESTROS";
        public static string strUsuarioID = "";
        public static bool HabilitadoPorToken = false;
        public static string TokenEnviado = "";
        public static string strProtocolo = "";
        public static List<TipoDeError> lstTipoDeError = APIHelper.LlenarTiposDeError();
        public TipoDeError oTipoError;
        #endregion

        // GET: api/<CategoriasController>
        /// <summary>
        /// Devuelve la lista de categorias de Productos
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {

            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


          //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
          //  GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            RespuestaConCategorias oRespuesta = new RespuestaConCategorias();
            #endregion

            if (!ModelState.IsValid)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cModeloNoValido);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cModeloNoValido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID,APIHelper.CategoriasGetList);
                oRespuesta.success = false;
                return StatusCode((int)APIHelper.cCodigosError.cModeloNoValido, oRespuesta);
            }
            else
            {

                try
                {

                    if (!HabilitadoPorToken)
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetList);
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
                                Paginacion oPaginacion = new Paginacion();
                                CategoriasMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                                oRespuesta.success = true;
                                oRespuesta.error = new Error();
                                oRespuesta.categoriasProductos = new List<CategoriaHija>();
                                List<CategoriaHija> lstCategorias = CategoriasMgr.GetList();
                                oRespuesta.categoriasProductos.AddRange(CategoriasMgr.GetList());


                                oRespuesta.categoriasProductos = lstCategorias.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                                oPaginacion.totalElementos = oRespuesta.categoriasProductos.Count;

                                oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                                oPaginacion.paginaActual = pageNumber;
                                oPaginacion.tamañoPagina = pageSize;
                                oRespuesta.paginacion = oPaginacion;
                                return Ok(oRespuesta);
                            }
                            else
                            {
                                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cTokenInvalido);
                                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cTokenInvalido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetList);
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);
                            }
                        }
                        else
                        {
                            oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                            oRespuesta.success = false;
                            return BadRequest(oRespuesta);
                        }
                    }
                }
                catch (Exception ex)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Descripcion: " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetList);
                    oRespuesta.success = false;                                       
                    return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                }
            }

        }

        // GET api/<CategoriasController>/5
        [HttpGet("GetItem/{categoriaID}")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult Get(string categoriaID)
        {
            #region ConnectionsStrings
            RespuestaCategoriasGetItem oRespuesta = new RespuestaCategoriasGetItem(); 
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


           // SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
           // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion


            if (!ModelState.IsValid)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cModeloNoValido);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cModeloNoValido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);                
                oRespuesta.success = false;
                return StatusCode((int)APIHelper.cCodigosError.cModeloNoValido, oRespuesta);
            
            }
            else
            {
                try
                {
                    if (!HabilitadoPorToken)
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);                       
                        oRespuesta.success = false;
                        return Unauthorized(oRespuesta);
                    }
                    else
                    {
                        string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                        if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                        {


                            if (categoriaID != null)
                            {
                                if (categoriaID.Length > 0)
                                {
                                    APISessionManager MiSessionMgrAPI = APIHelper.SetearMgrAPI(strUsuarioID);

                                    if (MiSessionMgrAPI.Habilitado)
                                    {

                                        CategoriasMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;

                                        Paginacion oPaginacion = new Paginacion();
                                        oPaginacion.totalElementos = 1;
                                        oPaginacion.totalPaginas = 1;
                                        oPaginacion.paginaActual = 1;
                                        oPaginacion.tamañoPagina = 1;
                                        oRespuesta.paginacion = oPaginacion;
                                        oRespuesta.error = new Error();
                                        oRespuesta.success = true;
                                        oRespuesta.categoriaProducto = CategoriasMgr.GetItem(categoriaID);
                                        Logger.LoguearErrores("Exitoso para el codigo " + categoriaID, "I", _SessionMgr.UsuarioID, APIHelper.CategoriasGetItem);
                                        return Ok(oRespuesta);
                                    }
                                    else
                                    {
                                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                                        oRespuesta.success = false;
                                        return Unauthorized(oRespuesta);
                                    }
                                }
                                else
                                {

                                    Logger.LoguearErrores("No se encontro categoria a buscar", "I", strUsuarioID, APIHelper.CategoriasGetItem);
                                    return NoContent();
                                }
                            }
                            else
                            {
                                Logger.LoguearErrores("No se encontro categoria a buscar", "I", strUsuarioID, APIHelper.CategoriasGetItem);
                                return NoContent();
                            }
                        }
                        else
                        {
                            oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetItem);
                            oRespuesta.success = false;
                            return BadRequest(oRespuesta);
                        }
                    }
                }
                catch (Exception ex)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Descripcion: " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);                   
                    oRespuesta.success = false;
                    return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                }
            }
        }

        [HttpGet("RubrosGetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult RubrosGetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
           
           // SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            RespuestaConRubros oRespuesta = new RespuestaConRubros();
            #endregion
            try
            {
                if(!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cModeloNoValido);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cModeloNoValido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = APIHelper.SetearMgrAPI(strUsuarioID);
                        CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = CategoriasMgr.RubrosGetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError+" Error al devolver Rubros. "+ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }


        [HttpGet("SubRubrosGetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult SubRubrosGetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

         //   SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
         //   GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            RespuestaConSubRubros oRespuesta = new RespuestaConSubRubros();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = APIHelper.SetearMgrAPI(strUsuarioID);
                        CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = CategoriasMgr.SubRubrosGetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.DescripcionError, strUsuarioID, APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" SubRubros. "+ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }

        [HttpGet("SubSubRubrosGetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult SubSubRubrosGetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

          //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
           // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            RespuestaConSubSubRubros oRespuesta = new RespuestaConSubSubRubros();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cModeloNoValido);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cModeloNoValido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {

                        APISessionManager MiApiSessionMgr = APIHelper.SetearMgrAPI(strUsuarioID);
                        CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = CategoriasMgr.SubSubRubrosGetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Error al devolver SubSubRubros. " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }


        [HttpGet("Filtro1GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult Filtro1GetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

       //     SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
       //     GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            RespuestaConFiltroArticulos1 oRespuesta = new RespuestaConFiltroArticulos1();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = APIHelper.SetearMgrAPI(strUsuarioID);
                        CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = CategoriasMgr.FiltroArticulos1GetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Error al devolver Filtro1. Descripcion: "+ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }


        [HttpGet("Filtro2GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult Filtro2GetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

          //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
          //  GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            RespuestaConFiltroArticulos2 oRespuesta = new RespuestaConFiltroArticulos2();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = APIHelper.SetearMgrAPI(strUsuarioID);
                        CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = CategoriasMgr.FiltroArticulos2GetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Error al devolver Filtro2 " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }

        [HttpGet("Filtro3GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult Filtro3GetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

          //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
           // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            RespuestaConFiltroArticulos3 oRespuesta = new RespuestaConFiltroArticulos3();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = APIHelper.SetearMgrAPI(strUsuarioID);
                        CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = CategoriasMgr.FiltroArticulos3GetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Error al devolver Filtro3. " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }


    }

    public class ResponseCategoriasGetItem
    {
        private String _CategoriaID;

        public String CategoriaID
        {
            get { return _CategoriaID; }
            set { _CategoriaID = value; }            
        }

        public ResponseCategoriasGetItem()
        {
            _CategoriaID = "";
        }

    }

    public class RubroHijo : GESI.ERP.Core.BO.v2kRubro
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public RubroHijo(v2kRubro oRubro)
        {
            RubroID = oRubro.RubroID;
            CategoriaID = oRubro.CategoriaID;
            Descripcion = oRubro.Descripcion;
        }

    }


    public class SubRubroHijo : GESI.ERP.Core.BO.v2kSubRubro
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public SubRubroHijo(v2kSubRubro oRubro)
        {
            RubroID = oRubro.RubroID;
            SubRubroID = oRubro.SubRubroID;
            CategoriaID = oRubro.CategoriaID;
            Descripcion = oRubro.Descripcion;
        }

    }

    public class SubSubRubroHijo : GESI.ERP.Core.BO.v2kSubSubRubro
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public SubSubRubroHijo(v2kSubSubRubro oRubro)
        {
            RubroID = oRubro.RubroID;
            SubRubroID = oRubro.SubRubroID;
            SubSubRubroID = oRubro.SubSubRubroID;
            CategoriaID = oRubro.CategoriaID;
            Descripcion = oRubro.Descripcion;
        }

    }


    public class Filtro1Hijo : GESI.ERP.Core.BO.v2kFiltroArticulos1ID
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public Filtro1Hijo(v2kFiltroArticulos1ID oRubro)
        {
            FiltroArticulos1ID = oRubro.FiltroArticulos1ID;
            CategoriaID = oRubro.CategoriaID;
            Descripcion = oRubro.Descripcion;
        }

    }


    public class Filtro2Hijo : GESI.ERP.Core.BO.v2kFiltroArticulos2ID
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public Filtro2Hijo(v2kFiltroArticulos2ID oRubro)
        {
            FiltroArticulos2ID = oRubro.FiltroArticulos2ID;
            CategoriaID = oRubro.CategoriaID;
            Descripcion = oRubro.Descripcion;
        }

    }

    public class Filtro3Hijo : GESI.ERP.Core.BO.v2kFiltroArticulos3ID
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public Filtro3Hijo(v2kFiltroArticulos3ID oRubro)
        {
            FiltroArticulos3ID = oRubro.FiltroArticulos3ID;
            CategoriaID = oRubro.CategoriaID;
            Descripcion = oRubro.Descripcion;
        }

    }

}
