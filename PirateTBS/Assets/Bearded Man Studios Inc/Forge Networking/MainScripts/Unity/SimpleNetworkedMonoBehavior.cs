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



using UnityEngine;

#if NETFX_CORE
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

using System;
using System.Reflection;
using System.Collections.Generic;

namespace BeardedManStudios.Network
{
	/// <summary>
	/// This is the base class for all objects that are to be established on the network
	/// </summary>
	[AddComponentMenu("Forge Networking/Simple Networked MonoBehavior")]
	public class SimpleNetworkedMonoBehavior : MonoBehaviour, INetworkingSerialized
	{
		/// <summary>
		/// This is the main attribute class that is used to describe RPC methods
		/// </summary>
		protected sealed class BRPC : Attribute
		{
			public Type interceptorType;

			public BRPC() { }

			public BRPC(Type interceptorType)
			{
				this.interceptorType = interceptorType;
			}
		}

		/// <summary>
		/// Determine if the initial setup has been done or not
		/// </summary>
		private static bool initialSetup = false;

		/// <summary>
		/// A list of all of the current networked behaviors
		/// </summary>
		public static Dictionary<ulong, SimpleNetworkedMonoBehavior> networkedBehaviors = new Dictionary<ulong, SimpleNetworkedMonoBehavior>();

		/// <summary>
		/// A number that is used in assigning the unique id for a networked object
		/// </summary>
		public static ulong ObjectCounter { get; private set; }

		/// <summary>
		/// Determine if the object calling this boolean is the owner of this networked object
		/// </summary>
		public bool IsOwner { get; protected set; }

		/// <summary>
		/// The player id who currently owns this networked object
		/// </summary>
		public ulong OwnerId { get; protected set; }

		/// <summary>
		/// Whether this object is allowed to be added to the buffer
		/// </summary>
		public bool IsClearedForBuffer { get; protected set; }

		/// <summary>
		/// Used to determine if the server owns this object, the server id
		/// will always be 0 in Forge Networking
		/// </summary>
		public bool IsServerOwner { get { return OwnerId == 0; } }

		private Dictionary<int, KeyValuePair<MethodInfo, List<IBRPCIntercept>>> rpcs = null;

		/// <summary>
		/// Used to map BRPC methods to integers for bandwidth compression
		/// </summary>
		protected Dictionary<int, KeyValuePair<MethodInfo, List<IBRPCIntercept>>> RPCs
		{
			get
			{
				if (rpcs == null)
					Reflect();

				return rpcs;
			}
		}

		/// <summary>
		/// A list of RPC (remote methods) that are pending to be called for this object
		/// </summary>
		protected List<NetworkingStreamRPC> rpcStack = new List<NetworkingStreamRPC>();
		private object rpcStackMutex = new System.Object();
		private string rpcStackExceptionMethodName = string.Empty;

		/// <summary>
		/// The sender of the RPC (player who requested the RPC to be called)
		/// </summary>
		protected NetworkingPlayer CurrentRPCSender { get; set; }

		// TODO:  Optimization when an object is removed there needs to be a way to replace its spot
		/// <summary>
		/// The Network ID of this Simple Networked Monobehavior
		/// </summary>
		public ulong NetworkedId { get; private set; }

		/// <summary>
		/// If this object has been setup on the network
		/// </summary>
		public bool IsSetup { get; protected set; }

		/// <summary>
		/// The owning socket (Net Worker) for this Simple Networked Monobehavior
		/// </summary>
		public NetWorker OwningNetWorker { get; protected set; }

		/// <summary>
		/// This is used by the server in order to easily associate this object to its owning player
		/// </summary>
		public NetworkingPlayer OwningPlayer { get; protected set; }

		/// <summary>
		/// The stream that is re-used for the RPCs that are being sent from this object
		/// </summary>
		private NetworkingStream rpcNetworkingStream = new NetworkingStream();

		/// <summary>
		/// The main cached buffer the RPC network stream
		/// </summary>
		private BMSByte getStreamBuffer = new BMSByte();

