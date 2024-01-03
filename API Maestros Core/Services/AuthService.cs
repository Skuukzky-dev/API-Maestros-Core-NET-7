using API_Maestros_Core.BLL;
using GESI.CORE.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Maestros_Core.Services
{
    public class AuthService : IAuthService
    {
        public bool ValidateLogin(string username, string password)
        {
            //aqui haríamos la validación, de momento simulamos validación login
            bool isCredentialValid = Loguear(username, password);
            if (isCredentialValid)
                return true;
            return false;
        }

        [HttpGet]
        public string GenerateToken(DateTime fechaActual, string username, TimeSpan tiempoValidez)
        {
            var fechaExpiracion = fechaActual.Add(tiempoValidez);
            //Configuramos las claims
            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub,username),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(fechaActual).ToUniversalTime().ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            ),
            new Claim("roles","Cliente"),
            new Claim("roles","Administrador"),
            };

            //Añadimos las credenciales
            // Guid guid = Guid.NewGuid();
            //Guid guid2 = Guid.NewGuid();
            //string conca = guid.ToString() + guid2.ToString();

            //var Signin = conca;

            //            var Signin = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["CadenaSignin"]; ORIGINAL
            //"CadenaSignin": "4f169056-fb2f-41fd-8eb9-c46d4603c1c494ab1bf5-cb82-4325-ad24-2da97951d130",

            var Signin = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AuthenticationSettings")["SigningKey"];



            var signingCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Signin)),


                    SecurityAlgorithms.HmacSha256Signature
            );//luego se debe configurar para obtener estos valores, así como el issuer y audience desde el appsetings.json

            //Configuracion del jwt token
            var jwt = new JwtSecurityToken(
                issuer: "Peticionario",
                audience: "Public",
                claims: claims,
                notBefore: fechaActual,
                expires: fechaExpiracion,
                signingCredentials: signingCredentials
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public static bool Loguear(String strUsuarioID, String strPassword)
        {

            try
            {
                /*
                byte[] base64EncodedBytes = null;
                String passSinCaracteres = "";
                String strUsuarioSinCaracteres = "";
                passSinCaracteres = strPassword;
                if (strPassword.Length > 0)
                {
                    if (strPassword.Length > 2)
                        passSinCaracteres = strPassword.Remove(3, 1);
                    base64EncodedBytes = System.Convert.FromBase64String(passSinCaracteres);
                    passSinCaracteres = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                }
                else
                {
                    passSinCaracteres = strPassword;
                }

                strUsuarioSinCaracteres = strUsuarioID;
                if (strUsuarioSinCaracteres.Length > 2) ;
                strUsuarioSinCaracteres = strUsuarioID.Remove(3, 1);
                base64EncodedBytes = System.Convert.FromBase64String(strUsuarioSinCaracteres);
                strUsuarioSinCaracteres = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                */

                //  GESI.CORE.DAL.DBHelper.
                SqlConnection sql = GESI.CORE.DAL.DBHelper.DevolverConnectionStringCORE();
                

                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                               
               
                //SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                //GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;

                //Logger.LoguearErrores("ConnectionString app.config :" + connectionString);


                // SqlConnection sqlapi = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConexionVersCom2k"].ConnectionString);

                // Logger.LoguearErrores("ConnectionStringPaz: " + sqlapi.ConnectionString, "I");
                bool Habilitado = false;

                

                bool login = GESI.CORE.BLL.UsuariosMgr.Login(strUsuarioID, strPassword);
                List<GESI.CORE.BO.Verscom2k.HabilitacionesAPI> lstHabilitacionesAPI =  GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);

                if(lstHabilitacionesAPI?.Count > 0)
                {
                    if(login)
                    {
                        Habilitado = true;
                    }
                }

              
                return Habilitado;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
