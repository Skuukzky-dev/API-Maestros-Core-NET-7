using GESI.ERP.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class SubRubro : GESI.ERP.Core.BO.v2kSubRubro
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public SubRubro(v2kSubRubro oRubro)
        {
            RubroID = oRubro.RubroID;
            SubRubroID = oRubro.SubRubroID;
            CategoriaID = oRubro.CategoriaID;
            Descripcion = oRubro.Descripcion;
        }

    }
}
