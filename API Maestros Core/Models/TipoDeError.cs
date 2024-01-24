namespace API_Maestros_Core.Models
{
    public class TipoDeError
    {
        private  int _CodigoError;
        private  string _DescripcionError;
        private int _CodigoStatus;
        private string _TipoErrorAdvertencia;

        public int CodigoError { get => _CodigoError; set => _CodigoError = value; }
        public  string DescripcionError { get => _DescripcionError; set => _DescripcionError = value; }
        public int CodigoStatus { get => _CodigoStatus; set => _CodigoStatus = value; }
        public string TipoErrorAdvertencia { get => _TipoErrorAdvertencia; set => _TipoErrorAdvertencia = value; }

        public TipoDeError(int _codigo,string _mensaje, int codigoStatus,string _tipo)
        {
            CodigoError = _codigo;
            DescripcionError = _mensaje;
            CodigoStatus = codigoStatus;
            TipoErrorAdvertencia = _tipo;
        }

        public TipoDeError()
        { }

    }
}
