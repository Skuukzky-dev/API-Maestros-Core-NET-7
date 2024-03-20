using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseRubros : Response
    {
        private List<Rubro>? _RubrosProductos;

        public List<Rubro>? rubrosProductos { get => _RubrosProductos; set => _RubrosProductos = value; }

        public Paginacion? paginacion { get; set; }
    }
}
