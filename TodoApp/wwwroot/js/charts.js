// Builds every <canvas data-chart='{...}'> on the page with Chart.js, styled for the dark theme.
(function () {
    if (typeof Chart === 'undefined') return;

    Chart.defaults.color = '#8f9bc7';
    Chart.defaults.borderColor = 'rgba(255,255,255,.07)';
    Chart.defaults.font.family = '"Segoe UI", "Trebuchet MS", system-ui, sans-serif';
    Chart.defaults.plugins.legend.labels.boxWidth = 10;
    Chart.defaults.plugins.legend.labels.usePointStyle = true;
    Chart.defaults.plugins.tooltip.backgroundColor = 'rgba(10,14,31,.95)';
    Chart.defaults.plugins.tooltip.borderColor = 'rgba(124,92,255,.6)';
    Chart.defaults.plugins.tooltip.borderWidth = 1;
    Chart.defaults.plugins.tooltip.padding = 10;

    const base = { responsive: true, maintainAspectRatio: false, animation: { duration: 800 } };
    const legendBottom = { position: 'bottom', labels: { padding: 14 } };

    const builders = {
        // stacked daily bars
        stackedBar: c => ({
            type: 'bar',
            data: {
                labels: c.labels,
                datasets: c.datasets.map(d => ({ label: d.label, data: d.data, backgroundColor: d.color, borderRadius: 4, maxBarThickness: 20 })),
            },
            options: {
                ...base,
                interaction: { mode: 'index', intersect: false },
                scales: { x: { stacked: true, grid: { display: false }, ticks: { maxRotation: 0, autoSkip: true, maxTicksLimit: 7 } },
                          y: { stacked: true, beginAtZero: true, ticks: { precision: 0 } } },
                plugins: { legend: legendBottom },
            },
        }),

        // two lines on two axes
        dualLine: c => ({
            type: 'line',
            data: {
                labels: c.labels,
                datasets: c.datasets.map(d => ({
                    label: d.label, data: d.data, yAxisID: d.axis, borderColor: d.color, backgroundColor: d.color + '33',
                    fill: d.axis === 'y', tension: 0.35, pointRadius: 2, pointHoverRadius: 5, borderWidth: 2,
                })),
            },
            options: {
                ...base,
                interaction: { mode: 'index', intersect: false },
                scales: {
                    x: { grid: { display: false }, ticks: { maxRotation: 0, autoSkip: true, maxTicksLimit: 7 } },
                    y: { beginAtZero: true, position: 'left', ticks: { precision: 0 } },
                    y1: { beginAtZero: true, position: 'right', grid: { drawOnChartArea: false }, ticks: { precision: 0 } },
                },
                plugins: { legend: legendBottom },
            },
        }),

        doughnut: c => ({
            type: 'doughnut',
            data: { labels: c.labels, datasets: [{ data: c.data, backgroundColor: c.colors, borderColor: '#101730', borderWidth: 3 }] },
            options: { ...base, cutout: '68%', plugins: { legend: legendBottom } },
        }),

        // character stats, scaled 0-100 for the shape; tooltips show the real numbers
        radar: c => ({
            type: 'radar',
            data: {
                labels: c.labels,
                datasets: [{
                    label: c.title, data: c.data, raw: c.raw, borderColor: '#00d4ff', backgroundColor: 'rgba(0,212,255,.2)',
                    pointBackgroundColor: '#00d4ff', borderWidth: 2,
                }],
            },
            options: {
                ...base,
                scales: { r: { min: 0, max: 100, ticks: { display: false }, grid: { color: 'rgba(255,255,255,.1)' },
                              angleLines: { color: 'rgba(255,255,255,.1)' }, pointLabels: { color: '#e9edff', font: { size: 12, weight: '600' } } } },
                plugins: { legend: { display: false }, tooltip: { callbacks: { label: ctx => ' ' + ctx.dataset.raw[ctx.dataIndex] } } },
            },
        }),
    };

    document.querySelectorAll('canvas[data-chart]').forEach(canvas => {
        try {
            const cfg = JSON.parse(canvas.dataset.chart);
            const build = builders[cfg.kind];
            if (build) new Chart(canvas, build(cfg));
        } catch (e) { console.error('Chart failed', e); }
    });
})();