		/// <summary>
		/// Used to lock the initialization thread
		/// </summary>
		private static object initializeMutex = new object();

		/// <summary>
		/// If this is marked as true then this object will not be cleaned up by the network on level load
		/// </summary>
		public bool dontDestroyOnLoad = false;

		/// <summary>
		/// If this is true, then a network destroy will be called on disconnect
		/// </summary>
		public bool destroyOnDisconnect = false;

		/// <summary>
		/// Get a generated Unique ID for the next simple networked mono behavior or its derivitive
		/// </summary>
		/// <returns>A Unique unsigned long ID</returns>
		public static ulong GenerateUniqueId()
		{
			if (Networking.IsBareMetal && ObjectCounter == 0)
				ObjectCounter++;

			return ++ObjectCounter;
		}

		/// <summary>
		/// Locate a Simple Networked Monobehavior given a ID
		/// </summary>
		/// <param name="id">ID of the Simple Networked Monobehavior</param>
		/// <returns>The Simple Networked Monobehavior found or <c>null</c> if not found</returns>
		public static SimpleNetworkedMonoBehavior Locate(ulong id)
		{
			if (networkedBehaviors.ContainsKey(id))
				return networkedBehaviors[id];

			return null;
		}

		/// <summary>
		/// Destroy a Simple Networked Monobehavior or any of its derivitives with the given Network ID
		/// </summary>
		/// <param name="networkId">Network ID to be destroyed</param>
		/// <returns><c>True</c> if the network behavoir was destroy, otherwise <c>False</c></returns>
		public static bool NetworkDestroy(ulong networkId)
		{
			// Make sure the object exists on the network before calling destroy
			SimpleNetworkedMonoBehavior behavior = Locate(networkId);

			if (behavior == null)
				return false;

			// Destroy the object from the scene and remove it from the lookup
			GameObject.Destroy(behavior.gameObject);
			networkedBehaviors.Remove(networkId);

			if (Networking.PrimarySocket.IsServer)
				Networking.PrimarySocket.ClearBufferedInstantiateFromID(networkId);

			return true;
		}

		public MethodInfo GetRPC(int id)
		{
			if (!RPCs.ContainsKey(id))
				return null;

			return RPCs[id].Key;
		}

		private static Dictionary<ulong, List<NetworkingStreamRPC>> missingIdBuffer = new Dictionary<ulong, List<NetworkingStreamRPC>>();
		private static object missingIdMutex = new object();
		public static void QueueRPCForInstantiate(ulong id, NetworkingStream stream)
		{
			if (id < ObjectCounter)
				return;

			lock (missingIdMutex)
			{
				if (!missingIdBuffer.ContainsKey(id))
					missingIdBuffer.Add(id, new List<NetworkingStreamRPC>());

				missingIdBuffer[id].Add(new NetworkingStreamRPC(stream, true));
			}
		}

		protected virtual void Reflect()
		{
			IsClearedForBuffer = true;
			rpcs = new Dictionary<int, KeyValuePair<MethodInfo, List<IBRPCIntercept>>>();
#if NETFX_CORE
			IEnumerable<MethodInfo> methods = this.GetType().GetRuntimeMethods();
#else
			MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
			foreach (MethodInfo method in methods)
			{
				BRPC[] attributes = null;
#if NETFX_CORE
				attributes = method.GetCustomAttributes<BRPC>().ToList().ToArray();
				//if (method.GetCustomAttribute<BRPC>() != null)
#else
				attributes = method.GetCustomAttributes(typeof(BRPC), true) as BRPC[];
#endif
				if (attributes != null && attributes.Length != 0)
				{
					RPCs.Add(RPCs.Count, new KeyValuePair<MethodInfo, List<IBRPCIntercept>>(method, new List<IBRPCIntercept>()));

					foreach (BRPC brpc in attributes)
					{
						if (brpc.interceptorType == null)
							continue;

						object interceptor = Activator.CreateInstance(brpc.interceptorType);

						if (interceptor == null || !(interceptor is IBRPCIntercept))
							throw new NetworkException("The type " + brpc.interceptorType.ToString() + " does not implement IBRPCIntercept");

						RPCs[RPCs.Count - 1].Value.Add((IBRPCIntercept)interceptor);
					}
				}
			}
		}

