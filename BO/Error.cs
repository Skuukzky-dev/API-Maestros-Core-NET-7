using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Error
    {
        private int _code;
        private string? _message;

        public int code { get => _code; set => _code = value; }
        public string? message { get => _message; set => _message = value; }
    }
}
