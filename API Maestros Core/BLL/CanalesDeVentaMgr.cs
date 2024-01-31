using API_Maestros_Core.Models;
using GESI.ERP.Core.BO;
using System.Runtime.CompilerServices;

namespace API_Maestros_Core.BLL
{

    /// <summary>
    /// DOCUMENTACIÓN API:
    /// 
    /// https://docs.google.com/document/d/1DTp_-oqErewTyGkTvkW0FwsUmlVNlsy8iopBPZYQBrs/edit?usp=sharing
    /// 
    /// </summary>
    /// 

    public class CanalesDeVentaMgr
    {
        public static GESI.CORE.BLL.SessionMgr _SessionMgr;

        /// <summary>
        /// Devuelve la respuesta sobre el endpoint api/CanalesDeVenta/GetList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static RespuestaConCanalesDeVenta GetListCanalesDeVenta(int[] CanalesDeVenta,int pageNumber = 1,int pageSize = 10, string referer = "")
        {
            RespuestaConCanalesDeVenta oRespuesta = null;
            try
            {
                APIHelper.SetearConnectionString();

                #region SessionManagers
                oRespuesta = new RespuestaConCanalesDeVenta();
                GESI.GESI.BLL.TablasGeneralesGESIMgr.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager ErpSessionMgr = new GESI.ERP.Core.SessionManager();
                ErpSessionMgr.EmpresaID = (uint)_SessionMgr.EmpresaID;
                ErpSessionMgr.UsuarioID = _SessionMgr.UsuarioID;
                #endregion

                oRespuesta.success = true;
                oRespuesta.error = new Error();
                oRespuesta.CanalesDeVenta = new List<CanalDeVentaHijo>();
                oRespuesta.paginacion = new Paginacion();
                List<GESI.ERP.Core.BO.cCanalDeVenta> lstCanalesDeVentaFinal = new List<cCanalDeVenta>();
                List<GESI.ERP.Core.BO.cCanalDeVenta> lstCanalesDeVenta = ErpSessionMgr.GetCanalesDeVentaHabilitados();

                for(int i = 0;i<CanalesDeVenta.Length;i++)
                {
                    List<cCanalDeVenta> CanalesDeVentaAuxiliar = lstCanalesDeVenta.Where(x => x.CanalDeVentaID == CanalesDeVenta[i]).ToList();

                    if(CanalesDeVentaAuxiliar.Count > 0)
                    {
                        lstCanalesDeVentaFinal.AddRange(CanalesDeVentaAuxiliar);
                    }
                }

                if(lstCanalesDeVentaFinal.Count > 0 )
                {
                    lstCanalesDeVenta = lstCanalesDeVentaFinal;
                }

                if (lstCanalesDeVenta?.Count > 0)
                {
                    List<CanalDeVentaHijo> lstCanalDeVentaAuxiliar = new List<CanalDeVentaHijo>();
                    foreach(GESI.ERP.Core.BO.cCanalDeVenta oCanal in lstCanalesDeVenta)
                    {
                        lstCanalDeVentaAuxiliar.Add(new CanalDeVentaHijo(oCanal));
                    }

                    oRespuesta.CanalesDeVenta = lstCanalDeVentaAuxiliar.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    oRespuesta.paginacion.totalElementos = lstCanalesDeVenta.Count;
                    oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);
                    oRespuesta.paginacion.paginaActual = pageNumber;
                    oRespuesta.paginacion.tamañoPagina = pageSize;
                    
                    Logger.LoguearErrores("Respuesta GetList Canales de venta OK. Referer: "+referer, "I", _SessionMgr.UsuarioID, APIHelper.CanalesDeVentaGetList);

                    return oRespuesta;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

      
    }

    public class CanalDeVentaHijo : GESI.ERP.Core.BO.cCanalDeVenta
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override ushort EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public CanalDeVentaHijo(GESI.ERP.Core.BO.cCanalDeVenta padre)
        {
            CanalDeVentaID = padre.CanalDeVentaID;
            Predeterminado = padre.Predeterminado;
            Descripcion = padre.Descripcion;
            PreciosConIVA = padre.PreciosConIVA;            
        }

    }

}