		public void Cleanup()
		{
			Unity.MainThreadManager.unityUpdate -= UnityUpdate;
			Unity.MainThreadManager.unityFixedUpdate -= UnityFixedUpdate;
		}

		public static void ResetForScene(List<SimpleNetworkedMonoBehavior> skip)
		{
			initialSetup = false;

			foreach (SimpleNetworkedMonoBehavior behavior in networkedBehaviors.Values)
			{
				if (!skip.Contains(behavior))
				{
					if (behavior.dontDestroyOnLoad)
						skip.Add(behavior);
					else
						behavior.Disconnect();
				}
			}

			networkedBehaviors.Clear();

			for (int i = skip.Count - 1; i >= 0; --i)
				if (skip[i] == null)
					skip.RemoveAt(i);

			foreach (SimpleNetworkedMonoBehavior behavior in skip)
				networkedBehaviors.Add(behavior.NetworkedId, behavior);
		}

		public static void ResetAll()
		{
			initialSetup = false;

			foreach (SimpleNetworkedMonoBehavior behavior in networkedBehaviors.Values)
				behavior.Disconnect();

			networkedBehaviors.Clear();

			ObjectCounter = 0;
		}

		/// <summary>
		/// Resets the buffer to be clear again so that it can start buffering
		/// </summary>
		public void ResetBufferClear()
		{
			IsClearedForBuffer = true;
		}

		/// <summary>
		/// An initial setup to make sure that a networking manager exists before running any core logic
		/// </summary>
		public static void Initialize()
		{
			lock (initializeMutex)
			{
				if (!initialSetup)
				{
					initialSetup = true;
					if (NetworkingManager.Instance == null)
						Instantiate(Resources.Load<GameObject>("BeardedManStudios/Networking Manager"));

					if (!NetworkingManager.Instance.Populate())
						Networking.connected += DelayedInitialize;
				}
			}
		}

		private static void DelayedInitialize(NetWorker socket)
		{
			initialSetup = false;
			Initialize();
			Networking.connected -= DelayedInitialize;
		}

		private void ThrowNetworkerException()
		{
#if UNITY_EDITOR
			Debug.Log("Try using the Forge Quick Start Menu and setting the \"Scene Name\" on the \"Canvas\" to the scene you are loading. Then running from that scene.");
#endif

			throw new NetworkException("The NetWorker doesn't exist. Is it possible that a connection hasn't been made?");
		}

		/// <summary>
		/// A start method that is called after the object has been setup on the network
		/// </summary>
		protected virtual void NetworkStart()
		{
			IsSetup = true;

			lock (missingIdMutex)
			{
				if (missingIdBuffer.ContainsKey(NetworkedId))
				{
					foreach (NetworkingStreamRPC rpcStream in missingIdBuffer[NetworkedId])
					{
						rpcStream.AssignBehavior(this);
						rpcStream.NetworkedBehavior.InvokeRPC(rpcStream);
					}

					missingIdBuffer.Remove(NetworkedId);
				}
			}

			Unity.MainThreadManager.unityUpdate += UnityUpdate;
			Unity.MainThreadManager.unityFixedUpdate += UnityFixedUpdate;

			// Just make sure that Unity doesn't destroy this objeect on load
			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}

