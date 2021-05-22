using Colyseus.Common;
using Colyseus.Common.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Colyseus.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketsController : ControllerBase, IConsumer<QueueMessage>
    {
        private readonly ILogger<WebSocketsController> _logger;
        private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> _groups = new ConcurrentDictionary<string, ConcurrentBag<string>>();
        private static readonly ConcurrentDictionary<string, WebSocket> _sessions = new ConcurrentDictionary<string, WebSocket>();

        private readonly IPublishEndpoint _publishEndpoint;
        public WebSocketsController(ILogger<WebSocketsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/ws")]
        public async Task Get()
        {

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var currentSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                var socketId = $"s:{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}";
                _sessions.TryAdd(socketId, currentSocket);

                while (true)
                {
                    if (HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        break;
                    }

                    var ioMessage = await ReceiveMessageAsync(currentSocket, HttpContext.RequestAborted);

                    var message = new QueueMessage(ioMessage)
                    {
                        Status = Constants.MessageStatus.Received
                    };

                    //using (var stream = new MemoryStream())
                    //{
                    //    //Serializer.Serialize(stream, message);
                    //    //_channel.BasicPublish(exchange: Constants.MessageStatus.Received,
                    //    //                      routingKey: "",
                    //    //                      basicProperties: null,
                    //    //                      body: stream.ToArray());


                    //}

                    await _publishEndpoint.Publish<QueueMessage>(message);
                }

                _sessions.TryRemove(socketId, out var dummy);

                await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, currentSocket.CloseStatusDescription, HttpContext.RequestAborted);
                currentSocket.Dispose();
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
            _logger.Log(LogLevel.Information, "Message received from Client");

            while (!result.CloseStatus.HasValue)
            {
                var serverMsg = Encoding.UTF8.GetBytes($"Server: Hello. You said: {Encoding.UTF8.GetString(buffer)}");
                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message sent to Client");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message received from Client");

            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _logger.Log(LogLevel.Information, "WebSocket connection closed");
        }

        private static async Task<IoMessage> ReceiveMessageAsync(WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                var message = Encoding.UTF8.GetString(ms.ToArray());
                return result.MessageType != WebSocketMessageType.Binary ? null : JsonConvert.DeserializeObject<IoMessage>(message);

            }
        }

        private static Task SendMessageAsync(WebSocket socket, IoMessage message, CancellationToken ct = default(CancellationToken))
        {
            if (socket == null || socket.State != WebSocketState.Open) return Task.FromCanceled(ct);

            //using (var stream = new MemoryStream())
            //{
            //    var ms = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            //    Serializer.Serialize(stream, message);
            //    stream.TryGetBuffer(out var buffer);
            //    return socket.SendAsync(ms, WebSocketMessageType.Binary, true, ct);
            //}

            var ms = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            return socket.SendAsync(ms, WebSocketMessageType.Binary, true, ct);
        }

        private static void ReceiveInternalControlMessage(QueueMessage message)
        {
            throw new NotImplementedException();
        }

        public async Task Consume(ConsumeContext<QueueMessage> context)
        {

            var message = context.Message;
            if (message.Status == Constants.MessageStatus.Dropped) return;

            var target = message.To;

            switch (target.Substring(0, 2))
            {
                case "s:":
                    _sessions.TryGetValue(message.To, out var webSocket);

                    await SendMessageAsync(webSocket, message);

                    break;
                case "g:":
                    _groups.TryGetValue(message.To, out var sessions);

                    foreach (var session in sessions)
                    {
                        _sessions.TryGetValue(session, out var socket);
                        await SendMessageAsync(socket, message);
                    }

                    break;
                case "i:":
                    if (target == Constants.MessageTarget.IOG)
                        ReceiveInternalControlMessage(message);
                    break;
                default:
                    return;
            }
        }
    }
}
