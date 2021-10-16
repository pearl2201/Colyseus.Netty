using Coleseus.Shared.App;
using Coleseus.Shared.App.Impl;
using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using Colyseus.NettyServer.ZombieGame.Domain;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.ZombieGame.Game
{
    public class SessionHandler : DefaultSessionEventHandler, IGameCommandInterpreter
    {
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<SessionHandler>();
        volatile int cmdCount;

        private Defender defender;
        private Zombie zombie;
        private IAM iam;

        public SessionHandler(ISession session, Defender defender, Zombie zombie, IAM iam) : base(session)
        {

            this.defender = defender;
            this.zombie = zombie;
            this.iam = iam;
        }

        public override void onDataIn(IEvent @event)
        {
            try
            {
                interpretCommand(@event.getSource());

            }
            catch (InvalidCommandException e)
            {

                _logger.Error("{}", e);
            }
        }


        public void interpretCommand(Object command)
        {
            cmdCount++;
            int type;
            int operation;
            bool isWebSocketProtocol = false;
            if (command is MessageBuffer<IByteBuffer>)
            {
                MessageBuffer<IByteBuffer> buf = (MessageBuffer<IByteBuffer>)command;
                type = buf.readInt();
                operation = buf.readInt();
            }
            else
            {
                // websocket
                isWebSocketProtocol = true;
                List<int> data = (List<int>)command;

                type = data[0];
                operation = data[1];
            }
            IAM iam = (IAM)type;
            ZombieCommands cmd = (ZombieCommands)(operation);
            switch (iam)

            {
                case IAM.ZOMBIE:
                    switch (cmd)
                    {
                        case ZombieCommands.EAT_BRAINS:
                            //LOG.trace("Interpreted command EAT_BRAINS");
                            zombie.eatBrains();
                            break;
                        case ZombieCommands.SELECT_TEAM:
                            _logger.Verbose("Interpreted command ZOMBIE SELECT_TEAM");
                            selectTeam(iam);
                            break;
                    }
                    break;
                case IAM.DEFENDER:
                    switch (cmd)
                    {
                        case ZombieCommands.SHOT_GUN:
                            //LOG.trace("Interpreted command SHOT_GUN");
                            defender.shotgun();
                            break;
                        case ZombieCommands.SELECT_TEAM:
                            _logger.Verbose("Interpreted command DEFENDER SELECT_TEAM");
                            selectTeam(iam);
                            break;
                    }
                    break;
                default:
                    _logger.Error("Received invalid command {}", cmd);
                    throw new InvalidCommandException("Received invalid command" + cmd);
            }

            if (isWebSocketProtocol)
            {
                Session.OnEvent(Events.NetworkEvent(cmdCount));
            }
            else if ((cmdCount % 10000) == 0)

            {
                NettyMessageBuffer buffer = new NettyMessageBuffer();
                //System.out.println("Command No: " + cmdCount);
                buffer.writeInt(cmdCount);
                //			Event tcpEvent = Events.dataOutTcpEvent(buffer);
                //			getSession().onEvent(tcpEvent);
                INetworkEvent udpEvent = null;
                udpEvent = Events.NetworkEvent(buffer, DeliveryGuaranty.FAST);
                Session.OnEvent(udpEvent);
            }
            else
            {
                NettyMessageBuffer buffer = new NettyMessageBuffer();

                buffer.writeInt(cmdCount);
                var udpEvent = Events.NetworkEvent(buffer, DeliveryGuaranty.FAST);
                Session.TcpSender.sendMessage(udpEvent);
            }
        }

        public void selectTeam(IAM iam)
        {
            this.iam = iam;
        }

        public Defender getDefender()
        {
            return defender;
        }

        public void setDefender(Defender defender)
        {
            this.defender = defender;
        }

        public Zombie getZombie()
        {
            return zombie;
        }

        public void setZombie(Zombie zombie)
        {
            this.zombie = zombie;
        }

        public IAM getIam()
        {
            return iam;
        }

        public void setIam(IAM iam)
        {
            this.iam = iam;
        }
    }
}
