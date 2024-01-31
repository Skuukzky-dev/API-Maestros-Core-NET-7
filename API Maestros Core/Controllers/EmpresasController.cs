using API_Maestros_Core.BLL;
using API_Maestros_Core.Models;
using GESI.ERP.Core.BO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

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
        public static string TokenEnviado = "";
        public static string strProtocolo = "";
        public static List<TipoDeError> lstTiposDeError = APIHelper.LlenarTiposDeError();
        public TipoDeError oTipoError;
        #endregion


        [HttpGet("GetSucursales")]
        [EnableCors("MyCorsPolicy")]
        [SwaggerResponse(200, "OK", typeof(RespuestaSucursales))]
        public IActionResult GetSucursales(int pageNumber = 1, int pageSize = 10)
        {
            RespuestaSucursales oRespuesta = new RespuestaSucursales();
            try
            {
                APIHelper.SetearConnectionString();
                string ProtocoloConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["Protocolo"];

                if (APIHelper.EvaluarProtocolo(ProtocoloConfig, this.HttpContext.Request.Scheme)) // Se evalua el protocolo que contiene el backend
                {
                    APISessionManager MiSessionMgrAPI = APIHelper.SetearMgrAPI(strUsuarioID);

                    if (MiSessionMgrAPI.Habilitado)
                    {
                        List<SucursalHija> lstSucursalesFinales = new List<SucursalHija>();
                        GESI.CORE.BLL.SucursalesMgr.SessionManager = MiSessionMgrAPI.SessionMgr;

                        GESI.CORE.BO.ListaSucursales olstSucursales = GESI.CORE.BLL.SucursalesMgr.GetList();

                        #region Pasaje a Sucursal Hija
                        foreach (GESI.CORE.BO.Sucursal oSucursal in olstSucursales)
                        {
                            SucursalHija oSucursalFinal = new SucursalHija(oSucursal);
                            lstSucursalesFinales.Add(oSucursalFinal);
                        }
                        #endregion

                        oRespuesta.Sucursales = lstSucursalesFinales;

                        if (oRespuesta.Sucursales?.Count > 0)
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
                        oTipoError = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cNuevoToken);
                        oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cNuevoToken, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.SucursalesGetList);
                        oRespuesta.success = false;
                        return Unauthorized(oRespuesta);
                    }
                }
                else
                {
                    oTipoError = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cProtocoloIncorrecto);
                    oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cProtocoloIncorrecto, oTipoError.DescripcionError, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.ProductosGetList);
                    oRespuesta.success = false;
                    return BadRequest(oRespuesta);
                }

            }
            catch (Exception ex)
            {
                oTipoError = lstTiposDeError.Find(x => x.CodigoError == (int)APIHelper.cCodigosError.cErrorInternoAplicacion);
                oRespuesta.error = APIHelper.DevolverErrorAPI((int)APIHelper.cCodigosError.cErrorInternoAplicacion, oTipoError.DescripcionError+" Error al devolver Sucursales. Descripcion: "+ex.Message, oTipoError.TipoErrorAdvertencia, strUsuarioID, APIHelper.SucursalesGetList);
                oRespuesta.success = false;
                return StatusCode((int)HttpStatusCode.InternalServerError,oRespuesta);
               
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
