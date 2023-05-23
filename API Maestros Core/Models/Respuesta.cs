using GESI.GESI.BO;

namespace API_Maestros_Core.Models
{
    public class Respuesta
    {
        private bool _success;
        private Error _error;


        public bool success { get => _success; set => _success = value; }
        public Error error { get => _error; set => _error = value; }

    }

    public class RespuestaConToken : Respuesta
    {
        private String _token;
        public string token { get => _token; set => _token = value; }
    }

    public class RespuestaConProductos : Respuesta
    {
        private List<GESI.ERP.Core.BO.cProducto> _Productos;
        public List<GESI.ERP.Core.BO.cProducto> Productos { get => _Productos; set => _Productos = value; }

        public Paginacion paginacion { get; set; }
    }

    public class RespuestaConCanalesDeVenta : Respuesta
    {
        private List<GESI.GESI.BO.CanalDeVenta> _CanalesDeVenta;

        public List<CanalDeVenta> CanalesDeVenta { get => _CanalesDeVenta; set => _CanalesDeVenta = value; }
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
        private String _message;

        public int code { get => _code; set => _code = value; }
        public string message { get => _message; set => _message = value; }
    }
}
