using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000133 RID: 307
	public static class EquipmentCatalog
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000582 RID: 1410 RVA: 0x0001632B File Offset: 0x0001452B
		// (set) Token: 0x06000583 RID: 1411 RVA: 0x00016332 File Offset: 0x00014532
		public static int equipmentCount { get; private set; }

		// Token: 0x06000584 RID: 1412 RVA: 0x0001633C File Offset: 0x0001453C
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			EquipmentCatalog.equipmentNameToIndex.Clear();
			Array.Resize<EquipmentDef>(ref EquipmentCatalog.equipmentDefs, 34);
			EquipmentCatalog.equipmentCount = EquipmentCatalog.equipmentDefs.Length;
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Blackhole, new EquipmentDef
			{
				cooldown = 60f,
				pickupModelPath = "Prefabs/PickupModels/PickupGravCube",
				pickupIconPath = "Textures/ItemIcons/texGravCubeIcon",
				nameToken = "EQUIPMENT_BLACKHOLE_NAME",
				pickupToken = "EQUIPMENT_BLACKHOLE_PICKUP",
				descriptionToken = "EQUIPMENT_BLACKHOLE_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = true
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.CommandMissile, new EquipmentDef
			{
				cooldown = 45f,
				pickupModelPath = "Prefabs/PickupModels/PickupMissileRack",
				pickupIconPath = "Textures/ItemIcons/texMissileRackIcon",
				nameToken = "EQUIPMENT_COMMANDMISSILE_NAME",
				pickupToken = "EQUIPMENT_COMMANDMISSILE_PICKUP",
				descriptionToken = "EQUIPMENT_COMMANDMISSILE_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = true
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.OrbitalLaser, new EquipmentDef
			{
				cooldown = 1f,
				nameToken = "EQUIPMENT_ORBITALLASER_NAME",
				pickupToken = "EQUIPMENT_ORBITALLASER_PICKUP",
				descriptionToken = "EQUIPMENT_ORBITALLASER_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Saw, new EquipmentDef
			{
				cooldown = 45f,
				nameToken = "EQUIPMENT_SAW_NAME",
				pickupToken = "EQUIPMENT_SAW_PICKUP",
				descriptionToken = "EQUIPMENT_SAW_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Fruit, new EquipmentDef
			{
				cooldown = 45f,
				pickupModelPath = "Prefabs/PickupModels/PickupFruit",
				pickupIconPath = "Textures/ItemIcons/texFruitIcon",
				nameToken = "EQUIPMENT_FRUIT_NAME",
				pickupToken = "EQUIPMENT_FRUIT_PICKUP",
				descriptionToken = "EQUIPMENT_FRUIT_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = true
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Meteor, new EquipmentDef
			{
				cooldown = 140f,
				pickupModelPath = "Prefabs/PickupModels/PickupMeteor",
				pickupIconPath = "Textures/ItemIcons/texMeteorIcon",
				nameToken = "EQUIPMENT_METEOR_NAME",
				pickupToken = "EQUIPMENT_METEOR_PICKUP",
				descriptionToken = "EQUIPMENT_METEOR_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = false,
				isLunar = true,
				colorIndex = ColorCatalog.ColorIndex.LunarItem,
				unlockableName = "Items.Meteor"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.SoulJar, new EquipmentDef
			{
				cooldown = 45f,
				nameToken = "EQUIPMENT_SOULJAR_NAME",
				pickupToken = "EQUIPMENT_SOULJAR_PICKUP",
				descriptionToken = "EQUIPMENT_SOULJAR_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.GhostGun, new EquipmentDef
			{
				cooldown = 10f,
				pickupModelPath = "Prefabs/PickupModels/PickupGhostRevolver",
				nameToken = "EQUIPMENT_GHOSTGUN_NAME",
				pickupToken = "EQUIPMENT_GHOSTGUN_PICKUP",
				descriptionToken = "EQUIPMENT_GHOSTGUN_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = true
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.CritOnUse, new EquipmentDef
			{
				cooldown = 60f,
				pickupModelPath = "Prefabs/PickupModels/PickupNeuralImplant",
				pickupIconPath = "Textures/ItemIcons/texNeuralImplantIcon",
				nameToken = "EQUIPMENT_CRITONUSE_NAME",
				pickupToken = "EQUIPMENT_CRITONUSE_PICKUP",
				descriptionToken = "EQUIPMENT_CRITONUSE_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = true
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.AffixRed, new EquipmentDef
			{
				cooldown = 10f,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixRed",
				pickupIconPath = "Textures/ItemIcons/texAffixRedIcon",
				nameToken = "EQUIPMENT_AFFIXRED_NAME",
				pickupToken = "EQUIPMENT_AFFIXRED_PICKUP",
				descriptionToken = "EQUIPMENT_AFFIXRED_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false,
				passiveBuff = BuffIndex.AffixRed
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.AffixPoison, new EquipmentDef
			{
				cooldown = 10f,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixPoison",
				pickupIconPath = "Textures/ItemIcons/texAffixPoisonIcon",
				nameToken = "EQUIPMENT_AFFIXPOISON_NAME",
				pickupToken = "EQUIPMENT_AFFIXPOISON_PICKUP",
				descriptionToken = "EQUIPMENT_AFFIXPOISON_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false,
				passiveBuff = BuffIndex.AffixPoison
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.AffixBlue, new EquipmentDef
			{
				cooldown = 25f,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixBlue",
				pickupIconPath = "Textures/ItemIcons/texAffixBlueIcon",
				nameToken = "EQUIPMENT_AFFIXBLUE_NAME",
				pickupToken = "EQUIPMENT_AFFIXBLUE_PICKUP",
				descriptionToken = "EQUIPMENT_AFFIXBLUE_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false,
				passiveBuff = BuffIndex.AffixBlue
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.AffixYellow, new EquipmentDef
			{
				cooldown = 25f,
				nameToken = "",
				pickupToken = "",
				descriptionToken = "",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false,
				passiveBuff = BuffIndex.None
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.AffixGold, new EquipmentDef
			{
				cooldown = 25f,
				nameToken = "EQUIPMENT_AFFIXGOLD_NAME",
				pickupToken = "EQUIPMENT_AFFIXGOLD_PICKUP",
				descriptionToken = "EQUIPMENT_AFFIXGOLD_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false,
				passiveBuff = BuffIndex.None
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.AffixWhite, new EquipmentDef
			{
				cooldown = 25f,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixWhite",
				pickupIconPath = "Textures/ItemIcons/texAffixWhiteIcon",
				nameToken = "EQUIPMENT_AFFIXWHITE_NAME",
				pickupToken = "EQUIPMENT_AFFIXWHITE_PICKUP",
				descriptionToken = "EQUIPMENT_AFFIXWHITE_DESC",
				canDrop = false,
				enigmaCompatible = false,
				passiveBuff = BuffIndex.AffixWhite
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.AffixHaunted, new EquipmentDef
			{
				cooldown = 10f,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixHaunted",
				pickupIconPath = "Textures/ItemIcons/texAffixHauntedIcon",
				nameToken = "EQUIPMENT_AFFIXHAUNTED_NAME",
				pickupToken = "EQUIPMENT_AFFIXHAUNTED_PICKUP",
				descriptionToken = "EQUIPMENT_AFFIXHAUNTED_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false,
				passiveBuff = BuffIndex.AffixHaunted
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.DroneBackup, new EquipmentDef
			{
				cooldown = 100f,
				pickupModelPath = "Prefabs/PickupModels/PickupRadio",
				pickupIconPath = "Textures/ItemIcons/texRadioIcon",
				nameToken = "EQUIPMENT_DRONEBACKUP_NAME",
				pickupToken = "EQUIPMENT_DRONEBACKUP_PICKUP",
				descriptionToken = "EQUIPMENT_DRONEBACKUP_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = true,
				unlockableName = "Items.DroneBackup"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.BFG, new EquipmentDef
			{
				cooldown = 140f,
				pickupModelPath = "Prefabs/PickupModels/PickupBFG",
				pickupIconPath = "Textures/ItemIcons/texBFGIcon",
				nameToken = "EQUIPMENT_BFG_NAME",
				pickupToken = "EQUIPMENT_BFG_PICKUP",
				descriptionToken = "EQUIPMENT_BFG_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = true,
				unlockableName = "Items.BFG"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Enigma, new EquipmentDef
			{
				cooldown = 60f,
				pickupIconPath = "Textures/ItemIcons/texEnigmaIcon",
				nameToken = "EQUIPMENT_ENIGMA_NAME",
				pickupToken = "EQUIPMENT_ENIGMA_PICKUP",
				descriptionToken = "EQUIPMENT_ENIGMA_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Jetpack, new EquipmentDef
			{
				cooldown = 60f,
				pickupIconPath = "Textures/ItemIcons/texChrysalisIcon",
				pickupModelPath = "Prefabs/PickupModels/PickupChrysalis",
				nameToken = "EQUIPMENT_JETPACK_NAME",
				pickupToken = "EQUIPMENT_JETPACK_PICKUP",
				descriptionToken = "EQUIPMENT_JETPACK_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = true
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Lightning, new EquipmentDef
			{
				cooldown = 20f,
				pickupIconPath = "Textures/ItemIcons/texCapacitorIcon",
				pickupModelPath = "Prefabs/PickupModels/PickupCapacitor",
				nameToken = "EQUIPMENT_LIGHTNING_NAME",
				pickupToken = "EQUIPMENT_LIGHTNING_PICKUP",
				descriptionToken = "EQUIPMENT_LIGHTNING_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = true,
				mageElement = MageElement.Lightning,
				unlockableName = "Items.Lightning"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.GoldGat, new EquipmentDef
			{
				cooldown = 5f,
				pickupIconPath = "Textures/ItemIcons/texGoldGatIcon",
				pickupModelPath = "Prefabs/PickupModels/PickupGoldGat",
				nameToken = "EQUIPMENT_GOLDGAT_NAME",
				pickupToken = "EQUIPMENT_GOLDGAT_PICKUP",
				descriptionToken = "EQUIPMENT_GOLDGAT_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = false,
				unlockableName = "Items.GoldGat"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.PassiveHealing, new EquipmentDef
			{
				cooldown = 15f,
				pickupIconPath = "Textures/ItemIcons/texWoodspriteIcon",
				pickupModelPath = "Prefabs/PickupModels/PickupWoodsprite",
				nameToken = "EQUIPMENT_PASSIVEHEALING_NAME",
				pickupToken = "EQUIPMENT_PASSIVEHEALING_PICKUP",
				descriptionToken = "EQUIPMENT_PASSIVEHEALING_DESC",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = false,
				appearsInSinglePlayer = true,
				unlockableName = "Items.PassiveHealing"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.LunarPotion, new EquipmentDef
			{
				cooldown = 5f,
				pickupIconPath = null,
				pickupModelPath = null,
				nameToken = "EQUIPMENT_LUNARPOTION_NAME",
				pickupToken = "EQUIPMENT_LUNARPOTION_NAME",
				descriptionToken = "EQUIPMENT_LUNARPOTION_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.BurnNearby, new EquipmentDef
			{
				cooldown = 45f,
				pickupModelPath = "Prefabs/PickupModels/PickupPotion",
				pickupIconPath = "Textures/ItemIcons/texPotionIcon",
				nameToken = "EQUIPMENT_BURNNEARBY_NAME",
				pickupToken = "EQUIPMENT_BURNNEARBY_PICKUP",
				descriptionToken = "EQUIPMENT_BURNNEARBY_DESC",
				unlockableName = "Items.BurnNearby",
				addressToken = "",
				canDrop = true,
				enigmaCompatible = true,
				isLunar = true,
				colorIndex = ColorCatalog.ColorIndex.LunarItem
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.SoulCorruptor, new EquipmentDef
			{
				cooldown = 45f,
				pickupModelPath = null,
				pickupIconPath = null,
				canDrop = false,
				enigmaCompatible = false
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Scanner, new EquipmentDef
			{
				cooldown = 45f,
				pickupModelPath = "Prefabs/PickupModels/PickupScanner",
				pickupIconPath = "Textures/ItemIcons/texScannerIcon",
				canDrop = true,
				enigmaCompatible = true,
				unlockableName = "Items.Scanner"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.CrippleWard, new EquipmentDef
			{
				cooldown = 15f,
				pickupModelPath = "Prefabs/PickupModels/PickupEffigy",
				pickupIconPath = "Textures/ItemIcons/texEffigyIcon",
				canDrop = true,
				enigmaCompatible = true,
				isLunar = true,
				colorIndex = ColorCatalog.ColorIndex.LunarItem
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Gateway, new EquipmentDef
			{
				cooldown = 45f,
				pickupModelPath = "Prefabs/PickupModels/PickupVase",
				pickupIconPath = "Textures/ItemIcons/texVaseIcon",
				canDrop = true,
				enigmaCompatible = true,
				isLunar = false,
				unlockableName = "Items.Gateway"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Tonic, new EquipmentDef
			{
				cooldown = 60f,
				pickupModelPath = "Prefabs/PickupModels/PickupTonic",
				pickupIconPath = "Textures/ItemIcons/texTonicIcon",
				canDrop = true,
				enigmaCompatible = false,
				isLunar = true,
				colorIndex = ColorCatalog.ColorIndex.LunarItem,
				unlockableName = "Items.Tonic"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.QuestVolatileBattery, new EquipmentDef
			{
				cooldown = 60f,
				pickupModelPath = "Prefabs/PickupModels/PickupBatteryArray",
				pickupIconPath = "Textures/ItemIcons/texBatteryArrayIcon",
				canDrop = false,
				enigmaCompatible = false,
				isLunar = false
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.Cleanse, new EquipmentDef
			{
				cooldown = 20f,
				pickupModelPath = "Prefabs/PickupModels/PickupWaterPack",
				pickupIconPath = "Textures/ItemIcons/texWaterPackIcon",
				canDrop = true,
				enigmaCompatible = true,
				isLunar = false,
				unlockableName = "Items.Cleanse"
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.FireBallDash, new EquipmentDef
			{
				cooldown = 30f,
				pickupModelPath = "Prefabs/PickupModels/PickupEgg",
				pickupIconPath = "Textures/ItemIcons/texEggIcon",
				canDrop = true,
				enigmaCompatible = true,
				isLunar = false
			});
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.GainArmor, new EquipmentDef
			{
				cooldown = 45f,
				pickupModelPath = "Prefabs/PickupModels/PickupElephantFigure",
				pickupIconPath = "Textures/ItemIcons/texElephantFigureIcon",
				canDrop = true,
				enigmaCompatible = true,
				isLunar = false
			});
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				if (EquipmentCatalog.GetEquipmentDef(equipmentIndex) == null)
				{
					Debug.LogErrorFormat("Equipment {0} is unregistered!", new object[]
					{
						equipmentIndex
					});
				}
			}
			EquipmentCatalog.modHelper.CollectAndRegisterAdditionalEntries(ref EquipmentCatalog.equipmentDefs);
			EquipmentCatalog.equipmentCount = EquipmentCatalog.equipmentDefs.Length;
			EquipmentCatalog.availability.MakeAvailable();
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x00017050 File Offset: 0x00015250
		private static void RegisterEquipment(EquipmentIndex equipmentIndex, EquipmentDef equipmentDef)
		{
			equipmentDef.equipmentIndex = equipmentIndex;
			EquipmentCatalog.equipmentDefs[(int)equipmentIndex] = equipmentDef;
			if (equipmentDef.canDrop)
			{
				EquipmentCatalog.equipmentList.Add(equipmentIndex);
			}
			if (equipmentDef.enigmaCompatible)
			{
				EquipmentCatalog.enigmaEquipmentList.Add(equipmentIndex);
			}
			if (equipmentDef.name == null)
			{
				equipmentDef.name = equipmentIndex.ToString();
			}
			string name = equipmentDef.name;
			string arg = name.ToUpper(CultureInfo.InvariantCulture);
			if (equipmentDef.nameToken == null)
			{
				equipmentDef.nameToken = string.Format(CultureInfo.InvariantCulture, "EQUIPMENT_{0}_NAME", arg);
			}
			if (equipmentDef.descriptionToken == null)
			{
				equipmentDef.descriptionToken = string.Format(CultureInfo.InvariantCulture, "EQUIPMENT_{0}_DESC", arg);
			}
			if (equipmentDef.pickupToken == null)
			{
				equipmentDef.pickupToken = string.Format(CultureInfo.InvariantCulture, "EQUIPMENT_{0}_PICKUP", arg);
			}
			if (equipmentDef.loreToken == null)
			{
				equipmentDef.loreToken = string.Format(CultureInfo.InvariantCulture, "EQUIPMENT_{0}_LORE", arg);
			}
			if (equipmentDef.pickupModelPath == null)
			{
				equipmentDef.pickupModelPath = "Prefabs/NullModel";
			}
			if (equipmentDef.pickupIconPath == null)
			{
				equipmentDef.pickupIconPath = "Textures/ItemIcons/texNullIcon";
			}
			EquipmentCatalog.equipmentNameToIndex[name] = equipmentIndex;
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x0001716A File Offset: 0x0001536A
		public static EquipmentDef GetEquipmentDef(EquipmentIndex equipmentIndex)
		{
			return HGArrayUtilities.GetSafe<EquipmentDef>(EquipmentCatalog.equipmentDefs, (int)equipmentIndex);
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00017178 File Offset: 0x00015378
		public static EquipmentIndex FindEquipmentIndex(string equipmentName)
		{
			EquipmentIndex result;
			if (EquipmentCatalog.equipmentNameToIndex.TryGetValue(equipmentName, out result))
			{
				return result;
			}
			return EquipmentIndex.None;
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00017197 File Offset: 0x00015397
		public static T[] GetPerEquipmentBuffer<T>()
		{
			return new T[EquipmentCatalog.equipmentCount];
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x000171A3 File Offset: 0x000153A3
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsIndexValid(in EquipmentIndex equipmentIndex)
		{
			return equipmentIndex < (EquipmentIndex)EquipmentCatalog.equipmentCount;
		}

		// Token: 0x040005F3 RID: 1523
		private static EquipmentDef[] equipmentDefs = Array.Empty<EquipmentDef>();

		// Token: 0x040005F4 RID: 1524
		public static List<EquipmentIndex> equipmentList = new List<EquipmentIndex>();

		// Token: 0x040005F5 RID: 1525
		public static List<EquipmentIndex> enigmaEquipmentList = new List<EquipmentIndex>();

		// Token: 0x040005F7 RID: 1527
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x040005F8 RID: 1528
		private static readonly Dictionary<string, EquipmentIndex> equipmentNameToIndex = new Dictionary<string, EquipmentIndex>();

		// Token: 0x040005F9 RID: 1529
		public static readonly CatalogModHelper<EquipmentDef> modHelper = new CatalogModHelper<EquipmentDef>(delegate(int i, EquipmentDef def)
		{
			EquipmentCatalog.RegisterEquipment((EquipmentIndex)i, def);
		}, (EquipmentDef v) => v.name);

		// Token: 0x040005FA RID: 1530
		public static readonly GenericStaticEnumerable<EquipmentIndex, EquipmentCatalog.AllEquipmentEnumerator> allEquipment;

		// Token: 0x02000134 RID: 308
		public struct AllEquipmentEnumerator : IEnumerator<EquipmentIndex>, IEnumerator, IDisposable
		{
			// Token: 0x0600058B RID: 1419 RVA: 0x0001721A File Offset: 0x0001541A
			public bool MoveNext()
			{
				this.position++;
				return this.position < (EquipmentIndex)EquipmentCatalog.equipmentCount;
			}

			// Token: 0x0600058C RID: 1420 RVA: 0x00017237 File Offset: 0x00015437
			public void Reset()
			{
				this.position = EquipmentIndex.None;
			}

			// Token: 0x170000AC RID: 172
			// (get) Token: 0x0600058D RID: 1421 RVA: 0x00017240 File Offset: 0x00015440
			public EquipmentIndex Current
			{
				get
				{
					return this.position;
				}
			}

			// Token: 0x170000AD RID: 173
			// (get) Token: 0x0600058E RID: 1422 RVA: 0x00017248 File Offset: 0x00015448
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x0600058F RID: 1423 RVA: 0x0000409B File Offset: 0x0000229B
			void IDisposable.Dispose()
			{
			}

			// Token: 0x040005FB RID: 1531
			private EquipmentIndex position;
		}
	}
}
