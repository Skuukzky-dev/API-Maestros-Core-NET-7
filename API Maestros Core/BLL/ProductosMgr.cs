﻿using API_Maestros_Core.Controllers;
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
        public static RespuestaConProductosHijos GetList(String strExpresionBusqueda, int[] CanalesDeVenta, string costoSolicitado, string costoUsuario, int pageNumber, int pageSize, string EstadosProductos, string CategoriasIDs, string imagenes, string stock = "N", int[] Almacenes = null)  // Usa GetSearchResults
        {
            try
            {
                #region ConnectionStrings
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
                GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
                GESI.ERP.Core.BLL.BASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
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

                List<string> lstCodigosProducto = GESI.ERP.Core.BLL.ProductosManager.GetSearchResults(strExpresionBusqueda, strEstado: EstadosProductos, strCategorias: CategoriasIDs);
                List<string> nuevosplit = lstCodigosProducto.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                string codigos = string.Join(",", nuevosplit);

                if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifico si tiene permisos para devolver costos
                {

                    lstProductos = GESI.ERP.Core.BLL.ProductosManager.GetList(codigos, CanalesDeVenta, costoSolicitado, EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);

                    foreach (GESI.ERP.Core.BO.cProducto oPrd in lstProductos)
                    {
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
                    oRespuesta.success = true;
                }
                else
                {
                    lstProductos = GESI.ERP.Core.BLL.ProductosManager.GetList(codigos, CanalesDeVenta, "N", EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                    foreach (GESI.ERP.Core.BO.cProducto oPrd in lstProductos)
                    {
                        lstHijos.Add(new HijoProductos(oPrd));
                    }
                    oRespuesta.error.message = "Permiso denegado en la solicitud de costos";
                    oRespuesta.error.code = 4017;
                    oRespuesta.success = true;
                }

                oRespuesta.productos = lstHijos;
                Paginacion oPaginacion = new Paginacion();
                oPaginacion.totalElementos = lstCodigosProducto.Count;
                oPaginacion.totalPaginas = (int)Math.Ceiling((double)oPaginacion.totalElementos / pageSize);
                oPaginacion.paginaActual = pageNumber;
                oPaginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion = oPaginacion;
                Logger.LoguearErrores("OK -> GetSearchResults", "I");
                return oRespuesta;

            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetSearchResults. Descripcion: " + ex.Message, "E");
                throw;
            }
        }


        /// <summary>
        /// Devuelve todos los productos 
        /// </summary>        
        /// <returns></returns>
        public static RespuestaConProductosHijos GetList(int pageNumber, int pageSize, int[] CanalesDeVenta, string costoSolicitado, string costoUsuario, string EstadosProductos, string CategoriasIDs, string imagenes, string fechamodificaciones = "", string stock = "N", int[] Almacenes = null) // Usa GetList
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
                GESI.ERP.Core.BLL.BASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
                #endregion

                #region Variables 
                GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                List<GESI.ERP.Core.BO.cProducto> lstProductos = new List<GESI.ERP.Core.BO.cProducto>();
                List<string> lstCodigosProducto = new List<string>();
                #endregion


                if (CategoriasIDs?.Length == 0)
                    CategoriasIDs = null;



                if (fechamodificaciones?.Length > 0)
                {
                    try
                    {
                        DateTime Fecha = DateTime.Parse(fechamodificaciones);
                        lstCodigosProducto = GESI.ERP.Core.BLL.ProductosManager.GetProductosModificadosDesdeFecha(Fecha, EstadosProductos, (int)LogCambios.tTablasGenerales);

                        string commaSeparatedIds = string.Join(",", lstCodigosProducto);
                        List<string> splits = commaSeparatedIds.Split(',').ToList();
                        List<string> nuevosplit = splits.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                        string resultado = string.Join(",", nuevosplit);



                        if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifico si tiene permisos para devolver costos
                        {
                            List<GESI.ERP.Core.BO.cProducto> lstProductosAux = GESI.ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, costoSolicitado, EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);

                            lstProductos = lstProductosAux;
                            oRespuesta.error = new Error();
                        }
                        else
                        {
                            List<GESI.ERP.Core.BO.cProducto> lstProductosAux = GESI.ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, "N", EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                            lstProductos = lstProductosAux;
                            oRespuesta.error = new Error();
                            oRespuesta.error.message = "Permiso denegado en la solicitud de costos";
                            oRespuesta.error.code = 4017;
                        }
                    }
                    catch (FormatException fex)
                    {
                        throw fex;
                    }
                }
                else
                {
                    lstCodigosProducto = GESI.ERP.Core.BLL.ProductosManager.GetSearchResults(strEstado: EstadosProductos, strCategorias: CategoriasIDs);
                    string commaSeparatedIds = string.Join(",", lstCodigosProducto);
                    List<string> splits = commaSeparatedIds.Split(',').ToList();
                    List<string> nuevosplit = splits.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    string resultado = string.Join(",", nuevosplit);

                    if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifico si tiene permisos para devolver costos
                    {
                        List<GESI.ERP.Core.BO.cProducto> lstProductosAux = GESI.ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, costoSolicitado, EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                        lstProductos = lstProductosAux;
                        oRespuesta.error = new Error();

                    }
                    else
                    {
                        List<GESI.ERP.Core.BO.cProducto> lstProductosAux = GESI.ERP.Core.BLL.ProductosManager.GetList(resultado, CanalesDeVenta, "N", EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                        lstProductos = lstProductosAux;
                        oRespuesta.error = new Error();
                        oRespuesta.error.message = "Permiso denegado en la solicitud de costos";
                        oRespuesta.error.code = 4017;
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

                Logger.LoguearErrores("OK -> GetList", "I");

                return oRespuesta;


            }
            catch (FormatException fex)
            {
                throw fex;
            }
            catch (AccessViolationException ax)
            {
                Logger.LoguearErrores("Permiso denegado sobre costoSolicitado. Descripcion: " + ax.Message, "E");
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
        public static RespuestaProductosGetItem GetItem(String ProductoID, int[] CanalesDeVenta, int CanalDeVentaID, string costoSolicitado, string costoUsuario, string EstadosProductos, string CategoriasIDs, string imagenes, string stock = "N", int[] Almacenes = null) // Usa GetItem
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
                GESI.ERP.Core.BLL.BASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
                GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
                GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
                GESI.ERP.Core.BLL.ProductosManager.ERPsessionManager = _SessionERP;
                #endregion

                #region Variables
                HijoProductos lstHijos = new HijoProductos();
                List<GESI.ERP.Core.BO.cProducto> oProduc = new List<GESI.ERP.Core.BO.cProducto>();
                #endregion

                if (VerificarPermisoSobreCostos(costoUsuario, costoSolicitado)) // Verifica si tiene permiso para devolver costos del proveedor
                {
                    #region Tiene permisos sobre costos                    
                    oProduc = GESI.ERP.Core.BLL.ProductosManager.GetList(ProductoID, CanalesDeVenta, costoSolicitado, EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                    oRespuesta.error = new Error();
                    #endregion
                }
                else
                {
                    oProduc = GESI.ERP.Core.BLL.ProductosManager.GetList(ProductoID, CanalesDeVenta, "N", EstadosProductos, CategoriasIDs, imagenes, stock, Almacenes);
                    oRespuesta.error = new Error();
                    oRespuesta.error.message = "Permiso denegado en la solicitud de costos";
                    oRespuesta.error.code = 4017;
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
                    oRespuesta.error.code = 2041;
                    oRespuesta.error.message = "No se encontro el producto buscado";
                    oRespuesta.paginacion = oPaginacion;
                }
                oRespuesta.success = true;
                #endregion
                Logger.LoguearErrores("OK -> GetItem ProductoID: "+ProductoID, "I");
                return oRespuesta;

            }
            catch (AccessViolationException ax)
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
        /// Devuelve las existencias por productoS
        /// </summary>
        /// <param name="codigos"></param>
        /// <param name="Almacenes"></param>
        public static RespuestaProductosGetExistencias GetExistencias(string codigos, int pageNumber = 1, int pageSize = 10,int[] Almacenes = null)
        {
            RespuestaProductosGetExistencias oRespuesta = new RespuestaProductosGetExistencias();
            #region ConnectionStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            GESI.ERP.Core.BLL.BASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
            GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
            GESI.ERP.Core.BLL.ExistenciasManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.BLL.ExistenciasManager.ERPsessionManager = _SessionERP;
            #endregion


            try
            {
                List<string> lstCodigosProducto = codigos.Split(",").ToList();
                
                string AlmacenesIDs = null;
                if (Almacenes?.Length > 0)
                    AlmacenesIDs = string.Join(",", Almacenes);

                List<GESI.ERP.Core.BO.cExistenciaProducto> lstExistencias = GESI.ERP.Core.BLL.ExistenciasManager.GetExistenciaProductos(codigos, AlmacenesIDs);
                List<ExistenciaHijas> lstExistenciasHijas = new List<ExistenciaHijas>();

                foreach(GESI.ERP.Core.BO.cExistenciaProducto oExistencia in lstExistencias)
                {
                    ExistenciaHijas oExHija = new ExistenciaHijas(oExistencia);
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
                Logger.LoguearErrores("OK -> GetExistencias. Codigos: "+codigos, "I");
                return oRespuesta;
            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetExistencias. Descripcion: " + ex.Message, "E");
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
        public static RespuestaProductosGetPrecios GetPrecios(string codigos, int[] CanalesDeVenta, int pageNumber = 1, int pageSize = 10, string fechamodificaciones = "", string EstadosProductos = "A")
        {
            RespuestaProductosGetPrecios oRespuesta = new RespuestaProductosGetPrecios();
            #region ConnectionStrings
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = System.IO.Directory.GetCurrentDirectory() + "\\app.config";
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            SqlConnection sqlapi = new SqlConnection(config.ConnectionStrings.ConnectionStrings["ConexionVersCom2k"].ConnectionString);
            GESI.CORE.DAL.Configuracion._ConnectionString = sqlapi.ConnectionString;
            GESI.ERP.Core.BLL.BASEManager.ConnectionStringEstoEstaMal = sqlapi.ConnectionString;
            GESI.ERP.Core.BLL.ProductosManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.SessionManager _SessionERP = new GESI.ERP.Core.SessionManager();
            GESI.ERP.Core.BLL.ExistenciasManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.BLL.ExistenciasManager.ERPsessionManager = _SessionERP;
            #endregion
            
            try
            {
                if (fechamodificaciones?.Length > 0)
                {
                    try
                    {
                        DateTime Fecha = DateTime.Parse(fechamodificaciones);
                        List<string> lstCodigosProducto = GESI.ERP.Core.BLL.ProductosManager.GetProductosModificadosDesdeFecha(Fecha, EstadosProductos, (int)LogCambios.tPrecios);
                        string combinedString = string.Join(",", lstCodigosProducto);
                        List<GESI.ERP.Core.BO.cProducto> lstProductos = GESI.ERP.Core.BLL.ProductosManager.GetList(combinedString, CanalesDeVenta, "N", "A", "", "N", "N");
                        oRespuesta = DeterminarPreciosGetPrecios(combinedString, CanalesDeVenta, pageNumber, pageSize,"N","A","","N");
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

                Logger.LoguearErrores("OK -> GetPrecios . Codigos "+codigos, "I");
                return oRespuesta;
            }
            catch (Exception ex)
            {
                Logger.LoguearErrores("Error al solicitar GetPrecios. Descripcion: " + ex.Message, "E");
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
        private static RespuestaProductosGetPrecios DeterminarPreciosGetPrecios(string codigos, int[] CanalesDeVenta, int pageNumber,int pageSize,string DevolverCostos = "N", string EstadoProductos = "A", string categoriasIDs = "", string DevuelveImagenes = "N")
        {
            try
            {
                RespuestaProductosGetPrecios oRespuesta = new RespuestaProductosGetPrecios();
                List<GESI.ERP.Core.BO.cProducto> lstProductos = GESI.ERP.Core.BLL.ProductosManager.GetList(codigos, CanalesDeVenta,DevolverCostos,EstadoProductos,categoriasIDs,DevuelveImagenes);
                List<GESI.ERP.Core.BO.cPrecioProducto> lstPrecios = new List<GESI.ERP.Core.BO.cPrecioProducto>();
                #region Lleno la lista de precios

                foreach (GESI.ERP.Core.BO.cProducto oProducto in lstProductos)
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
            catch(Exception ex)
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
