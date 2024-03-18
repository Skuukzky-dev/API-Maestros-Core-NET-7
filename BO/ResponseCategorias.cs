using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseCategorias: Response
    {
        private List<Categoria> _categoriasProductos;
        private Paginacion _paginacion;

        public List<Categoria> categoriasProductos { get => _categoriasProductos; set => _categoriasProductos = value; }
        public Paginacion paginacion { get => _paginacion; set => _paginacion = value; }
    }
}
