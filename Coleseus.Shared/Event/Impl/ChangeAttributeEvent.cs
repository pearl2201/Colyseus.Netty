using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event.Impl
{
    public class ChangeAttributeEvent : DefaultEvent
    {

        private string key;
        private Object value;

        public ChangeAttributeEvent(string key, Object value)
        {
            this.key = key;
            this.value = value;
        }


        public override int getType()
        {
            return Events.CHANGE_ATTRIBUTE;
        }

        public String getKey()
        {
            return key;
        }

        public void setKey(String key)
        {
            this.key = key;
        }

        public Object getValue()
        {
            return value;
        }

        public void setValue(Object value)
        {
            this.value = value;
            this.setSource(value);
        }


        public override string ToString()
        {
            return "ChangeAttributeEvent [key=" + key + ", value=" + value
                    + ", type=" + type + "]";
        }
    }
}
