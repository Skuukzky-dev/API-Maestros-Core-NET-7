using GESI.ERP.Core.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Producto : GESI.ERP.Core.BO.cProducto
    {
       
            /* [System.Text.Json.Serialization.JsonIgnore]
             [Newtonsoft.Json.JsonIgnore]

             public override short RubroID { get; set; }

             [System.Text.Json.Serialization.JsonIgnore]
             [Newtonsoft.Json.JsonIgnore]

             public override short SubRubroID { get; set; }

             [System.Text.Json.Serialization.JsonIgnore]
             [Newtonsoft.Json.JsonIgnore]

             public override short SubSubRubroID { get; set; }

             [System.Text.Json.Serialization.JsonIgnore]
             [Newtonsoft.Json.JsonIgnore]

             public override short FiltroArticulos1ID { get => base.FiltroArticulos1ID; set => base.FiltroArticulos1ID = value; }

             [System.Text.Json.Serialization.JsonIgnore]
             [Newtonsoft.Json.JsonIgnore]

             public override short FiltroArticulos2ID { get => base.FiltroArticulos2ID; set => base.FiltroArticulos2ID = value; }

             [System.Text.Json.Serialization.JsonIgnore]
             [Newtonsoft.Json.JsonIgnore]

             public override short FiltroArticulos3ID { get => base.FiltroArticulos3ID; set => base.FiltroArticulos3ID = value; }
            */

            [System.Text.Json.Serialization.JsonIgnore]
            [Newtonsoft.Json.JsonIgnore]

            public override int EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

            public List<int> ListaDeCategorias { get => _CodigosCategorias; set => _CodigosCategorias = value; }

            private List<int> _CodigosCategorias;

            [System.Text.Json.Serialization.JsonIgnore]
            [Newtonsoft.Json.JsonIgnore]
            public override List<cCategoriaXProducto> Categorias { get => base.Categorias; set => base.Categorias = value; }



            public Producto(GESI.ERP.Core.BO.cProducto padre)
            {
                EmpresaID = padre.EmpresaID;
                ProductoID = padre.ProductoID;
                Descripcion = padre.Descripcion;
                DescripcionExtendida = padre.DescripcionExtendida;
                AlicuotaIVA = padre.AlicuotaIVA;
                RubroID = padre.RubroID;
                SubRubroID = padre.SubRubroID;
                SubSubRubroID = padre.SubSubRubroID;
                FiltroArticulos1ID = padre.FiltroArticulos1ID;
                FiltroArticulos2ID = padre.FiltroArticulos2ID;
                FiltroArticulos3ID = padre.FiltroArticulos3ID;
                Unidad2XUnidad1 = padre.Unidad2XUnidad1;
                Unidad2XUnidad1Confirmar = padre.Unidad2XUnidad1Confirmar;
                CostosProveedores = padre.CostosProveedores;
                Imagenes = padre.Imagenes;
                Precios = padre.Precios;
                //  Categorias = padre.Categorias;
                Estado = padre.Estado;
                GrupoArtID = padre.GrupoArtID;
                ListaDeCategorias = new List<int>();
                Existencias = padre.Existencias;
                Largo = padre.Largo;
                Ancho = padre.Ancho;
                Alto = padre.Alto;
                Peso = padre.Peso;

            }




            public Producto()
            { }


    }
}
