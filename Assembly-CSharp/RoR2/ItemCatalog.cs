using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200044B RID: 1099
	public static class ItemCatalog
	{
		// Token: 0x06001869 RID: 6249 RVA: 0x000742D8 File Offset: 0x000724D8
		static ItemCatalog()
		{
			ItemCatalog.DefineItems();
			HGXml.Register<ItemIndex[]>(delegate(XElement element, ItemIndex[] obj)
			{
				element.Value = string.Join(" ", from v in obj
				select v.ToString());
			}, delegate(XElement element, ref ItemIndex[] output)
			{
				output = element.Value.Split(new char[]
				{
					' '
				}).Select(delegate(string v)
				{
					ItemIndex result;
					if (!Enum.TryParse<ItemIndex>(v, false, out result))
					{
						return ItemIndex.None;
					}
					return result;
				}).ToArray<ItemIndex>();
				return true;
			});
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x0007434B File Offset: 0x0007254B
		public static ItemIndex[] RequestItemOrderBuffer()
		{
			if (ItemCatalog.itemOrderBuffers.Count > 0)
			{
				return ItemCatalog.itemOrderBuffers.Pop();
			}
			return new ItemIndex[78];
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x0007436C File Offset: 0x0007256C
		public static void ReturnItemOrderBuffer(ItemIndex[] buffer)
		{
			ItemCatalog.itemOrderBuffers.Push(buffer);
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x00074379 File Offset: 0x00072579
		public static int[] RequestItemStackArray()
		{
			if (ItemCatalog.itemStackArrays.Count > 0)
			{
				return ItemCatalog.itemStackArrays.Pop();
			}
			return new int[78];
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x0007439A File Offset: 0x0007259A
		public static void ReturnItemStackArray(int[] itemStackArray)
		{
			Array.Clear(itemStackArray, 0, itemStackArray.Length);
			ItemCatalog.itemStackArrays.Push(itemStackArray);
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x000743B4 File Offset: 0x000725B4
		private static void RegisterItem(ItemIndex itemIndex, ItemDef itemDef)
		{
			itemDef.itemIndex = itemIndex;
			ItemCatalog.itemDefs[(int)itemIndex] = itemDef;
			switch (itemDef.tier)
			{
			case ItemTier.Tier1:
				ItemCatalog.tier1ItemList.Add(itemIndex);
				break;
			case ItemTier.Tier2:
				ItemCatalog.tier2ItemList.Add(itemIndex);
				break;
			case ItemTier.Tier3:
				ItemCatalog.tier3ItemList.Add(itemIndex);
				break;
			case ItemTier.Lunar:
				ItemCatalog.lunarItemList.Add(itemIndex);
				break;
			}
			string arg = itemIndex.ToString().ToUpper();
			if (itemDef.nameToken == null)
			{
				itemDef.nameToken = string.Format(CultureInfo.InvariantCulture, "ITEM_{0}_NAME", arg);
			}
			if (itemDef.descriptionToken == null)
			{
				itemDef.descriptionToken = string.Format(CultureInfo.InvariantCulture, "ITEM_{0}_DESC", arg);
			}
			if (itemDef.pickupToken == null)
			{
				itemDef.pickupToken = string.Format(CultureInfo.InvariantCulture, "ITEM_{0}_PICKUP", arg);
			}
			if (itemDef.loreToken == null)
			{
				itemDef.loreToken = string.Format(CultureInfo.InvariantCulture, "ITEM_{0}_LORE", arg);
			}
			if (itemDef.pickupModelPath == null)
			{
				itemDef.pickupModelPath = "Prefabs/NullModel";
			}
			if (itemDef.pickupIconPath == null)
			{
				itemDef.pickupIconPath = "Textures/ItemIcons/texNullIcon";
			}
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x000744D2 File Offset: 0x000726D2
		public static ItemDef GetItemDef(ItemIndex itemIndex)
		{
			if (itemIndex < ItemIndex.Syringe || itemIndex >= ItemIndex.Count)
			{
				return null;
			}
			return ItemCatalog.itemDefs[(int)itemIndex];
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x000744E8 File Offset: 0x000726E8
		private static void DefineItems()
		{
			ItemCatalog.itemDefs = new ItemDef[78];
			ItemCatalog.RegisterItem(ItemIndex.AACannon, new ItemDef
			{
				tier = ItemTier.NoTier,
				nameToken = "ITEM_AACANNON_NAME",
				pickupToken = "ITEM_AACANNON_PICKUP",
				descriptionToken = "ITEM_AACANNON_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.AlienHead, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupAlienHead",
				pickupIconPath = "Textures/ItemIcons/texAlienHeadIcon",
				nameToken = "ITEM_ALIENHEAD_NAME",
				pickupToken = "ITEM_ALIENHEAD_PICKUP",
				descriptionToken = "ITEM_ALIENHEAD_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.AttackSpeedOnCrit, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupWolfPelt",
				pickupIconPath = "Textures/ItemIcons/texWolfPeltIcon",
				nameToken = "ITEM_ATTACKSPEEDONCRIT_NAME",
				pickupToken = "ITEM_ATTACKSPEEDONCRIT_PICKUP",
				descriptionToken = "ITEM_ATTACKSPEEDONCRIT_DESC",
				addressToken = "",
				unlockableName = "Items.AttackSpeedOnCrit"
			});
			ItemCatalog.RegisterItem(ItemIndex.Bandolier, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupBandolier",
				pickupIconPath = "Textures/ItemIcons/texBandolierIcon",
				nameToken = "ITEM_BANDOLIER_NAME",
				pickupToken = "ITEM_BANDOLIER_PICKUP",
				descriptionToken = "ITEM_BANDOLIER_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.Bear, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupBear",
				pickupIconPath = "Textures/ItemIcons/texBearIcon",
				nameToken = "ITEM_BEAR_NAME",
				pickupToken = "ITEM_BEAR_PICKUP",
				descriptionToken = "ITEM_BEAR_DESC",
				addressToken = "",
				unlockableName = "Items.Bear"
			});
			ItemCatalog.RegisterItem(ItemIndex.Behemoth, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupBehemoth",
				pickupIconPath = "Textures/ItemIcons/texBehemothIcon",
				nameToken = "ITEM_BEHEMOTH_NAME",
				pickupToken = "ITEM_BEHEMOTH_PICKUP",
				descriptionToken = "ITEM_BEHEMOTH_DESC",
				addressToken = "",
				mageElement = MageElement.Fire
			});
			ItemCatalog.RegisterItem(ItemIndex.BleedOnHit, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupTriTip",
				pickupIconPath = "Textures/ItemIcons/texTriTipIcon",
				nameToken = "ITEM_BLEEDONHIT_NAME",
				pickupToken = "ITEM_BLEEDONHIT_PICKUP",
				descriptionToken = "ITEM_BLEEDONHIT_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.BoostDamage, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = null,
				pickupIconPath = null,
				nameToken = "ITEM_BOOSTDAMAGE_NAME",
				pickupToken = "ITEM_BOOSTDAMAGE_PICKUP",
				descriptionToken = "ITEM_BOOSTDAMAGE_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.BoostHp, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = null,
				pickupIconPath = null,
				nameToken = "ITEM_BOOSTHP_NAME",
				pickupToken = "ITEM_BOOSTHP_PICKUP",
				descriptionToken = "ITEM_BOOSTHP_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.BounceNearby, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupHook",
				pickupIconPath = "Textures/ItemIcons/texHookIcon",
				nameToken = "ITEM_BOUNCENEARBY_NAME",
				pickupToken = "ITEM_BOUNCENEARBY_PICKUP",
				descriptionToken = "ITEM_BOUNCENEARBY_DESC",
				addressToken = "",
				unlockableName = "Items.BounceNearby"
			});
			ItemCatalog.RegisterItem(ItemIndex.ChainLightning, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupUkulele",
				pickupIconPath = "Textures/ItemIcons/texUkuleleIcon",
				nameToken = "ITEM_CHAINLIGHTNING_NAME",
				pickupToken = "ITEM_CHAINLIGHTNING_PICKUP",
				descriptionToken = "ITEM_CHAINLIGHTNING_DESC",
				addressToken = "",
				mageElement = MageElement.Lightning
			});
			ItemCatalog.RegisterItem(ItemIndex.Clover, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupClover",
				pickupIconPath = "Textures/ItemIcons/texCloverIcon",
				nameToken = "ITEM_CLOVER_NAME",
				pickupToken = "ITEM_CLOVER_PICKUP",
				descriptionToken = "ITEM_CLOVER_DESC",
				addressToken = "",
				unlockableName = "Items.Clover"
			});
			ItemCatalog.RegisterItem(ItemIndex.CooldownOnCrit, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupSkull",
				nameToken = "ITEM_COOLDOWNONCRIT_NAME",
				pickupToken = "ITEM_COOLDOWNONCRIT_PICKUP",
				descriptionToken = "ITEM_COOLDOWNONCRIT_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.CritGlasses, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupGlasses",
				pickupIconPath = "Textures/ItemIcons/texGlassesIcon",
				nameToken = "ITEM_CRITGLASSES_NAME",
				pickupToken = "ITEM_CRITGLASSES_PICKUP",
				descriptionToken = "ITEM_CRITGLASSES_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.Crowbar, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupCrowbar",
				pickupIconPath = "Textures/ItemIcons/texCrowbarIcon",
				nameToken = "ITEM_CROWBAR_NAME",
				pickupToken = "ITEM_CROWBAR_PICKUP",
				descriptionToken = "ITEM_CROWBAR_DESC",
				addressToken = "",
				unlockableName = "Items.Crowbar"
			});
			ItemCatalog.RegisterItem(ItemIndex.Dagger, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupDagger",
				pickupIconPath = "Textures/ItemIcons/texDaggerIcon",
				nameToken = "ITEM_DAGGER_NAME",
				pickupToken = "ITEM_DAGGER_PICKUP",
				descriptionToken = "ITEM_DAGGER_PICKUP",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.EquipmentMagazine, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupBattery",
				pickupIconPath = "Textures/ItemIcons/texBatteryIcon",
				nameToken = "ITEM_EQUIPMENTMAGAZINE_NAME",
				pickupToken = "ITEM_EQUIPMENTMAGAZINE_PICKUP",
				descriptionToken = "ITEM_EQUIPMENTMAGAZINE_DESC",
				addressToken = "",
				unlockableName = "Items.EquipmentMagazine"
			});
			ItemCatalog.RegisterItem(ItemIndex.ExplodeOnDeath, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupWilloWisp",
				pickupIconPath = "Textures/ItemIcons/texWilloWispIcon",
				nameToken = "ITEM_EXPLODEONDEATH_NAME",
				pickupToken = "ITEM_EXPLODEONDEATH_PICKUP",
				descriptionToken = "ITEM_EXPLODEONDEATH_DESC",
				addressToken = "",
				mageElement = MageElement.Fire
			});
			ItemCatalog.RegisterItem(ItemIndex.FallBoots, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupGravBoots",
				pickupIconPath = "Textures/ItemIcons/texGravBootsIcon",
				nameToken = "ITEM_FALLBOOTS_NAME",
				pickupToken = "ITEM_FALLBOOTS_PICKUP",
				descriptionToken = "ITEM_FALLBOOTS_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.Feather, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupFeather",
				pickupIconPath = "Textures/ItemIcons/texFeatherIcon",
				nameToken = "ITEM_FEATHER_NAME",
				pickupToken = "ITEM_FEATHER_PICKUP",
				descriptionToken = "ITEM_FEATHER_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.HealOnCrit, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupScythe",
				pickupIconPath = "Textures/ItemIcons/texScytheIcon",
				nameToken = "ITEM_HEALONCRIT_NAME",
				pickupToken = "ITEM_HEALONCRIT_PICKUP",
				descriptionToken = "ITEM_HEALONCRIT_DESC",
				unlockableName = "Items.HealOnCrit",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.HealWhileSafe, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupSnail",
				pickupIconPath = "Textures/ItemIcons/texSnailIcon",
				nameToken = "ITEM_HEALWHILESAFE_NAME",
				pickupToken = "ITEM_HEALWHILESAFE_PICKUP",
				descriptionToken = "ITEM_HEALWHILESAFE_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.Icicle, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupFrostRelic",
				pickupIconPath = "Textures/ItemIcons/texFrostRelicIcon",
				nameToken = "ITEM_ICICLE_NAME",
				pickupToken = "ITEM_ICICLE_PICKUP",
				descriptionToken = "ITEM_ICICLE_DESC",
				addressToken = "",
				mageElement = MageElement.Ice
			});
			ItemCatalog.RegisterItem(ItemIndex.IgniteOnKill, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupGasoline",
				pickupIconPath = "Textures/ItemIcons/texGasolineIcon",
				nameToken = "ITEM_IGNITEONKILL_NAME",
				pickupToken = "ITEM_IGNITEONKILL_PICKUP",
				descriptionToken = "ITEM_IGNITEONKILL_DESC",
				addressToken = "",
				mageElement = MageElement.Fire
			});
			ItemCatalog.RegisterItem(ItemIndex.Infusion, new ItemDef
			{
				tier = ItemTier.Tier2,
				nameToken = "ITEM_INFUSION_NAME",
				pickupToken = "ITEM_INFUSION_PICKUP",
				descriptionToken = "ITEM_INFUSION_DESC",
				pickupModelPath = "Prefabs/PickupModels/PickupInfusion",
				pickupIconPath = "Textures/ItemIcons/texInfusionIcon",
				addressToken = "",
				unlockableName = "Items.Infusion"
			});
			ItemCatalog.RegisterItem(ItemIndex.LevelBonus, new ItemDef
			{
				tier = ItemTier.NoTier,
				nameToken = "ITEM_LEVELBONUS_NAME",
				pickupToken = "ITEM_LEVELBONUS_PICKUP",
				descriptionToken = "ITEM_LEVELBONUS_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.Hoof, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupHoof",
				pickupIconPath = "Textures/ItemIcons/texHoofIcon",
				nameToken = "ITEM_HOOF_NAME",
				pickupToken = "ITEM_HOOF_PICKUP",
				descriptionToken = "ITEM_HOOF_DESC",
				addressToken = "",
				unlockableName = "Items.Hoof"
			});
			ItemCatalog.RegisterItem(ItemIndex.Knurl, new ItemDef
			{
				tier = ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupKnurl",
				pickupIconPath = "Textures/ItemIcons/texKnurlIcon",
				nameToken = "ITEM_KNURL_NAME",
				pickupToken = "ITEM_KNURL_PICKUP",
				descriptionToken = "ITEM_KNURL_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.GhostOnKill, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupMask",
				pickupIconPath = "Textures/ItemIcons/texMaskIcon",
				nameToken = "ITEM_GHOSTONKILL_NAME",
				pickupToken = "ITEM_GHOSTONKILL_PICKUP",
				descriptionToken = "ITEM_GHOSTONKILL_DESC"
			});
			ItemCatalog.RegisterItem(ItemIndex.Medkit, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupMedkit",
				pickupIconPath = "Textures/ItemIcons/texMedkitIcon",
				nameToken = "ITEM_MEDKIT_NAME",
				pickupToken = "ITEM_MEDKIT_PICKUP",
				descriptionToken = "ITEM_MEDKIT_DESC",
				addressToken = "",
				unlockableName = "Items.Medkit"
			});
			ItemCatalog.RegisterItem(ItemIndex.Missile, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupMissileLauncher",
				pickupIconPath = "Textures/ItemIcons/texMissileLauncherIcon",
				nameToken = "ITEM_MISSILE_NAME",
				pickupToken = "ITEM_MISSILE_PICKUP",
				descriptionToken = "ITEM_MISSILE_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.Mushroom, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupMushroom",
				pickupIconPath = "Textures/ItemIcons/texMushroomIcon",
				nameToken = "ITEM_MUSHROOM_NAME",
				pickupToken = "ITEM_MUSHROOM_PICKUP",
				descriptionToken = "ITEM_MUSHROOM_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.NovaOnHeal, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupDevilHorns",
				pickupIconPath = "Textures/ItemIcons/texDevilHornsIcon",
				nameToken = "ITEM_NOVAONHEAL_NAME",
				pickupToken = "ITEM_NOVAONHEAL_PICKUP",
				descriptionToken = "ITEM_NOVAONHEAL_DESC",
				addressToken = "",
				unlockableName = "Items.NovaOnHeal"
			});
			ItemCatalog.RegisterItem(ItemIndex.PersonalShield, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupShieldGenerator",
				pickupIconPath = "Textures/ItemIcons/texPersonalShieldIcon",
				nameToken = "ITEM_PERSONALSHIELD_NAME",
				pickupToken = "ITEM_PERSONALSHIELD_PICKUP",
				descriptionToken = "ITEM_PERSONALSHIELD_DESC",
				addressToken = "",
				mageElement = MageElement.Lightning
			});
			ItemCatalog.RegisterItem(ItemIndex.Phasing, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupStealthkit",
				pickupIconPath = "Textures/ItemIcons/texStealthkitIcon",
				nameToken = "ITEM_PHASING_NAME",
				pickupToken = "ITEM_PHASING_PICKUP",
				descriptionToken = "ITEM_PHASING_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.PlantOnHit, new ItemDef
			{
				tier = ItemTier.NoTier,
				nameToken = "ITEM_PLANTONHIT_NAME",
				pickupToken = "ITEM_PLANTONHIT_PICKUP",
				descriptionToken = "ITEM_PLANTONHIT_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.PlasmaCore, new ItemDef
			{
				tier = ItemTier.NoTier,
				nameToken = "ITEM_PLASMACORE_NAME",
				pickupToken = "ITEM_PLASMACORE_PICKUP",
				descriptionToken = "ITEM_PLASMACORE_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.ShieldOnly, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupShieldBug",
				pickupIconPath = "Textures/ItemIcons/texShieldBugIcon",
				nameToken = "ITEM_SHIELDONLY_NAME",
				pickupToken = "ITEM_SHIELDONLY_PICKUP",
				descriptionToken = "ITEM_SHIELDONLY_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.Seed, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupSeed",
				pickupIconPath = "Textures/ItemIcons/texSeedIcon",
				nameToken = "ITEM_SEED_NAME",
				pickupToken = "ITEM_SEED_PICKUP",
				descriptionToken = "ITEM_SEED_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.ShockNearby, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupTeslaCoil",
				pickupIconPath = "Textures/ItemIcons/texTeslaCoilIcon",
				nameToken = "ITEM_SHOCKNEARBY_NAME",
				pickupToken = "ITEM_SHOCKNEARBY_PICKUP",
				descriptionToken = "ITEM_SHOCKNEARBY_DESC",
				addressToken = "",
				unlockableName = "Items.ShockNearby"
			});
			ItemCatalog.RegisterItem(ItemIndex.SprintOutOfCombat, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupWhip",
				pickupIconPath = "Textures/ItemIcons/texWhipIcon",
				nameToken = "ITEM_SPRINTOUTOFCOMBAT_NAME",
				pickupToken = "ITEM_SPRINTOUTOFCOMBAT_PICKUP",
				descriptionToken = "ITEM_SPRINTOUTOFCOMBAT_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.Syringe, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupSyringeCluster",
				pickupIconPath = "Textures/ItemIcons/texSyringeIcon",
				nameToken = "ITEM_SYRINGE_NAME",
				pickupToken = "ITEM_SYRINGE_PICKUP",
				descriptionToken = "ITEM_SYRINGE_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.Talisman, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupTalisman",
				pickupIconPath = "Textures/ItemIcons/texTalismanIcon",
				nameToken = "ITEM_TALISMAN_NAME",
				pickupToken = "ITEM_TALISMAN_PICKUP",
				descriptionToken = "ITEM_TALISMAN_DESC",
				addressToken = "",
				unlockableName = "Items.Talisman"
			});
			ItemCatalog.RegisterItem(ItemIndex.TempestOnKill, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupWaxBird",
				pickupIconPath = "Textures/ItemIcons/texWaxBirdIcon",
				nameToken = "ITEM_TEMPESTONKILL_NAME",
				pickupToken = "ITEM_TEMPESTONKILL_PICKUP",
				descriptionToken = "ITEM_TEMPESTONKILL_DESC",
				addressToken = "",
				unlockableName = "Items.TempestOnKill"
			});
			ItemCatalog.RegisterItem(ItemIndex.JumpBoost, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupWaxBird",
				pickupIconPath = "Textures/ItemIcons/texWaxBirdIcon",
				nameToken = "ITEM_JUMPBOOST_NAME",
				pickupToken = "ITEM_JUMPBOOST_PICKUP",
				descriptionToken = "ITEM_JUMPBOOST_DESC",
				addressToken = "",
				unlockableName = "Items.JumpBoost"
			});
			ItemCatalog.RegisterItem(ItemIndex.Tooth, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupToothNecklace",
				pickupIconPath = "Textures/ItemIcons/texToothNecklaceIcon",
				nameToken = "ITEM_TOOTH_NAME",
				pickupToken = "ITEM_TOOTH_PICKUP",
				descriptionToken = "ITEM_TOOTH_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.WarCryOnCombat, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupPauldron",
				pickupIconPath = "Textures/ItemIcons/texPauldronIcon",
				nameToken = "ITEM_WARCRYONCOMBAT_NAME",
				pickupToken = "ITEM_WARCRYONCOMBAT_PICKUP",
				descriptionToken = "ITEM_WARCRYONCOMBAT_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.WarCryOnMultiKill, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupPauldron",
				pickupIconPath = "Textures/ItemIcons/texPauldronIcon",
				nameToken = "ITEM_WARCRYONMULTIKILL_NAME",
				pickupToken = "ITEM_WARCRYONMULTIKILL_PICKUP",
				descriptionToken = "ITEM_WARCRYONMULTIKILL_DESC",
				addressToken = "",
				unlockableName = "Items.WarCryOnMultiKill"
			});
			ItemCatalog.RegisterItem(ItemIndex.WardOnLevel, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupWarbanner",
				pickupIconPath = "Textures/ItemIcons/texWarbannerIcon",
				nameToken = "ITEM_WARDONLEVEL_NAME",
				pickupToken = "ITEM_WARDONLEVEL_PICKUP",
				descriptionToken = "ITEM_WARDONLEVEL_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.StunChanceOnHit, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupStunGrenade",
				pickupIconPath = "Textures/ItemIcons/texStunGrenadeIcon",
				nameToken = "ITEM_STUNCHANCEONHIT_NAME",
				pickupToken = "ITEM_STUNCHANCEONHIT_PICKUP",
				descriptionToken = "ITEM_STUNCHANCEONHIT_DESC",
				addressToken = "",
				mageElement = MageElement.Lightning
			});
			ItemCatalog.RegisterItem(ItemIndex.Firework, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupFirework",
				pickupIconPath = "Textures/ItemIcons/texFireworkIcon",
				nameToken = "ITEM_FIREWORK_NAME",
				pickupToken = "ITEM_FIREWORK_PICKUP",
				descriptionToken = "ITEM_FIREWORK_DESC",
				addressToken = "",
				unlockableName = "Items.Firework"
			});
			ItemCatalog.RegisterItem(ItemIndex.LunarDagger, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupLunarDagger",
				pickupIconPath = "Textures/ItemIcons/texLunarDaggerIcon",
				nameToken = "ITEM_LUNARDAGGER_NAME",
				pickupToken = "ITEM_LUNARDAGGER_PICKUP",
				descriptionToken = "ITEM_LUNARDAGGER_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.GoldOnHit, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupBoneCrown",
				pickupIconPath = "Textures/ItemIcons/texBoneCrownIcon",
				nameToken = "ITEM_GOLDONHIT_NAME",
				pickupToken = "ITEM_GOLDONHIT_PICKUP",
				descriptionToken = "ITEM_GOLDONHIT_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.BeetleGland, new ItemDef
			{
				tier = ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupBeetleGland",
				pickupIconPath = "Textures/ItemIcons/texBeetleGlandIcon",
				nameToken = "ITEM_BEETLEGLAND_NAME",
				pickupToken = "ITEM_BEETLEGLAND_PICKUP",
				descriptionToken = "ITEM_BEETLEGLAND_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.BurnNearby, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupPotion",
				pickupIconPath = "Textures/ItemIcons/texPotionIcon",
				nameToken = "ITEM_BURNNEARBY_NAME",
				pickupToken = "ITEM_BURNNEARBY_PICKUP",
				descriptionToken = "ITEM_BURNNEARBY_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.CritHeal, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupCorpseflower",
				pickupIconPath = "Textures/ItemIcons/texCorpseflowerIcon",
				nameToken = "ITEM_CRITHEAL_NAME",
				pickupToken = "ITEM_CRITHEAL_PICKUP",
				descriptionToken = "ITEM_CRITHEAL_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.CrippleWardOnLevel, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupEffigy",
				pickupIconPath = "Textures/ItemIcons/texEffigyIcon",
				nameToken = "ITEM_CRIPPLEWARDONLEVEL_NAME",
				pickupToken = "ITEM_CRIPPLEWARDONLEVEL_PICKUP",
				descriptionToken = "ITEM_CRIPPLEWARDONLEVEL_DESC",
				addressToken = "",
				unlockableName = "Items.CrippleWardOnLevel"
			});
			ItemCatalog.RegisterItem(ItemIndex.SprintBonus, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupSoda",
				pickupIconPath = "Textures/ItemIcons/texSodaIcon",
				nameToken = "ITEM_SPRINTBONUS_NAME",
				pickupToken = "ITEM_SPRINTBONUS_PICKUP",
				descriptionToken = "ITEM_SPRINTBONUS_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.SecondarySkillMagazine, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupDoubleMag",
				pickupIconPath = "Textures/ItemIcons/texDoubleMagIcon",
				nameToken = "ITEM_SECONDARYSKILLMAGAZINE_NAME",
				pickupToken = "ITEM_SECONDARYSKILLMAGAZINE_PICKUP",
				descriptionToken = "ITEM_SECONDARYSKILLMAGAZINE_DESC",
				addressToken = "",
				unlockableName = "Items.SecondarySkillMagazine"
			});
			ItemCatalog.RegisterItem(ItemIndex.StickyBomb, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupStickyBomb",
				pickupIconPath = "Textures/ItemIcons/texStickyBombIcon",
				nameToken = "ITEM_STICKYBOMB_NAME",
				pickupToken = "ITEM_STICKYBOMB_PICKUP",
				descriptionToken = "ITEM_STICKYBOMB_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.TreasureCache, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupKey",
				pickupIconPath = "Textures/ItemIcons/texKeyIcon",
				nameToken = "ITEM_TREASURECACHE_NAME",
				pickupToken = "ITEM_TREASURECACHE_PICKUP",
				descriptionToken = "ITEM_TREASURECACHE_DESC",
				addressToken = "",
				unlockableName = "Items.TreasureCache"
			});
			ItemCatalog.RegisterItem(ItemIndex.BossDamageBonus, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupAPRounds",
				pickupIconPath = "Textures/ItemIcons/texAPRoundsIcon",
				nameToken = "ITEM_BOSSDAMAGEBONUS_NAME",
				pickupToken = "ITEM_BOSSDAMAGEBONUS_PICKUP",
				descriptionToken = "ITEM_BOSSDAMAGEBONUS_DESC",
				addressToken = "",
				unlockableName = "Items.BossDamageBonus"
			});
			ItemCatalog.RegisterItem(ItemIndex.SprintArmor, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupBuckler",
				pickupIconPath = "Textures/ItemIcons/texBucklerIcon",
				nameToken = "ITEM_SPRINTARMOR_NAME",
				pickupToken = "ITEM_SPRINTARMOR_PICKUP",
				descriptionToken = "ITEM_SPRINTARMOR_DESC",
				addressToken = ""
			});
			ItemCatalog.RegisterItem(ItemIndex.IceRing, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupIceRing",
				pickupIconPath = "Textures/ItemIcons/texIceRingIcon",
				unlockableName = "Items.ElementalRings"
			});
			ItemCatalog.RegisterItem(ItemIndex.FireRing, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupFireRing",
				pickupIconPath = "Textures/ItemIcons/texFireRingIcon",
				unlockableName = "Items.ElementalRings"
			});
			ItemCatalog.RegisterItem(ItemIndex.SlowOnHit, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupBauble",
				pickupIconPath = "Textures/ItemIcons/texBaubleIcon"
			});
			ItemCatalog.RegisterItem(ItemIndex.ExtraLife, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupHippo",
				pickupIconPath = "Textures/ItemIcons/texHippoIcon",
				unlockableName = "Items.ExtraLife"
			});
			ItemCatalog.RegisterItem(ItemIndex.ExtraLifeConsumed, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupHippo",
				pickupIconPath = "Textures/ItemIcons/texHippoIconConsumed"
			});
			ItemCatalog.RegisterItem(ItemIndex.UtilitySkillMagazine, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupAfterburner",
				pickupIconPath = "Textures/ItemIcons/texAfterburnerIcon"
			});
			ItemCatalog.RegisterItem(ItemIndex.HeadHunter, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupSkullcrown",
				pickupIconPath = "Textures/ItemIcons/texSkullcrownIcon"
			});
			ItemCatalog.RegisterItem(ItemIndex.KillEliteFrenzy, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupBrainstalk",
				pickupIconPath = "Textures/ItemIcons/texBrainstalkIcon",
				unlockableName = "Items.KillEliteFrenzy"
			});
			ItemCatalog.RegisterItem(ItemIndex.RepeatHeal, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupCorpseflower",
				pickupIconPath = "Textures/ItemIcons/texCorpseflowerIcon"
			});
			ItemCatalog.RegisterItem(ItemIndex.IncreaseHealing, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupAntler",
				pickupIconPath = "Textures/ItemIcons/texAntlerIcon",
				unlockableName = "Items.IncreaseHealing"
			});
			ItemCatalog.RegisterItem(ItemIndex.AutoCastEquipment, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupFossil",
				pickupIconPath = "Textures/ItemIcons/texFossilIcon",
				unlockableName = "Items.AutoCastEquipment"
			});
			ItemCatalog.RegisterItem(ItemIndex.DrizzlePlayerHelper, new ItemDef
			{
				tier = ItemTier.NoTier,
				hidden = true,
				canRemove = false
			});
			ItemCatalog.RegisterItem(ItemIndex.Ghost, new ItemDef
			{
				tier = ItemTier.NoTier,
				hidden = false,
				canRemove = false
			});
			ItemCatalog.RegisterItem(ItemIndex.HealthDecay, new ItemDef
			{
				tier = ItemTier.NoTier,
				hidden = false,
				canRemove = false
			});
			ItemCatalog.RegisterItem(ItemIndex.MageAttunement, new ItemDef
			{
				tier = ItemTier.NoTier,
				hidden = true,
				canRemove = false
			});
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				if (ItemCatalog.GetItemDef(itemIndex) == null)
				{
					Debug.LogErrorFormat("Item {0} is unregistered!", new object[]
					{
						itemIndex
					});
				}
			}
		}

		// Token: 0x04001C07 RID: 7175
		public static List<ItemIndex> tier1ItemList = new List<ItemIndex>();

		// Token: 0x04001C08 RID: 7176
		public static List<ItemIndex> tier2ItemList = new List<ItemIndex>();

		// Token: 0x04001C09 RID: 7177
		public static List<ItemIndex> tier3ItemList = new List<ItemIndex>();

		// Token: 0x04001C0A RID: 7178
		public static List<ItemIndex> lunarItemList = new List<ItemIndex>();

		// Token: 0x04001C0B RID: 7179
		private static ItemDef[] itemDefs;

		// Token: 0x04001C0C RID: 7180
		private static readonly Stack<ItemIndex[]> itemOrderBuffers = new Stack<ItemIndex[]>();

		// Token: 0x04001C0D RID: 7181
		private static readonly Stack<int[]> itemStackArrays = new Stack<int[]>();

		// Token: 0x04001C0E RID: 7182
		public static readonly GenericStaticEnumerable<ItemIndex, ItemCatalog.AllItemsEnumerator> allItems;

		// Token: 0x0200044C RID: 1100
		public struct AllItemsEnumerator : IEnumerator<ItemIndex>, IEnumerator, IDisposable
		{
			// Token: 0x06001871 RID: 6257 RVA: 0x00075D34 File Offset: 0x00073F34
			public bool MoveNext()
			{
				this.position++;
				return this.position < ItemIndex.Count;
			}

			// Token: 0x06001872 RID: 6258 RVA: 0x00075D4E File Offset: 0x00073F4E
			public void Reset()
			{
				this.position = ItemIndex.None;
			}

			// Token: 0x1700023B RID: 571
			// (get) Token: 0x06001873 RID: 6259 RVA: 0x00075D57 File Offset: 0x00073F57
			public ItemIndex Current
			{
				get
				{
					return this.position;
				}
			}

			// Token: 0x1700023C RID: 572
			// (get) Token: 0x06001874 RID: 6260 RVA: 0x00075D5F File Offset: 0x00073F5F
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x06001875 RID: 6261 RVA: 0x00004507 File Offset: 0x00002707
			void IDisposable.Dispose()
			{
			}

			// Token: 0x04001C0F RID: 7183
			private ItemIndex position;
		}
	}
}
