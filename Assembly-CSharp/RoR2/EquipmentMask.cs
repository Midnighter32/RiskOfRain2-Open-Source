using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000243 RID: 579
	[Serializable]
	public struct EquipmentMask
	{
		// Token: 0x06000AE1 RID: 2785 RVA: 0x00035AC7 File Offset: 0x00033CC7
		public bool HasEquipment(EquipmentIndex equipmentIndex)
		{
			return equipmentIndex >= EquipmentIndex.CommandMissile && equipmentIndex < EquipmentIndex.Count && (this.a & 1u << (int)equipmentIndex) > 0u;
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x00035AE4 File Offset: 0x00033CE4
		public void AddEquipment(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= EquipmentIndex.Count)
			{
				return;
			}
			this.a |= 1u << (int)equipmentIndex;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x00035B03 File Offset: 0x00033D03
		public void RemoveEquipment(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= EquipmentIndex.Count)
			{
				return;
			}
			this.a &= ~(1u << (int)equipmentIndex);
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x00035B24 File Offset: 0x00033D24
		public static EquipmentMask operator &(EquipmentMask mask1, EquipmentMask mask2)
		{
			return new EquipmentMask
			{
				a = (mask1.a & mask2.a)
			};
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x00035B50 File Offset: 0x00033D50
		static EquipmentMask()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				EquipmentMask.all.AddEquipment(equipmentIndex);
			}
		}

		// Token: 0x04000EC9 RID: 3785
		[SerializeField]
		public uint a;

		// Token: 0x04000ECA RID: 3786
		public static readonly EquipmentMask none;

		// Token: 0x04000ECB RID: 3787
		public static readonly EquipmentMask all = default(EquipmentMask);
	}
}
