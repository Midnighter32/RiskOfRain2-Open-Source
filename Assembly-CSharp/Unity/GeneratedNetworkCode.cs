using System;
using System.Runtime.InteropServices;
using RoR2;
using UnityEngine.Networking;

namespace Unity
{
	// Token: 0x020006ED RID: 1773
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	public class GeneratedNetworkCode
	{
		// Token: 0x06002789 RID: 10121 RVA: 0x000B852C File Offset: 0x000B672C
		public static void _ReadStructSyncListUserVote_VoteController(NetworkReader reader, VoteController.SyncListUserVote instance)
		{
			ushort num = reader.ReadUInt16();
			instance.Clear();
			for (ushort num2 = 0; num2 < num; num2 += 1)
			{
				instance.AddInternal(instance.DeserializeItem(reader));
			}
		}

		// Token: 0x0600278A RID: 10122 RVA: 0x000B8568 File Offset: 0x000B6768
		public static void _WriteStructSyncListUserVote_VoteController(NetworkWriter writer, VoteController.SyncListUserVote value)
		{
			ushort count = value.Count;
			writer.Write(count);
			for (ushort num = 0; num < count; num += 1)
			{
				value.SerializeItem(writer, value.GetItem((int)num));
			}
		}

		// Token: 0x0600278B RID: 10123 RVA: 0x000B85A8 File Offset: 0x000B67A8
		public static void _WriteArrayString_None(NetworkWriter writer, string[] value)
		{
			if (value == null)
			{
				writer.Write(0);
				return;
			}
			ushort value2 = (ushort)value.Length;
			writer.Write(value2);
			ushort num = 0;
			while ((int)num < value.Length)
			{
				writer.Write(value[(int)num]);
				num += 1;
			}
		}

		// Token: 0x0600278C RID: 10124 RVA: 0x000B85FC File Offset: 0x000B67FC
		public static string[] _ReadArrayString_None(NetworkReader reader)
		{
			int num = (int)reader.ReadUInt16();
			if (num == 0)
			{
				return new string[0];
			}
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = reader.ReadString();
			}
			return array;
		}

		// Token: 0x0600278D RID: 10125 RVA: 0x000B864C File Offset: 0x000B684C
		public static void _WriteArrayString_None(NetworkWriter writer, string[] value)
		{
			if (value == null)
			{
				writer.Write(0);
				return;
			}
			ushort value2 = (ushort)value.Length;
			writer.Write(value2);
			ushort num = 0;
			while ((int)num < value.Length)
			{
				writer.Write(value[(int)num]);
				num += 1;
			}
		}

		// Token: 0x0600278E RID: 10126 RVA: 0x000B869D File Offset: 0x000B689D
		public static void _WritePickupIndex_None(NetworkWriter writer, PickupIndex value)
		{
			writer.WritePackedUInt32((uint)value.value);
		}

