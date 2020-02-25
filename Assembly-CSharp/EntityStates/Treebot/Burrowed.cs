using System;
using EntityStates.Treebot.Weapon;
using RoR2;
using UnityEngine;

namespace EntityStates.Treebot
{
	// Token: 0x02000746 RID: 1862
	public class Burrowed : GenericCharacterMain
	{
		// Token: 0x06002B2C RID: 11052 RVA: 0x000B6000 File Offset: 0x000B4200
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Body", "Burrowed", 0.1f);
			base.skillLocator.primary = base.skillLocator.FindSkill(Burrowed.altPrimarySkillName);
			base.skillLocator.utility = base.skillLocator.FindSkill(Burrowed.altUtilitySkillName);
			base.skillLocator.primary.stateMachine.mainStateType = new SerializableEntityStateType(typeof(AimMortar));
			base.skillLocator.primary.stateMachine.SetNextStateToMain();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
			}
			if (this.childLocator)
			{
				base.characterBody.aimOriginTransform = this.childLocator.FindChild("AimOriginMortar");
			}
		}

		// Token: 0x06002B2D RID: 11053 RVA: 0x000B60DA File Offset: 0x000B42DA
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
		}

		// Token: 0x06002B2E RID: 11054 RVA: 0x000B60FB File Offset: 0x000B42FB
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002B2F RID: 11055 RVA: 0x0000409B File Offset: 0x0000229B
		public override void HandleMovements()
		{
		}

		// Token: 0x06002B30 RID: 11056 RVA: 0x000B6104 File Offset: 0x000B4304
		public override void OnExit()
		{
			base.skillLocator.primary = base.skillLocator.FindSkill(Burrowed.primarySkillName);
			base.skillLocator.utility = base.skillLocator.FindSkill(Burrowed.utilitySkillName);
			base.skillLocator.primary.stateMachine.mainStateType = new SerializableEntityStateType(typeof(Idle));
			base.skillLocator.primary.stateMachine.SetNextStateToMain();
			if (this.childLocator)
			{
				base.characterBody.aimOriginTransform = this.childLocator.FindChild("AimOriginSyringe");
			}
			base.OnExit();
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002715 RID: 10005
		public static float mortarCooldown;

		// Token: 0x04002716 RID: 10006
		public static string primarySkillName;

		// Token: 0x04002717 RID: 10007
		public static string altPrimarySkillName;

		// Token: 0x04002718 RID: 10008
		public static string utilitySkillName;

		// Token: 0x04002719 RID: 10009
		public static string altUtilitySkillName;

		// Token: 0x0400271A RID: 10010
		public float duration;

		// Token: 0x0400271B RID: 10011
		private ChildLocator childLocator;
	}
}
