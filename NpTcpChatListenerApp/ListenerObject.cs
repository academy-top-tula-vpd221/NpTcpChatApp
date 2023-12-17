using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;

namespace NpTcpChatListenerApp
{
    class ListenerObject
    {
        TcpListener listener;
        List<ClientObject> clients;

        public ListenerObject()
        {
            listener = new TcpListener(IPAddress.Loopback, 5000);
            clients = new List<ClientObject>();
        }

        internal void RemoveClient(string id)
        {
            ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
            if (client is not null)
                clients.Remove(client);
            client.Close();
        }

        internal async Task ListenAsync()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Server chat start. Waitng connections...");

                TcpClient clientTcp = await listener.AcceptTcpClientAsync();
                ClientObject client = new ClientObject(clientTcp, this);

                clients.Add(client);
                await Task.Run(client.ProcessAsync);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        internal async Task SendMessageAsync(string id, string message)
        {
            foreach(var client in clients)
            {
                if(client.Id != id)
                {
                    await client.Writer.WriteLineAsync(message);
                    await client.Writer.FlushAsync();
                }
            }
        }

        internal void Disconnect()
        {
            foreach (var client in clients)
                client.Close();

            listener.Stop();
        }
    }
}
