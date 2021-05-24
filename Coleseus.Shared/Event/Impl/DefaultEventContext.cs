using Coleseus.Shared.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event.Impl
{
    public class DefaultEventContext : IEventContext
    {
        private Object attachement;
        private ISession session;


        public Object getAttachment()
        {
            return attachement;
        }


        public ISession getSession()
        {
            return session;
        }


        public void setAttachment(Object attachement)
        {
            this.attachement = attachement;
        }


        public void setSession(ISession session)
        {
            this.session = session;
        }
    }
}
