using ConsultaACH.Blazor.Datos;
using ConsultaACH.Blazor.Modelos;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace ConsultaACH.Blazor.Servicios;

public class ServicioTransaccionesACH
{
    private readonly ContextoBD _contexto;

    public ServicioTransaccionesACH(ContextoBD contexto)
    {
        _contexto = contexto;
    }

    //todas las transacciones
    public async Task<List<TransaccionACH>> ObtenerTransaccionesAsync()
    {
        return await _contexto.TransaccionesACH
            .AsNoTracking()
            .OrderBy(t => t.FechaHoraProcesamiento)
            .ToListAsync();
    }

    //rango de fechas
    public async Task<List<TransaccionACH>> BuscarPorFechaAsync(
        DateTime fechaInicial,
        DateTime fechaFinal)
    {
        var inicio = fechaInicial.Date;
        var fin = fechaFinal.Date.AddDays(1);

        return await _contexto.TransaccionesACH
            .AsNoTracking()
            .Where(t =>
                t.FechaHoraProcesamiento >= inicio &&
                t.FechaHoraProcesamiento < fin)
            .OrderBy(t => t.FechaHoraProcesamiento)
            .ToListAsync();
    }

    //reporte Excel con todos los campos
    public byte[] ExportarExcelAsync(
        List<TransaccionACH> transacciones)
    {
        using var libro = new XLWorkbook();

        var hoja = libro.Worksheets.Add("Reporte ACH");

        string[] encabezados =
        {
            "Tipo de transacción",
            "Tipo de mensaje",
            "Fecha y hora de procesamiento",
            "Nombre del banco",
            "ID banco receptor",
            "Tipo de cuenta",
            "Descripción tipo de cuenta",
            "Cuenta origen",
            "Tipo cuenta destino",
            "Descripción tipo cuenta destino",
            "Cuenta destino",
            "Código moneda",
            "Monto",
            "Monto débito core bancario",
            "Monto crédito core bancario",
            "Tipo cambio core bancario",
            "Código motivo",
            "Resultados",
            "Estado",
            "Descripción estado",
            "ID instrucción",
            "ID mensaje",
            "ID transacción",
            "Ticket ACH"
        };

        for (int columna = 0; columna < encabezados.Length; columna++)
        {
            hoja.Cell(1, columna + 1).Value = encabezados[columna];
        }

        int fila = 2;

        foreach (var t in transacciones)
        {
            hoja.Cell(fila, 1).Value = t.TipoTransaccion ?? "";
            hoja.Cell(fila, 2).Value = t.TipoMensaje ?? "";

            if (t.FechaHoraProcesamiento.HasValue)
                hoja.Cell(fila, 3).Value = t.FechaHoraProcesamiento.Value;

            hoja.Cell(fila, 4).Value = t.NombreBanco ?? "";
            hoja.Cell(fila, 5).Value = t.IdBancoReceptor ?? "";
            hoja.Cell(fila, 6).Value = t.TipoCuenta ?? "";
            hoja.Cell(fila, 7).Value = t.DescripcionTipoCuenta ?? "";
            hoja.Cell(fila, 8).Value = t.CuentaOrigen ?? "";
            hoja.Cell(fila, 9).Value = t.TipoCuentaDestino ?? "";
            hoja.Cell(fila, 10).Value = t.DescripcionTipoCuentaDestino ?? "";
            hoja.Cell(fila, 11).Value = t.CuentaDestino ?? "";
            hoja.Cell(fila, 12).Value = t.CodigoMoneda ?? "";

            if (t.Monto.HasValue)
                hoja.Cell(fila, 13).Value = t.Monto.Value;

            if (t.MontoDebitoCoreBancario.HasValue)
                hoja.Cell(fila, 14).Value = t.MontoDebitoCoreBancario.Value;

            if (t.MontoCreditoCoreBancario.HasValue)
                hoja.Cell(fila, 15).Value = t.MontoCreditoCoreBancario.Value;

            if (t.TipoCambioCoreBancario.HasValue)
                hoja.Cell(fila, 16).Value = t.TipoCambioCoreBancario.Value;

            hoja.Cell(fila, 17).Value = t.CodigoMotivo ?? "";
            hoja.Cell(fila, 18).Value = t.Resultados ?? "";
            hoja.Cell(fila, 19).Value = t.Estado ?? "";
            hoja.Cell(fila, 20).Value = t.DescripcionEstado ?? "";
            hoja.Cell(fila, 21).Value = t.IdInstruccion ?? "";
            hoja.Cell(fila, 22).Value = t.IdMensaje ?? "";
            hoja.Cell(fila, 23).Value = t.IdTransaccion ?? "";
            hoja.Cell(fila, 24).Value = t.TicketAch ?? "";

            fila++;
        }

        //formato a los encabezados
        var encabezado = hoja.Range(1, 1, 1, 24);
        encabezado.Style.Font.Bold = true;
        encabezado.Style.Font.FontColor = XLColor.White;
        encabezado.Style.Fill.BackgroundColor = XLColor.DarkGreen;

        hoja.Columns().AdjustToContents();

        using var memoria = new MemoryStream();

        libro.SaveAs(memoria);

        return memoria.ToArray();
    }

