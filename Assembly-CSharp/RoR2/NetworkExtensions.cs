using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020000B0 RID: 176
	public static class NetworkExtensions
	{
		// Token: 0x06000354 RID: 852 RVA: 0x0000D6CF File Offset: 0x0000B8CF
		public static void WriteAchievementIndex(this NetworkWriter writer, AchievementIndex value)
		{
			writer.WritePackedUInt32((uint)value.intValue);
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0000D6E0 File Offset: 0x0000B8E0
		public static AchievementIndex ReadAchievementIndex(this NetworkReader reader)
		{
			return new AchievementIndex
			{
				intValue = (int)reader.ReadPackedUInt32()
			};
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0000D703 File Offset: 0x0000B903
		public static void WriteBodyIndex(this NetworkWriter writer, int bodyIndex)
		{
			writer.WritePackedUInt32((uint)(bodyIndex + 1));
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0000D70E File Offset: 0x0000B90E
		public static int ReadBodyIndex(this NetworkReader reader)
		{
			return (int)(reader.ReadPackedUInt32() - 1U);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0000D718 File Offset: 0x0000B918
		public static void WriteBuffMask(this NetworkWriter writer, BuffMask buffMask)
		{
			BuffMask.WriteBuffMask(writer, buffMask);
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0000D721 File Offset: 0x0000B921
		public static BuffMask ReadBuffMask(this NetworkReader reader)
		{
			return BuffMask.ReadBuffMask(reader);
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0000D729 File Offset: 0x0000B929
		public static void WriteBuffIndex(this NetworkWriter writer, BuffIndex buffIndex)
		{
			writer.Write((byte)(buffIndex + 1));
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0000D735 File Offset: 0x0000B935
		public static BuffIndex ReadBuffIndex(this NetworkReader reader)
		{
			return (BuffIndex)(reader.ReadByte() - 1);
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0000D73F File Offset: 0x0000B93F
		public static DamageType ReadDamageType(this NetworkReader reader)
		{
			return (DamageType)reader.ReadPackedUInt32();
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0000D747 File Offset: 0x0000B947
		public static void Write(this NetworkWriter writer, DamageType damageType)
		{
			writer.WritePackedUInt64((ulong)damageType);
		}

		// Token: 0x0600035E RID: 862 RVA: 0x0000D751 File Offset: 0x0000B951
		public static DamageColorIndex ReadDamageColorIndex(this NetworkReader reader)
		{
			return (DamageColorIndex)reader.ReadByte();
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0000D759 File Offset: 0x0000B959
		public static void Write(this NetworkWriter writer, DamageColorIndex damageColorIndex)
		{
			writer.Write((byte)damageColorIndex);
		}

		// Token: 0x06000360 RID: 864 RVA: 0x0000D703 File Offset: 0x0000B903
		public static void WriteEffectIndex(this NetworkWriter writer, EffectIndex effectIndex)
		{
			writer.WritePackedUInt32((uint)(effectIndex + 1));
		}

		// Token: 0x06000361 RID: 865 RVA: 0x0000D70E File Offset: 0x0000B90E
		public static EffectIndex ReadEffectIndex(this NetworkReader reader)
		{
			return (EffectIndex)(reader.ReadPackedUInt32() - 1U);
		}

		// Token: 0x06000362 RID: 866 RVA: 0x0000D762 File Offset: 0x0000B962
		public static void Write(this NetworkWriter writer, EffectData effectData)
		{
			effectData.Serialize(writer);
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0000D76B File Offset: 0x0000B96B
		public static EffectData ReadEffectData(this NetworkReader reader)
		{
			EffectData effectData = new EffectData();
			effectData.Deserialize(reader);
			return effectData;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0000D779 File Offset: 0x0000B979
		public static void ReadEffectData(this NetworkReader reader, EffectData effectData)
		{
			effectData.Deserialize(reader);
		}

		// Token: 0x06000365 RID: 869 RVA: 0x0000D703 File Offset: 0x0000B903
		public static void Write(this NetworkWriter writer, EquipmentIndex equipmentIndex)
		{
			writer.WritePackedUInt32((uint)(equipmentIndex + 1));
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0000D70E File Offset: 0x0000B90E
		public static EquipmentIndex ReadEquipmentIndex(this NetworkReader reader)
		{
			return (EquipmentIndex)(reader.ReadPackedUInt32() - 1U);
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0000D782 File Offset: 0x0000B982
		public static void Write(this NetworkWriter writer, HurtBoxReference hurtBoxReference)
		{
			hurtBoxReference.Write(writer);
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0000D78C File Offset: 0x0000B98C
		public static HurtBoxReference ReadHurtBoxReference(this NetworkReader reader)
		{
			HurtBoxReference result = default(HurtBoxReference);
			result.Read(reader);
			return result;
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0000D7AA File Offset: 0x0000B9AA
		public static void Write(this NetworkWriter writer, Run.TimeStamp timeStamp)
		{
			Run.TimeStamp.Serialize(writer, timeStamp);
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0000D7B3 File Offset: 0x0000B9B3
		public static Run.TimeStamp ReadTimeStamp(this NetworkReader reader)
		{
			return Run.TimeStamp.Deserialize(reader);
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0000D7BB File Offset: 0x0000B9BB
		public static void Write(this NetworkWriter writer, Run.FixedTimeStamp timeStamp)
		{
			Run.FixedTimeStamp.Serialize(writer, timeStamp);
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0000D7C4 File Offset: 0x0000B9C4
		public static Run.FixedTimeStamp ReadFixedTimeStamp(this NetworkReader reader)
		{
			return Run.FixedTimeStamp.Deserialize(reader);
		}

		// Token: 0x0600036D RID: 877 RVA: 0x0000D7CC File Offset: 0x0000B9CC
		public static void WriteBitArray(this NetworkWriter writer, [NotNull] bool[] values)
		{
			writer.WriteBitArray(values, values.Length);
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0000D7D8 File Offset: 0x0000B9D8
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

		// Token: 0x0600036F RID: 879 RVA: 0x0000D845 File Offset: 0x0000BA45
		public static void ReadBitArray(this NetworkReader reader, [NotNull] bool[] values)
		{
			reader.ReadBitArray(values, values.Length);
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0000D854 File Offset: 0x0000BA54
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

		// Token: 0x06000371 RID: 881 RVA: 0x0000D703 File Offset: 0x0000B903
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WritePackedIndex32(this NetworkWriter writer, int index)
		{
			writer.WritePackedUInt32((uint)(index + 1));
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0000D70E File Offset: 0x0000B90E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ReadPackedIndex32(this NetworkReader reader)
		{
			return (int)(reader.ReadPackedUInt32() - 1U);
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0000D8BC File Offset: 0x0000BABC
		public static void Write(this NetworkWriter writer, NetworkPlayerName networkPlayerName)
		{
			networkPlayerName.Serialize(writer);
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0000D8C8 File Offset: 0x0000BAC8
		public static NetworkPlayerName ReadNetworkPlayerName(this NetworkReader reader)
		{
			NetworkPlayerName result = default(NetworkPlayerName);
			result.Deserialize(reader);
			return result;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0000D8E6 File Offset: 0x0000BAE6
		public static void Write(this NetworkWriter writer, PackedUnitVector3 value)
		{
			writer.Write(value.value);
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0000D8F4 File Offset: 0x0000BAF4
		public static PackedUnitVector3 ReadPackedUnitVector3(this NetworkReader reader)
		{
			return new PackedUnitVector3(reader.ReadUInt16());
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0000D901 File Offset: 0x0000BB01
		public static void Write(this NetworkWriter writer, PickupIndex value)
		{
			PickupIndex.WriteToNetworkWriter(writer, value);
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0000D90A File Offset: 0x0000BB0A
		public static PickupIndex ReadPickupIndex(this NetworkReader reader)
		{
			return PickupIndex.ReadFromNetworkReader(reader);
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0000D912 File Offset: 0x0000BB12
		public static void Write(this NetworkWriter writer, PitchYawPair pitchYawPair)
		{
			writer.Write(pitchYawPair.pitch);
			writer.Write(pitchYawPair.yaw);
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0000D930 File Offset: 0x0000BB30
		public static PitchYawPair ReadPitchYawPair(this NetworkReader reader)
		{
			float pitch = reader.ReadSingle();
			float yaw = reader.ReadSingle();
			return new PitchYawPair(pitch, yaw);
		}

		// Token: 0x0600037B RID: 891 RVA: 0x0000D950 File Offset: 0x0000BB50
		public static void Write(this NetworkWriter writer, RuleBook src)
		{
			src.Serialize(writer);
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0000D959 File Offset: 0x0000BB59
		public static void ReadRuleBook(this NetworkReader reader, RuleBook dest)
		{
			dest.Deserialize(reader);
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0000D962 File Offset: 0x0000BB62
		public static void Write(this NetworkWriter writer, RuleMask src)
		{
			src.Serialize(writer);
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0000D96B File Offset: 0x0000BB6B
		public static void ReadRuleMask(this NetworkReader reader, RuleMask dest)
		{
			dest.Deserialize(reader);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0000D974 File Offset: 0x0000BB74
		public static void Write(this NetworkWriter writer, RuleChoiceMask src)
		{
			src.Serialize(writer);
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0000D97D File Offset: 0x0000BB7D
		public static void ReadRuleChoiceMask(this NetworkReader reader, RuleChoiceMask dest)
		{
			dest.Deserialize(reader);
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0000D988 File Offset: 0x0000BB88
		public static void Write(this NetworkWriter writer, TeamIndex teamIndex)
		{
			byte value = (byte)(teamIndex + 1);
			writer.Write(value);
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0000D9A1 File Offset: 0x0000BBA1
		public static TeamIndex ReadTeamIndex(this NetworkReader reader)
		{
			return (TeamIndex)(reader.ReadByte() - 1);
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0000D9AC File Offset: 0x0000BBAC
		public static void Write(this NetworkWriter writer, UnlockableIndex index)
		{
			writer.Write((byte)index.internalValue);
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0000D9BC File Offset: 0x0000BBBC
		public static UnlockableIndex ReadUnlockableIndex(this NetworkReader reader)
		{
			return new UnlockableIndex
			{
				internalValue = (uint)reader.ReadByte()
			};
		}
	}
}
