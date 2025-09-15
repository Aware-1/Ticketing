// wwwroot/js/charts-helper.js

// Initialize Chart.js charts for reports
window.initializeReportCharts = (data) => {
    try {
        // Ticket Trend Chart
        const trendCtx = document.getElementById('ticketTrendChart');
        if (trendCtx) {
            new Chart(trendCtx, {
                type: 'line',
                data: data.ticketTrend,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    },
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    }
                }
            });
        }

        // Priority Distribution Chart
        const priorityCtx = document.getElementById('priorityChart');
        if (priorityCtx) {
            new Chart(priorityCtx, {
                type: 'doughnut',
                data: data.priority,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    }
                }
            });
        }

    } catch (error) {
        console.error('Error initializing charts:', error);
    }
};

// Initialize dashboard charts
window.initializeDashboardCharts = (containerId, chartData) => {
    try {
        const container = document.getElementById(containerId);
        if (!container) return;

        // Create a simple bar chart using CSS
        container.innerHTML = '';

        chartData.forEach((item, index) => {
            const barContainer = document.createElement('div');
            barContainer.className = 'chart-bar-container mb-2';

            const label = document.createElement('div');
            label.className = 'd-flex justify-content-between align-items-center mb-1';
            label.innerHTML = `<span>${item.label}</span><span>${item.value}</span>`;

            const progressBar = document.createElement('div');
            progressBar.className = 'progress';
            progressBar.style.height = '8px';

            const progress = document.createElement('div');
            progress.className = `progress-bar bg-${item.color || 'primary'}`;
            progress.style.width = `${item.percentage}%`;

            progressBar.appendChild(progress);
            barContainer.appendChild(label);
            barContainer.appendChild(progressBar);
            container.appendChild(barContainer);
        });

    } catch (error) {
        console.error('Error creating dashboard chart:', error);
    }
};

// Real-time chart updates
window.updateChartData = (chartId, newData) => {
    try {
        const chart = Chart.getChart(chartId);
        if (chart) {
            chart.data = newData;
            chart.update('none'); // No animation for real-time updates
        }
    } catch (error) {
        console.error('Error updating chart:', error);
    }
};

// Performance metrics visualization
window.createPerformanceIndicator = (containerId, percentage, label, color = 'primary') => {
    const container = document.getElementById(containerId);
    if (!container) return;

    const radius = 40;
    const circumference = 2 * Math.PI * radius;
    const offset = circumference - (percentage / 100) * circumference;

    container.innerHTML = `
        <div class="performance-indicator text-center">
            <svg width="100" height="100" class="performance-circle">
                <circle cx="50" cy="50" r="${radius}" 
                        fill="none" stroke="#e9ecef" stroke-width="8"/>
                <circle cx="50" cy="50" r="${radius}" 
                        fill="none" stroke="var(--bs-${color})" stroke-width="8"
                        stroke-dasharray="${circumference}" 
                        stroke-dashoffset="${offset}"
                        stroke-linecap="round"
                        transform="rotate(-90 50 50)"/>
                <text x="50" y="50" text-anchor="middle" dy="5" 
                      font-size="18" font-weight="bold" class="text-${color}">
                    ${Math.round(percentage)}%
                </text>
            </svg>
            <div class="mt-2">
                <small class="text-muted">${label}</small>
            </div>
        </div>
    `;
};

// Animate counters
window.animateCounter = (elementId, finalValue, duration = 1000, prefix = '', suffix = '') => {
    const element = document.getElementById(elementId);
    if (!element) return;

    let startValue = 0;
    const startTime = Date.now();

    const updateCounter = () => {
        const elapsed = Date.now() - startTime;
        const progress = Math.min(elapsed / duration, 1);

        // Easing function
        const easeOut = 1 - Math.pow(1 - progress, 3);
        const currentValue = Math.round(startValue + (finalValue - startValue) * easeOut);

        element.textContent = prefix + currentValue + suffix;

        if (progress < 1) {
            requestAnimationFrame(updateCounter);
        }
    };

    updateCounter();
};

// Export chart as image
window.exportChartAsImage = (chartId, filename = 'chart.png') => {
    try {
        const chart = Chart.getChart(chartId);
        if (chart) {
            const url = chart.toBase64Image();
            const link = document.createElement('a');
            link.download = filename;
            link.href = url;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    } catch (error) {
        console.error('Error exporting chart:', error);
    }
};

// Initialize trend sparklines
window.createSparkline = (containerId, data, color = '#007bff') => {
    const container = document.getElementById(containerId);
    if (!container || !data.length) return;

    const width = container.offsetWidth || 100;
    const height = 30;
    const max = Math.max(...data);
    const min = Math.min(...data);
    const range = max - min || 1;

    let path = '';
    data.forEach((value, index) => {
        const x = (index / (data.length - 1)) * width;
        const y = height - ((value - min) / range) * height;
        path += index === 0 ? `M${x},${y}` : `L${x},${y}`;
    });

    container.innerHTML = `
        <svg width="${width}" height="${height}" class="sparkline">
            <path d="${path}" fill="none" stroke="${color}" stroke-width="2"/>
        </svg>
    `;
};

// Real-time data point animation
window.addDataPoint = (chartId, newPoint) => {
    try {
        const chart = Chart.getChart(chartId);
        if (!chart) return;

        chart.data.labels.push(newPoint.label);
        chart.data.datasets.forEach(dataset => {
            dataset.data.push(newPoint.value);
        });

        // Remove old data points if too many
        if (chart.data.labels.length > 20) {
            chart.data.labels.shift();
            chart.data.datasets.forEach(dataset => {
                dataset.data.shift();
            });
        }

        chart.update('active');
    } catch (error) {
        console.error('Error adding data point:', error);
    }
};

// Status indicator
window.updateStatusIndicator = (elementId, status, message) => {
    const element = document.getElementById(elementId);
    if (!element) return;

    const statusColors = {
        'online': 'success',
        'offline': 'danger',
        'away': 'warning',
        'busy': 'info'
    };

    const color = statusColors[status] || 'secondary';

    element.innerHTML = `
        <div class="d-flex align-items-center">
            <div class="status-dot bg-${color} me-2"></div>
            <span class="text-${color}">${message}</span>
        </div>
    `;
};

// CSS for custom components
const chartStyles = `
<style>
.performance-circle {
    transition: all 0.3s ease;
}

.performance-indicator:hover .performance-circle {
    transform: scale(1.05);
}

.chart-bar-container {
    transition: all 0.2s ease;
}

.chart-bar-container:hover {
    transform: translateX(5px);
}

.status-dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    animation: pulse 2s infinite;
}

@keyframes pulse {
    0% { opacity: 1; }
    50% { opacity: 0.5; }
    100% { opacity: 1; }
}

.sparkline {
    display: block;
    margin: 0 auto;
}
</style>
`;

// Inject styles
if (!document.querySelector('#chart-styles')) {
    const styleSheet = document.createElement('div');
    styleSheet.id = 'chart-styles';
    styleSheet.innerHTML = chartStyles;
    document.head.appendChild(styleSheet);
}