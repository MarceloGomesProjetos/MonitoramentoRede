using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MonitoramentoRede.Hubs
{
    public class NetworkMonitoringService : IHostedService, IDisposable
    {
        private readonly IHubContext<NetworkMonitorHub> _hubContext;
        private Timer? _timer;

        public NetworkMonitoringService(IHubContext<NetworkMonitorHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(MonitorNetwork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void MonitorNetwork(object? state)
        {
            // Simular dados de monitoramento
            var random = new Random();
            var devices = new[] { "Router", "Switch", "Server", "Firewall" };
            
            foreach (var device in devices)
            {
                var bandwidthUsage = random.NextDouble() * 100;
                var status = bandwidthUsage > 80 ? "Critical" : bandwidthUsage > 60 ? "Warning" : "Normal";
                var responseTime = random.NextDouble() * 100;
                var isOnline = random.NextDouble() > 0.1; // 90% de chance de estar online
                
                _hubContext.Clients.All.SendAsync("ReceiveNetworkStatus", device, status, bandwidthUsage);
                _hubContext.Clients.All.SendAsync("ReceivePingResult", device, responseTime, isOnline);
                
                // Simular upload/download
                var upload = random.NextDouble() * 100;
                var download = random.NextDouble() * 100;
                _hubContext.Clients.All.SendAsync("ReceiveBandwidthData", device, upload, download);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}