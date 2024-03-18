using GESI.ERP.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Filtro3 : GESI.ERP.Core.BO.v2kFiltroArticulos3ID
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public Filtro3(v2kFiltroArticulos3ID oFiltro3)
        {
            FiltroArticulos3ID = oFiltro3.FiltroArticulos3ID;
            CategoriaID = oFiltro3.CategoriaID;
            Descripcion = oFiltro3.Descripcion;
        }

    }
}
