using GESI.CORE.API.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BLL
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
        public static ResponseEmpresas DevolverEmpresas(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                ResponseEmpresas oRespuesta = new ResponseEmpresas();

                List<CORE.BO.Empresa> lstEmpresas = CORE.BLL.EmpresasMgr.GetByUsuario(_MiApiSessionMgr.SessionMgr.UsuarioID);
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
        public static ResponseSucursales DevolverSucursales(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                ResponseSucursales oRespuesta = new ResponseSucursales();
                List<Sucursal> lstSucursalesFinales = new List<Sucursal>();
                CORE.BLL.SucursalesMgr.SessionManager = _MiApiSessionMgr.SessionMgr;

                CORE.BO.ListaSucursales olstSucursales = CORE.BLL.SucursalesMgr.GetList();

                #region Pasaje a Sucursal Hija
                foreach (CORE.BO.Sucursal oSucursal in olstSucursales)
                {
                    Sucursal oSucursalFinal = new Sucursal(oSucursal);
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
            catch (Exception ex) { throw ex; }
        }
    }
}
