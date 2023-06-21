using API_Maestros_Core.Controllers;
using GESI.GESI.BO;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;

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
        public static List<HijoProductos> GetList(String strExpresionBusqueda,int pageNumber, int pageSize)
        {
            try
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                GESI.ERP.Core.BLL.cBASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;


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

                List<HijoProductos> lstHijos = new List<HijoProductos>();
                foreach(GESI.ERP.Core.BO.cProducto oPrd in lstProductos)
                {
                    lstHijos.Add(new HijoProductos(oPrd));
                }

               

                return lstHijos;

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
        public static List<HijoProductos> GetItem(String ProductoID, String CanalesDeVenta,int CanalDeVentaID)
        {
            try
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                GESI.ERP.Core.BLL.cBASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;

                GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                

                string[] canales = CanalesDeVenta.Split(',');
                int[] ints = Array.ConvertAll(canales, s => int.Parse(s));
                List<HijoProductos> lstHijos = new List<HijoProductos>();
                List<GESI.ERP.Core.BO.cProducto> oProduc = GESI.ERP.Core.BLL.ProductosManager.GetList(ProductoID, ints, "S");
                if(CanalDeVentaID > 0)
                {
                    if (oProduc.Count > 0)
                    {
                        if (oProduc[0].Precios?.Count > 0)
                        {
                            List<GESI.ERP.Core.BO.cPrecioProducto> lstPrecioProducto = oProduc[0].Precios.Where(x => x.CanalDeVenta == CanalDeVentaID).ToList();
                            if (lstPrecioProducto.Count > 0)
                            {
                                oProduc[0].Precios = new List<GESI.ERP.Core.BO.cPrecioProducto>();
                                oProduc[0].Precios.AddRange(lstPrecioProducto);
                            }
                        }
                    }
                }

                if(oProduc?.Count > 0)
                {
                    foreach (GESI.ERP.Core.BO.cProducto oPrd in oProduc)
                    {
                        lstHijos.Add(new HijoProductos(oPrd));
                    }
                }

                return lstHijos;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }

    
}
