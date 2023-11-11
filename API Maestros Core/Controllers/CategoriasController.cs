using API_Maestros_Core.BLL;
using API_Maestros_Core.Models;
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


            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            RespuestaConCategorias oRespuesta = new RespuestaConCategorias();
            #endregion

            if (!ModelState.IsValid)
            {
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cModeloNoValido, "Modelo no válido", "E", strUsuarioID,APIHelper.CategoriasGetList);
                oRespuesta.success = false;
                return StatusCode((int)APIHelper.cCodigosError.cModeloNoValido, oRespuesta);
            }
            else
            {

                try
                {

                    if (!HabilitadoPorToken)
                    {
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, "No esta autorizado a acceder al servicio. No se encontro el token del usuario", "E", strUsuarioID, APIHelper.CategoriasGetList);
                        oRespuesta.success = false;                        
                        return Unauthorized(oRespuesta);                        
                    }
                    else
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
                            oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cTokenInvalido, "No esta autorizado a acceder al recurso", "E", strUsuarioID, APIHelper.CategoriasGetList);
                            oRespuesta.success = false;                            
                            return Unauthorized(oRespuesta);
                        }
                    }
                }
                catch (Exception ex)
                {
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, "Error interno de la aplicacion. Descripcion: " + ex.Message, "E", strUsuarioID, APIHelper.CategoriasGetList);
                    oRespuesta.success = false;                                       
                    return StatusCode(500, oRespuesta);
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


            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion


            if (!ModelState.IsValid)
            {
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cModeloNoValido, "Modelo no valido", "E", strUsuarioID, APIHelper.CategoriasGetItem);                
                oRespuesta.success = false;
                return StatusCode((int)APIHelper.cCodigosError.cModeloNoValido, oRespuesta);
            
            }
            else
            {
                try
                {
                    if (!HabilitadoPorToken)
                    {
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cModeloNoValido, "Modelo no valido", "E", strUsuarioID, APIHelper.CategoriasGetItem);                       
                        oRespuesta.success = false;
                        return Unauthorized(oRespuesta);
                    }
                    else
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
                                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, "No esta autorizado a acceder al servicio. No se encontro el token del usuario", "E", strUsuarioID, APIHelper.CategoriasGetItem);                                   
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
                }
                catch (Exception ex)
                {
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, "Error interno de la aplicacion. Descripcion: " + ex.Message, "E", strUsuarioID, APIHelper.CategoriasGetItem);                   
                    oRespuesta.success = false;
                    return StatusCode(500, oRespuesta);
                }
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

   
        

}
