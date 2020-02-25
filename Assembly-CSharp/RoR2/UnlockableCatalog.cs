using System;
using System.Collections.Generic;
using System.Linq;

namespace RoR2
{
	// Token: 0x02000462 RID: 1122
	public static class UnlockableCatalog
	{
		// Token: 0x06001B29 RID: 6953 RVA: 0x00072CB2 File Offset: 0x00070EB2
		private static void RegisterUnlockable(string name, UnlockableDef unlockableDef)
		{
			unlockableDef.name = name;
			unlockableDef.index = new UnlockableIndex(UnlockableCatalog.nameToDefTable.Count);
			UnlockableCatalog.nameToDefTable.Add(name, unlockableDef);
		}

		// Token: 0x06001B2A RID: 6954 RVA: 0x00072CDC File Offset: 0x00070EDC
		public static UnlockableDef GetUnlockableDef(string name)
		{
			UnlockableDef result;
			UnlockableCatalog.nameToDefTable.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x00072CF8 File Offset: 0x00070EF8
		public static UnlockableDef GetUnlockableDef(UnlockableIndex index)
		{
			return UnlockableCatalog.indexToDefTable[index.value];
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06001B2C RID: 6956 RVA: 0x00072D07 File Offset: 0x00070F07
		public static int unlockableCount
		{
			get
			{
				return UnlockableCatalog.indexToDefTable.Length;
			}
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x00072D10 File Offset: 0x00070F10
		[SystemInitializer(new Type[]
		{
			typeof(SurvivorCatalog)
		})]
		private static void Init()
		{
			UnlockableCatalog.RegisterUnlockable("Logs.BeetleBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BEETLE"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.BeetleGuardBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BEETLEGUARD"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.BeetleQueenBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BEETLEQUEEN"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.BisonBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BISON"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ClayBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_CLAY"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ClayBossBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_CLAYBOSS"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.GolemBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_GOLEM"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.TitanBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_TITAN"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.TitanGoldBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_TITANGOLD"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ImpBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_IMP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.JellyfishBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_JELLYFISH"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.VagrantBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_VAGRANT"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.LemurianBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_LEMURIAN"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.MagmaWormBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_MAGMAWORM"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.WispBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_WISP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.GreaterWispBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_GREATERWISP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.AncientWispBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_ANCIENTWISP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.HermitCrabBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_HERMITCRAB"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.BellBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_BELL"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.LemurianBruiserBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_LEMURIANBRUISER"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ImpBossBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_IMPBOSS"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ShopkeeperBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_SHOPKEEPER"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ElectricWormBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_ELECTRICWORM"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.ClayBruiserBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_CLAYBRUISER"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.GravekeeperBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_GRAVEKEEPER"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.VultureBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_VULTURE"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.RoboBallBossBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_ROBOBALLBOSS"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.RoboBallMiniBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_ROBOBALLMINI"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.SuperRoboBallBossBody.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_SUPERROBOBALLBOSS"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Nullifier.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_NULLIFIER"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Scav.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_SCAV"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Huntress", new UnlockableDef
			{
				nameToken = "HUNTRESS_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Bandit", new UnlockableDef
			{
				nameToken = "BANDIT_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Engineer", new UnlockableDef
			{
				nameToken = "ENGI_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Mercenary", new UnlockableDef
			{
				nameToken = "MERC_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Mage", new UnlockableDef
			{
				nameToken = "MAGE_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Toolbot", new UnlockableDef
			{
				nameToken = "TOOLBOT_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Treebot", new UnlockableDef
			{
				nameToken = "TREEBOT_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Loader", new UnlockableDef
			{
				nameToken = "LOADER_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Characters.Croco", new UnlockableDef
			{
				nameToken = "CROCO_BODY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.ExtraLife", new UnlockableDef
			{
				nameToken = "ITEM_EXTRALIFE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.BFG", new UnlockableDef
			{
				nameToken = "EQUIPMENT_BFG_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.ShockNearby", new UnlockableDef
			{
				nameToken = "ITEM_SHOCKNEARBY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.AttackSpeedOnCrit", new UnlockableDef
			{
				nameToken = "ITEM_ATTACKSPEEDONCRIT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Infusion", new UnlockableDef
			{
				nameToken = "ITEM_INFUSION_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Medkit", new UnlockableDef
			{
				nameToken = "ITEM_MEDKIT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Bear", new UnlockableDef
			{
				nameToken = "ITEM_BEAR_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Clover", new UnlockableDef
			{
				nameToken = "ITEM_CLOVER_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.GoldGat", new UnlockableDef
			{
				nameToken = "EQUIPMENT_GOLDGAT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.EquipmentMagazine", new UnlockableDef
			{
				nameToken = "ITEM_EQUIPMENTMAGAZINE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.DroneBackup", new UnlockableDef
			{
				nameToken = "EQUIPMENT_DRONEBACKUP_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Firework", new UnlockableDef
			{
				nameToken = "ITEM_FIREWORK_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.WarCryOnMultiKill", new UnlockableDef
			{
				nameToken = "ITEM_WARCRYONMULTIKILL_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Lightning", new UnlockableDef
			{
				nameToken = "EQUIPMENT_LIGHTNING_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.PassiveHealing", new UnlockableDef
			{
				nameToken = "EQUIPMENT_PASSIVEHEALING_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Meteor", new UnlockableDef
			{
				nameToken = "EQUIPMENT_METEOR_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.BurnNearby", new UnlockableDef
			{
				nameToken = "EQUIPMENT_BURNNEARBY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.AutoCastEquipment", new UnlockableDef
			{
				nameToken = "ITEM_AUTOCASTEQUIPMENT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.HealOnCrit", new UnlockableDef
			{
				nameToken = "EQUIPMENT_HEALONCRIT_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.ElementalRings", new UnlockableDef
			{
				nameToken = "ITEM_ELEMENTALRINGS_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Scanner", new UnlockableDef
			{
				nameToken = "EQUIPMENT_SCANNER_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Talisman", new UnlockableDef
			{
				nameToken = "ITEM_TALISMAN_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.BossDamageBonus", new UnlockableDef
			{
				nameToken = "ITEM_BOSSDAMAGEBONUS_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.KillEliteFrenzy", new UnlockableDef
			{
				nameToken = "ITEM_KILLELITEFRENZY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.SecondarySkillMagazine", new UnlockableDef
			{
				nameToken = "ITEM_SECONDARYSKILLMAGAZINE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.JumpBoost", new UnlockableDef
			{
				nameToken = "ITEM_JUMPBOOST_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Crowbar", new UnlockableDef
			{
				nameToken = "ITEM_CROWBAR_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Hoof", new UnlockableDef
			{
				nameToken = "ITEM_HOOF_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.BounceNearby", new UnlockableDef
			{
				nameToken = "ITEM_BOUNCENEARBY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.TreasureCache", new UnlockableDef
			{
				nameToken = "ITEM_TREASURECACHE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.RepeatHeal", new UnlockableDef
			{
				nameToken = "ITEM_REPEATHEAL_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.NovaOnHeal", new UnlockableDef
			{
				nameToken = "ITEM_NOVAONHEAL_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.CrippleWardOnLevel", new UnlockableDef
			{
				nameToken = "ITEM_CRIPPLEWARDONLEVEL_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.IncreaseHealing", new UnlockableDef
			{
				nameToken = "ITEM_INCREASEHEALING_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Gateway", new UnlockableDef
			{
				nameToken = "EQUIPMENT_GATEWAY_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Tonic", new UnlockableDef
			{
				nameToken = "EQUIPMENT_TONIC_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.ExecuteLowHealthElite", new UnlockableDef
			{
				nameToken = "ITEM_EXECUTELOWHEALTHELITE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.EnergizedOnEquipmentUse", new UnlockableDef
			{
				nameToken = "ITEM_ENERGIZEDONEQUIPMENTUSE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.Cleanse", new UnlockableDef
			{
				nameToken = "EQUIPMENT_CLEANSE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.LunarSkillReplacements", new UnlockableDef
			{
				nameToken = "ITEM_LUNARSKILLREPLACEMENTS_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Commando.FireShotgunBlast", new UnlockableDef
			{
				nameToken = "COMMANDO_SECONDARY_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Commando.ThrowGrenade", new UnlockableDef
			{
				nameToken = "COMMANDO_SPECIAL_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Toolbot.Grenade", new UnlockableDef
			{
				nameToken = "TOOLBOT_PRIMARY_ALT2_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Toolbot.Buzzsaw", new UnlockableDef
			{
				nameToken = "TOOLBOT_PRIMARY_ALT3_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Merc.Uppercut", new UnlockableDef
			{
				nameToken = "MERC_SECONDARY_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Merc.EvisProjectile", new UnlockableDef
			{
				nameToken = "MERC_SPECIAL_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Huntress.MiniBlink", new UnlockableDef
			{
				nameToken = "HUNTRESS_UTILITY_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Huntress.Snipe", new UnlockableDef
			{
				nameToken = "HUNTRESS_SPECIAL_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Engi.WalkerTurret", new UnlockableDef
			{
				nameToken = "ENGI_SPECIAL_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Engi.SpiderMine", new UnlockableDef
			{
				nameToken = "ENGI_SPIDERMINE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Treebot.Barrage", new UnlockableDef
			{
				nameToken = "TREEBOT_SECONDARY_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Treebot.PlantSonicBoom", new UnlockableDef
			{
				nameToken = "TREEBOT_UTILITY_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Mage.IceBomb", new UnlockableDef
			{
				nameToken = "MAGE_SECONDARY_ICE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Mage.FlyUp", new UnlockableDef
			{
				nameToken = "MAGE_SPECIAL_LIGHTNING_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Mage.LightningBolt", new UnlockableDef
			{
				nameToken = "MAGE_PRIMARY_ICE_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Loader.YankHook", new UnlockableDef
			{
				nameToken = "LOADER_YANKHOOK_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Loader.ZapFist", new UnlockableDef
			{
				nameToken = "LOADER_UTILITY_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skills.Croco.ChainableLeap", new UnlockableDef
			{
				nameToken = "CROCO_SKILL_CHAINABLELEAP_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skins.Commando.Alt1", new UnlockableDef
			{
				nameToken = "COMMANDO_SKIN_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skins.Huntress.Alt1", new UnlockableDef
			{
				nameToken = "HUNTRESS_SKIN_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skins.Mage.Alt1", new UnlockableDef
			{
				nameToken = "MAGE_SKIN_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skins.Merc.Alt1", new UnlockableDef
			{
				nameToken = "MERC_SKIN_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skins.Toolbot.Alt1", new UnlockableDef
			{
				nameToken = "TOOLBOT_SKIN_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skins.Treebot.Alt1", new UnlockableDef
			{
				nameToken = "TREEBOT_SKIN_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skins.Loader.Alt1", new UnlockableDef
			{
				nameToken = "LOADER_SKIN_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Skins.Croco.Alt1", new UnlockableDef
			{
				nameToken = "CROCO_SKIN_ALT1_NAME"
			});
			UnlockableCatalog.RegisterUnlockable("Items.ShieldOnly", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_BLUEPRINT_SHIELDONLY",
				displayModelPath = "Prefabs/PickupModels/PickupShieldBug"
			});
			UnlockableCatalog.RegisterUnlockable("Shop.BonusLunar.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_SHOP_BONUS_LUNAR_1"
			});
			UnlockableCatalog.RegisterUnlockable("Shop.BonusLunar.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_SHOP_BONUS_LUNAR_2"
			});
			UnlockableCatalog.RegisterUnlockable("Shop.BonusLunar.3", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_SHOP_BONUS_LUNAR_3"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.blackbeach", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_BLACKBEACH"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.goolake", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_GOOLAKE"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.bazaar", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_BAZAAR"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.frozenwall", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_FROZENWALL"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.golemplains", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_GOLEMPLAINS"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.foggyswamp", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_FOGGYSWAMP"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.dampcavesimple", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_DAMPCAVE"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.mysteryspace", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_MYSTERYSPACE"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.wispgraveyard", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_WISPGRAVEYARD"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.goldshores", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_GOLDSHORES"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.shipgraveyard", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_SHIPGRAVEYARD"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.arena", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_ARENA"
			});
			UnlockableCatalog.RegisterUnlockable("Logs.Stages.limbo", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_LOG_STAGES_LIMBO"
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.blackbeach.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.blackbeach.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.blackbeach.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.goolake.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.goolake.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.goolake.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.bazaar.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.bazaar.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.bazaar.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.frozenwall.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.frozenwall.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.frozenwall.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.foggyswamp.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.foggyswamp.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.foggyswamp.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.dampcavesimple.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.dampcavesimple.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.dampcavesimple.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.wispgraveyard.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.wispgraveyard.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.wispgraveyard.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.shipgraveyard.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.shipgraveyard.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.shipgraveyard.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains2.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains2.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains2.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.golemplains2.3", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.blackbeach2.0", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.blackbeach2.1", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.RegisterUnlockable("NewtStatue.blackbeach2.2", new UnlockableDef
			{
				nameToken = "UNLOCKABLE_NEWTSTATUE",
				hidden = true
			});
			UnlockableCatalog.indexToDefTable = new UnlockableDef[UnlockableCatalog.nameToDefTable.Count];
			foreach (KeyValuePair<string, UnlockableDef> keyValuePair in UnlockableCatalog.nameToDefTable)
			{
				UnlockableCatalog.indexToDefTable[keyValuePair.Value.index.value] = keyValuePair.Value;
			}
			for (int i = 0; i < UnlockableCatalog.indexToDefTable.Length; i++)
			{
				UnlockableDef unlockableDef = UnlockableCatalog.indexToDefTable[i];
				unlockableDef.sortScore = UnlockableCatalog.GuessUnlockableSortScore(unlockableDef);
			}
		}

		// Token: 0x06001B2E RID: 6958 RVA: 0x00073E9C File Offset: 0x0007209C
		public static int GetUnlockableSortScore(string unlockableName)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
			if (unlockableDef == null)
			{
				return 0;
			}
			return unlockableDef.sortScore;
		}

		// Token: 0x06001B2F RID: 6959 RVA: 0x00073EB0 File Offset: 0x000720B0
		private static int GuessUnlockableSortScore(UnlockableDef unlockableDef)
		{
			int num = 0;
			int num2 = num + 200;
			int num3 = num2 + 200;
			int num4 = num3 + 200;
			int result = num4 + 200;
			if (unlockableDef == null)
			{
				return 10000;
			}
			string name = unlockableDef.name;
			int i = 0;
			while (i < SurvivorCatalog.idealSurvivorOrder.Length)
			{
				SurvivorIndex survivorIndex = SurvivorCatalog.idealSurvivorOrder[i];
				if (name.Contains(survivorIndex.ToString()))
				{
					if (SurvivorCatalog.GetSurvivorDef(survivorIndex).unlockableName == name)
					{
						return num3 + i * 10;
					}
					int num5 = num4 + i * 10;
					if (name.Contains("Skin"))
					{
						num5++;
					}
					return num5;
				}
				else
				{
					i++;
				}
			}
			ItemDef itemDef = ItemCatalog.allItems.Select(new Func<ItemIndex, ItemDef>(ItemCatalog.GetItemDef)).FirstOrDefault((ItemDef v) => v.unlockableName == unlockableDef.name);
			if (itemDef != null)
			{
				return (int)(num + itemDef.tier);
			}
			EquipmentDef equipmentDef = EquipmentCatalog.allEquipment.Select(new Func<EquipmentIndex, EquipmentDef>(EquipmentCatalog.GetEquipmentDef)).FirstOrDefault((EquipmentDef v) => v.unlockableName == unlockableDef.name);
			if (equipmentDef == null)
			{
				return result;
			}
			if (equipmentDef.isBoss)
			{
				return num2 + 1;
			}
			if (equipmentDef.isLunar)
			{
				return num2 - 1;
			}
			return num2;
		}

		// Token: 0x04001898 RID: 6296
		private static readonly Dictionary<string, UnlockableDef> nameToDefTable = new Dictionary<string, UnlockableDef>();

		// Token: 0x04001899 RID: 6297
		private static UnlockableDef[] indexToDefTable;

		// Token: 0x0400189A RID: 6298
		public static ResourceAvailability availability;
	}
}
