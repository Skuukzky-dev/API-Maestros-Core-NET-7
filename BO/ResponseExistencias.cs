using GESI.CORE.API.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseExistencias : Response
    {
        private List<Existencia>? _existencias;


        public Paginacion? paginacion { get; set; }
        public List<Existencia>? existencias { get => _existencias; set => _existencias = value; }
    }
}
