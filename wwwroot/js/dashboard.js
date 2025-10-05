@section Scripts {
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <script>
        // ==============================
        // Gráfico de Vehículos por Tipo
        // ==============================
        var ctxVehiculos = document.getElementById("vehiculosChart").getContext("2d");
        var vehiculosChart = new Chart(ctxVehiculos, {
            type: "bar",
            data: {
                labels: @Html.Raw(Json.Serialize(ViewData["VehiculosPorTipoLabels"])),
                datasets: [{
                    label: "Cantidad de Vehículos",
                    data: @Html.Raw(Json.Serialize(ViewData["VehiculosPorTipoData"])),
                    backgroundColor: ["#007bff", "#28a745", "#ffc107", "#dc3545", "#6f42c1"],
                    borderColor: "#000",
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false },
                    title: { display: true, text: "Vehículos por Tipo" }
                },
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });

        // ==============================
        // Gráfico de Ocupación del Parqueo
        // ==============================
        var ctxOcupacion = document.getElementById("ocupacionChart").getContext("2d");
        var ocupacionChart = new Chart(ctxOcupacion, {
            type: "doughnut",
            data: {
                labels: ["Ocupados", "Disponibles"],
                datasets: [{
                    data: [@ViewData["Ocupados"], @ViewData["Disponibles"]],
                    backgroundColor: ["#dc3545", "#28a745"],
                    borderColor: "#000",
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { position: "bottom" },
                    title: { display: true, text: "Ocupación del Parqueo" }
                }
            }
        });

        // ==============================
        // Gráfico de Clientes Diarios
        // ==============================
        var ctxClientes = document.getElementById("clientesChart").getContext("2d");
        var clientesChart = new Chart(ctxClientes, {
            type: "bar",
            data: {
                labels: @Html.Raw(Json.Serialize(ViewData["DiasClientes"])),
                datasets: [{
                    label: "Clientes",
                    data: @Html.Raw(Json.Serialize(ViewData["ClientesDiarios"])),
                    backgroundColor: "#28a745",
                    borderColor: "#000",
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { display: false },
                    title: { display: true, text: "Clientes Diarios" }
                },
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });

        // ==============================
        // Exportar PDF con gráficos
        // ==============================
        document.getElementById("exportPdfWithCharts").addEventListener("click", function() {
            var vehiculosImg = vehiculosChart.toBase64Image();
            var ocupacionImg = ocupacionChart.toBase64Image();
            var clientesImg = clientesChart.toBase64Image();

            fetch('/Reportes/ExportarPdfConGraficos', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    vehiculos: vehiculosImg,
                    ocupacion: ocupacionImg,
                    clientes: clientesImg
                })
            })
            .then(res => res.blob())
            .then(blob => {
                var url = window.URL.createObjectURL(blob);
                var a = document.createElement('a');
                a.href = url;
                a.download = "ReporteCompletoConGraficos.pdf";
                a.click();
            });
        });
    </script>
}
