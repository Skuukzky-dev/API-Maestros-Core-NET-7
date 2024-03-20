using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseFiltro2 : Response
    {
        private List<Filtro2>? _FiltroArticulos2;

        public List<Filtro2>? filtro2Productos { get => _FiltroArticulos2; set => _FiltroArticulos2 = value; }

        public Paginacion? paginacion { get; set; }
    }
}
