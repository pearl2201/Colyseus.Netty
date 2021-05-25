using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.App.Impl
{
    public class DefaultPlayer : IPlayer
    {
        public object getId()
        {
            throw new NotImplementedException();
        }

        public void setId(object uniqueKey)
        {
            throw new NotImplementedException();
        }

        public string getName()
        {
            throw new NotImplementedException();
        }

        public void setName(string name)
        {
            throw new NotImplementedException();
        }

        public string getEmailId()
        {
            throw new NotImplementedException();
        }

        public void setEmailId(string emailId)
        {
            throw new NotImplementedException();
        }

        public bool addSession(IPlayerSession session)
        {
            throw new NotImplementedException();
        }

        public bool removeSession(IPlayerSession session)
        {
            throw new NotImplementedException();
        }

        public void logout(IPlayerSession playerSession)
        {
            throw new NotImplementedException();
        }
    }
}
