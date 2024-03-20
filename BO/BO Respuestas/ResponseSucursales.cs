using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseSucursales : Response
    {
        private List<Sucursal> _sucursales;
        public Paginacion? paginacion { get; set; }
        public List<Sucursal> Sucursales { get => _sucursales; set => _sucursales = value; }
    }
}
