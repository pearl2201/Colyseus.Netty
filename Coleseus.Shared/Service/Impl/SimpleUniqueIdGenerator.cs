using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Service.Impl
{
    public class SimpleUniqueIdGenerator : UniqueIDGeneratorService
    {
        public object generate()
        {
            return Guid.NewGuid();
        }

        public object generateFor(Type klass)
        {
            return Guid.NewGuid();
        }
    }
}
