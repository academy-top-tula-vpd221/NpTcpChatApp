using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NpTcpChatListenerApp
{
    internal class ClientObject
    {
        internal string Id { get; } = Guid.NewGuid().ToString();

        internal StreamWriter Writer { get; }
        internal StreamReader Reader { get; }

        TcpClient client;
        ListenerObject listener;

        public ClientObject(TcpClient client, ListenerObject listener)
        {
            this.client = client; 
            this.listener = listener;

            var stream = client.GetStream();

            Writer = new StreamWriter(stream);
            Reader = new StreamReader(stream);
        }

        internal async Task ProcessAsync()
        {
            try
            {
                string? clientName = await Reader.ReadLineAsync();
                string? message = $"{clientName} commin to chat";
                await listener.SendMessageAsync(message, this.Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message is null) continue;
                        message = $"{clientName}: {message}";
                        Console.WriteLine(message);
                        await listener.SendMessageAsync(message, this.Id);
                    }
                    catch
                    {
                        message = $"{clientName} out from chat";
                        Console.WriteLine(message);
                        await listener.SendMessageAsync(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                listener.RemoveClient(this.Id);
            }
        }

        internal void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
