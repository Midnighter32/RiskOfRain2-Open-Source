using System;

namespace RoR2.Achievements.Treebot
{
	// Token: 0x020006D3 RID: 1747
	[RegisterAchievement("TreebotLowHealthTeleporter", "Skills.Treebot.Barrage", "RescueTreebot", null)]
	public class TreebotLowHealthTeleporterAchievement : BaseAchievement
	{
		// Token: 0x06002896 RID: 10390 RVA: 0x000AC8CA File Offset: 0x000AAACA
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("TreebotBody");
		}

		// Token: 0x06002897 RID: 10391 RVA: 0x000AC9BB File Offset: 0x000AABBB
		private void OnTeleporterBeginChargingGlobal(TeleporterInteraction teleporterInteraction)
		{
			this.failed = false;
			this.healthMonitor.SetActive(true);
		}

		// Token: 0x06002898 RID: 10392 RVA: 0x000AC9D0 File Offset: 0x000AABD0
		private void OnTeleporterChargedGlobal(TeleporterInteraction teleporterInteraction)
		{
			if (!this.failed)
			{
				base.Grant();
			}
		}

		// Token: 0x06002899 RID: 10393 RVA: 0x000AC9E0 File Offset: 0x000AABE0
		private void SubscribeHealthMonitor()
		{
			RoR2Application.onFixedUpdate += this.OnFixedUpdateMonitorHealth;
		}

		// Token: 0x0600289A RID: 10394 RVA: 0x000AC9F3 File Offset: 0x000AABF3
		private void UnsubscribeHealthMonitor()
		{
			RoR2Application.onFixedUpdate -= this.OnFixedUpdateMonitorHealth;
		}

		// Token: 0x0600289B RID: 10395 RVA: 0x000ACA06 File Offset: 0x000AAC06
		private void OnFixedUpdateMonitorHealth()
		{
			if (!this.healthComponent || TreebotLowHealthTeleporterAchievement.requirement < this.healthComponent.combinedHealthFraction)
			{
				this.failed = true;
				this.healthMonitor.SetActive(false);
			}
		}

		// Token: 0x0600289C RID: 10396 RVA: 0x000ACA3A File Offset: 0x000AAC3A
		public override void OnInstall()
		{
			base.OnInstall();
			this.healthMonitor = new ToggleAction(new Action(this.SubscribeHealthMonitor), new Action(this.UnsubscribeHealthMonitor));
		}

		// Token: 0x0600289D RID: 10397 RVA: 0x000ACA68 File Offset: 0x000AAC68
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			TeleporterInteraction.onTeleporterBeginChargingGlobal += this.OnTeleporterBeginChargingGlobal;
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterChargedGlobal;
			base.localUser.onBodyChanged += this.OnBodyChanged;
			this.OnBodyChanged();
		}

		// Token: 0x0600289E RID: 10398 RVA: 0x000ACABA File Offset: 0x000AACBA
		private void OnBodyChanged()
		{
			CharacterBody cachedBody = base.localUser.cachedBody;
			this.healthComponent = ((cachedBody != null) ? cachedBody.healthComponent : null);
		}

		// Token: 0x0600289F RID: 10399 RVA: 0x000ACADC File Offset: 0x000AACDC
		protected override void OnBodyRequirementBroken()
		{
			base.localUser.onBodyChanged -= this.OnBodyChanged;
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterChargedGlobal;
			TeleporterInteraction.onTeleporterBeginChargingGlobal -= this.OnTeleporterBeginChargingGlobal;
			this.healthMonitor.SetActive(false);
			this.healthComponent = null;
			base.OnBodyRequirementBroken();
		}

		// Token: 0x04002521 RID: 9505
		private static readonly float requirement = 0.5f;

		// Token: 0x04002522 RID: 9506
		private ToggleAction healthMonitor;

		// Token: 0x04002523 RID: 9507
		private HealthComponent targetHealthComponent;

		// Token: 0x04002524 RID: 9508
		private bool failed = true;

		// Token: 0x04002525 RID: 9509
		private HealthComponent healthComponent;
	}
}
