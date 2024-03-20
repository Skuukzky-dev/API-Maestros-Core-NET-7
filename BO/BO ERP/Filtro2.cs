using GESI.ERP.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Filtro2 : GESI.ERP.Core.BO.v2kFiltroArticulos2ID
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public Filtro2(v2kFiltroArticulos2ID oFiltro2)
        {
            FiltroArticulos2ID = oFiltro2.FiltroArticulos2ID;
            CategoriaID = oFiltro2.CategoriaID;
            Descripcion = oFiltro2.Descripcion;
        }

    }
}
