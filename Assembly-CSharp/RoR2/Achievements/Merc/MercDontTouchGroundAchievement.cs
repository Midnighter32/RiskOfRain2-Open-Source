using System;
using UnityEngine;

namespace RoR2.Achievements.Merc
{
	// Token: 0x020006DC RID: 1756
	[RegisterAchievement("MercDontTouchGround", "Skills.Merc.Uppercut", "CompleteUnknownEnding", null)]
	public class MercDontTouchGroundAchievement : BaseAchievement
	{
		// Token: 0x060028CE RID: 10446 RVA: 0x000ACDE8 File Offset: 0x000AAFE8
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("MercBody");
		}

		// Token: 0x060028CF RID: 10447 RVA: 0x000AD015 File Offset: 0x000AB215
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			RoR2Application.onFixedUpdate += this.MercFixedUpdate;
			base.localUser.onBodyChanged += this.OnBodyChanged;
			this.OnBodyChanged();
		}

		// Token: 0x060028D0 RID: 10448 RVA: 0x000AD04B File Offset: 0x000AB24B
		protected override void OnBodyRequirementBroken()
		{
			base.localUser.onBodyChanged -= this.OnBodyChanged;
			RoR2Application.onFixedUpdate -= this.MercFixedUpdate;
			base.OnBodyRequirementBroken();
		}

		// Token: 0x060028D1 RID: 10449 RVA: 0x000AD07B File Offset: 0x000AB27B
		private void OnBodyChanged()
		{
			this.body = base.localUser.cachedBody;
			this.motor = (this.body ? this.body.characterMotor : null);
		}

		// Token: 0x060028D2 RID: 10450 RVA: 0x000AD0B0 File Offset: 0x000AB2B0
		private void MercFixedUpdate()
		{
			this.stopwatch = ((this.motor && !this.motor.isGrounded && !this.body.currentVehicle) ? (this.stopwatch + Time.fixedDeltaTime) : 0f);
			if (MercDontTouchGroundAchievement.requirement <= this.stopwatch)
			{
				base.Grant();
			}
		}

		// Token: 0x04002530 RID: 9520
		private static readonly float requirement = 30f;

		// Token: 0x04002531 RID: 9521
		private CharacterMotor motor;

		// Token: 0x04002532 RID: 9522
		private CharacterBody body;

		// Token: 0x04002533 RID: 9523
		private float stopwatch;
	}
}
