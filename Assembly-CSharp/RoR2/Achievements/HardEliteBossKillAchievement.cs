using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006A3 RID: 1699
	[RegisterAchievement("HardEliteBossKill", "Items.KillEliteFrenzy", null, typeof(HardEliteBossKillAchievement.EliteBossKillServerAchievement))]
	internal class HardEliteBossKillAchievement : BaseAchievement
	{
		// Token: 0x060025D2 RID: 9682 RVA: 0x000AFC0E File Offset: 0x000ADE0E
		public override void OnInstall()
		{
			base.OnInstall();
			NetworkUser.OnPostNetworkUserStart += this.OnPostNetworkUserStart;
			Run.onRunStartGlobal += this.OnRunStart;
		}

		// Token: 0x060025D3 RID: 9683 RVA: 0x000AFC38 File Offset: 0x000ADE38
		public override void OnUninstall()
		{
			NetworkUser.OnPostNetworkUserStart -= this.OnPostNetworkUserStart;
			Run.onRunStartGlobal -= this.OnRunStart;
			base.OnUninstall();
		}

		// Token: 0x060025D4 RID: 9684 RVA: 0x000AFC62 File Offset: 0x000ADE62
		private void UpdateTracking()
		{
			base.SetServerTracked(Run.instance && Run.instance.selectedDifficulty >= DifficultyIndex.Hard);
		}

		// Token: 0x060025D5 RID: 9685 RVA: 0x000AFC89 File Offset: 0x000ADE89
		private void OnPostNetworkUserStart(NetworkUser networkUser)
		{
			this.UpdateTracking();
		}

		// Token: 0x060025D6 RID: 9686 RVA: 0x000AFC89 File Offset: 0x000ADE89
		private void OnRunStart(Run run)
		{
			this.UpdateTracking();
		}

		// Token: 0x020006A4 RID: 1700
		private class EliteBossKillServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025D8 RID: 9688 RVA: 0x000AFC91 File Offset: 0x000ADE91
			public override void OnInstall()
			{
				base.OnInstall();
				HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Add(this);
				if (HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Count == 1)
				{
					GlobalEventManager.onCharacterDeathGlobal += HardEliteBossKillAchievement.EliteBossKillServerAchievement.OnCharacterDeath;
				}
			}

			// Token: 0x060025D9 RID: 9689 RVA: 0x000AFCC2 File Offset: 0x000ADEC2
			public override void OnUninstall()
			{
				if (HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Count == 1)
				{
					GlobalEventManager.onCharacterDeathGlobal -= HardEliteBossKillAchievement.EliteBossKillServerAchievement.OnCharacterDeath;
				}
				HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Remove(this);
				base.OnUninstall();
			}

			// Token: 0x060025DA RID: 9690 RVA: 0x000AFCF4 File Offset: 0x000ADEF4
			private static void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victim)
				{
					return;
				}
				CharacterBody component = damageReport.victim.GetComponent<CharacterBody>();
				if (!component || !component.isChampion || !component.isElite)
				{
					return;
				}
				foreach (HardEliteBossKillAchievement.EliteBossKillServerAchievement eliteBossKillServerAchievement in HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList)
				{
					GameObject masterObject = eliteBossKillServerAchievement.serverAchievementTracker.networkUser.masterObject;
					if (masterObject)
					{
						CharacterMaster component2 = masterObject.GetComponent<CharacterMaster>();
						if (component2)
						{
							CharacterBody body = component2.GetBody();
							if (body && body.healthComponent && body.healthComponent.alive)
							{
								eliteBossKillServerAchievement.Grant();
							}
						}
					}
				}
			}

			// Token: 0x04002871 RID: 10353
			private static readonly List<HardEliteBossKillAchievement.EliteBossKillServerAchievement> instancesList = new List<HardEliteBossKillAchievement.EliteBossKillServerAchievement>();
		}
	}
}
