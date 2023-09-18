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
        /// Devuelve toda la lista de productos de manera paginada
        /// </summary>
        /// <param name="expresion"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetList")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConProductosHijos))]

        public IActionResult Get( int pageNumber = 1, int pageSize = 10, string costos = "N")
        {
            
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
                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 4012;
                    oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                    Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario", "E");                   
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                    _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                    bool Habilitado = false;
                    string strCanalesDeVenta = "";
                    string strCostoUsuario = "N";
                    string strEstadosProductos = "A";
                    string strCategoriasIDs = "";
                       
                            foreach (GESI.CORE.BO.Verscom2k.HabilitacionesAPI oHabilitacionAPI in moHabilitacionesAPI)
                            {
                                if (oHabilitacionAPI.TipoDeAPI.Equals(mostrTipoAPI))
                                {
                                    _SessionMgr.EmpresaID = oHabilitacionAPI.EmpresaID;
                                    _SessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                                    _SessionMgr.SucursalID = oHabilitacionAPI.SucursalID;
                                    _SessionMgr.EntidadID = 1;
                                    strCostoUsuario = oHabilitacionAPI.CostosDeProveedor;
                                    strCanalesDeVenta = oHabilitacionAPI.CanalesDeVenta;
                                    strEstadosProductos = oHabilitacionAPI.Estados;
                                    strCategoriasIDs = oHabilitacionAPI.CategoriasIDs;
                                    Habilitado = true;
                                } 
                            }

                            if (Habilitado)
                            {
                                int TamanoPagina = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["TamanoPagina"]);

                                if (pageSize <= TamanoPagina) // Si el tamaño de la pagina enviado es menor a 100.
                                {
                                    if(pageSize > 100)
                                    { pageSize = 100; } 

                                    List<string> canalesaux = strCanalesDeVenta.Split(',').ToList();
                                    int[] intCanales = new int[canalesaux.Count];
                                    for (int i = 0; i < canalesaux.Count; i++)
                                    {
                                        int.TryParse(canalesaux[i], out intCanales[i]); // Convertir cada substring en un entero y asignarlo al array de enteros
                                    }

                                    ProductosMgr._SessionMgr = _SessionMgr;
                                    oRespuesta = ProductosMgr.GetList(pageNumber, pageSize, intCanales, costos, strCostoUsuario,strEstadosProductos,strCategoriasIDs);

                                    return Ok(oRespuesta);
                                }
                                else
                                {
                                        if (TamanoPagina > 100)
                                            TamanoPagina = 100;

                                    List<string> canalesaux = strCanalesDeVenta.Split(',').ToList();
                                    int[] intCanales = new int[canalesaux.Count];
                                    for (int i = 0; i < canalesaux.Count; i++)
                                    {
                                        int.TryParse(canalesaux[i], out intCanales[i]); // Convertir cada substring en un entero y asignarlo al array de enteros
                                    }

                                    ProductosMgr._SessionMgr = _SessionMgr;
                                    oRespuesta = ProductosMgr.GetList(pageNumber, TamanoPagina, intCanales, costos, strCostoUsuario, strEstadosProductos, strCategoriasIDs);

                                    return Ok(oRespuesta);

                                }
                                
                            }
                            else
                            {

                                oRespuesta.error = new Error();
                                oRespuesta.error.code = 4012;
                                oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                                Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario", "E");
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);
                            }
                        
               
                }
            }            
            catch (Exception ex)
            {
                oRespuesta.error = new Error();
                oRespuesta.error.code = 5002;
                oRespuesta.error.message = "error interno de la aplicacion. Descripcion: " + ex.Message;
                Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message, "E");
                oRespuesta.success = false;
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
        [HttpGet("GetItem")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaConProductosHijos))]
        public IActionResult Get(string productoID = "",int canalDeVentaID = 0,string costos = "N")
        {
            RespuestaProductosGetItem oRespuesta = new RespuestaProductosGetItem();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if (!ModelState.IsValid)
            {
                oRespuesta.error = new Error();
                oRespuesta.error.code = 4151;
                oRespuesta.error.message = "Modelo no valido";
                Logger.LoguearErrores("Modelo no valido", "E");
                oRespuesta.success = false;
                return StatusCode(415, oRespuesta);
                
            }
            else
            {
                if (productoID == null)
                {

                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 4017;
                    oRespuesta.error.message = "No se encontro el productoID de la solicitud";
                    Logger.LoguearErrores("No se encontro el productoID de la solicitud", "I");
                    oRespuesta.success = false;
                    return BadRequest(oRespuesta);
                }
                else
                {
                    if (!(productoID.Length > 0))
                    {
                        oRespuesta.error = new Error();
                        oRespuesta.error.code = 4017;
                        oRespuesta.error.message = "No se encontro el productoID de la solicitud";
                        Logger.LoguearErrores("No se encontro el productoID de la solicitud", "I");
                        oRespuesta.success = false;
                        return BadRequest(oRespuesta);

                    }
                    else
                    {

                        SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                        GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                        try
                        {
                            if (!HabilitadoPorToken)
                            {
                                oRespuesta.error = new Error();
                                oRespuesta.error.code = 4012;
                                oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                                Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario", "E");
                                oRespuesta.success = false;
                                return Unauthorized(oRespuesta);
                            }
                            else
                            {
                                string costoUsuario = "N";
                                moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                                string strCanalesDeVenta = null;
                                _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                                bool Habilitado = false;
                                string strEstadosProductos = "A";
                                string strCategoriasIDs = "";
                                if (productoID != null)
                                {
                                    if (productoID.Length > 0)
                                    {
                                        #region SessionManager
                                        foreach (GESI.CORE.BO.Verscom2k.HabilitacionesAPI oHabilitacionAPI in moHabilitacionesAPI)
                                        {
                                            if (oHabilitacionAPI.TipoDeAPI.Equals(mostrTipoAPI))
                                            {
                                                _SessionMgr.EmpresaID = oHabilitacionAPI.EmpresaID;
                                                _SessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                                                _SessionMgr.SucursalID = oHabilitacionAPI.SucursalID;
                                                _SessionMgr.EntidadID = 1;
                                                costoUsuario = oHabilitacionAPI.CostosDeProveedor;
                                                strCanalesDeVenta = oHabilitacionAPI.CanalesDeVenta;
                                                strEstadosProductos = oHabilitacionAPI.Estados;
                                                strCategoriasIDs = oHabilitacionAPI.CategoriasIDs;
                                                Habilitado = true;
                                            }
                                        }
                                        #endregion

                                        if (Habilitado)
                                        {

                                            ProductosMgr._SessionMgr = _SessionMgr;
                                            oRespuesta = ProductosMgr.GetItem(productoID, strCanalesDeVenta,canalDeVentaID,costos,costoUsuario,strEstadosProductos,strCategoriasIDs);

                                            if(oRespuesta.producto?.ProductoID?.Length > 0)
                                            {
                                                return Ok(oRespuesta);
                                            }
                                            else
                                            {
                                                return StatusCode(204, oRespuesta);
                                            }
                                          /* 
                                           * Paginacion oPaginacion = new Paginacion();
                                           
                                            oPaginacion.totalPaginas = 1;
                                            oPaginacion.paginaActual = 1;
                                            oPaginacion.tamañoPagina = 1;                                            
                                            oRespuesta.error = new Error();
                                            oRespuesta.success = true;
                                            if (lstProductos?.ProductoID?.Length > 0)
                                            {
                                                oRespuesta.producto = new HijoProductos();
                                                oRespuesta.producto = lstProductos;

                                                if (lstProductos?.ProductoID?.Length > 0)
                                                {
                                                    oPaginacion.totalElementos = 1;
                                                }
                                                else
                                                {
                                                    oPaginacion.totalElementos = 0;
                                                }
                                                Logger.LoguearErrores("Exitoso para el codigo " + productoID);
                                                oRespuesta.paginacion = oPaginacion;
                                                return Ok(oRespuesta);
                                            }
                                            else
                                            {
                                                oRespuesta.producto = null;
                                                oPaginacion.totalElementos = 0;
                                                oRespuesta.error.code = 4041;
                                                oRespuesta.error.message = "No se encontro el producto buscado";
                                                oRespuesta.paginacion = oPaginacion;
                                                return StatusCode(204,oRespuesta);
                                            }*/

                                        }
                                        else
                                        {
                                            oRespuesta.error = new Error();
                                            oRespuesta.error.code = 4012;
                                            oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                                            Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario", "E");
                                            oRespuesta.success = false;
                                            return Unauthorized(oRespuesta);
                                        }
                                    }
                                    else
                                    {
                                        oRespuesta.error = new Error();
                                        oRespuesta.error.code = 2041;
                                        oRespuesta.success = false;
                                        oRespuesta.error.message = "No se encontro expresion a buscar";
                                        Logger.LoguearErrores("No se encontro expresion a buscar.", "I");                                        
                                        return StatusCode(204, oRespuesta);
                                    }
                                }
                                else
                                {
                                    oRespuesta.error = new Error();
                                    oRespuesta.error.code = 4041;
                                    oRespuesta.error.message = "No se encontro expresion a buscar";
                                    oRespuesta.success = false;
                                    Logger.LoguearErrores("No se encontro expresion a buscar", "I");                                    
                                    return StatusCode(204, oRespuesta);
                                }
                            }
                        }
                        catch (AccessViolationException ax)
                        {
                            oRespuesta.error = new Error();
                            oRespuesta.error.code = 4012;
                            oRespuesta.error.message = "No esta autorizado a acceder al servicio. No esta habilitado a ver los costos del proveedor";
                            oRespuesta.success = false;
                            return Unauthorized(oRespuesta);
                           
                        }
                        catch (Exception ex)
                        {
                            oRespuesta.error = new Error();
                            oRespuesta.error.code = 5002;
                            oRespuesta.error.message = "Error interno de la aplicacion. Descripcion: " + ex.Message;
                            oRespuesta.success = false;
                            Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message, "E");
                            return StatusCode(500, oRespuesta);
                        }
                    }
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
        public IActionResult Get(string expresion = "",int pageNumber = 1 , int pageSize = 10,string costos = "N")
        {
            #region ConnectionStrings
            RespuestaConProductosHijos oRespuesta = new RespuestaConProductosHijos();
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            #endregion

            try
            {
                if (!HabilitadoPorToken)
                {
                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 4012;
                    oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                    Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario", "E");
                    oRespuesta.success = false;
                    return Unauthorized(oRespuesta);
                }
                else
                {
                    moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                    _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                    bool Habilitado = false;
                    string strCanalesDeVenta = "";
                    string strCostoUsuario = "N";
                    string strCategoriasIDs = null;
                    string strEstadosProductos = null;
                    if (expresion?.Length > 0)
                    {
                        #region SessionManager
                        foreach (GESI.CORE.BO.Verscom2k.HabilitacionesAPI oHabilitacionAPI in moHabilitacionesAPI)
                        {
                            if (oHabilitacionAPI.TipoDeAPI.Equals(mostrTipoAPI))
                            {
                                _SessionMgr.EmpresaID = oHabilitacionAPI.EmpresaID;
                                _SessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                                _SessionMgr.SucursalID = oHabilitacionAPI.SucursalID;
                                _SessionMgr.EntidadID = 1;
                                strCanalesDeVenta = oHabilitacionAPI.CanalesDeVenta;
                                strCostoUsuario = oHabilitacionAPI.CostosDeProveedor;
                                strEstadosProductos = oHabilitacionAPI.Estados;
                                strCategoriasIDs = oHabilitacionAPI.CategoriasIDs;
                                Habilitado = true;
                            }
                        }
                        #endregion
                        List<string> canalesaux = strCanalesDeVenta.Split(',').ToList();
                        int[] intCanales = new int[canalesaux.Count];
                        for (int i = 0; i < canalesaux.Count; i++)
                        {
                            int.TryParse(canalesaux[i], out intCanales[i]); // Convertir cada substring en un entero y asignarlo al array de enteros
                        }
                        if (Habilitado)
                        {
                            int TamanoPagina = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["TamanoPagina"]);

                            
                            if (pageSize <= TamanoPagina) // Si el tamaño de la pagina enviado es menor a 100.
                            {
                                if (pageSize > 100)
                                { pageSize = 100; }

                                ProductosMgr._SessionMgr = _SessionMgr;
                                oRespuesta = ProductosMgr.GetList(expresion,intCanales,costos, strCostoUsuario,pageNumber,pageSize,strEstadosProductos,strCategoriasIDs);
                                Logger.LoguearErrores("Respuesta exitosa para la expresion " + expresion, "I");
                                return Ok(oRespuesta);
                            }
                            else
                            {
                                if (TamanoPagina > 100) // Si el tamaño de la variable configuracion es mayor a 100. Toma los 100
                                    TamanoPagina = 100;

                                ProductosMgr._SessionMgr = _SessionMgr;
                                ProductosMgr._SessionMgr = _SessionMgr;
                                oRespuesta = ProductosMgr.GetList(expresion, intCanales, costos, strCostoUsuario, pageNumber, TamanoPagina, strEstadosProductos, strCategoriasIDs);                             
                                Logger.LoguearErrores("Respuesta exitosa para la expresion " + expresion, "I");
                                return Ok(oRespuesta);
                            }

                        }
                        else
                        {

                            oRespuesta.error = new Error();
                            oRespuesta.error.code = 4012;
                            oRespuesta.error.message = "No esta autorizado a acceder al servicio. No se encontro el token del usuario";
                            oRespuesta.success = false;
                            Logger.LoguearErrores("No esta autorizado a acceder al servicio. No se encontro el token del usuario", "E");
                            return Unauthorized(oRespuesta);
                        }

                    }
                    else
                    {
                        oRespuesta.error = new Error();
                        oRespuesta.error.code = 2041;
                        oRespuesta.error.message = "No se encontro expresion a buscar";
                        Logger.LoguearErrores("No se encontro expresion a buscar", "I");
                        oRespuesta.success = false;
                        return StatusCode(204, oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                oRespuesta.error = new Error();
                oRespuesta.error.code = 5002;
                oRespuesta.error.message = "error interno de la aplicacion. Descripcion: " + ex.Message;
                Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message, "E");
                oRespuesta.success = false;
                return StatusCode(500, oRespuesta);
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
        
        public List<int> ListaDeCategorias { get => _CodigosCategorias; set => _CodigosCategorias = value; }

        private List<int> _CodigosCategorias;

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
            Estado = padre.Estado;
            GrupoArtID = padre.GrupoArtID;
            ListaDeCategorias = new List<int>();
        }

        public HijoProductos()
        { }
            

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
