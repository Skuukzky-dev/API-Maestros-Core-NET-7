using API_Maestros_Core.Controllers;
using API_Maestros_Core.Models;
using GESI.CORE.BO.Verscom2k;
using GESI.GESI.BO;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Printing;


namespace API_Maestros_Core.BLL
{
    public class ProductosMgr
    {

        public static GESI.CORE.BLL.SessionMgr? _SessionMgr;

        /// <summary>
        /// Devuelve una lista de resultados de Busqueda
        /// </summary>
        /// <param name="strExpresionBusqueda"></param>
        /// <returns></returns>
        public static RespuestaConProductosHijos GetList(String strExpresionBusqueda, int[] CanalesDeVenta, string costoSolicitado, string costoUsuario,int pageNumber,int pageSize,string EstadosProductos,string CategoriasIDs)  // Usa GetSearchResults
        {
            try
            {
                #region ConnectionStrings
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                GESI.ERP.Core.BLL.cBASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
                #endregion

                #region Variables
                GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                RespuestaConProductosHijos oRespuesta = new RespuestaConProductosHijos();
                oRespuesta.error = new Error();
                List<HijoProductos> lstHijos = new List<HijoProductos>();
                List<GESI.ERP.Core.BO.cProducto> lstProductos = new List<GESI.ERP.Core.BO.cProducto>();
                #endregion

                if (CategoriasIDs.Length == 0)
                    CategoriasIDs = null;

                List<string> lstCodigosProducto = GESI.ERP.Core.BLL.ProductosManager.GetSearchResults(strExpresionBusqueda,strEstado:EstadosProductos,strCategorias:CategoriasIDs);                
                List<string> nuevosplit = lstCodigosProducto.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                string codigos = string.Join(",", nuevosplit);

                if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifico si tiene permisos para devolver costos
                {

                    lstProductos = GESI.ERP.Core.BLL.ProductosManager.GetList(codigos, CanalesDeVenta, costoSolicitado,EstadosProductos,CategoriasIDs);
                    
                    foreach (GESI.ERP.Core.BO.cProducto oPrd in lstProductos)
                    {
                        HijoProductos oHijo = new HijoProductos(oPrd);

                        if (oPrd.Categorias?.Count > 0)
                        {
                           foreach(GESI.ERP.Core.BO.cCategoriaXProducto oCategoria in oPrd.Categorias)
                           {
                                oHijo.ListaDeCategorias.Add(oCategoria.CategoriaID);
                           }
                        }

                        lstHijos.Add(oHijo);                                           

                    }
                    oRespuesta.success = true;
                }
                else
                {
                    lstProductos = GESI.ERP.Core.BLL.ProductosManager.GetList(codigos, CanalesDeVenta,"N", EstadosProductos, CategoriasIDs);                  
                    foreach (GESI.ERP.Core.BO.cProducto oPrd in lstProductos)
                    {
                        lstHijos.Add(new HijoProductos(oPrd));
                    }
                    oRespuesta.error.message = "Permiso denegado en la solicitud de costos";
                    oRespuesta.success = true;
                }

                oRespuesta.productos = lstHijos;               
                Paginacion oPaginacion = new Paginacion();
                oPaginacion.totalElementos = lstCodigosProducto.Count;
                oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                oPaginacion.paginaActual = pageNumber;
                oPaginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion = oPaginacion; 

                return oRespuesta;

            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetSearchResults. Descripcion: " + ex.Message,"E");
                throw;
            }
        }


        /// <summary>
        /// Devuelve todos los productos 
        /// </summary>        
        /// <returns></returns>
        public static RespuestaConProductosHijos GetList( int pageNumber, int pageSize, int[] CanalesDeVenta, string costoSolicitado,string costoUsuario,string EstadosProductos,string CategoriasIDs) // Usa GetList
        {
            try
            {
                RespuestaConProductosHijos oRespuesta = new RespuestaConProductosHijos();
                oRespuesta.paginacion = new Paginacion();

                #region ConnectionStrings
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                GESI.ERP.Core.BLL.cBASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
                #endregion

                #region Variables 
                GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                List<GESI.ERP.Core.BO.cProducto> lstProductos = new List<GESI.ERP.Core.BO.cProducto>();
                #endregion


                if (CategoriasIDs?.Length == 0)
                    CategoriasIDs = null;

                List<string> lstCodigosProducto = GESI.ERP.Core.BLL.ProductosManager.GetSearchResults(strEstado:EstadosProductos,strCategorias:CategoriasIDs);
                string commaSeparatedIds = string.Join(",", lstCodigosProducto);
                List<string> splits = commaSeparatedIds.Split(',').ToList();
                List<string> nuevosplit = splits.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                string resultado = string.Join(",", nuevosplit);

                if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifico si tiene permisos para devolver costos
                {
                    List<GESI.ERP.Core.BO.cProducto> lstProductosAux = GESI.ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, costoSolicitado,EstadosProductos,CategoriasIDs);
                    lstProductos = lstProductosAux;
                    oRespuesta.error = new Error();
                   
                }
                else
                {
                    List<GESI.ERP.Core.BO.cProducto> lstProductosAux = GESI.ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, "N", EstadosProductos, CategoriasIDs);
                    lstProductos = lstProductosAux;
                    oRespuesta.error = new Error();
                    oRespuesta.error.message = "Permiso denegado en la solicitud de costos";
                }

                #region Paginacion
                Paginacion oPaginacion = new Paginacion();
                oPaginacion.totalElementos = lstCodigosProducto.Count;
                oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                oPaginacion.paginaActual = pageNumber;
                oPaginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion = oPaginacion;
                #endregion

