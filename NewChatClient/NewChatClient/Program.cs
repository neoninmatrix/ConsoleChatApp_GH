using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        TcpClient client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 1234);

        Console.WriteLine("Server'a bağlanıldı");

        NetworkStream stream = client.GetStream();

        Console.Write("Lütfen isminizi giriniz: ");
        string clientName = Console.ReadLine();
        byte[] nameBuffer = Encoding.UTF8.GetBytes(clientName);
        await stream.WriteAsync(nameBuffer, 0, nameBuffer.Length);

        _ = Task.Run(() => ReadMessages(stream));

        while (true)
        {
            string message = Console.ReadLine();
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(messageBuffer, 0, messageBuffer.Length);
        }
    }

    public static async Task ReadMessages(NetworkStream stream)
    {
        byte[] bufferMessage = new byte[1024];

        while (true)
        {
            try
            {
                int bufMesLength = await stream.ReadAsync(bufferMessage, 0, bufferMessage.Length);
                if (bufMesLength > 0)
                {
                    string message = Encoding.UTF8.GetString(bufferMessage, 0, bufMesLength);
                    Console.WriteLine("Server: " + message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Okuma hatası: " + ex.Message);
                break;
            }
        }
    }
}