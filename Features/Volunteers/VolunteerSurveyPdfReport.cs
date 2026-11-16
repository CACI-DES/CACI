using System;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CACI_Life.Features.Volunteers
{
    public class VolunteerSurveyPdfReport : IDocument
    {
        private readonly VolunteerSurveyEntry _entry;
        private readonly SurveyDefinition _definition;

        public VolunteerSurveyPdfReport(VolunteerSurveyEntry entry)
        {
            _entry = entry;
            _definition = SurveyDefinition.Get(entry.Team);
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(36);
                page.Size(PageSizes.Letter);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial").LineHeight(1.2f));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.PaddingBottom(10).BorderBottom(1.5f).BorderColor("#1E3A8A").Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("COVENANT APOSTOLIC CONGREGATIONS, INT.")
                        .FontColor("#475569")
                        .FontSize(9)
                        .Bold()
                        .LetterSpacing(0.05f);

                    col.Item().Text($"{_definition.Title} VOLUNTEER SURVEY")
                        .FontColor("#1E3A8A")
                        .FontSize(16)
                        .ExtraBold();

                    col.Item().PaddingTop(2).Text($"Volunteer: {_entry.FullName}")
                        .FontColor("#0F172A")
                        .FontSize(11)
                        .Bold();

                    col.Item().PaddingTop(2).Text(_definition.IntroParagraph)
                        .FontSize(7.5f)
                        .FontColor("#475569")
                        .Italic();
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(10).Row(row =>
            {
                // Left Column
                row.RelativeItem(1).Column(col =>
                {
                    // 1. BASIC INFORMATION
                    ComposeSectionHeader(col, "1", "BASIC INFORMATION");
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        RenderField(inner, "Full Name", _entry.FullName);
                        RenderField(inner, "Phone Number", _entry.PhoneNumber);
                        RenderField(inner, "Email Address", _entry.EmailAddress);
                        RenderField(inner, "Preferred Contact", _entry.PreferredContact);
                    });

                    // 2. SPIRITUAL & PERSONAL BACKGROUND
                    ComposeSectionHeader(col, "2", "SPIRITUAL & PERSONAL BACKGROUND");
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        RenderField(inner, "Membership Length", _entry.MembershipLength);
                        RenderField(inner, "Other Ministries", _entry.OtherMinistries ?? "None");
                        RenderTextArea(inner, "Testimony / Walk with Christ", _entry.Testimony);
                    });

                    // 3. FEAST PREFERENCE
                    ComposeSectionHeader(col, "3", "FEAST PREFERENCE");
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        inner.Item().Row(r =>
                        {
                            foreach (var opt in _definition.FeastOptions)
                            {
                                var isSel = _entry.FeastPreference.Equals(opt, StringComparison.OrdinalIgnoreCase);
                                var txt = r.RelativeItem().Text($"{(isSel ? "[x]" : "[ ]")} {opt}");
                                if (isSel)
                                {
                                    txt.Bold().FontColor("#1E3A8A");
                                }
                                else
                                {
                                    txt.FontColor("#000000");
                                }
                            }
                        });
                    });

                    // 4. AVAILABILITY
                    ComposeSectionHeader(col, "4", "AVAILABILITY");
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        var availText = string.IsNullOrEmpty(_entry.Availability) ? "None selected" : _entry.Availability;
                        if (!string.IsNullOrEmpty(_entry.AvailabilityOther))
                        {
                            availText += $", Other: {_entry.AvailabilityOther}";
                        }
                        inner.Item().PaddingBottom(1).Text(availText);
                    });

                    // 5. GIFTS / TALENTS / SKILLS / AGE GROUPS
                    ComposeSectionHeader(col, "5", _definition.GiftsLabel);
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        if (_definition.IsYouthMinistry)
                        {
                            // Render age group checkboxes
                            foreach (var opt in _definition.GiftsOptions)
                            {
                                var isChecked = IsChecked(_entry.AgeGroupPreference, opt);
                                var txt = inner.Item().Text($"{(isChecked ? "[x]" : "[ ]")} {opt}");
                                if (isChecked)
                                {
                                    txt.Bold().FontColor("#1E3A8A");
                                }
                                else
                                {
                                    txt.FontColor("#000000");
                                }
                            }
                        }
                        else
                        {
                            // Render standard skills checkboxes in a tighter grid
                            foreach (var opt in _definition.GiftsOptions)
                            {
                                var isChecked = IsChecked(_entry.GiftsTalentsSkills, opt);
                                var txt = inner.Item().PaddingBottom(1).Text($"{(isChecked ? "[x]" : "[ ]")} {opt}");
                                if (isChecked)
                                {
                                    txt.Bold().FontColor("#1E3A8A");
                                }
                                else
                                {
                                    txt.FontColor("#000000");
                                }
                            }
                        }
                    });

                    // 6. EXPERIENCE
                    ComposeSectionHeader(col, "6", "EXPERIENCE");
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        if (_definition.IsYouthMinistry)
                        {
                            RenderTextArea(inner, _definition.GiftsExperienceLabel, _entry.GiftsExperienceText);
                            RenderTextArea(inner, _definition.ExperienceQuestion, _entry.ExperienceText);
                        }
                        else
                        {
                            RenderTextArea(inner, _definition.ExperienceQuestion, _entry.ExperienceText);
                        }
                    });
                });

                // Gap
                row.ConstantItem(20);

                // Right Column
                row.RelativeItem(1).Column(col =>
                {
                    // 7. AREAS OF INTEREST
                    ComposeSectionHeader(col, "7", "AREAS OF INTEREST");
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        foreach (var group in _definition.InterestGroups)
                        {
                            inner.Item().PaddingTop(3).PaddingBottom(1).Text(group.Name).Bold().FontSize(8).FontColor("#1E3A8A");
                            foreach (var item in group.Items)
                            {
                                var isChecked = IsChecked(_entry.AreasOfInterest, item);
                                var txt = inner.Item().PaddingLeft(6).PaddingBottom(1).Text($"{(isChecked ? "[x]" : "[ ]")} {item}");
                                if (isChecked)
                                {
                                    txt.Bold().FontColor("#1E3A8A");
                                }
                                else
                                {
                                    txt.FontColor("#000000");
                                }
                            }
                        }
                    });

                    // 8. COMFORT LEVEL & TRAINING
                    ComposeSectionHeader(col, "8", "COMFORT LEVEL & TRAINING");
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        inner.Item().Text("I feel most comfortable:").Bold().FontSize(8).FontColor("#475569");
                        foreach (var opt in _definition.ComfortOptions)
                        {
                            var isChecked = IsChecked(_entry.ComfortLevelChoice, opt);
                            var txt = inner.Item().PaddingLeft(6).PaddingBottom(1).Text($"{(isChecked ? "[x]" : "[ ]")} {opt}");
                            if (isChecked)
                            {
                                txt.Bold().FontColor("#1E3A8A");
                            }
                            else
                            {
                                txt.FontColor("#000000");
                            }
                        }

                        inner.Item().PaddingTop(4).Text("Open to new roles with training?").Bold().FontSize(8).FontColor("#475569");
                        inner.Item().Row(r =>
                        {
                            foreach (var opt in new[] { "Yes", "No", "Maybe" })
                            {
                                var isSel = _entry.OpenToTraining.Equals(opt, StringComparison.OrdinalIgnoreCase);
                                var txt = r.RelativeItem().Text($"{(isSel ? "[x]" : "[ ]")} {opt}");
                                if (isSel)
                                {
                                    txt.Bold().FontColor("#1E3A8A");
                                }
                                else
                                {
                                    txt.FontColor("#000000");
                                }
                            }
                        });
                    });

                    // 9. HEART & VISION
                    ComposeSectionHeader(col, "9", "HEART & VISION");
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        RenderTextArea(inner, _definition.HeartVisionQuestion1, _entry.HeartVision1);
                        RenderTextArea(inner, _definition.HeartVisionQuestion2, _entry.HeartVision2);
                    });

                    // 10. ADDITIONAL INFORMATION
                    ComposeSectionHeader(col, "10", "ADDITIONAL INFORMATION");
                    col.Item().PaddingBottom(6).Column(inner =>
                    {
                        RenderTextArea(inner, "Restrictions, preferences, or concerns:", _entry.Restrictions);
                        RenderField(inner, "Specific drawn role", _entry.SpecificRole ?? "None");
                    });

                    // 11. FINAL STEP
                    ComposeSectionHeader(col, "11", "FINAL STEP");
                    col.Item().Column(inner =>
                    {
                        inner.Item().PaddingBottom(3).Text("Are you willing to complete a background check (if required)?").Bold().FontSize(8);
                        
                        var isYes = _entry.FinalStepChoice.Equals("Yes", StringComparison.OrdinalIgnoreCase);
                        var isNo = _entry.FinalStepChoice.Equals("No", StringComparison.OrdinalIgnoreCase);
                        
                        inner.Item().Row(r =>
                        {
                            var txtYes = r.ConstantItem(60).Text($"{(isYes ? "[x]" : "[ ]")} Yes");
                            if (isYes) txtYes.Bold().FontColor("#1E3A8A");
                            else txtYes.FontColor("#000000");

                            var txtNo = r.ConstantItem(60).Text($"{(isNo ? "[x]" : "[ ]")} No");
                            if (isNo) txtNo.Bold().FontColor("#1E3A8A");
                            else txtNo.FontColor("#000000");
                        });

                        inner.Item().PaddingTop(6).Text(_definition.ClosingText)
                            .FontSize(8)
                            .FontColor("#1E3A8A")
                            .Bold()
                            .Italic();
                    });
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.PaddingTop(5).BorderTop(1).BorderColor("#CBD5E1").Row(row =>
            {
                row.RelativeItem().Text($"Submitted on: {_entry.SubmissionDate.ToLocalTime():MM/dd/yyyy hh:mm tt}")
                    .FontSize(8)
                    .FontColor("#64748B");

                row.RelativeItem().AlignRight().Text("Page 1 of 1")
                    .FontSize(8)
                    .FontColor("#64748B");
            });
        }

        private void ComposeSectionHeader(ColumnDescriptor column, string number, string title)
        {
            column.Item().PaddingTop(6).PaddingBottom(4).Background("#1E3A8A").PaddingLeft(4).PaddingRight(4).PaddingTop(2).PaddingBottom(2).Row(row =>
            {
                row.ConstantItem(15).Text(number).Bold().FontSize(8.5f).FontColor("#FFFFFF");
                row.RelativeItem().Text(title).Bold().FontSize(8.5f).FontColor("#FFFFFF");
            });
        }

        private void RenderField(ColumnDescriptor column, string label, string value)
        {
            column.Item().PaddingTop(2).Row(row =>
            {
                row.ConstantItem(85).Text(label + ":").Bold().FontColor("#475569");
                row.RelativeItem().Text(value ?? string.Empty);
            });
        }

        private void RenderTextArea(ColumnDescriptor column, string label, string? value)
        {
            column.Item().PaddingTop(3).Column(inner =>
            {
                inner.Item().Text(label).Bold().FontColor("#475569");
                inner.Item().PaddingLeft(4).BorderLeft(1).BorderColor("#CBD5E1").PaddingLeft(4)
                    .Text(string.IsNullOrWhiteSpace(value) ? "No answer provided." : value)
                    .FontColor(string.IsNullOrWhiteSpace(value) ? "#94A3B8" : "#000000")
                    .Italic(string.IsNullOrWhiteSpace(value));
            });
        }

        private bool IsChecked(string? field, string option)
        {
            if (string.IsNullOrEmpty(field)) return false;
            return field.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Any(x => x.Equals(option, StringComparison.OrdinalIgnoreCase));
        }
    }
}
