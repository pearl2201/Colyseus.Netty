using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Communication
{
    public interface MessageSender
    {
        /**
	 * This method delegates to the underlying native session object to send a
	 * message to the client.
	 * 
	 * @param message
	 *            The message to be sent to client.
	 * @return The boolean or future associated with this operation if
	 *         synchronous or asynchronous implementation respectively.
	 */
        Object sendMessage(Object message);

        /**
		 * Returns the delivery guaranty of the implementation. Currently only
		 * RELIABLE and FAST are supported, their respective integer values are 0
		 * and 1.
		 * 
		 * @return The guaranty instance  associated with the implementation.
		 */
        DeliveryGuaranty getDeliveryGuaranty();

        /**
		 * Since message sender would have a network connection, it would require
		 * some cleanup. This method can be overriden to close underlying channels
		 * and so on.
		 */
        void close();

       
    }
	/**
		* An interface whose implementations would transmit messages reliably to
		* the remote machine/vm. The transport for instance could be TCP.
		* 
		* @author Abraham Menacherry
		* 
		*/
	public interface Reliable : MessageSender { }

	/**
	 * An interface whose implementations would transmit messages fast but
	 * <b>unreliably</b> to the remote machine/vm. The transport for instance
	 * could be UDP.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
	public interface Fast : MessageSender { }
}
