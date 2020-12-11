using System;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace WebSocketServerfinalNew.Lib
{
    public class Echo
    {
        public byte[] buffer = new byte[1024 * 4];

        public WebSocketReceiveResult result;

        public async Task ManageSocket(HttpContext context, WebSocket webSocket, WebSocketServerConnectionManager _manager){
 

            // добавить веб сокет в список
            string ConnID = _manager.AddSocket(webSocket);

            // отправить ID клиенту
            await _manager.SendConnIDAsync(webSocket, ConnID);

            // полученное сообщение
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // если клиент просит закрыть соединение
            if(result.MessageType == WebSocketMessageType.Close){

                // получение соответ. ID из списка
                string id = _manager.GetAllSockets().FirstOrDefault(s => s.Value == webSocket).Key;

                Console.WriteLine($"Receive->Close");

                // удалене веб соккета из списка
                _manager.GetAllSockets().TryRemove(id, out WebSocket sock);

                Console.WriteLine("Managed Connections: " + _manager.GetAllSockets().Count.ToString());

                await sock.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                return;
            }  
        }
    }
}