		/// <summary>
		/// Setup the Simple Networked Monobehavior stack with a NetWorker
		/// </summary>
		/// <param name="owningSocket">The NetWorker to be setup with</param>
		public static void SetupObjects(SimpleNetworkedMonoBehavior[] behaviors, NetWorker owningSocket)
		{
			if (ObjectCounter == 0)
				GenerateUniqueId();

			NetworkingManager.Instance.Setup(owningSocket, owningSocket.IsServer, 0, 0);

			// TODO:  Got through all objects in NetworkingManager stack and set them up
			foreach (SimpleNetworkedMonoBehavior behavior in behaviors)
				if (!(behavior is NetworkingManager) && behavior != null)
					behavior.Setup(owningSocket, owningSocket.IsServer, GenerateUniqueId(), 0);
		}

		/// <summary>
		/// Setup the Simple Networked Monobehavior stack with a NetWorker, owner, network id, and owner id
		/// </summary>
		/// <param name="owningSocket">The NetWorker to be setup with</param>
		/// <param name="isOwner">If this object is the owner</param>
		/// <param name="networkId">The NetworkID for this Simple Networked Monobehavior</param>
		/// <param name="ownerId">The OwnerID for this Simple Networked Monobehavior</param>
#if NETFX_CORE
		public virtual async void Setup(NetWorker owningSocket, bool isOwner, ulong networkId, ulong ownerId)
#else
		public virtual void Setup(NetWorker owningSocket, bool isOwner, ulong networkId, ulong ownerId)
#endif
		{
			Reflect();

			if (owningSocket == null)
				ThrowNetworkerException();

			int count = 0;
			while (NetworkingManager.Instance != this && !NetworkingManager.Instance.IsSetup)
			{
#if NETFX_CORE
				await Task.Delay(TimeSpan.FromMilliseconds(25));
#else
				System.Threading.Thread.Sleep(25);
#endif

				if (++count == 20)
					throw new NetworkException("The NetworkingManager could not be found");
			}

			OwningNetWorker = owningSocket;
			IsOwner = isOwner;
			OwnerId = ownerId;
			NetworkedId = networkId;

			if (NetworkedId != 0 || !networkedBehaviors.ContainsKey(0))
				networkedBehaviors.Add(NetworkedId, this);

			if (OwningNetWorker.Me != null && ownerId == OwningNetWorker.Me.NetworkId)
				OwningPlayer = OwningNetWorker.Me;
			else if (OwningNetWorker.IsServer)
			{
				foreach (NetworkingPlayer player in OwningNetWorker.Players)
				{
					if (ownerId == player.NetworkId)
					{
						OwningPlayer = player;
						break;
					}
				}
			}

			if (OwningPlayer != null && OwningNetWorker.IsServer && this is NetworkedMonoBehavior)
				OwningPlayer.SetMyBehavior((NetworkedMonoBehavior)this);

			NetworkStart();
		}

		protected virtual void UnityUpdate()
		{
			if (!NetworkingManager.IsOnline)
			{
				DoOwnerUpdate();
				return;
			}

			if (this == null)
			{
				Unity.MainThreadManager.unityUpdate -= UnityUpdate;
				return;
			}

			// If there are any pending RPC calls, then do them now on the main thread
			if (rpcStack.Count != 0)
			{
				lock (rpcStackMutex)
				{
					NetworkingStreamRPC stream = rpcStack[0];
					rpcStackExceptionMethodName = stream.MethodName;

					rpcStack.RemoveAt(0);

					foreach (KeyValuePair<int, KeyValuePair<MethodInfo, List<IBRPCIntercept>>> rpc in RPCs)
					{
						if (stream == null)
						{
							DoOwnerUpdate();
							return;
						}

						if (rpc.Value.Key.Name == stream.MethodName)
						{
							CurrentRPCSender = stream.Sender;
							rpc.Value.Key.Invoke(this, stream.Arguments);
							CurrentRPCSender = null;
							DoOwnerUpdate();
							return;
						}
					}
				}

				throw new NetworkException(13, "Invoked network method " + rpcStackExceptionMethodName + " not found or not marked with [BRPC]");
			}

			DoOwnerUpdate();
		}

		private void DoOwnerUpdate()
		{
			if (IsOwner)
				OwnerUpdate();
			else
				NonOwnerUpdate();
		}

