using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseEmpresas : Response
    {
        private List<GESI.CORE.BO.Empresa>? _empresas;
        public Paginacion? paginacion { get; set; }
        public List<GESI.CORE.BO.Empresa>? Empresas { get => _empresas; set => _empresas = value; }
    }
}
