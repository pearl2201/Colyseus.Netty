using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colyseus.Server.Transport;

namespace Colyseus.Server {

  public abstract class Room<TState, TMetadata> : IDisposable {
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

    public Room (IPresence presence) {
      this.presence = presence;
    }

    public void Dispose () {
      throw new NotImplementedException ();
    }

    public abstract Task<dynamic> OnCreate (dynamic options);

    public abstract Task<dynamic> OnJoin (Client client, dynamic options, dynamic auth);

    public abstract Task<dynamic> OnLeave (Client client, bool consented);

    public virtual Task<dynamic> OnAuth (Client client, dynamic options, dynamic httpIncommingMessage) {
      return Task.Run (() => { return (dynamic) true; });
    }

    public bool HasReachedMaxClients () {
      return false;
    }

    public void SetSeatReservationTime (double seconds) {

    }

    public bool HasReservedSeat (string sessionId) {
      return true;
    }

    public void SetSimulationInterval (Action<float> onTickCallback, float delay = DEFAULT_SIMULATION_INTERVAL) {

    }

    public void SetPatchRate (float milliseconds) {

    }

    public void SetState (TState newState) {

    }

    public async Task SetMetadata (TMetadata meta) {

    }

    public async Task setPrivate (bool priv = true) {

    }

    public async Task Lock () {

    }
    public async Task unlock () { }
    public void send (Client client, string type, dynamic message, ISendOptions? options) {

    }
    public void send (Client client, Schema message, ISendOptions? options) {

    }

    public void broadcast (type: string, message?: any, IBroadcastOptions? options) {

    }
    public void broadcast<T extends Schema> (T message, IBroadcastOptions? options) {

    }

    public onMessage<T> (string messageType, Action<dynamic[]> callback) {

    }

    public async Task disconnect () {

    }

    public async Task _onJoin (Client client, dynamic req) {

    }

    public void allowReconnection (Client previousClient, float seconds = float.MaxValue) {

    }

    protected void resetAutoDisposeTimeout (float timeoutInSeconds = 1) {

    }

    private broadcastMessageSchema<T> (T message, IBroadcastOptions options) where T:Schema{

    }

    private void broadcastMessageType (dynamic type, dynamic message, IBroadcastOptions options) {

    }

    private void sendFullState (Client client) {

    }

    private broadcastAfterPatch () {

    }

    private async Task _reserveSeat (
      string sessionId,
      bool joinOptions = true,
      float seconds this.seatReservationTime,
      bool allowReconnection = false,
    ) {

    }

    private void _disposeIfEmpty () {

    }

    private async Task _dispose () {

    }

    private void _onMessage (Client client, byte[] bytes) {

    }

    private void _forciblyCloseClient (Client client, int? closeCode) {

    }

    private async Task<dynamic> _onLeave (Client client: , int? code) {

    }

    private async Task _incrementClientCount () {

    }

    private async Task _decrementClientCount () {

    }
  }
}