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
	// Token: 0x020000DE RID: 222
	public static class Chat
	{
		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x00011A68 File Offset: 0x0000FC68
		public static ReadOnlyCollection<string> readOnlyLog
		{
			get
			{
				return Chat._readOnlyLog;
			}
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600044F RID: 1103 RVA: 0x00011A70 File Offset: 0x0000FC70
		// (remove) Token: 0x06000450 RID: 1104 RVA: 0x00011AA4 File Offset: 0x0000FCA4
		public static event Action onChatChanged;

		// Token: 0x06000451 RID: 1105 RVA: 0x00011AD8 File Offset: 0x0000FCD8
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

		// Token: 0x06000452 RID: 1106 RVA: 0x00011B32 File Offset: 0x0000FD32
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

		// Token: 0x06000453 RID: 1107 RVA: 0x00011B4D File Offset: 0x0000FD4D
		public static void SendBroadcastChat(Chat.ChatMessageBase message)
		{
			Chat.SendBroadcastChat(message, QosChannelIndex.chat.intVal);
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00011B60 File Offset: 0x0000FD60
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

		// Token: 0x06000455 RID: 1109 RVA: 0x00011BDC File Offset: 0x0000FDDC
		public static void SendPlayerConnectedMessage(NetworkUser user)
		{
			Chat.SendBroadcastChat(new Chat.PlayerChatMessage
			{
				networkPlayerName = user.GetNetworkPlayerName(),
				baseToken = "PLAYER_CONNECTED"
			});
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00011BFF File Offset: 0x0000FDFF
		public static void SendPlayerDisconnectedMessage(NetworkUser user)
		{
			Chat.SendBroadcastChat(new Chat.PlayerChatMessage
			{
				networkPlayerName = user.GetNetworkPlayerName(),
				baseToken = "PLAYER_DISCONNECTED"
			});
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00011C22 File Offset: 0x0000FE22
		public static void AddPickupMessage(CharacterBody body, string pickupToken, Color32 pickupColor, uint pickupQuantity)
		{
			Chat.AddMessage(new Chat.PlayerPickupChatMessage
			{
				subjectAsCharacterBody = body,
				baseToken = "PLAYER_PICKUP",
				pickupToken = pickupToken,
				pickupColor = pickupColor,
				pickupQuantity = pickupQuantity
			});
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00011C58 File Offset: 0x0000FE58
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

		// Token: 0x06000459 RID: 1113 RVA: 0x00011C8C File Offset: 0x0000FE8C
		private static void AddMessage(Chat.ChatMessageBase message)
		{
			string text = message.ConstructChatString();
			if (text != null)
			{
				Chat.AddMessage(text);
				message.OnProcessed();
			}
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00011CAF File Offset: 0x0000FEAF
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

		// Token: 0x04000424 RID: 1060
		private static List<string> log = new List<string>();

		// Token: 0x04000425 RID: 1061
		private static ReadOnlyCollection<string> _readOnlyLog = Chat.log.AsReadOnly();

		// Token: 0x04000427 RID: 1063
		private static IntConVar cvChatMaxMessages = new IntConVar("chat_max_messages", ConVarFlags.None, "30", "Maximum number of chat messages to store.");

		// Token: 0x04000428 RID: 1064
		private static readonly BoolConVar cvChatDebug = new BoolConVar("chat_debug", ConVarFlags.None, "0", "Enables logging of chat network messages.");

		// Token: 0x020000DF RID: 223
		public abstract class ChatMessageBase : MessageBase
		{
			// Token: 0x0600045C RID: 1116 RVA: 0x00011D4A File Offset: 0x0000FF4A
			static ChatMessageBase()
			{
				Chat.ChatMessageBase.BuildMessageTypeNetMap();
			}

			// Token: 0x0600045D RID: 1117
			public abstract string ConstructChatString();

			// Token: 0x0600045E RID: 1118 RVA: 0x0000409B File Offset: 0x0000229B
			public virtual void OnProcessed()
			{
			}

			// Token: 0x0600045F RID: 1119 RVA: 0x00011D68 File Offset: 0x0000FF68
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

			// Token: 0x06000460 RID: 1120 RVA: 0x00011DD0 File Offset: 0x0000FFD0
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

			// Token: 0x06000461 RID: 1121 RVA: 0x00011E1D File Offset: 0x0001001D
			public byte GetTypeIndex()
			{
				return Chat.ChatMessageBase.chatMessageTypeToIndex[base.GetType()];
			}

			// Token: 0x06000462 RID: 1122 RVA: 0x00011E30 File Offset: 0x00010030
			public static Chat.ChatMessageBase Instantiate(byte typeIndex)
			{
				Type type = Chat.ChatMessageBase.chatMessageIndexToType[(int)typeIndex];
				if (Chat.cvChatDebug.value)
				{
					Debug.LogFormat("Received chat message typeIndex={0} type={1}", new object[]
					{
						typeIndex,
						(type != null) ? type.Name : null
					});
				}
				if (type != null)
				{
					return (Chat.ChatMessageBase)Activator.CreateInstance(type);
				}
				return null;
			}

			// Token: 0x06000464 RID: 1124 RVA: 0x0000409B File Offset: 0x0000229B
			public override void Serialize(NetworkWriter writer)
			{
			}

			// Token: 0x06000465 RID: 1125 RVA: 0x0000409B File Offset: 0x0000229B
			public override void Deserialize(NetworkReader reader)
			{
			}

			// Token: 0x04000429 RID: 1065
			private static readonly Dictionary<Type, byte> chatMessageTypeToIndex = new Dictionary<Type, byte>();

			// Token: 0x0400042A RID: 1066
			private static readonly List<Type> chatMessageIndexToType = new List<Type>();
		}

		// Token: 0x020000E0 RID: 224
		public class UserChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000466 RID: 1126 RVA: 0x00011E9C File Offset: 0x0001009C
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

			// Token: 0x06000467 RID: 1127 RVA: 0x00011EF1 File Offset: 0x000100F1
			public override void OnProcessed()
			{
				base.OnProcessed();
				Util.PlaySound("Play_UI_chatMessage", RoR2Application.instance.gameObject);
			}

			// Token: 0x06000469 RID: 1129 RVA: 0x00011F16 File Offset: 0x00010116
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.sender);
				writer.Write(this.text);
			}

			// Token: 0x0600046A RID: 1130 RVA: 0x00011F30 File Offset: 0x00010130
			public override void Deserialize(NetworkReader reader)
			{
				this.sender = reader.ReadGameObject();
				this.text = reader.ReadString();
			}

			// Token: 0x0400042B RID: 1067
			public GameObject sender;

			// Token: 0x0400042C RID: 1068
			public string text;
		}

		// Token: 0x020000E1 RID: 225
		public class NpcChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x0600046B RID: 1131 RVA: 0x00011F4C File Offset: 0x0001014C
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

			// Token: 0x0600046C RID: 1132 RVA: 0x00011FA4 File Offset: 0x000101A4
			public override void OnProcessed()
			{
				base.OnProcessed();
				Util.PlaySound(this.sound, this.sender);
			}

			// Token: 0x0600046E RID: 1134 RVA: 0x00011FBE File Offset: 0x000101BE
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.sender);
				writer.Write(this.baseToken);
				writer.Write(this.sound);
			}

			// Token: 0x0600046F RID: 1135 RVA: 0x00011FE4 File Offset: 0x000101E4
			public override void Deserialize(NetworkReader reader)
			{
				this.sender = reader.ReadGameObject();
				this.baseToken = reader.ReadString();
				this.sound = reader.ReadString();
			}

			// Token: 0x0400042D RID: 1069
			public GameObject sender;

			// Token: 0x0400042E RID: 1070
			public string baseToken;

			// Token: 0x0400042F RID: 1071
			public string sound;
		}

		// Token: 0x020000E2 RID: 226
		public class SimpleChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000470 RID: 1136 RVA: 0x0001200C File Offset: 0x0001020C
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

			// Token: 0x06000472 RID: 1138 RVA: 0x0001204B File Offset: 0x0001024B
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.baseToken);
				GeneratedNetworkCode._WriteArrayString_None(writer, this.paramTokens);
			}

			// Token: 0x06000473 RID: 1139 RVA: 0x00012065 File Offset: 0x00010265
			public override void Deserialize(NetworkReader reader)
			{
				this.baseToken = reader.ReadString();
				this.paramTokens = GeneratedNetworkCode._ReadArrayString_None(reader);
			}

			// Token: 0x04000430 RID: 1072
			public string baseToken;

			// Token: 0x04000431 RID: 1073
			public string[] paramTokens;
		}

		// Token: 0x020000E3 RID: 227
		public class SubjectChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x17000091 RID: 145
			// (get) Token: 0x06000475 RID: 1141 RVA: 0x000120CC File Offset: 0x000102CC
			// (set) Token: 0x06000474 RID: 1140 RVA: 0x00012080 File Offset: 0x00010280
			public NetworkUser subjectAsNetworkUser
			{
				get
				{
					return this.subjectNetworkUserGetComponent.Get(this.subjectNetworkUserObject);
				}
				set
				{
					this.subjectNetworkUserObject = (value ? value.gameObject : null);
					CharacterBody characterBody = null;
					if (value)
					{
						characterBody = value.GetCurrentBody();
					}
					this.subjectCharacterBodyGameObject = (characterBody ? characterBody.gameObject : null);
				}
			}

			// Token: 0x17000092 RID: 146
			// (get) Token: 0x06000477 RID: 1143 RVA: 0x00012110 File Offset: 0x00010310
			// (set) Token: 0x06000476 RID: 1142 RVA: 0x000120DF File Offset: 0x000102DF
			public CharacterBody subjectAsCharacterBody
			{
				get
				{
					return this.subjectCharacterBodyGetComponent.Get(this.subjectCharacterBodyGameObject);
				}
				set
				{
					this.subjectCharacterBodyGameObject = (value ? value.gameObject : null);
					NetworkUser networkUser = Util.LookUpBodyNetworkUser(value);
					this.subjectNetworkUserObject = ((networkUser != null) ? networkUser.gameObject : null);
				}
			}

			// Token: 0x06000478 RID: 1144 RVA: 0x00012123 File Offset: 0x00010323
			protected string GetSubjectName()
			{
				if (this.subjectAsNetworkUser)
				{
					return Util.EscapeRichTextForTextMeshPro(this.subjectAsNetworkUser.userName);
				}
				if (this.subjectAsCharacterBody)
				{
					return this.subjectAsCharacterBody.GetDisplayName();
				}
				return "???";
			}

			// Token: 0x06000479 RID: 1145 RVA: 0x00012161 File Offset: 0x00010361
			protected bool IsSecondPerson()
			{
				return LocalUserManager.readOnlyLocalUsersList.Count == 1 && this.subjectAsNetworkUser && this.subjectAsNetworkUser.localUser != null;
			}

			// Token: 0x0600047A RID: 1146 RVA: 0x0001218D File Offset: 0x0001038D
			protected string GetResolvedToken()
			{
				if (!this.IsSecondPerson())
				{
					return this.baseToken;
				}
				return this.baseToken + "_2P";
			}

			// Token: 0x0600047B RID: 1147 RVA: 0x000121AE File Offset: 0x000103AE
			public override string ConstructChatString()
			{
				return string.Format(Language.GetString(this.GetResolvedToken()), this.GetSubjectName());
			}

			// Token: 0x0600047C RID: 1148 RVA: 0x000121C6 File Offset: 0x000103C6
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.subjectNetworkUserObject);
				writer.Write(this.subjectCharacterBodyGameObject);
				writer.Write(this.baseToken);
			}

			// Token: 0x0600047D RID: 1149 RVA: 0x000121F3 File Offset: 0x000103F3
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.subjectNetworkUserObject = reader.ReadGameObject();
				this.subjectCharacterBodyGameObject = reader.ReadGameObject();
				this.baseToken = reader.ReadString();
			}

			// Token: 0x04000432 RID: 1074
			private GameObject subjectNetworkUserObject;

			// Token: 0x04000433 RID: 1075
			private GameObject subjectCharacterBodyGameObject;

			// Token: 0x04000434 RID: 1076
			public string baseToken;

			// Token: 0x04000435 RID: 1077
			private MemoizedGetComponent<NetworkUser> subjectNetworkUserGetComponent;

			// Token: 0x04000436 RID: 1078
			private MemoizedGetComponent<CharacterBody> subjectCharacterBodyGetComponent;
		}

		// Token: 0x020000E4 RID: 228
		public class SubjectFormatChatMessage : Chat.SubjectChatMessage
		{
			// Token: 0x0600047F RID: 1151 RVA: 0x00012220 File Offset: 0x00010420
			public override string ConstructChatString()
			{
				string @string = Language.GetString(base.GetResolvedToken());
				string subjectName = base.GetSubjectName();
				string[] array = new string[1 + this.paramTokens.Length];
				array[0] = subjectName;
				Array.Copy(this.paramTokens, 0, array, 1, this.paramTokens.Length);
				for (int i = 1; i < array.Length; i++)
				{
					array[i] = Language.GetString(array[i]);
				}
				string format = @string;
				object[] args = array;
				return string.Format(format, args);
			}

			// Token: 0x06000480 RID: 1152 RVA: 0x00012290 File Offset: 0x00010490
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write((byte)this.paramTokens.Length);
				for (int i = 0; i < this.paramTokens.Length; i++)
				{
					writer.Write(this.paramTokens[i]);
				}
			}

			// Token: 0x06000481 RID: 1153 RVA: 0x000122D4 File Offset: 0x000104D4
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.paramTokens = new string[(int)reader.ReadByte()];
				for (int i = 0; i < this.paramTokens.Length; i++)
				{
					this.paramTokens[i] = reader.ReadString();
				}
			}

			// Token: 0x04000437 RID: 1079
			private static readonly string[] empty = new string[0];

			// Token: 0x04000438 RID: 1080
			public string[] paramTokens = Chat.SubjectFormatChatMessage.empty;
		}

		// Token: 0x020000E5 RID: 229
		public class PlayerPickupChatMessage : Chat.SubjectChatMessage
		{
			// Token: 0x06000484 RID: 1156 RVA: 0x0001233C File Offset: 0x0001053C
			public override string ConstructChatString()
			{
				string subjectName = base.GetSubjectName();
				string @string = Language.GetString(base.GetResolvedToken());
				string arg = "";
				if (this.pickupQuantity != 1U)
				{
					arg = "(" + this.pickupQuantity + ")";
				}
				string text = Language.GetString(this.pickupToken) ?? "???";
				text = Util.GenerateColoredString(text, this.pickupColor);
				return string.Format(@string, subjectName, text, arg);
			}

			// Token: 0x06000485 RID: 1157 RVA: 0x000123AF File Offset: 0x000105AF
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.pickupToken);
				writer.Write(this.pickupColor);
				writer.WritePackedUInt32(this.pickupQuantity);
			}

			// Token: 0x06000486 RID: 1158 RVA: 0x000123DC File Offset: 0x000105DC
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.pickupToken = reader.ReadString();
				this.pickupColor = reader.ReadColor32();
				this.pickupQuantity = reader.ReadPackedUInt32();
			}

			// Token: 0x04000439 RID: 1081
			public string pickupToken;

			// Token: 0x0400043A RID: 1082
			public Color32 pickupColor;

			// Token: 0x0400043B RID: 1083
			public uint pickupQuantity;
		}

		// Token: 0x020000E6 RID: 230
		public class PlayerDeathChatMessage : Chat.SubjectFormatChatMessage
		{
			// Token: 0x06000488 RID: 1160 RVA: 0x00012414 File Offset: 0x00010614
			public override string ConstructChatString()
			{
				string text = base.ConstructChatString();
				if (text != null)
				{
					return "<style=cDeath><sprite name=\"Skull\" tint=1> " + text + " <sprite name=\"Skull\" tint=1></style>";
				}
				return text;
			}

			// Token: 0x06000489 RID: 1161 RVA: 0x0001243D File Offset: 0x0001063D
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
			}

			// Token: 0x0600048A RID: 1162 RVA: 0x00012446 File Offset: 0x00010646
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
			}
		}

		// Token: 0x020000E7 RID: 231
		public class NamedObjectChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x0600048C RID: 1164 RVA: 0x00012457 File Offset: 0x00010657
			public override string ConstructChatString()
			{
				return string.Format(Language.GetString(this.baseToken), base.GetObjectName(this.namedObject));
			}

			// Token: 0x0600048E RID: 1166 RVA: 0x00012475 File Offset: 0x00010675
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.namedObject);
				writer.Write(this.baseToken);
				GeneratedNetworkCode._WriteArrayString_None(writer, this.paramTokens);
			}

			// Token: 0x0600048F RID: 1167 RVA: 0x0001249B File Offset: 0x0001069B
			public override void Deserialize(NetworkReader reader)
			{
				this.namedObject = reader.ReadGameObject();
				this.baseToken = reader.ReadString();
				this.paramTokens = GeneratedNetworkCode._ReadArrayString_None(reader);
			}

			// Token: 0x0400043C RID: 1084
			public GameObject namedObject;

			// Token: 0x0400043D RID: 1085
			public string baseToken;

			// Token: 0x0400043E RID: 1086
			public string[] paramTokens;
		}

		// Token: 0x020000E8 RID: 232
		public class PlayerChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000490 RID: 1168 RVA: 0x000124C1 File Offset: 0x000106C1
			public override string ConstructChatString()
			{
				return string.Format(Language.GetString(this.baseToken), this.networkPlayerName.GetResolvedName());
			}

			// Token: 0x06000491 RID: 1169 RVA: 0x000124DE File Offset: 0x000106DE
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.networkPlayerName);
				writer.Write(this.baseToken);
			}

			// Token: 0x06000492 RID: 1170 RVA: 0x000124FF File Offset: 0x000106FF
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.networkPlayerName = reader.ReadNetworkPlayerName();
				this.baseToken = reader.ReadString();
			}

			// Token: 0x0400043F RID: 1087
			public NetworkPlayerName networkPlayerName;

			// Token: 0x04000440 RID: 1088
			public string baseToken;
		}
	}
}
