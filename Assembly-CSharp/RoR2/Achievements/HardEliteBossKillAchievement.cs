using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006AB RID: 1707
	[RegisterAchievement("HardEliteBossKill", "Items.KillEliteFrenzy", null, typeof(HardEliteBossKillAchievement.EliteBossKillServerAchievement))]
	internal class HardEliteBossKillAchievement : BaseAchievement
	{
		// Token: 0x060027DD RID: 10205 RVA: 0x000AB45A File Offset: 0x000A965A
		public override void OnInstall()
		{
			base.OnInstall();
			NetworkUser.OnPostNetworkUserStart += this.OnPostNetworkUserStart;
			Run.onRunStartGlobal += this.OnRunStart;
		}

		// Token: 0x060027DE RID: 10206 RVA: 0x000AB484 File Offset: 0x000A9684
		public override void OnUninstall()
		{
			NetworkUser.OnPostNetworkUserStart -= this.OnPostNetworkUserStart;
			Run.onRunStartGlobal -= this.OnRunStart;
			base.OnUninstall();
		}

		// Token: 0x060027DF RID: 10207 RVA: 0x000AB4AE File Offset: 0x000A96AE
		private void UpdateTracking()
		{
			base.SetServerTracked(Run.instance && Run.instance.selectedDifficulty >= DifficultyIndex.Hard);
		}

		// Token: 0x060027E0 RID: 10208 RVA: 0x000AB4D5 File Offset: 0x000A96D5
		private void OnPostNetworkUserStart(NetworkUser networkUser)
		{
			this.UpdateTracking();
		}

		// Token: 0x060027E1 RID: 10209 RVA: 0x000AB4D5 File Offset: 0x000A96D5
		private void OnRunStart(Run run)
		{
			this.UpdateTracking();
		}

		// Token: 0x020006AC RID: 1708
		private class EliteBossKillServerAchievement : BaseServerAchievement
		{
			// Token: 0x060027E3 RID: 10211 RVA: 0x000AB4DD File Offset: 0x000A96DD
			public override void OnInstall()
			{
				base.OnInstall();
				HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Add(this);
				if (HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Count == 1)
				{
					GlobalEventManager.onCharacterDeathGlobal += HardEliteBossKillAchievement.EliteBossKillServerAchievement.OnCharacterDeath;
				}
			}

			// Token: 0x060027E4 RID: 10212 RVA: 0x000AB50E File Offset: 0x000A970E
			public override void OnUninstall()
			{
				if (HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Count == 1)
				{
					GlobalEventManager.onCharacterDeathGlobal -= HardEliteBossKillAchievement.EliteBossKillServerAchievement.OnCharacterDeath;
				}
				HardEliteBossKillAchievement.EliteBossKillServerAchievement.instancesList.Remove(this);
				base.OnUninstall();
			}

			// Token: 0x060027E5 RID: 10213 RVA: 0x000AB540 File Offset: 0x000A9740
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

			// Token: 0x040024FF RID: 9471
			private static readonly List<HardEliteBossKillAchievement.EliteBossKillServerAchievement> instancesList = new List<HardEliteBossKillAchievement.EliteBossKillServerAchievement>();
		}
	}
}
