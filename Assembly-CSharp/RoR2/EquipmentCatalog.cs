using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000245 RID: 581
	public static class EquipmentCatalog
	{
		// Token: 0x06000AEA RID: 2794 RVA: 0x00035C0C File Offset: 0x00033E0C
		static EquipmentCatalog()
		{
			EquipmentCatalog.equipmentDefs = new EquipmentDef[27];
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
			EquipmentCatalog.RegisterEquipment(EquipmentIndex.AffixGreen, new EquipmentDef
			{
				cooldown = 10f,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixGreen",
				pickupIconPath = "Textures/ItemIcons/texAffixGreenIcon",
				nameToken = "EQUIPMENT_AFFIXGREEN_NAME",
				pickupToken = "EQUIPMENT_AFFIXGREEN_PICKUP",
				descriptionToken = "EQUIPMENT_AFFIXGREEN_DESC",
				addressToken = "",
				canDrop = false,
				enigmaCompatible = false,
				passiveBuff = BuffIndex.None
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
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x000366D8 File Offset: 0x000348D8
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
			string arg = equipmentIndex.ToString().ToUpper();
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
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x000367CB File Offset: 0x000349CB
		public static EquipmentDef GetEquipmentDef(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= EquipmentIndex.Count)
			{
				return null;
			}
			return EquipmentCatalog.equipmentDefs[(int)equipmentIndex];
		}

		// Token: 0x04000EDF RID: 3807
		private static EquipmentDef[] equipmentDefs;

		// Token: 0x04000EE0 RID: 3808
		public static List<EquipmentIndex> equipmentList = new List<EquipmentIndex>();

		// Token: 0x04000EE1 RID: 3809
		public static List<EquipmentIndex> enigmaEquipmentList = new List<EquipmentIndex>();

		// Token: 0x04000EE2 RID: 3810
		public static readonly GenericStaticEnumerable<EquipmentIndex, EquipmentCatalog.AllEquipmentEnumerator> allEquipment;

		// Token: 0x02000246 RID: 582
		public struct AllEquipmentEnumerator : IEnumerator<EquipmentIndex>, IEnumerator, IDisposable
		{
			// Token: 0x06000AED RID: 2797 RVA: 0x000367DF File Offset: 0x000349DF
			public bool MoveNext()
			{
				this.position++;
				return this.position < EquipmentIndex.Count;
			}

			// Token: 0x06000AEE RID: 2798 RVA: 0x000367F9 File Offset: 0x000349F9
			public void Reset()
			{
				this.position = EquipmentIndex.None;
			}

			// Token: 0x170000C2 RID: 194
			// (get) Token: 0x06000AEF RID: 2799 RVA: 0x00036802 File Offset: 0x00034A02
			public EquipmentIndex Current
			{
				get
				{
					return this.position;
				}
			}

			// Token: 0x170000C3 RID: 195
			// (get) Token: 0x06000AF0 RID: 2800 RVA: 0x0003680A File Offset: 0x00034A0A
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x06000AF1 RID: 2801 RVA: 0x00004507 File Offset: 0x00002707
			void IDisposable.Dispose()
			{
			}

			// Token: 0x04000EE3 RID: 3811
			private EquipmentIndex position;
		}
	}
}
