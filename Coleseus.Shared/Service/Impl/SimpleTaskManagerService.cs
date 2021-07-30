using Coleseus.Shared.App;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coleseus.Shared.Service.Impl
{


    /**
	 * A thin wrapper on a <a href="http://download.oracle.com/javase/1.5.0/docs/api/java/util/concurrent/ScheduledThreadPoolExecutor.html"
	 * >ScheduledThreadPoolExecutor</a> class. It is used so as to keep track of all
	 * the tasks. In future they could be made durable tasks which can be
	 * transferred between multiple nodes for fail over, etc.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
    public class SimpleTaskManagerService :  TaskManagerService
    {
        private readonly List<ScheduleTask> _tasks;

        public SimpleTaskManagerService()
        {
            _tasks = new List<ScheduleTask>();
        }

        public void AddTask(ScheduleTask task)
        {
            _tasks.Add(task);
        }

        public void RemoveTask(ScheduleTask task)
        {
            _tasks.Remove(task);
        }

        public void Start()
        {
            foreach (ScheduleTask task in _tasks) task.Start();
        }

        public void Stop()
        {
            foreach (ScheduleTask task in _tasks) task.Stop();
        }

        public void Clear()
        {
            _tasks.Clear();
        }

    }

}
