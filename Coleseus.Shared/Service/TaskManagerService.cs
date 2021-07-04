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
        void Execute(ScheduleTask task);

        /**
		 * Creates and executes a one-shot action that becomes enabled after the given delay.
		 * @param task The task to be scheduled.
		 * @param delay Initial delay before the task is executed
		 * @param unit seconds, milliseconds etc for delay
		 * @return A scheduled future, which is a handle to the task and can be used
		 *         to cancel it, get completed status etc.
		 */

        Task Schedule(ScheduleTask task, long delay, TimeUnit unit);

        /**
		 * Creates and executes a periodic action that becomes enabled first after
		 * the given initial delay, and subsequently with the given period; that is
		 * executions will commence after initialDelay then initialDelay+period,
		 * then initialDelay + 2 * period, and so on.
		 * 
		 * @param task
		 *            The task to be scheduled.
		 * @param initialDelay
		 *            Initial delay before the task is first done.
		 * @param period
		 *            Fixed period of execution.
		 * @param unit
		 *            seconds, milliseconds etc for delay.
		 * @return A scheduled future, which is a handle to the task and can be used
		 *         to cancel it, get completed status etc.
		 */

        Task ScheduleAtFixedRate(ScheduleTask task, long initialDelay,
           long period, TimeUnit unit);

        /**
		 * Creates and executes a periodic action that becomes enabled first after
		 * the given initial delay, and subsequently with the given delay between
		 * the termination of one execution and the commencement of the next.
		 * 
		 * @param task
		 *            The task to be scheduled.
		 * @param initialDelay
		 *            Initial delay before the task is first done.
		 * @param delay
		 *            The delay between each subsequent execution
		 * @param unit
		 *            seconds, milliseconds etc for delay.
		 * @return A scheduled future, which is a handle to the task and can be used
		 *         to cancel it, get completed status etc.
		 */

        Task ScheduleWithFixedDelay(ScheduleTask task,
           long initialDelay, long delay, TimeUnit unit);

    }
}
