using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033E RID: 830
	public struct EquipmentState : IEquatable<EquipmentState>
	{
		// Token: 0x06001130 RID: 4400 RVA: 0x00055990 File Offset: 0x00053B90
		public EquipmentState(EquipmentIndex equipmentIndex, Run.FixedTimeStamp chargeFinishTime, byte charges)
		{
			this.equipmentIndex = equipmentIndex;
			this.chargeFinishTime = chargeFinishTime;
			this.charges = charges;
			this.dirty = true;
			this.equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x000559BC File Offset: 0x00053BBC
		public bool Equals(EquipmentState other)
		{
			return this.equipmentIndex == other.equipmentIndex && this.chargeFinishTime.Equals(other.chargeFinishTime) && this.charges == other.charges;
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x000559FD File Offset: 0x00053BFD
		public override bool Equals(object obj)
		{
			return obj != null && obj is EquipmentState && this.Equals((EquipmentState)obj);
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00055A1C File Offset: 0x00053C1C
		public override int GetHashCode()
		{
			return (int)(this.equipmentIndex * (EquipmentIndex)397 ^ (EquipmentIndex)this.chargeFinishTime.GetHashCode());
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x00055A4C File Offset: 0x00053C4C
		public static EquipmentState Deserialize(NetworkReader reader)
		{
			EquipmentIndex equipmentIndex = reader.ReadEquipmentIndex();
			Run.FixedTimeStamp fixedTimeStamp = reader.ReadFixedTimeStamp();
			byte b = reader.ReadByte();
			return new EquipmentState(equipmentIndex, fixedTimeStamp, b);
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x00055A74 File Offset: 0x00053C74
		public static void Serialize(NetworkWriter writer, EquipmentState equipmentState)
		{
			writer.Write(equipmentState.equipmentIndex);
			writer.Write(equipmentState.chargeFinishTime);
			writer.Write(equipmentState.charges);
		}

		// Token: 0x04001543 RID: 5443
		public readonly EquipmentIndex equipmentIndex;

		// Token: 0x04001544 RID: 5444
		public readonly Run.FixedTimeStamp chargeFinishTime;

		// Token: 0x04001545 RID: 5445
		public readonly byte charges;

		// Token: 0x04001546 RID: 5446
		public bool dirty;

		// Token: 0x04001547 RID: 5447
		[CanBeNull]
		public readonly EquipmentDef equipmentDef;

		// Token: 0x04001548 RID: 5448
		public static readonly EquipmentState empty = new EquipmentState(EquipmentIndex.None, Run.FixedTimeStamp.negativeInfinity, 0);
	}
}
