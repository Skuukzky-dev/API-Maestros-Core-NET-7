using API_Maestros_Core.Controllers;
using API_Maestros_Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace API_Maestros_Core.BLL
{
    public class EmpresasMgr
    {
        public static APISessionManager _MiApiSessionMgr;

        
        /// <summary>
        /// Devuelve las Empresas Habiltiadas por Usuario
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static RespuestaEmpresas DevolverEmpresas(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                RespuestaEmpresas oRespuesta = new RespuestaEmpresas();

                List<GESI.CORE.BO.Empresa> lstEmpresas = GESI.CORE.BLL.EmpresasMgr.GetByUsuario(_MiApiSessionMgr.SessionMgr.UsuarioID);
                oRespuesta.Empresas = lstEmpresas;
                oRespuesta.error = new Error();
                oRespuesta.success = true;
                oRespuesta.paginacion = new Paginacion();
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);
                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.totalElementos = lstEmpresas.Count;

                return oRespuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Devuelve la Lista de Sucursales 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public static RespuestaSucursales DevolverSucursales(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                RespuestaSucursales oRespuesta = new RespuestaSucursales();
                List<SucursalHija> lstSucursalesFinales = new List<SucursalHija>();
                GESI.CORE.BLL.SucursalesMgr.SessionManager = _MiApiSessionMgr.SessionMgr;

                GESI.CORE.BO.ListaSucursales olstSucursales = GESI.CORE.BLL.SucursalesMgr.GetList();

                #region Pasaje a Sucursal Hija
                foreach (GESI.CORE.BO.Sucursal oSucursal in olstSucursales)
                {
                    SucursalHija oSucursalFinal = new SucursalHija(oSucursal);
                    lstSucursalesFinales.Add(oSucursalFinal);
                }
                #endregion

                oRespuesta.Sucursales = lstSucursalesFinales;

               
                    oRespuesta.error = new Error();
                    oRespuesta.success = true;
                    oRespuesta.paginacion = new Paginacion();
                    oRespuesta.paginacion.tamañoPagina = pageSize;
                    oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);
                    oRespuesta.paginacion.paginaActual = pageNumber;
                    oRespuesta.paginacion.totalElementos = lstSucursalesFinales.Count;

                    return oRespuesta;
              
            }
            catch(Exception ex) { throw ex; }
        }
    }
}
