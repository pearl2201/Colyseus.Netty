using Coleseus.Shared.App;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public enum TimeUnit
{
    MILLISECONDS
}
namespace Coleseus.Shared.Service
{

    /**
	 * Defines and interface for management of tasks in the server. It declares
	 * functionality for executing recurring tasks with delay etc. The interface may
	 * also be used as a repository of tasks, with features like persistence built
	 * in by the implementation.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
    public interface TaskManagerService
    {
        void AddTask(ScheduleTask task);

        void RemoveTask(ScheduleTask task);

        void Start();

        void Stop();

        void Clear();
    }
}
