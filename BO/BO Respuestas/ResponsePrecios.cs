using GESI.ERP.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponsePrecios : Response
    {
        private List<GESI.ERP.Core.BO.cPrecioProducto>? _precios;
        public Paginacion? paginacion { get; set; }
        public List<cPrecioProducto>? precios { get => _precios; set => _precios = value; }
    }
}
