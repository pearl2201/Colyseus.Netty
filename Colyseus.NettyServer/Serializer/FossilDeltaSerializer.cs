using System.IO;
using System.Linq;
using System.Text;
using GameDevWare.Serialization;
using Serilog;

namespace Colyseus
{
	public class FossilDeltaSerializer : ISerializer<IndexedDictionary<string, object>>
	{
		public StateContainer State = new StateContainer(new IndexedDictionary<string, object>());
		protected byte[] previousState = null;

		public void SetState(byte[] rawEncodedState, int offset)
		{
			//Debug.Log("FULL STATE");
			//PrintByteArray(rawEncodedState);
			previousState = rawEncodedState.Skip(offset).ToArray();
			State.Set(MsgPack.Deserialize<IndexedDictionary<string, object>>(new MemoryStream(previousState)));
		}

		public IndexedDictionary<string, object> GetState()
		{
			return State.state;
		}

		public void Patch(byte[] bytes, int offset)
		{
			//Debug.Log("PATCH STATE");
			//PrintByteArray(bytes);
			previousState = Fossil.Delta.Apply(previousState, bytes.Skip(offset).ToArray());
			var newState = MsgPack.Deserialize<IndexedDictionary<string, object>>(new MemoryStream(previousState));
			State.Set(newState);
		}

		public void Teardown()
		{
			State.RemoveAllListeners();
		}

		public void Handshake(byte[] bytes, int offset)
		{
			Log.Information("Handshake FossilDeltaSerializer!");
		}

		//public void PrintByteArray(byte[] bytes)
		//{
		//	var sb = new StringBuilder("[");
		//	foreach (var b in bytes)
		//	{
		//		sb.Append(b + ", ");
		//	}
		//	sb.Append("]");
		//	Debug.Log(sb.ToString());
		//}

	}
}
