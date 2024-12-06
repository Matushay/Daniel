document.addEventListener("DOMContentLoaded", function () {
    fetch('/api/dashboard/usuariosPorRol') // Cambia la URL si es necesario
        .then(response => {
            if (!response.ok) {
                throw new Error('Error al cargar los datos: ' + response.status);
            }
            return response.json();
        })
        .then(data => {
            // Procesa los datos
            const roles = data.map(item => item.rol); // Extrae los nombres de los roles
            const cantidades = data.map(item => item.cantidad); // Extrae las cantidades

            // Renderiza el gráfico
            renderChart(roles, cantidades);
        })
        .catch(error => console.error('Error:', error));
});

function renderChart(labels, data) {
    const ctx = document.getElementById('usuariosPorRolChart').getContext('2d');
    new Chart(ctx, {
        type: 'bar', // Puedes usar 'bar', 'pie', 'line', etc.
        data: {
            labels: labels, // Etiquetas (nombres de los roles)
            datasets: [{
                label: 'Usuarios por Rol',
                data: data, // Cantidades de usuarios
                backgroundColor: [
                    'rgba(75, 192, 192, 0.2)', // Cambia los colores según tu diseño
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)',
                    'rgba(255, 99, 132, 0.2)'
                ],
                borderColor: [
                    'rgba(75, 192, 192, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(255, 99, 132, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true // Asegura que el eje Y comience en 0
                }
            }
        }
    });
}
