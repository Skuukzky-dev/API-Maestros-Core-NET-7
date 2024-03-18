using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseFiltro1 : Response
    {
        private List<Filtro1>? _FiltroArticulos1;

        public List<Filtro1>? filtro1Productos { get => _FiltroArticulos1; set => _FiltroArticulos1 = value; }

        public Paginacion? paginacion { get; set; }
    }
}
