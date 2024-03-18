using API_Maestros_Core.BLL;
using API_Maestros_Core.Controllers;
using API_Maestros_Core.Services;
using AspNetCoreRateLimit;
using GESI.CORE.BLL;
using GESI.CORE.DAL;
using GESI.GESI.BLL.wsfev1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => { options.CustomSchemaIds(type => type.FullName);
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API Maestros Grupo ESI" });options.EnableAnnotations(); });


builder.Services.AddAuthorization(options =>
        options.DefaultPolicy =
        new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build()
    );
var issuer = builder.Configuration["AuthenticationSettings:Issuer"];
var audience = builder.Configuration["AuthenticationSettings:Audience"];
var signinKey = builder.Configuration["AuthenticationSettings:SigningKey"];

var myCorsPolicy = "MyCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myCorsPolicy,
       policy =>
       {
           policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("MiHeaderPersonalizado"); ;
       });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Audience = audience;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signinKey))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = async (context) =>
        {
            Console.WriteLine("Printing in the delegate OnAuthFailed");
        },
        OnChallenge = async (context) =>
        {
            Console.WriteLine("Printing in the delegate OnChallenge");

            // this is a default method
            // the response statusCode and headers are set here
            context.HandleResponse();

            // AuthenticateFailure property contains 
            // the details about why the authentication has failed
            if (context.AuthenticateFailure != null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; 
                GESI.CORE.API.BO.Response tok = new GESI.CORE.API.BO.Response();
                tok.success = false;
                tok.error = new GESI.CORE.API.BO.Error();
                tok.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cTokenInvalido;
                context.Response.ContentType = "application/json";
                tok.error.message = "Token invalido. Acceso denegado";
                
                GESI.CORE.API.BLL.Logger.LoguearErrores("Token invalido. Acceso denegado", "I","", context.Request.Path.Value);
              
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(tok));
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                GESI.CORE.API.BO.Response tok = new GESI.CORE.API.BO.Response();
                tok.success = false;
                tok.error = new GESI.CORE.API.BO.Error();
                tok.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cTokenNoEncontrado;
                context.Response.ContentType = "application/json";
                tok.error.message = "No se encontro el Token en el Request";

                GESI.CORE.API.BLL.Logger.LoguearErrores("No se encontro el Token en el Request", "I","", context.Request.Path.Value);
                
                // we can write our own custom response content here
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(tok));
            }
            
            
        },
        OnTokenValidated = async(context) =>
        {
          
            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken != null)
            {
                ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    identity.AddClaim(new Claim("access_token", accessToken.RawData));

                    GESI.CORE.API.BLL.APIHelper.SetearConnectionString();

                    List<GESI.CORE.BO.Verscom2k.APILogin>  MiObjetoLogin = GESI.CORE.DAL.Verscom2k.ApiLoginDB.GetItem(accessToken.RawData);
                   
                    if(MiObjetoLogin != null) // ESTA OK. ENCONTRO EL TOKEN EN LA TABLA DE TOKENS X USUARIO
                    {
                       
                            ProductosController.strUsuarioID = MiObjetoLogin[0].UsuarioID;
                            ProductosController.HabilitadoPorToken = true;
                            ProductosController.TokenEnviado = accessToken.RawData;
                            ProductosController.strProtocolo = context.Request.Scheme;
                            var referrerUrl = context.Request.Headers[":authority"].ToString();
            

                            CanalesDeVentaController.strUsuarioID = MiObjetoLogin[0].UsuarioID;
                            CanalesDeVentaController.HabilitadoPorToken = true;
                            CanalesDeVentaController.TokenEnviado = accessToken.RawData;
                            CanalesDeVentaController.strProtocolo = context.Request.Scheme;
        

                            CategoriasController.strUsuarioID = MiObjetoLogin[0].UsuarioID;
                            CategoriasController.HabilitadoPorToken = true;
                            CategoriasController.TokenEnviado = accessToken.RawData;
                            CategoriasController.strProtocolo = context.Request.Scheme;
                            

                            EmpresasController.strUsuarioID = MiObjetoLogin[0].UsuarioID;
                            EmpresasController.HabilitadoPorToken = true;
                            EmpresasController.TokenEnviado = accessToken.RawData;
                            EmpresasController.strProtocolo = context.Request.Scheme;

                    }
                    else // NO LO ENCONTRO EN LA BASE
                    {
                        ProductosController.HabilitadoPorToken = false;
                        CanalesDeVentaController.HabilitadoPorToken = false;
                        CategoriasController.HabilitadoPorToken = false;
                        EmpresasController.HabilitadoPorToken = false;
               
                    }
                }
            }

        }
    };
}
);

builder.Services.Configure<IpRateLimitOptions>(options =>
{
    var CantidadMaximaSolicitudes = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["RateLimit"];
    options.GeneralRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Limit = Convert.ToDouble(CantidadMaximaSolicitudes),
                Period = "1m",
                
            }
        };
    
    options.QuotaExceededMessage = "Se excedio la cantidad maxima de solicitudes a realizar. Cantidad maxima Permitida: "+ CantidadMaximaSolicitudes;
    //Logger.LoguearErrores("Se excedio la cantidad maxima de solicitudes a realizar. Cantidad maxima Permitida: " + CantidadMaximaSolicitudes, "I");
});




builder.Services.AddRateLimiting(builder.Configuration);



var app = builder.Build();


app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());



// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DefaultModelsExpandDepth(-1); // Disable swagger schemas at bottom
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    
});




app.Use(async (context, next) =>
{
    var allowedIPs = new List<string> { "192.168.1.1", "192.168.1.2", "::1" }; // IPs permitidas
    var ipAddress = context.Connection.RemoteIpAddress;
    String urlConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["UrlPermitidas"];

    List<string> spliturl = new List<string>();

    if (urlConfig.Length > 0)
        spliturl = urlConfig.Split(',').ToList();

    String IPConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["IPsPermitidas"];

    List<string> splitIPs = new List<string>();

    if (IPConfig.Length > 0)
        splitIPs = IPConfig.Split(',').ToList();

    var referrerUrl = context.Request.Headers["Referer"].ToString();
    
    if (referrerUrl != null)
    {
        if (referrerUrl.Length <= 0)
        {
            if (splitIPs.Count == 0)
            {
                if (!context.Request.Headers.ContainsKey("GrupoESI"))
                {
                    RespuestaToken oresp = new RespuestaToken();
                    oresp.error = new ErrorToken();
                    oresp.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cEncabezadoNoEncontrado;
                    oresp.error.message = "No se encontro encabezado para la petición.";

                    GESI.CORE.API.BLL.Logger.LoguearErrores("No se encontro encabezado para la petición Endpoint: " + context.Request.Path.Value + " IP: " + context.Connection.RemoteIpAddress, "I", "", context.Request.Path.Value);

                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(oresp));
                    return;
                }
            }
            else
            {
                if (!splitIPs.Contains(ipAddress.ToString()))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    RespuestaToken oresp = new RespuestaToken();
                    oresp.error = new ErrorToken();
                    oresp.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cURLNoPermitida;
                    oresp.error.message = "No esta autorizado a acceder al recurso IP restringida";

                    GESI.CORE.API.BLL.Logger.LoguearErrores("No esta autorizado a acceder al recurso IP Restringida: " + ipAddress.ToString()+" | Referer: "+referrerUrl,"I", "", context.Request.Path.Value);

                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(oresp));
                    return;
                }
                else
                {
                    if (!context.Request.Headers.ContainsKey("GrupoESI"))
                    {
                        RespuestaToken oresp = new RespuestaToken();
                        oresp.error = new ErrorToken();
                        oresp.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cEncabezadoNoEncontrado;
                        oresp.error.message = "No se encontro encabezado para la petición";
                        
                        GESI.CORE.API.BLL.Logger.LoguearErrores("No se encontro encabezado para la petición Endpoint: "+context.Request.Path.Value+" IP: "+ context.Connection.RemoteIpAddress+" Referer: " + referrerUrl, "I", "", context.Request.Path.Value);
                        
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(oresp));
                        return;
                    }
                    else
                    {
                
                    }
                }
            }

        }
        else //Tiene URL
        {
            if (spliturl.Count == 0)
            {
                if (!context.Request.Headers.ContainsKey("GrupoESI"))
                {
                    RespuestaToken oresp = new RespuestaToken();
                    oresp.error = new ErrorToken();
                    oresp.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cEncabezadoNoEncontrado;
                    oresp.error.message = "No se encontro encabezado para la petición";

                    GESI.CORE.API.BLL.Logger.LoguearErrores("No se encontro encabezado para la petición Endpoint: " + context.Request.Path.Value + " IP: " + context.Connection.RemoteIpAddress + " Referer: " + referrerUrl, "I", "", context.Request.Path.Value);
                    
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(oresp));
                    return;
                }
            }
            else
            {
                bool Habilitado = false;
                foreach(string urls in spliturl)
                {
                    string urldescencriptada = GESI.CORE.API.BLL.APIHelper.DesEncriptarCadenaConfiguracion(urls);

                    if (referrerUrl.Contains(urldescencriptada))
                    {
                        Habilitado = true;
                    }
                }

                if(!Habilitado)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    RespuestaToken oresp = new RespuestaToken();
                    oresp.error = new ErrorToken();
                    oresp.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cURLNoPermitida;
                    oresp.error.message = "No esta autorizado a acceder al recurso. URL restringida";

                    GESI.CORE.API.BLL.Logger.LoguearErrores("No esta autorizado a acceder al recurso. URL restringida: " + referrerUrl + "| spliturl: " + spliturl, "E", "", context.Request.Path.Value);

                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(oresp));
                    return;
                }
                else
                {

                    if (!context.Request.Headers.ContainsKey("GrupoESI"))
                    {
                        RespuestaToken oresp = new RespuestaToken();
                        oresp.error = new ErrorToken();
                        oresp.error.code = (int)GESI.CORE.API.BLL.APIHelper.cCodigosError.cEncabezadoNoEncontrado;
                        oresp.error.message = "No se encontro encabezado para la petición";

                        GESI.CORE.API.BLL.Logger.LoguearErrores("No se encontro encabezado para la petición Endpoint: " + context.Request.Path.Value + " IP: " + context.Connection.RemoteIpAddress, "E", "", context.Request.Path.Value);

                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(oresp));
                        return;
                    }
                    else
                    {
                        //Logger.LoguearErrores("Dominios Autorizados: " + urlConfig, "I", "", context.Request.Path.Value);
                    }
                    
                }
            }
        }
    }

    await next();
});


app.UseHttpsRedirection();
app.UseRateLimiting();
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();


app.Run();
