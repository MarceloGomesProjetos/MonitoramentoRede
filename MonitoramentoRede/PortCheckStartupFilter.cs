using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;

public class PortCheckStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            CheckPort(5110);
            CheckPort(7111);
            next(app);
        };
    }

    private void CheckPort(int port)
    {
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
            Console.WriteLine($"Porta {port} está disponível");
        }
        catch (SocketException)
        {
            Console.WriteLine($"ATENÇÃO: Porta {port} está em uso!");
            Console.WriteLine("Execute este comando para liberar a porta:");
            Console.WriteLine($"    Stop-Process -Id (Get-NetTCPConnection -LocalPort {port}).OwningProcess) -Force");
            throw;
        }
    }    
}