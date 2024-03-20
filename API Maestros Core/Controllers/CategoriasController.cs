using API_Maestros_Core.BLL;
using GESI.CORE.API.BO;
using GESI.GESI.BO;
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
        public static List<GESI.CORE.API.BO.TipoDeError> lstTipoDeError = GESI.CORE.API.BLL.APIHelper.LlenarTiposDeError();
        public GESI.CORE.API.BO.TipoDeError oTipoError;
        #endregion

        // GET: api/<CategoriasController>
        /// <summary>
        /// Devuelve la lista de categorias de Productos
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(GESI.CORE.API.BO.ResponseCategorias))]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {

            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


            //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //  GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            GESI.CORE.API.BO.ResponseCategorias oRespuesta = new GESI.CORE.API.BO.ResponseCategorias();
            #endregion

            if (!ModelState.IsValid)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID,GESI.CORE.API.BLL.APIHelper.CategoriasGetList);
                oRespuesta.success = false;
                return StatusCode((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido, oRespuesta);
            }
            else
            {

                try
                {

                    if (!HabilitadoPorToken)
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetList);
                        oRespuesta.success = false;                        
                        return Unauthorized(oRespuesta);                        
                    }
                    else
                    {
                        string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                        if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                        {

                            GESI.CORE.API.BO.APISessionManager MiSessionMgrAPI = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);

                            if (MiSessionMgrAPI.Habilitado)
                            {
                                GESI.CORE.API.BO.Paginacion oPaginacion = new GESI.CORE.API.BO.Paginacion();
                                GESI.CORE.API.BLL.CategoriasMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;
                                oRespuesta.success = true;
                                oRespuesta.error = new GESI.CORE.API.BO.Error();
                                oRespuesta.categoriasProductos = new List<GESI.CORE.API.BO.Categoria>();
                                List<GESI.CORE.API.BO.Categoria> lstCategorias = GESI.CORE.API.BLL.CategoriasMgr.GetList();
                                oRespuesta.categoriasProductos.AddRange(GESI.CORE.API.BLL.CategoriasMgr.GetList());


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
                                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cTokenInvalido);
                                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cTokenInvalido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetList);
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);
                            }
                        }
                        else
                        {
                            oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                            oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                            oRespuesta.success = false;
                            return BadRequest(oRespuesta);
                        }
                    }
                }
                catch (Exception ex)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Descripcion: " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetList);
                    oRespuesta.success = false;                                       
                    return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                }
            }

        }

        // GET api/<CategoriasController>/5
        [HttpGet("GetItem")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseCategoria))]
        public IActionResult Get(string categoriaID)
        {
            #region ConnectionsStrings
            ResponseCategoria oRespuesta = new ResponseCategoria(); 
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


           // SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
           // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion


            if (!ModelState.IsValid)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);                
                oRespuesta.success = false;
                return StatusCode((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido, oRespuesta);
            
            }
            else
            {
                try
                {
                    if (!HabilitadoPorToken)
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);                       
                        oRespuesta.success = false;
                        return Unauthorized(oRespuesta);
                    }
                    else
                    {
                        string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                        if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                        {


                            if (categoriaID != null)
                            {
                                if (categoriaID.Length > 0)
                                {
                                    APISessionManager MiSessionMgrAPI = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);

                                    if (MiSessionMgrAPI.Habilitado)
                                    {

                                        GESI.CORE.API.BLL.CategoriasMgr._SessionMgr = MiSessionMgrAPI.SessionMgr;

                                        Paginacion oPaginacion = new Paginacion();
                                        oPaginacion.totalElementos = 1;
                                        oPaginacion.totalPaginas = 1;
                                        oPaginacion.paginaActual = 1;
                                        oPaginacion.tamañoPagina = 1;
                                        oRespuesta.paginacion = oPaginacion;
                                        oRespuesta.error = new Error();
                                        oRespuesta.success = true;
                                        oRespuesta.categoriaProducto = GESI.CORE.API.BLL.CategoriasMgr.GetItem(categoriaID);
                                        GESI.CORE.API.BLL.Logger.LoguearErrores("Exitoso para el codigo " + categoriaID, "I", _SessionMgr.UsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                                        return Ok(oRespuesta);
                                    }
                                    else
                                    {
                                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                                        oRespuesta.success = false;
                                        return Unauthorized(oRespuesta);
                                    }
                                }
                                else
                                {

                                    GESI.CORE.API.BLL.Logger.LoguearErrores("No se encontro categoria a buscar", "I", strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                                    return NoContent();
                                }
                            }
                            else
                            {
                                GESI.CORE.API.BLL.Logger.LoguearErrores("No se encontro categoria a buscar", "I", strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                                return NoContent();
                            }
                        }
                        else
                        {
                            oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                            oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetItem);
                            oRespuesta.success = false;
                            return BadRequest(oRespuesta);
                        }
                    }
                }
                catch (Exception ex)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Descripcion: " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);                   
                    oRespuesta.success = false;
                    return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
                }
            }
        }

        [HttpGet("RubrosGetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseRubros))]
        public IActionResult RubrosGetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            ResponseRubros oRespuesta = new ResponseRubros();
            #endregion
            try
            {
                if(!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);
                        GESI.CORE.API.BLL.CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = GESI.CORE.API.BLL.CategoriasMgr.RubrosGetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError+" Error al devolver Rubros. "+ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }


        [HttpGet("SubRubrosGetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseSubRubros))]
        public IActionResult SubRubrosGetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            //   SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //   GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            ResponseSubRubros oRespuesta = new ResponseSubRubros();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);
                        GESI.CORE.API.BLL.CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = GESI.CORE.API.BLL.CategoriasMgr.SubRubrosGetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.DescripcionError, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" SubRubros. "+ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }

        [HttpGet("SubSubRubrosGetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseSubSubRubros))]
        public IActionResult SubSubRubrosGetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            ResponseSubSubRubros oRespuesta = new ResponseSubSubRubros();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cModeloNoValido, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {

                        APISessionManager MiApiSessionMgr = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);
                        GESI.CORE.API.BLL.CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = GESI.CORE.API.BLL.CategoriasMgr.SubSubRubrosGetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Error al devolver SubSubRubros. " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }


        [HttpGet("Filtro1GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseFiltro1))]
        public IActionResult Filtro1GetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            //     SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //     GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            ResponseFiltro1 oRespuesta = new ResponseFiltro1();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);
                        GESI.CORE.API.BLL.CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = GESI.CORE.API.BLL.CategoriasMgr.FiltroArticulos1GetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Error al devolver Filtro1. Descripcion: "+ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }


        [HttpGet("Filtro2GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseFiltro2))]
        public IActionResult Filtro2GetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            //  GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            ResponseFiltro2 oRespuesta = new ResponseFiltro2();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);
                        GESI.CORE.API.BLL.CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = GESI.CORE.API.BLL.CategoriasMgr.FiltroArticulos2GetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Error al devolver Filtro2 " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }

        [HttpGet("Filtro3GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(ResponseFiltro3))]
        public IActionResult Filtro3GetList(int pageNumber = 1, int pageSize = 10)
        {
            #region ConnectionsStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            //  SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            ResponseFiltro3 oRespuesta = new ResponseFiltro3();
            #endregion
            try
            {
                if (!HabilitadoPorToken)
                {
                    oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken);
                    oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                    if (GESI.CORE.API.BLL.APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                    {
                        APISessionManager MiApiSessionMgr = GESI.CORE.API.BLL.APIHelper.SetearMgrAPI(strUsuarioID);
                        GESI.CORE.API.BLL.CategoriasMgr._SessionMgr = MiApiSessionMgr.SessionMgr;
                        oRespuesta = GESI.CORE.API.BLL.CategoriasMgr.FiltroArticulos3GetList(pageNumber, pageSize);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto);
                        oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.ProductosGetList);
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                oTipoError = lstTipoDeError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = GESI.CORE.API.BLL.APIHelper.DevolverErrorAPI((int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Error al devolver Filtro3. " + ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, GESI.CORE.API.BLL.APIHelper.CategoriasGetItem);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError, oRespuesta);
            }
        }


    }

  

    

}
