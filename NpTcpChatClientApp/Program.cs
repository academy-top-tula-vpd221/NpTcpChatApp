using System.Net;
using System.Net.Sockets;

string host = IPAddress.Loopback.ToString();
int port = 5000;

using TcpClient client = new();
Console.Write("Input name: ");
string? name = Console.ReadLine();
Console.WriteLine($"Welcome to chat, {name}");

StreamReader? reader = null;
StreamWriter? writer = null;

try
{
    client.Connect(host, port);
    reader = new StreamReader(client.GetStream());
    writer = new StreamWriter(client.GetStream());

    if (reader is null || writer is null) return;

    Task.Run(() => ReceiveMessageAsync(reader));
    await SendMessageAsync(writer);
}
catch(Exception e)
{
    Console.WriteLine($"{e.Message}");
}
finally
{
    writer?.Close();
    reader?.Close();
}


async Task SendMessageAsync(StreamWriter writer)
{
    await writer.WriteLineAsync(name);
    await writer.FlushAsync();
    Console.WriteLine("Input messages");

    while(true)
    {
        string? message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }
}

async Task ReceiveMessageAsync(StreamReader reader)
{
    while(true)
    {
        try
        {
            string? message = await reader.ReadLineAsync();
            if (String.IsNullOrEmpty(message)) continue;

            PrintMessage(message);
        }
        catch
        {
            break;
        }
    }
}

void PrintMessage(string message)
{
    var position = Console.GetCursorPosition();
    var left = position.Left;
    var top = position.Top;

    Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
    Console.SetCursorPosition(0, top);
    Console.WriteLine(message);
    Console.SetCursorPosition(0, top + 1);
}