using Colyseus.Server.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colyseus.Server
{

    public abstract class Room<TState, TMetadata> : IDisposable
    {
        private const float DEFAULT_PATCH_RATE = 1000f / 20; // 20fps (50ms)
        private const float DEFAULT_SIMULATION_INTERVAL = 1000f / 60; // 60fps (16.66ms)

        public const float DEFAULT_SEAT_RESERVATION_TIME = 15;

        //public const Type SimulationCallback = Action<float>;

        //public const Type RoomConstructor<T> = Func<Presence?, Room<T>> where T: class;


        public string roomId;
        public string roomName;

        public int maxClients = int.MaxValue;
        public float patchRate = DEFAULT_PATCH_RATE;
        public bool autoDispose = true;

        public TState state;
        public IPresence presence;

        public Client[] clients;

        public Room(IPresence presence)
        {
            this.presence = presence;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public abstract Task<dynamic> OnCreate(dynamic options);

        public abstract Task<dynamic> OnJoin(Client client, dynamic options, dynamic auth);

        public abstract Task<dynamic> OnLeave(Client client, bool consented);

        public virtual Task<dynamic> OnAuth(Client client, dynamic options, HttpRequest httpIncommingMessage)
        {
            return Task.Run(() => { return (dynamic)true; });
        }

        public bool HasReachedMaxClients()
        {
            return false;
        }

        public void SetSeatReservationTime(double seconds)
        {

        }

        public bool HasReservedSeat(string sessionId)
        {
            return true;
        }

        public void SetSimulationInterval(Action<float> onTickCallback, float delay = DEFAULT_SIMULATION_INTERVAL)
        {

        }

        public void SetPatchRate(float milliseconds)
        {

        }

        public void SetState(TState newState)
        {

        }

        public async Task SetMetadata(TMetadata meta)
        {

        }

        public async Task setPrivate(bool priv = true)
        {

        }

        public async Task Lock()
        {

        }
        public async unlock()
        {
        }
        public send(client: Client, type: string | number, message: any, options?: ISendOptions) : void;
  public send(client: Client, message: Schema, options?: ISendOptions) : void;
  public send(client: Client, messageOrType: any, messageOrOptions?: any | ISendOptions, options?: ISendOptions) : void {

            }

    public broadcast(type: string | number, message?: any, options?: IBroadcastOptions);
    public broadcast<T extends Schema>(message: T, options?: IBroadcastOptions);
  public broadcast(
    typeOrSchema: string | number | Schema,
    messageOrOptions?: any | IBroadcastOptions,
    options?: IBroadcastOptions,
  )
    {

    }

    public onMessage<T = any>(messageType: '*', callback: (client: Client, type: string | number, message: T) => void);
  public onMessage<T = any>(messageType: string | number, callback: (client: Client, message: T) => void);
  public onMessage<T = any>(messageType: '*' | string | number, callback: (...args: any[]) => void) {

        }

public async disconnect(): Promise<any> {

}


public async ['_onJoin'] (client: Client, req ?: http.IncomingMessage) {

}

public allowReconnection(previousClient: Client, seconds: number = Infinity): Deferred
{

}

protected resetAutoDisposeTimeout(timeoutInSeconds: number = 1) {

}

private broadcastMessageSchema<T extends Schema>(message: T, options: IBroadcastOptions = { }) {

}

private broadcastMessageType(type: string, message ?: any, options: IBroadcastOptions = { }) {

}

private sendFullState(client: Client): void
{

}

private broadcastAfterPatch()
{

}

private async _reserveSeat(
  sessionId: string,
    joinOptions: any = true,
    seconds: number = this.seatReservationTime,
    allowReconnection: boolean = false,
  ) {

}


private _disposeIfEmpty()
{

}


private async _dispose(): Promise<any> {

}


private _onMessage(client: Client, bytes: number[]) {

}

private _forciblyCloseClient(client: Client, closeCode: number) {

}

private async _onLeave(client: Client, code ?: number): Promise<any> {

}


private async _incrementClientCount()
{

}


private async _decrementClientCount()
{

}
}
}
