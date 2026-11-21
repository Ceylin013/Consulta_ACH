using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultaACH.Blazor.Modelos;

[Table("TransaccionesACH")]
public class TransaccionACH
{
    public long Id { get; set; }

    [Column("tipo_transaccion")]
    public string? TipoTransaccion { get; set; }

    [Column("tipo_mensaje")]
    public string? TipoMensaje { get; set; }

    [Column("fecha_hora_procesamiento")]
    public DateTime? FechaHoraProcesamiento { get; set; }

    [Column("nombre_banco")]
    public string? NombreBanco { get; set; }

    [Column("id_banco_receptor")]
    public string? IdBancoReceptor { get; set; }

    [Column("tipo_cuenta")]
    public string? TipoCuenta { get; set; }

    [Column("descripcion_tipo_cuenta")]
    public string? DescripcionTipoCuenta { get; set; }

    [Column("cuenta_origen")]
    public string? CuentaOrigen { get; set; }

    [Column("tipo_cuenta_destino")]
    public string? TipoCuentaDestino { get; set; }

    [Column("descripcion_tipo_cuenta_destino")]
    public string? DescripcionTipoCuentaDestino { get; set; }

    [Column("cuenta_destino")]
    public string? CuentaDestino { get; set; }

    [Column("codigo_moneda")]
    public string? CodigoMoneda { get; set; }

    [Column("monto", TypeName = "decimal(18,2)")]
    public decimal? Monto { get; set; }

    [Column("monto_debito_core_bancario", TypeName = "decimal(18,2)")]
    public decimal? MontoDebitoCoreBancario { get; set; }

    [Column("monto_credito_core_bancario", TypeName = "decimal(18,2)")]
    public decimal? MontoCreditoCoreBancario { get; set; }

    [Column("tipo_cambio_core_bancario", TypeName = "decimal(18,6)")]
    public decimal? TipoCambioCoreBancario { get; set; }

    [Column("codigo_motivo")]
    public string? CodigoMotivo { get; set; }

    [Column("resultados")]
    public string? Resultados { get; set; }

    [Column("estado")]
    public string? Estado { get; set; }

    [Column("descripcion_estado")]
    public string? DescripcionEstado { get; set; }

    [Column("id_instruccion")]
    public string? IdInstruccion { get; set; }

    [Column("id_mensaje")]
    public string? IdMensaje { get; set; }

    [Column("id_transaccion")]
    public string? IdTransaccion { get; set; }

    [Column("ticket_ach")]
    public string? TicketAch { get; set; }
}