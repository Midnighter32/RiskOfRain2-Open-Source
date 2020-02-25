using System;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace EntityStates.Vulture
{
	// Token: 0x0200073A RID: 1850
	public class Fly : VultureModeState
	{
		// Token: 0x06002AF5 RID: 10997 RVA: 0x000B4DF0 File Offset: 0x000B2FF0
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.characterMotor)
			{
				base.characterMotor.isFlying = true;
				base.characterMotor.useGravity = false;
				base.characterMotor.velocity.y = Fly.launchSpeed;
				base.characterMotor.Motor.ForceUnground();
			}
			base.PlayAnimation("Body", "Jump");
			if (this.activatorSkillSlot)
			{
				this.activatorSkillSlot.SetSkillOverride(this, Fly.landingSkill, GenericSkill.SkillOverridePriority.Contextual);
			}
			if (Fly.jumpEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(Fly.jumpEffectPrefab, base.gameObject, Fly.jumpEffectMuzzleString, false);
			}
		}

		// Token: 0x06002AF6 RID: 10998 RVA: 0x000B4EA0 File Offset: 0x000B30A0
		public override void OnExit()
		{
			if (this.activatorSkillSlot)
			{
				this.activatorSkillSlot.UnsetSkillOverride(this, Fly.landingSkill, GenericSkill.SkillOverridePriority.Contextual);
			}
			if (base.characterMotor)
			{
				base.characterMotor.isFlying = false;
				base.characterMotor.useGravity = true;
			}
			if (base.modelLocator)
			{
				base.modelLocator.normalizeToFloor = true;
			}
			base.OnExit();
		}

		// Token: 0x040026D2 RID: 9938
		public static SkillDef landingSkill;

		// Token: 0x040026D3 RID: 9939
		public static float launchSpeed;

		// Token: 0x040026D4 RID: 9940
		public static GameObject jumpEffectPrefab;

		// Token: 0x040026D5 RID: 9941
		public static string jumpEffectMuzzleString;
	}
}
