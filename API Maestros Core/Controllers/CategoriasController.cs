using API_Maestros_Core.BLL;
using API_Maestros_Core.Models;
using GESI.GESI.BO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;

            RespuestaConCategorias oRespuesta = new RespuestaConCategorias();
            try
            {

                //GESI.GESI.BO.ListaCanalesDeVenta lstCanales = new GESI.GESI.BO.ListaCanalesDeVenta();
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
                        oPaginacion.totalElementos = oRespuesta.CategoriasProductos.Count;
                       
                        oRespuesta.CategoriasProductos = lstCategorias.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                         // 
                        oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                        oPaginacion.paginaActual = pageNumber;
                        oPaginacion.tamañoPagina = pageSize;
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

        // GET api/<CategoriasController>/5
        [HttpGet("GetItem/{CategoriaID}")]
        public IActionResult Get(String CategoriaID)
        {
            HttpResponseMessage respuesta = new HttpResponseMessage();
            RespuestaConCategorias oRespuesta = new RespuestaConCategorias();

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
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
                    if (CategoriaID != null)
                    {
                        if (CategoriaID.Length > 0)
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
                                oRespuesta.CategoriasProductos.AddRange(CategoriasMgr.GetItem(CategoriaID));
                                string json = JsonConvert.SerializeObject(oRespuesta);
                                respuesta.Content = new StringContent(json);
                                Logger.LoguearErrores("Exitoso para el codigo " + CategoriaID);
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

     /*   // POST api/<CategoriasController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CategoriasController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CategoriasController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
