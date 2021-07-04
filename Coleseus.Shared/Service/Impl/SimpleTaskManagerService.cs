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
    public class SimpleTaskManagerService : ScheduledThreadPoolExecutor, TaskManagerService
    {
        /**
         * Used to create a unique identifier for each task
         */
        private AtomicInteger taskNum;

        public SimpleTaskManagerService(int corePoolSize)
        {
            base(corePoolSize);
            taskNum = new AtomicInteger(0);
        }


        public void Execute(ScheduleTask task)
        {
            base.execute(task);
        }


        public Task Schedule(ScheduleTask task, long delay, TimeUnit unit)
        {
            task.setId(taskNum.incrementAndGet());
            return base.schedule(task, delay, unit);
        }


        public Task ScheduleAtFixedRate(ScheduleTask task, long initialDelay,
                long period, TimeUnit unit)
        {
            task.setId(taskNum.incrementAndGet());
            return base.scheduleAtFixedRate(task, initialDelay, period, unit);
        }


        public Task ScheduleWithFixedDelay(ScheduleTask task,
                long initialDelay, long delay, TimeUnit unit)
        {
            task.setId(taskNum.incrementAndGet());
            return base.scheduleWithFixedDelay(task, initialDelay, delay, unit);
        }

    }

}
