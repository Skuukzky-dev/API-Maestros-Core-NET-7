using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class TotalProductoFoja
    {
        private string _productoID;
        private decimal _cantidadU1;
        private decimal _cantidadU2;

        public string productoID { get => _productoID; set => _productoID = value; }
        public decimal cantidadU1 { get => _cantidadU1; set => _cantidadU1 = value; }
        public decimal cantidadU2 { get => _cantidadU2; set => _cantidadU2 = value; }

        public TotalProductoFoja(string productoID, decimal cantidadU1, decimal cantidadU2 = 0)
        {
            _productoID = productoID;
            _cantidadU1 = cantidadU1;
            _cantidadU2 = cantidadU2;
        }
    }
}
