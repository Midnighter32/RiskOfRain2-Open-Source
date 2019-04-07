using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using RoR2.ConVar;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000212 RID: 530
	public static class Chat
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000A51 RID: 2641 RVA: 0x00033B14 File Offset: 0x00031D14
		public static ReadOnlyCollection<string> readOnlyLog
		{
			get
			{
				return Chat._readOnlyLog;
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000A52 RID: 2642 RVA: 0x00033B1C File Offset: 0x00031D1C
		// (remove) Token: 0x06000A53 RID: 2643 RVA: 0x00033B50 File Offset: 0x00031D50
		public static event Action onChatChanged;

		// Token: 0x06000A54 RID: 2644 RVA: 0x00033B84 File Offset: 0x00031D84
		public static void AddMessage(string message)
		{
			int num = Mathf.Max(Chat.cvChatMaxMessages.value, 1);
			while (Chat.log.Count > num)
			{
				Chat.log.RemoveAt(0);
			}
			Chat.log.Add(message);
			if (Chat.onChatChanged != null)
			{
				Chat.onChatChanged();
			}
			Debug.Log(message);
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x00033BDE File Offset: 0x00031DDE
		public static void Clear()
		{
			Chat.log.Clear();
			Action action = Chat.onChatChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00033BF9 File Offset: 0x00031DF9
		public static void SendBroadcastChat(Chat.ChatMessageBase message)
		{
			Chat.SendBroadcastChat(message, QosChannelIndex.chat.intVal);
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x00033C0C File Offset: 0x00031E0C
		public static void SendBroadcastChat(Chat.ChatMessageBase message, int channelIndex)
		{
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(59);
			networkWriter.Write(message.GetTypeIndex());
			networkWriter.Write(message);
			networkWriter.FinishMessage();
			foreach (NetworkConnection networkConnection in NetworkServer.connections)
			{
				if (networkConnection != null)
				{
					networkConnection.SendWriter(networkWriter, channelIndex);
				}
			}
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x00033C88 File Offset: 0x00031E88
		public static void SendPlayerConnectedMessage(NetworkUser user)
		{
			Chat.SendBroadcastChat(new Chat.PlayerChatMessage
			{
				networkPlayerName = user.GetNetworkPlayerName(),
				baseToken = "PLAYER_CONNECTED"
			});
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x00033CAB File Offset: 0x00031EAB
		public static void SendPlayerDisconnectedMessage(NetworkUser user)
		{
			Chat.SendBroadcastChat(new Chat.PlayerChatMessage
			{
				networkPlayerName = user.GetNetworkPlayerName(),
				baseToken = "PLAYER_DISCONNECTED"
			});
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x00033CCE File Offset: 0x00031ECE
		public static void AddPickupMessage(CharacterBody body, string pickupToken, Color32 pickupColor, uint pickupQuantity)
		{
			Chat.AddMessage(new Chat.PlayerPickupChatMessage
			{
				subjectCharacterBody = body,
				baseToken = "PLAYER_PICKUP",
				pickupToken = pickupToken,
				pickupColor = pickupColor,
				pickupQuantity = pickupQuantity
			});
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x00033D04 File Offset: 0x00031F04
		[NetworkMessageHandler(msgType = 59, client = true)]
		private static void HandleBroadcastChat(NetworkMessage netMsg)
		{
			Chat.ChatMessageBase chatMessageBase = Chat.ChatMessageBase.Instantiate(netMsg.reader.ReadByte());
			if (chatMessageBase != null)
			{
				chatMessageBase.Deserialize(netMsg.reader);
				Chat.AddMessage(chatMessageBase);
			}
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x00033D38 File Offset: 0x00031F38
		private static void AddMessage(Chat.ChatMessageBase message)
		{
			string text = message.ConstructChatString();
			if (text != null)
			{
				Chat.AddMessage(text);
				message.OnProcessed();
			}
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x00033D5B File Offset: 0x00031F5B
		[ConCommand(commandName = "say", flags = ConVarFlags.ExecuteOnServer, helpText = "Sends a chat message.")]
		private static void CCSay(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			if (args.sender)
			{
				Chat.SendBroadcastChat(new Chat.UserChatMessage
				{
					sender = args.sender.gameObject,
					text = args[0]
				});
			}
		}

		// Token: 0x04000DCB RID: 3531
		private static List<string> log = new List<string>();

		// Token: 0x04000DCC RID: 3532
		private static ReadOnlyCollection<string> _readOnlyLog = Chat.log.AsReadOnly();

		// Token: 0x04000DCE RID: 3534
		private static IntConVar cvChatMaxMessages = new IntConVar("chat_max_messages", ConVarFlags.None, "30", "Maximum number of chat messages to store.");

		// Token: 0x02000213 RID: 531
		public abstract class ChatMessageBase : MessageBase
		{
			// Token: 0x06000A5F RID: 2655 RVA: 0x00033DD0 File Offset: 0x00031FD0
			static ChatMessageBase()
			{
				Chat.ChatMessageBase.BuildMessageTypeNetMap();
			}

			// Token: 0x06000A60 RID: 2656
			public abstract string ConstructChatString();

			// Token: 0x06000A61 RID: 2657 RVA: 0x00004507 File Offset: 0x00002707
			public virtual void OnProcessed()
			{
			}

			// Token: 0x06000A62 RID: 2658 RVA: 0x00033DEC File Offset: 0x00031FEC
			private static void BuildMessageTypeNetMap()
			{
				foreach (Type type in typeof(Chat.ChatMessageBase).Assembly.GetTypes())
				{
					if (type.IsSubclassOf(typeof(Chat.ChatMessageBase)))
					{
						Chat.ChatMessageBase.chatMessageTypeToIndex.Add(type, (byte)Chat.ChatMessageBase.chatMessageIndexToType.Count);
						Chat.ChatMessageBase.chatMessageIndexToType.Add(type);
					}
				}
			}

			// Token: 0x06000A63 RID: 2659 RVA: 0x00033E54 File Offset: 0x00032054
			protected string GetObjectName(GameObject namedObject)
			{
				string result = "???";
				if (namedObject)
				{
					result = namedObject.name;
					NetworkUser networkUser = namedObject.GetComponent<NetworkUser>();
					if (!networkUser)
					{
						networkUser = Util.LookUpBodyNetworkUser(namedObject);
					}
					if (networkUser)
					{
						result = Util.EscapeRichTextForTextMeshPro(networkUser.userName);
					}
				}
				return result;
			}

			// Token: 0x06000A64 RID: 2660 RVA: 0x00033EA1 File Offset: 0x000320A1
			public byte GetTypeIndex()
			{
				return Chat.ChatMessageBase.chatMessageTypeToIndex[base.GetType()];
			}

			// Token: 0x06000A65 RID: 2661 RVA: 0x00033EB4 File Offset: 0x000320B4
			public static Chat.ChatMessageBase Instantiate(byte typeIndex)
			{
				Type type = Chat.ChatMessageBase.chatMessageIndexToType[(int)typeIndex];
				Debug.LogFormat("Received chat message typeIndex={0} type={1}", new object[]
				{
					typeIndex,
					(type != null) ? type.Name : null
				});
				if (type != null)
				{
					return (Chat.ChatMessageBase)Activator.CreateInstance(type);
				}
				return null;
			}

			// Token: 0x06000A67 RID: 2663 RVA: 0x00004507 File Offset: 0x00002707
			public override void Serialize(NetworkWriter writer)
			{
			}

			// Token: 0x06000A68 RID: 2664 RVA: 0x00004507 File Offset: 0x00002707
			public override void Deserialize(NetworkReader reader)
			{
			}

			// Token: 0x04000DCF RID: 3535
			private static readonly Dictionary<Type, byte> chatMessageTypeToIndex = new Dictionary<Type, byte>();

			// Token: 0x04000DD0 RID: 3536
			private static readonly List<Type> chatMessageIndexToType = new List<Type>();
		}

		// Token: 0x02000214 RID: 532
		public class UserChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A69 RID: 2665 RVA: 0x00033F14 File Offset: 0x00032114
			public override string ConstructChatString()
			{
				if (this.sender)
				{
					NetworkUser component = this.sender.GetComponent<NetworkUser>();
					if (component)
					{
						return string.Format(CultureInfo.InvariantCulture, "<color=#e5eefc>{0}: {1}</color>", Util.EscapeRichTextForTextMeshPro(component.userName), Util.EscapeRichTextForTextMeshPro(this.text));
					}
				}
				return null;
			}

			// Token: 0x06000A6A RID: 2666 RVA: 0x00033F69 File Offset: 0x00032169
			public override void OnProcessed()
			{
				base.OnProcessed();
				Util.PlaySound("Play_UI_chatMessage", RoR2Application.instance.gameObject);
			}

			// Token: 0x06000A6C RID: 2668 RVA: 0x00033F8E File Offset: 0x0003218E
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.sender);
				writer.Write(this.text);
			}

			// Token: 0x06000A6D RID: 2669 RVA: 0x00033FA8 File Offset: 0x000321A8
			public override void Deserialize(NetworkReader reader)
			{
				this.sender = reader.ReadGameObject();
				this.text = reader.ReadString();
			}

			// Token: 0x04000DD1 RID: 3537
			public GameObject sender;

			// Token: 0x04000DD2 RID: 3538
			public string text;
		}

		// Token: 0x02000215 RID: 533
		public class NpcChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A6E RID: 2670 RVA: 0x00033FC4 File Offset: 0x000321C4
			public override string ConstructChatString()
			{
				if (this.sender)
				{
					CharacterBody component = this.sender.GetComponent<CharacterBody>();
					string arg = ((component != null) ? component.GetDisplayName() : null) ?? "???";
					return string.Format(CultureInfo.InvariantCulture, "<style=cWorldEvent>{0}: {1}</style>", arg, Language.GetString(this.baseToken));
				}
				return null;
			}

			// Token: 0x06000A6F RID: 2671 RVA: 0x0003401C File Offset: 0x0003221C
			public override void OnProcessed()
			{
				base.OnProcessed();
				Util.PlaySound(this.sound, this.sender);
			}

			// Token: 0x06000A71 RID: 2673 RVA: 0x00034036 File Offset: 0x00032236
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.sender);
				writer.Write(this.baseToken);
				writer.Write(this.sound);
			}

			// Token: 0x06000A72 RID: 2674 RVA: 0x0003405C File Offset: 0x0003225C
			public override void Deserialize(NetworkReader reader)
			{
				this.sender = reader.ReadGameObject();
				this.baseToken = reader.ReadString();
				this.sound = reader.ReadString();
			}

			// Token: 0x04000DD3 RID: 3539
			public GameObject sender;

			// Token: 0x04000DD4 RID: 3540
			public string baseToken;

			// Token: 0x04000DD5 RID: 3541
			public string sound;
		}

		// Token: 0x02000216 RID: 534
		public class SimpleChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A73 RID: 2675 RVA: 0x00034084 File Offset: 0x00032284
			public override string ConstructChatString()
			{
				string text = Language.GetString(this.baseToken);
				if (this.paramTokens != null && this.paramTokens.Length != 0)
				{
					IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
					string format = text;
					object[] args = this.paramTokens;
					text = string.Format(invariantCulture, format, args);
				}
				return text;
			}

			// Token: 0x06000A75 RID: 2677 RVA: 0x000340C3 File Offset: 0x000322C3
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.baseToken);
				GeneratedNetworkCode._WriteArrayString_None(writer, this.paramTokens);
			}

			// Token: 0x06000A76 RID: 2678 RVA: 0x000340DD File Offset: 0x000322DD
			public override void Deserialize(NetworkReader reader)
			{
				this.baseToken = reader.ReadString();
				this.paramTokens = GeneratedNetworkCode._ReadArrayString_None(reader);
			}

			// Token: 0x04000DD6 RID: 3542
			public string baseToken;

			// Token: 0x04000DD7 RID: 3543
			public string[] paramTokens;
		}

		// Token: 0x02000217 RID: 535
		public class SubjectChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x170000B5 RID: 181
			// (get) Token: 0x06000A78 RID: 2680 RVA: 0x00034110 File Offset: 0x00032310
			// (set) Token: 0x06000A77 RID: 2679 RVA: 0x000340F7 File Offset: 0x000322F7
			public NetworkUser subjectNetworkUser
			{
				get
				{
					if (!this.subject)
					{
						return null;
					}
					return this.subject.GetComponent<NetworkUser>();
				}
				set
				{
					this.subject = (value ? value.gameObject : null);
				}
			}

			// Token: 0x170000B6 RID: 182
			// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0003413C File Offset: 0x0003233C
			// (set) Token: 0x06000A79 RID: 2681 RVA: 0x0003412C File Offset: 0x0003232C
			public CharacterBody subjectCharacterBody
			{
				get
				{
					GameObject subjectCharacterBodyGameObject = this.subjectCharacterBodyGameObject;
					if (!subjectCharacterBodyGameObject)
					{
						return null;
					}
					return subjectCharacterBodyGameObject.GetComponent<CharacterBody>();
				}
				set
				{
					this.subjectNetworkUser = Util.LookUpBodyNetworkUser(value);
				}
			}

			// Token: 0x170000B7 RID: 183
			// (get) Token: 0x06000A7C RID: 2684 RVA: 0x0003417C File Offset: 0x0003237C
			// (set) Token: 0x06000A7B RID: 2683 RVA: 0x00034160 File Offset: 0x00032360
			public GameObject subjectCharacterBodyGameObject
			{
				get
				{
					NetworkUser subjectNetworkUser = this.subjectNetworkUser;
					if (subjectNetworkUser)
					{
						GameObject masterObject = subjectNetworkUser.masterObject;
						if (masterObject)
						{
							CharacterMaster component = masterObject.GetComponent<CharacterMaster>();
							if (component)
							{
								return component.GetBodyObject();
							}
						}
					}
					return null;
				}
				set
				{
					this.subjectCharacterBody = (value ? value.GetComponent<CharacterBody>() : null);
				}
			}

			// Token: 0x06000A7D RID: 2685 RVA: 0x000341C0 File Offset: 0x000323C0
			protected bool IsSecondPerson()
			{
				if (LocalUserManager.readOnlyLocalUsersList.Count == 1 && this.subject)
				{
					NetworkUser component = this.subject.GetComponent<NetworkUser>();
					if (component && component.localUser != null)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06000A7E RID: 2686 RVA: 0x00034206 File Offset: 0x00032406
			protected string GetResolvedToken()
			{
				if (!this.IsSecondPerson())
				{
					return this.baseToken;
				}
				return this.baseToken + "_2P";
			}

			// Token: 0x06000A7F RID: 2687 RVA: 0x00034227 File Offset: 0x00032427
			public override string ConstructChatString()
			{
				return string.Format(Language.GetString(this.GetResolvedToken()), base.GetObjectName(this.subject));
			}

			// Token: 0x06000A80 RID: 2688 RVA: 0x00034245 File Offset: 0x00032445
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.subject);
				writer.Write(this.baseToken);
			}

			// Token: 0x06000A81 RID: 2689 RVA: 0x00034266 File Offset: 0x00032466
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.subject = reader.ReadGameObject();
				this.baseToken = reader.ReadString();
			}

			// Token: 0x04000DD8 RID: 3544
			public GameObject subject;

			// Token: 0x04000DD9 RID: 3545
			public string baseToken;
		}

		// Token: 0x02000218 RID: 536
		public class SubjectFormatChatMessage : Chat.SubjectChatMessage
		{
			// Token: 0x06000A83 RID: 2691 RVA: 0x00034288 File Offset: 0x00032488
			public override string ConstructChatString()
			{
				string @string = Language.GetString(base.GetResolvedToken());
				string objectName = base.GetObjectName(this.subject);
				string[] array = new string[1 + this.paramTokens.Length];
				array[0] = objectName;
				Array.Copy(this.paramTokens, 0, array, 1, this.paramTokens.Length);
				for (int i = 1; i < array.Length; i++)
				{
					array[i] = Language.GetString(array[i]);
				}
				string format = @string;
				object[] args = array;
				return string.Format(format, args);
			}

			// Token: 0x06000A84 RID: 2692 RVA: 0x000342FC File Offset: 0x000324FC
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write((byte)this.paramTokens.Length);
				for (int i = 0; i < this.paramTokens.Length; i++)
				{
					writer.Write(this.paramTokens[i]);
				}
			}

			// Token: 0x06000A85 RID: 2693 RVA: 0x00034340 File Offset: 0x00032540
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.paramTokens = new string[(int)reader.ReadByte()];
				for (int i = 0; i < this.paramTokens.Length; i++)
				{
					this.paramTokens[i] = reader.ReadString();
				}
			}

			// Token: 0x04000DDA RID: 3546
			private static readonly string[] empty = new string[0];

			// Token: 0x04000DDB RID: 3547
			public string[] paramTokens = Chat.SubjectFormatChatMessage.empty;
		}

		// Token: 0x02000219 RID: 537
		public class PlayerPickupChatMessage : Chat.SubjectChatMessage
		{
			// Token: 0x06000A88 RID: 2696 RVA: 0x000343A8 File Offset: 0x000325A8
			public override string ConstructChatString()
			{
				string objectName = base.GetObjectName(this.subject);
				string @string = Language.GetString(base.GetResolvedToken());
				string arg = "";
				if (this.pickupQuantity != 1u)
				{
					arg = "(" + this.pickupQuantity + ")";
				}
				string text = Language.GetString(this.pickupToken) ?? "???";
				text = Util.GenerateColoredString(text, this.pickupColor);
				return string.Format(@string, objectName, text, arg);
			}

			// Token: 0x06000A89 RID: 2697 RVA: 0x00034421 File Offset: 0x00032621
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.pickupToken);
				writer.Write(this.pickupColor);
				writer.WritePackedUInt32(this.pickupQuantity);
			}

			// Token: 0x06000A8A RID: 2698 RVA: 0x0003444E File Offset: 0x0003264E
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.pickupToken = reader.ReadString();
				this.pickupColor = reader.ReadColor32();
				this.pickupQuantity = reader.ReadPackedUInt32();
			}

			// Token: 0x04000DDC RID: 3548
			public string pickupToken;

			// Token: 0x04000DDD RID: 3549
			public Color32 pickupColor;

			// Token: 0x04000DDE RID: 3550
			public uint pickupQuantity;
		}

		// Token: 0x0200021A RID: 538
		public class PlayerDeathChatMessage : Chat.SubjectFormatChatMessage
		{
			// Token: 0x06000A8C RID: 2700 RVA: 0x00034484 File Offset: 0x00032684
			public override string ConstructChatString()
			{
				string text = base.ConstructChatString();
				if (text != null)
				{
					return "<style=cDeath><sprite name=\"Skull\" tint=1> " + text + " <sprite name=\"Skull\" tint=1></color>";
				}
				return text;
			}

			// Token: 0x06000A8D RID: 2701 RVA: 0x000344AD File Offset: 0x000326AD
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
			}

			// Token: 0x06000A8E RID: 2702 RVA: 0x000344B6 File Offset: 0x000326B6
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
			}
		}

		// Token: 0x0200021B RID: 539
		public class NamedObjectChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A90 RID: 2704 RVA: 0x000344C7 File Offset: 0x000326C7
			public override string ConstructChatString()
			{
				return string.Format(Language.GetString(this.baseToken), base.GetObjectName(this.namedObject));
			}

			// Token: 0x06000A92 RID: 2706 RVA: 0x000344E5 File Offset: 0x000326E5
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.namedObject);
				writer.Write(this.baseToken);
				GeneratedNetworkCode._WriteArrayString_None(writer, this.paramTokens);
			}

			// Token: 0x06000A93 RID: 2707 RVA: 0x0003450B File Offset: 0x0003270B
			public override void Deserialize(NetworkReader reader)
			{
				this.namedObject = reader.ReadGameObject();
				this.baseToken = reader.ReadString();
				this.paramTokens = GeneratedNetworkCode._ReadArrayString_None(reader);
			}

			// Token: 0x04000DDF RID: 3551
			public GameObject namedObject;

			// Token: 0x04000DE0 RID: 3552
			public string baseToken;

			// Token: 0x04000DE1 RID: 3553
			public string[] paramTokens;
		}

		// Token: 0x0200021C RID: 540
		public class PlayerChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A94 RID: 2708 RVA: 0x00034531 File Offset: 0x00032731
			public override string ConstructChatString()
			{
				return string.Format(Language.GetString(this.baseToken), this.networkPlayerName.GetResolvedName());
			}

			// Token: 0x06000A95 RID: 2709 RVA: 0x0003454E File Offset: 0x0003274E
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.networkPlayerName);
				writer.Write(this.baseToken);
			}

			// Token: 0x06000A96 RID: 2710 RVA: 0x0003456F File Offset: 0x0003276F
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.networkPlayerName = reader.ReadNetworkPlayerName();
				this.baseToken = reader.ReadString();
			}

			// Token: 0x04000DE2 RID: 3554
			public NetworkPlayerName networkPlayerName;

			// Token: 0x04000DE3 RID: 3555
			public string baseToken;
		}
	}
}
