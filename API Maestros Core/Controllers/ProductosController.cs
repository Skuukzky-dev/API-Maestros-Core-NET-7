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
        #endregion
        // GET: api/<ProductosController>
        /// <summary>
        /// Devuelve la lista de Resultados de Busqueda de una Expresion
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConProductosHijos))]

        public IActionResult Get(string id = "", int pageNumber = 1, int pageSize = 10)
        {
            HttpResponseMessage respuesta = new HttpResponseMessage();
            RespuestaConProductosHijos oRespuesta = new RespuestaConProductosHijos();

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
                    this.StatusCode((int)HttpStatusCode.Unauthorized);
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                    _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                    bool Habilitado = false;
                    if (id != null)
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
                                ProductosMgr._SessionMgr = _SessionMgr;
                                List<HijoProductos> lstProductos = ProductosMgr.GetList(id, pageNumber, pageSize);
                
                                Paginacion oPaginacion = new Paginacion();
                                oPaginacion.totalElementos = lstProductos.Count;
                                oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                                oPaginacion.paginaActual = pageNumber;
                                oPaginacion.tamañoPagina = pageSize;
                                oRespuesta.paginacion = oPaginacion;
                        
                            
                                respuesta.StatusCode = HttpStatusCode.OK;
                                oRespuesta.error = new Error();
                                oRespuesta.success = true;
                                oRespuesta.Productos = lstProductos; 

                                string json1 = JsonConvert.SerializeObject(oRespuesta, Formatting.Indented);
        
                                Logger.LoguearErrores("Respuesta exitosa para la expresion "+id);
                          
                                ContentResult Content = new ContentResult();
                                Content.Content = json1;
                                Content.StatusCode = (int)HttpStatusCode.OK;
                                Content.ContentType = "application/json";
                                return Content;
              
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
                                this.StatusCode((int)HttpStatusCode.Unauthorized);
                                return Unauthorized(oRespuesta);
                            }
                        
                    }
                    else
                    {
                        respuesta.StatusCode = HttpStatusCode.BadRequest;
                        //RespuestaConProductos oRespuesta = new RespuestaConProductos();
                        oRespuesta.error = new Error();
                        oRespuesta.error.code = 2041;
                        oRespuesta.error.message = "No se encontro expresion a buscar";
                        Logger.LoguearErrores("No se encontro expresion a buscar");
                        oRespuesta.success = false;
                        string json = JsonConvert.SerializeObject(oRespuesta);
                        respuesta.Content = new StringContent(json);
                        this.StatusCode((int)HttpStatusCode.NotFound);
                        return StatusCode(204,oRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.StatusCode = HttpStatusCode.BadRequest;
               // RespuestaConProductos oRespuesta = new RespuestaConProductos();
                oRespuesta.error = new Error();
                oRespuesta.error.code = 5001;
                oRespuesta.error.message = "error interno de la aplicacion. Descripcion: " + ex.Message;
                Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message);
                oRespuesta.success = false;
                string json = JsonConvert.SerializeObject(oRespuesta);
                respuesta.Content = new StringContent(json);
                this.StatusCode((int)HttpStatusCode.InternalServerError);
                return StatusCode(500,oRespuesta);
            }
         
        }

        // GET api/<ProductosController>/5
        /// <summary>
        /// Devuelve un producto con sus precios e imagenes
        /// </summary>
        /// <param name="ProductoID"></param>
        /// <param name="CanalDeVentaID"></param>
        /// <returns></returns>
        [HttpGet("GetItem/{ProductoID}")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConProductosHijos))]
        public IActionResult Get(string ProductoID,int CanalDeVentaID = 0)
        {
            HttpResponseMessage respuesta = new HttpResponseMessage();
            RespuestaConProductosHijos oRespuesta = new RespuestaConProductosHijos();

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
                    if (ProductoID != null)
                    {
                        if (ProductoID.Length > 0)
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

                                ProductosMgr._SessionMgr = _SessionMgr;
                                List<HijoProductos> lstProductos = ProductosMgr.GetItem(ProductoID, strCanalesDeVenta,CanalDeVentaID);

                                Paginacion oPaginacion = new Paginacion();
                                oPaginacion.totalElementos = 1;
                                oPaginacion.totalPaginas = 1;
                                oPaginacion.paginaActual = 1;
                                oPaginacion.tamañoPagina = 1;
                                oRespuesta.paginacion = oPaginacion;

                                respuesta.StatusCode = HttpStatusCode.OK;
                                oRespuesta.error = new Error();
                                oRespuesta.success = true;
                                oRespuesta.Productos = new List<HijoProductos>();
                                oRespuesta.Productos.AddRange(lstProductos);
                                string json = JsonConvert.SerializeObject(oRespuesta);
                                respuesta.Content = new StringContent(json);
                                Logger.LoguearErrores("Exitoso para el codigo "+ProductoID);

                                ContentResult Content = new ContentResult();
                                Content.Content = json;
                                Content.StatusCode = (int)HttpStatusCode.OK;
                                Content.ContentType = "application/json";
                                return Content;

                                
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


   

    public class HijoProductos : GESI.ERP.Core.BO.cProducto
    {
        [System.Text.Json.Serialization.JsonIgnore]
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

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

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
            Categorias = padre.Categorias;
        }

    }
}
