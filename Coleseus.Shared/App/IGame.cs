using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.App
{
    public interface IGame
    {

        /**
		 * @return Returns the unique id associated with this game object.
		 */
        Object getId();

        /**
		 * @param id
		 *            Sets the unique id for this game.
		 */
        void setId(Object id);

		/**
		 * Get the name of the game. Preferably should be a unique name.
		 * 
		 * @return Returns the name of the game.
		 */
		string getGameName();

        /**
		 * Set the name of the game. Preferably it should be a unique value.
		 * 
		 * @param gameName
		 *            Set the preferably unique game name.
		 */
        void setGameName(string gameName);

        /**
		 * Each game requires a different set of game commands. This method will set
		 * the interpreter which will convert these commands to method calls.
		 * 
		 * @return The associated {@link GameCommandInterpreter} instance.
		 */
        IGameCommandInterpreter getGameCommandInterpreter();

        /**
		 * Set the interpreter associated with this game. This method will be used
		 * if the creation of the interpreter is outside of the implementing game
		 * room instance, say by a {@link Game} instance or set by the spring
		 * container.
		 * 
		 * @param interpreter
		 *            The interpreter instance to set.
		 */
        void setGameCommandInterpreter(IGameCommandInterpreter interpreter);

        /**
		 * Unloads the current game, by closing all sessions. This will delegate
		 * to {@link GameRoom#close()}
		 * 
		 * @return In case of Netty Implementation it would return a collection of
		 *         {@link ChannelFuture} object.
		 */
        Object unload();
    }
}
