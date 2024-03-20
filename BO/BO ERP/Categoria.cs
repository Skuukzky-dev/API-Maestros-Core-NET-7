using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Categoria : GESI.ERP.Core.BO.cCategoriaDeProducto
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public Categoria(GESI.ERP.Core.BO.cCategoriaDeProducto padre)
        {
            CategoriaID = padre.CategoriaID;
            Descripcion = padre.Descripcion;
            CategoriaPadreID = padre.CategoriaPadreID;
        }

        public Categoria()
        {

        }

    }
}
