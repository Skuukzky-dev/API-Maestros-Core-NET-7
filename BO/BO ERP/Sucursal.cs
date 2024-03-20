using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Sucursal : GESI.CORE.BO.Sucursal
    {
        /*
        #region Atributos a Ignorar
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int EmpresaID { get; set; }


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string Calle { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string Piso { get => base.Piso; set => base.Piso = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string Torre { get => base.Torre; set => base.Torre = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string Departamento { get => base.Departamento; set => base.Departamento = value; }


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string Entre { get => base.Departamento; set => base.Departamento = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string Localidad { get => base.Departamento; set => base.Departamento = value; }


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string CP { get => base.Departamento; set => base.Departamento = value; }


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int RegionID { get => base.RegionID; set => base.RegionID = value; }


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string Telefono { get => base.Telefono; set => base.Telefono = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string Fax { get => base.Fax; set => base.Fax = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string email { get => base.email; set => base.email = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int RangoClientesDesde { get => base.RangoClientesDesde; set => base.RangoClientesDesde = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int RangoClientesHasta { get => base.RangoClientesHasta; set => base.RangoClientesHasta = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override decimal RecargoPrecios { get => base.RecargoPrecios; set => base.RecargoPrecios = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override bool SoloMaestrosAsociados { get => base.SoloMaestrosAsociados; set => base.SoloMaestrosAsociados = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override string IdExtra { get => base.IdExtra; set => base.IdExtra = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override bool FiltrarVentasSucursal { get => base.FiltrarVentasSucursal; set => base.FiltrarVentasSucursal = value; }


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int? CtroCostoID { get => base.CtroCostoID; set => base.CtroCostoID = value; }


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int PaisID { get => base.PaisID; set => base.PaisID = value; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int? Numero { get => base.Numero; set => base.Numero = value; }

         [System.Text.Json.Serialization.JsonIgnore]
         [Newtonsoft.Json.JsonIgnore]

         public override int rangoProveedoresDesde { get => base.rangoProveedoresDesde; set => base.rangoProveedoresDesde = value; }


         [System.Text.Json.Serialization.JsonIgnore]
         [Newtonsoft.Json.JsonIgnore]

         public override int rangoProveedoresHasta { get => base.rangoProveedoresHasta; set => base.rangoProveedoresHasta = value; }
        

        #endregion
        */

        public Sucursal(GESI.CORE.BO.Sucursal padre)
        {
            EmpresaID = padre.EmpresaID;
            SucursalID = padre.SucursalID;
            Descripcion = padre.Descripcion;

        }



    }
}
