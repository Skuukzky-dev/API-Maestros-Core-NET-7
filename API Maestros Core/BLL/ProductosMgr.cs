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
        public static List<GESI.ERP.Core.BO.cProducto> GetList(String strExpresionBusqueda,int pageNumber, int pageSize)
        {
            try
            {
                GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                int[] marks = new int[] { 1, 3, 6};
                List<GESI.ERP.Core.BO.cProducto> lstProductos = new List<GESI.ERP.Core.BO.cProducto>();
                if (strExpresionBusqueda.Length <= 0)
                {
                    lstProductos = GESI.ERP.Core.BLL.ProductosManager.GetSearchResults();
                    string commaSeparatedIds = string.Join(",", lstProductos.Select(p => p.ProductoID));
                    List<string> splits = commaSeparatedIds.Split(',').ToList();
                    List<string> nuevosplit = splits.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    int blockSize = pageSize;
                    int blockIndex = pageNumber;

                    string resultado = string.Join(",", nuevosplit);

                    List<GESI.ERP.Core.BO.cProducto> lstProductosAux = GESI.ERP.Core.BLL.ProductosManager.GetList(resultado, marks, "S");

                    lstProductos = lstProductosAux;
                }
                else
                {
                    lstProductos = GESI.ERP.Core.BLL.ProductosManager.GetSearchResults(strExpresionBusqueda);
                }

               
                 // Indexing starts from 0, so the second block has an index of 1

             
                // Now, the 'secondBlock' list contains the second block of 50 elements from 'lstProductosBusqueda'
                //    commaSeparatedIds = "WLAC0034,CSC210";
                /* String strCodigos = "";
                 foreach(GESI.ERP.Core.BO.cProducto Prod in lstProductos)
                 {
                     strCodigos = strCodigos + Prod.ProductoID + ",";
                 }
                 strCodigos = strCodigos.Remove(strCodigos.Length - 1);*/
                

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
