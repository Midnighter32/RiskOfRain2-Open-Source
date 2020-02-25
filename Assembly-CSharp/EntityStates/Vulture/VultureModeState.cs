using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Vulture
{
	// Token: 0x02000738 RID: 1848
	public class VultureModeState : BaseSkillState
	{
		// Token: 0x06002AEF RID: 10991 RVA: 0x000B4CC0 File Offset: 0x000B2EC0
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			if (this.animator)
			{
				this.flyOverrideLayer = this.animator.GetLayerIndex("FlyOverride");
			}
			if (base.characterMotor)
			{
				base.characterMotor.walkSpeedPenaltyCoefficient = this.movementSpeedMultiplier;
			}
			if (base.modelLocator)
			{
				base.modelLocator.normalizeToFloor = false;
			}
			Util.PlaySound(this.enterSoundString, base.gameObject);
		}

		// Token: 0x06002AF0 RID: 10992 RVA: 0x000B4D4C File Offset: 0x000B2F4C
		public override void Update()
		{
			base.Update();
			if (this.animator)
			{
				this.animator.SetLayerWeight(this.flyOverrideLayer, Util.Remap(Mathf.Clamp01(base.age / this.mecanimTransitionDuration), 0f, 1f, 1f - this.flyOverrideMecanimLayerWeight, this.flyOverrideMecanimLayerWeight));
			}
		}

		// Token: 0x06002AF1 RID: 10993 RVA: 0x000B4DB0 File Offset: 0x000B2FB0
		public override void OnExit()
		{
			if (base.characterMotor)
			{
				base.characterMotor.walkSpeedPenaltyCoefficient = 1f;
			}
			base.OnExit();
		}

		// Token: 0x040026CC RID: 9932
		[SerializeField]
		public float mecanimTransitionDuration;

		// Token: 0x040026CD RID: 9933
		[SerializeField]
		public float flyOverrideMecanimLayerWeight;

		// Token: 0x040026CE RID: 9934
		[SerializeField]
		public float movementSpeedMultiplier;

		// Token: 0x040026CF RID: 9935
		[SerializeField]
		public string enterSoundString;

		// Token: 0x040026D0 RID: 9936
		protected Animator animator;

		// Token: 0x040026D1 RID: 9937
		protected int flyOverrideLayer;
	}
}
