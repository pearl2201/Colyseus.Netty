using Coleseus.Shared.App;
using System;
using System.Collections.Concurrent;
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


        public override bool putSession(String key, ISession session)
        {
            taskManagerService.schedule(new ClearSessionTask(key, sessions),
                    reconnectDelay, TimeUnit.MILLISECONDS);
            return base.putSession(key, session);
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

    public class ClearSessionTask : ScheduleTask
    {

        string reconnectKey;
        ConcurrentDictionary<string, ISession> sessions;

        public ClearSessionTask(string reconnectKey,
                ConcurrentDictionary<string, ISession> sessions)
        {
            this.reconnectKey = reconnectKey;
            this.sessions = sessions;
        }


        public void run()
        {
            ISession session = sessions[reconnectKey];
            if (null != session)
            {
                lock (session)

                {
                    // at this point it could have been removed by re-connect
                    // handler already, hence another null check required.
                    if (sessions.TryRemove(reconnectKey, out ISession removeSession))
                    {
                        removeSession.close();
                    }

                }
            }
        }


        public Object getId()
        {
            return null;
        }


        public void setId(Object id)
        {
        }

    }
}
