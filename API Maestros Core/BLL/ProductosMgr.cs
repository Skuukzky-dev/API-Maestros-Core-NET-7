namespace API_Maestros_Core.BLL
{
    public class ProductosMgr
    {

        public static GESI.CORE.BLL.SessionMgr _SessionMgr;

        /// <summary>
        /// Devuelve una lista de resultados de Busqueda
        /// </summary>
        /// <param name="strExpresionBusqueda"></param>
        /// <returns></returns>
        public static List<GESI.ERP.Core.BO.cProducto> GetList(String strExpresionBusqueda)
        {
            try
            {
                GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                List<GESI.ERP.Core.BO.cProducto> lstProductos = GESI.ERP.Core.BLL.ProductosManager.GetSearchResults(strExpresionBusqueda);

                return lstProductos;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Devuelve un producto con todos los datos
        /// </summary>
        /// <param name="ProductoID"></param>
        /// <param name="CanalesDeVenta"></param>
        /// <returns></returns>
        public static GESI.ERP.Core.BO.cProducto GetItem(String ProductoID, String CanalesDeVenta,int CanalDeVentaID)
        {
            try
            {
                GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;


                string[] canales = CanalesDeVenta.Split(',');
                int[] ints = Array.ConvertAll(canales, s => int.Parse(s));
                GESI.ERP.Core.BO.cProducto oProduc = GESI.ERP.Core.BLL.ProductosManager.GetItem(ProductoID, ints);

                if(CanalDeVentaID > 0)
                {
                    List<GESI.ERP.Core.BO.cPrecioProducto> lstPrecioProducto = oProduc.Precios.Where(x => x.CanalDeVenta == CanalDeVentaID).ToList();
                    if(lstPrecioProducto.Count > 0)
                    {
                        oProduc.Precios = new List<GESI.ERP.Core.BO.cPrecioProducto>();
                        oProduc.Precios.AddRange(lstPrecioProducto);
                    }
                }

                return oProduc;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
