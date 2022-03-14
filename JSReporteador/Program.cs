// See https://aka.ms/new-console-template for more information
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using JSReporteador;
using System.Runtime.InteropServices;
using System.Text.Json;

Console.WriteLine("Hello, World!");



var rawData = new
{
    logo=$"{Directory.GetCurrentDirectory()}/Templates/invoice_logo.png",
    number = 123,
    seller = new
    {
        name = "Wepsys, SRL.",
        road = "12345 Sunny Road",
        country = "Sunnyville, TX 12345"
    },
    buyer = new
    {
        name = "Acme Corp.",
        road = "16 Johnson Road",
        country = "Paris, France 8060"
    },
    items = new List<Items>
    {
        new Items
        {
            name = "Polux Team",
            price = 150000
        },
        new Items
        {
            name = "Database Design",
            price = 20000
        },
        new Items
        {
            name = "WebApi Design",
            price = 15000
        },
        new Items
        {
            name = "ApiRest Design",
            price = 50000
        },
        new Items
        {
            name = "ApiRest Design",
            price = 50000
        }
    }
};

var data = JsonSerializer.Serialize(rawData);

var htmlTemplatefilename = $"{Directory.GetCurrentDirectory()}/Templates/invoice.html";
var jsHelperfilename = $"{Directory.GetCurrentDirectory()}/Templates/helpers.js";
var htmlTemplate = System.IO.File.ReadAllText(htmlTemplatefilename);
var jsHelpers = System.IO.File.ReadAllText(jsHelperfilename);

Console.WriteLine(htmlTemplate);

try
{
    var rs = new LocalReporting()
            .UseBinary(JsReportBinary.GetBinary())
             .UseBinary(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                                            JsReportBinary.GetBinary() :
                                            jsreport.Binary.Linux.JsReportBinary.GetBinary())
            .KillRunningJsReportProcesses()
            .RunInDirectory(Path.Combine(Directory.GetCurrentDirectory(), "jsreport"))
            .Configure(cfg => cfg.AllowedLocalFilesAccess())
            .AsUtility()
            .Create();

    var report = await rs.RenderAsync(new RenderRequest
    {
        Template = new Template
        {
            Recipe = Recipe.ChromePdf,
            Engine = Engine.Handlebars,
            Helpers = jsHelpers,
            Content = htmlTemplate
        },
        Data = rawData

    });

    using (var fs = File.Create("D:/Reportes/Reporte.pdf"))
    {
        report.Content.CopyTo(fs);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