		protected virtual void UnityFixedUpdate()
		{
			if (this == null)
			{
				Unity.MainThreadManager.unityFixedUpdate -= UnityFixedUpdate;
				return;
			}

			if (IsOwner)
				OwnerFixedUpdate();
			else
				NonOwnerFixedUpdate();
		}

		protected virtual void OwnerUpdate() { }

		protected virtual void NonOwnerUpdate() { }

		protected virtual void OwnerFixedUpdate() { }

		protected virtual void NonOwnerFixedUpdate() { }

		int tmpRPCMapId = 0;
		/// <summary>
		/// To Invoke an RPC on a given Networking Stream RPC
		/// </summary>
		/// <param name="stream">Networking Stream RPC to read from</param>
		public bool InvokeRPC(NetworkingStreamRPC stream)
		{
			lock (rpcStackMutex)
			{
				tmpRPCMapId = ObjectMapper.Map<int>(stream);
				if (!RPCs.ContainsKey(tmpRPCMapId))
					return true;

				stream.SetName(RPCs[tmpRPCMapId].Key.Name);

				List<object> args = new List<object>();

				MethodInfo invoke = null;
				List<IBRPCIntercept> attributes = null;
				foreach (KeyValuePair<int, KeyValuePair<MethodInfo, List<IBRPCIntercept>>> m in RPCs)
				{
					if (m.Value.Key.Name == stream.MethodName)
					{
						invoke = m.Value.Key;
						attributes = m.Value.Value;
						break;
					}
				}

				int start = stream.Bytes.StartIndex(stream.ByteReadIndex);
				ParameterInfo[] pars = invoke.GetParameters();
				foreach (ParameterInfo p in pars)
				{
					if (p.ParameterType == typeof(MessageInfo))
						args.Add(new MessageInfo(stream.RealSenderId, stream.FrameIndex));
					else
						args.Add(ObjectMapper.Map(p.ParameterType, stream));
				}

				stream.SetArguments(args.ToArray());

				if (ReferenceEquals(this, NetworkingManager.Instance))
				{
					if (OwningNetWorker.IsServer && stream.MethodName == NetworkingStreamRPC.INSTANTIATE_METHOD_NAME)
						stream.Arguments[1] = stream.SetupInstantiateId(stream, start);

					if (stream.MethodName == NetworkingStreamRPC.DESTROY_METHOD_NAME)
					{
						if (Networking.PrimarySocket.ClearBufferedInstantiateFromID((ulong)args[0]))
						{
							// Set flag if method removed instantiate
							IsClearedForBuffer = !stream.BufferedRPC;
						}
					}
				}

				foreach (IBRPCIntercept interceptor in attributes)
				{
					if (!interceptor.ValidateRPC(stream))
						return false;
				}

				rpcStack.Add(stream);
				return true;
			}
		}

		/// <summary>
		/// Creates a network stream for the method with the specified string name and returns the method info
		/// </summary>
		/// <param name="methodName">The name of the method to call from this class</param>
		/// <param name="receivers">The players on the network that will be receiving RPC</param>
		/// <param name="arguments">The list of arguments that will be sent for the RPC</param>
		/// <returns></returns>
		private int GetStreamRPC(string methodName, NetworkReceivers receivers, params object[] arguments)
		{
			foreach (KeyValuePair<int, KeyValuePair<MethodInfo, List<IBRPCIntercept>>> rpc in RPCs)
			{
				if (rpc.Value.Key.Name == methodName)
				{
					if (!NetworkingManager.IsOnline)
						return rpc.Key;

					getStreamBuffer.Clear();
					ObjectMapper.MapBytes(getStreamBuffer, rpc.Key);

#if UNITY_EDITOR
					if (arguments.Length != rpc.Value.Key.GetParameters().Length)
						throw new NetworkException("The number of arguments [" + arguments.Length + "] provided for the " + methodName + " RPC call do not match the method signature argument count [" + rpc.Value.Key.GetParameters().Length + "]");
#endif

					if (arguments != null && arguments.Length > 0)
						ObjectMapper.MapBytes(getStreamBuffer, arguments);

					bool buffered = receivers == NetworkReceivers.AllBuffered || receivers == NetworkReceivers.OthersBuffered;

					rpcNetworkingStream.SetProtocolType(OwningNetWorker is CrossPlatformUDP ? Networking.ProtocolType.UDP : Networking.ProtocolType.TCP);
					rpcNetworkingStream.Prepare(OwningNetWorker, NetworkingStream.IdentifierType.RPC, this, getStreamBuffer, receivers, buffered);

					return rpc.Key;
				}
			}

			throw new NetworkException(14, "No method marked with [BRPC] was found by the name " + methodName);
		}

