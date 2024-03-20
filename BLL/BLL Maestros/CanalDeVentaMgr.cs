using GESI.CORE.API.BO;
using GESI.ERP.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BLL
{
    public class CanalesDeVentaMgr
    {
        public static CORE.BLL.SessionMgr _SessionMgr;

        /// <summary>
        /// Devuelve la respuesta sobre el endpoint api/CanalesDeVenta/GetList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static ResponseCanalesDeVenta GetListCanalesDeVenta(int[] CanalesDeVenta, int pageNumber = 1, int pageSize = 10, string referer = "")
        {
            ResponseCanalesDeVenta oRespuesta = null;
            try
            {
                APIHelper.SetearConnectionString();

                #region SessionManagers
                oRespuesta = new ResponseCanalesDeVenta();
                GESI.BLL.TablasGeneralesGESIMgr.SessionManager = _SessionMgr;
                ERP.Core.SessionManager ErpSessionMgr = new ERP.Core.SessionManager();
                ErpSessionMgr.EmpresaID = (uint)_SessionMgr.EmpresaID;
                ErpSessionMgr.UsuarioID = _SessionMgr.UsuarioID;
                #endregion

                oRespuesta.success = true;
                oRespuesta.error = new Error();
                oRespuesta.canalesDeVenta = new List<CanalDeVenta>();
                oRespuesta.paginacion = new Paginacion();
                List<ERP.Core.BO.cCanalDeVenta> lstCanalesDeVentaFinal = new List<cCanalDeVenta>();
                List<ERP.Core.BO.cCanalDeVenta> lstCanalesDeVenta = ErpSessionMgr.GetCanalesDeVentaHabilitados();

                for (int i = 0; i < CanalesDeVenta.Length; i++)
                {
                    List<cCanalDeVenta> CanalesDeVentaAuxiliar = lstCanalesDeVenta.Where(x => x.CanalDeVentaID == CanalesDeVenta[i]).ToList();

                    if (CanalesDeVentaAuxiliar.Count > 0)
                    {
                        lstCanalesDeVentaFinal.AddRange(CanalesDeVentaAuxiliar);
                    }
                }

                if (lstCanalesDeVentaFinal.Count > 0)
                {
                    lstCanalesDeVenta = lstCanalesDeVentaFinal;
                }

                if (lstCanalesDeVenta?.Count > 0)
                {
                    List<CanalDeVenta> lstCanalDeVentaAuxiliar = new List<CanalDeVenta>();
                    foreach (ERP.Core.BO.cCanalDeVenta oCanal in lstCanalesDeVenta)
                    {
                        lstCanalDeVentaAuxiliar.Add(new CanalDeVenta(oCanal));
                    }

                    oRespuesta.canalesDeVenta = lstCanalDeVentaAuxiliar.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    oRespuesta.paginacion.totalElementos = lstCanalesDeVenta.Count;
                    oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);
                    oRespuesta.paginacion.paginaActual = pageNumber;
                    oRespuesta.paginacion.tamañoPagina = pageSize;

                    Logger.LoguearErrores("Respuesta GetList Canales de venta OK. Referer: " + referer, "I", _SessionMgr.UsuarioID, APIHelper.CanalesDeVentaGetList);

                    return oRespuesta;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
