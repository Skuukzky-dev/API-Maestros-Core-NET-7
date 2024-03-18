using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseCanalesDeVenta: Response
    {
        private List<CanalDeVenta>? _CanalesDeVenta;

        public List<CanalDeVenta>? CanalesDeVenta { get => _CanalesDeVenta; set => _CanalesDeVenta = value; }

        public Paginacion? paginacion { get; set; }
    }
}
