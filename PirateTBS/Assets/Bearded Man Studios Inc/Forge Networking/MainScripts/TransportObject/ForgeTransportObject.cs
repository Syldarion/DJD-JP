/*-----------------------------+------------------------------\
|                                                             |
|                        !!!NOTICE!!!                         |
|                                                             |
|  These libraries are under heavy development so they are    |
|  subject to make many changes as development continues.     |
|  For this reason, the libraries may not be well commented.  |
|  THANK YOU for supporting forge with all your feedback      |
|  suggestions, bug reports and comments!                     |
|                                                             |
|                               - The Forge Team              |
|                                 Bearded Man Studios, Inc.   |
|                                                             |
|  This source code, project files, and associated files are  |
|  copyrighted by Bearded Man Studios, Inc. (2012-2015) and   |
|  may not be redistributed without written permission.       |
|                                                             |
\------------------------------+-----------------------------*/



using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BeardedManStudios.Network
{
	public class ForgeTransportObject
	{
		public delegate void TransportFinished(ForgeTransportObject target);

		public event TransportFinished transportFinished
		{
			add
			{
				transportFinishedInvoker += value;
			}
			remove
			{
				transportFinishedInvoker -= value;
			}
		}
		TransportFinished transportFinishedInvoker;

#if NETFX_CORE
		IEnumerable<FieldInfo> fields;
#else
		FieldInfo[] fields;
#endif

		private static ulong currentId = 0;
		private ulong id = 0;
		private object serializerMutex = new Object();
		private BMSByte serializer = new BMSByte();

		public static Dictionary<ulong, ForgeTransportObject> transportObjects = new Dictionary<ulong, ForgeTransportObject>();

		public ForgeTransportObject()
		{
			id = currentId++;
			Initialize();
			transportObjects.Add(id, this);
		}

		public static ForgeTransportObject Locate(ulong identifier)
		{
			if (transportObjects.ContainsKey(identifier))
				return transportObjects[identifier];

			return null;
		}

		private void Initialize()
		{
			if (Networking.PrimarySocket == null)
				return;

#if NETFX_CORE
			fields = this.GetType().GetRuntimeFields();
#else
			fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
		}

		public void Send(NetworkReceivers receivers = NetworkReceivers.Others, bool reliable = true)
		{
			lock (serializerMutex)
			{
				serializer.Clear();
				ObjectMapper.MapBytes(serializer, id);

				foreach (FieldInfo field in fields)
					ObjectMapper.MapBytes(serializer, field.GetValue(this));

				Networking.WriteCustom(WriteCustomMapping.TRANSPORT_OBJECT, Networking.PrimarySocket, serializer, reliable, receivers);
			}
		}

		public void ReadFromNetwork(NetworkingStream stream)
		{
			Deserialize(stream);
		}

		private void Deserialize(NetworkingStream stream)
		{
			lock (serializerMutex)
			{
				foreach (FieldInfo field in fields)
					field.SetValue(this, ObjectMapper.Map(field.FieldType, stream));

				if (transportFinishedInvoker != null)
					transportFinishedInvoker(this);
			}
		}
	}
}