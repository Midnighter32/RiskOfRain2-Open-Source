using System;

namespace RoR2.Achievements.Huntress
{
	// Token: 0x020006EA RID: 1770
	[RegisterAchievement("HuntressMaintainFullHealthOnFrozenWall", "Skills.Huntress.Snipe", "CompleteThreeStages", null)]
	public class HuntressMaintainFullHealthOnFrozenWallAchievement : BaseAchievement
	{
		// Token: 0x0600291C RID: 10524 RVA: 0x000AD590 File Offset: 0x000AB790
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("HuntressBody");
		}

		// Token: 0x0600291D RID: 10525 RVA: 0x000AD6C6 File Offset: 0x000AB8C6
		private void SubscribeHealthCheck()
		{
			RoR2Application.onFixedUpdate += this.CheckHealth;
		}

		// Token: 0x0600291E RID: 10526 RVA: 0x000AD6D9 File Offset: 0x000AB8D9
		private void UnsubscribeHealthCheck()
		{
			RoR2Application.onFixedUpdate -= this.CheckHealth;
		}

		// Token: 0x0600291F RID: 10527 RVA: 0x000AD6EC File Offset: 0x000AB8EC
		private void SubscribeTeleporterCheck()
		{
			TeleporterInteraction.onTeleporterChargedGlobal += this.CheckTeleporter;
		}

		// Token: 0x06002920 RID: 10528 RVA: 0x000AD6FF File Offset: 0x000AB8FF
		private void UnsubscribeTeleporterCheck()
		{
			TeleporterInteraction.onTeleporterChargedGlobal -= this.CheckTeleporter;
		}

		// Token: 0x06002921 RID: 10529 RVA: 0x000AD712 File Offset: 0x000AB912
		private void CheckTeleporter(TeleporterInteraction teleporterInteraction)
		{
			if (this.sceneOk && this.characterOk && !this.failed)
			{
				base.Grant();
			}
		}

		// Token: 0x06002922 RID: 10530 RVA: 0x000AD734 File Offset: 0x000AB934
		public override void OnInstall()
		{
			base.OnInstall();
			this.healthCheck = new ToggleAction(new Action(this.SubscribeHealthCheck), new Action(this.UnsubscribeHealthCheck));
			this.teleporterCheck = new ToggleAction(new Action(this.SubscribeTeleporterCheck), new Action(this.UnsubscribeTeleporterCheck));
			SceneCatalog.onMostRecentSceneDefChanged += this.OnMostRecentSceneDefChanged;
			base.localUser.onBodyChanged += this.OnBodyChanged;
		}

		// Token: 0x06002923 RID: 10531 RVA: 0x000AD7B8 File Offset: 0x000AB9B8
		public override void OnUninstall()
		{
			base.localUser.onBodyChanged -= this.OnBodyChanged;
			SceneCatalog.onMostRecentSceneDefChanged -= this.OnMostRecentSceneDefChanged;
			this.healthCheck.Dispose();
			this.teleporterCheck.Dispose();
			base.OnUninstall();
		}

		// Token: 0x06002924 RID: 10532 RVA: 0x000AD80C File Offset: 0x000ABA0C
		private void OnBodyChanged()
		{
			if (this.sceneOk && this.characterOk && !this.failed && base.localUser.cachedBody)
			{
				this.healthComponent = base.localUser.cachedBody.healthComponent;
				this.healthCheck.SetActive(true);
				this.teleporterCheck.SetActive(true);
			}
		}

		// Token: 0x06002925 RID: 10533 RVA: 0x000AD871 File Offset: 0x000ABA71
		private void OnMostRecentSceneDefChanged(SceneDef sceneDef)
		{
			this.sceneOk = (sceneDef.baseSceneName == HuntressMaintainFullHealthOnFrozenWallAchievement.requiredScene);
			if (this.sceneOk)
			{
				this.failed = false;
			}
		}

		// Token: 0x06002926 RID: 10534 RVA: 0x000AD898 File Offset: 0x000ABA98
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			this.characterOk = true;
		}

		// Token: 0x06002927 RID: 10535 RVA: 0x000AD8A7 File Offset: 0x000ABAA7
		protected override void OnBodyRequirementBroken()
		{
			this.characterOk = false;
			this.Fail();
			base.OnBodyRequirementBroken();
		}

		// Token: 0x06002928 RID: 10536 RVA: 0x000AD8BC File Offset: 0x000ABABC
		private void Fail()
		{
			this.failed = true;
			this.healthCheck.SetActive(false);
			this.teleporterCheck.SetActive(false);
		}

		// Token: 0x06002929 RID: 10537 RVA: 0x000AD8DD File Offset: 0x000ABADD
		private void CheckHealth()
		{
			if (this.healthComponent && this.healthComponent.combinedHealth < this.healthComponent.fullCombinedHealth)
			{
				this.Fail();
			}
		}

		// Token: 0x04002542 RID: 9538
		private static readonly string requiredScene = "frozenwall";

		// Token: 0x04002543 RID: 9539
		private HealthComponent healthComponent;

		// Token: 0x04002544 RID: 9540
		private bool failed;

		// Token: 0x04002545 RID: 9541
		private bool sceneOk;

		// Token: 0x04002546 RID: 9542
		private bool characterOk;

		// Token: 0x04002547 RID: 9543
		private ToggleAction healthCheck;

		// Token: 0x04002548 RID: 9544
		private ToggleAction teleporterCheck;
	}
}
