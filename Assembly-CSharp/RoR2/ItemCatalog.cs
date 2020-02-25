using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003B1 RID: 945
	public static class ItemCatalog
	{
		// Token: 0x170002AC RID: 684
		// (get) Token: 0x060016D9 RID: 5849 RVA: 0x00061EAE File Offset: 0x000600AE
		// (set) Token: 0x060016DA RID: 5850 RVA: 0x00061EB5 File Offset: 0x000600B5
		public static int itemCount { get; private set; }

		// Token: 0x060016DB RID: 5851 RVA: 0x00061EC0 File Offset: 0x000600C0
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			ItemCatalog.itemNameToIndex.Clear();
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
			ItemCatalog.modHelper.CollectAndRegisterAdditionalEntries(ref ItemCatalog.itemDefs);
			ItemCatalog.itemCount = ItemCatalog.itemDefs.Length;
			ItemCatalog.itemStackArrays.Clear();
			ItemCatalog.availability.MakeAvailable();
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x00061F4E File Offset: 0x0006014E
		public static ItemIndex[] RequestItemOrderBuffer()
		{
			if (ItemCatalog.itemOrderBuffers.Count > 0)
			{
				return ItemCatalog.itemOrderBuffers.Pop();
			}
			return new ItemIndex[ItemCatalog.itemCount];
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x00061F72 File Offset: 0x00060172
		public static void ReturnItemOrderBuffer(ItemIndex[] buffer)
		{
			ItemCatalog.itemOrderBuffers.Push(buffer);
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x00061F7F File Offset: 0x0006017F
		public static int[] RequestItemStackArray()
		{
			if (ItemCatalog.itemStackArrays.Count > 0)
			{
				return ItemCatalog.itemStackArrays.Pop();
			}
			return new int[ItemCatalog.itemCount];
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x00061FA3 File Offset: 0x000601A3
		public static void ReturnItemStackArray(int[] itemStackArray)
		{
			if (itemStackArray.Length != ItemCatalog.itemCount)
			{
				return;
			}
			Array.Clear(itemStackArray, 0, itemStackArray.Length);
			ItemCatalog.itemStackArrays.Push(itemStackArray);
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x00061FC8 File Offset: 0x000601C8
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
			if (itemDef.name == null)
			{
				itemDef.name = itemIndex.ToString();
			}
			string name = itemDef.name;
			string arg = name.ToUpper(CultureInfo.InvariantCulture);
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
			ItemCatalog.itemNameToIndex[name] = itemIndex;
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x0006210D File Offset: 0x0006030D
		public static ItemDef GetItemDef(ItemIndex itemIndex)
		{
			return HGArrayUtilities.GetSafe<ItemDef>(ItemCatalog.itemDefs, (int)itemIndex);
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x0006211C File Offset: 0x0006031C
		public static ItemIndex FindItemIndex(string itemName)
		{
			ItemIndex result;
			if (ItemCatalog.itemNameToIndex.TryGetValue(itemName, out result))
			{
				return result;
			}
			return ItemIndex.None;
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x0006213B File Offset: 0x0006033B
		public static T[] GetPerItemBuffer<T>()
		{
			return new T[ItemCatalog.itemCount];
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x00062147 File Offset: 0x00060347
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsIndexValid(in ItemIndex itemIndex)
		{
			return itemIndex < (ItemIndex)ItemCatalog.itemCount;
		}

		// Token: 0x060016E5 RID: 5861 RVA: 0x00062154 File Offset: 0x00060354
		private static void DefineItems()
		{
			ItemCatalog.itemDefs = new ItemDef[99];
			ItemCatalog.itemCount = ItemCatalog.itemDefs.Length;
			ItemCatalog.RegisterItem(ItemIndex.AACannon, new ItemDef
			{
				tier = ItemTier.NoTier,
				nameToken = "ITEM_AACANNON_NAME",
				pickupToken = "ITEM_AACANNON_PICKUP",
				descriptionToken = "ITEM_AACANNON_DESC"
			});
			ItemCatalog.RegisterItem(ItemIndex.AlienHead, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupAlienHead",
				pickupIconPath = "Textures/ItemIcons/texAlienHeadIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.AttackSpeedOnCrit, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupWolfPelt",
				pickupIconPath = "Textures/ItemIcons/texWolfPeltIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				},
				unlockableName = "Items.AttackSpeedOnCrit"
			});
			ItemCatalog.RegisterItem(ItemIndex.Bandolier, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupBandolier",
				pickupIconPath = "Textures/ItemIcons/texBandolierIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Bear, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupBear",
				pickupIconPath = "Textures/ItemIcons/texBearIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				},
				unlockableName = "Items.Bear"
			});
			ItemCatalog.RegisterItem(ItemIndex.Behemoth, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupBehemoth",
				pickupIconPath = "Textures/ItemIcons/texBehemothIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.BleedOnHit, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupTriTip",
				pickupIconPath = "Textures/ItemIcons/texTriTipIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.BoostDamage, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = null,
				pickupIconPath = null,
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.BoostHp, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = null,
				pickupIconPath = null,
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.BounceNearby, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupHook",
				pickupIconPath = "Textures/ItemIcons/texHookIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				},
				unlockableName = "Items.BounceNearby"
			});
			ItemCatalog.RegisterItem(ItemIndex.ChainLightning, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupUkulele",
				pickupIconPath = "Textures/ItemIcons/texUkuleleIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Clover, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupClover",
				pickupIconPath = "Textures/ItemIcons/texCloverIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				},
				unlockableName = "Items.Clover"
			});
			ItemCatalog.RegisterItem(ItemIndex.CooldownOnCrit, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupSkull",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.CritGlasses, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupGlasses",
				pickupIconPath = "Textures/ItemIcons/texGlassesIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Crowbar, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupCrowbar",
				pickupIconPath = "Textures/ItemIcons/texCrowbarIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				},
				unlockableName = "Items.Crowbar"
			});
			ItemCatalog.RegisterItem(ItemIndex.Dagger, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupDagger",
				pickupIconPath = "Textures/ItemIcons/texDaggerIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.EquipmentMagazine, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupBattery",
				pickupIconPath = "Textures/ItemIcons/texBatteryIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.EquipmentRelated
				},
				unlockableName = "Items.EquipmentMagazine"
			});
			ItemCatalog.RegisterItem(ItemIndex.ExplodeOnDeath, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupWilloWisp",
				pickupIconPath = "Textures/ItemIcons/texWilloWispIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.FallBoots, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupGravBoots",
				pickupIconPath = "Textures/ItemIcons/texGravBootsIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.Damage,
					ItemTag.AIBlacklist
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Feather, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupFeather",
				pickupIconPath = "Textures/ItemIcons/texFeatherIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.AIBlacklist
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.HealOnCrit, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupScythe",
				pickupIconPath = "Textures/ItemIcons/texScytheIcon",
				unlockableName = "Items.HealOnCrit",
				tags = new ItemTag[]
				{
					ItemTag.Healing
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.HealWhileSafe, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupSnail",
				pickupIconPath = "Textures/ItemIcons/texSnailIcon",
				tags = new ItemTag[]
				{
					ItemTag.Healing
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Icicle, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupFrostRelic",
				pickupIconPath = "Textures/ItemIcons/texFrostRelicIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.IgniteOnKill, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupGasoline",
				pickupIconPath = "Textures/ItemIcons/texGasolineIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Infusion, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupInfusion",
				pickupIconPath = "Textures/ItemIcons/texInfusionIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.Healing,
					ItemTag.OnKillEffect
				},
				unlockableName = "Items.Infusion"
			});
			ItemCatalog.RegisterItem(ItemIndex.LevelBonus, new ItemDef
			{
				tier = ItemTier.NoTier,
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Hoof, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupHoof",
				pickupIconPath = "Textures/ItemIcons/texHoofIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				},
				unlockableName = "Items.Hoof"
			});
			ItemCatalog.RegisterItem(ItemIndex.Knurl, new ItemDef
			{
				tier = ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupKnurl",
				pickupIconPath = "Textures/ItemIcons/texKnurlIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.Healing
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.GhostOnKill, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupMask",
				pickupIconPath = "Textures/ItemIcons/texMaskIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.Damage,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Medkit, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupMedkit",
				pickupIconPath = "Textures/ItemIcons/texMedkitIcon",
				tags = new ItemTag[]
				{
					ItemTag.Healing
				},
				unlockableName = "Items.Medkit"
			});
			ItemCatalog.RegisterItem(ItemIndex.Missile, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupMissileLauncher",
				pickupIconPath = "Textures/ItemIcons/texMissileLauncherIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Mushroom, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupMushroom",
				pickupIconPath = "Textures/ItemIcons/texMushroomIcon",
				tags = new ItemTag[]
				{
					ItemTag.Healing,
					ItemTag.AIBlacklist
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.NovaOnHeal, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupDevilHorns",
				pickupIconPath = "Textures/ItemIcons/texDevilHornsIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				},
				unlockableName = "Items.NovaOnHeal"
			});
			ItemCatalog.RegisterItem(ItemIndex.PersonalShield, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupShieldGenerator",
				pickupIconPath = "Textures/ItemIcons/texPersonalShieldIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Phasing, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupStealthkit",
				pickupIconPath = "Textures/ItemIcons/texStealthkitIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.PlantOnHit, new ItemDef
			{
				tier = ItemTier.NoTier,
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.PlasmaCore, new ItemDef
			{
				tier = ItemTier.NoTier,
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.ShieldOnly, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupShieldBug",
				pickupIconPath = "Textures/ItemIcons/texShieldBugIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Seed, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupSeed",
				pickupIconPath = "Textures/ItemIcons/texSeedIcon",
				tags = new ItemTag[]
				{
					ItemTag.Healing
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.ShockNearby, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupTeslaCoil",
				pickupIconPath = "Textures/ItemIcons/texTeslaCoilIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				},
				unlockableName = "Items.ShockNearby"
			});
			ItemCatalog.RegisterItem(ItemIndex.SprintOutOfCombat, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupWhip",
				pickupIconPath = "Textures/ItemIcons/texWhipIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Syringe, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupSyringeCluster",
				pickupIconPath = "Textures/ItemIcons/texSyringeIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Talisman, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupTalisman",
				pickupIconPath = "Textures/ItemIcons/texTalismanIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.OnKillEffect,
					ItemTag.EquipmentRelated
				},
				unlockableName = "Items.Talisman"
			});
			ItemCatalog.RegisterItem(ItemIndex.TempestOnKill, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupWaxBird",
				pickupIconPath = "Textures/ItemIcons/texWaxBirdIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.OnKillEffect
				},
				unlockableName = "Items.TempestOnKill"
			});
			ItemCatalog.RegisterItem(ItemIndex.JumpBoost, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupWaxBird",
				pickupIconPath = "Textures/ItemIcons/texWaxBirdIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.SprintRelated
				},
				unlockableName = "Items.JumpBoost"
			});
			ItemCatalog.RegisterItem(ItemIndex.Tooth, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupToothNecklace",
				pickupIconPath = "Textures/ItemIcons/texToothNecklaceIcon",
				tags = new ItemTag[]
				{
					ItemTag.Healing,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.WarCryOnCombat, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupPauldron",
				pickupIconPath = "Textures/ItemIcons/texPauldronIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.WarCryOnMultiKill, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupPauldron",
				pickupIconPath = "Textures/ItemIcons/texPauldronIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.OnKillEffect
				},
				unlockableName = "Items.WarCryOnMultiKill"
			});
			ItemCatalog.RegisterItem(ItemIndex.WardOnLevel, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupWarbanner",
				pickupIconPath = "Textures/ItemIcons/texWarbannerIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.AIBlacklist
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.StunChanceOnHit, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupStunGrenade",
				pickupIconPath = "Textures/ItemIcons/texStunGrenadeIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.AIBlacklist
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Firework, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupFirework",
				pickupIconPath = "Textures/ItemIcons/texFireworkIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.AIBlacklist
				},
				unlockableName = "Items.Firework"
			});
			ItemCatalog.RegisterItem(ItemIndex.LunarDagger, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupLunarDagger",
				pickupIconPath = "Textures/ItemIcons/texLunarDaggerIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.GoldOnHit, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupBoneCrown",
				pickupIconPath = "Textures/ItemIcons/texBoneCrownIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.BeetleGland, new ItemDef
			{
				tier = ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupBeetleGland",
				pickupIconPath = "Textures/ItemIcons/texBeetleGlandIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.BurnNearby, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupPotion",
				pickupIconPath = "Textures/ItemIcons/texPotionIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.CritHeal, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupCorpseflower",
				pickupIconPath = "Textures/ItemIcons/texCorpseflowerIcon",
				tags = new ItemTag[]
				{
					ItemTag.Healing
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.CrippleWardOnLevel, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupEffigy",
				pickupIconPath = "Textures/ItemIcons/texEffigyIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				},
				unlockableName = "Items.CrippleWardOnLevel"
			});
			ItemCatalog.RegisterItem(ItemIndex.SprintBonus, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupSoda",
				pickupIconPath = "Textures/ItemIcons/texSodaIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.SprintRelated
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.SecondarySkillMagazine, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupDoubleMag",
				pickupIconPath = "Textures/ItemIcons/texDoubleMagIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				},
				unlockableName = "Items.SecondarySkillMagazine"
			});
			ItemCatalog.RegisterItem(ItemIndex.StickyBomb, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupStickyBomb",
				pickupIconPath = "Textures/ItemIcons/texStickyBombIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.TreasureCache, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupKey",
				pickupIconPath = "Textures/ItemIcons/texKeyIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.AIBlacklist
				},
				unlockableName = "Items.TreasureCache"
			});
			ItemCatalog.RegisterItem(ItemIndex.BossDamageBonus, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupAPRounds",
				pickupIconPath = "Textures/ItemIcons/texAPRoundsIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.AIBlacklist
				},
				unlockableName = "Items.BossDamageBonus"
			});
			ItemCatalog.RegisterItem(ItemIndex.SprintArmor, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupBuckler",
				pickupIconPath = "Textures/ItemIcons/texBucklerIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.SprintRelated
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.IceRing, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupIceRing",
				pickupIconPath = "Textures/ItemIcons/texIceRingIcon",
				unlockableName = "Items.ElementalRings",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.FireRing, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupFireRing",
				pickupIconPath = "Textures/ItemIcons/texFireRingIcon",
				unlockableName = "Items.ElementalRings",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.SlowOnHit, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupBauble",
				pickupIconPath = "Textures/ItemIcons/texBaubleIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.ExtraLife, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupHippo",
				pickupIconPath = "Textures/ItemIcons/texHippoIcon",
				unlockableName = "Items.ExtraLife",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.ExtraLifeConsumed, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupModelPath = "Prefabs/PickupModels/PickupHippo",
				pickupIconPath = "Textures/ItemIcons/texHippoIconConsumed",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.UtilitySkillMagazine, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupAfterburner",
				pickupIconPath = "Textures/ItemIcons/texAfterburnerIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.HeadHunter, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupSkullcrown",
				pickupIconPath = "Textures/ItemIcons/texSkullcrownIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.Damage,
					ItemTag.AIBlacklist,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.KillEliteFrenzy, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupBrainstalk",
				pickupIconPath = "Textures/ItemIcons/texBrainstalkIcon",
				unlockableName = "Items.KillEliteFrenzy",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.AIBlacklist,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.RepeatHeal, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupCorpseflower",
				pickupIconPath = "Textures/ItemIcons/texCorpseflowerIcon",
				tags = new ItemTag[]
				{
					ItemTag.Healing
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.IncreaseHealing, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupAntler",
				pickupIconPath = "Textures/ItemIcons/texAntlerIcon",
				unlockableName = "Items.IncreaseHealing",
				tags = new ItemTag[]
				{
					ItemTag.Healing
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.AutoCastEquipment, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupFossil",
				pickupIconPath = "Textures/ItemIcons/texFossilIcon",
				unlockableName = "Items.AutoCastEquipment",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.EquipmentRelated
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.ExecuteLowHealthElite, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupGuillotine",
				pickupIconPath = "Textures/ItemIcons/texGuillotineIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.AIBlacklist
				},
				unlockableName = "Items.ExecuteLowHealthElite"
			});
			ItemCatalog.RegisterItem(ItemIndex.EnergizedOnEquipmentUse, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupWarHorn",
				pickupIconPath = "Textures/ItemIcons/texWarHornIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.EquipmentRelated
				},
				unlockableName = "Items.EnergizedOnEquipmentUse"
			});
			ItemCatalog.RegisterItem(ItemIndex.BarrierOnOverHeal, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupAegis",
				pickupIconPath = "Textures/ItemIcons/texAegisIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.Healing
				}
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
			ItemCatalog.RegisterItem(ItemIndex.TonicAffliction, new ItemDef
			{
				tier = ItemTier.NoTier,
				pickupIconPath = "Textures/ItemIcons/texTonicAfflictionIcon",
				hidden = false,
				canRemove = false,
				tags = new ItemTag[]
				{
					ItemTag.Cleansable
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.TitanGoldDuringTP, new ItemDef
			{
				tier = ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupGoldHeart",
				pickupIconPath = "Textures/ItemIcons/texGoldHeartIcon",
				hidden = false,
				canRemove = false,
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.WorldUnique
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.SprintWisp, new ItemDef
			{
				tier = ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupBrokenMask",
				pickupIconPath = "Textures/ItemIcons/texBrokenMaskIcon",
				hidden = false,
				canRemove = false,
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.SprintRelated
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.BarrierOnKill, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupBrooch",
				pickupIconPath = "Textures/ItemIcons/texBroochIcon",
				hidden = false,
				canRemove = false,
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.Healing,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.ArmorReductionOnHit, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupWarhammer",
				pickupIconPath = "Textures/ItemIcons/texWarhammerIcon",
				hidden = false,
				canRemove = false,
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.TPHealingNova, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupGlowFlower",
				pickupIconPath = "Textures/ItemIcons/texGlowFlowerIcon",
				tags = new ItemTag[]
				{
					ItemTag.Healing,
					ItemTag.AIBlacklist
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.NearbyDamageBonus, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupDiamond",
				pickupIconPath = "Textures/ItemIcons/texDiamondIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.LunarUtilityReplacement, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupBirdFoot",
				pickupIconPath = "Textures/ItemIcons/texBirdFootIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility
				},
				unlockableName = "Items.LunarSkillReplacements"
			});
			ItemCatalog.RegisterItem(ItemIndex.MonsoonPlayerHelper, new ItemDef
			{
				tier = ItemTier.NoTier,
				hidden = true,
				canRemove = false
			});
			ItemCatalog.RegisterItem(ItemIndex.Thorns, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupRazorwire",
				pickupIconPath = "Textures/ItemIcons/texRazorwireIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.AIBlacklist
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.RegenOnKill, new ItemDef
			{
				tier = ItemTier.Tier1,
				pickupModelPath = "Prefabs/PickupModels/PickupSteak",
				pickupIconPath = "Textures/ItemIcons/texSteakIcon",
				tags = new ItemTag[]
				{
					ItemTag.Healing,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.Pearl, new ItemDef
			{
				tier = ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupPearl",
				pickupIconPath = "Textures/ItemIcons/texPearlIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.WorldUnique
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.ShinyPearl, new ItemDef
			{
				tier = ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupShinyPearl",
				pickupIconPath = "Textures/ItemIcons/texShinyPearlIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.Healing,
					ItemTag.Utility,
					ItemTag.WorldUnique
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.BonusGoldPackOnKill, new ItemDef
			{
				tier = ItemTier.Tier2,
				pickupModelPath = "Prefabs/PickupModels/PickupTome",
				pickupIconPath = "Textures/ItemIcons/texTomeIcon",
				tags = new ItemTag[]
				{
					ItemTag.Utility,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.LaserTurbine, new ItemDef
			{
				tier = ItemTier.Tier3,
				pickupModelPath = "Prefabs/PickupModels/PickupLaserTurbine",
				pickupIconPath = "Textures/ItemIcons/texLaserTurbineIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage,
					ItemTag.OnKillEffect
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.LunarPrimaryReplacement, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupBirdEye",
				pickupIconPath = "Textures/ItemIcons/texBirdEyeIcon",
				unlockableName = "Items.LunarSkillReplacements",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.NovaOnLowHealth, new ItemDef
			{
				tier = ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupJellyGuts",
				pickupIconPath = "Textures/ItemIcons/texJellyGutsIcon",
				tags = new ItemTag[]
				{
					ItemTag.Damage
				}
			});
			ItemCatalog.RegisterItem(ItemIndex.LunarTrinket, new ItemDef
			{
				tier = ItemTier.Lunar,
				pickupModelPath = "Prefabs/PickupModels/PickupBeads",
				pickupIconPath = "Textures/ItemIcons/texBeadsIcon",
				unlockableName = "Characters.Mercenary"
			});
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				if (ItemCatalog.GetItemDef(itemIndex) == null)
				{
					Debug.LogErrorFormat("Item {0} is unregistered!", new object[]
					{
						itemIndex
					});
				}
				itemIndex++;
			}
		}

		// Token: 0x040015D6 RID: 5590
		public static List<ItemIndex> tier1ItemList = new List<ItemIndex>();

		// Token: 0x040015D7 RID: 5591
		public static List<ItemIndex> tier2ItemList = new List<ItemIndex>();

		// Token: 0x040015D8 RID: 5592
		public static List<ItemIndex> tier3ItemList = new List<ItemIndex>();

		// Token: 0x040015D9 RID: 5593
		public static List<ItemIndex> lunarItemList = new List<ItemIndex>();

		// Token: 0x040015DA RID: 5594
		private static ItemDef[] itemDefs = Array.Empty<ItemDef>();

		// Token: 0x040015DC RID: 5596
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x040015DD RID: 5597
		public static readonly CatalogModHelper<ItemDef> modHelper = new CatalogModHelper<ItemDef>(delegate(int i, ItemDef def)
		{
			ItemCatalog.RegisterItem((ItemIndex)i, def);
		}, (ItemDef v) => v.name);

		// Token: 0x040015DE RID: 5598
		private static readonly Dictionary<string, ItemIndex> itemNameToIndex = new Dictionary<string, ItemIndex>();

		// Token: 0x040015DF RID: 5599
		private static readonly Stack<ItemIndex[]> itemOrderBuffers = new Stack<ItemIndex[]>();

		// Token: 0x040015E0 RID: 5600
		private static readonly Stack<int[]> itemStackArrays = new Stack<int[]>();

		// Token: 0x040015E1 RID: 5601
		public static readonly GenericStaticEnumerable<ItemIndex, ItemCatalog.AllItemsEnumerator> allItems;

		// Token: 0x020003B2 RID: 946
		public struct AllItemsEnumerator : IEnumerator<ItemIndex>, IEnumerator, IDisposable
		{
			// Token: 0x060016E7 RID: 5863 RVA: 0x00063A66 File Offset: 0x00061C66
			public bool MoveNext()
			{
				this.position++;
				return this.position < (ItemIndex)ItemCatalog.itemCount;
			}

			// Token: 0x060016E8 RID: 5864 RVA: 0x00063A83 File Offset: 0x00061C83
			public void Reset()
			{
				this.position = ItemIndex.None;
			}

			// Token: 0x170002AD RID: 685
			// (get) Token: 0x060016E9 RID: 5865 RVA: 0x00063A8C File Offset: 0x00061C8C
			public ItemIndex Current
			{
				get
				{
					return this.position;
				}
			}

			// Token: 0x170002AE RID: 686
			// (get) Token: 0x060016EA RID: 5866 RVA: 0x00063A94 File Offset: 0x00061C94
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x060016EB RID: 5867 RVA: 0x0000409B File Offset: 0x0000229B
			void IDisposable.Dispose()
			{
			}

			// Token: 0x040015E2 RID: 5602
			private ItemIndex position;
		}
	}
}
