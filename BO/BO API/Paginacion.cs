using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
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
}
