using API_Maestros_Core.Models;
using API_Maestros_Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService authService;

        public LoginController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [EnableCors("MyCorsPolicy")]
        public RespuestaToken Token(UserLogin credenciales)
        {
            try
            {
                HttpResponseMessage message = new HttpResponseMessage();
                if (authService.ValidateLogin(credenciales.Username, credenciales.Password))
                {
                    var fechaActual = DateTime.UtcNow;
                    var TiempoExpiracionToken = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["TiempoExpiracion"];
                    var validez = TimeSpan.FromHours(Convert.ToDouble(TiempoExpiracionToken));
                    var fechaExpiracion = fechaActual.Add(validez);

                    var token = authService.GenerateToken(fechaActual, credenciales.Username, validez);

                    RespuestaToken respuestaToken = new RespuestaToken();
                    respuestaToken.error = new ErrorToken();
                    respuestaToken.success = true;
                    respuestaToken.token = token;
                    message.StatusCode = HttpStatusCode.OK;

                    GESI.CORE.BO.Verscom2k.APILogin ApiLogin = new GESI.CORE.BO.Verscom2k.APILogin();

                    ApiLogin.FechaYHora = DateTime.Now.ToString();
                    
                    ApiLogin.UsuarioID = credenciales.Username;
                    ApiLogin.Token = token;
                    int resultado = GESI.CORE.BLL.Verscom2k.ApiLoginMgr.Save(ApiLogin);



                    message.Content = new StringContent(JsonConvert.SerializeObject(respuestaToken));
                    //Logger.LoguearErrores("Loggeado correctamente con usuario: " + credenciales.Username);
                    return respuestaToken;
                }
                RespuestaToken rspContenidoRespuesta = new RespuestaToken();
                rspContenidoRespuesta.success = false;
                rspContenidoRespuesta.error = new ErrorToken();
                rspContenidoRespuesta.error.Code = 401;
                rspContenidoRespuesta.error.Message = "Usuario y / o contraseña incorrectos";

                message.StatusCode = HttpStatusCode.Unauthorized;
                message.Content = new StringContent(JsonConvert.SerializeObject(rspContenidoRespuesta));
              //  Logger.LoguearErrores("Usuario y / o contraseña incorrectos");
                return rspContenidoRespuesta;
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage();
                RespuestaToken rspContenidoRespuesta = new RespuestaToken();
                rspContenidoRespuesta.success = false;
                rspContenidoRespuesta.error = new ErrorToken();
                rspContenidoRespuesta.error.Code = 401;
                rspContenidoRespuesta.error.Message = "error al devolver el token. Descripcion: " + ex.Message;

                message.StatusCode = HttpStatusCode.Unauthorized;
                message.Content = new StringContent(JsonConvert.SerializeObject(rspContenidoRespuesta));
                // Logger.LoguearErrores("Usuario y / o contraseña incorrectos");
                return rspContenidoRespuesta;
            }
        }
    }

    public class ErrorToken
    {
        private int _code;
        private String _message;

        public int Code { get => _code; set => _code = value; }
        public string Message { get => _message; set => _message = value; }
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
