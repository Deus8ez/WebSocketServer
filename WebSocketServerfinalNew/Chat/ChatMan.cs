using System;
using Newtonsoft.Json;
using System.Text;
using System.Net.WebSockets;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketServerfinalNew.Chat
{
    public class ChatMan
    {
        // функционал чата
        public async Task RouteJSONMessageAsync(string message, WebSocketServerConnectionManager _manager)
        {
            // изменения JSON в объект
            var routeOb = JsonConvert.DeserializeObject<dynamic>(message);

            // если есть получатель
            if (Guid.TryParse(routeOb.To.ToString(), out Guid guidOutput))
                {
                    Console.WriteLine("Targeted");

                    // поиск получателя в списке
                    var sock = _manager.GetAllSockets().FirstOrDefault(s => s.Key == routeOb.To.ToString());

                    // если есть
                    if (sock.Value != null)
                        {
                            if (sock.Value.State == WebSocketState.Open)
                            {
                                await sock.Value.SendAsync(Encoding.UTF8.GetBytes(routeOb.Message.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    // если нет
                    else
                        {
                            Console.WriteLine("Invalid Recipient");
                        }
                }
            // если нет отправить всем
            else
                {
                    Console.WriteLine("Broadcast");
                    
                    foreach (var sock in _manager.GetAllSockets())
                        {
                            if (sock.Value.State == WebSocketState.Open)
                            {
                                await sock.Value.SendAsync(Encoding.UTF8.GetBytes(routeOb.Message.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                }
        }

        // рассылка сообщений
        public async Task ManageChat(WebSocketReceiveResult result, byte[] buffer, WebSocket webSocket, WebSocketServerConnectionManager _manager, ChatMan _chatManager){
            while (!result.CloseStatus.HasValue)
                {   
                    // отправка
                    await _chatManager.RouteJSONMessageAsync(Encoding.UTF8.GetString(buffer, 0, result.Count), _manager);
                    
                    // ожидание нового сообщения от клиента
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);                 

                }
        }
    }
}