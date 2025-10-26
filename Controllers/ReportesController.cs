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

        [HttpPost]
        public IActionResult ExportarExcel(string graficoVehiculos, string graficoOcupacion, string graficoClientes)
        {
            var vehiculos = _context.Vehiculos.ToList();
            var espacios = _context.EspacioEstacionamientos.ToList();

            using (var workbook = new XLWorkbook())
            {
                // Hoja Vehículos
                var wsVeh = workbook.Worksheets.Add("Vehículos");
                wsVeh.Cell("A1").Value = "Reporte de Vehículos";
                wsVeh.Range("A1:D1").Merge().Style.Font.SetBold().Font.FontSize = 14;
                wsVeh.Range("A1:D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                wsVeh.Cell(2, 1).Value = "Placa"; wsVeh.Cell(2, 2).Value = "Marca";
                wsVeh.Cell(2, 3).Value = "Color"; wsVeh.Cell(2, 4).Value = "Tipo";

                int row = 3;
                foreach (var v in vehiculos)
                {
                    wsVeh.Cell(row, 1).Value = v.NoPlaca;
                    wsVeh.Cell(row, 2).Value = v.Marca;
                    wsVeh.Cell(row, 3).Value = v.Color;
                    wsVeh.Cell(row, 4).Value = v.TipoVehiculo?.Descripcion;
                    row++;
                }
                wsVeh.Range(2, 1, row - 1, 4).CreateTable().Theme = XLTableTheme.TableStyleMedium9;
                wsVeh.Columns().AdjustToContents();

                // Hoja Espacios
                var wsEsp = workbook.Worksheets.Add("Espacios");
                wsEsp.Cell("A1").Value = "Reporte de Espacios";
                wsEsp.Range("A1:E1").Merge().Style.Font.SetBold().Font.FontSize = 14;
                wsEsp.Range("A1:E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                wsEsp.Cell(2, 1).Value = "ID"; wsEsp.Cell(2, 2).Value = "Número";
                wsEsp.Cell(2, 3).Value = "Nivel"; wsEsp.Cell(2, 4).Value = "Tipo"; wsEsp.Cell(2, 5).Value = "Estado";

                row = 3;
                foreach (var e in espacios)
                {
                    wsEsp.Cell(row, 1).Value = e.Id_Espacio;
                    wsEsp.Cell(row, 2).Value = e.No_Espacio;
                    wsEsp.Cell(row, 3).Value = e.Nivel;
                    wsEsp.Cell(row, 4).Value = e.TipoEspacio;
                    wsEsp.Cell(row, 5).Value = e.Estado;
                    row++;
                }
                wsEsp.Range(2, 1, row - 1, 5).CreateTable().Theme = XLTableTheme.TableStyleMedium2;
                wsEsp.Columns().AdjustToContents();

                // Hoja Dashboard con gráficos
                var wsDash = workbook.Worksheets.Add("Dashboard");
                wsDash.Cell("A1").Value = "Dashboard SIPARK";
                wsDash.Range("A1:C1").Merge().Style.Font.SetBold().Font.FontSize = 16;
                wsDash.Range("A1:C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                int imgRow = 3;
                void InsertarGrafico(string base64, string titulo)
                {
                    if (!string.IsNullOrEmpty(base64))
                    {
                        var bytes = Convert.FromBase64String(base64.Split(',')[1]);
                        using (var ms = new MemoryStream(bytes))
                        {
                            wsDash.AddPicture(ms).MoveTo(wsDash.Cell(imgRow, 1));
                            wsDash.Cell(imgRow - 1, 1).Value = titulo;
                            wsDash.Cell(imgRow - 1, 1).Style.Font.SetBold();
                            imgRow += 20;
                        }
                    }
                }

                InsertarGrafico(graficoVehiculos, "Vehículos por Tipo");
                InsertarGrafico(graficoOcupacion, "Dinero Ganado");
                InsertarGrafico(graficoClientes, "Clientes Últimos 7 Días");

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ReporteCompleto.xlsx");
                }
            }
        }

        [HttpPost]
        public IActionResult ExportarPdf(string graficoVehiculos, string graficoOcupacion, string graficoClientes)
        {
            var vehiculos = _context.Vehiculos.ToList();
            var espacios = _context.EspacioEstacionamientos.ToList();

            using (var stream = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 40, 40, 50, 50);
                var writer = PdfWriter.GetInstance(doc, stream);
                writer.PageEvent = new PdfFooter();
                doc.Open();

                // Portada
                doc.Add(new Paragraph("SIPARK - Reporte General", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
                doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy")));
                doc.Add(new Paragraph(" "));
                doc.Add(new LineSeparator());

                // Vehículos
                doc.Add(new Paragraph("\nVehículos", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                PdfPTable tablaVeh = new PdfPTable(4) { WidthPercentage = 100 };
                tablaVeh.SetWidths(new float[] { 20, 30, 20, 30 });
                string[] headersVeh = { "Placa", "Marca", "Color", "Tipo" };
                foreach (var h in headersVeh)
                {
                    var c = new PdfPCell(new Phrase(h, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE)));
                    c.BackgroundColor = new BaseColor(0, 102, 204); c.HorizontalAlignment = Element.ALIGN_CENTER;
                    tablaVeh.AddCell(c);
                }
                foreach (var v in vehiculos)
                {
                    tablaVeh.AddCell(v.NoPlaca);
                    tablaVeh.AddCell(v.Marca);
                    tablaVeh.AddCell(v.Color);
                    tablaVeh.AddCell(v.TipoVehiculo?.Descripcion ?? "N/A");
                }
                doc.Add(tablaVeh);

                // Espacios
                doc.Add(new Paragraph("\nEspacios de Parqueo", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                PdfPTable tablaEsp = new PdfPTable(5) { WidthPercentage = 100 };
                tablaEsp.SetWidths(new float[] { 10, 20, 20, 25, 25 });
                string[] headersEsp = { "ID", "Número", "Nivel", "Tipo", "Estado" };
                foreach (var h in headersEsp)
                {
                    var c = new PdfPCell(new Phrase(h, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE)));
                    c.BackgroundColor = new BaseColor(0, 153, 76); c.HorizontalAlignment = Element.ALIGN_CENTER;
                    tablaEsp.AddCell(c);
                }
                foreach (var e in espacios)
                {
                    tablaEsp.AddCell(e.Id_Espacio.ToString());
                    tablaEsp.AddCell(e.No_Espacio.ToString());
                    tablaEsp.AddCell(e.Nivel.ToString());
                    tablaEsp.AddCell(e.TipoEspacio);
                    tablaEsp.AddCell(e.Estado);
                }
                doc.Add(tablaEsp);

                // Gráficos
                void InsertarGrafico(string base64, string titulo)
                {
                    if (!string.IsNullOrEmpty(base64))
                    {
                        doc.NewPage();
                        doc.Add(new Paragraph(titulo, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)));
                        var bytes = Convert.FromBase64String(base64.Split(',')[1]);
                        var img = iTextSharp.text.Image.GetInstance(bytes);
                        img.ScaleToFit(450f, 300f);
                        img.Alignment = Element.ALIGN_CENTER;
                        doc.Add(img);
                    }
                }

                InsertarGrafico(graficoVehiculos, "Gráfico: Vehículos por Tipo");
                InsertarGrafico(graficoOcupacion, "Gráfico: Dinero Ganado");
                InsertarGrafico(graficoClientes, "Gráfico: Clientes Últimos 7 Días");

                doc.Close();
                return File(stream.ToArray(), "application/pdf", "ReporteCompleto.pdf");
            }
        }
    }

    public class PdfFooter : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            var font = new Font(Font.FontFamily.HELVETICA, 10, Font.ITALIC);
            var footer = new Phrase("Página " + writer.PageNumber, font);
            ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER, footer, (document.Right + document.Left) / 2, document.Bottom - 10, 0);
        }
    }
}
