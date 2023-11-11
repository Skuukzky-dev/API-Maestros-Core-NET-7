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
        public static List<CategoriaHija> GetList()
        {
            try
            {
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.CategoriasManager.ERPsessionManager = _SessionERP;
                GESI.ERP.Core.BLL.CategoriasManager.SessionManager = _SessionMgr;
                List<CategoriaHija> lstCategoriasHijas = new List<CategoriaHija>();
                List<GESI.ERP.Core.BO.cCategoriaDeProducto> lstCategoriasProducto = GESI.ERP.Core.BLL.CategoriasManager.GetList();
            
                if(lstCategoriasProducto?.Count > 0)
                {
                    foreach(GESI.ERP.Core.BO.cCategoriaDeProducto oCategoria in lstCategoriasProducto)
                    {
                        lstCategoriasHijas.Add(new CategoriaHija(oCategoria));
                    }
                }


                return lstCategoriasHijas;

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
        public static CategoriaHija GetItem(String CategoriaID)
        {
            try
            {

                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.CategoriasManager.ERPsessionManager = _SessionERP;
                GESI.ERP.Core.BLL.CategoriasManager.SessionManager = _SessionMgr;
                List<CategoriaHija> lstCategoriasHijas = new List<CategoriaHija>();
                List<GESI.ERP.Core.BO.cCategoriaDeProducto> lstCategoriasProducto = GESI.ERP.Core.BLL.CategoriasManager.GetList(CategoriaID);

                if(lstCategoriasProducto?.Count > 0)
                {
                  
                        foreach (GESI.ERP.Core.BO.cCategoriaDeProducto oCategoria in lstCategoriasProducto)
                        {
                            lstCategoriasHijas.Add(new CategoriaHija(oCategoria));
                        }

                    return lstCategoriasHijas[0];
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


        public class CategoriaHija : GESI.ERP.Core.BO.cCategoriaDeProducto
        {
            [System.Text.Json.Serialization.JsonIgnore]
            [Newtonsoft.Json.JsonIgnore]
            public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

            public CategoriaHija(GESI.ERP.Core.BO.cCategoriaDeProducto padre)
            {
                CategoriaID = padre.CategoriaID;
                Descripcion = padre.Descripcion;
                CategoriaPadreID = padre.CategoriaPadreID;
            }

            public CategoriaHija()
            {

            }
            
        }

    }
}
