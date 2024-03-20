using GESI.ERP.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Filtro1 : GESI.ERP.Core.BO.v2kFiltroArticulos1ID
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public Filtro1(v2kFiltroArticulos1ID oFiltro1)
        {
            FiltroArticulos1ID = oFiltro1.FiltroArticulos1ID;
            CategoriaID = oFiltro1.CategoriaID;
            Descripcion = oFiltro1.Descripcion;
        }

    }
}
