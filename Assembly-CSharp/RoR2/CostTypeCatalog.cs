using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000FA RID: 250
	public static class CostTypeCatalog
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060004C3 RID: 1219 RVA: 0x00012D3C File Offset: 0x00010F3C
		public static int costTypeCount
		{
			get
			{
				return CostTypeCatalog.costTypeDefs.Length;
			}
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x00012D45 File Offset: 0x00010F45
		private static void Register(CostTypeIndex costType, CostTypeDef costTypeDef)
		{
			if (costType < CostTypeIndex.Count)
			{
				costTypeDef.name = costType.ToString();
			}
			CostTypeCatalog.costTypeDefs[(int)costType] = costTypeDef;
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x00012D68 File Offset: 0x00010F68
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			CostTypeCatalog.costTypeDefs = new CostTypeDef[11];
			CostTypeIndex costType = CostTypeIndex.None;
			CostTypeDef costTypeDef7 = new CostTypeDef();
			costTypeDef7.buildCostString = delegate(CostTypeDef costTypeDef, CostTypeDef.BuildCostStringContext context)
			{
				context.stringBuilder.Append("");
			};
			costTypeDef7.isAffordable = ((CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context) => true);
			costTypeDef7.payCost = delegate(CostTypeDef costTypeDef, CostTypeDef.PayCostContext context)
			{
			};
			CostTypeCatalog.Register(costType, costTypeDef7);
			CostTypeIndex costType2 = CostTypeIndex.Money;
			CostTypeDef costTypeDef2 = new CostTypeDef();
			costTypeDef2.costStringFormatToken = "COST_MONEY_FORMAT";
			costTypeDef2.isAffordable = delegate(CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context)
			{
				CharacterBody component = context.activator.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						return (ulong)master.money >= (ulong)((long)context.cost);
					}
				}
				return false;
			};
			costTypeDef2.payCost = delegate(CostTypeDef costTypeDef, CostTypeDef.PayCostContext context)
			{
				if (context.activatorMaster)
				{
					context.activatorMaster.money -= (uint)context.cost;
				}
			};
			costTypeDef2.colorIndex = ColorCatalog.ColorIndex.Money;
			CostTypeCatalog.Register(costType2, costTypeDef2);
			CostTypeIndex costType3 = CostTypeIndex.PercentHealth;
			CostTypeDef costTypeDef3 = new CostTypeDef();
			costTypeDef3.costStringFormatToken = "COST_PERCENTHEALTH_FORMAT";
			costTypeDef3.saturateWorldStyledCostString = false;
			costTypeDef3.darkenWorldStyledCostString = true;
			costTypeDef3.isAffordable = delegate(CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context)
			{
				HealthComponent component = context.activator.GetComponent<HealthComponent>();
				return component && component.combinedHealth / component.fullCombinedHealth * 100f >= (float)context.cost;
			};
			costTypeDef3.payCost = delegate(CostTypeDef costTypeDef, CostTypeDef.PayCostContext context)
			{
				HealthComponent component = context.activator.GetComponent<HealthComponent>();
				if (component)
				{
					float combinedHealth = component.combinedHealth;
					float num = component.fullCombinedHealth * (float)context.cost / 100f;
					if (combinedHealth > num)
					{
						component.TakeDamage(new DamageInfo
						{
							damage = num,
							attacker = context.purchasedObject,
							position = context.purchasedObject.transform.position,
							damageType = (DamageType.NonLethal | DamageType.BypassArmor)
						});
					}
				}
			};
			costTypeDef3.colorIndex = ColorCatalog.ColorIndex.Blood;
			CostTypeCatalog.Register(costType3, costTypeDef3);
			CostTypeIndex costType4 = CostTypeIndex.LunarCoin;
			CostTypeDef costTypeDef4 = new CostTypeDef();
			costTypeDef4.costStringFormatToken = "COST_LUNARCOIN_FORMAT";
			costTypeDef4.saturateWorldStyledCostString = false;
			costTypeDef4.darkenWorldStyledCostString = true;
			costTypeDef4.isAffordable = delegate(CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context)
			{
				NetworkUser networkUser = Util.LookUpBodyNetworkUser(context.activator.gameObject);
				return networkUser && (ulong)networkUser.lunarCoins >= (ulong)((long)context.cost);
			};
			costTypeDef4.payCost = delegate(CostTypeDef costTypeDef, CostTypeDef.PayCostContext context)
			{
				NetworkUser networkUser = Util.LookUpBodyNetworkUser(context.activator.gameObject);
				if (networkUser)
				{
					networkUser.DeductLunarCoins((uint)context.cost);
				}
			};
			costTypeDef4.colorIndex = ColorCatalog.ColorIndex.LunarCoin;
			CostTypeCatalog.Register(costType4, costTypeDef4);
			CostTypeCatalog.Register(CostTypeIndex.WhiteItem, new CostTypeDef
			{
				costStringFormatToken = "COST_ITEM_FORMAT",
				isAffordable = new CostTypeDef.IsAffordableDelegate(CostTypeCatalog.<>c.<>9.<Init>g__IsAffordableItem|5_0),
				payCost = new CostTypeDef.PayCostDelegate(CostTypeCatalog.<>c.<>9.<Init>g__PayCostItems|5_1),
				colorIndex = ColorCatalog.ColorIndex.Tier1Item,
				itemTier = ItemTier.Tier1
			});
			CostTypeCatalog.Register(CostTypeIndex.GreenItem, new CostTypeDef
			{
				costStringFormatToken = "COST_ITEM_FORMAT",
				saturateWorldStyledCostString = true,
				isAffordable = new CostTypeDef.IsAffordableDelegate(CostTypeCatalog.<>c.<>9.<Init>g__IsAffordableItem|5_0),
				payCost = new CostTypeDef.PayCostDelegate(CostTypeCatalog.<>c.<>9.<Init>g__PayCostItems|5_1),
				colorIndex = ColorCatalog.ColorIndex.Tier2Item,
				itemTier = ItemTier.Tier2
			});
			CostTypeCatalog.Register(CostTypeIndex.RedItem, new CostTypeDef
			{
				costStringFormatToken = "COST_ITEM_FORMAT",
				saturateWorldStyledCostString = false,
				darkenWorldStyledCostString = true,
				isAffordable = new CostTypeDef.IsAffordableDelegate(CostTypeCatalog.<>c.<>9.<Init>g__IsAffordableItem|5_0),
				payCost = new CostTypeDef.PayCostDelegate(CostTypeCatalog.<>c.<>9.<Init>g__PayCostItems|5_1),
				colorIndex = ColorCatalog.ColorIndex.Tier3Item,
				itemTier = ItemTier.Tier3
			});
			CostTypeCatalog.Register(CostTypeIndex.BossItem, new CostTypeDef
			{
				costStringFormatToken = "COST_ITEM_FORMAT",
				darkenWorldStyledCostString = true,
				isAffordable = new CostTypeDef.IsAffordableDelegate(CostTypeCatalog.<>c.<>9.<Init>g__IsAffordableItem|5_0),
				payCost = new CostTypeDef.PayCostDelegate(CostTypeCatalog.<>c.<>9.<Init>g__PayCostItems|5_1),
				colorIndex = ColorCatalog.ColorIndex.BossItem,
				itemTier = ItemTier.Boss
			});
			CostTypeIndex costType5 = CostTypeIndex.Equipment;
			CostTypeDef costTypeDef5 = new CostTypeDef();
			costTypeDef5.costStringFormatToken = "COST_EQUIPMENT_FORMAT";
			costTypeDef5.isAffordable = delegate(CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context)
			{
				CharacterBody component = context.activator.gameObject.GetComponent<CharacterBody>();
				if (component)
				{
					Inventory inventory = component.inventory;
					if (inventory)
					{
						return inventory.currentEquipmentIndex != EquipmentIndex.None;
					}
				}
				return false;
			};
			costTypeDef5.payCost = new CostTypeDef.PayCostDelegate(CostTypeCatalog.<>c.<>9.<Init>g__PayEquipment|5_2);
			costTypeDef5.colorIndex = ColorCatalog.ColorIndex.Equipment;
			CostTypeCatalog.Register(costType5, costTypeDef5);
			CostTypeIndex costType6 = CostTypeIndex.VolatileBattery;
			CostTypeDef costTypeDef6 = new CostTypeDef();
			costTypeDef6.costStringFormatToken = "COST_VOLATILEBATTERY_FORMAT";
			costTypeDef6.isAffordable = delegate(CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context)
			{
				CharacterBody component = context.activator.gameObject.GetComponent<CharacterBody>();
				if (component)
				{
					Inventory inventory = component.inventory;
					if (inventory)
					{
						return inventory.currentEquipmentIndex == EquipmentIndex.QuestVolatileBattery;
					}
				}
				return false;
			};
			costTypeDef6.payCost = new CostTypeDef.PayCostDelegate(CostTypeCatalog.<>c.<>9.<Init>g__PayEquipment|5_2);
			costTypeDef6.colorIndex = ColorCatalog.ColorIndex.Equipment;
			CostTypeCatalog.Register(costType6, costTypeDef6);
			CostTypeCatalog.Register(CostTypeIndex.LunarItemOrEquipment, new CostTypeDef
			{
				costStringFormatToken = "COST_LUNAR_FORMAT",
				isAffordable = new CostTypeDef.IsAffordableDelegate(CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.IsAffordable),
				payCost = new CostTypeDef.PayCostDelegate(CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.PayCost),
				colorIndex = ColorCatalog.ColorIndex.LunarItem
			});
			CostTypeCatalog.modHelper.CollectAndRegisterAdditionalEntries(ref CostTypeCatalog.costTypeDefs);
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x000131AD File Offset: 0x000113AD
		public static CostTypeDef GetCostTypeDef(CostTypeIndex costTypeIndex)
		{
			if (costTypeIndex >= CostTypeIndex.Count)
			{
				return null;
			}
			return CostTypeCatalog.costTypeDefs[(int)costTypeIndex];
		}

		// Token: 0x04000492 RID: 1170
		private static CostTypeDef[] costTypeDefs;

		// Token: 0x04000493 RID: 1171
		public static readonly CatalogModHelper<CostTypeDef> modHelper = new CatalogModHelper<CostTypeDef>(delegate(int i, CostTypeDef def)
		{
			CostTypeCatalog.Register((CostTypeIndex)i, def);
		}, (CostTypeDef v) => v.name);

		// Token: 0x020000FB RID: 251
		private static class LunarItemOrEquipmentCostTypeHelper
		{
			// Token: 0x060004C8 RID: 1224 RVA: 0x000131EC File Offset: 0x000113EC
			public static bool IsAffordable(CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context)
			{
				CharacterBody component = context.activator.GetComponent<CharacterBody>();
				if (!component)
				{
					return false;
				}
				Inventory inventory = component.inventory;
				if (!inventory)
				{
					return false;
				}
				int cost = context.cost;
				int num = 0;
				for (int i = 0; i < CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices.Length; i++)
				{
					int itemCount = inventory.GetItemCount(CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices[i]);
					num += itemCount;
					if (num >= cost)
					{
						return true;
					}
				}
				int j = 0;
				int equipmentSlotCount = inventory.GetEquipmentSlotCount();
				while (j < equipmentSlotCount)
				{
					EquipmentState equipment = inventory.GetEquipment((uint)j);
					for (int k = 0; k < CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarEquipmentIndices.Length; k++)
					{
						if (equipment.equipmentIndex == CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarEquipmentIndices[k])
						{
							num++;
							if (num >= cost)
							{
								return true;
							}
						}
					}
					j++;
				}
				return false;
			}

			// Token: 0x060004C9 RID: 1225 RVA: 0x000132B0 File Offset: 0x000114B0
			public static void PayCost(CostTypeDef costTypeDef, CostTypeDef.PayCostContext context)
			{
				CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.<>c__DisplayClass3_0 CS$<>8__locals1;
				CS$<>8__locals1.context = context;
				CS$<>8__locals1.inventory = CS$<>8__locals1.context.activator.GetComponent<CharacterBody>().inventory;
				int cost = CS$<>8__locals1.context.cost;
				CS$<>8__locals1.itemWeight = 0;
				CS$<>8__locals1.equipmentWeight = 0;
				for (int i = 0; i < CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices.Length; i++)
				{
					ItemIndex itemIndex = CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices[i];
					int itemCount = CS$<>8__locals1.inventory.GetItemCount(itemIndex);
					CS$<>8__locals1.itemWeight += itemCount;
				}
				int j = 0;
				int equipmentSlotCount = CS$<>8__locals1.inventory.GetEquipmentSlotCount();
				while (j < equipmentSlotCount)
				{
					EquipmentState equipment = CS$<>8__locals1.inventory.GetEquipment((uint)j);
					if (Array.IndexOf<EquipmentIndex>(CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarEquipmentIndices, equipment.equipmentIndex) != -1)
					{
						int equipmentWeight = CS$<>8__locals1.equipmentWeight + 1;
						CS$<>8__locals1.equipmentWeight = equipmentWeight;
					}
					j++;
				}
				CS$<>8__locals1.totalWeight = CS$<>8__locals1.itemWeight + CS$<>8__locals1.equipmentWeight;
				for (int k = 0; k < cost; k++)
				{
					CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.<PayCost>g__TakeOne|3_0(ref CS$<>8__locals1);
				}
			}

			// Token: 0x060004CA RID: 1226 RVA: 0x000133B4 File Offset: 0x000115B4
			private static void PayOne(Inventory inventory)
			{
				new WeightedSelection<ItemIndex>(CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices.Length);
				new WeightedSelection<uint>(inventory.GetEquipmentSlotCount());
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices.Length; i++)
				{
					ItemIndex itemIndex = CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices[i];
					int itemCount = inventory.GetItemCount(itemIndex);
					num += itemCount;
				}
				int j = 0;
				int equipmentSlotCount = inventory.GetEquipmentSlotCount();
				while (j < equipmentSlotCount)
				{
					EquipmentState equipment = inventory.GetEquipment((uint)j);
					if (Array.IndexOf<EquipmentIndex>(CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarEquipmentIndices, equipment.equipmentIndex) != -1)
					{
						num2++;
					}
					j++;
				}
			}

			// Token: 0x060004CB RID: 1227 RVA: 0x00013444 File Offset: 0x00011644
			[SystemInitializer(new Type[]
			{
				typeof(ItemCatalog),
				typeof(EquipmentCatalog)
			})]
			private static void Init()
			{
				CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices = ItemCatalog.lunarItemList.ToArray();
				CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarEquipmentIndices = (from v in EquipmentCatalog.equipmentList
				where EquipmentCatalog.GetEquipmentDef(v).isLunar
				select v).ToArray<EquipmentIndex>();
			}

			// Token: 0x060004CD RID: 1229 RVA: 0x000134AC File Offset: 0x000116AC
			[CompilerGenerated]
			internal static void <PayCost>g__TakeOne|3_0(ref CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.<>c__DisplayClass3_0 A_0)
			{
				float nextNormalizedFloat = A_0.context.rng.nextNormalizedFloat;
				float num = (float)A_0.itemWeight / (float)A_0.totalWeight;
				if (nextNormalizedFloat < num)
				{
					int num2 = Mathf.FloorToInt(Util.Remap(Util.Remap(nextNormalizedFloat, 0f, num, 0f, 1f), 0f, 1f, 0f, (float)A_0.itemWeight));
					int num3 = 0;
					for (int i = 0; i < CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices.Length; i++)
					{
						ItemIndex itemIndex = CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarItemIndices[i];
						int itemCount = A_0.inventory.GetItemCount(itemIndex);
						num3 += itemCount;
						if (num2 < num3)
						{
							A_0.inventory.RemoveItem(itemIndex, 1);
							A_0.context.results.itemsTaken.Add(itemIndex);
							return;
						}
					}
					return;
				}
				int num4 = Mathf.FloorToInt(Util.Remap(Util.Remap(nextNormalizedFloat, num, 1f, 0f, 1f), 0f, 1f, 0f, (float)A_0.equipmentWeight));
				int num5 = 0;
				for (int j = 0; j < A_0.inventory.GetEquipmentSlotCount(); j++)
				{
					EquipmentIndex equipmentIndex = A_0.inventory.GetEquipment((uint)j).equipmentIndex;
					if (Array.IndexOf<EquipmentIndex>(CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.lunarEquipmentIndices, equipmentIndex) != -1)
					{
						num5++;
						if (num4 < num5)
						{
							A_0.inventory.SetEquipment(EquipmentState.empty, (uint)j);
							A_0.context.results.equipmentTaken.Add(equipmentIndex);
						}
					}
				}
			}

			// Token: 0x04000494 RID: 1172
			private static ItemIndex[] lunarItemIndices = Array.Empty<ItemIndex>();

			// Token: 0x04000495 RID: 1173
			private static EquipmentIndex[] lunarEquipmentIndices = Array.Empty<EquipmentIndex>();
		}
	}
}
