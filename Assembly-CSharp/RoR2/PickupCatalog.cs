using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003DE RID: 990
	public static class PickupCatalog
	{
		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06001800 RID: 6144 RVA: 0x0006870D File Offset: 0x0006690D
		// (set) Token: 0x06001801 RID: 6145 RVA: 0x00068714 File Offset: 0x00066914
		public static int pickupCount { get; private set; }

		// Token: 0x06001802 RID: 6146 RVA: 0x0006871C File Offset: 0x0006691C
		public static T[] GetPerPickupBuffer<T>()
		{
			return new T[PickupCatalog.pickupCount];
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x00068728 File Offset: 0x00066928
		public static void SetEntries(PickupDef[] pickupDefs)
		{
			Array.Resize<PickupDef>(ref PickupCatalog.entries, pickupDefs.Length);
			PickupCatalog.pickupCount = pickupDefs.Length;
			Array.Copy(pickupDefs, PickupCatalog.entries, PickupCatalog.entries.Length);
			Array.Resize<PickupIndex>(ref PickupCatalog.itemIndexToPickupIndex, ItemCatalog.itemCount);
			Array.Resize<PickupIndex>(ref PickupCatalog.equipmentIndexToPickupIndex, EquipmentCatalog.equipmentCount);
			PickupCatalog.nameToPickupIndex.Clear();
			for (int i = 0; i < PickupCatalog.entries.Length; i++)
			{
				PickupDef pickupDef = PickupCatalog.entries[i];
				PickupIndex pickupIndex = new PickupIndex(i);
				pickupDef.pickupIndex = pickupIndex;
				if (pickupDef.itemIndex != ItemIndex.None)
				{
					PickupCatalog.itemIndexToPickupIndex[(int)pickupDef.itemIndex] = pickupIndex;
				}
				if (pickupDef.equipmentIndex != EquipmentIndex.None)
				{
					PickupCatalog.equipmentIndexToPickupIndex[(int)pickupDef.equipmentIndex] = pickupIndex;
				}
			}
			for (int j = 0; j < PickupCatalog.entries.Length; j++)
			{
				PickupDef pickupDef2 = PickupCatalog.entries[j];
				PickupCatalog.nameToPickupIndex[pickupDef2.internalName] = pickupDef2.pickupIndex;
			}
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x00068814 File Offset: 0x00066A14
		[SystemInitializer(new Type[]
		{
			typeof(ItemCatalog),
			typeof(EquipmentCatalog)
		})]
		private static void Init()
		{
			List<PickupDef> list = new List<PickupDef>();
			for (int i = 0; i < ItemCatalog.itemCount; i++)
			{
				ItemIndex itemIndex = (ItemIndex)i;
				ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
				PickupDef pickupDef = new PickupDef();
				PickupDef pickupDef2 = pickupDef;
				string str = "ItemIndex.";
				ItemIndex itemIndex2 = (ItemIndex)i;
				pickupDef2.internalName = str + itemIndex2.ToString();
				pickupDef.itemIndex = itemIndex;
				pickupDef.displayPrefab = itemDef.pickupModelPrefab;
				pickupDef.dropletDisplayPrefab = PickupCatalog.<Init>g__GetPickupDropletDisplayPrefabForTier|11_0(itemDef.tier);
				pickupDef.nameToken = itemDef.nameToken;
				pickupDef.baseColor = ColorCatalog.GetColor(itemDef.colorIndex);
				pickupDef.darkColor = ColorCatalog.GetColor(itemDef.darkColorIndex);
				pickupDef.unlockableName = itemDef.unlockableName;
				pickupDef.interactContextToken = "ITEM_PICKUP_CONTEXT";
				pickupDef.isLunar = (itemDef.tier == ItemTier.Lunar);
				pickupDef.isBoss = (itemDef.tier == ItemTier.Boss);
				pickupDef.iconTexture = itemDef.pickupIconTexture;
				list.Add(pickupDef);
			}
			GameObject dropletDisplayPrefab = Resources.Load<GameObject>("Prefabs/ItemPickups/EquipmentOrb");
			for (int j = 0; j < EquipmentCatalog.equipmentCount; j++)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)j;
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
				PickupDef pickupDef3 = new PickupDef();
				PickupDef pickupDef4 = pickupDef3;
				string str2 = "EquipmentIndex.";
				EquipmentIndex equipmentIndex2 = (EquipmentIndex)j;
				pickupDef4.internalName = str2 + equipmentIndex2.ToString();
				pickupDef3.equipmentIndex = equipmentIndex;
				pickupDef3.displayPrefab = equipmentDef.pickupModelPrefab;
				pickupDef3.dropletDisplayPrefab = dropletDisplayPrefab;
				pickupDef3.nameToken = equipmentDef.nameToken;
				pickupDef3.baseColor = ColorCatalog.GetColor(equipmentDef.colorIndex);
				pickupDef3.darkColor = pickupDef3.baseColor;
				pickupDef3.unlockableName = equipmentDef.unlockableName;
				pickupDef3.interactContextToken = "EQUIPMENT_PICKUP_CONTEXT";
				pickupDef3.isLunar = equipmentDef.isLunar;
				pickupDef3.isBoss = equipmentDef.isBoss;
				pickupDef3.iconTexture = equipmentDef.pickupIconTexture;
				list.Add(pickupDef3);
			}
			PickupDef pickupDef5 = new PickupDef();
			pickupDef5.internalName = "LunarCoin.Coin0";
			pickupDef5.coinValue = 1U;
			pickupDef5.nameToken = "PICKUP_LUNAR_COIN";
			pickupDef5.displayPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupLunarCoin");
			pickupDef5.dropletDisplayPrefab = Resources.Load<GameObject>("Prefabs/ItemPickups/LunarOrb");
			pickupDef5.baseColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem);
			pickupDef5.darkColor = pickupDef5.baseColor;
			pickupDef5.interactContextToken = "LUNAR_COIN_PICKUP_CONTEXT";
			list.Add(pickupDef5);
			Action<List<PickupDef>> action = PickupCatalog.modifyPickups;
			if (action != null)
			{
				action(list);
			}
			PickupCatalog.SetEntries(list.ToArray());
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x00068AB4 File Offset: 0x00066CB4
		public static PickupIndex FindPickupIndex(string pickupName)
		{
			PickupIndex result;
			if (PickupCatalog.nameToPickupIndex.TryGetValue(pickupName, out result))
			{
				return result;
			}
			return PickupIndex.none;
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x00068AD7 File Offset: 0x00066CD7
		public static PickupIndex FindPickupIndex(ItemIndex itemIndex)
		{
			return HGArrayUtilities.GetSafe<PickupIndex>(PickupCatalog.itemIndexToPickupIndex, (int)itemIndex, PickupIndex.none);
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x00068AE9 File Offset: 0x00066CE9
		public static PickupIndex FindPickupIndex(EquipmentIndex equipmentIndex)
		{
			return HGArrayUtilities.GetSafe<PickupIndex>(PickupCatalog.equipmentIndexToPickupIndex, (int)equipmentIndex, PickupIndex.none);
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x00068AFB File Offset: 0x00066CFB
		public static PickupDef GetPickupDef(PickupIndex pickupIndex)
		{
			return HGArrayUtilities.GetSafe<PickupDef>(PickupCatalog.entries, pickupIndex.value);
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x00068B0D File Offset: 0x00066D0D
		public static GameObject GetHiddenPickupDisplayPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x00068B1C File Offset: 0x00066D1C
		[ConCommand(commandName = "pickup_print_all", flags = ConVarFlags.None, helpText = "Prints all pickup definitions.")]
		private static void CCPickupPrintAll(ConCommandArgs args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < PickupCatalog.pickupCount; i++)
			{
				PickupDef pickupDef = PickupCatalog.GetPickupDef(new PickupIndex(i));
				stringBuilder.Append("[").Append(i).Append("]={internalName=").Append(pickupDef.internalName).Append("}").AppendLine();
			}
			Debug.Log(stringBuilder.ToString());
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x00068BB8 File Offset: 0x00066DB8
		[CompilerGenerated]
		internal static GameObject <Init>g__GetPickupDropletDisplayPrefabForTier|11_0(ItemTier tier)
		{
			string text = null;
			switch (tier)
			{
			case ItemTier.Tier1:
				text = "Prefabs/ItemPickups/Tier1Orb";
				break;
			case ItemTier.Tier2:
				text = "Prefabs/ItemPickups/Tier2Orb";
				break;
			case ItemTier.Tier3:
				text = "Prefabs/ItemPickups/Tier3Orb";
				break;
			case ItemTier.Lunar:
				text = "Prefabs/ItemPickups/LunarOrb";
				break;
			case ItemTier.Boss:
				text = "Prefabs/ItemPickups/BossOrb";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				return Resources.Load<GameObject>(text);
			}
			return null;
		}

		// Token: 0x040016B5 RID: 5813
		private static PickupDef[] entries = Array.Empty<PickupDef>();

		// Token: 0x040016B7 RID: 5815
		private static PickupIndex[] itemIndexToPickupIndex = Array.Empty<PickupIndex>();

		// Token: 0x040016B8 RID: 5816
		private static PickupIndex[] equipmentIndexToPickupIndex = Array.Empty<PickupIndex>();

		// Token: 0x040016B9 RID: 5817
		private static readonly Dictionary<string, PickupIndex> nameToPickupIndex = new Dictionary<string, PickupIndex>();

		// Token: 0x040016BA RID: 5818
		public static Action<List<PickupDef>> modifyPickups;
	}
}
