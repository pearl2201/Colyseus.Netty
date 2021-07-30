using Coleseus.Shared.App;
using Coleseus.Shared.App.Impl;
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
        TaskManagerService _taskManagerService;
        int reconnectDelay = ColyseusConfig.DEFAULT_RECONNECT_DELAY;

        public ReconnectSessionRegistry(TaskManagerService taskManagerService) : base()
        {
            _taskManagerService = taskManagerService;
        }

        public override bool putSession(String key, ISession session)
        {
            _taskManagerService.AddTask(new ClearSessionTask(key, sessions)
            {
                TaskTimeSpan = TimeSpan.FromMilliseconds(1),
                TaskRunAtStart = true
            });
            return base.putSession(key, session);
        }


        public TaskManagerService getTaskManagerService()
        {
            return _taskManagerService;
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

    public class ClearSessionTask : AbstractScheduleTask
    {

        string reconnectKey;
        ConcurrentDictionary<string, ISession> sessions;

        public ClearSessionTask(string reconnectKey,
                ConcurrentDictionary<string, ISession> sessions)
        {
            this.reconnectKey = reconnectKey;
            this.sessions = sessions;
        }

        public override void Execute()
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
    }
}
