using GESI.CORE.API.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BLL
{
    public class CategoriasMgr
    {
        public static CORE.BLL.SessionMgr _SessionMgr;


        /// <summary>
        /// Devuelve toda la lista de categorias de productos
        /// </summary>
        /// <returns></returns>
        public static List<Categoria> GetList()
        {
            try
            {
                APIHelper.SetearConnectionString();

                ERP.Core.SessionManager _SessionERP = new ERP.Core.SessionManager();
                ERP.Core.BLL.CategoriasManager.ERPsessionManager = _SessionERP;
                ERP.Core.BLL.CategoriasManager.SessionManager = _SessionMgr;
                List<Categoria> lstCategoriasHijas = new List<Categoria>();
                List<ERP.Core.BO.cCategoriaDeProducto> lstCategoriasProducto = ERP.Core.BLL.CategoriasManager.GetList();

                if (lstCategoriasProducto?.Count > 0)
                {
                    foreach (ERP.Core.BO.cCategoriaDeProducto oCategoria in lstCategoriasProducto)
                    {
                        lstCategoriasHijas.Add(new Categoria(oCategoria));
                    }
                }


                return lstCategoriasHijas;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// Devuelve la categoria enviada
        /// </summary>
        /// <param name="CategoriaID"></param>
        /// <returns></returns>
        public static Categoria GetItem(String CategoriaID)
        {
            try
            {
                APIHelper.SetearConnectionString();

                ERP.Core.SessionManager _SessionERP = new ERP.Core.SessionManager();
                ERP.Core.BLL.CategoriasManager.ERPsessionManager = _SessionERP;
                ERP.Core.BLL.CategoriasManager.SessionManager = _SessionMgr;
                List<Categoria> lstCategoriasHijas = new List<Categoria>();
                List<ERP.Core.BO.cCategoriaDeProducto> lstCategoriasProducto = ERP.Core.BLL.CategoriasManager.GetList(CategoriaID);

                if (lstCategoriasProducto?.Count > 0)
                {

                    foreach (ERP.Core.BO.cCategoriaDeProducto oCategoria in lstCategoriasProducto)
                    {
                        lstCategoriasHijas.Add(new Categoria(oCategoria));
                    }

                    return lstCategoriasHijas[0];
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


       


        /// <summary>
        /// Devuelve la lista de Rubros en Endpoint RubrosGetList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static ResponseRubros RubrosGetList(int pageNumber = 1, int pageSize = 10)
        {
            APIHelper.SetearConnectionString();

            ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new ERP.Core.SessionManager();
            ResponseRubros oRespuesta = new ResponseRubros();
            List<ERP.Core.BO.v2kRubro> lstRubros = new List<ERP.Core.BO.v2kRubro>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = ERP.Core.BLL.V2KRubrosManager.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<Rubro> RubroHijo = new List<Rubro>();
                foreach (ERP.Core.BO.v2kRubro oRubro in lstRubros)
                {
                    RubroHijo.Add(new Rubro(oRubro));
                }

                List<Rubro> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                oRespuesta.success = true;
                oRespuesta.rubrosProductos = nuevosplit;
                oRespuesta.error = new Error();
                Logger.LoguearErrores("RubrosGetList: Exitoso", "I", _SessionMgr.UsuarioID, "RubrosGetList");

                return oRespuesta;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Devuelve la lista de SubRubros en endpoint SubRubrosGetList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static ResponseSubRubros SubRubrosGetList(int pageNumber = 1, int pageSize = 10)
        {
            APIHelper.SetearConnectionString();

            ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new ERP.Core.SessionManager();
            ResponseSubRubros oRespuesta = new ResponseSubRubros();
            List<ERP.Core.BO.v2kSubRubro> lstRubros = new List<ERP.Core.BO.v2kSubRubro>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = ERP.Core.BLL.V2KSubRubrosManager.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<SubRubro> lstSubRubroHijo = new List<SubRubro>();
                foreach (ERP.Core.BO.v2kSubRubro oSubRubro in lstRubros)
                {
                    lstSubRubroHijo.Add(new SubRubro(oSubRubro));
                }

                List<SubRubro> nuevosplit = lstSubRubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                oRespuesta.success = true;
                oRespuesta.subRubrosProductos = nuevosplit;
                oRespuesta.error = new Error();
                Logger.LoguearErrores("SubRubrosGetList: Exitoso", "I", _SessionMgr.UsuarioID, "SubRubrosGetList");

                return oRespuesta;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Devuelve la lista de SubSubRubros en Endpoint SubSubRubrosGetList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static ResponseSubSubRubros SubSubRubrosGetList(int pageNumber = 1, int pageSize = 10)
        {
            APIHelper.SetearConnectionString();

            ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new ERP.Core.SessionManager();
            ResponseSubSubRubros oRespuesta = new ResponseSubSubRubros();
            List<ERP.Core.BO.v2kSubSubRubro> lstRubros = new List<ERP.Core.BO.v2kSubSubRubro>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = ERP.Core.BLL.V2KSubSubRubrosManager.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<SubSubRubro> RubroHijo = new List<SubSubRubro>();
                foreach (ERP.Core.BO.v2kSubSubRubro oRubro in lstRubros)
                {
                    RubroHijo.Add(new SubSubRubro(oRubro));
                }

                List<SubSubRubro> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                oRespuesta.success = true;
                oRespuesta.subsubRubrosProductos = nuevosplit;
                oRespuesta.error = new Error();
                Logger.LoguearErrores("SubSubRubrosGetList: Exitoso", "I", _SessionMgr.UsuarioID, "SubSubRubrosGetList");
                return oRespuesta;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Devuelve la lista de FiltroArticulos1 en Endpoint FiltroArticulos1GetList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static ResponseFiltro1 FiltroArticulos1GetList(int pageNumber = 1, int pageSize = 10)
        {
            APIHelper.SetearConnectionString();

            ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new ERP.Core.SessionManager();
            ResponseFiltro1 oRespuesta = new ResponseFiltro1();
            List<ERP.Core.BO.v2kFiltroArticulos1ID> lstRubros = new List<ERP.Core.BO.v2kFiltroArticulos1ID>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = ERP.Core.BLL.V2KFiltroArticulos1.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<Filtro1> RubroHijo = new List<Filtro1>();
                foreach (ERP.Core.BO.v2kFiltroArticulos1ID oRubro in lstRubros)
                {
                    RubroHijo.Add(new Filtro1(oRubro));
                }

                List<Filtro1> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                oRespuesta.success = true;
                oRespuesta.filtro1Productos = nuevosplit;
                oRespuesta.error = new Error();
                Logger.LoguearErrores("FiltroArticulos1GetList: Exitoso", "I", _SessionMgr.UsuarioID, "FiltroArticulos1GetList");
                return oRespuesta;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Devuelve la lista de FiltroArticulos2 en Endpoint FiltroArticulos2GetList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static ResponseFiltro2 FiltroArticulos2GetList(int pageNumber = 1, int pageSize = 10)
        {
            APIHelper.SetearConnectionString();

            ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new ERP.Core.SessionManager();
            ResponseFiltro2 oRespuesta = new ResponseFiltro2();
            List<ERP.Core.BO.v2kFiltroArticulos2ID> lstRubros = new List<ERP.Core.BO.v2kFiltroArticulos2ID>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = ERP.Core.BLL.V2KFiltroArticulos2.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<Filtro2> RubroHijo = new List<Filtro2>();
                foreach (ERP.Core.BO.v2kFiltroArticulos2ID oRubro in lstRubros)
                {
                    RubroHijo.Add(new Filtro2(oRubro));
                }

                List<Filtro2> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                oRespuesta.success = true;
                oRespuesta.filtro2Productos = nuevosplit;
                oRespuesta.error = new Error();
                Logger.LoguearErrores("FiltroArticulos2GetList: Exitoso", "I", _SessionMgr.UsuarioID, "FiltroArticulos2GetList");
                return oRespuesta;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Devuelve la lista de FiltroArticulos3 en Endpoint FiltroArticulos3GetList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static ResponseFiltro3 FiltroArticulos3GetList(int pageNumber = 1, int pageSize = 10)
        {
            APIHelper.SetearConnectionString();

            ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new ERP.Core.SessionManager();
            ResponseFiltro3 oRespuesta = new ResponseFiltro3();
            List<ERP.Core.BO.v2kFiltroArticulos3ID> lstRubros = new List<ERP.Core.BO.v2kFiltroArticulos3ID>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = ERP.Core.BLL.V2KFiltroArticulos3.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<Filtro3> RubroHijo = new List<Filtro3>();
                foreach (ERP.Core.BO.v2kFiltroArticulos3ID oRubro in lstRubros)
                {
                    RubroHijo.Add(new Filtro3(oRubro));
                }

                List<Filtro3> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                oRespuesta.success = true;
                oRespuesta.filtro3Productos = nuevosplit;
                oRespuesta.error = new Error();
                Logger.LoguearErrores("FiltroArticulos3GetList: Exitoso", "I", _SessionMgr.UsuarioID, "FiltroArticulos3GetList");
                return oRespuesta;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
