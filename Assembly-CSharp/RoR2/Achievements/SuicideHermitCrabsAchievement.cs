using System;
using RoR2.Stats;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006CA RID: 1738
	[RegisterAchievement("SuicideHermitCrabs", "Items.AutoCastEquipment", null, typeof(SuicideHermitCrabsAchievement.SuicideHermitCrabsServerAchievement))]
	public class SuicideHermitCrabsAchievement : BaseAchievement
	{
		// Token: 0x06002860 RID: 10336 RVA: 0x000AC189 File Offset: 0x000AA389
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			base.SetServerTracked(true);
		}

		// Token: 0x06002861 RID: 10337 RVA: 0x000AC1AF File Offset: 0x000AA3AF
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002862 RID: 10338 RVA: 0x000AC1CE File Offset: 0x000AA3CE
		private void Check()
		{
			if (base.userProfile.statSheet.GetStatValueULong(StatDef.suicideHermitCrabsAchievementProgress) >= 20UL)
			{
				base.Grant();
			}
		}

		// Token: 0x06002863 RID: 10339 RVA: 0x000AC1F0 File Offset: 0x000AA3F0
		public override float ProgressForAchievement()
		{
			return base.userProfile.statSheet.GetStatValueULong(StatDef.suicideHermitCrabsAchievementProgress) / 20f;
		}

		// Token: 0x04002513 RID: 9491
		private const int requirement = 20;

		// Token: 0x020006CB RID: 1739
		private class SuicideHermitCrabsServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002865 RID: 10341 RVA: 0x000AC210 File Offset: 0x000AA410
			public override void OnInstall()
			{
				base.OnInstall();
				this.crabBodyIndex = BodyCatalog.FindBodyIndex("HermitCrabBody");
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x06002866 RID: 10342 RVA: 0x000AC239 File Offset: 0x000AA439
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x06002867 RID: 10343 RVA: 0x000AC254 File Offset: 0x000AA454
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victimBody)
				{
					return;
				}
				GameObject inflictor = damageReport.damageInfo.inflictor;
				if (!inflictor || !inflictor.GetComponent<MapZone>())
				{
					return;
				}
				if (damageReport.victimBody.bodyIndex == this.crabBodyIndex && damageReport.victimBody.teamComponent.teamIndex != TeamIndex.Player)
				{
					PlayerStatsComponent masterPlayerStatsComponent = base.networkUser.masterPlayerStatsComponent;
					if (masterPlayerStatsComponent)
					{
						masterPlayerStatsComponent.currentStats.PushStatValue(StatDef.suicideHermitCrabsAchievementProgress, 1UL);
					}
				}
			}

			// Token: 0x04002514 RID: 9492
			private int crabBodyIndex;
		}
	}
}
