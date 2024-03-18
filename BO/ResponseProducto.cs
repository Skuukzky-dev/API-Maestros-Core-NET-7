using GESI.CORE.API.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseProducto : Response
    {
        private Producto? _productos;


        public Paginacion? paginacion { get; set; }
        public Producto? producto { get => _productos; set => _productos = value; }
    }
}
