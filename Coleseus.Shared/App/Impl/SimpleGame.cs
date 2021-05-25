using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Coleseus.Shared.App.Impl
{

    /**
	 * Domain object representing a game. This is a convenience implementation of
	 * the {@link Game} interface so that simple games need not implement their own.
	 * <b>Note</b> This implementation will throw exception if any of the setter
	 * methods are invoked. All variables are final in this class and expected to be
	 * set at object construction.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
    public class SimpleGame : IGame
    {

        /**
         * This variable could be used as a database key.
         */
        private Object id;

        /**
         * The name of the game.
         */
        private String gameName;
        /**
         * Each game has its own specific commands. This instance will be used to
         * transform those commands(most probably in the form of bytes) to actual
         * java method calls.
         */
        private IGameCommandInterpreter gameCommandInterpreter;

        public SimpleGame(Object id, String gameName) : this(id, gameName, null)
        {

        }

        public SimpleGame(Object id, String gameName,
                IGameCommandInterpreter gameCommandInterpreter)
        {

            this.id = id;
            this.gameName = gameName;
            this.gameCommandInterpreter = gameCommandInterpreter;
        }

        /**
         * Meant as a database access key.
         * 
         * @return The unique identifier of this Game.
         */

        public Object getId()
        {
            return id;
        }

        /**
         * Meant as a database access key.
         * 
         * @param id
         *            Set the unique identifier for this game.
         */

        public void setId(Object id)
        {
            throw new Exception(
                    "Game id is a final variable to be set at Game construction. "
                            + "It cannot be set again.");
        }


        public String getGameName()
        {
            return gameName;
        }


        public void setGameName(String gameName)
        {
            throw new Exception(
                    "GameName is a final variable to be set at Game construction. "
                            + "It cannot be set again.");
        }


        public IGameCommandInterpreter getGameCommandInterpreter()
        {
            return gameCommandInterpreter;
        }


        public void setGameCommandInterpreter(IGameCommandInterpreter interpreter)
        {
            throw new Exception(
                    "Game id is a final variable to be set at Game construction. "
                            + "It cannot be set again.");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Object unload()
        {
            return null;
        }


        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result
                    + ((gameName == null) ? 0 : gameName.GetHashCode());
            result = prime * result + ((id == null) ? 0 : id.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            SimpleGame other = (SimpleGame)obj;
            if (gameName == null)
            {
                if (other.gameName != null)
                    return false;
            }
            else if (!gameName.Equals(other.gameName))
                return false;
            if (id == null)
            {
                if (other.id != null)
                    return false;
            }
            else if (!id.Equals(other.id))
                return false;
            return true;
        }

    }
}
