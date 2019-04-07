using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001EF RID: 495
	public static class NetworkExtensions
	{
		// Token: 0x06000994 RID: 2452 RVA: 0x0003097F File Offset: 0x0002EB7F
		public static void WriteAchievementIndex(this NetworkWriter writer, AchievementIndex value)
		{
			writer.WritePackedUInt32((uint)value.intValue);
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x00030990 File Offset: 0x0002EB90
		public static AchievementIndex ReadAchievementIndex(this NetworkReader reader)
		{
			return new AchievementIndex
			{
				intValue = (int)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x000309B3 File Offset: 0x0002EBB3
		public static void WriteBodyIndex(this NetworkWriter writer, int bodyIndex)
		{
			writer.WritePackedUInt32((uint)(bodyIndex + 1));
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x000309BE File Offset: 0x0002EBBE
		public static int ReadBodyIndex(this NetworkReader reader)
		{
			return (int)(reader.ReadPackedUInt32() - 1u);
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x000309C8 File Offset: 0x0002EBC8
		public static void WriteBuffMask(this NetworkWriter writer, BuffMask buffMask)
		{
			BuffMask.WriteBuffMask(writer, buffMask);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x000309D1 File Offset: 0x0002EBD1
		public static BuffMask ReadBuffMask(this NetworkReader reader)
		{
			return BuffMask.ReadBuffMask(reader);
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x000309D9 File Offset: 0x0002EBD9
		public static DamageType ReadDamageType(this NetworkReader reader)
		{
			return (DamageType)reader.ReadUInt16();
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x000309E1 File Offset: 0x0002EBE1
		public static void Write(this NetworkWriter writer, DamageType damageType)
		{
			writer.Write((ushort)damageType);
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x000309EA File Offset: 0x0002EBEA
		public static DamageColorIndex ReadDamageColorIndex(this NetworkReader reader)
		{
			return (DamageColorIndex)reader.ReadByte();
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x000309F2 File Offset: 0x0002EBF2
		public static void Write(this NetworkWriter writer, DamageColorIndex damageColorIndex)
		{
			writer.Write((byte)damageColorIndex);
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x000309FB File Offset: 0x0002EBFB
		public static void Write(this NetworkWriter writer, EffectData effectData)
		{
			effectData.Serialize(writer);
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00030A04 File Offset: 0x0002EC04
		public static EffectData ReadEffectData(this NetworkReader reader)
		{
			EffectData effectData = new EffectData();
			effectData.Deserialize(reader);
			return effectData;
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00030A12 File Offset: 0x0002EC12
		public static void ReadEffectData(this NetworkReader reader, EffectData effectData)
		{
			effectData.Deserialize(reader);
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x000309B3 File Offset: 0x0002EBB3
		public static void Write(this NetworkWriter writer, EquipmentIndex equipmentIndex)
		{
			writer.WritePackedUInt32((uint)(equipmentIndex + 1));
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x000309BE File Offset: 0x0002EBBE
		public static EquipmentIndex ReadEquipmentIndex(this NetworkReader reader)
		{
			return (EquipmentIndex)(reader.ReadPackedUInt32() - 1u);
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00030A1B File Offset: 0x0002EC1B
		public static void Write(this NetworkWriter writer, HurtBoxReference hurtBoxReference)
		{
			hurtBoxReference.Write(writer);
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x00030A28 File Offset: 0x0002EC28
		public static HurtBoxReference ReadHurtBoxReference(this NetworkReader reader)
		{
			HurtBoxReference result = default(HurtBoxReference);
			result.Read(reader);
			return result;
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x00030A46 File Offset: 0x0002EC46
		public static void Write(this NetworkWriter writer, Run.TimeStamp timeStamp)
		{
			Run.TimeStamp.Serialize(writer, timeStamp);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00030A4F File Offset: 0x0002EC4F
		public static Run.TimeStamp ReadTimeStamp(this NetworkReader reader)
		{
			return Run.TimeStamp.Deserialize(reader);
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x00030A57 File Offset: 0x0002EC57
		public static void Write(this NetworkWriter writer, Run.FixedTimeStamp timeStamp)
		{
			Run.FixedTimeStamp.Serialize(writer, timeStamp);
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x00030A60 File Offset: 0x0002EC60
		public static Run.FixedTimeStamp ReadFixedTimeStamp(this NetworkReader reader)
		{
			return Run.FixedTimeStamp.Deserialize(reader);
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x000309B3 File Offset: 0x0002EBB3
		public static void Write(this NetworkWriter writer, ItemIndex itemIndex)
		{
			writer.WritePackedUInt32((uint)(itemIndex + 1));
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x000309BE File Offset: 0x0002EBBE
		public static ItemIndex ReadItemIndex(this NetworkReader reader)
		{
			return (ItemIndex)(reader.ReadPackedUInt32() - 1u);
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x00030A68 File Offset: 0x0002EC68
		public static void WriteItemStacks(this NetworkWriter writer, int[] srcItemStacks)
		{
			int num = 0;
			for (int i = 0; i < 10; i++)
			{
				byte b = 0;
				int num2 = 0;
				while (num2 < 8 && num < 78)
				{
					if (srcItemStacks[num] > 0)
					{
						b |= (byte)(1 << num2);
					}
					num2++;
					num++;
				}
				NetworkExtensions.itemMaskByteBuffer[i] = b;
			}
			for (int j = 0; j < 10; j++)
			{
				writer.Write(NetworkExtensions.itemMaskByteBuffer[j]);
			}
			for (int k = 0; k < 78; k++)
			{
				int num3 = srcItemStacks[k];
				if (num3 > 0)
				{
					writer.WritePackedUInt32((uint)num3);
				}
			}
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00030AF8 File Offset: 0x0002ECF8
		public static void ReadItemStacks(this NetworkReader reader, int[] destItemStacks)
		{
			for (int i = 0; i < 10; i++)
			{
				NetworkExtensions.itemMaskByteBuffer[i] = reader.ReadByte();
			}
			int num = 0;
			for (int j = 0; j < 10; j++)
			{
				byte b = NetworkExtensions.itemMaskByteBuffer[j];
				int num2 = 0;
				while (num2 < 8 && num < 78)
				{
					destItemStacks[num] = (int)(((b & (byte)(1 << num2)) != 0) ? reader.ReadPackedUInt32() : 0u);
					num2++;
					num++;
				}
			}
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x00030B67 File Offset: 0x0002ED67
		public static void WriteBitArray(this NetworkWriter writer, [NotNull] bool[] values)
		{
			writer.WriteBitArray(values, values.Length);
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x00030B74 File Offset: 0x0002ED74
		public static void WriteBitArray(this NetworkWriter writer, [NotNull] bool[] values, int bufferLength)
		{
			int num = bufferLength + 7 >> 3;
			int num2 = num - 1;
			int num3 = bufferLength - (num2 << 3);
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				byte b = 0;
				int num5 = (i < num2) ? 8 : num3;
				int j = 0;
				while (j < num5)
				{
					if (values[num4])
					{
						b |= (byte)(1 << j);
					}
					j++;
					num4++;
				}
				writer.Write(b);
			}
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x00030BE1 File Offset: 0x0002EDE1
		public static void ReadBitArray(this NetworkReader reader, [NotNull] bool[] values)
		{
			reader.ReadBitArray(values, values.Length);
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x00030BF0 File Offset: 0x0002EDF0
		public static void ReadBitArray(this NetworkReader reader, [NotNull] bool[] values, int bufferLength)
		{
			int num = bufferLength + 7 >> 3;
			int num2 = num - 1;
			int num3 = bufferLength - (num2 << 3);
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				int num5 = (i < num2) ? 8 : num3;
				byte b = reader.ReadByte();
				int j = 0;
				while (j < num5)
				{
					values[num4] = ((b & (byte)(1 << j)) > 0);
					j++;
					num4++;
				}
			}
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x00030C58 File Offset: 0x0002EE58
		public static void Write(this NetworkWriter writer, NetworkPlayerName networkPlayerName)
		{
			networkPlayerName.Serialize(writer);
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x00030C64 File Offset: 0x0002EE64
		public static NetworkPlayerName ReadNetworkPlayerName(this NetworkReader reader)
		{
			NetworkPlayerName result = default(NetworkPlayerName);
			result.Deserialize(reader);
			return result;
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x00030C82 File Offset: 0x0002EE82
		public static void Write(this NetworkWriter writer, PitchYawPair pitchYawPair)
		{
			writer.Write(pitchYawPair.pitch);
			writer.Write(pitchYawPair.yaw);
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x00030CA0 File Offset: 0x0002EEA0
		public static PitchYawPair ReadPitchYawPair(this NetworkReader reader)
		{
			float pitch = reader.ReadSingle();
			float yaw = reader.ReadSingle();
			return new PitchYawPair(pitch, yaw);
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x00030CC0 File Offset: 0x0002EEC0
		public static void Write(this NetworkWriter writer, RuleBook src)
		{
			src.Serialize(writer);
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x00030CC9 File Offset: 0x0002EEC9
		public static void ReadRuleBook(this NetworkReader reader, RuleBook dest)
		{
			dest.Deserialize(reader);
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x00030CD2 File Offset: 0x0002EED2
		public static void Write(this NetworkWriter writer, RuleMask src)
		{
			src.Serialize(writer);
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x00030CDB File Offset: 0x0002EEDB
		public static void ReadRuleMask(this NetworkReader reader, RuleMask dest)
		{
			dest.Deserialize(reader);
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x00030CE4 File Offset: 0x0002EEE4
		public static void Write(this NetworkWriter writer, RuleChoiceMask src)
		{
			src.Serialize(writer);
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x00030CED File Offset: 0x0002EEED
		public static void ReadRuleChoiceMask(this NetworkReader reader, RuleChoiceMask dest)
		{
			dest.Deserialize(reader);
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x00030CF8 File Offset: 0x0002EEF8
		public static void Write(this NetworkWriter writer, TeamIndex teamIndex)
		{
			byte value = (byte)(teamIndex + 1);
			writer.Write(value);
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x00030D11 File Offset: 0x0002EF11
		public static TeamIndex ReadTeamIndex(this NetworkReader reader)
		{
			return (TeamIndex)(reader.ReadByte() - 1);
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x00030D1C File Offset: 0x0002EF1C
		public static void Write(this NetworkWriter writer, UnlockableIndex index)
		{
			writer.Write((byte)index.internalValue);
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x00030D2C File Offset: 0x0002EF2C
		public static UnlockableIndex ReadUnlockableIndex(this NetworkReader reader)
		{
			return new UnlockableIndex
			{
				internalValue = (uint)reader.ReadByte()
			};
		}

		// Token: 0x04000CF4 RID: 3316
		private const int itemMaskBitCount = 78;

		// Token: 0x04000CF5 RID: 3317
		private const int itemMaskByteCount = 10;

		// Token: 0x04000CF6 RID: 3318
		private static readonly byte[] itemMaskByteBuffer = new byte[10];
	}
}
