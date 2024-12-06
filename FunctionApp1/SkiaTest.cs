using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.IO;

namespace SkiaTest
{
    public class SkiaTest
    {
        private readonly ILogger _logger;

        public SkiaTest(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SkiaTest>();
        }

        [Function("SkiaTest")]
        public void Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            var path = Path.GetTempPath();
            var outputFile = Path.Combine(path, "report.pdf");

            _logger.LogInformation("Writing to file...");
            // Create a PDF document
            var metadata = new SKDocumentPdfMetadata
            {
                Author = "Your Name",
                Title = "Sample PDF Report",
                Creation = DateTime.Now,
                Modified = DateTime.Now
            };

            using (var stream = File.Create(outputFile))
            {
                var document = SKDocument.CreatePdf(stream, metadata);
                using (var canvas = document.BeginPage(612, 792)) // A4 size in points
                {
                    canvas.Clear(SKColors.White);
                    using (var paint = new SKPaint
                    {
                        Color = SKColors.Black
                   })
                    {
                        using (var font = new SKFont
                        {
                            Size = 24,
                        })
                        {
                            canvas.DrawText("Hello, World!", 100, 100,
                                SKTextAlign.Left, font, paint);
                        }
                    }
                }
                document.EndPage();
                document.Close();
            }
            _logger.LogInformation($"File written to: {outputFile}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }

    }
}
