using API_Maestros_Core.Models;
using GESI.ERP.Core.BO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Maestros_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresasController : ControllerBase
    {
        #region Variables
        public static GESI.CORE.BLL.SessionMgr _SessionMgr;
        public static List<GESI.CORE.BO.Verscom2k.HabilitacionesAPI> moHabilitacionesAPI;
        public static string mostrTipoAPI = "LEER_MAESTROS";
        public static string strUsuarioID = "";
        public static bool HabilitadoPorToken = false;
        #endregion

       

        [HttpGet("GetSucursales")]
        [EnableCors("MyCorsPolicy")]
        public IActionResult GetSucursales(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                #region Variables
                RespuestaSucursales oRespuesta = new RespuestaSucursales();
                _SessionMgr = new GESI.CORE.BLL.SessionMgr();
                #endregion

                bool Habilitado = false;

                #region Habilitaciones API
                moHabilitacionesAPI = GESI.CORE.BLL.Verscom2k.HabilitacionesAPIMgr.GetList(strUsuarioID);

                foreach (GESI.CORE.BO.Verscom2k.HabilitacionesAPI oHabilitacionAPI in moHabilitacionesAPI)
                {
                    if (oHabilitacionAPI.TipoDeAPI.Equals(mostrTipoAPI))
                    {
                        _SessionMgr.EmpresaID = oHabilitacionAPI.EmpresaID;
                        _SessionMgr.UsuarioID = oHabilitacionAPI.UsuarioID;
                        _SessionMgr.SucursalID = oHabilitacionAPI.SucursalID;
                        _SessionMgr.EntidadID = 1;
                        Habilitado = true;
                    }
                }
                #endregion


                if (Habilitado)
                {
                    List<SucursalHija> lstSucursalesFinales = new List<SucursalHija>();
                    GESI.CORE.BLL.SucursalesMgr.SessionManager = _SessionMgr;

                        GESI.CORE.BO.ListaSucursales olstSucursales = GESI.CORE.BLL.SucursalesMgr.GetList();

                        foreach(GESI.CORE.BO.Sucursal oSucursal in olstSucursales)
                        {
                            SucursalHija oSucursalFinal = new SucursalHija(oSucursal);
                            lstSucursalesFinales.Add(oSucursalFinal);
                        }

                        oRespuesta.Sucursales = lstSucursalesFinales;
                
                        if(oRespuesta.Sucursales?.Count > 0)
                        {
                            oRespuesta.error = new Error();
                            oRespuesta.success = true;
                            oRespuesta.paginacion = new Paginacion();
                            oRespuesta.paginacion.tamañoPagina = pageSize;
                            oRespuesta.paginacion.totalPaginas = (int)Math.Ceiling((double)oRespuesta.paginacion.totalElementos / pageSize);
                            oRespuesta.paginacion.paginaActual = pageNumber;
                            oRespuesta.paginacion.totalElementos = lstSucursalesFinales.Count;                            

                            return Ok(oRespuesta);
                        }
                        else
                        {
                            return NoContent();
                        }
                }
                else
                {
                    oRespuesta.error = new Error();
                    oRespuesta.error.code = 4016;
                    oRespuesta.error.message = "No está autorizado a acceder al servicio. No se encontró el token del usuario";
                    return Unauthorized(oRespuesta);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }


    public class SucursalHija : GESI.CORE.BO.Sucursal
    {
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

        public override string  Torre { get => base.Torre; set => base.Torre = value; }

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

       /* [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int rangoProveedoresDesde { get => base.rangoProveedoresDesde; set => base.rangoProveedoresDesde = value; }


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]

        public override int rangoProveedoresHasta { get => base.rangoProveedoresHasta; set => base.rangoProveedoresHasta = value; }
       */

        #endregion


        public SucursalHija(GESI.CORE.BO.Sucursal padre)
        {
            EmpresaID = padre.EmpresaID;
            SucursalID = padre.SucursalID;
            Descripcion = padre.Descripcion;
            
        }

       


    }
}
