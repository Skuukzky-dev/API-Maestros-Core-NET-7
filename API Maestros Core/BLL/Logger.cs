namespace API_Maestros_Core.BLL
{
    public class Logger
    {
        /// <summary>
        /// Loguea los casos exitosos y errores de la API
        /// </summary>
        /// <param name="strDescripcionError"></param>
        public static void LoguearErrores(String strDescripcionError)
        {
            try
            {
                using (StreamWriter mylogs = File.AppendText(System.IO.Directory.GetCurrentDirectory() + "\\logAPI.txt"))         //se crea el archivo
                {
                    mylogs.WriteLine(DateTime.Now.ToString() + "|" + strDescripcionError);
                    mylogs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