    // Obtener monto total de transacciones exitosas
    public async Task<decimal> ObtenerMontoExitosoAsync(
        DateTime fechaInicial, DateTime fechaFinal)
    {
        var inicio = fechaInicial.Date;
        var fin = fechaFinal.Date.AddDays(1);

        return await _contexto.TransaccionesACH
            .Where(t =>
                t.FechaHoraProcesamiento >= inicio &&
                t.FechaHoraProcesamiento < fin &&
                t.DescripcionEstado == "SUCCESS")
            .SumAsync(t => (decimal?)t.Monto) ?? 0;
    }

    // Obtener cantidad de todas las transacciones
    public async Task<int> ObtenerCantidadTransaccionesAsync(
        DateTime fechaInicial, DateTime fechaFinal)
    {
        var inicio = fechaInicial.Date;
        var fin = fechaFinal.Date.AddDays(1);

        return await _contexto.TransaccionesACH
            .CountAsync(t =>
                t.FechaHoraProcesamiento >= inicio &&
                t.FechaHoraProcesamiento < fin);
    }

    // Obtener cantidad de transacciones fallidas
    public async Task<int> ObtenerCantidadFallidasAsync(
        DateTime fechaInicial, DateTime fechaFinal)
    {
        var inicio = fechaInicial.Date;
        var fin = fechaFinal.Date.AddDays(1);

        return await _contexto.TransaccionesACH
            .CountAsync(t =>
                t.FechaHoraProcesamiento >= inicio &&
                t.FechaHoraProcesamiento < fin &&
                t.DescripcionEstado == "FAILED");
    }

    // Obtener monto total de transacciones fallidas
    public async Task<decimal> ObtenerMontoFallidoAsync(
        DateTime fechaInicial, DateTime fechaFinal)
    {
        var inicio = fechaInicial.Date;
        var fin = fechaFinal.Date.AddDays(1);

        return await _contexto.TransaccionesACH
            .Where(t =>
                t.FechaHoraProcesamiento >= inicio &&
                t.FechaHoraProcesamiento < fin &&
                t.DescripcionEstado == "FAILED")
            .SumAsync(t => (decimal?)t.Monto) ?? 0;
    }

    public async Task<int> ObtenerCantidadExitosasAsync(
        DateTime fechaInicial, DateTime fechaFinal)
    {
        var inicio = fechaInicial.Date;
        var fin = fechaFinal.Date.AddDays(1);

        return await _contexto.TransaccionesACH
            .CountAsync(t =>
                t.FechaHoraProcesamiento >= inicio &&
                t.FechaHoraProcesamiento < fin &&
                t.DescripcionEstado == "SUCCESS");
    }

    public async Task<List<TransaccionACH>> ObtenerTransaccionesRecientesAsync(
        DateTime fechaInicial, DateTime fechaFinal)
    {
        var inicio = fechaInicial.Date;
        var fin = fechaFinal.Date.AddDays(1);

        return await _contexto.TransaccionesACH
            .AsNoTracking()
            .Where(t =>
                t.FechaHoraProcesamiento >= inicio &&
                t.FechaHoraProcesamiento < fin)
            .OrderByDescending(t => t.FechaHoraProcesamiento)
            .Take(10)
            .ToListAsync();
    }
}