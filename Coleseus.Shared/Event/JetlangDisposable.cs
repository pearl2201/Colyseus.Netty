using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event
{
    /**
     * If the Event dispatcher uses Jetlang internally then it would require to
     * dispose of Jetlang {@link ChannelSubscription}s using the dispose method
     * during cleanup.
     * 
     * @author Abraham Menacherry
     * 
     */
    public interface JetlangDisposable
    {
        IDisposable getDisposable();

        void setDisposable(IDisposable disposable);
    }
}
