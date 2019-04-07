using System;
using RoR2;
using UnityEngine;

namespace EntityStates.HermitCrab
{
	// Token: 0x02000159 RID: 345
	internal class BurrowIn : BaseState
	{
		// Token: 0x060006B2 RID: 1714 RVA: 0x00020020 File Offset: 0x0001E220
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = BurrowIn.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "BurrowIn", "BurrowIn.playbackRate", this.duration, 0.1f);
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			Util.PlaySound(BurrowIn.burrowInSoundString, base.gameObject);
			EffectManager.instance.SimpleMuzzleFlash(BurrowIn.burrowPrefab, base.gameObject, "BurrowCenter", false);
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x000200B0 File Offset: 0x0001E2B0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				Burrowed nextState = new Burrowed();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000836 RID: 2102
		public static GameObject burrowPrefab;

		// Token: 0x04000837 RID: 2103
		public static float baseDuration;

		// Token: 0x04000838 RID: 2104
		public static string burrowInSoundString;

		// Token: 0x04000839 RID: 2105
		private float stopwatch;

		// Token: 0x0400083A RID: 2106
		private float duration;

		// Token: 0x0400083B RID: 2107
		private Transform modelTransform;

		// Token: 0x0400083C RID: 2108
		private ChildLocator childLocator;
	}
}
