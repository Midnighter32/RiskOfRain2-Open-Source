using System;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000209 RID: 521
	public static class BuffCatalog
	{
		// Token: 0x06000A2B RID: 2603 RVA: 0x00032791 File Offset: 0x00030991
		private static void RegisterBuff(BuffIndex buffIndex, BuffDef buffDef)
		{
			buffDef.buffIndex = buffIndex;
			BuffCatalog.buffDefs[(int)buffIndex] = buffDef;
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x000327A2 File Offset: 0x000309A2
		public static BuffDef GetBuffDef(BuffIndex buffIndex)
		{
			if (buffIndex < BuffIndex.Slow50 || buffIndex > BuffIndex.Count)
			{
				return null;
			}
			return BuffCatalog.buffDefs[(int)buffIndex];
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x000327B8 File Offset: 0x000309B8
		static BuffCatalog()
		{
			BuffCatalog.RegisterBuff(BuffIndex.ArmorBoost, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffGenericShield",
				buffColor = new Color(0.8392157f, 0.7882353f, 0.22745098f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow50, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.91764706f, 0.40784314f, 0.41960785f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.ClayGoo, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.2f, 0.09019608f, 0.09019608f)
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
				canStack = true
			});
			BuffCatalog.RegisterBuff(BuffIndex.RepairMode, new BuffDef());
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
			BuffCatalog.RegisterBuff(BuffIndex.TempestSpeed, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffTempestSpeedIcon",
				buffColor = new Color(0.8392157f, 0.7882353f, 0.22745098f)
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
			BuffCatalog.RegisterBuff(BuffIndex.Slow30, new BuffDef());
			BuffCatalog.RegisterBuff(BuffIndex.EngiTeamShield, new BuffDef());
			BuffCatalog.RegisterBuff(BuffIndex.CommandoBoost, new BuffDef());
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
				iconPath = "Textures/BuffIcons/texBuffCrippleIcon"
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow80, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.64705884f, 0.87058824f, 0.92941177f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.Slow60, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
				buffColor = new Color(0.6784314f, 0.6117647f, 0.4117647f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixRed, new BuffDef
			{
				isElite = true,
				iconPath = "Textures/BuffIcons/texBuffAffixRed"
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixBlue, new BuffDef
			{
				isElite = true,
				iconPath = "Textures/BuffIcons/texBuffAffixBlue"
			});
			BuffCatalog.RegisterBuff(BuffIndex.NoCooldowns, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texNoCooldownsBuffIcon",
				buffColor = new Color(0.73333335f, 0.54509807f, 0.9764706f)
			});
			BuffCatalog.RegisterBuff(BuffIndex.AffixWhite, new BuffDef
			{
				isElite = true,
				iconPath = "Textures/BuffIcons/texBuffAffixWhite"
			});
			BuffCatalog.RegisterBuff(BuffIndex.Invincibility, new BuffDef
			{
				iconPath = "Textures/BuffIcons/texBuffGenericShield",
				buffColor = new Color(0.8117647f, 0.6117647f, 0.8784314f)
			});
			for (BuffIndex buffIndex = BuffIndex.Slow50; buffIndex < BuffIndex.Count; buffIndex++)
			{
				if (BuffCatalog.buffDefs[(int)buffIndex] == null)
				{
					Debug.LogWarningFormat("Unregistered buff {0}!", new object[]
					{
						Enum.GetName(typeof(BuffIndex), buffIndex)
					});
				}
			}
			BuffCatalog.eliteBuffIndices = (from buffDef in BuffCatalog.buffDefs
			where buffDef.isElite
			select buffDef.buffIndex).ToArray<BuffIndex>();
		}

		// Token: 0x04000D8F RID: 3471
		private static BuffDef[] buffDefs = new BuffDef[31];

		// Token: 0x04000D90 RID: 3472
		public static readonly BuffIndex[] eliteBuffIndices;
	}
}
