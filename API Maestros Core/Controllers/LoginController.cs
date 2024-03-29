﻿using API_Maestros_Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private const string UsuarioSinContraseña = "Debe ingresar una contraseña";
        public List<GESI.CORE.API.BO.TipoDeError> lstTipoError = GESI.CORE.API.BLL.APIHelper.LlenarTiposDeError();
        private readonly IAuthService authService;
        
        public LoginController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaToken))]
        public IActionResult Token(GESI.CORE.API.BO.UserLogin credenciales)
        {
            try
            {
                GESI.CORE.API.BLL.APIHelper.SetearConnectionString();

                RespuestaToken rspContenidoRespuesta = new RespuestaToken();
                HttpContext context = HttpContext;
                HttpResponseMessage message = new HttpResponseMessage();

                if (authService.ValidateLogin(credenciales.Username, credenciales.Password))
                {                   
                    if (credenciales.Password.Length > 0)
                    {
                        
                        List<GESI.CORE.BO.Verscom2k.APILogin> lstAPIToken = GESI.CORE.DAL.Verscom2k.ApiLoginDB.GetItem("", credenciales.Username);
                        GESI.CORE.BO.Verscom2k.APILogin APIToken = new GESI.CORE.BO.Verscom2k.APILogin();

                        if (lstAPIToken?.Count > 0)
                            APIToken = lstAPIToken[0];

                        DateTime dtFechaObtenidaSQL = GESI.CORE.DAL.Verscom2k.TablasGeneralesGESIDB.ObtenerFechaYHoraSQL(); // FECHA ACTUAL . HAY QUE SACARLA DESDE EL SERVIDOR DE LA BASE NO DE LA HORA DE LA MAQUINA
                        var TiempoExpiracionToken = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["TiempoExpiracion"];
                        var validez = TimeSpan.FromHours(Convert.ToDouble(TiempoExpiracionToken));
                        var fechaExpiracion = dtFechaObtenidaSQL.Add(validez);

                 //       var token = authService.GenerateToken(dtFechaObtenidaSQL, credenciales.Username, validez);
                 // COMENTADO LUEGO DE VERIFICAR QUE GRABA OK LA FECHA        RespuestaToken respuestaToken = DevolverRespuestaTokenYActualizarEnBase(token, credenciales.Username, dtFechaObtenidaSQL, context.Request.HttpContext.Connection.RemoteIpAddress.ToString(), APIToken.FechaYHora);
                 //       return Ok(respuestaToken);


                        if (APIToken?.Token?.Length > 0)  // ENCONTRO OBJETO TOKEN
                        {
                            TimeSpan horasDiferencia = dtFechaObtenidaSQL - APIToken.FechaYHora;
                            int intHorasDiferencia = (int)horasDiferencia.TotalHours;
                            
                            if(intHorasDiferencia >= Convert.ToInt32(TiempoExpiracionToken)) // EXPIRÓ EL TIEMPO DEL TOKEN . Genera un nuevo Token
                            {
                                var token = authService.GenerateToken(dtFechaObtenidaSQL, credenciales.Username, validez);
                                RespuestaToken respuestaToken = DevolverRespuestaTokenYActualizarEnBase(token, credenciales.Username, dtFechaObtenidaSQL, context.Request.HttpContext.Connection.RemoteIpAddress.ToString(),APIToken.FechaYHora);
                                return Ok(respuestaToken);
                    
                            }
                            else
                            {
                                if (intHorasDiferencia < 0)  // TIENE UNA FECHA FUTURA. GENERA NUEVO TOKEN
                                {
                                    var token = authService.GenerateToken(dtFechaObtenidaSQL, credenciales.Username, validez);
                                    RespuestaToken respuestaToken = DevolverRespuestaTokenYActualizarEnBase(token, credenciales.Username, dtFechaObtenidaSQL, context.Request.HttpContext.Connection.RemoteIpAddress.ToString(), APIToken.FechaYHora);
                                    return Ok(respuestaToken);
                    
                                }
                                else    // DEVUELVE EL MISMO TOKEN QUE SE ENCUENTRA EN LA TABLA . VIGENTE
                                {
                    
                                    RespuestaToken respuestaToken = new RespuestaToken();
                                    respuestaToken.error = new ErrorToken();
                                    respuestaToken.success = true;
                                    respuestaToken.token = APIToken.Token;
                                    return Ok(respuestaToken);
                                }
                            }                           
                    
                        }
                        else   // GENERA UN NUEVO TOKEN 
                        {                         
                            var token = authService.GenerateToken(dtFechaObtenidaSQL, credenciales.Username, validez);
                            RespuestaToken respuestaToken = new RespuestaToken();
                            if (APIToken == null)
                                respuestaToken = DevolverRespuestaTokenYActualizarEnBase(token, credenciales.Username, dtFechaObtenidaSQL, context.Request.HttpContext.Connection.RemoteIpAddress.ToString(), dtFechaObtenidaSQL);
                            else
                            respuestaToken = DevolverRespuestaTokenYActualizarEnBase(token, credenciales.Username, dtFechaObtenidaSQL, context.Request.HttpContext.Connection.RemoteIpAddress.ToString(), APIToken.FechaYHora);
                            return Ok(respuestaToken);
                        }
                    }
                    else
                    {
                        GESI.CORE.API.BO.TipoDeError oTipo = lstTipoError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cUsuarioIncorrecto);
                        rspContenidoRespuesta.success = false;
                        rspContenidoRespuesta.error = new ErrorToken();
                        rspContenidoRespuesta.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cUsuarioIncorrecto;
                        rspContenidoRespuesta.error.message = oTipo.DescripcionError;
                        GESI.CORE.API.BLL.Logger.LoguearErrores(UsuarioSinContraseña, "I", credenciales.Username, GESI.CORE.API.BLL.APIHelper.Login, (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cUsuarioIncorrecto);

                        return Unauthorized(rspContenidoRespuesta);
                    }
                }

                GESI.CORE.API.BO.TipoDeError oTipo1 = lstTipoError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cUsuarioIncorrecto);
                rspContenidoRespuesta = new RespuestaToken();
                rspContenidoRespuesta.success = false;
                rspContenidoRespuesta.error = new ErrorToken();
                rspContenidoRespuesta.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cUsuarioIncorrecto;
                rspContenidoRespuesta.error.message = oTipo1.DescripcionError;
                GESI.CORE.API.BLL.Logger.LoguearErrores("Usuario y / o contraseña incorrectos. Usuario: " + credenciales.Username + "|" + context.Connection.RemoteIpAddress , "I", credenciales.Username, GESI.CORE.API.BLL.APIHelper.Login,(int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cUsuarioIncorrecto);

                return Unauthorized(rspContenidoRespuesta);
            }
            catch (Exception ex)
            {
                GESI.CORE.API.BO.TipoDeError oTipo2 = lstTipoError.Find(x => x.CodigoError == (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAlDevolverToken);
                HttpResponseMessage message = new HttpResponseMessage();
                RespuestaToken rspContenidoRespuesta = new RespuestaToken();
                rspContenidoRespuesta.success = false;
                rspContenidoRespuesta.error = new ErrorToken();
                rspContenidoRespuesta.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAlDevolverToken;
                rspContenidoRespuesta.error.message = oTipo2.DescripcionError+" Descripcion: " + ex.Message;
                
                GESI.CORE.API.BLL.Logger.LoguearErrores("Error al devolver el token. Descripcion: " + ex.Message, "I", credenciales.Username, GESI.CORE.API.BLL.APIHelper.Login, (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cErrorInternoAlDevolverToken);
                
                return StatusCode((int)HttpStatusCode.InternalServerError, rspContenidoRespuesta);
            }
        }

        /// <summary>
        /// Devuelve la respuesta del Token y Actualiza la base con la fecha Actual
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="usuario"></param>
        /// <param name="FechaAActualizar"></param>
        /// <param name="clientIpAddress"></param>
        /// <returns></returns>
        public static RespuestaToken DevolverRespuestaTokenYActualizarEnBase(string Token, string usuario,DateTime FechaAActualizar,string clientIpAddress,DateTime dtFechaEnBase)
        {
            try
            {
                #region Objeto a Grabar
                GESI.CORE.BO.Verscom2k.APILogin ApiLogin = new GESI.CORE.BO.Verscom2k.APILogin();
                ApiLogin.FechaYHora = FechaAActualizar;
                ApiLogin.UsuarioID = usuario;
                ApiLogin.Token = Token;
                #endregion


                #region Respuesta Token
                RespuestaToken respuestaToken = new RespuestaToken();
                respuestaToken.error = new ErrorToken();
                respuestaToken.success = true;
                respuestaToken.token = Token;
                #endregion

                GESI.CORE.API.BLL.Logger.LoguearErrores("Fecha a Actualizar: " + FechaAActualizar + " | UsuarioID : " + usuario+" | Fecha en Base: "+dtFechaEnBase,"I",usuario,"api/Login");

                int resultado = GESI.CORE.BLL.Verscom2k.ApiLoginMgr.Save(ApiLogin);

                GESI.CORE.API.BLL.Logger.LoguearErrores("Logueado exitosamente. Usuario: " + ApiLogin.UsuarioID + "|" + clientIpAddress + " Token: " + Token, "I", ApiLogin.UsuarioID, GESI.CORE.API.BLL.APIHelper.Login);

                return respuestaToken;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class ErrorToken
    {
        private int _code;
        private String _message;

        public int code { get => _code; set => _code = value; }
        public string message { get => _message; set => _message = value; }
    }


    public class RespuestaToken
    {
        private bool _Success;
        private ErrorToken _Error;
        private String _Token;

        public bool success { get => _Success; set => _Success = value; }
        public ErrorToken error { get => _Error; set => _Error = value; }
        public string token { get => _Token; set => _Token = value; }
    }
}