		// Token: 0x0600278F RID: 10127 RVA: 0x000B86AC File Offset: 0x000B68AC
		public static PickupIndex _ReadPickupIndex_None(NetworkReader reader)
		{
			return new PickupIndex
			{
				value = (int)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x06002790 RID: 10128 RVA: 0x000B86D3 File Offset: 0x000B68D3
		public static void _WriteParentIdentifier_NetworkParent(NetworkWriter writer, NetworkParent.ParentIdentifier value)
		{
			writer.Write(value.parentNetworkIdentity);
			writer.WritePackedUInt32((uint)value.indexInParentChildLocatorPlusOne);
		}

		// Token: 0x06002791 RID: 10129 RVA: 0x000B86F0 File Offset: 0x000B68F0
		public static NetworkParent.ParentIdentifier _ReadParentIdentifier_NetworkParent(NetworkReader reader)
		{
			return new NetworkParent.ParentIdentifier
			{
				parentNetworkIdentity = reader.ReadNetworkIdentity(),
				indexInParentChildLocatorPlusOne = (byte)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x06002792 RID: 10130 RVA: 0x000B8728 File Offset: 0x000B6928
		public static UnlockableIndex _ReadUnlockableIndex_None(NetworkReader reader)
		{
			return new UnlockableIndex
			{
				internalValue = reader.ReadPackedUInt32()
			};
		}

		// Token: 0x06002793 RID: 10131 RVA: 0x000B8750 File Offset: 0x000B6950
		public static UnlockableIndex[] _ReadArrayUnlockableIndex_None(NetworkReader reader)
		{
			int num = (int)reader.ReadUInt16();
			if (num == 0)
			{
				return new UnlockableIndex[0];
			}
			UnlockableIndex[] array = new UnlockableIndex[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = GeneratedNetworkCode._ReadUnlockableIndex_None(reader);
			}
			return array;
		}

		// Token: 0x06002794 RID: 10132 RVA: 0x000B87A0 File Offset: 0x000B69A0
		public static void _WriteUnlockableIndex_None(NetworkWriter writer, UnlockableIndex value)
		{
			writer.WritePackedUInt32(value.internalValue);
		}

		// Token: 0x06002795 RID: 10133 RVA: 0x000B87B0 File Offset: 0x000B69B0
		public static void _WriteArrayUnlockableIndex_None(NetworkWriter writer, UnlockableIndex[] value)
		{
			if (value == null)
			{
				writer.Write(0);
				return;
			}
			ushort value2 = (ushort)value.Length;
			writer.Write(value2);
			ushort num = 0;
			while ((int)num < value.Length)
			{
				GeneratedNetworkCode._WriteUnlockableIndex_None(writer, value[(int)num]);
				num += 1;
			}
		}

		// Token: 0x06002796 RID: 10134 RVA: 0x000B8801 File Offset: 0x000B6A01
		public static void _WriteNetworkUserId_None(NetworkWriter writer, NetworkUserId value)
		{
			writer.WritePackedUInt64(value.value);
			writer.WritePackedUInt32((uint)value.subId);
		}

		// Token: 0x06002797 RID: 10135 RVA: 0x000B881C File Offset: 0x000B6A1C
		public static NetworkUserId _ReadNetworkUserId_None(NetworkReader reader)
		{
			return new NetworkUserId
			{
				value = reader.ReadPackedUInt64(),
				subId = (byte)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x06002798 RID: 10136 RVA: 0x000B8854 File Offset: 0x000B6A54
		public static PingerController.PingInfo _ReadPingInfo_PingerController(NetworkReader reader)
		{
			return new PingerController.PingInfo
			{
				active = reader.ReadBoolean(),
				origin = reader.ReadVector3(),
				normal = reader.ReadVector3(),
				targetNetworkIdentity = reader.ReadNetworkIdentity()
			};
		}

		// Token: 0x06002799 RID: 10137 RVA: 0x000B88A8 File Offset: 0x000B6AA8
		public static void _WritePingInfo_PingerController(NetworkWriter writer, PingerController.PingInfo value)
		{
			writer.Write(value.active);
			writer.Write(value.origin);
			writer.Write(value.normal);
			writer.Write(value.targetNetworkIdentity);
		}

		// Token: 0x0600279A RID: 10138 RVA: 0x000B88DA File Offset: 0x000B6ADA
		public static void _WriteItemMask_None(NetworkWriter writer, ItemMask value)
		{
			writer.WritePackedUInt64(value.a);
			writer.WritePackedUInt64(value.b);
		}

		// Token: 0x0600279B RID: 10139 RVA: 0x000B88F4 File Offset: 0x000B6AF4
		public static void _WriteEquipmentMask_None(NetworkWriter writer, EquipmentMask value)
		{
			writer.WritePackedUInt32(value.a);
		}

		// Token: 0x0600279C RID: 10140 RVA: 0x000B8902 File Offset: 0x000B6B02
		public static void _WriteArtifactMask_None(NetworkWriter writer, ArtifactMask value)
		{
			writer.WritePackedUInt32((uint)value.a);
		}

		// Token: 0x0600279D RID: 10141 RVA: 0x000B8910 File Offset: 0x000B6B10
		public static ItemMask _ReadItemMask_None(NetworkReader reader)
		{
			return new ItemMask
			{
				a = reader.ReadPackedUInt64(),
				b = reader.ReadPackedUInt64()
			};
		}

		// Token: 0x0600279E RID: 10142 RVA: 0x000B8948 File Offset: 0x000B6B48
		public static EquipmentMask _ReadEquipmentMask_None(NetworkReader reader)
		{
			return new EquipmentMask
			{
				a = reader.ReadPackedUInt32()
			};
		}

		// Token: 0x0600279F RID: 10143 RVA: 0x000B8970 File Offset: 0x000B6B70
		public static ArtifactMask _ReadArtifactMask_None(NetworkReader reader)
		{
			return new ArtifactMask
			{
				a = (ushort)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060027A0 RID: 10144 RVA: 0x000B8998 File Offset: 0x000B6B98
		public static CubicBezier3 _ReadCubicBezier3_None(NetworkReader reader)
		{
			return new CubicBezier3
			{
				a = reader.ReadVector3(),
				b = reader.ReadVector3(),
				c = reader.ReadVector3(),
				d = reader.ReadVector3()
			};
		}

		// Token: 0x060027A1 RID: 10145 RVA: 0x000B89EC File Offset: 0x000B6BEC
		public static WormBodyPositions2.KeyFrame _ReadKeyFrame_WormBodyPositions2(NetworkReader reader)
		{
			return new WormBodyPositions2.KeyFrame
			{
				curve = GeneratedNetworkCode._ReadCubicBezier3_None(reader),
				length = reader.ReadSingle(),
				time = reader.ReadSingle()
			};
		}

		// Token: 0x060027A2 RID: 10146 RVA: 0x000B8A31 File Offset: 0x000B6C31
		public static void _WriteCubicBezier3_None(NetworkWriter writer, CubicBezier3 value)
		{
			writer.Write(value.a);
			writer.Write(value.b);
			writer.Write(value.c);
			writer.Write(value.d);
		}

		// Token: 0x060027A3 RID: 10147 RVA: 0x000B8A63 File Offset: 0x000B6C63
		public static void _WriteKeyFrame_WormBodyPositions2(NetworkWriter writer, WormBodyPositions2.KeyFrame value)
		{
			GeneratedNetworkCode._WriteCubicBezier3_None(writer, value.curve);
			writer.Write(value.length);
			writer.Write(value.time);
		}

		// Token: 0x060027A4 RID: 10148 RVA: 0x000B8A89 File Offset: 0x000B6C89
		public static void _WriteHurtBoxReference_None(NetworkWriter writer, HurtBoxReference value)
		{
			writer.Write(value.rootObject);
			writer.WritePackedUInt32((uint)value.hurtBoxIndexPlusOne);
		}

		// Token: 0x060027A5 RID: 10149 RVA: 0x000B8AA4 File Offset: 0x000B6CA4
		public static HurtBoxReference _ReadHurtBoxReference_None(NetworkReader reader)
		{
			return new HurtBoxReference
			{
				rootObject = reader.ReadGameObject(),
				hurtBoxIndexPlusOne = (byte)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060027A6 RID: 10150 RVA: 0x000B8ADC File Offset: 0x000B6CDC
		public static ServerAchievementIndex _ReadServerAchievementIndex_None(NetworkReader reader)
		{
			return new ServerAchievementIndex
			{
				intValue = (int)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060027A7 RID: 10151 RVA: 0x000B8B03 File Offset: 0x000B6D03
		public static void _WriteServerAchievementIndex_None(NetworkWriter writer, ServerAchievementIndex value)
		{
			writer.WritePackedUInt32((uint)value.intValue);
		}
	}
}
