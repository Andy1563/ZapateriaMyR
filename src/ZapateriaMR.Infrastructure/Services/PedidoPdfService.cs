using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZapateriaMR.Application.DTOs.Pedidos;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Infrastructure.Services;

public class PedidoPdfService : IPedidoPdfService
{
    public Task<byte[]> GenerarPedidoPdfAsync(PedidoDetalleDto pedido)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(container => ComposeHeader(container, pedido));

                page.Content().Element(container => ComposeContent(container, pedido));

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Zapatería M y R - Comprobante generado automáticamente - Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
            });
        }).GeneratePdf();

        return Task.FromResult(pdf);
    }

    private static void ComposeHeader(IContainer container, PedidoDetalleDto pedido)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text("Zapatería M y R")
                        .FontSize(22)
                        .Bold()
                        .FontColor("#E04E1A");

                    left.Item().Text("Comprobante de pedido")
                        .FontSize(12)
                        .FontColor(Colors.Grey.Darken2);
                });

                row.ConstantItem(190).Column(right =>
                {
                    right.Item().AlignRight().Text(pedido.NumeroPedido)
                        .FontSize(13)
                        .Bold();

                    right.Item().AlignRight().Text($"Fecha: {pedido.FechaPedido.ToLocalTime():dd/MM/yyyy HH:mm}")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken2);

                    right.Item().AlignRight().Text($"Estado: {ObtenerEstadoTexto(pedido.Estado)}")
                        .FontSize(9)
                        .Bold();
                });
            });

            column.Item().PaddingTop(12).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private static void ComposeContent(IContainer container, PedidoDetalleDto pedido)
    {
        container.PaddingTop(20).Column(column =>
        {
            column.Spacing(18);

            column.Item().Element(c => ComposeCustomerInfo(c, pedido));

            column.Item().Element(c => ComposeProductsTable(c, pedido));

            column.Item().AlignRight().Width(260).Element(c => ComposeTotals(c, pedido));

            if (!string.IsNullOrWhiteSpace(pedido.Observaciones))
            {
                column.Item().Background(Colors.Grey.Lighten4).Padding(12).Column(obs =>
                {
                    obs.Item().Text("Observaciones").Bold();
                    obs.Item().Text(pedido.Observaciones);
                });
            }
        });
    }

    private static void ComposeCustomerInfo(IContainer container, PedidoDetalleDto pedido)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(column =>
        {
            column.Item().Text("Información del cliente").FontSize(13).Bold();

            column.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text("Cliente").FontColor(Colors.Grey.Darken2);
                    left.Item().Text(pedido.NombreCliente).Bold();

                    left.Item().PaddingTop(8).Text("Correo").FontColor(Colors.Grey.Darken2);
                    left.Item().Text(string.IsNullOrWhiteSpace(pedido.CorreoCliente) ? "No registrado" : pedido.CorreoCliente);
                });

                row.RelativeItem().Column(right =>
                {
                    right.Item().Text("Teléfono").FontColor(Colors.Grey.Darken2);
                    right.Item().Text(string.IsNullOrWhiteSpace(pedido.TelefonoCliente) ? "No registrado" : pedido.TelefonoCliente);

                    right.Item().PaddingTop(8).Text("Entrega estimada").FontColor(Colors.Grey.Darken2);
                    right.Item().Text(pedido.FechaEntregaEstimada.HasValue
                        ? pedido.FechaEntregaEstimada.Value.ToString("dd/MM/yyyy")
                        : "No registrada");
                });
            });

            column.Item().PaddingTop(8).Text("Dirección de entrega").FontColor(Colors.Grey.Darken2);
            column.Item().Text(string.IsNullOrWhiteSpace(pedido.DireccionEntrega)
                ? "No registrada"
                : pedido.DireccionEntrega);
        });
    }

    private static void ComposeProductsTable(IContainer container, PedidoDetalleDto pedido)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(4);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1.3f);
                columns.RelativeColumn(1.4f);
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("Producto");
                header.Cell().Element(HeaderCell).Text("SKU");
                header.Cell().Element(HeaderCell).AlignRight().Text("Cant.");
                header.Cell().Element(HeaderCell).AlignRight().Text("Precio");
                header.Cell().Element(HeaderCell).AlignRight().Text("Subtotal");
            });

            foreach (var detalle in pedido.Detalles)
            {
                table.Cell().Element(BodyCell).Text(detalle.NombreProducto);
                table.Cell().Element(BodyCell).Text(detalle.CodigoSku);
                table.Cell().Element(BodyCell).AlignRight().Text(detalle.Cantidad.ToString());
                table.Cell().Element(BodyCell).AlignRight().Text($"₡{detalle.PrecioUnitario:N2}");
                table.Cell().Element(BodyCell).AlignRight().Text($"₡{detalle.Subtotal:N2}");
            }
        });
    }

    private static void ComposeTotals(IContainer container, PedidoDetalleDto pedido)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Subtotal").FontColor(Colors.Grey.Darken2);
                row.ConstantItem(120).AlignRight().Text($"₡{pedido.Subtotal:N2}").Bold();
            });

            column.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            column.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Text("Total").FontSize(13).Bold();
                row.ConstantItem(120).AlignRight().Text($"₡{pedido.Total:N2}")
                    .FontSize(13)
                    .Bold()
                    .FontColor("#E04E1A");
            });
        });
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container
            .Background("#E04E1A")
            .PaddingVertical(6)
            .PaddingHorizontal(5)
            .DefaultTextStyle(x => x.FontColor(Colors.White).Bold());
    }

    private static IContainer BodyCell(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten3)
            .PaddingVertical(6)
            .PaddingHorizontal(5);
    }

    private static string ObtenerEstadoTexto(EstadoPedido estado)
    {
        return estado switch
        {
            EstadoPedido.Pendiente => "Pendiente",
            EstadoPedido.Entregado => "Entregado",
            EstadoPedido.Cancelado => "Cancelado",
            _ => estado.ToString()
        };
    }
}