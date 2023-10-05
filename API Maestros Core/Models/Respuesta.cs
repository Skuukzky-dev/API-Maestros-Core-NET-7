using API_Maestros_Core.Controllers;
using GESI.ERP.Core.BO;
using GESI.GESI.BO;

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
        private List<GESI.ERP.Core.BO.cExistenciaProducto>? _existencias;


        public Paginacion? paginacion { get; set; }
        public List<GESI.ERP.Core.BO.cExistenciaProducto>? existencias { get => _existencias; set => _existencias = value; }
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
        private List<GESI.GESI.BO.CanalDeVenta>? _CanalesDeVenta;

        public List<CanalDeVenta>? CanalesDeVenta { get => _CanalesDeVenta; set => _CanalesDeVenta = value; }

        public Paginacion? paginacion { get; set; } 
    }

    public class RespuestaConCategorias : Respuesta
    {
        private List<GESI.ERP.Core.BO.cCategoriaDeProducto>? _CategoriasProductos;

        public List<cCategoriaDeProducto>? categoriasProductos { get => _CategoriasProductos; set => _CategoriasProductos = value; }

        public Paginacion? paginacion { get; set; }
    }

    public class RespuestaCategoriasGetItem : Respuesta
    {
        private GESI.ERP.Core.BO.cCategoriaDeProducto? _CategoriasProductos;

        public cCategoriaDeProducto? categoriaProducto { get => _CategoriasProductos; set => _CategoriasProductos = value; }

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
}
