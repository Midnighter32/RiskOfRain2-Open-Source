using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000262 RID: 610
	public struct EquipmentState : IEquatable<EquipmentState>
	{
		// Token: 0x06000D88 RID: 3464 RVA: 0x0003CD18 File Offset: 0x0003AF18
		public EquipmentState(EquipmentIndex equipmentIndex, Run.FixedTimeStamp chargeFinishTime, byte charges)
		{
			this.equipmentIndex = equipmentIndex;
			this.chargeFinishTime = chargeFinishTime;
			this.charges = charges;
			this.dirty = true;
			this.equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x0003CD44 File Offset: 0x0003AF44
		public bool Equals(EquipmentState other)
		{
			return this.equipmentIndex == other.equipmentIndex && this.chargeFinishTime.Equals(other.chargeFinishTime) && this.charges == other.charges;
		}

		// Token: 0x06000D8A RID: 3466 RVA: 0x0003CD85 File Offset: 0x0003AF85
		public override bool Equals(object obj)
		{
			return obj != null && obj is EquipmentState && this.Equals((EquipmentState)obj);
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x0003CDA4 File Offset: 0x0003AFA4
		public override int GetHashCode()
		{
			return (int)(this.equipmentIndex * (EquipmentIndex)397 ^ (EquipmentIndex)this.chargeFinishTime.GetHashCode());
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x0003CDD4 File Offset: 0x0003AFD4
		public static EquipmentState Deserialize(NetworkReader reader)
		{
			EquipmentIndex equipmentIndex = reader.ReadEquipmentIndex();
			Run.FixedTimeStamp fixedTimeStamp = reader.ReadFixedTimeStamp();
			byte b = reader.ReadByte();
			return new EquipmentState(equipmentIndex, fixedTimeStamp, b);
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x0003CDFC File Offset: 0x0003AFFC
		public static void Serialize(NetworkWriter writer, EquipmentState equipmentState)
		{
			writer.Write(equipmentState.equipmentIndex);
			writer.Write(equipmentState.chargeFinishTime);
			writer.Write(equipmentState.charges);
		}

		// Token: 0x04000D92 RID: 3474
		public readonly EquipmentIndex equipmentIndex;

		// Token: 0x04000D93 RID: 3475
		public readonly Run.FixedTimeStamp chargeFinishTime;

		// Token: 0x04000D94 RID: 3476
		public readonly byte charges;

		// Token: 0x04000D95 RID: 3477
		public bool dirty;

		// Token: 0x04000D96 RID: 3478
		[CanBeNull]
		public readonly EquipmentDef equipmentDef;

		// Token: 0x04000D97 RID: 3479
		public static readonly EquipmentState empty = new EquipmentState(EquipmentIndex.None, Run.FixedTimeStamp.negativeInfinity, 0);
	}
}
