using System;
using RoR2;
using UnityEngine;

namespace EntityStates.HermitCrab
{
	// Token: 0x0200015A RID: 346
	internal class BurrowOut : BaseState
	{
		// Token: 0x060006B7 RID: 1719 RVA: 0x00020100 File Offset: 0x0001E300
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = BurrowOut.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "BurrowOut", "BurrowOut.playbackRate", this.duration, 0.1f);
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			Util.PlaySound(BurrowOut.burrowOutSoundString, base.gameObject);
			EffectManager.instance.SimpleMuzzleFlash(BurrowOut.burrowPrefab, base.gameObject, "BurrowCenter", false);
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0002018E File Offset: 0x0001E38E
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400083D RID: 2109
		public static GameObject burrowPrefab;

		// Token: 0x0400083E RID: 2110
		public static float baseDuration;

		// Token: 0x0400083F RID: 2111
		public static string burrowOutSoundString;

		// Token: 0x04000840 RID: 2112
		private float stopwatch;

		// Token: 0x04000841 RID: 2113
		private Transform modelTransform;

		// Token: 0x04000842 RID: 2114
		private ChildLocator childLocator;

		// Token: 0x04000843 RID: 2115
		private float duration;
	}
}
