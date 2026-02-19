using Sylvaro.Application.Abstractions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Sylvaro.Infrastructure.Exports;

public class PdfExportService : IExportService
{
    public Task<byte[]> GeneratePdfAsync(string title, IReadOnlyCollection<string> lines, CancellationToken cancellationToken = default)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var bronze = Color.FromHex("#5C3A21");
        var darkBrown = Color.FromHex("#3A2414");
        var lightStone = Color.FromHex("#ECE8E2");
        var charcoal = Color.FromHex("#1F1C18");

        var bytes = Document.Create(container =>
            container.Page(page =>
            {
                page.Margin(24);
                page.PageColor(Colors.White);

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(brand =>
                        {
                            brand.Item().Text("SYLVARO")
                                .FontFamily("Times New Roman")
                                .SemiBold()
                                .FontSize(22)
                                .FontColor(bronze)
                                .LetterSpacing(1.2f);
                            brand.Item().Text("Regulatory Intelligence Infrastructure")
                                .FontSize(9)
                                .FontColor(darkBrown);
                        });
                    });

                    col.Item().PaddingTop(8).LineHorizontal(1).LineColor(lightStone);
                    col.Item().PaddingTop(8).Text(title)
                        .FontSize(16)
                        .SemiBold()
                        .FontColor(darkBrown);
                });

                page.Content().PaddingVertical(12).Column(col =>
                {
                    foreach (var line in lines)
                    {
                        col.Item().PaddingBottom(4).Text(line).FontSize(11).FontColor(charcoal);
                    }
                });

                page.Footer().Column(col =>
                {
                    col.Item().LineHorizontal(1).LineColor(lightStone);
                    col.Item().PaddingTop(4).Row(row =>
                    {
                        row.RelativeItem().Text("SYLVARO Governance Export").FontSize(9).FontColor(darkBrown);
                        row.RelativeItem().AlignRight().Text($"Generated: {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss} UTC").FontSize(9).FontColor(darkBrown);
                    });
                });
            }))
            .GeneratePdf();

        return Task.FromResult(bytes);
    }
}
