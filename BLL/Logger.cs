using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BLL
{
    /// <summary>
    /// DOCUMENTACIÓN API:
    /// 
    /// https://docs.google.com/document/d/1DTp_-oqErewTyGkTvkW0FwsUmlVNlsy8iopBPZYQBrs/edit?usp=sharing
    /// 
    /// </summary>
    /// 
    public class Logger
    {
        /// <summary>
        /// Loguea los casos exitosos y errores de la API
        /// </summary>
        /// <param name="strDescripcionError"></param>
        public static void LoguearErrores(String strDescripcionError, String strTipo, String usuario, String Endpoint, int codigoErrorInterno = 200)
        {
            int i = 0;
            int max_intentos = 3;
            do
            {
                try
                {
                    using (StreamWriter mylogs = File.AppendText(System.IO.Directory.GetCurrentDirectory() + "\\logAPI.txt"))
                    {
                        mylogs.WriteLine(DateTime.Now.ToString() + "|" + strTipo + "|" + usuario + "|" + Endpoint + "|" + codigoErrorInterno + "|" + strDescripcionError);
                        mylogs.Close();
                        // Si la grabación es exitosa, establecer la variable i a 2.
                        i = max_intentos;
                    }
                }
                catch (Exception ex)
                {
                    // Si se produce un error, intenta escribirlo de nuevo.
                    i++;
                    System.Threading.Thread.Sleep(50);
                }
                // Esperar 50 ms antes de incrementar la variable.

            } while (i < max_intentos);

        }
    }
}
