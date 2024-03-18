using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Response
    {
        private bool _success;
        private Error? _error;


        public bool success { get => _success; set => _success = value; }
        public Error? error { get => _error; set => _error = value; }
    }
}
