using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

class Program
{

    public static ConcurrentDictionary<string, TcpClient> clientList = new ConcurrentDictionary<string, TcpClient>();

    static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 1234);
        server.Start();
        Console.WriteLine("Server başlatıldı");

        while (true)
        {
            TcpClient client = await server.AcceptTcpClientAsync();
            _ = Task.Run(() => { HandleClientActions(client); });
        }
    }

    public static async Task HandleClientActions(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] NameBuffer = new byte[1024];
        int BufferLength = await stream.ReadAsync(NameBuffer, 0, NameBuffer.Length);
        string ClientName = Encoding.UTF8.GetString(NameBuffer, 0, BufferLength);

        clientList.TryAdd(ClientName, client);

        Console.WriteLine($"{ClientName} server'a bağlandı");

        try
        {
            while (true)
            {
                byte[] MessageBuffer = new byte[1024];
                int MsgBufLength = await stream.ReadAsync(MessageBuffer, 0, MessageBuffer.Length);
                string Message = Encoding.UTF8.GetString(MessageBuffer, 0, MsgBufLength);
                Console.WriteLine($"{ClientName}: {Message} || {DateTime.Now}");
                await BroadcastMessage(client, Message);
            }
        }
        catch
        {
            Console.WriteLine($"{ClientName} server'dan ayrıldı");
            clientList.TryRemove(ClientName, out _);
            stream.Close();
        }


    }

    public static async Task BroadcastMessage(TcpClient client, string message)
    {
        byte[] MessageBuffer = Encoding.UTF8.GetBytes(message);
        #region eskisi

        //foreach (var clients in clientList.Values)
        //{
        //// clients'daki client mesaj gönderen client'a eşit değilse YAP
        //    if (clients != client)
        //    {
        //        NetworkStream stream = client.GetStream(); // Client ile iletişime geç
        //        await stream.WriteAsync(MessageBuffer, 0, MessageBuffer.Length);
        //    }
        //}

        #endregion
        foreach (var clients in clientList.Values)
        {
            if (clients != client) // gönderene göndermeme işlemi
            {
                NetworkStream stream = clients.GetStream();
                await stream.WriteAsync(MessageBuffer, 0, MessageBuffer.Length);
            }
        }
    }

}