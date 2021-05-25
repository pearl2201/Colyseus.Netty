using Coleseus.Shared.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Service
{
    public interface ISessionRegistryService<T>
    {
        ISession getSession(T key);

        bool putSession(T key, ISession session);

        bool removeSession(T key);
        // Add a session type object also to get udp/tcp/any
    }
}
