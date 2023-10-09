using GESI.CORE.BLL;

namespace API_Maestros_Core.Models
{
    public class APISessionManager
    {
        private GESI.CORE.BLL.SessionMgr _SessionMgr;
        private string _CostosXProveedor;
        private string _EstadoProductos;
        private int[] _Almacenes;
        private int[] _CanalesDeVenta;
        private bool _Habilitado;
        private string _CategoriasIDs;
        private int[] _SucursalIDs;

        public SessionMgr SessionMgr { get => _SessionMgr; set => _SessionMgr = value; }
        public string CostosXProveedor { get => _CostosXProveedor; set => _CostosXProveedor = value; }
        public string EstadoProductos { get => _EstadoProductos; set => _EstadoProductos = value; }
        public int[] Almacenes { get => _Almacenes; set => _Almacenes = value; }
        public int[] CanalesDeVenta { get => _CanalesDeVenta; set => _CanalesDeVenta = value; }

        public bool Habilitado { get => _Habilitado; set => _Habilitado = value; }
        public string CategoriasIDs { get => _CategoriasIDs; set => _CategoriasIDs = value; }
        public int[] SucursalIDs { get => _SucursalIDs; set => _SucursalIDs = value; }
    }
}
