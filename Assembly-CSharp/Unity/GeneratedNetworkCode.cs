using System;
using System.Runtime.InteropServices;
using RoR2;
using UnityEngine.Networking;

namespace Unity
{
	// Token: 0x0200093B RID: 2363
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	public class GeneratedNetworkCode
	{
		// Token: 0x060034D3 RID: 13523 RVA: 0x000E7974 File Offset: 0x000E5B74
		public static void _ReadStructSyncListUserVote_VoteController(NetworkReader reader, VoteController.SyncListUserVote instance)
		{
			ushort num = reader.ReadUInt16();
			instance.Clear();
			for (ushort num2 = 0; num2 < num; num2 += 1)
			{
				instance.AddInternal(instance.DeserializeItem(reader));
			}
		}

		// Token: 0x060034D4 RID: 13524 RVA: 0x000E79B0 File Offset: 0x000E5BB0
		public static void _WriteStructSyncListUserVote_VoteController(NetworkWriter writer, VoteController.SyncListUserVote value)
		{
			ushort count = value.Count;
			writer.Write(count);
			for (ushort num = 0; num < count; num += 1)
			{
				value.SerializeItem(writer, value.GetItem((int)num));
			}
		}

		// Token: 0x060034D5 RID: 13525 RVA: 0x000E79F0 File Offset: 0x000E5BF0
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

		// Token: 0x060034D6 RID: 13526 RVA: 0x000E7A44 File Offset: 0x000E5C44
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

		// Token: 0x060034D7 RID: 13527 RVA: 0x000E7A94 File Offset: 0x000E5C94
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

		// Token: 0x060034D8 RID: 13528 RVA: 0x000E7AE5 File Offset: 0x000E5CE5
		public static void _WriteNetworkMasterIndex_MasterCatalog(NetworkWriter writer, MasterCatalog.NetworkMasterIndex value)
		{
			writer.WritePackedUInt32(value.i);
		}

		// Token: 0x060034D9 RID: 13529 RVA: 0x000E7AF4 File Offset: 0x000E5CF4
		public static MasterCatalog.NetworkMasterIndex _ReadNetworkMasterIndex_MasterCatalog(NetworkReader reader)
		{
			return new MasterCatalog.NetworkMasterIndex
			{
				i = reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060034DA RID: 13530 RVA: 0x000E7B1C File Offset: 0x000E5D1C
		public static CharacterMotor.HitGroundInfo _ReadHitGroundInfo_CharacterMotor(NetworkReader reader)
		{
			return new CharacterMotor.HitGroundInfo
			{
				velocity = reader.ReadVector3(),
				position = reader.ReadVector3()
			};
		}

		// Token: 0x060034DB RID: 13531 RVA: 0x000E7B52 File Offset: 0x000E5D52
		public static void _WriteHitGroundInfo_CharacterMotor(NetworkWriter writer, CharacterMotor.HitGroundInfo value)
		{
			writer.Write(value.velocity);
			writer.Write(value.position);
		}

		// Token: 0x060034DC RID: 13532 RVA: 0x000E7B6C File Offset: 0x000E5D6C
		public static void _WritePickupIndex_None(NetworkWriter writer, PickupIndex value)
		{
			writer.WritePackedUInt32((uint)value.value);
		}

		// Token: 0x060034DD RID: 13533 RVA: 0x000E7B7C File Offset: 0x000E5D7C
		public static PickupIndex _ReadPickupIndex_None(NetworkReader reader)
		{
			return new PickupIndex
			{
				value = (int)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060034DE RID: 13534 RVA: 0x000E7BA3 File Offset: 0x000E5DA3
		public static void _WriteHurtBoxReference_None(NetworkWriter writer, HurtBoxReference value)
		{
			writer.Write(value.rootObject);
			writer.WritePackedUInt32((uint)value.hurtBoxIndexPlusOne);
		}

		// Token: 0x060034DF RID: 13535 RVA: 0x000E7BC0 File Offset: 0x000E5DC0
		public static HurtBoxReference _ReadHurtBoxReference_None(NetworkReader reader)
		{
			return new HurtBoxReference
			{
				rootObject = reader.ReadGameObject(),
				hurtBoxIndexPlusOne = (byte)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060034E0 RID: 13536 RVA: 0x000E7BF6 File Offset: 0x000E5DF6
		public static void _WriteFixedTimeStamp_Run(NetworkWriter writer, Run.FixedTimeStamp value)
		{
			writer.Write(value.t);
		}

		// Token: 0x060034E1 RID: 13537 RVA: 0x000E7C04 File Offset: 0x000E5E04
		public static void _WriteSpinChargeState_LaserTurbineController(NetworkWriter writer, LaserTurbineController.SpinChargeState value)
		{
			writer.Write(value.initialCharge);
			writer.Write(value.initialSpin);
			GeneratedNetworkCode._WriteFixedTimeStamp_Run(writer, value.snapshotTime);
		}

		// Token: 0x060034E2 RID: 13538 RVA: 0x000E7C2C File Offset: 0x000E5E2C
		public static Run.FixedTimeStamp _ReadFixedTimeStamp_Run(NetworkReader reader)
		{
			return new Run.FixedTimeStamp
			{
				t = reader.ReadSingle()
			};
		}

		// Token: 0x060034E3 RID: 13539 RVA: 0x000E7C54 File Offset: 0x000E5E54
		public static LaserTurbineController.SpinChargeState _ReadSpinChargeState_LaserTurbineController(NetworkReader reader)
		{
			return new LaserTurbineController.SpinChargeState
			{
				initialCharge = reader.ReadSingle(),
				initialSpin = reader.ReadSingle(),
				snapshotTime = GeneratedNetworkCode._ReadFixedTimeStamp_Run(reader)
			};
		}

		// Token: 0x060034E4 RID: 13540 RVA: 0x000E7C99 File Offset: 0x000E5E99
		public static void _WriteParentIdentifier_NetworkParent(NetworkWriter writer, NetworkParent.ParentIdentifier value)
		{
			writer.WritePackedUInt32((uint)value.indexInParentChildLocatorPlusOne);
			writer.Write(value.parentNetworkInstanceId);
		}

		// Token: 0x060034E5 RID: 13541 RVA: 0x000E7CB4 File Offset: 0x000E5EB4
		public static NetworkParent.ParentIdentifier _ReadParentIdentifier_NetworkParent(NetworkReader reader)
		{
			return new NetworkParent.ParentIdentifier
			{
				indexInParentChildLocatorPlusOne = (byte)reader.ReadPackedUInt32(),
				parentNetworkInstanceId = reader.ReadNetworkId()
			};
		}

		// Token: 0x060034E6 RID: 13542 RVA: 0x000E7CEC File Offset: 0x000E5EEC
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

		// Token: 0x060034E7 RID: 13543 RVA: 0x000E7D40 File Offset: 0x000E5F40
		public static UnlockableIndex _ReadUnlockableIndex_None(NetworkReader reader)
		{
			return new UnlockableIndex
			{
				internalValue = reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060034E8 RID: 13544 RVA: 0x000E7D68 File Offset: 0x000E5F68
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

		// Token: 0x060034E9 RID: 13545 RVA: 0x000E7DB8 File Offset: 0x000E5FB8
		public static void _WriteUnlockableIndex_None(NetworkWriter writer, UnlockableIndex value)
		{
			writer.WritePackedUInt32(value.internalValue);
		}

		// Token: 0x060034EA RID: 13546 RVA: 0x000E7DC8 File Offset: 0x000E5FC8
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

		// Token: 0x060034EB RID: 13547 RVA: 0x000E7E19 File Offset: 0x000E6019
		public static void _WriteNetworkUserId_None(NetworkWriter writer, NetworkUserId value)
		{
			writer.WritePackedUInt64(value.value);
			writer.WritePackedUInt32((uint)value.subId);
		}

		// Token: 0x060034EC RID: 13548 RVA: 0x000E7E34 File Offset: 0x000E6034
		public static NetworkUserId _ReadNetworkUserId_None(NetworkReader reader)
		{
			return new NetworkUserId
			{
				value = reader.ReadPackedUInt64(),
				subId = (byte)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060034ED RID: 13549 RVA: 0x000E7E6C File Offset: 0x000E606C
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

		// Token: 0x060034EE RID: 13550 RVA: 0x000E7EC0 File Offset: 0x000E60C0
		public static void _WritePingInfo_PingerController(NetworkWriter writer, PingerController.PingInfo value)
		{
			writer.Write(value.active);
			writer.Write(value.origin);
			writer.Write(value.normal);
			writer.Write(value.targetNetworkIdentity);
		}

		// Token: 0x060034EF RID: 13551 RVA: 0x000E7EF2 File Offset: 0x000E60F2
		public static void _WriteItemMask_None(NetworkWriter writer, ItemMask value)
		{
			writer.WritePackedUInt64(value.a);
			writer.WritePackedUInt64(value.b);
		}

		// Token: 0x060034F0 RID: 13552 RVA: 0x000E7F0C File Offset: 0x000E610C
		public static void _WriteEquipmentMask_None(NetworkWriter writer, EquipmentMask value)
		{
			writer.WritePackedUInt64(value.a);
		}

		// Token: 0x060034F1 RID: 13553 RVA: 0x000E7F1A File Offset: 0x000E611A
		public static void _WriteArtifactMask_None(NetworkWriter writer, ArtifactMask value)
		{
			writer.WritePackedUInt32((uint)value.a);
		}

		// Token: 0x060034F2 RID: 13554 RVA: 0x000E7F28 File Offset: 0x000E6128
		public static void _WriteRunStopwatch_Run(NetworkWriter writer, Run.RunStopwatch value)
		{
			writer.Write(value.offsetFromFixedTime);
			writer.Write(value.isPaused);
		}

		// Token: 0x060034F3 RID: 13555 RVA: 0x000E7F44 File Offset: 0x000E6144
		public static ItemMask _ReadItemMask_None(NetworkReader reader)
		{
			return new ItemMask
			{
				a = reader.ReadPackedUInt64(),
				b = reader.ReadPackedUInt64()
			};
		}

		// Token: 0x060034F4 RID: 13556 RVA: 0x000E7F7C File Offset: 0x000E617C
		public static EquipmentMask _ReadEquipmentMask_None(NetworkReader reader)
		{
			return new EquipmentMask
			{
				a = reader.ReadPackedUInt64()
			};
		}

		// Token: 0x060034F5 RID: 13557 RVA: 0x000E7FA4 File Offset: 0x000E61A4
		public static ArtifactMask _ReadArtifactMask_None(NetworkReader reader)
		{
			return new ArtifactMask
			{
				a = (ushort)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060034F6 RID: 13558 RVA: 0x000E7FCC File Offset: 0x000E61CC
		public static Run.RunStopwatch _ReadRunStopwatch_Run(NetworkReader reader)
		{
			return new Run.RunStopwatch
			{
				offsetFromFixedTime = reader.ReadSingle(),
				isPaused = reader.ReadBoolean()
			};
		}

		// Token: 0x060034F7 RID: 13559 RVA: 0x000E8004 File Offset: 0x000E6204
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

		// Token: 0x060034F8 RID: 13560 RVA: 0x000E8058 File Offset: 0x000E6258
		public static WormBodyPositions2.KeyFrame _ReadKeyFrame_WormBodyPositions2(NetworkReader reader)
		{
			return new WormBodyPositions2.KeyFrame
			{
				curve = GeneratedNetworkCode._ReadCubicBezier3_None(reader),
				length = reader.ReadSingle(),
				time = reader.ReadSingle()
			};
		}

		// Token: 0x060034F9 RID: 13561 RVA: 0x000E809D File Offset: 0x000E629D
		public static void _WriteCubicBezier3_None(NetworkWriter writer, CubicBezier3 value)
		{
			writer.Write(value.a);
			writer.Write(value.b);
			writer.Write(value.c);
			writer.Write(value.d);
		}

		// Token: 0x060034FA RID: 13562 RVA: 0x000E80CF File Offset: 0x000E62CF
		public static void _WriteKeyFrame_WormBodyPositions2(NetworkWriter writer, WormBodyPositions2.KeyFrame value)
		{
			GeneratedNetworkCode._WriteCubicBezier3_None(writer, value.curve);
			writer.Write(value.length);
			writer.Write(value.time);
		}

		// Token: 0x060034FB RID: 13563 RVA: 0x000E80F5 File Offset: 0x000E62F5
		public static void _WriteCSteamID_None(NetworkWriter writer, CSteamID value)
		{
			writer.WritePackedUInt64(value.value);
		}

		// Token: 0x060034FC RID: 13564 RVA: 0x000E8104 File Offset: 0x000E6304
		public static CSteamID _ReadCSteamID_None(NetworkReader reader)
		{
			return new CSteamID
			{
				value = reader.ReadPackedUInt64()
			};
		}

		// Token: 0x060034FD RID: 13565 RVA: 0x000E812C File Offset: 0x000E632C
		public static ServerAchievementIndex _ReadServerAchievementIndex_None(NetworkReader reader)
		{
			return new ServerAchievementIndex
			{
				intValue = (int)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x060034FE RID: 13566 RVA: 0x000E8153 File Offset: 0x000E6353
		public static void _WriteServerAchievementIndex_None(NetworkWriter writer, ServerAchievementIndex value)
		{
			writer.WritePackedUInt32((uint)value.intValue);
		}
	}
}
