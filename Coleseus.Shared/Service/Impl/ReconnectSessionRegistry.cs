using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Service.Impl
{

	/**
	 * A session registry that will do auto cleanup of the {@link Session} after
	 * waiting for a specified amount of time for reconnection from remote client.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
	public class ReconnectSessionRegistry : SessionRegistry<String>
{
		TaskManagerService taskManagerService;
	int reconnectDelay = ColyseusConfig.DEFAULT_RECONNECT_DELAY;

	
	public bool putSession(String key, Session session)
	{
		taskManagerService.schedule(new ClearSessionTask(key, sessions),
				reconnectDelay, TimeUnit.MILLISECONDS);
		return super.putSession(key, session);
	}

	protected static class ClearSessionTask implements Task
	{

		final String reconnectKey;
		final Map<String, Session> sessions;

		protected ClearSessionTask(String reconnectKey,
				Map<String, Session> sessions)
	{
		this.reconnectKey = reconnectKey;
		this.sessions = sessions;
	}

	@Override
		public void run()
	{
		Session session = sessions.get(reconnectKey);
		if (null != session)
		{
			synchronized(session)
				{
				// at this point it could have been removed by re-connect
				// handler already, hence another null check required.
				Session removeSession = sessions.remove(reconnectKey);
				if (null != removeSession)
					removeSession.close();
			}
		}
	}

	@Override
		public Object getId()
	{
		return null;
	}

	@Override
		public void setId(Object id)
	{
	}

}

public TaskManagerService getTaskManagerService()
{
	return taskManagerService;
}

public void setTaskManagerService(TaskManagerService taskManagerService)
{
	this.taskManagerService = taskManagerService;
}

public int getReconnectDelay()
{
	return reconnectDelay;
}

public void setReconnectDelay(int reconnectDelay)
{
	this.reconnectDelay = reconnectDelay;
}

}

}
