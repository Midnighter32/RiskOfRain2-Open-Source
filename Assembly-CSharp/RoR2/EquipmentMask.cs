using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000131 RID: 305
	[Serializable]
	public struct EquipmentMask
	{
		// Token: 0x06000576 RID: 1398 RVA: 0x000161BC File Offset: 0x000143BC
		public bool HasEquipment(EquipmentIndex equipmentIndex)
		{
			return EquipmentCatalog.IsIndexValid(equipmentIndex) && (this.a & 1UL << (int)equipmentIndex) > 0UL;
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x000161DB File Offset: 0x000143DB
		public void AddEquipment(EquipmentIndex equipmentIndex)
		{
			if (!EquipmentCatalog.IsIndexValid(equipmentIndex))
			{
				return;
			}
			this.a |= 1UL << (int)equipmentIndex;
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x000161FB File Offset: 0x000143FB
		public void RemoveEquipment(EquipmentIndex equipmentIndex)
		{
			if (!EquipmentCatalog.IsIndexValid(equipmentIndex))
			{
				return;
			}
			this.a &= ~(1UL << (int)equipmentIndex);
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x0001621C File Offset: 0x0001441C
		public static EquipmentMask operator &(EquipmentMask mask1, EquipmentMask mask2)
		{
			return new EquipmentMask
			{
				a = (mask1.a & mask2.a)
			};
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x00016248 File Offset: 0x00014448
		[SystemInitializer(new Type[]
		{
			typeof(EquipmentCatalog)
		})]
		private static void Init()
		{
			EquipmentMask.all = default(EquipmentMask);
			EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile;
			EquipmentIndex equipmentCount = (EquipmentIndex)EquipmentCatalog.equipmentCount;
			while (equipmentIndex < equipmentCount)
			{
				EquipmentMask.all.AddEquipment(equipmentIndex);
				equipmentIndex++;
			}
		}

		// Token: 0x040005DB RID: 1499
		[SerializeField]
		public ulong a;

		// Token: 0x040005DC RID: 1500
		private const ulong maskOne = 1UL;

		// Token: 0x040005DD RID: 1501
		public static EquipmentMask none;

		// Token: 0x040005DE RID: 1502
		public static EquipmentMask all;
	}
}
