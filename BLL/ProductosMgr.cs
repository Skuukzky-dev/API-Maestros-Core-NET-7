using GESI.CORE.API.BO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BLL
{
    public class ProductosMgr
    {


        public static CORE.BLL.SessionMgr? _SessionMgr;
        public static List<TipoDeError> lstTipoErrores;
        public static TipoDeError oTipo;

        enum LogCambios
        {
            tTablasGenerales = 310,
            tPrecios = 312
        }

        /// <summary>
        /// Devuelve una lista de resultados de Busqueda
        /// </summary>
        /// <param name="strExpresionBusqueda"></param>
        /// <returns></returns>
        public static ResponseProductos GetList(String strExpresionBusqueda, int[] CanalesDeVenta, string costoSolicitado, string costoUsuario, int pageNumber, int pageSize, string EstadosProductos, string CategoriasIDs, string imagenes, string stock = "N", int[] Almacenes = null, string publicaEcommerce = "T", int canalDeVentaID = 0, string orden = "O")  // Usa GetSearchResults
        {
            try
            {

                #region ConnectionStrings
                APIHelper.SetearConnectionString();
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                // SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                // GESI.ERP.Core.BLL.BASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
                #endregion

                #region Variables
                ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                ERP.Core.SessionManager _SessionERP = new ERP.Core.SessionManager();
                ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                ResponseProductos oRespuesta = new ResponseProductos();
                oRespuesta.error = new Error();
                List<Producto> lstHijos = new List<Producto>();
                List<ERP.Core.BO.cProducto> lstProductos = new List<ERP.Core.BO.cProducto>();
                #endregion

                if (CategoriasIDs.Length == 0)
                    CategoriasIDs = null;

                APIHelper.QueEstabaHaciendo = "Obteniendo lista de codigos de la expresion";

                int? publicaEcommerceint = FiltroEcommerce(publicaEcommerce);
                strExpresionBusqueda = Uri.UnescapeDataString(strExpresionBusqueda);
                List<string> lstCodigosProducto = ERP.Core.BLL.ProductosManager.GetSearchResults(strExpresionBusqueda, strEstado: EstadosProductos, strCategorias: CategoriasIDs, intPublicaECommerce: (ushort?)publicaEcommerceint);

                if (canalDeVentaID == 0)
                {
                    List<string> nuevosplit = lstCodigosProducto.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    string codigos = string.Join(",", nuevosplit);

                    APIHelper.QueEstabaHaciendo = "Verificando permisos en costos";

                    if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifico si tiene permisos para devolver costos
                    {
                        APIHelper.QueEstabaHaciendo = "Obteniendo productos";

                        lstProductos = ERP.Core.BLL.ProductosManager.GetList(codigos, CanalesDeVenta, costoSolicitado, EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);

                        foreach (ERP.Core.BO.cProducto oPrd in lstProductos)
                        {
                            Producto oHijo = new Producto(oPrd);

                            if (oPrd.Categorias?.Count > 0)
                            {
                                foreach (ERP.Core.BO.cCategoriaXProducto oCategoria in oPrd.Categorias)
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

                        lstProductos = ERP.Core.BLL.ProductosManager.GetList(codigos, CanalesDeVenta, "N", EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);

                        foreach (ERP.Core.BO.cProducto oPrd in lstProductos)
                        {
                            lstHijos.Add(new Producto(oPrd));
                        }

                        oTipo = lstTipoErrores.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cPermisoDenegadoCostos);
                        oRespuesta.error.message = oTipo.DescripcionError;
                        oRespuesta.error.code = (int)APIHelper.cCodigosError.cPermisoDenegadoCostos;
                        oRespuesta.success = true;
                    }
                }
                else
                {
                    #region ORDEN
                    string codigos = string.Join(",", lstCodigosProducto);
                    if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifico si tiene permisos para devolver costos
                    {
                        APIHelper.QueEstabaHaciendo = "Obteniendo productos";

                        lstProductos = ERP.Core.BLL.ProductosManager.GetList(codigos, new int[] { canalDeVentaID }, costoSolicitado, EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);

                        lstProductos = lstProductos.OrderBy(x => x.Precios?.FirstOrDefault()?.PrecioFinal).ToList();
                        lstProductos = lstProductos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                        foreach (ERP.Core.BO.cProducto oPrd in lstProductos)
                        {
                            Producto oHijo = new Producto(oPrd);

                            if (oPrd.Categorias?.Count > 0)
                            {
                                foreach (ERP.Core.BO.cCategoriaXProducto oCategoria in oPrd.Categorias)
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

                        lstProductos = ERP.Core.BLL.ProductosManager.GetList(codigos, new int[] { canalDeVentaID }, "N", EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);

                        lstProductos = lstProductos.OrderBy(x => x.Precios?.FirstOrDefault()?.PrecioFinal).ToList();
                        lstProductos = lstProductos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                        foreach (ERP.Core.BO.cProducto oPrd in lstProductos)
                        {
                            lstHijos.Add(new Producto(oPrd));
                        }

                        oTipo = lstTipoErrores.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cPermisoDenegadoCostos);
                        oRespuesta.error.message = oTipo.DescripcionError;
                        oRespuesta.error.code = (int)APIHelper.cCodigosError.cPermisoDenegadoCostos;
                        oRespuesta.success = true;
                    }
                    #endregion
                }

                APIHelper.QueEstabaHaciendo = "Completando paginado";

                oRespuesta.productos = lstHijos;
                Paginacion oPaginacion = new Paginacion();
                oPaginacion.totalElementos = lstCodigosProducto.Count;
                oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                oPaginacion.paginaActual = pageNumber;
                oPaginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion = oPaginacion;
                int cantidadreg = 0;

                if (lstCodigosProducto?.Count > 0)
                {
                    cantidadreg = lstCodigosProducto.Count;


                }

                Logger.LoguearErrores("GetSearchResults: Exitoso Expresion:" + strExpresionBusqueda + " Cantidad: " + cantidadreg, "I", _SessionMgr.UsuarioID, APIHelper.ProductosGetSearchResult);
                return oRespuesta;

            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetSearchResults. Descripcion: " + ex.Message + " . Haciendo: " + APIHelper.QueEstabaHaciendo, "E", _SessionMgr.UsuarioID, APIHelper.ProductosGetSearchResult, (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                throw;
            }
        }


        /// <summary>
        /// Devuelve todos los productos 
        /// </summary>        
        /// <returns></returns>
        public static ResponseProductos GetList(int pageNumber, int pageSize, int[] CanalesDeVenta, string costoSolicitado, string costoUsuario, string EstadosProductos, string CategoriasIDs, string imagenes, string fechamodificaciones = "", string stock = "N", int[] Almacenes = null, string publicaEcommerce = "T") // Usa GetList
        {
            try
            {
                APIHelper.SetearConnectionString();
                ResponseProductos oRespuesta = new ResponseProductos();
                oRespuesta.paginacion = new Paginacion();

                #region ConnectionStrings
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                //   GESI.ERP.Core.BLL.ProductosManager.ConnectionStringEstoEstaMal = ConnectionString;
                // GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                //   GESI.ERP.Core.BLL.BASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
                #endregion

                #region Variables 
                ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                ERP.Core.SessionManager _SessionERP = new ERP.Core.SessionManager();
                ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                List<ERP.Core.BO.cProducto> lstProductos = new List<ERP.Core.BO.cProducto>();
                List<string> lstCodigosProducto = new List<string>();
                #endregion


                if (CategoriasIDs?.Length == 0)
                    CategoriasIDs = null;



                if (fechamodificaciones?.Length > 0)
                {
                    try
                    {
                        DateTime Fecha = DateTime.Parse(fechamodificaciones);
                        APIHelper.QueEstabaHaciendo = "Obteniendo productos modificados a partir de fecha";
                        lstCodigosProducto = ERP.Core.BLL.ProductosManager.GetProductosModificadosDesdeFecha(Fecha, EstadosProductos, (int)LogCambios.tTablasGenerales);

                        string commaSeparatedIds = string.Join(",", lstCodigosProducto);
                        List<string> splits = commaSeparatedIds.Split(',').ToList();
                        List<string> nuevosplit = splits.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                        string resultado = string.Join(",", nuevosplit);


                        APIHelper.QueEstabaHaciendo = "Verificando permisos sobre costos";

                        if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifico si tiene permisos para devolver costos
                        {
                            APIHelper.QueEstabaHaciendo = "Obteniendo listado de productos";

                            List<ERP.Core.BO.cProducto> lstProductosAux = ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, costoSolicitado, EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);

                            lstProductos = lstProductosAux;
                            oRespuesta.error = new Error();
                        }
                        else
                        {
                            APIHelper.QueEstabaHaciendo = "Obteniendo listado de productos";
                            oTipo = lstTipoErrores.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cPermisoDenegadoCostos);
                            List<ERP.Core.BO.cProducto> lstProductosAux = ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, "N", EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                            lstProductos = lstProductosAux;
                            oRespuesta.error = new Error();
                            oRespuesta.error.message = oTipo.DescripcionError;
                            oRespuesta.error.code = (int)APIHelper.cCodigosError.cPermisoDenegadoCostos;
                        }
                    }
                    catch (FormatException fex)
                    {
                        throw fex;
                    }
                }
                else
                {
                    APIHelper.QueEstabaHaciendo = "Buscando resultados de productos";
                    int? publica = FiltroEcommerce(publicaEcommerce);
                    lstCodigosProducto = ERP.Core.BLL.ProductosManager.GetSearchResults(strEstado: EstadosProductos, strCategorias: CategoriasIDs, intPublicaECommerce: (ushort?)publica);
                    string commaSeparatedIds = string.Join(",", lstCodigosProducto);
                    List<string> splits = commaSeparatedIds.Split(',').ToList();
                    List<string> nuevosplit = splits.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    string resultado = string.Join(",", nuevosplit);

                    APIHelper.QueEstabaHaciendo = "Verificando permisos sobre costos";

                    if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifico si tiene permisos para devolver costos
                    {
                        APIHelper.QueEstabaHaciendo = "Obteniendo productos";
                        List<ERP.Core.BO.cProducto> lstProductosAux = ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, costoSolicitado, EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                        lstProductos = lstProductosAux;
                        oRespuesta.error = new Error();

                    }
                    else
                    {
                        APIHelper.QueEstabaHaciendo = "Obteniendo productos";
                        oTipo = lstTipoErrores.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cPermisoDenegadoCostos);
                        List<ERP.Core.BO.cProducto> lstProductosAux = ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, "N", EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                        lstProductos = lstProductosAux;
                        oRespuesta.error = new Error();
                        oRespuesta.error.message = oTipo.DescripcionError;
                        oRespuesta.error.code = (int)APIHelper.cCodigosError.cPermisoDenegadoCostos;
                    }
                }

                #region Paginacion
                Paginacion oPaginacion = new Paginacion();
                oPaginacion.totalElementos = lstCodigosProducto.Count;
                oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                oPaginacion.paginaActual = pageNumber;
                oPaginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion = oPaginacion;
                #endregion

                List<Producto> lstHijos = new List<Producto>();
                foreach (ERP.Core.BO.cProducto oPrd in lstProductos)
                {
                    oPrd.CostosProveedores = null;
                    Producto oHijo = new Producto(oPrd);

                    if (oPrd.Categorias?.Count > 0)
                    {
                        foreach (ERP.Core.BO.cCategoriaXProducto oCategoria in oPrd.Categorias)
                        {
                            oHijo.ListaDeCategorias.Add(oCategoria.CategoriaID);
                        }
                    }

                    lstHijos.Add(oHijo);
                }



                oRespuesta.productos = lstHijos;
                oRespuesta.success = true;

                Logger.LoguearErrores("GetList: Existoso", "I", _SessionMgr.UsuarioID, APIHelper.ProductosGetList);

                return oRespuesta;


            }
            catch (FormatException fex)
            {
                throw fex;
            }
            catch (AccessViolationException ax)
            {
                Logger.LoguearErrores("Permiso denegado sobre costoSolicitado. Descripcion: " + ax.Message, "E", _SessionMgr.UsuarioID, APIHelper.ProductosGetList, (int)APIHelper.cCodigosError.cPermisoDenegadoCostos);
                throw ax;
            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetList. Descripcion: " + ex.Message + " Haciendo: " + APIHelper.QueEstabaHaciendo, "E", _SessionMgr.UsuarioID, APIHelper.ProductosGetList, (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                throw;
            }
        }


        /// <summary>
        /// Devuelve un producto con todos los datos
        /// </summary>
        /// <param name="ProductoID"></param>
        /// <param name="CanalesDeVenta"></param>
        /// <returns></returns>
        public static ResponseProducto GetItem(String ProductoID, int[] CanalesDeVenta, int CanalDeVentaID, string costoSolicitado, string costoUsuario, string EstadosProductos, string CategoriasIDs, string imagenes, string stock = "N", int[] Almacenes = null) // Usa GetItem
        {
            try
            {
                APIHelper.SetearConnectionString();
                ResponseProducto oRespuesta = new ResponseProducto();

                #region ConnectionStrings
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);


                ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                ERP.Core.SessionManager _SessionERP = new ERP.Core.SessionManager();
                ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                #endregion

                #region Variables
                Producto lstHijos = new Producto();
                List<ERP.Core.BO.cProducto> oProduc = new List<ERP.Core.BO.cProducto>();
                #endregion

                APIHelper.QueEstabaHaciendo = "Verificando permisos sobre costos";

                if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifica si tiene permiso para devolver costos del proveedor
                {
                    #region Tiene permisos sobre costos                    
                    oProduc = ERP.Core.BLL.ProductosManager.GetList(ProductoID, CanalesDeVenta, costoSolicitado, EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                    oRespuesta.error = new Error();
                    #endregion
                }
                else
                {
                    APIHelper.QueEstabaHaciendo = "Obteniendo listado de productos";
                    oTipo = lstTipoErrores.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cPermisoDenegadoCostos);
                    oProduc = ERP.Core.BLL.ProductosManager.GetList(ProductoID, CanalesDeVenta, "N", EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                    oRespuesta.error = new Error();
                    oRespuesta.error.message = oTipo.DescripcionError;
                    oRespuesta.error.code = (int)APIHelper.cCodigosError.cPermisoDenegadoCostos;
                }

                if (CanalDeVentaID > 0)
                {
                    if (oProduc.Count > 0)
                    {
                        if (oProduc[0].Precios?.Count > 0)
                        {
                            List<ERP.Core.BO.cPrecioProducto> lstPrecioProducto = oProduc[0].Precios.Where(x => x.CanalDeVenta == CanalDeVentaID).ToList();
                            if (lstPrecioProducto.Count > 0)
                            {
                                oProduc[0].Precios = new List<ERP.Core.BO.cPrecioProducto>();
                                oProduc[0].Precios.AddRange(lstPrecioProducto);
                            }
                            else
                            {
                                oProduc[0].Precios = new List<ERP.Core.BO.cPrecioProducto>();
                            }
                        }
                    }
                }

                if (oProduc?.Count > 0)
                {
                    foreach (ERP.Core.BO.cProducto oPrd in oProduc)
                    {
                        oPrd.CostosProveedores = null;
                        //  lstHijos = new HijoProductos(oPrd);

                        Producto oHijo = new Producto(oPrd);

                        if (oPrd.Categorias?.Count > 0)
                        {
                            foreach (ERP.Core.BO.cCategoriaXProducto oCategoria in oPrd.Categorias)
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
                    oRespuesta.producto = new Producto();
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
                    oRespuesta.error.code = 2041;
                    oRespuesta.error.message = "No se encontro el producto buscado";
                    oRespuesta.paginacion = oPaginacion;
                }
                oRespuesta.success = true;
                #endregion

                Logger.LoguearErrores("GetItem: Exitoso ProductoID: " + ProductoID, "I", _SessionMgr.UsuarioID, APIHelper.ProductosGetItem);
                return oRespuesta;

            }
            catch (AccessViolationException ax)
            {
                Logger.LoguearErrores("Permiso denegado sobre costoSolicitado. Descripcion: " + ax.Message, "E", _SessionMgr.UsuarioID, APIHelper.ProductosGetItem, (int)APIHelper.cCodigosError.cPermisoDenegadoCostos);
                throw ax;
            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetItem. Descripcion: " + ex.Message + " .Haciendo: " + APIHelper.QueEstabaHaciendo, "E", _SessionMgr.UsuarioID, APIHelper.ProductosGetItem, (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                throw;
            }

        }

        /// <summary>
        /// Devuelve las existencias por productos
        /// </summary>
        /// <param name="codigos"></param>
        /// <param name="Almacenes"></param>
        public static ResponseExistencias GetExistencias(string codigos, int pageNumber = 1, int pageSize = 10, int[] Almacenes = null)
        {
            APIHelper.SetearConnectionString();
            ResponseExistencias oRespuesta = new ResponseExistencias();
            #region ConnectionStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
            ERP.Core.SessionManager _SessionERP = new ERP.Core.SessionManager();
            ERP.Core.BLL.ExistenciasManager.SessionManager = _SessionMgr;
            ERP.Core.BLL.ExistenciasManager.ERPsessionManager = _SessionERP;
            #endregion


            try
            {
                List<string> lstCodigosProducto = codigos.Split(",").ToList();

                string AlmacenesIDs = null;
                if (Almacenes?.Length > 0)
                    AlmacenesIDs = string.Join(",", Almacenes);

                List<ERP.Core.BO.cExistenciaProducto> lstExistencias = ERP.Core.BLL.ExistenciasManager.GetExistenciaProductos(codigos, AlmacenesIDs);
                List<Existencia> lstExistenciasHijas = new List<Existencia>();

                foreach (ERP.Core.BO.cExistenciaProducto oExistencia in lstExistencias)
                {
                    Existencia oExHija = new Existencia(oExistencia);
                    lstExistenciasHijas.Add(oExHija);
                }


                if (lstExistencias?.Count > 0)
                {
                    if (lstExistencias.Count > 1)
                    {
                        Paginacion oPaginacion = new Paginacion();
                        oPaginacion.totalElementos = lstExistencias.Count;
                        lstExistencias = lstExistencias.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                        oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                        oPaginacion.paginaActual = pageNumber;
                        oPaginacion.tamañoPagina = pageSize;
                        oRespuesta.paginacion = oPaginacion;
                    }
                    else
                    {
                        Paginacion oPaginacion = new Paginacion();
                        oPaginacion.totalPaginas = 1;
                        oPaginacion.paginaActual = 1;
                        oPaginacion.tamañoPagina = 1;
                        oPaginacion.totalElementos = 1;
                        oRespuesta.success = true;
                        oRespuesta.paginacion = oPaginacion;
                    }

                    oRespuesta.existencias = lstExistenciasHijas;
                }
                else
                {
                    Paginacion oPaginacion = new Paginacion();
                    oPaginacion.totalPaginas = 1;
                    oPaginacion.paginaActual = 1;
                    oPaginacion.tamañoPagina = 1;
                    oPaginacion.totalElementos = 0;
                    oRespuesta.success = true;
                    oRespuesta.existencias = null;
                }
                oRespuesta.error = new Error();
                Logger.LoguearErrores("GetExistencias:Exitoso. Codigos: " + codigos, "I", _SessionMgr.UsuarioID, APIHelper.ProductosGetExistencias);
                return oRespuesta;
            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetExistencias. Descripcion: " + ex.Message + " .Haciendo: " + APIHelper.QueEstabaHaciendo, "E", _SessionMgr.UsuarioID, APIHelper.ProductosGetExistencias, (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                throw;
            }
        }


        /// <summary>
        /// Devuelve los precios por productoS
        /// </summary>
        /// <param name="codigos"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="CanalesDeVenta"></param>
        public static ResponsePrecios GetPrecios(string codigos, int[] CanalesDeVenta, int pageNumber = 1, int pageSize = 10, string fechamodificaciones = "", string EstadosProductos = "A")
        {
            APIHelper.SetearConnectionString();
            ResponsePrecios oRespuesta = new ResponsePrecios();
            #region ConnectionStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
            ERP.Core.SessionManager _SessionERP = new ERP.Core.SessionManager();
            ERP.Core.BLL.ExistenciasManager.SessionManager = _SessionMgr;
            ERP.Core.BLL.ExistenciasManager.ERPsessionManager = _SessionERP;
            #endregion

            try
            {
                if (fechamodificaciones?.Length > 0)
                {
                    try
                    {
                        DateTime Fecha = DateTime.Parse(fechamodificaciones);
                        List<string> lstCodigosProducto = ERP.Core.BLL.ProductosManager.GetProductosModificadosDesdeFecha(Fecha, EstadosProductos, (int)LogCambios.tPrecios);
                        string combinedString = string.Join(",", lstCodigosProducto);
                        List<ERP.Core.BO.cProducto> lstProductos = ERP.Core.BLL.ProductosManager.GetList(combinedString, CanalesDeVenta, "N", "A", "", "N", "N");
                        oRespuesta = DeterminarPreciosGetPrecios(combinedString, CanalesDeVenta, pageNumber, pageSize, "N", "A", "", "N");
                    }
                    catch (FormatException fex)
                    {
                        throw fex;
                    }
                }
                else
                {
                    if (codigos?.Length > 0)
                    {
                        #region Detalla Codigos a buscar
                        List<string> lstCodigosProducto = codigos.Split(",").ToList();
                        oRespuesta = DeterminarPreciosGetPrecios(codigos, CanalesDeVenta, pageNumber, pageSize, "N", "A", "", "N");
                        #endregion
                    }
                    else
                    {
                        Paginacion oPaginacion = new Paginacion();
                        oPaginacion.totalPaginas = 1;
                        oPaginacion.paginaActual = 1;
                        oPaginacion.tamañoPagina = 1;
                        oPaginacion.totalElementos = 0;
                        oRespuesta.success = true;
                        oRespuesta.Precios = null;
                    }
                }

                oRespuesta.error = new Error();

                Logger.LoguearErrores("GetPrecios:Exitoso . Codigos " + codigos, "I", _SessionMgr.UsuarioID, APIHelper.ProductoGetPrecios);
                return oRespuesta;
            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetPrecios. Descripcion: " + ex.Message, "E", _SessionMgr.UsuarioID, APIHelper.ProductoGetPrecios, (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                throw ex;
            }
        }

        /// <summary>
        /// Devuelve la respuesta con los precios de los productos
        /// </summary>
        /// <param name="codigos"></param>
        /// <param name="CanalesDeVenta"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="DevolverCostos"></param>
        /// <param name="EstadoProductos"></param>
        /// <param name="categoriasIDs"></param>
        /// <param name="DevuelveImagenes"></param>
        /// <returns></returns>
        private static ResponsePrecios DeterminarPreciosGetPrecios(string codigos, int[] CanalesDeVenta, int pageNumber, int pageSize, string DevolverCostos = "N", string EstadoProductos = "A", string categoriasIDs = "", string DevuelveImagenes = "N")
        {
            try
            {
                APIHelper.SetearConnectionString();

                ResponsePrecios oRespuesta = new ResponsePrecios();
                List<ERP.Core.BO.cProducto> lstProductos = ERP.Core.BLL.ProductosManager.GetList(codigos, CanalesDeVenta, DevolverCostos, EstadoProductos, categoriasIDs, DevuelveImagenes);
                List<ERP.Core.BO.cPrecioProducto> lstPrecios = new List<ERP.Core.BO.cPrecioProducto>();
                #region Lleno la lista de precios

                foreach (ERP.Core.BO.cProducto oProducto in lstProductos)
                {
                    lstPrecios.AddRange(oProducto.Precios);
                }
                #endregion

                if (lstProductos?.Count > 0)
                {
                    if (lstProductos?.Count > 1)
                    {
                        Paginacion oPaginacion = new Paginacion();
                        oPaginacion.totalElementos = lstPrecios.Count;
                        lstPrecios = lstPrecios.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                        oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                        oPaginacion.paginaActual = pageNumber;
                        oPaginacion.tamañoPagina = pageSize;
                        oRespuesta.paginacion = oPaginacion;
                        oRespuesta.Precios = lstPrecios;
                        oRespuesta.success = true;

                    }
                    else
                    {
                        Paginacion oPaginacion = new Paginacion();
                        oPaginacion.totalPaginas = 1;
                        oPaginacion.paginaActual = 1;
                        oPaginacion.tamañoPagina = 1;
                        oPaginacion.totalElementos = lstPrecios.Count;
                        oRespuesta.Precios = lstPrecios;
                        oRespuesta.success = true;
                        oRespuesta.paginacion = oPaginacion;
                    }
                }
                else
                {
                    Paginacion oPaginacion = new Paginacion();
                    oPaginacion.totalPaginas = 1;
                    oPaginacion.paginaActual = 1;
                    oPaginacion.tamañoPagina = 1;
                    oPaginacion.totalElementos = 0;
                    oRespuesta.success = true;
                    oRespuesta.Precios = null;
                }
                return oRespuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Verifica si el usuario tiene permisos para solicitar Costos Por Proveedor
        /// </summary>
        /// <param name="costoUsuario"></param>
        /// <param name="costoSolicitado"></param>
        /// <returns></returns>
        private static bool VerificarPermisoSobreCostos(string costoUsuario, string costoSolicitado)
        {
            bool Habilitado = true;
            switch (costoUsuario)
            {
                case "N":
                    switch (costoSolicitado)
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

        /// <summary>
        /// Devuelve el valor si se filtra por publicaEcommerce o no
        /// </summary>
        /// <param name="publicaEcommerce"></param>
        /// <returns></returns>
        private static int? FiltroEcommerce(string publicaEcommerce)
        {
            int? publica = null;

            switch (publicaEcommerce)
            {
                case "N":
                    publica = 0;
                    break;

                case "S":
                    publica = 255;
                    break;

                default:
                    publica = null;
                    break;
            }
            return publica;
        }

    }
}