                    List<HijoProductos> lstHijos = new List<HijoProductos>();
                    foreach (GESI.ERP.Core.BO.cProducto oPrd in lstProductos)
                    {
                        oPrd.CostosProveedores = null;
                            HijoProductos oHijo = new HijoProductos(oPrd);

                            if (oPrd.Categorias?.Count > 0)
                            {
                                foreach (GESI.ERP.Core.BO.cCategoriaXProducto oCategoria in oPrd.Categorias)
                                {
                                    oHijo.ListaDeCategorias.Add(oCategoria.CategoriaID);
                                }
                            }

                            lstHijos.Add(oHijo);
                    }



                    oRespuesta.productos = lstHijos;
                    oRespuesta.success = true;                    

                    return oRespuesta;
             

            }
            catch (AccessViolationException ax)
            {
                Logger.LoguearErrores("Permiso denegado sobre costoSolicitado. Descripcion: " + ax.Message,"E");
                throw ax;
            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetList. Descripcion: " + ex.Message, "E");
                throw;
            }
        }


        /// <summary>
        /// Devuelve un producto con todos los datos
        /// </summary>
        /// <param name="ProductoID"></param>
        /// <param name="CanalesDeVenta"></param>
        /// <returns></returns>
        public static RespuestaProductosGetItem GetItem(String ProductoID, String CanalesDeVenta,int CanalDeVentaID,string costoSolicitado,string costoUsuario,string EstadosProductos,string CategoriasIDs) // Usa GetItem
        {
            try
            {
                RespuestaProductosGetItem oRespuesta = new RespuestaProductosGetItem();
                
                #region ConnectionStrings
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                GESI.ERP.Core.BLL.cBASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
                GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                #endregion

                #region Variables
                HijoProductos lstHijos = new HijoProductos();
                string[] canales = CanalesDeVenta.Split(',');
                int[] ints = Array.ConvertAll(canales, s => int.Parse(s));
                List<GESI.ERP.Core.BO.cProducto> oProduc = new List<GESI.ERP.Core.BO.cProducto>();
                #endregion

                if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifica si tiene permiso para devolver costos del proveedor
                {
                    #region Tiene permisos sobre costos                    
                    oProduc = GESI.ERP.Core.BLL.ProductosManager.GetList(ProductoID, ints, costoSolicitado,EstadosProductos,CategoriasIDs);
                    oRespuesta.error = new Error();
                    #endregion
                }
                else
                {                   
                    oProduc = GESI.ERP.Core.BLL.ProductosManager.GetList(ProductoID, ints, "N", EstadosProductos, CategoriasIDs);
                    oRespuesta.error = new Error();
                    oRespuesta.error.message = "Permiso denegado en la solicitud de costos";
                }
                    
                    if (CanalDeVentaID > 0)
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
                                else
                                {
                                    oProduc[0].Precios = new List<GESI.ERP.Core.BO.cPrecioProducto>();
                                }
                            }
                        }
                    }

                    if (oProduc?.Count > 0)
                    {
                        foreach (GESI.ERP.Core.BO.cProducto oPrd in oProduc)
                        {
                            oPrd.CostosProveedores = null;
                          //  lstHijos = new HijoProductos(oPrd);

                        HijoProductos oHijo = new HijoProductos(oPrd);

                        if (oPrd.Categorias?.Count > 0)
                        {
                            foreach (GESI.ERP.Core.BO.cCategoriaXProducto oCategoria in oPrd.Categorias)
                            {
                                oHijo.ListaDeCategorias.Add(oCategoria.CategoriaID);
                            }
                        }
                        
                        lstHijos = oHijo;




                        }
                    }                                    
                

                #region Paginacion

                Paginacion oPaginacion = new Paginacion();
                oPaginacion.totalPaginas = 1;
                oPaginacion.paginaActual = 1;
                oPaginacion.tamañoPagina = 1;
                
                oRespuesta.success = true;
                
                if (lstHijos?.ProductoID?.Length > 0)
                {
                    oRespuesta.producto = new HijoProductos();
                    oRespuesta.producto = lstHijos;

                    if (lstHijos?.ProductoID?.Length > 0)
                    {
                        oPaginacion.totalElementos = 1;
                    }
                    else
                    {
                        oPaginacion.totalElementos = 0;
                    }

                    oRespuesta.paginacion = oPaginacion;

                }
                else
                {
                    oRespuesta.producto = null;
                    oPaginacion.totalElementos = 0;
                    oRespuesta.error.code = 4041;
                    oRespuesta.error.message = "No se encontro el producto buscado";
                    oRespuesta.paginacion = oPaginacion;
                }
                oRespuesta.success = true;
                #endregion

                return oRespuesta;

            }
            catch(AccessViolationException ax )
            {
                Logger.LoguearErrores("Permiso denegado sobre costoSolicitado. Descripcion: " + ax.Message, "E");
                throw ax;
            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetItem. Descripcion: " + ex.Message, "E");
                throw;
            }
           
        }


        /// <summary>
        /// Verifica si el usuario tiene permisos para solicitar Costos Por Proveedor
        /// </summary>
        /// <param name="costoUsuario"></param>
        /// <param name="costoSolicitado"></param>
        /// <returns></returns>
        private static bool VerificarPermisoSobreCostos(string costoUsuario,string costoSolicitado)
        {
            bool Habilitado = true;
            switch(costoUsuario)
            {
                case "N":
                    switch(costoSolicitado)
                    {
                        case "P":
                            Habilitado = false;
                            break;
                        case "S":
                            Habilitado = false;
                            break;
                    }
                    break;
                case "P":
                    switch (costoSolicitado)
                    {                        
                        case "S":
                            Habilitado = false;
                            break;
                    }
                    break;
            }
            return Habilitado;
        }


    }

    
}
