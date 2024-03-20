using GESI.ERP.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class SubSubRubro : GESI.ERP.Core.BO.v2kSubSubRubro
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public SubSubRubro(v2kSubSubRubro oRubro)
        {
            RubroID = oRubro.RubroID;
            SubRubroID = oRubro.SubRubroID;
            SubSubRubroID = oRubro.SubSubRubroID;
            CategoriaID = oRubro.CategoriaID;
            Descripcion = oRubro.Descripcion;
        }

    }
}
