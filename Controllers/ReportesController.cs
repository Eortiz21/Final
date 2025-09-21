using Microsoft.AspNetCore.Mvc;
using Primera.Models;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Primera.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ==============================
        // Exportar Excel (Vehículos y Espacios)
        // ==============================
        public IActionResult ExportarExcel()
        {
            var vehiculos = _context.Vehiculos.ToList();
            var espacios = _context.EspacioEstacionamientos.ToList();

            using (var workbook = new XLWorkbook())
            {
                // Hoja Vehículos
                var wsVehiculos = workbook.Worksheets.Add("Vehículos");
                wsVehiculos.Cell(1, 1).Value = "Placa";
                wsVehiculos.Cell(1, 2).Value = "Marca";
                wsVehiculos.Cell(1, 3).Value = "Color";
                wsVehiculos.Cell(1, 4).Value = "Tipo";

                int row = 2;
                foreach (var v in vehiculos)
                {
                    wsVehiculos.Cell(row, 1).Value = v.NoPlaca;
                    wsVehiculos.Cell(row, 2).Value = v.Marca;
                    wsVehiculos.Cell(row, 3).Value = v.Color;
                    wsVehiculos.Cell(row, 4).Value = v.TipoVehiculo?.Descripcion;
                    row++;
                }

                // Hoja Espacios de Parqueo
                var wsEspacios = workbook.Worksheets.Add("Espacios");
                wsEspacios.Cell(1, 1).Value = "ID";
                wsEspacios.Cell(1, 2).Value = "Número de Espacio";
                wsEspacios.Cell(1, 3).Value = "Nivel";
                wsEspacios.Cell(1, 4).Value = "Tipo";
                wsEspacios.Cell(1, 5).Value = "Estado";

                row = 2;
                foreach (var e in espacios)
                {
                    wsEspacios.Cell(row, 1).Value = e.Id_Espacio;
                    wsEspacios.Cell(row, 2).Value = e.No_Espacio;
                    wsEspacios.Cell(row, 3).Value = e.Nivel;
                    wsEspacios.Cell(row, 4).Value = e.TipoEspacio;
                    wsEspacios.Cell(row, 5).Value = e.Estado;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "ReporteCompleto.xlsx");
                }
            }
        }

        // ==============================
        // Exportar PDF usando iTextSharp
        // ==============================
        public IActionResult ExportarPdf()
        {
            var vehiculos = _context.Vehiculos.ToList();
            var espacios = _context.EspacioEstacionamientos.ToList();

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(document, stream);
                document.Open();

                // Vehículos
                document.Add(new Paragraph("=== Vehículos ==="));
                foreach (var v in vehiculos)
                {
                    document.Add(new Paragraph($"{v.NoPlaca} - {v.Marca} - {v.Color} - {v.TipoVehiculo?.Descripcion}"));
                }

                document.Add(new Paragraph("\n=== Espacios de Parqueo ==="));
                foreach (var e in espacios)
                {
                    document.Add(new Paragraph($"{e.Id_Espacio} - {e.No_Espacio} - {e.Nivel} - {e.TipoEspacio} - {e.Estado}"));
                }

                document.Close();

                return File(stream.ToArray(), "application/pdf", "ReporteCompleto.pdf");
            }
        }
    }
}
