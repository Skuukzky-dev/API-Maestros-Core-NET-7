using API_Maestros_Core.Controllers;
using GESI.CORE.BO;
using GESI.ERP.Core.BO;
using GESI.GESI.BO;
using static API_Maestros_Core.BLL.CategoriasMgr;

namespace API_Maestros_Core.Models
{
    public class Respuesta
    {
        private bool _success;
        private Error? _error;


        public bool success { get => _success; set => _success = value; }
        public Error? error { get => _error; set => _error = value; }
    }

    public class RespuestaConToken : Respuesta
    {
        private String? _token;
        public string? token { get => _token; set => _token = value; }
    }

    public class RespuestaProductosGetItem : Respuesta
    {
        private HijoProductos? _productos;


        public Paginacion? paginacion { get; set; }
        public HijoProductos? producto { get => _productos; set => _productos = value; }
    }

    public class RespuestaProductosGetExistencias : Respuesta
    {
        private List<ExistenciaHijas>? _existencias;


        public Paginacion? paginacion { get; set; }
        public List<ExistenciaHijas>? existencias { get => _existencias; set => _existencias = value; }
    }

    public class RespuestaProductosGetPrecios : Respuesta
    {
        private List<GESI.ERP.Core.BO.cPrecioProducto>? _precios;
        public Paginacion? paginacion { get; set; }
        public List<cPrecioProducto>? Precios { get => _precios; set => _precios = value; }
    }


    public class RespuestaConProductosHijos : Respuesta
    {
        private List<HijoProductos>? _productos;
    

        public Paginacion? paginacion { get; set; }
        public List<HijoProductos>? productos { get => _productos; set => _productos = value; }
    }

    public class RespuestaProductosGetResult : Respuesta
    {
        public Paginacion? paginacion { get; set; }
        public List<ResultProducts>? productos { get => _productos; set => _productos = value; }

        private List<ResultProducts>? _productos;
    }

    public class RespuestaConCanalesDeVenta : Respuesta
    {
        private List<GESI.ERP.Core.BO.cCanalDeVenta>? _CanalesDeVenta;

        public List<GESI.ERP.Core.BO.cCanalDeVenta>? CanalesDeVenta { get => _CanalesDeVenta; set => _CanalesDeVenta = value; }

        public Paginacion? paginacion { get; set; } 
    }

    public class RespuestaConCategorias : Respuesta
    {
        private List<CategoriaHija>? _CategoriasProductos;

        public List<CategoriaHija>? categoriasProductos { get => _CategoriasProductos; set => _CategoriasProductos = value; }

        public Paginacion? paginacion { get; set; }
    }

    public class RespuestaConRubros : Respuesta
    {
        private List<RubroHijo>? _RubrosProductos;

        public List<RubroHijo>? rubrosProductos { get => _RubrosProductos; set => _RubrosProductos = value; }

        public Paginacion? paginacion { get; set; }
    }

    public class RespuestaConSubRubros : Respuesta
    {
        private List<SubRubroHijo>? _SubRubrosProductos;

        public List<SubRubroHijo>? subRubrosProductos { get => _SubRubrosProductos; set => _SubRubrosProductos = value; }

        public Paginacion? paginacion { get; set; }
    }

    public class RespuestaConSubSubRubros : Respuesta
    {
        private List<SubSubRubroHijo>? _SubSubRubrosProductos;

        public List<SubSubRubroHijo>? subsubRubrosProductos { get => _SubSubRubrosProductos; set => _SubSubRubrosProductos = value; }

        public Paginacion? paginacion { get; set; }
    }

    public class RespuestaConFiltroArticulos1 : Respuesta
    {
        private List<Filtro1Hijo>? _FiltroArticulos1;

        public List<Filtro1Hijo>? filtro1Productos { get => _FiltroArticulos1; set => _FiltroArticulos1 = value; }

        public Paginacion? paginacion { get; set; }
    }

    public class RespuestaConFiltroArticulos2 : Respuesta
    {
        private List<Filtro2Hijo>? _FiltroArticulos2;

        public List<Filtro2Hijo>? filtro2Productos { get => _FiltroArticulos2; set => _FiltroArticulos2 = value; }

        public Paginacion? paginacion { get; set; }
    }

    public class RespuestaConFiltroArticulos3 : Respuesta
    {
        private List<Filtro3Hijo>? _FiltroArticulos3;

        public List<Filtro3Hijo>? filtro3Productos { get => _FiltroArticulos3; set => _FiltroArticulos3 = value; }

        public Paginacion? paginacion { get; set; }
    }

    public class RespuestaCategoriasGetItem : Respuesta
    {
        private CategoriaHija? _CategoriasProductos;

        public CategoriaHija? categoriaProducto { get => _CategoriasProductos; set => _CategoriasProductos = value; }

        public Paginacion? paginacion { get; set; }
    }

    public class Paginacion
    {
        private int _TotalElementos;
        private int _TotalPaginas;
        private int _PaginaActual;
        private int _TamañoPagina;

        public int totalElementos { get => _TotalElementos; set => _TotalElementos = value; }
        public int totalPaginas { get => _TotalPaginas; set => _TotalPaginas = value; }
        public int paginaActual { get => _PaginaActual; set => _PaginaActual = value; }
        public int tamañoPagina { get => _TamañoPagina; set => _TamañoPagina = value; }
    }

    public class Error
    {
        private int _code;
        private string? _message;

        public int code { get => _code; set => _code = value; }
        public string? message { get => _message; set => _message = value; }
    }

    public class RespuestaImagen : Respuesta
    {
        private string? _message;
        private int _imagenID;

        public string? message { get => _message; set => _message = value; }
        public int imagenID { get => _imagenID; set => _imagenID = value; }
    }

    public class RespuestaSucursales : Respuesta
    {
        private List<SucursalHija> _sucursales;
        public Paginacion? paginacion { get; set; }
        public List<SucursalHija> Sucursales { get => _sucursales; set => _sucursales = value; }
    }
}
