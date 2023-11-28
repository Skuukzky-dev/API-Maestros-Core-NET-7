using API_Maestros_Core.Controllers;
using API_Maestros_Core.Models;
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


        /// <summary>
        /// Devuelve la lista de Rubros en Endpoint RubrosGetList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static RespuestaConRubros RubrosGetList(int pageNumber = 1, int pageSize = 10)
        {

            GESI.ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new GESI.ERP.Core.SessionManager();
            RespuestaConRubros oRespuesta = new RespuestaConRubros();
            List<GESI.ERP.Core.BO.v2kRubro> lstRubros = new List<GESI.ERP.Core.BO.v2kRubro>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = GESI.ERP.Core.BLL.V2KRubrosManager.GetList((uint)_SessionMgr.EmpresaID);
                
                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<RubroHijo> RubroHijo = new List<RubroHijo>();
                foreach (GESI.ERP.Core.BO.v2kRubro oRubro in lstRubros)
                {
                    RubroHijo.Add(new RubroHijo(oRubro));
                }

                List<RubroHijo> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                oRespuesta.success = true;
                oRespuesta.rubrosProductos = nuevosplit;
                oRespuesta.error = new Error();
                Logger.LoguearErrores("RubrosGetList: Exitoso" , "I", _SessionMgr.UsuarioID, "RubrosGetList");

                return oRespuesta;

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }



        public static RespuestaConSubRubros SubRubrosGetList(int pageNumber = 1, int pageSize = 10)
        {

            GESI.ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new GESI.ERP.Core.SessionManager();
            RespuestaConSubRubros oRespuesta = new RespuestaConSubRubros();
            List<GESI.ERP.Core.BO.v2kSubRubro> lstRubros = new List<GESI.ERP.Core.BO.v2kSubRubro>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = GESI.ERP.Core.BLL.V2KSubRubrosManager.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<SubRubroHijo> lstSubRubroHijo = new List<SubRubroHijo>();
                foreach(GESI.ERP.Core.BO.v2kSubRubro oSubRubro in lstRubros)
                {
                    lstSubRubroHijo.Add(new SubRubroHijo(oSubRubro));
                }
                
                List<SubRubroHijo> nuevosplit = lstSubRubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

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




        public static RespuestaConSubSubRubros SubSubRubrosGetList(int pageNumber = 1, int pageSize = 10)
        {

            GESI.ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new GESI.ERP.Core.SessionManager();
            RespuestaConSubSubRubros oRespuesta = new RespuestaConSubSubRubros();
            List<GESI.ERP.Core.BO.v2kSubSubRubro> lstRubros = new List<GESI.ERP.Core.BO.v2kSubSubRubro>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = GESI.ERP.Core.BLL.V2KSubSubRubrosManager.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<SubSubRubroHijo> RubroHijo = new List<SubSubRubroHijo>();
                foreach (GESI.ERP.Core.BO.v2kSubSubRubro oRubro in lstRubros)
                {
                    RubroHijo.Add(new SubSubRubroHijo(oRubro));
                }

                List<SubSubRubroHijo> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
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


        public static RespuestaConFiltroArticulos1 FiltroArticulos1GetList(int pageNumber = 1, int pageSize = 10)
        {

            GESI.ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new GESI.ERP.Core.SessionManager();
            RespuestaConFiltroArticulos1 oRespuesta = new RespuestaConFiltroArticulos1();
            List<GESI.ERP.Core.BO.v2kFiltroArticulos1ID> lstRubros = new List<GESI.ERP.Core.BO.v2kFiltroArticulos1ID>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = GESI.ERP.Core.BLL.V2KFiltroArticulos1.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<Filtro1Hijo> RubroHijo = new List<Filtro1Hijo>();
                foreach (GESI.ERP.Core.BO.v2kFiltroArticulos1ID oRubro in lstRubros)
                {
                    RubroHijo.Add(new Filtro1Hijo(oRubro));
                }

                List<Filtro1Hijo> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
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


        public static RespuestaConFiltroArticulos2 FiltroArticulos2GetList(int pageNumber = 1, int pageSize = 10)
        {

            GESI.ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new GESI.ERP.Core.SessionManager();
            RespuestaConFiltroArticulos2 oRespuesta = new RespuestaConFiltroArticulos2();
            List<GESI.ERP.Core.BO.v2kFiltroArticulos2ID> lstRubros = new List<GESI.ERP.Core.BO.v2kFiltroArticulos2ID>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = GESI.ERP.Core.BLL.V2KFiltroArticulos2.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<Filtro2Hijo> RubroHijo = new List<Filtro2Hijo>();
                foreach (GESI.ERP.Core.BO.v2kFiltroArticulos2ID oRubro in lstRubros)
                {
                    RubroHijo.Add(new Filtro2Hijo(oRubro));
                }

                List<Filtro2Hijo> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
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


        public static RespuestaConFiltroArticulos3 FiltroArticulos3GetList(int pageNumber = 1, int pageSize = 10)
        {

            GESI.ERP.Core.BLL.V2KRubrosManager.SessionManager = _SessionMgr;
            GESI.ERP.Core.BLL.V2KRubrosManager.ERPsessionManager = new GESI.ERP.Core.SessionManager();
            RespuestaConFiltroArticulos3 oRespuesta = new RespuestaConFiltroArticulos3();
            List<GESI.ERP.Core.BO.v2kFiltroArticulos3ID> lstRubros = new List<GESI.ERP.Core.BO.v2kFiltroArticulos3ID>();

            try
            {
                oRespuesta.paginacion = new Paginacion();
                lstRubros = GESI.ERP.Core.BLL.V2KFiltroArticulos3.GetList((uint)_SessionMgr.EmpresaID);

                oRespuesta.paginacion.paginaActual = pageNumber;
                oRespuesta.paginacion.tamañoPagina = pageSize;
                oRespuesta.paginacion.totalElementos = lstRubros.Count;
                oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);

                List<Filtro3Hijo> RubroHijo = new List<Filtro3Hijo>();
                foreach (GESI.ERP.Core.BO.v2kFiltroArticulos3ID oRubro in lstRubros)
                {
                    RubroHijo.Add(new Filtro3Hijo(oRubro));
                }

                List<Filtro3Hijo> nuevosplit = RubroHijo.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
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
