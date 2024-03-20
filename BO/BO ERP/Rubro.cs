using GESI.ERP.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Rubro : GESI.ERP.Core.BO.v2kRubro
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public Rubro(v2kRubro oRubro)
        {
            RubroID = oRubro.RubroID;
            CategoriaID = oRubro.CategoriaID;
            Descripcion = oRubro.Descripcion;
        }

    }
}
