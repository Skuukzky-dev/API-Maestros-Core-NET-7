using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseFiltro3 : Response
    {
        private List<Filtro3>? _FiltroArticulos3;

        public List<Filtro3>? filtro3Productos { get => _FiltroArticulos3; set => _FiltroArticulos3 = value; }

        public Paginacion? paginacion { get; set; }
    }
}
