using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using DotNetty.Codecs;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
using System.Linq;

namespace Coleseus.Shared.Handlers.Netty
{

    /**
	 * This class will convert an incoming {@link TextWebSocketFrame} to an
	 * {@link Event}. The incoming data is expected to be a JSon string
	 * representation of an Event object. This class uses {@link ObjectMapper} to do
	 * the decoding to {@link DefaultEvent}. If the incoming event is of type
	 * {@link Events#NETWORK_MESSAGE} then it will be converted to
	 * {@link Events#SESSION_MESSAGE}.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */

    //public class TextWebsocketDecoder :
    //        MessageToMessageDecoder<TextWebSocketFrame>
    //{


    //    /**
    //     * This will be put into the {@link ChannelHandlerContext} the first time
    //     * attr method is invoked on it. The get is also a set.
    //     */
    //    private AttributeKey<IEvent> eventClass = AttributeKey<IEvent>.NewInstance(
    //            "eventClass");


    //    protected override void Decode(IChannelHandlerContext ctx, TextWebSocketFrame frame,
    //            List<Object> @out)
    //    {
    //        // Get the existing class from the context. If not available, then
    //        // default to DefaultEvent.class
    //        IAttribute<IEvent> attr = ctx.GetAttribute(eventClass);
    //        Type theClass = attr.GetType();
    //        bool unknownClass = false;
    //        if (null == theClass)
    //        {
    //            unknownClass = true;
    //            theClass = typeof(DefaultEvent);
    //        }
    //        string json = frame.Text();
    //        IEvent @event = (IEvent)JsonConvert.DeserializeObject(json, theClass);

    //        // If the class is unknown then either check if its the default event or
    //        // a different class. Put the right one in the context.
    //        if (unknownClass)

    //        {
    //            string cName = ((DefaultEvent)@event).GetType();
    //            if (null == cName)
    //            {
    //                attr.Set(typeof(DefaultEvent));
    //            }
    //            else
    //            {


    //                Class <? extends Event > newClass = (Class <? extends Event >) Class
    //                            .forName(cName);
    //                @event = (IEvent)JsonConvert.DeserializeObject(json, theClass);
    //                attr.Set(newClass);
    //            }
    //        }

    //        if (@event.getType() == Events.NETWORK_MESSAGE)

    //        {
    //            @event.setType(Events.SESSION_MESSAGE);
    //        }
    //        @out.Add(@event);
    //    }


    //    public ObjectMapper getJackson()
    //    {
    //        return jackson;
    //    }

    //    public void setJackson(ObjectMapper jackson)
    //    {
    //        this.jackson = jackson;
    //    }

    //    public override bool IsSharable => true;

    //    public static Type ByName(string name)
    //    {
    //        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
    //        {
    //            var tt = assembly.GetType(name);
    //            if (tt != null)
    //            {
    //                return tt;
    //            }
    //        }

    //        return null;
    //    }
    //}

}
