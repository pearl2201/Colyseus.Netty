using Coleseus.Shared.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event
{
    /**
 * This interface is implemented by event handlers which are listening on
 * messages published to a {@link Session}.
 * 
 * @author Abraham Menacherry
 * 
 */
    public interface SessionEventHandler: IEventHandler
    {
        ISession getSession();
        void setSession(ISession session);
    }
}
