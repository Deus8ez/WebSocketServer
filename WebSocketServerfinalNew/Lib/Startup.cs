using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using WebSocketServerfinalNew.Chat;
using WebSocketServerfinalNew.Lib;


namespace WebSocketServerfinalNew
{

    public class Startup
    {
        private Echo _echo = new Echo();
        
        private WebSocketServerConnectionManager _manager = new WebSocketServerConnectionManager();

        private ChatMan _chatManager = new ChatMan();

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // свойства вебсокета
            var webSocketOptions = new WebSocketOptions() 
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            // применение свойств
            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {

                if (context.WebSockets.IsWebSocketRequest)
                {
                    Console.WriteLine("WebSocket Connected");

                    // рукопожатие
                    using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        // добавления сокета
                        await _echo.ManageSocket(context, webSocket, _manager);

                        // работа с сообщениями
                        await _chatManager.ManageChat(_echo.result, _echo.buffer, webSocket, _manager, _chatManager);

                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            });
        }

    }
}
