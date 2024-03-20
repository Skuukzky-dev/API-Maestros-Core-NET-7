using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseSubSubRubros : Response
    {
        private List<SubSubRubro>? _SubSubRubrosProductos;

        public List<SubSubRubro>? subsubRubrosProductos { get => _SubSubRubrosProductos; set => _SubSubRubrosProductos = value; }

        public Paginacion? paginacion { get; set; }
    }
}
