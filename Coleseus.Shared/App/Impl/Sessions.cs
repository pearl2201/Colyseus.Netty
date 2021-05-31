using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.App.Impl
{
    /**
   * Factory class used to create a {@link PlayerSession} instance. It will
   * create a new instance, initialize it and set the {@link GameRoom} reference
   * if necessary.
   * 
   * @author Abraham Menacherry
   * 
   */
    public class Sessions : SessionFactory
    {

        public static SessionFactory INSTANCE = new Sessions();


        public ISession newSession()
        {
            return new SessionBuilder().build();
        }


        public IPlayerSession newPlayerSession(GameRoom gameRoom, IPlayer player)
        {
            var builder = new PlayerSessionBuilder().SetParentGameRoom(gameRoom);
            builder.player = player;
            return builder.build();
        }

    }

}
