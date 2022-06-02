using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Storage.Module.Repositories.Interfaces;
using Storage.Module.StaticClasses;

namespace Storage.Module.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WebSocketController> _logger;

        public WebSocketController(IServiceScopeFactory scopeFactory, ILogger<WebSocketController> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        [HttpGet("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                _logger.Log(LogLevel.Information, "WebSocket connection established");
                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            _logger.LogInformation("Message received from Client");

            while (!result.CloseStatus.HasValue)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    ISettingsRepository settingsRepository =
                        scope.ServiceProvider
                            .GetRequiredService<ISettingsRepository>();
                    var binanceSellEnable = await settingsRepository.GetSettingsByKeyAsync<bool>(SettingsKeys.BinanceSellEnable);

                    var serverMsg = Encoding.UTF8.GetBytes(binanceSellEnable.Value.ToString());
                    await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                }
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _logger.LogInformation("WebSocket connection closed");
        }
    }
}