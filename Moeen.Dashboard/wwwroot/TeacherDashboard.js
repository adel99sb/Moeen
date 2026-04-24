
window.initTeacherCharts = () => {
    const Config = {
        fontFamily: "'Segoe UI', sans-serif",
        colors: {
            primary: '#195042',
            accent1: '#2e7d6a',
            accent2: '#5a9f8e',
            accent3: '#8fc9b9'
        },
        weeks: ['أسبوع 1', 'أسبوع 2', 'أسبوع 3', 'أسبوع 4']
    };

    const metrics = [
        { k: 'tests', l: 'اختبارات', c: Config.colors.primary },
        { k: 'recite', l: 'تسميعات', c: Config.colors.accent1 },
        { k: 'review', l: 'مراجعات', c: Config.colors.accent2 },
        { k: 'attend', l: 'حضور', c: Config.colors.accent3 }
    ];

    function createTeacherChart(elementId, dataset) {
        const ctx = document.getElementById(elementId);
        if (!ctx) return;
        if (ctx._chartInstance) { try { ctx._chartInstance.destroy(); } catch { } }

        const chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: Config.weeks,
                datasets: metrics.map(m => ({
                    label: m.l,
                    data: dataset.map(d => d[m.k]),
                    backgroundColor: m.c,
                    borderRadius: 6
                }))
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: { beginAtZero: true, grid: { color: '#f0f0f0' } },
                    x: { grid: { display: false } }
                },
                plugins: {
                    legend: { labels: { font: { family: Config.fontFamily } } }
                }
            }
        });

        ctx._chartInstance = chart;
        return chart;
    }

    const h1Data = [{ tests: 8, recite: 15, review: 12, attend: 28 }, { tests: 10, recite: 18, review: 14, attend: 30 }, { tests: 12, recite: 20, review: 16, attend: 29 }, { tests: 9, recite: 17, review: 15, attend: 30 }];
    const h2Data = [{ tests: 6, recite: 12, review: 10, attend: 25 }, { tests: 7, recite: 14, review: 11, attend: 27 }, { tests: 8, recite: 16, review: 13, attend: 26 }, { tests: 7, recite: 15, review: 12, attend: 28 }];

    createTeacherChart('chartH1', h1Data);
    createTeacherChart('chartH2', h2Data);
};