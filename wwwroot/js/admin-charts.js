const chartInstances = {};

function destroyChart(canvasId) {
    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
        delete chartInstances[canvasId];
    }
}

function renderLineChart(canvasId, labels, data, etiqueta) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'line',
        data: {
            labels,
            datasets: [{
                label: etiqueta ?? '',
                data,
                borderColor: '#EF6C57',
                backgroundColor: 'rgba(239, 108, 87, .12)',
                fill: true,
                tension: 0.3,
                pointRadius: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });
}

function renderBarChart(canvasId, labels, data, etiqueta) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                label: etiqueta ?? '',
                data,
                backgroundColor: '#EF6C57',
                borderRadius: 6,
                maxBarThickness: 36
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });
}

function renderDonutChart(canvasId, labels, data, colores) {
    destroyChart(canvasId);
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels,
            datasets: [{
                data,
                backgroundColor: colores,
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { position: 'bottom' } }
        }
    });
}

window.adminCharts = { renderLineChart, renderBarChart, renderDonutChart, destroyChart };
