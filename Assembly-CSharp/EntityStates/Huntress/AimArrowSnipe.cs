using System;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x0200082A RID: 2090
	public class AimArrowSnipe : BaseArrowBarrage
	{
		// Token: 0x06002F53 RID: 12115 RVA: 0x000C9FFC File Offset: 0x000C81FC
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAimAnimator = base.GetModelTransform().GetComponent<AimAnimator>();
			if (this.modelAimAnimator)
			{
				this.modelAimAnimator.enabled = true;
			}
			this.primarySkillSlot = (base.skillLocator ? base.skillLocator.primary : null);
			if (this.primarySkillSlot)
			{
				this.primarySkillSlot.SetSkillOverride(this, AimArrowSnipe.primarySkillDef, GenericSkill.SkillOverridePriority.Contextual);
			}
			base.PlayCrossfade("Body", "ArrowBarrageLoop", 0.1f);
			this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
			if (AimArrowSnipe.crosshairOverridePrefab)
			{
				base.characterBody.crosshairPrefab = AimArrowSnipe.crosshairOverridePrefab;
			}
		}

		// Token: 0x06002F54 RID: 12116 RVA: 0x000CA0BB File Offset: 0x000C82BB
		protected override void HandlePrimaryAttack()
		{
			if (this.primarySkillSlot)
			{
				this.primarySkillSlot.ExecuteIfReady();
			}
		}

		// Token: 0x06002F55 RID: 12117 RVA: 0x000CA0D8 File Offset: 0x000C82D8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.characterDirection)
			{
				base.characterDirection.moveVector = base.GetAimRay().direction;
			}
			if (!this.primarySkillSlot || this.primarySkillSlot.stock == 0)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002F56 RID: 12118 RVA: 0x000CA137 File Offset: 0x000C8337
		public override void OnExit()
		{
			if (this.primarySkillSlot)
			{
				this.primarySkillSlot.UnsetSkillOverride(this, AimArrowSnipe.primarySkillDef, GenericSkill.SkillOverridePriority.Contextual);
			}
			base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			base.OnExit();
		}

		// Token: 0x04002CD5 RID: 11477
		public static SkillDef primarySkillDef;

		// Token: 0x04002CD6 RID: 11478
		public static GameObject crosshairOverridePrefab;

		// Token: 0x04002CD7 RID: 11479
		private GameObject defaultCrosshairPrefab;

		// Token: 0x04002CD8 RID: 11480
		private GenericSkill primarySkillSlot;

		// Token: 0x04002CD9 RID: 11481
		private AimAnimator modelAimAnimator;
	}
}
