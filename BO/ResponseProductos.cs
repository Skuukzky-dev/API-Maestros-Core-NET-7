using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseProductos : Response
    {
        private List<Producto>? _productos;


        public Paginacion? paginacion { get; set; }
        public List<Producto>? productos { get => _productos; set => _productos = value; }
    }

}
