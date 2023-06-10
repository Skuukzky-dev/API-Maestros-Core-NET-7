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
            var Signin = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["CadenaSignin"];

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
                               
               
                SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;

                //Logger.LoguearErrores("ConnectionString app.config :" + connectionString);


                // SqlConnection sqlapi = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConexionVersCom2k"].ConnectionString);

                Logger.LoguearErrores("ConnectionStringPaz: " + sqlapi.ConnectionString);

                bool login = GESI.CORE.BLL.UsuariosMgr.Login(strUsuarioID, strPassword);
                return login;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
