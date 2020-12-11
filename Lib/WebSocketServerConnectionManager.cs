using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace WebSocketServerfinalNew
{
    public class WebSocketServerConnectionManager
    {
        // список вебсокетов
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        //добавление сокета 
        public string AddSocket(WebSocket socket)
        {
            // генерация ID
            string ConnID = Guid.NewGuid().ToString();

            // добавление в коллекцию
            _sockets.TryAdd(ConnID, socket);

            Console.WriteLine("WebSocketServerConnectionManager-> AddSocket: WebSocket added with ID: " + ConnID);

            return ConnID;
        }

        // отправить ID клиенту 
        public async Task SendConnIDAsync(WebSocket socket, string connID)
        {
            var buffer = Encoding.UTF8.GetBytes("ConnID: " + connID);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }   

        // вернуть все сокеты
        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }
    }
}