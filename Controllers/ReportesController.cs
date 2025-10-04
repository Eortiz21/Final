using Microsoft.AspNetCore.Mvc;
using Primera.Models;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using iTextSharp.text.pdf.draw;

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
        // Exportar Excel
        // ==============================
        [HttpPost]
        public IActionResult ExportarExcel(string graficoVehiculos, string graficoOcupacion, string graficoClientes)
        {
            var vehiculos = _context.Vehiculos.ToList();
            var espacios = _context.EspacioEstacionamientos.ToList();

            using (var workbook = new XLWorkbook())
            {
                // ----------------- Hoja Vehículos -----------------
                var wsVehiculos = workbook.Worksheets.Add("Vehículos");
                wsVehiculos.Cell("A1").Value = "Reporte de Vehículos";
                wsVehiculos.Range("A1:D1").Merge().Style
                    .Font.SetBold().Font.FontSize = 14;
                wsVehiculos.Range("A1:D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                wsVehiculos.Cell(2, 1).Value = "Placa";
                wsVehiculos.Cell(2, 2).Value = "Marca";
                wsVehiculos.Cell(2, 3).Value = "Color";
                wsVehiculos.Cell(2, 4).Value = "Tipo";

                int row = 3;
                foreach (var v in vehiculos)
                {
                    wsVehiculos.Cell(row, 1).Value = v.NoPlaca;
                    wsVehiculos.Cell(row, 2).Value = v.Marca;
                    wsVehiculos.Cell(row, 3).Value = v.Color;
                    wsVehiculos.Cell(row, 4).Value = v.TipoVehiculo?.Descripcion;
                    row++;
                }

                var tblVehiculos = wsVehiculos.Range(2, 1, row - 1, 4).CreateTable();
                tblVehiculos.Theme = XLTableTheme.TableStyleMedium9;
                wsVehiculos.Columns().AdjustToContents();

                // ----------------- Hoja Espacios -----------------
                var wsEspacios = workbook.Worksheets.Add("Espacios");
                wsEspacios.Cell("A1").Value = "Reporte de Espacios";
                wsEspacios.Range("A1:E1").Merge().Style
                    .Font.SetBold().Font.FontSize = 14;
                wsEspacios.Range("A1:E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                wsEspacios.Cell(2, 1).Value = "ID";
                wsEspacios.Cell(2, 2).Value = "Número";
                wsEspacios.Cell(2, 3).Value = "Nivel";
                wsEspacios.Cell(2, 4).Value = "Tipo";
                wsEspacios.Cell(2, 5).Value = "Estado";

                row = 3;
                foreach (var e in espacios)
                {
                    wsEspacios.Cell(row, 1).Value = e.Id_Espacio;
                    wsEspacios.Cell(row, 2).Value = e.No_Espacio;
                    wsEspacios.Cell(row, 3).Value = e.Nivel;
                    wsEspacios.Cell(row, 4).Value = e.TipoEspacio;
                    wsEspacios.Cell(row, 5).Value = e.Estado;
                    row++;
                }

                var tblEspacios = wsEspacios.Range(2, 1, row - 1, 5).CreateTable();
                tblEspacios.Theme = XLTableTheme.TableStyleMedium2;
                wsEspacios.Columns().AdjustToContents();

                // ----------------- Hoja Dashboard -----------------
                var wsDash = workbook.Worksheets.Add("Dashboard");
                wsDash.Cell("A1").Value = "Dashboard de Reportes";
                wsDash.Range("A1:C1").Merge().Style
                    .Font.SetBold().Font.FontSize = 16;
                wsDash.Range("A1:C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                int imgRow = 3;

                void InsertarGrafico(string base64, string titulo)
                {
                    if (!string.IsNullOrEmpty(base64))
                    {
                        var imgBytes = Convert.FromBase64String(base64.Split(',')[1]);
                        using (var ms = new MemoryStream(imgBytes))
                        {
                            var pic = wsDash.AddPicture(ms).MoveTo(wsDash.Cell(imgRow, 1));
                            wsDash.Cell(imgRow - 1, 1).Value = titulo;
                            wsDash.Cell(imgRow - 1, 1).Style.Font.SetBold();
                            imgRow += 20;
                        }
                    }
                }

                InsertarGrafico(graficoVehiculos, "Vehículos por Tipo");
                InsertarGrafico(graficoOcupacion, "Ocupación de Parqueos");
                InsertarGrafico(graficoClientes, "Clientes Diarios");

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
        // Exportar PDF
        // ==============================
        [HttpPost]
        public IActionResult ExportarPdf(string graficoVehiculos, string graficoOcupacion, string graficoClientes)
        {
            var vehiculos = _context.Vehiculos.ToList();
            var espacios = _context.EspacioEstacionamientos.ToList();

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 40, 40, 50, 50);
                var writer = PdfWriter.GetInstance(document, stream);

                // Pie de página
                writer.PageEvent = new PdfFooter();

                document.Open();

                // Portada
                document.Add(new Paragraph("AutoManager - Reporte General", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
                document.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.HELVETICA, 12)));
                document.Add(new Paragraph(" "));
                document.Add(new LineSeparator());

                // Sección Vehículos
                document.Add(new Paragraph("\nVehículos", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                PdfPTable tablaVeh = new PdfPTable(4) { WidthPercentage = 100 };
                tablaVeh.SetWidths(new float[] { 20, 30, 20, 30 });
                string[] headersVeh = { "Placa", "Marca", "Color", "Tipo" };

                foreach (var h in headersVeh)
                {
                    var celda = new PdfPCell(new Phrase(h, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE)));
                    celda.BackgroundColor = new BaseColor(0, 102, 204);
                    celda.HorizontalAlignment = Element.ALIGN_CENTER;
                    tablaVeh.AddCell(celda);
                }

                foreach (var v in vehiculos)
                {
                    tablaVeh.AddCell(v.NoPlaca);
                    tablaVeh.AddCell(v.Marca);
                    tablaVeh.AddCell(v.Color);
                    tablaVeh.AddCell(v.TipoVehiculo?.Descripcion ?? "N/A");
                }
                document.Add(tablaVeh);

                // Sección Espacios
                document.Add(new Paragraph("\nEspacios de Parqueo", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                PdfPTable tablaEsp = new PdfPTable(5) { WidthPercentage = 100 };
                tablaEsp.SetWidths(new float[] { 10, 20, 20, 25, 25 });
                string[] headersEsp = { "ID", "Número", "Nivel", "Tipo", "Estado" };

                foreach (var h in headersEsp)
                {
                    var celda = new PdfPCell(new Phrase(h, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE)));
                    celda.BackgroundColor = new BaseColor(0, 153, 76);
                    celda.HorizontalAlignment = Element.ALIGN_CENTER;
                    tablaEsp.AddCell(celda);
                }

                foreach (var e in espacios)
                {
                    tablaEsp.AddCell(e.Id_Espacio.ToString());
                    tablaEsp.AddCell(e.No_Espacio.ToString());
                    tablaEsp.AddCell(e.Nivel.ToString());
                    tablaEsp.AddCell(e.TipoEspacio);
                    tablaEsp.AddCell(e.Estado);
                }
                document.Add(tablaEsp);

                // Sección Gráficas
                void InsertarGrafico(string base64, string titulo)
                {
                    if (!string.IsNullOrEmpty(base64))
                    {
                        document.NewPage();
                        document.Add(new Paragraph(titulo, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                        var imgBytes = Convert.FromBase64String(base64.Split(',')[1]);
                        var chartImage = iTextSharp.text.Image.GetInstance(imgBytes);
                        chartImage.ScaleToFit(450f, 300f);
                        chartImage.Alignment = Element.ALIGN_CENTER;
                        document.Add(chartImage);
                    }
                }

                InsertarGrafico(graficoVehiculos, "Gráfico: Vehículos por Tipo");
                InsertarGrafico(graficoOcupacion, "Gráfico: Ocupación de Parqueos");
                InsertarGrafico(graficoClientes, "Gráfico: Clientes Diarios");

                document.Close();

                return File(stream.ToArray(), "application/pdf", "ReporteCompleto.pdf");
            }
        }
    }

    // Clase para pie de página
    public class PdfFooter : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            var font = new Font(Font.FontFamily.HELVETICA, 10, Font.ITALIC);
            var footer = new Phrase("Página " + writer.PageNumber, font);

            var cb = writer.DirectContent;
            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, footer,
                (document.Right + document.Left) / 2, document.Bottom - 10, 0);
        }
    }
}
