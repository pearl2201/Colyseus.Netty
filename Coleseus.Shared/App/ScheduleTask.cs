using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.App
{
    /**
 * Represents a task that can be executed in the game system. Any class that
 * implements this interface and submits instances to the
 * {@link TaskManagerService} instance will be managed by the container. It
 * will automatically store the task such that restarts of the server do not
 * stop recurring tasks from stopping. In future, this may also be used for
 * sending tasks from one server node to another during node shutdown etc.
 * 
 * @author Abraham Menacherry
 * 
 */
    public interface ScheduleTask
    {
        /**
		 * @return returns the unique task id of the task. For future
		 *         implementations, this value has to be unique across multiple
		 *         server nodes.
		 */
        Object getId();

        /**
         * @param id
         *            Set the unique task id.
         */
        void setId(Object id);

        /**
      * When an object implementing interface <code>Runnable</code> is used
      * to create a thread, starting the thread causes the object's
      * <code>run</code> method to be called in that separately executing
      * thread.
      * <p>
      * The general contract of the method <code>run</code> is that it may
      * take any action whatsoever.
      *
      * @see     java.lang.Thread#run()
      */
        void run();
    }

}
