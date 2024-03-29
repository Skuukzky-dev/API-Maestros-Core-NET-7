﻿using GESI.CORE.API.BO;
using GESI.CORE.BLL;
using GESI.CORE.BO.Verscom2k;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;

namespace GESI.CORE.API.BLL
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
                MiSessionMgrAPI.ERPSessionMgr = new ERP.Core.SessionManager();
                MiSessionMgrAPI.SessionMgr = new SessionMgr();
                MiSessionMgrAPI.Habilitado = false;

                List<HabilitacionesAPI> moHabilitacionesAPI = CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(usuarioID);

                #region Variables Auxiliares
                string strCanalesDeVenta = "";
                string strCategoriasIDs = "";
                string AlmacenesIDs = "";
                int[] Almacenes = null;
                #endregion

                if (moHabilitacionesAPI?.Count > 0)
                {
                    foreach (HabilitacionesAPI oHabilitacionAPI in moHabilitacionesAPI)
                    {
                        if (oHabilitacionAPI.TipoDeAPI.Equals(TipoDeAPI)) // VERIFICA SI ES DE TIPO LEER_MAESTROS
                        {
                            MiSessionMgrAPI.SessionMgr.EmpresaID = oHabilitacionAPI.EmpresaID;
                            MiSessionMgrAPI.SessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                            MiSessionMgrAPI.SessionMgr.SucursalID = oHabilitacionAPI.SucursalID;
                            MiSessionMgrAPI.SessionMgr.EntidadID = 1;

                            MiSessionMgrAPI.ERPSessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                            MiSessionMgrAPI.ERPSessionMgr.EmpresaID = (uint)oHabilitacionAPI.EmpresaID;


                            MiSessionMgrAPI.CostosXProveedor = oHabilitacionAPI.CostosDeProveedor;
                            MiSessionMgrAPI.EstadoProductos = oHabilitacionAPI.Estados;
                            MiSessionMgrAPI.CategoriasIDs = oHabilitacionAPI.CategoriasIDs;
                            MiSessionMgrAPI.Habilitado = true;
                            strCanalesDeVenta = oHabilitacionAPI.CanalesDeVenta;
                            strCategoriasIDs = oHabilitacionAPI.CategoriasIDs;
                            AlmacenesIDs = oHabilitacionAPI.AlmacenesIDs;
                        }
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

                    // Devolver Canales de Venta habilitados por usuario

                    List<ERP.Core.BO.cCanalDeVenta> lstCanalesXUsuario = MiSessionMgrAPI.ERPSessionMgr.GetCanalesDeVentaHabilitados();

                    if (strCanalesDeVenta?.Length > 0)
                    {
                        List<string> canalesaux = strCanalesDeVenta.Split(',').ToList();
                        int[] intCanales = new int[canalesaux.Count];
                        List<int> lstCanales = new List<int>();
                        for (int i = 0; i < canalesaux.Count; i++)
                        {
                            int.TryParse(canalesaux[i], out intCanales[i]); // Convertir cada substring en un entero y asignarlo al array de enteros

                            List<ERP.Core.BO.cCanalDeVenta> lstCanalesAux = lstCanalesXUsuario.Where(x => x.CanalDeVentaID == intCanales[i]).ToList();

                            if (lstCanalesAux?.Count > 0)
                            {
                                lstCanales.Add(intCanales[i]);
                            }

                        }

                        if (lstCanales?.Count > 0)
                        {
                            MiSessionMgrAPI.CanalesDeVenta = lstCanales.ToArray();
                        }
                        else
                        {
                            MiSessionMgrAPI.CanalesDeVenta = intCanales;
                        }
                    }
                    else
                    {
                        if (lstCanalesXUsuario?.Count > 0)
                        {
                            List<int> lstCanalesAux = new List<int>();
                            foreach (ERP.Core.BO.cCanalDeVenta oCanal in lstCanalesXUsuario)
                            {
                                lstCanalesAux.Add(oCanal.CanalDeVentaID);
                            }

                            MiSessionMgrAPI.CanalesDeVenta = lstCanalesAux.ToArray();
                        }

                    }
                    #endregion

                }

                return MiSessionMgrAPI;
            }
            catch (Exception ex)
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
        public static Error DevolverErrorAPI(int code, string message, string tipo, string usuario, string endpoint)
        {
            //List<TipoDeError> lstTiposDeErrores = APIHelper.LlenarTiposDeError();
            Logger.LoguearErrores(message, tipo, usuario, endpoint, code);
            Error error = new Error();
            error.code = code;
            error.message = message;

            return error;
        }

        /// <summary>
        /// Evalua el protocolo a utilizar en back con la variable de configuracion
        /// </summary>
        /// <param name="protocoloconfig"></param>
        /// <param name="protocolo"></param>
        /// <returns></returns>
        public static bool EvaluarProtocolo(string protocoloconfig, string protocolo)
        {
            bool Habilitado = false;

            if (protocoloconfig.Length > 0)
            {
                if (protocolo.Equals(protocoloconfig))
                {
                    Habilitado = true;
                }
            }
            else
            {
                Habilitado = true;
            }

            return Habilitado;

        }

        /// <summary>
        /// Tabla con los codigos de error
        /// </summary>
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
            cCodigoNoHalladoEnLaSolicitud = 4041,
            cProtocoloIncorrecto = 5555,
            cURLNoPermitida = 4018,
            cEncabezadoNoEncontrado = 4020
        }

        /// <summary>
        /// Desencripta la cadena de configuracion
        /// </summary>
        /// <param name="CadenaEncriptada"></param>
        /// <returns></returns>
        public static string DesEncriptarCadenaConfiguracion(string CadenaEncriptada)
        {
            try
            {
                string CadenaSinCaracteres;
                string CadenaDesEncriptada = "";
                if (CadenaEncriptada.Length > 3)
                {
                    CadenaSinCaracteres = CadenaEncriptada.Remove(3, 1);
                    var base64EncodedBytes = System.Convert.FromBase64String(CadenaSinCaracteres);
                    CadenaSinCaracteres = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                    CadenaDesEncriptada = CadenaSinCaracteres;
                }

                return CadenaDesEncriptada;
            }
            catch { return ""; }

        }


        public static List<TipoDeError> LlenarTiposDeError()
        {
            List<TipoDeError> lstTiposDeError = new List<TipoDeError>();

            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cUsuarioIncorrecto, "Usuario y / o contraseña incorrectos", (int)HttpStatusCode.Unauthorized, "I"));
            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cTokenInvalido, "Token invalido. Acceso denegado", (int)HttpStatusCode.Unauthorized, "I"));
            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cTokenNoEncontrado, "No se encontro el Token en el Request", (int)HttpStatusCode.Unauthorized, "I"));
            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cTokenEncabezadoNoEncontrado, "No se encontro el Token en el Request", (int)HttpStatusCode.Unauthorized, "I"));
            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cNuevoToken, "No está autorizado a acceder al servicio. No se encontró el token del usuario", (int)HttpStatusCode.Unauthorized, "I"));
            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cPermisoDenegadoCostos, "Permiso Denegado en costos", (int)HttpStatusCode.Unauthorized, "I"));
            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cURLNoPermitida, "URL o IP Restringida", (int)HttpStatusCode.Unauthorized, "I"));
            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cEncabezadoNoEncontrado, "No se encontro el Encabezado en la solicitud", (int)HttpStatusCode.Unauthorized, "I"));


            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cSintaxisIncorrecta, "Sintaxis incorrecta de categorias a filtrar", (int)HttpStatusCode.BadRequest, "E"));

            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cErrorConversionDato, "Se definio un formato de fecha incorrecto", (int)HttpStatusCode.InternalServerError, "E"));
            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cErrorInternoAlDevolverToken, "Error al devolver el token", (int)HttpStatusCode.InternalServerError, "E"));
            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cErrorInternoAplicacion, "Error interno de la aplicación", (int)HttpStatusCode.InternalServerError, "E"));

            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cModeloNoValido, "Modelo no válido", (int)HttpStatusCode.UnsupportedMediaType, "E"));

            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cCodigoNoHalladoEnLaSolicitud, "No se encontró codigo o expresión a buscar", (int)HttpStatusCode.NoContent, "I"));

            lstTiposDeError.Add(new TipoDeError((int)cCodigosError.cProtocoloIncorrecto, "Protocolo Incorrecto en la solicitud", (int)HttpStatusCode.BadRequest, "E"));

            return lstTiposDeError;
        }


        /// <summary>
        /// Setea el connectionString en caso de cambiar la base de datos desde config
        /// </summary>
        public static void SetearConnectionString()
        {
            try
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                string connectionenstring = CORE.SEC.Seguridad.DesencriptarTexto(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k|enc"].ConnectionString);

                SqlConnection sqlapi = new SqlConnection(connectionenstring);
                CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                ERP.Core.BLL.BASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
            }
            catch (Exception ex) { Logger.LoguearErrores("Error al setear connectionString: " + ex.Message, "E", "Usuario", "--"); }
        }


        #region Constantes de Endpoints
        public const string ProductosGetList = "Productos/GetList";
        public const string ProductosGetItem = "Productos/GetItem";
        public const string ProductosGetSearchResult = "Productos/GetSearchResult";
        public const string ProductosGetExistencias = "Productos/GetExistencias";
        public const string ProductoGetPrecios = "Productos/GetPrecios";
        public const string CanalesDeVentaGetList = "CanalesDeVenta/GetList";
        public const string Login = "api/Login";
        public const string CategoriasGetList = "Categorias/GetList";
        public const string CategoriasGetItem = "Categorias/GetItem";
        public const string SucursalesGetList = "Empresas/GetSucursales";
        public const string TipoDeAPI = "LEER_MAESTROS";
        public static string QueEstabaHaciendo = "";
        #endregion




    }
}
