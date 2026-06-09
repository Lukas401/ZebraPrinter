using LabelManager.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LabelManager.Infra.Zpl;

    public class PrintService : IPrintService
{
    private readonly string _host;
    private readonly int _port;

    // Padrão: Virtual-ZPL-Printer escuta em localhost:9100
    public PrintService(string host = "127.0.0.1", int port = 9100)
    {
        _host = host;
        _port = port;
    }

    public async Task EnviarZplAsync(string zpl)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(_host, _port);

        await using var stream = client.GetStream();
        var bytes = Encoding.UTF8.GetBytes(zpl);
        await stream.WriteAsync(bytes);
        await stream.FlushAsync();
    }
}

