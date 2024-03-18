using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseCategoria:Response
    {
        private Categoria _categoria;
        private Paginacion _paginacion;

        public Categoria categoriaProducto { get => _categoria; set => _categoria = value; }
        public Paginacion paginacion { get => _paginacion; set => _paginacion = value; }
    }
}
