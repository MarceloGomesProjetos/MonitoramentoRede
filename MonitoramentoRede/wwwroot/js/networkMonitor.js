document.addEventListener('DOMContentLoaded', function() {
    // Configuração do SignalR
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/networkMonitorHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    
    // Iniciar conexão
    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    };
    
    connection.onclose(async () => {
        await start();
    });
    
    start();
    
    // Configuração dos gráficos
    const bandwidthCtx = document.getElementById('bandwidthChart').getContext('2d');
    const responseTimeCtx = document.getElementById('responseTimeChart').getContext('2d');
    
    const bandwidthChart = new Chart(bandwidthCtx, {
        type: 'line',
        data: {
            labels: [],
            datasets: [
                { label: 'Upload (Mbps)', data: [], borderColor: 'rgba(255, 99, 132, 1)', backgroundColor: 'rgba(255, 99, 132, 0.2)' },
                { label: 'Download (Mbps)', data: [], borderColor: 'rgba(54, 162, 235, 1)', backgroundColor: 'rgba(54, 162, 235, 0.2)' }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
    
    const responseTimeChart = new Chart(responseTimeCtx, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [
                { label: 'Response Time (ms)', data: [], backgroundColor: 'rgba(75, 192, 192, 0.6)' }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
    
    // Dados dos dispositivos
    const devices = {};
    
    // Atualizar status da rede
    connection.on("ReceiveNetworkStatus", (deviceId, status, bandwidthUsage) => {
        if (!devices[deviceId]) {
            devices[deviceId] = { id: deviceId };
            updateStatusGrid();
        }
        
        devices[deviceId].status = status;
        devices[deviceId].bandwidthUsage = bandwidthUsage.toFixed(2);
        
        updateStatusGrid();
    });
    
    // Atualizar resultados de ping
    connection.on("ReceivePingResult", (deviceId, responseTime, isOnline) => {
        if (!devices[deviceId]) {
            devices[deviceId] = { id: deviceId };
            updateStatusGrid();
        }
        
        devices[deviceId].responseTime = responseTime.toFixed(2);
        devices[deviceId].isOnline = isOnline;
        
        updateStatusGrid();
        updateCharts(deviceId);
    });
    
    // Atualizar dados de banda
    connection.on("ReceiveBandwidthData", (deviceId, upload, download) => {
        if (!devices[deviceId]) {
            devices[deviceId] = { id: deviceId };
            updateStatusGrid();
        }
        
        devices[deviceId].upload = upload.toFixed(2);
        devices[deviceId].download = download.toFixed(2);
        
        updateCharts(deviceId);
    });
    
    // Atualizar grid de status
    function updateStatusGrid() {
        const grid = document.getElementById('networkStatusGrid');
        grid.innerHTML = '';
        
        const table = document.createElement('table');
        table.className = 'table table-striped';
        
        const thead = document.createElement('thead');
        thead.innerHTML = `
            <tr>
                <th>Dispositivo</th>
                <th>Status</th>
                <th>Largura de Banda</th>
                <th>Tempo de Resposta</th>
                <th>Online</th>
            </tr>
        `;
        table.appendChild(thead);
        
        const tbody = document.createElement('tbody');
        for (const deviceId in devices) {
            const device = devices[deviceId];
            
            const row = document.createElement('tr');
            
            // Determinar classe CSS baseada no status
            let statusClass = '';
            if (device.status === 'Critical') statusClass = 'table-danger';
            else if (device.status === 'Warning') statusClass = 'table-warning';
            else if (device.status === 'Normal') statusClass = 'table-success';
            
            row.innerHTML = `
                <td>${device.id}</td>
                <td class="${statusClass}">${device.status || 'N/A'}</td>
                <td>${device.bandwidthUsage || 'N/A'}%</td>
                <td>${device.responseTime || 'N/A'} ms</td>
                <td>${device.isOnline ? '✅' : '❌'}</td>
            `;
            
            tbody.appendChild(row);
        }
        
        table.appendChild(tbody);
        grid.appendChild(table);
    }
    
    // Atualizar gráficos
    function updateCharts(deviceId) {
        const device = devices[deviceId];
        
        // Atualizar gráfico de banda
        if (bandwidthChart.data.labels.length > 10) {
            bandwidthChart.data.labels.shift();
            bandwidthChart.data.datasets[0].data.shift();
            bandwidthChart.data.datasets[1].data.shift();
        }
        
        bandwidthChart.data.labels.push(new Date().toLocaleTimeString());
        bandwidthChart.data.datasets[0].data.push(device.upload);
        bandwidthChart.data.datasets[1].data.push(device.download);
        bandwidthChart.update();
        
        // Atualizar gráfico de tempo de resposta
        if (responseTimeChart.data.labels.length > 10) {
            responseTimeChart.data.labels.shift();
            responseTimeChart.data.datasets[0].data.shift();
        }
        
        responseTimeChart.data.labels.push(deviceId);
        responseTimeChart.data.datasets[0].data.push(device.responseTime);
        responseTimeChart.update();
    }
});