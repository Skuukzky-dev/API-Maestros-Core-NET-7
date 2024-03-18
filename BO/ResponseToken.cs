using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class ResponseToken : Response
    {
        private string _token;
        private Paginacion _paginacion;

        public string Token { get => _token; set => _token = value; }
        public Paginacion Paginacion { get => _paginacion; set => _paginacion = value; }
    }
}
