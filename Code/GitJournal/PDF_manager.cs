using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Diagnostics;

namespace GitJournal
{
    public class PDF_manager
    {
        Controller _controller;

        public PDF_manager(Controller controller)
        {
            _controller = controller;
        }

        public void createPDF(string writer, string repoName,string filePath)
        {
            bool isUserNotConnected = false;
            string fileName = "";
            if (repoName == null)
            {
                fileName = "GitJournal_JDT.pdf";
                isUserNotConnected = true;
            }
            else
                fileName = $"{repoName.Split("/")[1]}_JDT.pdf";
            string headerText = "Journal de travail";

            double marginTop = 50;
            double marginBottom = 50;
            double DefaultrowHeight = 25;
            double rowHeight = DefaultrowHeight;
            double col1Width = 200;
            double col2Width = 400;
            double col3Width = 75;
            double col4Width = 50;
            double col5Width = 50;
            double tableWidth = col1Width + col2Width + col3Width + col4Width + col5Width;

            var dates = Enumerable.Range(0, 100)
                .Select(i => DateTime.Today.AddDays(i).ToString("yyyy-MM-dd"))
                .ToArray();

            int margin = 3;

            bool displayUser = _controller._JDT._UserToDisplay.Count > 1;

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Landscape;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Arial", 12);
            XFont dateFont = new XFont("Arial", 20);
            DrawHeader(gfx, page, headerText);
            DrawFooter(gfx, page, writer, fileName);

            double currentY = marginTop;
            bool displayDay = false;

            foreach (Commit_Info[] commitGroupByDay in _controller._JDTmanager.SplitCommitsByDay())
            {
                int totalRows = 1 + commitGroupByDay.Length; // header + data rows
                double tableHeight = 0;
                double verticalGap = 20;

                // Check if the table fits on the current page; else add a new page
                if (currentY + DefaultrowHeight * 5 + marginBottom > page.Height)
                {
                    Debug.WriteLine(".................\nNew Page (Start table) !!\n.................\n");
                    // Debug.WriteLine($"currentY:{currentY}\rowHeight:{rowHeight}\nmarginBottom:{marginBottom}\n== {currentY + rowHeight + marginBottom}\npage.Height:{page.Height}\n-------------------------");

                    // Add new page
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Landscape;
                    gfx = XGraphics.FromPdfPage(page);
                    DrawHeader(gfx, page, headerText);
                    DrawFooter(gfx, page, writer, fileName);
                    currentY = marginTop;
                }

                double startX = (page.Width - tableWidth) / 2;


                // string dateText = dates[a];
                // Then draw the date text on top
                gfx.DrawString(commitGroupByDay[0].Date.ToString("dd MMMM yyyy"), dateFont, XBrushes.DarkSlateGray,
                    new XRect(startX, currentY, tableWidth, rowHeight),
                    XStringFormats.TopLeft);
                tableHeight += rowHeight;
                currentY += 30;

                // Draw header background first
                gfx.DrawRectangle(XBrushes.LightGray, startX, currentY, tableWidth, rowHeight);
                tableHeight += rowHeight;

                // Draw header text
                gfx.DrawString("Titre", font, XBrushes.Black, new XRect(startX, currentY, col1Width, rowHeight), XStringFormats.CenterLeft);
                if (displayUser)
                {
                    gfx.DrawString("Contenu", font, XBrushes.Black, new XRect(startX + col1Width, currentY, col2Width, rowHeight), XStringFormats.CenterLeft);
                    gfx.DrawString("Utilisateur", font, XBrushes.Black, new XRect(startX + col1Width+col2Width, currentY, col3Width, rowHeight), XStringFormats.CenterLeft);
                }else
                    gfx.DrawString("Contenu", font, XBrushes.Black, new XRect(startX+col1Width, currentY, col2Width+ col3Width, rowHeight), XStringFormats.CenterLeft);

                gfx.DrawString("Status", font, XBrushes.Black, new XRect(startX + col1Width+ col2Width + col3Width, currentY, col4Width, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString("Durée", font, XBrushes.Black, new XRect(startX + col1Width + col2Width + col3Width + col5Width, currentY, col3Width, rowHeight), XStringFormats.CenterLeft);

                // Draw data rows
                int counter = 0;
                double nextY = currentY+ rowHeight;
                bool justReturned = false;
                // Debug.WriteLine($"\ncurrentY : {currentY}\rowHeight : {rowHeight}\nextY : {nextY}");
                foreach (Commit_Info SingleCommit in commitGroupByDay)
                {
                    if (_controller._JDT._UserToDisplay.Contains(SingleCommit.User) || isUserNotConnected)
                    {


                        // double y = currentY + rowHeight * (counter + 1);
                        List<int> WarpedLine = new List<int>();
                        WarpedLine.Add(CountWrappedLines(gfx, SingleCommit.Title, font, col1Width));
                        WarpedLine.Add(CountWrappedLines(gfx, SingleCommit.Content, font, col2Width));
                        if (displayUser)
                            WarpedLine.Add(CountWrappedLines(gfx, SingleCommit.User, font, col3Width));
                        WarpedLine.Add(CountWrappedLines(gfx, SingleCommit.Status, font, col4Width));
                        WarpedLine.Add(CountWrappedLines(gfx, SingleCommit.Duration.ToString(@"hh\:mm"), font, col5Width));

                        Debug.WriteLine($"---------------------{SingleCommit.Title} | {WarpedLine.Max()}");

                        rowHeight = DefaultrowHeight * WarpedLine.Max();

                        // Check if the table fits on the current page; else add a new page
                        if (nextY + rowHeight + marginBottom > page.Height)
                        {
                            Debug.WriteLine(".................\nNew Page (Mid table) !!\n.................\n");
                            // Debug.WriteLine($"currentY:{nextY}\rowHeight:{rowHeight}\nmarginBottom:{marginBottom}\n== {nextY + rowHeight + marginBottom}\npage.Height:{page.Height}\n-------------------------");
                            // Add new page
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Landscape;
                            gfx = XGraphics.FromPdfPage(page);
                            DrawHeader(gfx, page, headerText);
                            DrawFooter(gfx, page, writer, fileName);
                            currentY = marginTop;
                            nextY = marginTop;
                        }

                        // Debug.WriteLine($"WarpedLine.Max(): {WarpedLine.Max()}");

                        gfx.DrawRectangle(XPens.Black, startX, nextY, col1Width, rowHeight);
                        if (displayUser)
                        {
                            gfx.DrawRectangle(XPens.Black, startX + col1Width + col2Width, nextY, col3Width, rowHeight);
                            gfx.DrawRectangle(XPens.Black, startX + col1Width, nextY, col2Width, rowHeight);
                        }
                        else
                            gfx.DrawRectangle(XPens.Black, startX + col1Width, nextY, col2Width + col3Width, rowHeight);
                        gfx.DrawRectangle(XPens.Black, startX + col1Width + col2Width + col3Width, nextY, col4Width, rowHeight);
                        gfx.DrawRectangle(XPens.Black, startX + col1Width + col2Width + col3Width + col4Width, nextY, col5Width, rowHeight);


                        DrawStringWrapped(gfx, SingleCommit.Title, font, XBrushes.Black, new XRect(startX, nextY, col1Width, rowHeight), margin);
                        if (displayUser)
                        {
                            // gfx.DrawString(SingleCommit.Content, font, XBrushes.Black, new XRect(startX + col1Width + margin, y, col2Width, rowHeight), XStringFormats.CenterLeft);
                            DrawStringWrapped(gfx, SingleCommit.Content, font, XBrushes.Black, new XRect(startX + col1Width + margin, nextY, col2Width, rowHeight), margin);
                            gfx.DrawString(SingleCommit.User, font, XBrushes.Black, new XRect(startX + col1Width + col2Width, nextY, col3Width, rowHeight), XStringFormats.Center);
                        }
                        else
                            DrawStringWrapped(gfx, SingleCommit.Content, font, XBrushes.Black, new XRect(startX + col1Width + margin, nextY, col2Width + col3Width, rowHeight), margin);
                        // gfx.DrawString(SingleCommit.Content, font, XBrushes.Black, new XRect(startX + col1Width, y, col2Width+col3Width + margin, rowHeight), XStringFormats.CenterLeft);
                        gfx.DrawString(SingleCommit.Status, font, XBrushes.Black, new XRect(startX + col1Width + col2Width + col3Width, nextY, col4Width, rowHeight), XStringFormats.Center);
                        gfx.DrawString(SingleCommit.Duration.ToString(@"hh\:mm"), font, XBrushes.Black, new XRect(startX + col1Width + col2Width + col3Width + col4Width, nextY, col5Width, rowHeight), XStringFormats.Center);
                        displayDay = true;
                        counter++;
                        if (justReturned)
                        {
                            justReturned = false;
                            nextY += marginTop;
                            tableHeight = 0;
                            rowHeight = 0;
                        }
                        else
                        {
                            nextY += rowHeight;
                            tableHeight += rowHeight;
                            rowHeight = DefaultrowHeight;
                        }
                        WarpedLine.Clear();
                    }
                }
                // Debug.WriteLine($"tableHeight : {tableHeight}\rowHeight : {rowHeight}\n");
                rowHeight = DefaultrowHeight;
                currentY = nextY+ verticalGap;
            }

            // Save or use document here, e.g.

            document.Save(_controller.GetUniqueFilePath($"{filePath}\\{fileName}"));
        }

        private void DrawHeader(XGraphics gfx, PdfPage page, string headerText)
        {
            XFont headerFont = new XFont("Arial", 14, XFontStyle.Bold);
            gfx.DrawString(headerText, headerFont, XBrushes.Black,
                new XRect(0, 20, page.Width, 30), XStringFormats.TopCenter);
        }

        private void DrawFooter(XGraphics gfx, PdfPage page, string writerName, string fileName)
        {
            writerName ??= "";
            XFont footerFont = new XFont("Arial", 10, XFontStyle.Regular);
            double margin = 40;
            double yPos = page.Height - 40;

            // Left: Date 
            string dateText = DateTime.Now.ToString("dd-MM-yyyy");
            gfx.DrawString(dateText, footerFont, XBrushes.Gray, new XPoint(margin, yPos));

            // Center: Writer Name
            gfx.DrawString(writerName, footerFont, XBrushes.Gray,
                new XRect(0, yPos - 10, page.Width, 20), XStringFormats.BottomCenter);

            // Right: File Name
            gfx.DrawString(fileName, footerFont, XBrushes.Gray,
                new XPoint(page.Width - margin, yPos), XStringFormats.BottomRight);
        }

        void DrawStringWrapped(XGraphics gfx, string text, XFont font, XBrush brush, XRect rect, double margin)
        {
            var allLines = new List<string>();

            // Split by existing line breaks first
            var paragraphs = text.Replace("\r\n", "\n") // Normalize line breaks
                     .Split('\n')
                     .Where(p => !string.IsNullOrWhiteSpace(p)) // Ignore empty lines
                     .ToList();

            foreach (var paragraph in paragraphs)
            {
                paragraph.Replace("\r\n", "");
                paragraph.Replace("\n", "");
                var words = paragraph.Split(' ');
                string currentLine = "";

                foreach (var word in words)
                {
                    string testLine = currentLine.Length == 0 ? word : currentLine + " " + word;
                    var size = gfx.MeasureString(testLine, font);
                    if (size.Width > rect.Width - 2 * margin)
                    {
                        if (currentLine.Length > 0)
                        {
                            allLines.Add(currentLine);
                            currentLine = word;
                        }
                        else
                        {
                            // Single very long word: force break
                            allLines.Add(word);
                            currentLine = "";
                        }
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }
                if (!string.IsNullOrEmpty(currentLine))
                    allLines.Add(currentLine);
            }

            double lineHeight = font.GetHeight();
            double y = rect.Y + margin;
            foreach (var line in allLines)
            {
                gfx.DrawString(line, font, brush, new XRect(rect.X + margin, y, rect.Width, lineHeight), XStringFormats.CenterLeft);
                y += lineHeight;
            }
        }


        int CountWrappedLines(XGraphics gfx, string text, XFont font, double maxWidth)
        {
            var paragraphs = text.Replace("\r\n", "\n") // Normalize line breaks
                                 .Split('\n')
                                 .Where(p => !string.IsNullOrWhiteSpace(p)) // Ignore empty lines
                                 .ToList();

            int totalLines = 0;

            foreach (var paragraph in paragraphs)
            {
                var words = paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string currentLine = "";
                int lineCount = 1;

                foreach (var word in words)
                {
                    string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    var size = gfx.MeasureString(testLine+"a", font);

                    if (size.Width > maxWidth)
                    {
                        totalLines++; // Commit current line
                        currentLine = word;

                        // Break up long word if needed
                        var wordSize = gfx.MeasureString(word, font);
                        if (wordSize.Width > maxWidth)
                        {
                            int charsPerLine = 1;
                            for (int i = 1; i <= word.Length; i++)
                            {
                                if (gfx.MeasureString(word.Substring(0, i), font).Width > maxWidth)
                                {
                                    charsPerLine = i - 1;
                                    break;
                                }
                            }

                            if (charsPerLine == 0) charsPerLine = 1;
                            int additional = (int)Math.Ceiling((double)word.Length / charsPerLine);
                            totalLines += additional - 1;
                            currentLine = "";
                        }
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                    totalLines++;
            }

            return totalLines;
        }


    }
}
