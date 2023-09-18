using System.Net.NetworkInformation;

namespace API_Maestros_Core.BLL
{
    public class CategoriasMgr
    {
        public static GESI.CORE.BLL.SessionMgr _SessionMgr;


        /// <summary>
        /// Devuelve toda la lista de categorias de productos
        /// </summary>
        /// <returns></returns>
        public static List<GESI.ERP.Core.BO.cCategoriaDeProducto> GetList()
        {
            try
            {
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.CategoriasManager.ERPsessionManager = _SessionERP;
                GESI.ERP.Core.BLL.CategoriasManager.SessionManager = _SessionMgr;

                List<GESI.ERP.Core.BO.cCategoriaDeProducto> lstCategoriasProducto = GESI.ERP.Core.BLL.CategoriasManager.GetList();
            
                return lstCategoriasProducto;

            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        
        /// <summary>
        /// Devuelve la categoria enviada
        /// </summary>
        /// <param name="CategoriaID"></param>
        /// <returns></returns>
        public static GESI.ERP.Core.BO.cCategoriaDeProducto GetItem(String CategoriaID)
        {
            try
            {

                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.CategoriasManager.ERPsessionManager = _SessionERP;
                GESI.ERP.Core.BLL.CategoriasManager.SessionManager = _SessionMgr;

                List<GESI.ERP.Core.BO.cCategoriaDeProducto> lstCategoriasProducto = GESI.ERP.Core.BLL.CategoriasManager.GetList(CategoriaID);

                if(lstCategoriasProducto?.Count > 0)
                { 
                return lstCategoriasProducto[0];
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
}
