using Coleseus.Shared.App;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Service.Impl
{
	public class SessionRegistry<T> : ISessionRegistryService<T>
{
	protected ConcurrentDictionary<T, ISession> sessions;

	public SessionRegistry()
	{
		sessions = new ConcurrentDictionary<T, ISession>();
	}

	
	public ISession getSession(T key)
	{
		return sessions[key];
	}

	
	public virtual bool putSession(T key, ISession session)
	{
		if (null == key || null == session)
		{
			return false;
		}

		if (sessions.TryAdd(key, session))
		{
			return true;
		}
		return false;
	}

	
	public bool removeSession(T key)
	{
		return sessions.TryRemove(key, out ISession session);
	}

}
}
