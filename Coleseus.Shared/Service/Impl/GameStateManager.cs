using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Serilog;

namespace Coleseus.Shared.Service.Impl
{
    public class GameStateManager : IGameStateManagerService
    {
        private readonly ILogger _logger = Serilog.Log.Logger.ForContext<GameStateManager>();

        private Object state;
        byte[] serializedBytes;
        private int syncKey;
        private readonly object _syncObj = new object();
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


        public Object State { get { return state; } set { state = value; } }


        public bool CompareAndSetState(Object key, Object state)
        {
            bool syncKeySet = CompareAndSetSyncKey(key);
            if (syncKeySet)
            {
                this.state = state;
            }
            return syncKeySet;
        }


        public Object GetSyncKey()
        {
            return syncKey;
        }


        public bool CompareAndSetSyncKey(Object key)
        {
            lock (_syncObj)
            {
                if (null == key || !(key is int))
                {
                    _logger.Error("Invalid key provided: {}", key);
                    return false;
                }


                int newKey = (int)key;
                if (newKey == syncKey)
                {
                    Interlocked.Increment(ref syncKey);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public byte[] GetSerializedByteArray()
        {
            return serializedBytes;
        }


        public void SetSerializedByteArray(byte[] serializedBytes)
        {
            this.serializedBytes = serializedBytes;
        }


        public Object ComputeAndSetNextState(Object state, Object syncKey,
                Object stateAlgorithm)
        {
            throw new MissingMethodException("computeAndSetNextState"
                    + "(Object state, Object syncKey,"
                    + "Object stateAlgorithm) not supported yet");
        }

        public Object ComputeNextState(Object state, Object syncKey,
                    Object stateAlgorithm)
        {
            throw new MissingMethodException("computeNextState"
                    + "(Object state, Object syncKey, Object stateAlgorithm)"
                    + " not supported yet");
        }


        public Object GetStateAlgorithm()
        {
            throw new MissingMethodException("getStateAlgorithm()"
                    + " not supported yet");
        }
    }
}
