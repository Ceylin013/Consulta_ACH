let graficoTransacciones = null;

window.actualizarGraficoTransacciones = (exitosas, fallidas) => {

    const elemento = document.getElementById("graficoTransacciones");

    if (!elemento) return;

    const datos = {
        labels: ["Exitosas", "Fallidas"],
        datasets: [{
            data: [exitosas, fallidas],
            backgroundColor: ["#198754", "#dc3545"],
            borderWidth: 1
        }]
    };

    if (graficoTransacciones) {
        graficoTransacciones.data = datos;
        graficoTransacciones.update();
    } else {
        graficoTransacciones = new Chart(elemento, {
            type: "pie",
            data: datos,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: "bottom"
                    }
                }
            }
        });
    }
};