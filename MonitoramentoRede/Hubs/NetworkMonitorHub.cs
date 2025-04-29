using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MonitoramentoRede.Hubs
{
    public class NetworkMonitorHub : Hub
    {
        public async Task SendNetworkStatus(string deviceId, string status, double bandwidthUsage)
        {
            await Clients.All.SendAsync("ReceiveNetworkStatus", deviceId, status, bandwidthUsage);
        }
        
        public async Task SendPingResult(string deviceId, double responseTime, bool isOnline)
        {
            await Clients.All.SendAsync("ReceivePingResult", deviceId, responseTime, isOnline);
        }
        
        public async Task SendBandwidthData(string deviceId, double upload, double download)
        {
            await Clients.All.SendAsync("ReceiveBandwidthData", deviceId, upload, download);
        }
    }
}