using API_Maestros_Core.BLL;
using API_Maestros_Core.Controllers;
using API_Maestros_Core.Models;
using API_Maestros_Core.Services;
using AspNetCoreRateLimit;
using GESI.CORE.BLL;
using GESI.GESI.BLL.wsfev1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
                context.Response.StatusCode = 401;
                RespuestaConToken tok = new RespuestaConToken();
                tok.success = false;
                tok.error = new Error();
                tok.error.code = 4012;
                context.Response.ContentType = "application/json";
                tok.error.message = "Token invalido. Acceso denegado";
                Logger.LoguearErrores("Token invalido. Acceso denegado", "I","", context.Request.Path.Value);
                
                // we can write our own custom response content here
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(tok));
            }
            else
            {
                context.Response.StatusCode = 401;
                RespuestaConToken tok = new RespuestaConToken();
                tok.success = false;
                tok.error = new Error();
                tok.error.code = 4013;
                context.Response.ContentType = "application/json";
                tok.error.message = "No se encontro el Token en el Request";
                Logger.LoguearErrores("No se encontro el Token en el Request", "I","", context.Request.Path.Value);
                // we can write our own custom response content here
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(tok));
            }
            
            
        },
        OnTokenValidated = async(context) =>
        {
          //  GESI.CORE.BO.Verscom2k.APILogin MiObjetoLogin = GESI.CORE.BLL.Verscom2k.ApiLoginMgr.GetItem(context.SecurityToken["RowData"]);
            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken != null)
            {
                ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    identity.AddClaim(new Claim("access_token", accessToken.RawData));
                    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                    fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                    System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


                    SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                    GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;

                    GESI.CORE.BO.Verscom2k.APILogin MiObjetoLogin = GESI.CORE.BLL.Verscom2k.ApiLoginMgr.GetItem(accessToken.RawData);

                    if(MiObjetoLogin != null) // ESTA OK. ENCONTRO EL TOKEN EN LA TABLA DE TOKENS X USUARIO
                    {
                        ProductosController.strUsuarioID = MiObjetoLogin.UsuarioID;
                        ProductosController.HabilitadoPorToken = true;
                        ProductosController.TokenEnviado = accessToken.RawData;

                        CanalesDeVentaController.strUsuarioID = MiObjetoLogin.UsuarioID;
                        CanalesDeVentaController.HabilitadoPorToken = true;
                        CanalesDeVentaController.TokenEnviado = accessToken.RawData;

                        CategoriasController.strUsuarioID = MiObjetoLogin.UsuarioID;
                        CategoriasController.HabilitadoPorToken = true;
                        CategoriasController.TokenEnviado = accessToken.RawData;


                        EmpresasController.strUsuarioID = MiObjetoLogin.UsuarioID;
                        EmpresasController.HabilitadoPorToken = true;
                        EmpresasController.TokenEnviado = accessToken.RawData;

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
                    oresp.error.code = 4015;
                    oresp.error.message = "No se encontro encabezado para la petición. Endpoint: "+context.Request.Path.Value;
                    Logger.LoguearErrores("No se encontro encabezado para la petición Endpoint: "+context.Request.Path.Value, "I", "", context.Request.Path.Value);
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
                    context.Response.StatusCode = 401;
                    RespuestaToken oresp = new RespuestaToken();
                    oresp.error = new ErrorToken();
                    oresp.error.code = 4011;
                    oresp.error.message = "No esta autorizado a acceder al recurso IP: " + ipAddress.ToString();
                    Logger.LoguearErrores("No esta autorizado a acceder al recurso IP: " + ipAddress.ToString(),"I", "", context.Request.Path.Value);
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
                        oresp.error.code = 4015;
                        oresp.error.message = "No se encontro encabezado para la petición . Endpoint: "+ context.Request.Path.Value;
                        Logger.LoguearErrores("No se encontro encabezado para la petición Endpoint: "+context.Request.Path.Value,"I", "", context.Request.Path.Value);
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(oresp));
                        return;
                    }
                    else
                    {
                        Logger.LoguearErrores("IPs Autorizadas: " + IPConfig,"I", "", context.Request.Path.Value);
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
                    oresp.error.code = 4015;
                    oresp.error.message = "No se encontro encabezado para la petición . Endpoint: "+context.Request.Path.Value;
                    Logger.LoguearErrores("No se encontro encabezado para la petición Endpoint: " + context.Request.Path.Value, "I", "", context.Request.Path.Value);
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(oresp));
                    return;
                }
            }
            else
            {


                if (!spliturl.Contains(referrerUrl.ToString()))
                {
                    context.Response.StatusCode = 401;
                    RespuestaToken oresp = new RespuestaToken();
                    oresp.error = new ErrorToken();
                    oresp.error.code = 4011;
                    oresp.error.message = "No esta autorizado a acceder al recurso. URL: " + referrerUrl;
                    Logger.LoguearErrores("No esta autorizado a acceder al recurso. URL: " + referrerUrl, "E", "", context.Request.Path.Value);
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
                        oresp.error.code = 4015;
                        oresp.error.message = "No se encontro encabezado para la petición . Endpoint: " + context.Request.Path.Value;
                        Logger.LoguearErrores("No se encontro encabezado para la petición Endpoint: " + context.Request.Path.Value, "E", "", context.Request.Path.Value);
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(oresp));
                        return;
                    }
                    else
                    {
                        Logger.LoguearErrores("Dominios Autorizados: " + urlConfig, "I", "", context.Request.Path.Value);
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
