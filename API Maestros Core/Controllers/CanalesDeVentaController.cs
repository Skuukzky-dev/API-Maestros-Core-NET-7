﻿using API_Maestros_Core.BLL;
using API_Maestros_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanalesDeVentaController : ControllerBase
    {
        #region Variables
        public static GESI.CORE.BLL.SessionMgr _SessionMgr;
        public static List<GESI.CORE.BO.Verscom2k.HabilitacionesAPI> moHabilitacionesAPI;
        public static string mostrTipoAPI = "LEER_MAESTROS";
        public static string strUsuarioID = "";
        public static bool HabilitadoPorToken = false;
        #endregion

        // GET: api/<CanalesDeVentaController>
        /// <summary>
        /// Devuelve la lista de canales de venta en un CanalesDeVenta/GetList
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        [Authorize]
        [EnableCors("MyCorsPolicy")]
        public IActionResult Get()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;

            RespuestaConCanalesDeVenta oRespuesta = new RespuestaConCanalesDeVenta();
            try
            {
               
                GESI.GESI.BO.ListaCanalesDeVenta lstCanales = new GESI.GESI.BO.ListaCanalesDeVenta();
                moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);
                _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                bool Habilitado = false;
                if (!HabilitadoPorToken)
                {
                    oRespuesta.success = false;
                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 401;
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
                        GESI.GESI.BLL.TablasGeneralesGESIMgr.SessionManager = _SessionMgr;
                        oRespuesta.success = true;
                        oRespuesta.error = new Error();
                        oRespuesta.CanalesDeVenta = new List<GESI.GESI.BO.CanalDeVenta>();
                        oRespuesta.CanalesDeVenta.AddRange(GESI.GESI.BLL.TablasGeneralesGESIMgr.CanalesDeVentaGetList());
                        this.StatusCode((int)HttpStatusCode.OK);
                        return Ok(oRespuesta);
                    }
                    else
                    {
                        oRespuesta.success = false;
                        oRespuesta.error = new Error();
                        oRespuesta.error.code = 401;
                        oRespuesta.error.message = "No esta autorizado a acceder al recurso";
                        Logger.LoguearErrores("No esta autorizado a acceder al recurso");
                        this.StatusCode((int)HttpStatusCode.Unauthorized);
                        return Unauthorized(oRespuesta);
                    }
                }
            }
            catch(Exception ex)
            {
                oRespuesta.success = false;
                oRespuesta.error = new Error();
                oRespuesta.error.code = 500;
                oRespuesta.error.message = "Error interno de la aplicacion. Descripcion: "+ex.Message;
                Logger.LoguearErrores("Error interno de la aplicacion. Descripcion: " + ex.Message);
                this.StatusCode((int)HttpStatusCode.InternalServerError);
                return Unauthorized(oRespuesta);
            }

          
        }
       
    }
}