		/// <summary>
		/// Used for the server to call an RPC method on a NetWorker(Socket) on a particular player
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="socket">The NetWorker(Socket) being used</param>
		/// <param name="player">The NetworkingPlayer who will execute this RPC</param>
		/// <param name="arguments">The RPC function parameters to be passed in</param>
		public void AuthoritativeRPC(string methodName, NetWorker socket, NetworkingPlayer player, bool runOnServer, params object[] arguments)
		{
			int rpcId = GetStreamRPC(methodName, NetworkReceivers.All, arguments);

			if (socket is CrossPlatformUDP)
				((CrossPlatformUDP)socket).Write("BMS_INTERNAL_Rpc_" + methodName, player, rpcNetworkingStream, true);
			else
				socket.Write(player, rpcNetworkingStream);

			if (socket.IsServer && runOnServer)
			{
				Unity.MainThreadManager.Run(() =>
				{
					bool faildValidate = false;

					foreach (IBRPCIntercept intercept in RPCs[rpcId].Value)
					{
						if (!intercept.ValidateRPC(RPCs[rpcId].Key))
						{
							faildValidate = true;
							break;
						}
					}

					if (!faildValidate)
						RPCs[rpcId].Key.Invoke(this, arguments);
				});
			}
		}

		/// <summary>
		/// Call an RPC method on a NetWorker(Socket) with receivers and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="socket">The NetWorker(Socket) being used</param>
		/// <param name="receivers">Who shall receive the RPC</param>
		/// <param name="arguments">The RPC function parameters to be passed in</param>
		public void RPC(string methodName, NetWorker socket, NetworkReceivers receivers, params object[] arguments)
		{
			int rpcId = GetStreamRPC(methodName, receivers, arguments);

			if (NetworkingManager.IsOnline)
			{
				if (socket is CrossPlatformUDP)
					((CrossPlatformUDP)socket).Write("BMS_INTERNAL_Rpc_" + methodName, rpcNetworkingStream, true);
				else
					socket.Write(rpcNetworkingStream);
			}

			if ((!NetworkingManager.IsOnline || socket.IsServer) && receivers != NetworkReceivers.Others && receivers != NetworkReceivers.OthersBuffered && receivers != NetworkReceivers.OthersProximity)
			{
				Unity.MainThreadManager.Run(() =>
				{
					bool faildValidate = false;

					foreach (IBRPCIntercept intercept in RPCs[rpcId].Value)
					{
						if (!intercept.ValidateRPC(RPCs[rpcId].Key))
						{
							faildValidate = true;
							break;
						}
					}

					if (faildValidate)
						return;

					List<object> args = new List<object>();
					int argCount = 0;
					foreach (ParameterInfo info in RPCs[rpcId].Key.GetParameters())
					{
						if (info.ParameterType == typeof(MessageInfo))
							args.Add(new MessageInfo(OwningNetWorker.Me.NetworkId, NetworkingManager.Instance.CurrentFrame));
						else
							args.Add(arguments[argCount++]);
					}

					RPCs[rpcId].Key.Invoke(this, args.ToArray());
				});
			}
		}

