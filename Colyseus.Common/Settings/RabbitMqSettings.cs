using System;
using System.Collections.Generic;
using System.Text;

namespace Colyseus.Common.Settings
{
    public class RabbitMqSettings
    {
        public string Host { get; set; }

        public ushort Port { get; set; }
        public string Virtual { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Endpoint { get; set; }
    }
}
