using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class CanalDeVenta : GESI.ERP.Core.BO.cCanalDeVenta
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override ushort EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public CanalDeVenta(GESI.ERP.Core.BO.cCanalDeVenta padre)
        {
            CanalDeVentaID = padre.CanalDeVentaID;
            Predeterminado = padre.Predeterminado;
            Descripcion = padre.Descripcion;
            PreciosConIVA = padre.PreciosConIVA;
        }

    }
}