		/// <summary>
		/// Call an Unreliable RPC method on a NetWorker(Socket) with receivers and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="socket">The NetWorker(Socket) being used</param>
		/// <param name="receivers">Who shall receive the RPC</param>
		/// <param name="arguments">The RPC function parameters to be passed in</param>
		public void URPC(string methodName, NetWorker socket, NetworkReceivers receivers, params object[] arguments)
		{
			int rpcId = GetStreamRPC(methodName, receivers, arguments);

			if (socket is CrossPlatformUDP)
				((CrossPlatformUDP)socket).Write("BMS_INTERNAL_Rpc_" + methodName, rpcNetworkingStream, false);
			else
				socket.Write(rpcNetworkingStream);

			if (socket.IsServer && receivers != NetworkReceivers.Others && receivers != NetworkReceivers.OthersBuffered && receivers != NetworkReceivers.OthersProximity)
			{
				Unity.MainThreadManager.Run(() =>
				{
					bool faildValidate = false;

					foreach (IBRPCIntercept intercept in RPCs[rpcId].Value)
					{
						if (!intercept.ValidateRPC(RPCs[rpcId].Key))
						{
							faildValidate = true;
							break;
						}
					}

					if (faildValidate)
						return;

					List<object> args = new List<object>();
					int argCount = 0;
					foreach (ParameterInfo info in RPCs[rpcId].Key.GetParameters())
					{
						if (info.ParameterType == typeof(MessageInfo))
							args.Add(new MessageInfo(OwningNetWorker.Me.NetworkId, NetworkingManager.Instance.CurrentFrame));
						else
							args.Add(arguments[argCount++]);
					}

					RPCs[rpcId].Key.Invoke(this, args.ToArray());
				});
			}
		}

		/// <summary>
		/// Call an RPC method with arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="arguments">Extra parameters passed in</param>
		public void RPC(string methodName, params object[] arguments)
		{
			RPC(methodName, OwningNetWorker, NetworkReceivers.All, arguments);
		}

		/// <summary>
		/// Call an RPC method with a receiver and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="rpcMode">Who shall receive the RPC</param>
		/// <param name="arguments">Extra parameters passed in</param>
		public void RPC(string methodName, NetworkReceivers rpcMode, params object[] arguments)
		{
			RPC(methodName, OwningNetWorker, rpcMode, arguments);
		}

		/// <summary>
		/// Call an Unreliable RPC method with a receiver and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="rpcMode">Who shall receive the RPC</param>
		/// <param name="arguments">Extra parameters passed in</param>
		public void URPC(string methodName, NetworkReceivers rpcMode, params object[] arguments)
		{
			URPC(methodName, OwningNetWorker, rpcMode, arguments);
		}

		/// <summary>
		/// Call an RPC method with a NetWorker(Socket) and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="socket">The NetWorker(Socket) being used</param>
		/// <param name="arguments">Extra parameters passed in</param>
		public void RPC(string methodName, NetWorker socket, params object[] arguments)
		{
			RPC(methodName, socket, NetworkReceivers.All, arguments);
		}

		/// <summary>
		/// Serialize the Simple Networked Monobehavior
		/// </summary>
		/// <returns></returns>
		public virtual BMSByte Serialized() { return null; }

		/// <summary>
		/// Deserialize the Networking Stream
		/// </summary>
		/// <param name="stream">Stream to be deserialized</param>
		public virtual void Deserialize(NetworkingStream stream) { }

		/// <summary>
		/// Used to do final cleanup when disconnecting. This gets called currently only on application quit
		/// </summary>
		public virtual void Disconnect()
		{
			Cleanup();

			if (destroyOnDisconnect)
				Networking.Destroy(this);
		}

		protected virtual void OnDestroy()
		{
			Cleanup();
		}

		protected virtual void OnApplicationQuit() { Disconnect(); }

		protected virtual void NetworkDisconnect()
		{
			Disconnect();
			initialSetup = false;
			networkedBehaviors.Clear();
			ObjectCounter = 0;
			missingIdBuffer.Clear();
		}
	}
}