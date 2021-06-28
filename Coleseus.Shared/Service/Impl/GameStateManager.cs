using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Serilog;

namespace Coleseus.Shared.Service.Impl
{
    class GameStateManager : IGameStateManagerService
    {
        private readonly ILogger _logger = Serilog.Log.Logger.ForContext<GameStateManager>();

        private Object state;
        byte[] serializedBytes;
        private int syncKey;

        public GameStateManager()
        {
            state = null;
            syncKey = -1;
        }

        public GameStateManager(Object state, int syncKey) : base()
        {

            this.state = state;
            this.syncKey = syncKey;
        }


        public Object getState()
        {
            return state;
        }


        public void setState(Object state)
        {
            this.state = state;
        }


        public bool compareAndSetState(Object key, Object state)
        {
            bool syncKeySet = compareAndSetSyncKey(key);
            if (compareAndSetSyncKey(key))
            {
                this.state = state;
            }
            return syncKeySet;
        }


        public Object getSyncKey()
        {
            return syncKey;
        }


        public bool compareAndSetSyncKey(Object key)
        {
            if (null == key || !(key is int))
            {
                _logger.Error("Invalid key provided: {}", key);
                return false;
            }

            int newKey = (int)key;
            return Interlocked.Increment(ref syncKey); 
        }


        public byte[] getSerializedByteArray()
        {
            return serializedBytes;
        }


        public void setSerializedByteArray(byte[] serializedBytes)
        {
            this.serializedBytes = serializedBytes;
        }


        public Object computeAndSetNextState(Object state, Object syncKey,
                Object stateAlgorithm)
        {
            throw new MissingMethodException("computeAndSetNextState"
                    + "(Object state, Object syncKey,"
                    + "Object stateAlgorithm) not supported yet");
        }

        public Object computeNextState(Object state, Object syncKey,
                    Object stateAlgorithm)
        {
            throw new MissingMethodException("computeNextState"
                    + "(Object state, Object syncKey, Object stateAlgorithm)"
                    + " not supported yet");
        }


        public Object getStateAlgorithm()
        {
            throw new MissingMethodException("getStateAlgorithm()"
                    + " not supported yet");
        }
    }
}
