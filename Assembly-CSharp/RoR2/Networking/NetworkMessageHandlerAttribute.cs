using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000595 RID: 1429
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class NetworkMessageHandlerAttribute : Attribute
	{
		// Token: 0x06002030 RID: 8240 RVA: 0x00097190 File Offset: 0x00095390
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CollectHandlers()
		{
			NetworkMessageHandlerAttribute.clientMessageHandlers.Clear();
			NetworkMessageHandlerAttribute.serverMessageHandlers.Clear();
			HashSet<short> hashSet = new HashSet<short>();
			Type[] types = typeof(NetworkMessageHandlerAttribute).Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(false);
					for (int k = 0; k < customAttributes.Length; k++)
					{
						NetworkMessageHandlerAttribute networkMessageHandlerAttribute = ((Attribute)customAttributes[k]) as NetworkMessageHandlerAttribute;
						if (networkMessageHandlerAttribute != null)
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
				}
			}
			for (short num = 48; num < 71; num += 1)
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

		// Token: 0x06002031 RID: 8241 RVA: 0x0009733C File Offset: 0x0009553C
		public static void RegisterServerMessages()
		{
			foreach (NetworkMessageHandlerAttribute networkMessageHandlerAttribute in NetworkMessageHandlerAttribute.serverMessageHandlers)
			{
				NetworkServer.RegisterHandler(networkMessageHandlerAttribute.msgType, networkMessageHandlerAttribute.messageHandler);
			}
		}

		// Token: 0x06002032 RID: 8242 RVA: 0x00097398 File Offset: 0x00095598
		public static void RegisterClientMessages(NetworkClient client)
		{
			foreach (NetworkMessageHandlerAttribute networkMessageHandlerAttribute in NetworkMessageHandlerAttribute.clientMessageHandlers)
			{
				client.RegisterHandler(networkMessageHandlerAttribute.msgType, networkMessageHandlerAttribute.messageHandler);
			}
		}

		// Token: 0x0400224C RID: 8780
		public short msgType;

		// Token: 0x0400224D RID: 8781
		public bool server;

		// Token: 0x0400224E RID: 8782
		public bool client;

		// Token: 0x0400224F RID: 8783
		private NetworkMessageDelegate messageHandler;

		// Token: 0x04002250 RID: 8784
		private static List<NetworkMessageHandlerAttribute> clientMessageHandlers = new List<NetworkMessageHandlerAttribute>();

		// Token: 0x04002251 RID: 8785
		private static List<NetworkMessageHandlerAttribute> serverMessageHandlers = new List<NetworkMessageHandlerAttribute>();
	}
}
