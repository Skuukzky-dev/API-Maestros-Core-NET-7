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
        #endregion

        // GET: api/<CategoriasController>
        /// <summary>
        /// Devuelve la lista de categorias de Productos
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult Get([FromBody]ResponseGetList oResponse = null)
        {

            if (oResponse == null)
                oResponse = new ResponseGetList();

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            RespuestaConCategorias oRespuesta = new RespuestaConCategorias();

            if (!ModelState.IsValid)
            {
                oRespuesta.success = false;
                oRespuesta.error = new Error();
                oRespuesta.error.code = 4151;
                oRespuesta.error.message = "Modelo no válido";
                Logger.LoguearErrores("Modelo no válido");
                this.StatusCode((int)HttpStatusCode.Unauthorized);
                return StatusCode(415, oRespuesta);
            }
            else
            {

                try
                {

                    moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                    _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                    bool Habilitado = false;
                    if (!HabilitadoPorToken)
                    {
                        oRespuesta.success = false;
                        oRespuesta.error = new Error();
                        oRespuesta.error.code = 4012;
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
                            Paginacion oPaginacion = new Paginacion();
                            CategoriasMgr._SessionMgr = _SessionMgr;
                            oRespuesta.success = true;
                            oRespuesta.error = new Error();
                            oRespuesta.CategoriasProductos = new List<GESI.ERP.Core.BO.cCategoriaDeProducto>();
                            List<GESI.ERP.Core.BO.cCategoriaDeProducto> lstCategorias = CategoriasMgr.GetList();
                            oRespuesta.CategoriasProductos.AddRange(CategoriasMgr.GetList());
                           

                            oRespuesta.CategoriasProductos = lstCategorias.Skip((oResponse.pageNumber - 1) * oResponse.pageSize).Take(oResponse.pageSize).ToList();
                            oPaginacion.totalElementos = oRespuesta.CategoriasProductos.Count;
                            // 
                            oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / oResponse.pageSize);
                            oPaginacion.paginaActual = oResponse.pageNumber;
                            oPaginacion.tamañoPagina = oResponse.pageSize;
                            oRespuesta.paginacion = oPaginacion;

                            this.StatusCode((int)HttpStatusCode.OK);
                            return Ok(oRespuesta);
                        }
                        else
                        {
                            oRespuesta.success = false;
                            oRespuesta.error = new Error();
                            oRespuesta.error.code = 4012;
                            oRespuesta.error.message = "No esta autorizado a acceder al recurso";
                            Logger.LoguearErrores("No esta autorizado a acceder al recurso");
                            this.StatusCode((int)HttpStatusCode.Unauthorized);
                            return Unauthorized(oRespuesta);
                        }
                    }
                }
                catch (Exception ex)
                {
                    oRespuesta.success = false;
                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 5001;
                    oRespuesta.error.message = "Error interno de la aplicacion. Descripcion: " + ex.Message;
                    Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message);
                    this.StatusCode((int)HttpStatusCode.InternalServerError);
                    return Unauthorized(oRespuesta);
                }
            }

        }

        // GET api/<CategoriasController>/5
        [HttpGet("GetItem")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConCategorias))]
        public IActionResult Get([FromBody] ResponseCategoriasGetItem oRequestRecibido = null)
        {
            HttpResponseMessage respuesta = new HttpResponseMessage();
            RespuestaConCategorias oRespuesta = new RespuestaConCategorias();

            if (oRequestRecibido == null)
                oRequestRecibido = new ResponseCategoriasGetItem();

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;

            if (!ModelState.IsValid)
            {
                respuesta.StatusCode = HttpStatusCode.BadRequest;
                // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                oRespuesta.error = new Error();
                oRespuesta.error.code = 4151;
                oRespuesta.error.message = "Modelo no valido";
                Logger.LoguearErrores("Modelo no valido");
                oRespuesta.success = false;
                string json = JsonConvert.SerializeObject(oRespuesta);
                respuesta.Content = new StringContent(json);
                this.StatusCode((int)HttpStatusCode.BadRequest);
                return StatusCode(415, oRespuesta);
                //return Unauthorized(oRespuesta);
            }
            else
            {

                try
                {
                    if (!HabilitadoPorToken)
                    {
                        respuesta.StatusCode = HttpStatusCode.BadRequest;
                        // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                        oRespuesta.error = new Error();
                        oRespuesta.error.code = 4012;
                        oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                        Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario");
                        oRespuesta.success = false;
                        string json = JsonConvert.SerializeObject(oRespuesta);
                        respuesta.Content = new StringContent(json);
                        this.StatusCode((int)HttpStatusCode.BadRequest);
                        return Unauthorized(oRespuesta);
                    }
                    else
                    {
                        moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                        string strCanalesDeVenta = null;
                        _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                        bool Habilitado = false;
                        if (oRequestRecibido.CategoriaID != null)
                        {
                            if (oRequestRecibido.CategoriaID.Length > 0)
                            {
                                foreach (GESI.CORE.BO.Verscom2k.HabilitacionesAPI oHabilitacionAPI in moHabilitacionesAPI)
                                {
                                    if (oHabilitacionAPI.TipoDeAPI.Equals(mostrTipoAPI))
                                    {
                                        _SessionMgr.EmpresaID = oHabilitacionAPI.EmpresaID;
                                        _SessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                                        _SessionMgr.SucursalID = oHabilitacionAPI.SucursalID;
                                        _SessionMgr.EntidadID = 1;
                                        strCanalesDeVenta = oHabilitacionAPI.CanalesDeVenta;
                                        Habilitado = true;
                                    }
                                }

                                if (Habilitado)
                                {

                                    CategoriasMgr._SessionMgr = _SessionMgr;

                                    Paginacion oPaginacion = new Paginacion();
                                    oPaginacion.totalElementos = 1;
                                    oPaginacion.totalPaginas = 1;
                                    oPaginacion.paginaActual = 1;
                                    oPaginacion.tamañoPagina = 1;
                                    oRespuesta.paginacion = oPaginacion;

                                    respuesta.StatusCode = HttpStatusCode.OK;
                                    oRespuesta.error = new Error();
                                    oRespuesta.success = true;
                                    oRespuesta.CategoriasProductos = new List<GESI.ERP.Core.BO.cCategoriaDeProducto>();
                                    oRespuesta.CategoriasProductos.AddRange(CategoriasMgr.GetItem(oRequestRecibido.CategoriaID));
                                    string json = JsonConvert.SerializeObject(oRespuesta);
                                    respuesta.Content = new StringContent(json);
                                    Logger.LoguearErrores("Exitoso para el codigo " + oRequestRecibido.CategoriaID);
                                    this.StatusCode((int)HttpStatusCode.OK);
                                    return Ok(oRespuesta);
                                }
                                else
                                {
                                    respuesta.StatusCode = HttpStatusCode.BadRequest;
                                    // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                                    oRespuesta.error = new Error();
                                    oRespuesta.error.code = 4012;
                                    oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                                    Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario");
                                    oRespuesta.success = false;
                                    string json = JsonConvert.SerializeObject(oRespuesta);
                                    respuesta.Content = new StringContent(json);
                                    this.StatusCode((int)HttpStatusCode.BadRequest);
                                    return Unauthorized(oRespuesta);
                                }
                            }
                            else
                            {
                                respuesta.StatusCode = HttpStatusCode.BadRequest;
                                //    RespuestaConProductos oRespuesta = new RespuestaConProductos();
                                oRespuesta.error = new Error();
                                oRespuesta.error.code = 2041;
                                oRespuesta.error.message = "No se encontro expresion a buscar";
                                Logger.LoguearErrores("No se encontro expresion a buscar");
                                oRespuesta.success = false;
                                string json = JsonConvert.SerializeObject(oRespuesta);
                                respuesta.Content = new StringContent(json);
                                this.StatusCode((int)HttpStatusCode.NotFound);
                                return StatusCode(204, oRespuesta);
                            }
                        }
                        else
                        {
                            respuesta.StatusCode = HttpStatusCode.BadRequest;
                            //RespuestaConProductos oRespuesta = new RespuestaConProductos();
                            oRespuesta.error = new Error();
                            oRespuesta.error.code = 4041;
                            oRespuesta.error.message = "No se encontro expresion a buscar";
                            Logger.LoguearErrores("No se encontro expresion a buscar");
                            oRespuesta.success = false;
                            string json = JsonConvert.SerializeObject(oRespuesta);
                            respuesta.Content = new StringContent(json);
                            this.StatusCode((int)HttpStatusCode.NotFound);
                            return StatusCode(204, oRespuesta);
                        }
                    }
                }
                catch (Exception ex)
                {
                    respuesta.StatusCode = HttpStatusCode.BadRequest;
                    //RespuestaConProductos oRespuesta = new RespuestaConProductos();
                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 5001;
                    oRespuesta.error.message = "Error interno de la aplicacion. Descripcion: " + ex.Message;
                    Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message);
                    oRespuesta.success = false;
                    string json = JsonConvert.SerializeObject(oRespuesta);
                    respuesta.Content = new StringContent(json);
                    this.StatusCode((int)HttpStatusCode.InternalServerError);
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
