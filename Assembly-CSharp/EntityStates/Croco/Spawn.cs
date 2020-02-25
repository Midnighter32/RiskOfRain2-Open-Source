using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Croco
{
	// Token: 0x020008AD RID: 2221
	public class Spawn : BaseState
	{
		// Token: 0x060031CC RID: 12748 RVA: 0x000D6898 File Offset: 0x000D4A98
		public override void OnEnter()
		{
			base.OnEnter();
			base.modelLocator.normalizeToFloor = true;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				this.modelAnimator.SetFloat(AnimationParameters.aimWeight, 0f);
			}
			base.PlayAnimation("Body", "SleepLoop");
			EffectManager.SpawnEffect(Spawn.spawnEffectPrefab, new EffectData
			{
				origin = base.characterBody.footPosition
			}, false);
		}

		// Token: 0x060031CD RID: 12749 RVA: 0x000D6916 File Offset: 0x000D4B16
		public override void OnExit()
		{
			if (this.modelAnimator)
			{
				this.modelAnimator.SetFloat(AnimationParameters.aimWeight, 1f);
			}
			base.OnExit();
		}

		// Token: 0x060031CE RID: 12750 RVA: 0x000D6940 File Offset: 0x000D4B40
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= Spawn.minimumSleepDuration && (base.inputBank.moveVector.sqrMagnitude >= Mathf.Epsilon || base.inputBank.CheckAnyButtonDown()))
			{
				this.outer.SetNextState(new WakeUp());
			}
		}

		// Token: 0x0400305B RID: 12379
		public static float minimumSleepDuration;

		// Token: 0x0400305C RID: 12380
		public static GameObject spawnEffectPrefab;

		// Token: 0x0400305D RID: 12381
		private Animator modelAnimator;
	}
}
