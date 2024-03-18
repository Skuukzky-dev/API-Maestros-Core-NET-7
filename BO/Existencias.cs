using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESI.CORE.API.BO
{
    public class Existencia : GESI.ERP.Core.BO.cExistenciaProducto
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override uint EmpresaID { get => base.EmpresaID; set => base.EmpresaID = value; }

        public Existencia(GESI.ERP.Core.BO.cExistenciaProducto padre)
        {
            ProductoID = padre.ProductoID;
            AlmacenID = padre.AlmacenID;
            FechaExistencia = padre.FechaExistencia;
            Unidad1 = padre.Unidad1;
            Unidad2 = padre.Unidad2;
            Unidad1AcopioComprometido = padre.Unidad1AcopioComprometido;
            Unidad2AcopioComprometido = padre.Unidad2AcopioComprometido;
            Unidad1AcopioNoComprometido = padre.Unidad1AcopioNoComprometido;
            Unidad2AcopioNoComprometido = padre.Unidad2AcopioNoComprometido;
            Unidad1NPEPendientes = padre.Unidad1NPEPendientes;
            Unidad2NPEPendientes = padre.Unidad2NPEPendientes;
            Unidad1Disponible = padre.Unidad1Disponible;
            Unidad2Disponible = padre.Unidad2Disponible;
        }

    }
}
