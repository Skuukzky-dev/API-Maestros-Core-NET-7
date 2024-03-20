using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseSubRubros : Response
    {
        private List<SubRubro>? _SubRubrosProductos;

        public List<SubRubro>? subRubrosProductos { get => _SubRubrosProductos; set => _SubRubrosProductos = value; }

        public Paginacion? paginacion { get; set; }
    }
}
