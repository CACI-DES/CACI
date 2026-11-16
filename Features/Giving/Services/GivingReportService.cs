using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using CACI_Life.Features.Giving.Models;

namespace CACI_Life.Features.Giving.Services
{
    public class GivingReportService
    {
        public byte[] GenerateGivingHistoryPdf(string email, string donorName, List<OnlineGiving> history, DateTime startDate, DateTime endDate)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var items = new List<PdfHistoryItem>();
            foreach (var record in history)
            {
                var recordDate = record.CompletedAt ?? record.CreatedAt;
                if (!string.IsNullOrEmpty(record.AllocationsJson))
                {
                    try
                    {
                        var allocations = System.Text.Json.JsonSerializer.Deserialize<List<GivingAllocation>>(record.AllocationsJson);
                        if (allocations != null)
                        {
                            foreach (var alloc in allocations)
                            {
                                if (alloc.Amount > 0)
                                {
                                    items.Add(new PdfHistoryItem
                                    {
                                        Date = recordDate,
                                        CategoryMemo = alloc.AccountName + (string.IsNullOrEmpty(record.Memo) ? "" : $" - {record.Memo}"),
                                        Amount = alloc.Amount,
                                        Deductible = IsDeductible(alloc.AccountName) ? "Yes" : "No"
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        AddLegacyItems(record, recordDate, items);
                    }
                }
                else
                {
                    AddLegacyItems(record, recordDate, items);
                }
            }
            items = items.OrderByDescending(x => x.Date).ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(1, Unit.Inch);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Verdana));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("CACI Online Giving").FontSize(22).SemiBold().FontColor("#174477");
                            col.Item().Text("Official Giving Statement").FontSize(14).Medium().FontColor("#001F5C");
                        });

                        row.ConstantItem(250).AlignRight().Column(col =>
                        {
                            col.Item().Text($"Period: {startDate:MM/dd/yyyy} - {endDate:MM/dd/yyyy}").FontSize(10);
                            col.Item().Text($"Statement Date: {DateTime.Now:MM/dd/yyyy}").FontSize(10);
                            col.Item().Text($"Email: {email}").FontSize(10);
                        });
                    });

                    page.Content().PaddingVertical(20).Column(x =>
                    {
                        x.Spacing(15);

                        // Donor Info
                        x.Item().Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Donor Information").FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
                                c.Item().Text(donorName).FontSize(14).SemiBold();
                            });
                        });

                        x.Item().PaddingTop(10).Text("Giving Transactions").FontSize(16).SemiBold().FontColor("#174477");

                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);  // Date
                                columns.RelativeColumn();    // Category/Memo
                                columns.ConstantColumn(90);  // Amount
                                columns.ConstantColumn(80);  // Deductible
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Date");
                                header.Cell().Element(CellStyle).Text("Category/Memo");
                                header.Cell().Element(CellStyle).AlignRight().Text("Amount");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Deductible");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            // Rows
                            foreach (var item in items)
                            {
                                table.Cell().Element(RowStyle).Text(item.Date.ToString("MM/dd/yyyy"));
                                table.Cell().Element(RowStyle).Text(item.CategoryMemo);
                                table.Cell().Element(RowStyle).AlignRight().Text(item.Amount.ToString("C"));
                                table.Cell().Element(RowStyle).AlignCenter().Text(item.Deductible);

                                static IContainer RowStyle(IContainer container)
                                {
                                    return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                                }
                            }
                        });

                        x.Item().AlignRight().PaddingTop(20).Text(t =>
                        {
                            t.Span("Total Contributed: ").FontSize(12).SemiBold();
                            t.Span(items.Sum(p => p.Amount).ToString("C")).FontSize(12).SemiBold().FontColor(Colors.Green.Darken2);
                        });
                    });

                    page.Footer().AlignCenter().Column(col => {
                        col.Item().PaddingTop(20).BorderTop(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().Text("Covenant Apostolic Congregations Intl. | admin@caci.life").FontSize(9).FontColor(Colors.Grey.Medium);
                        col.Item().AlignCenter().Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static bool IsDeductible(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName)) return true;
            var name = categoryName.ToLower();
            if (name.Contains("dues") || name.Contains("credential") || name.Contains("fee") || name.Contains("card"))
            {
                return false;
            }
            return true;
        }

        private static void AddLegacyItems(OnlineGiving record, DateTime recordDate, List<PdfHistoryItem> list)
        {
            if (record.TitheAmount > 0)
                list.Add(new PdfHistoryItem { Date = recordDate, CategoryMemo = "Tithing" + (string.IsNullOrEmpty(record.Memo) ? "" : $" - {record.Memo}"), Amount = record.TitheAmount, Deductible = "Yes" });
            if (record.OfferingAmount > 0)
                list.Add(new PdfHistoryItem { Date = recordDate, CategoryMemo = "Offering" + (string.IsNullOrEmpty(record.Memo) ? "" : $" - {record.Memo}"), Amount = record.OfferingAmount, Deductible = "Yes" });
            if (record.DonationAmount > 0)
                list.Add(new PdfHistoryItem { Date = recordDate, CategoryMemo = "Donation" + (string.IsNullOrEmpty(record.Memo) ? "" : $" - {record.Memo}"), Amount = record.DonationAmount, Deductible = "Yes" });
            if (record.MinistryDuesAmount > 0)
                list.Add(new PdfHistoryItem { Date = recordDate, CategoryMemo = "Ministry Dues" + (string.IsNullOrEmpty(record.Memo) ? "" : $" - {record.Memo}"), Amount = record.MinistryDuesAmount, Deductible = "No" });
        }
    }

    public class PdfHistoryItem
    {
        public DateTime Date { get; set; }
        public string CategoryMemo { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Deductible { get; set; } = "Yes";
    }
}
