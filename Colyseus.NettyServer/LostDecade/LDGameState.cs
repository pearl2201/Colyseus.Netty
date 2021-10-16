using Coleseus.Shared.Communication;
using Coleseus.Shared.Handlers.Netty;
using DotNetty.Buffers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.LostDecade
{
    public class LDGameState : IDataBufferSchema
    {
        public const string HASH_CODE = "LDGameState";
        public HashSet<Entity> Entities { get; set; }
        public Entity Monster { get; set; }
        public Entity Hero { get; set; }
        public bool Reset { get; set; }

        public LDGameState()
        {

        }


        public LDGameState(HashSet<Entity> entities, Entity monster, Entity hero)
        {

            this.Entities = entities;
            this.Monster = monster;
            this.Hero = hero;
        }

        public void AddEntitiy(Entity hero)
        {
            // only the id will match, but other values maybe different.
            Entities.Remove(hero);
            Entities.Add(hero);
        }

        public MessageBuffer<IByteBuffer> ToMessageBuffer(MessageBuffer<IByteBuffer> messageBuffer)
        {
            var msg = JsonConvert.SerializeObject(this);
            messageBuffer.writeString(msg);
            return messageBuffer;
        }


        public MessageBuffer<IByteBuffer> ToMessageBuffer()
        {
            var info = JsonConvert.SerializeObject(this);
            MessageBuffer<IByteBuffer> messageBuffer = new NettyMessageBuffer();
            messageBuffer.writeString(info);

            return messageBuffer;
        }

        public static LDGameState FromMessageBuffer(MessageBuffer<IByteBuffer> messageBuffer)
        {
            var info = messageBuffer.readString();
            var state = JsonConvert.DeserializeObject<LDGameState>(info);
            return state;
        }
    }
}
