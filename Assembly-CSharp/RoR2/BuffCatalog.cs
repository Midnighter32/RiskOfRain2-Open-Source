using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000D2 RID: 210
	public static class BuffCatalog
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000413 RID: 1043 RVA: 0x0000FFCC File Offset: 0x0000E1CC
		// (set) Token: 0x06000414 RID: 1044 RVA: 0x0000FFD3 File Offset: 0x0000E1D3
		public static int buffCount { get; private set; }

		// Token: 0x06000415 RID: 1045 RVA: 0x0000FFDB File Offset: 0x0000E1DB
		private static void RegisterBuff(BuffIndex buffIndex, BuffDef buffDef)
		{
			if (buffIndex < BuffIndex.Count)
			{
				buffDef.name = buffIndex.ToString();
			}
			buffDef.buffIndex = buffIndex;
			BuffCatalog.buffDefs[(int)buffIndex] = buffDef;
			BuffCatalog.nameToBuffIndex[buffDef.name] = buffIndex;
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00010015 File Offset: 0x0000E215
		public static BuffDef GetBuffDef(BuffIndex buffIndex)
		{
			return HGArrayUtilities.GetSafe<BuffDef>(BuffCatalog.buffDefs, (int)buffIndex);
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x00010024 File Offset: 0x0000E224
		public static BuffIndex FindBuffIndex(string buffName)
		{
			BuffIndex result;
			if (BuffCatalog.nameToBuffIndex.TryGetValue(buffName, out result))
			{
				return result;
			}
			return BuffIndex.None;
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x00010043 File Offset: 0x0000E243
		public static T[] GetPerBuffBuffer<T>()
		{
			return new T[BuffCatalog.buffCount];
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00010050 File Offset: 0x0000E250
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			BuffCatalog.nameToBuffIndex.Clear();
			BuffCatalog.buffDefs = new BuffDef[51];
			BuffCatalog.buffCount = BuffCatalog.buffDefs.Length;
			BuffCatalog.RegisterBuff(BuffIndex.ArmorBoost, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffGenericShield",
				buffColor = new Color(0.8392157f, 0.7882353f, 0.22745098f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow50, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.91764706f, 0.40784314f, 0.41960785f),
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.ClayGoo, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.2f, 0.09019608f, 0.09019608f),
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.AttackSpeedOnCrit, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffAttackSpeedOnCritIcon",
				buffColor = new Color(0.9098039f, 0.5058824f, 0.23921569f),
				canStack = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.BeetleJuice, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffBeetleJuiceIcon",
				buffColor = new Color(0.8392157f, 0.7882353f, 0.22745098f),
				canStack = true,
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.OnFire, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffOnFireIcon",
				buffColor = new Color(0.9137255f, 0.37254903f, 0.1882353f),
				canStack = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.MedkitHeal, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffMedkitHealIcon",
				buffColor = new Color(0.78431374f, 0.9372549f, 0.42745098f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Warbanner, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffWarbannerIcon",
				buffColor = new Color(0.8392157f, 0.7882353f, 0.22745098f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.EnrageAncientWisp, new BuffDef());
			BuffCatalog.RegisterBuff(BuffIndex.Cloak, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffCloakIcon",
				buffColor = new Color(0.3764706f, 0.84313726f, 0.8980392f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.CloakSpeed, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texMovespeedBuffIcon",
				buffColor = new Color(0.3764706f, 0.84313726f, 0.8980392f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.LightningShield, new BuffDef
			{
				iconPath = null
			});
			BuffCatalog.RegisterBuff(BuffIndex.FullCrit, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffFullCritIcon",
				buffColor = new Color(0.8392157f, 0.22745098f, 0.22745098f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixPoison, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffAffixPoisonIcon",
				eliteIndex = EliteIndex.Poison
			});
			BuffCatalog.RegisterBuff(BuffIndex.EngiShield, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffEngiShieldIcon",
				buffColor = new Color(0.3764706f, 0.84313726f, 0.8980392f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.BugWings, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texMovespeedBuffIcon",
				buffColor = new Color(0.3764706f, 0.84313726f, 0.8980392f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.TeslaField, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffTeslaIcon",
				buffColor = new Color(0.85882354f, 0.53333336f, 0.9843137f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.WarCryBuff, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texWarcryBuffIcon",
				buffColor = new Color(0.827451f, 0.19607843f, 0.09803922f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow30, new BuffDef
			{
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.EngiTeamShield, new BuffDef());
			BuffCatalog.RegisterBuff(BuffIndex.Energized, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffWarHornIcon",
				buffColor = new Color(1f, 0.7882353f, 0.05490196f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.GoldEmpowered, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffAttackSpeedOnCritIcon",
				buffColor = new Color(1f, 0.7882353f, 0.05490196f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Immune, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffGenericShield",
				buffColor = new Color(1f, 0.7882353f, 0.05490196f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Cripple, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffCrippleIcon",
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow80, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.64705884f, 0.87058824f, 0.92941177f),
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow60, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.6784314f, 0.6117647f, 0.4117647f),
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixRed, new BuffDef
			{
				eliteIndex = EliteIndex.Fire,
				iconPath = "Textures/BuffIcons/texBuffAffixRed"
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixBlue, new BuffDef
			{
				eliteIndex = EliteIndex.Lightning,
				iconPath = "Textures/BuffIcons/texBuffAffixBlue"
			});
			BuffCatalog.RegisterBuff(BuffIndex.NoCooldowns, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texNoCooldownsBuffIcon",
				buffColor = new Color(0.73333335f, 0.54509807f, 0.9764706f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixWhite, new BuffDef
			{
				eliteIndex = EliteIndex.Ice,
				iconPath = "Textures/BuffIcons/texBuffAffixWhite"
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixHaunted, new BuffDef
			{
				eliteIndex = EliteIndex.Haunted,
				iconPath = "Textures/BuffIcons/texBuffAffixHaunted"
			});
			BuffCatalog.RegisterBuff(BuffIndex.HiddenInvincibility, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffGenericShield",
				buffColor = new Color(0.54509807f, 0.80784315f, 0.8392157f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.TonicBuff, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffTonicIcon"
			});
			BuffCatalog.RegisterBuff(BuffIndex.HealingDisabled, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffHealingDisabledIcon",
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.Weak, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffWeakIcon",
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.Entangle, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffEntangleIcon",
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.Pulverized, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffPulverizeIcon",
				buffColor = new Color(0.9607843f, 0.62352943f, 0.28627452f),
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.PulverizeBuildup, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffPulverizeIcon",
				buffColor = new Color(0.61960787f, 0.54901963f, 0.40392157f),
				canStack = true,
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.LoaderOvercharged, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffTeslaIcon",
				buffColor = Color.yellow,
				canStack = false
			});
			BuffCatalog.RegisterBuff(BuffIndex.LoaderPylonPowered, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffTeslaIcon",
				buffColor = Color.yellow,
				canStack = false
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixHauntedRecipient, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffCloakIcon",
				buffColor = new Color(0.3764706f, 0.84313726f, 0.8980392f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Deafened, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffCloakIcon",
				buffColor = Color.gray
			});
			BuffCatalog.RegisterBuff(BuffIndex.Intangible, new BuffDef
			{
				iconPath = null,
				buffColor = Color.gray
			});
			BuffCatalog.RegisterBuff(BuffIndex.ElephantArmorBoost, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffElephantArmorBoostIcon",
				buffColor = Color.white
			});
			BuffCatalog.RegisterBuff(BuffIndex.NullifyStack, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffNullifyStackIcon",
				buffColor = new Color(0.5176471f, 0.34117648f, 0.70980394f),
				isDebuff = true,
				canStack = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.Nullified, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffNullifiedIcon",
				buffColor = new Color(0.79607844f, 0.4745098f, 0.8352941f),
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.MeatRegenBoost, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffRegenBoostIcon",
				buffColor = new Color(0.78431374f, 0.9372549f, 0.42745098f),
				isDebuff = false,
				canStack = false
			});
			BuffCatalog.RegisterBuff(BuffIndex.NullSafeZone, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffNullifiedIcon",
				buffColor = new Color(0.79607844f, 0.4745098f, 0.8352941f),
				isDebuff = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.Bleeding, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffBleedingIcon",
				buffColor = new Color(0.67058825f, 0.16862746f, 0.16862746f, 1f),
				canStack = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.Poisoned, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffBleedingIcon",
				buffColor = new Color(0.7882353f, 0.9490196f, 0.3019608f),
				canStack = false
			});
			BuffCatalog.RegisterBuff(BuffIndex.WhipBoost, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texMovespeedBuffIcon",
				buffColor = new Color(0.3764706f, 0.84313726f, 0.8980392f)
			});
			for (BuffIndex buffIndex = BuffIndex.Slow50; buffIndex < BuffIndex.Count; buffIndex++)
			{
				if (BuffCatalog.buffDefs[(int)buffIndex] == null)
				{
					Debug.LogWarningFormat("Unregistered buff {0}!", new object[]
					{
						buffIndex.ToString()
					});
				}
			}
			BuffCatalog.modHelper.CollectAndRegisterAdditionalEntries(ref BuffCatalog.buffDefs);
			BuffCatalog.buffCount = BuffCatalog.buffDefs.Length;
			BuffCatalog.eliteBuffIndices = (from buffDef in BuffCatalog.buffDefs
			where buffDef.isElite
			select buffDef.buffIndex).ToArray<BuffIndex>();
		}

		// Token: 0x040003DD RID: 989
		private static BuffDef[] buffDefs;

		// Token: 0x040003DF RID: 991
		public static BuffIndex[] eliteBuffIndices;

		// Token: 0x040003E0 RID: 992
		private static readonly Dictionary<string, BuffIndex> nameToBuffIndex = new Dictionary<string, BuffIndex>();

		// Token: 0x040003E1 RID: 993
		public static readonly CatalogModHelper<BuffDef> modHelper = new CatalogModHelper<BuffDef>(delegate(int i, BuffDef def)
		{
			BuffCatalog.RegisterBuff((BuffIndex)i, def);
		}, (BuffDef v) => v.name);
	}
}
