using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Coleseus.Shared.App.Impl
{

    public class DefaultPlayer : IPlayer
    {
        /**
         * This variable could be used as a database key.
         */
        private Object id;

        /**
         * The name of the gamer.
         */
        private string name;
        /**
         * Email id of the gamer.
         */
        private string emailId;

        private Mutex mut = new Mutex();

        /**
         * One player can be connected to multiple games at the same time. Each
         * session in this set defines a connection to a game. TODO, each player
         * should not have multiple sessions to the same game.
         */
        private HashSet<IPlayerSession> playerSessions;

        public DefaultPlayer()
        {
            id = Guid.NewGuid();
            playerSessions = new HashSet<IPlayerSession>();
        }

        public DefaultPlayer(Object id, string name, string emailId) : base()
        {

            this.id = id;
            this.name = name;
            this.emailId = emailId;
            playerSessions = new HashSet<IPlayerSession>();
        }


        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
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
            DefaultPlayer other = (DefaultPlayer)obj;
            if (id == null)
            {
                if (other.id != null)
                    return false;
            }
            else if (!id.Equals(other.id))
                return false;
            return true;
        }


        public Object getId()
        {
            return id;
        }


        public void setId(Object id)
        {
            this.id = id;
        }

        public String getName()
        {
            return name;
        }


        public void setName(String name)
        {
            this.name = name;
        }


        public String getEmailId()
        {
            return emailId;
        }


        public void setEmailId(String emailId)
        {
            this.emailId = emailId;
        }


        public bool addSession(IPlayerSession session)
        {
            mut.WaitOne();
            var result = playerSessions.Add(session);
            mut.ReleaseMutex();
            return result;
        }


        public bool removeSession(IPlayerSession session)
        {
            mut.WaitOne();
            bool remove = playerSessions.Remove(session);
            if (playerSessions.Count == 0)
            {
                logout(session);
            }
            mut.ReleaseMutex();
            return remove;
        }

        public void logout(IPlayerSession session)
        {
            mut.WaitOne();
            session.Close();
            if (null != playerSessions)
            {
                playerSessions.Remove(session);
            }
            mut.ReleaseMutex();
        }

        public HashSet<IPlayerSession> getPlayerSessions()
        {
            return playerSessions;
        }

        public void setPlayerSessions(HashSet<IPlayerSession> playerSessions)
        {
            this.playerSessions = playerSessions;
        }

    }

}
