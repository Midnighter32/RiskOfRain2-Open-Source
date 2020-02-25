using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000566 RID: 1382
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class NetworkMessageHandlerAttribute : SearchableAttribute
	{
		// Token: 0x060020FA RID: 8442 RVA: 0x0008E76C File Offset: 0x0008C96C
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CollectHandlers()
		{
			NetworkMessageHandlerAttribute.clientMessageHandlers.Clear();
			NetworkMessageHandlerAttribute.serverMessageHandlers.Clear();
			HashSet<short> hashSet = new HashSet<short>();
			foreach (SearchableAttribute searchableAttribute in SearchableAttribute.GetInstances<NetworkMessageHandlerAttribute>())
			{
				NetworkMessageHandlerAttribute networkMessageHandlerAttribute = (NetworkMessageHandlerAttribute)searchableAttribute;
				MethodInfo methodInfo = networkMessageHandlerAttribute.target as MethodInfo;
				if (!(methodInfo == null) && methodInfo.IsStatic)
				{
					networkMessageHandlerAttribute.messageHandler = (NetworkMessageDelegate)Delegate.CreateDelegate(typeof(NetworkMessageDelegate), methodInfo);
					if (networkMessageHandlerAttribute.messageHandler != null)
					{
						if (networkMessageHandlerAttribute.client)
						{
							NetworkMessageHandlerAttribute.clientMessageHandlers.Add(networkMessageHandlerAttribute);
							hashSet.Add(networkMessageHandlerAttribute.msgType);
						}
						if (networkMessageHandlerAttribute.server)
						{
							NetworkMessageHandlerAttribute.serverMessageHandlers.Add(networkMessageHandlerAttribute);
							hashSet.Add(networkMessageHandlerAttribute.msgType);
						}
					}
					if (networkMessageHandlerAttribute.messageHandler == null)
					{
						Debug.LogWarningFormat("Could not register message handler for {0}. The function signature is likely incorrect.", new object[]
						{
							methodInfo.Name
						});
					}
					if (!networkMessageHandlerAttribute.client && !networkMessageHandlerAttribute.server)
					{
						Debug.LogWarningFormat("Could not register message handler for {0}. It is marked as neither server nor client.", new object[]
						{
							methodInfo.Name
						});
					}
				}
			}
			for (short num = 48; num < 75; num += 1)
			{
				if (!hashSet.Contains(num))
				{
					Debug.LogWarningFormat("Network message MsgType.Highest + {0} is unregistered.", new object[]
					{
						(int)(num - 47)
					});
				}
			}
		}

		// Token: 0x060020FB RID: 8443 RVA: 0x0008E8E8 File Offset: 0x0008CAE8
		public static void RegisterServerMessages()
		{
			foreach (NetworkMessageHandlerAttribute networkMessageHandlerAttribute in NetworkMessageHandlerAttribute.serverMessageHandlers)
			{
				NetworkServer.RegisterHandler(networkMessageHandlerAttribute.msgType, networkMessageHandlerAttribute.messageHandler);
			}
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x0008E944 File Offset: 0x0008CB44
		public static void RegisterClientMessages(NetworkClient client)
		{
			foreach (NetworkMessageHandlerAttribute networkMessageHandlerAttribute in NetworkMessageHandlerAttribute.clientMessageHandlers)
			{
				client.RegisterHandler(networkMessageHandlerAttribute.msgType, networkMessageHandlerAttribute.messageHandler);
			}
		}

		// Token: 0x04001E03 RID: 7683
		public short msgType;

		// Token: 0x04001E04 RID: 7684
		public bool server;

		// Token: 0x04001E05 RID: 7685
		public bool client;

		// Token: 0x04001E06 RID: 7686
		private NetworkMessageDelegate messageHandler;

		// Token: 0x04001E07 RID: 7687
		private static List<NetworkMessageHandlerAttribute> clientMessageHandlers = new List<NetworkMessageHandlerAttribute>();

		// Token: 0x04001E08 RID: 7688
		private static List<NetworkMessageHandlerAttribute> serverMessageHandlers = new List<NetworkMessageHandlerAttribute>();
	}
}
