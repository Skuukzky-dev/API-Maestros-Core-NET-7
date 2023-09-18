using API_Maestros_Core.Controllers;
using API_Maestros_Core.Models;
using GESI.CORE.BO;

namespace API_Maestros_Core.BLL
{
    public class ImagenesMgr
    {
        private static string strMotivoRechazo = "";

        /// <summary>
        /// Graba la Imagen en la base de datos
        /// </summary>
        /// <param name="oImagen"></param>
        /// <returns></returns>
        public static RespuestaImagen Save(ImagenProducto oImagen)
        {
            RespuestaImagen oRespuesta = new RespuestaImagen();
            oRespuesta.error = new Error();
            try
            {
                if(!VerificarTamanoImagen(oImagen))  // Supera el tamaño 256KB
                {
                    oRespuesta.success = false;
                    oRespuesta.error.code = 400;
                    oRespuesta.error.message = "El tamaño de la imagen no debe superar los 256kb";
                }
                else // Esta OK
                { 
                    // Aca agregar metodo de Jorge para grabar Imagen 
                    oRespuesta.success = true;
                    oRespuesta.message = "Se subio correctamente la imagen del producto";
                }
            }
            catch(Exception ex)
            {
                oRespuesta.success = false;
                oRespuesta.error.message = ex.Message;
            }
            return oRespuesta;
        }

        /// <summary>
        /// Verifica si el tamaño de la imagen no supera los 256 KB
        /// </summary>
        /// <param name="oImagen"></param>
        /// <returns></returns>

        private static bool VerificarTamanoImagen(ImagenProducto oImagen)
        {
            int longitudBytes = oImagen.imagen.Length;
            double longitudKB = (double)longitudBytes / 1024;

            if(longitudKB > 256)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
