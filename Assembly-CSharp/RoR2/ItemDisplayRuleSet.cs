using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000106 RID: 262
	[CreateAssetMenu]
	public class ItemDisplayRuleSet : ScriptableObject
	{
		// Token: 0x060004EE RID: 1262 RVA: 0x00013D1C File Offset: 0x00011F1C
		[SystemInitializer(new Type[]
		{
			typeof(ItemCatalog),
			typeof(EquipmentCatalog)
		})]
		private static void Init()
		{
			ItemDisplayRuleSet.runtimeDependenciesReady = true;
			for (int i = 0; i < ItemDisplayRuleSet.instancesList.Count; i++)
			{
				ItemDisplayRuleSet.instancesList[i].GenerateRuntimeValues();
			}
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x00013D54 File Offset: 0x00011F54
		private void OnEnable()
		{
			ItemDisplayRuleSet.instancesList.Add(this);
			if (ItemDisplayRuleSet.runtimeDependenciesReady)
			{
				this.GenerateRuntimeValues();
			}
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x00013D6E File Offset: 0x00011F6E
		private void OnDisable()
		{
			ItemDisplayRuleSet.instancesList.Remove(this);
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00013D7C File Offset: 0x00011F7C
		public void ConvertRuntimeValuesToSerializableValues()
		{
			List<ItemDisplayRuleSet.NamedRuleGroup> list = new List<ItemDisplayRuleSet.NamedRuleGroup>();
			int i = 0;
			int num = Math.Min(this.runtimeItemRuleGroups.Length, 99);
			while (i < num)
			{
				if (this.runtimeItemRuleGroups[i].rules.Length != 0)
				{
					List<ItemDisplayRuleSet.NamedRuleGroup> list2 = list;
					ItemDisplayRuleSet.NamedRuleGroup item = default(ItemDisplayRuleSet.NamedRuleGroup);
					ItemIndex itemIndex = (ItemIndex)i;
					item.name = itemIndex.ToString();
					item.displayRuleGroup = this.runtimeItemRuleGroups[i];
					list2.Add(item);
				}
				i++;
			}
			List<ItemDisplayRuleSet.NamedRuleGroup> list3 = new List<ItemDisplayRuleSet.NamedRuleGroup>();
			int j = 0;
			int num2 = Math.Min(this.runtimeEquipmentRuleGroups.Length, 34);
			while (j < num2)
			{
				if (this.runtimeEquipmentRuleGroups[j].rules.Length != 0)
				{
					List<ItemDisplayRuleSet.NamedRuleGroup> list4 = list3;
					ItemDisplayRuleSet.NamedRuleGroup item = default(ItemDisplayRuleSet.NamedRuleGroup);
					EquipmentIndex equipmentIndex = (EquipmentIndex)j;
					item.name = equipmentIndex.ToString();
					item.displayRuleGroup = this.runtimeEquipmentRuleGroups[j];
					list4.Add(item);
				}
				j++;
			}
			this.namedItemRuleGroups = list.ToArray();
			this.namedEquipmentRuleGroups = list3.ToArray();
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00013E8C File Offset: 0x0001208C
		private void GenerateRuntimeValues()
		{
			this.runtimeItemRuleGroups = new DisplayRuleGroup[ItemCatalog.itemCount];
			this.runtimeEquipmentRuleGroups = new DisplayRuleGroup[EquipmentCatalog.equipmentCount];
			int i = 0;
			int itemCount = ItemCatalog.itemCount;
			while (i < itemCount)
			{
				this.runtimeItemRuleGroups[i] = this.FindItemDisplayRuleGroup(ItemCatalog.GetItemDef((ItemIndex)i).name);
				i++;
			}
			int j = 0;
			int equipmentCount = EquipmentCatalog.equipmentCount;
			while (j < equipmentCount)
			{
				this.runtimeEquipmentRuleGroups[j] = this.FindEquipmentDisplayRuleGroup(EquipmentCatalog.GetEquipmentDef((EquipmentIndex)j).name);
				j++;
			}
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00013F18 File Offset: 0x00012118
		public DisplayRuleGroup FindItemDisplayRuleGroup(string itemName)
		{
			for (int i = 0; i < this.namedItemRuleGroups.Length; i++)
			{
				ref ItemDisplayRuleSet.NamedRuleGroup ptr = ref this.namedItemRuleGroups[i];
				if (ptr.name == itemName)
				{
					return ptr.displayRuleGroup;
				}
			}
			return DisplayRuleGroup.empty;
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00013F60 File Offset: 0x00012160
		public DisplayRuleGroup FindEquipmentDisplayRuleGroup(string equipmentName)
		{
			for (int i = 0; i < this.namedEquipmentRuleGroups.Length; i++)
			{
				ref ItemDisplayRuleSet.NamedRuleGroup ptr = ref this.namedEquipmentRuleGroups[i];
				if (ptr.name == equipmentName)
				{
					return ptr.displayRuleGroup;
				}
			}
			return DisplayRuleGroup.empty;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00013FA7 File Offset: 0x000121A7
		public DisplayRuleGroup GetItemDisplayRuleGroup(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= (ItemIndex)this.runtimeItemRuleGroups.Length)
			{
				return DisplayRuleGroup.empty;
			}
			return this.runtimeItemRuleGroups[(int)itemIndex];
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x00013FCC File Offset: 0x000121CC
		public void SetItemDisplayRuleGroup(string itemName, DisplayRuleGroup displayRuleGroup)
		{
			for (int i = 0; i < this.namedItemRuleGroups.Length; i++)
			{
				ref ItemDisplayRuleSet.NamedRuleGroup ptr = ref this.namedItemRuleGroups[i];
				if (ptr.name == itemName)
				{
					ptr.displayRuleGroup = displayRuleGroup;
					return;
				}
			}
			ItemDisplayRuleSet.NamedRuleGroup namedRuleGroup = new ItemDisplayRuleSet.NamedRuleGroup
			{
				name = itemName,
				displayRuleGroup = displayRuleGroup
			};
			HGArrayUtilities.ArrayAppend<ItemDisplayRuleSet.NamedRuleGroup>(ref this.namedItemRuleGroups, ref namedRuleGroup);
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x00014036 File Offset: 0x00012236
		public DisplayRuleGroup GetEquipmentDisplayRuleGroup(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= (EquipmentIndex)this.runtimeEquipmentRuleGroups.Length)
			{
				return DisplayRuleGroup.empty;
			}
			return this.runtimeEquipmentRuleGroups[(int)equipmentIndex];
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001405C File Offset: 0x0001225C
		public void SetEquipmentDisplayRuleGroup(string equipmentName, DisplayRuleGroup displayRuleGroup)
		{
			for (int i = 0; i < this.namedEquipmentRuleGroups.Length; i++)
			{
				ref ItemDisplayRuleSet.NamedRuleGroup ptr = ref this.namedEquipmentRuleGroups[i];
				if (ptr.name == equipmentName)
				{
					ptr.displayRuleGroup = displayRuleGroup;
					return;
				}
			}
			ItemDisplayRuleSet.NamedRuleGroup namedRuleGroup = new ItemDisplayRuleSet.NamedRuleGroup
			{
				name = equipmentName,
				displayRuleGroup = displayRuleGroup
			};
			HGArrayUtilities.ArrayAppend<ItemDisplayRuleSet.NamedRuleGroup>(ref this.namedEquipmentRuleGroups, ref namedRuleGroup);
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x000140C6 File Offset: 0x000122C6
		public void Reset()
		{
			this.namedItemRuleGroups = Array.Empty<ItemDisplayRuleSet.NamedRuleGroup>();
			this.namedEquipmentRuleGroups = Array.Empty<ItemDisplayRuleSet.NamedRuleGroup>();
			this.runtimeItemRuleGroups = Array.Empty<DisplayRuleGroup>();
			this.runtimeEquipmentRuleGroups = Array.Empty<DisplayRuleGroup>();
		}

		// Token: 0x040004BB RID: 1211
		[SerializeField]
		private ItemDisplayRuleSet.NamedRuleGroup[] namedItemRuleGroups = Array.Empty<ItemDisplayRuleSet.NamedRuleGroup>();

		// Token: 0x040004BC RID: 1212
		[SerializeField]
		private ItemDisplayRuleSet.NamedRuleGroup[] namedEquipmentRuleGroups = Array.Empty<ItemDisplayRuleSet.NamedRuleGroup>();

		// Token: 0x040004BD RID: 1213
		private DisplayRuleGroup[] runtimeItemRuleGroups;

		// Token: 0x040004BE RID: 1214
		private DisplayRuleGroup[] runtimeEquipmentRuleGroups;

		// Token: 0x040004BF RID: 1215
		private static readonly List<ItemDisplayRuleSet> instancesList = new List<ItemDisplayRuleSet>();

		// Token: 0x040004C0 RID: 1216
		private static bool runtimeDependenciesReady = false;

		// Token: 0x02000107 RID: 263
		[Serializable]
		public struct NamedRuleGroup
		{
			// Token: 0x040004C1 RID: 1217
			public string name;

			// Token: 0x040004C2 RID: 1218
			public DisplayRuleGroup displayRuleGroup;
		}
	}
}
