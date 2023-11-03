using API_Maestros_Core.Models;
using GESI.CORE.BLL;
using GESI.CORE.BO.Verscom2k;
using GESI.CORE.DAL.Verscom2k;

namespace API_Maestros_Core.BLL
{
    public class APIHelper
    {
        /// <summary>
        /// Retorna un SessionMgr con los valores de la tabla HabilitacionesAPI
        /// </summary>
        /// <param name="usuarioID"></param>
        /// <returns></returns>
        public static APISessionManager SetearMgrAPI(string usuarioID)
        {
            APISessionManager MiSessionMgrAPI = null;
            try
            {
                MiSessionMgrAPI = new APISessionManager();
                MiSessionMgrAPI.SessionMgr = new SessionMgr();
                MiSessionMgrAPI.Habilitado = false;

                List<HabilitacionesAPI>  moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(usuarioID);

                #region Variables Auxiliares
                string strCanalesDeVenta = "";
                string strCategoriasIDs = "";
                string AlmacenesIDs = "";
                int[] Almacenes = null;
                #endregion

                if (moHabilitacionesAPI?.Count > 0)
                {
                    foreach(HabilitacionesAPI oHabilitacionAPI in moHabilitacionesAPI)
                    {
                        MiSessionMgrAPI.SessionMgr.EmpresaID = oHabilitacionAPI.EmpresaID;
                        MiSessionMgrAPI.SessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                        MiSessionMgrAPI.SessionMgr.SucursalID = oHabilitacionAPI.SucursalID;
                        MiSessionMgrAPI.SessionMgr.EntidadID = 1;
                        MiSessionMgrAPI.CostosXProveedor = oHabilitacionAPI.CostosDeProveedor;
                        MiSessionMgrAPI.EstadoProductos = oHabilitacionAPI.Estados;
                        MiSessionMgrAPI.CategoriasIDs = oHabilitacionAPI.CategoriasIDs;
                        MiSessionMgrAPI.Habilitado = true;
                        strCanalesDeVenta = oHabilitacionAPI.CanalesDeVenta;                        
                        strCategoriasIDs = oHabilitacionAPI.CategoriasIDs;
                        AlmacenesIDs = oHabilitacionAPI.AlmacenesIDs;                        
                    }

                    #region Almacenes
                    if (AlmacenesIDs?.Length > 0)
                    {
                        List<string> AlmacenesAux = AlmacenesIDs.Split(',').ToList();
                        Almacenes = new int[AlmacenesAux.Count];
                        for (int i = 0; i < AlmacenesAux.Count; i++)
                        {
                            int.TryParse(AlmacenesAux[i], out Almacenes[i]); // Convertir cada substring en un entero y asignarlo al array de enteros
                        }
                        MiSessionMgrAPI.Almacenes = Almacenes;
                    }
                    #endregion

                    #region CanalesDeVenta
                    if (strCanalesDeVenta?.Length > 0)
                    {
                        List<string> canalesaux = strCanalesDeVenta.Split(',').ToList();
                        int[] intCanales = new int[canalesaux.Count];
                        for (int i = 0; i < canalesaux.Count; i++)
                        {
                            int.TryParse(canalesaux[i], out intCanales[i]); // Convertir cada substring en un entero y asignarlo al array de enteros
                        }
                        MiSessionMgrAPI.CanalesDeVenta = intCanales;
                    }
                    #endregion

                } 

                return MiSessionMgrAPI;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Devuelve un objeto Error de la API
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public static Error DevolverErrorAPI(int code, string message,string tipo,string usuario,string endpoint)
        {
            Logger.LoguearErrores(message, tipo,usuario,endpoint);
            Error error = new Error();
            error.code = code;
            error.message = message;

            return error;
        }


        public enum cCodigosError
        {
            cUsuarioIncorrecto = 4011,
            cTokenInvalido = 4012,
            cTokenNoEncontrado = 4013,
            cTokenEncabezadoNoEncontrado = 4015,
            cNuevoToken = 4016,
            cSintaxisIncorrecta = 4001,
            cErrorConversionDato = 4002,
            cErrorInternoAlDevolverToken = 5001,
            cErrorInternoAplicacion = 5002,
            cModeloNoValido = 4151,
            cPermisoDenegadoCostos = 4017,
            cCodigoNoHalladoEnLaSolicitud = 4041

        }

        public const string ProductosGetList = "GetList";
        public const string ProductosGetItem = "GetItem";
        public const string ProductosGetSearchResult = "GetSearchResult";
        public const string ProductosGetExistencias = "GetExistencias";
        public const string ProductoGetPrecios = "GetPrecios";
        public const string CanalesDeVentaGetList = "CanalesDeVenta/GetList";
        public const string Login = "Login";
        public const string CategoriasGetList = "Categorias/GetList";
        public const string CategoriasGetItem = "Categorias/GetItem";
        public const string SucursalesGetList = "GetSucursales";

        public static string QueEstabaHaciendo = "";
        
                
    }
}
