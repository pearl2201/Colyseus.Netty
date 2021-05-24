using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.App
{
    /**
     * Used to create sessions. Implementations of this factory can be passed on to
     * {@link GameRoom}'s which can then use it to create player sessions during
     * login.
     * 
     * @author Abraham Menacherry
     * 
     */
    public interface SessionFactory
    {
         ISession newSession();

         IPlayerSession newPlayerSession(GameRoom gameRoom, Player player);
    }
}
