using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000226 RID: 550
	[CreateAssetMenu]
	public class ItemDisplayRuleSet : ScriptableObject
	{
		// Token: 0x06000AA7 RID: 2727 RVA: 0x00034B94 File Offset: 0x00032D94
		public DisplayRuleGroup GetItemDisplayRuleGroup(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= (ItemIndex)this.itemRuleGroups.Length)
			{
				return new DisplayRuleGroup
				{
					rules = null
				};
			}
			return this.itemRuleGroups[(int)itemIndex];
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x00034BCE File Offset: 0x00032DCE
		public void SetItemDisplayRuleGroup(ItemIndex itemIndex, DisplayRuleGroup displayRuleGroup)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= (ItemIndex)this.itemRuleGroups.Length)
			{
				return;
			}
			this.itemRuleGroups[(int)itemIndex] = displayRuleGroup;
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x00034BF0 File Offset: 0x00032DF0
		public DisplayRuleGroup GetEquipmentDisplayRuleGroup(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= (EquipmentIndex)this.equipmentRuleGroups.Length)
			{
				return new DisplayRuleGroup
				{
					rules = null
				};
			}
			return this.equipmentRuleGroups[(int)equipmentIndex];
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x00034C2A File Offset: 0x00032E2A
		public void SetEquipmentDisplayRuleGroup(EquipmentIndex equipmentIndex, DisplayRuleGroup displayRuleGroup)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= (EquipmentIndex)this.equipmentRuleGroups.Length)
			{
				return;
			}
			this.equipmentRuleGroups[(int)equipmentIndex] = displayRuleGroup;
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x00034C49 File Offset: 0x00032E49
		public void Reset()
		{
			this.itemRuleGroups = new DisplayRuleGroup[78];
			this.equipmentRuleGroups = new DisplayRuleGroup[27];
		}

		// Token: 0x04000E12 RID: 3602
		[SerializeField]
		[FormerlySerializedAs("ruleGroups")]
		private DisplayRuleGroup[] itemRuleGroups = new DisplayRuleGroup[78];

		// Token: 0x04000E13 RID: 3603
		[SerializeField]
		private DisplayRuleGroup[] equipmentRuleGroups = new DisplayRuleGroup[27];
	}
}
