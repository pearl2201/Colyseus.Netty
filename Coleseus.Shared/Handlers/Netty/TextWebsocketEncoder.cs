using Coleseus.Shared.Event;
using DotNetty.Codecs;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Coleseus.Shared.Handlers.Netty
{
    /**
   * This encoder will convert an incoming object (mostly expected to be an
   * {@link Event} object) to a {@link TextWebSocketFrame} object. It uses
   * {@link ObjectMapper} from jackson library to do the Object to JSon String
   * encoding.
   * 
   * @author Abraham Menacherry
   * 
   */

    //public class TextWebsocketEncoder : MessageToMessageEncoder<IEvent>
    //{


    //    protected override void Encode(IChannelHandlerContext ctx, IEvent msg,
    //            List<Object> @out)
    //    {
    //        string json = JsonConvert.SerializeObject()
    //        @out.Add(new TextWebSocketFrame(json));
    //    }


    //    public override bool IsSharable => true;
    //}

 

    public class TypeInfoConverter : JsonConverter
    {
        private readonly IEnumerable<Type> _types;

        public TypeInfoConverter(IEnumerable<Type> types)
        {
            Contract.Requires(types != null);

            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var converters = serializer.Converters.Where(x => !(x is TypeInfoConverter)).ToArray();

            var jObject = JObject.FromObject(value);
            jObject.AddFirst(new JProperty("Type", value.GetType().Name));
            jObject.WriteTo(writer, converters);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t.IsAssignableFrom(objectType));
        }
    }
}
