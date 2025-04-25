using QRCoder;
using QuestPDF.Companion;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var svg = SvgImage.FromFile("rockaway-logotype.svg");

using var stream = File.OpenRead("PTSansNarrow-Regular.ttf");
FontManager.RegisterFontWithCustomName("PT Sans", stream);

byte[] qrCodeBytes;

using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
using (QRCodeData qrCodeData
	= qrGenerator.CreateQrCode(
		Guid.NewGuid().ToString(),
		QRCodeGenerator.ECCLevel.Q))
using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData)) {
	qrCodeBytes = qrCode.GetGraphic(20);
}

void DrawTicket(ColumnDescriptor column) {
	column.Spacing(30);
	column.Item().Height(13, Unit.Centimetre).Border(1).Row(row => {
		row.ConstantItem(12, Unit.Centimetre)
			.Padding(20).Column(column => {
				column.Spacing(10);
				column.Item().Width(150).Svg(svg).FitWidth();
				column.Item().Text("gig tickets for good people").FontSize(17);
				column.Item().BorderTop(1).Height(130).Column(c => {
					c.Item().PaddingTop(10).Text("The Silver Mountain String Band ft Simon J. MacCorkindale and the Hill Valley Hornswogglers").FontSize(20).Bold();
					c.Item().Text("plus special guests: ᵾnɨȼøđɇɍ, <BODY>BAG");
				});
				column.Item().Text("Saturday 15 October 2025. Doors 7pm").Bold();
				column.Item().Text(text => {
					text.Span("John Dee ").FontSize(16).Bold();
					text.Span("Torggata 16, Oslo 0181, Norway");
				});
				column.Item().Text(text => {
					text.Span("+47 12 34 5678890");
					text.Span(" • ");
					text.Span("https://johndee.no");
				});
				column.Item().Text("General Admission").Bold().FontSize(20);

			});
		row.RelativeItem().Column(column => {
			column.Item().AlignRight().Image(qrCodeBytes).FitWidth();
			column.Item().AlignCenter()
			.Text(Guid.NewGuid().ToString())
			.FontSize(8).FontFamily("Consolas");
		});
	});
}
var pdf = Document.Create(container => {
	container.Page(page => {
		page.Size(PageSizes.A4);
		page.DefaultTextStyle(text => text.FontFamily("PT Sans").FontSize(14));
		page.Margin(1, Unit.Centimetre);
		page.Content().Column(column => {
			DrawTicket(column);
			DrawTicket(column);
			DrawTicket(column);
			DrawTicket(column);
			DrawTicket(column);
		});
	});
});

pdf.ShowInCompanion();