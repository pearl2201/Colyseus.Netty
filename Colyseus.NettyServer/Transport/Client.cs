using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colyseus.Schema;

namespace Colyseus.Server.Transport
{
    public interface ISendOptions
    {
        bool? AfterNextPatch { get; }
    }

    public enum ClientState { JOINING, JOINED, RECONNECTED, LEAVING }

    public abstract class Client
    {

        public int readyState;

        public string id;
        public string sessionId; // TODO: remove sessionId on version 1.0.0
        public ClientState state;

        //ref: EventEmitter;

        //upgradeReq?: http.IncomingMessage; // cross-compatibility for ws (v3.x+) and uws

        /**
         * User-defined data can be attached to the Client instance through this variable.
         */
        public dynamic userData;

        /**
         * auth data provided by your `onAuth`
         */
        public dynamic auth;
        public int? pingCount; // ping / pong
        public dynamic[] _enqueuedMessages;

        public abstract void raw(byte[] data, ISendOptions options);
        public abstract void enqueueRaw(byte[] data, ISendOptions options);

        public abstract string Send(string type, dynamic message, ISendOptions options);

        public abstract string Send(int type, dynamic message, ISendOptions options);
        public abstract void send(Colyseus.Schema.Schema message, ISendOptions options);

        public abstract void error(int? code, string message);
        public abstract void leave(int? code, string data);
        public abstract void close(int? code, string data);
    }
}
