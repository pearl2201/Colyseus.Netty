using Coleseus.Shared.App;
using Coleseus.Shared.App.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Service.Impl
{

    /**
	 * The lookup service abstracts away the implementation detail on getting the
	 * game objects from the reference key provided by the client. This lookup is
	 * now done from a hashmap but can be done from database or any other manner.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
    public class SimpleLookupService : ILookupService
    {
        private Dictionary<string, GameRoom> refKeyGameRoomMap;

        public SimpleLookupService()
        {
            refKeyGameRoomMap = new Dictionary<string, GameRoom>();
        }

        public SimpleLookupService(Dictionary<string, GameRoom> refKeyGameRoomMap)
        {
           
            this.refKeyGameRoomMap = refKeyGameRoomMap;
        }


        public override IGame gameLookup(Object gameContextKey)
        {
            // TODO Auto-generated method stub
            return null;
        }


        public override GameRoom gameRoomLookup(Object gameContextKey)
        {
            Console.WriteLine(string.Join(",", this.refKeyGameRoomMap.Keys));
            Console.WriteLine("GameContextKey: " + (string)gameContextKey);
            return refKeyGameRoomMap[(string)gameContextKey];
        }


        public override IPlayer playerLookup(Credentials loginDetail)
        {
            return new DefaultPlayer();
        }

        public Dictionary<string, GameRoom> getRefKeyGameRoomMap()
        {
            return refKeyGameRoomMap;
        }

        public override void setGameRoomLookup(Dictionary<string, GameRoom> rooms)
        {
            refKeyGameRoomMap = rooms;
        }
    }
}
