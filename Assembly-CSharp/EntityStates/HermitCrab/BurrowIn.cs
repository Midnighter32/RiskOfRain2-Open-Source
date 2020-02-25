using System;
using RoR2;
using UnityEngine;

namespace EntityStates.HermitCrab
{
	// Token: 0x0200083B RID: 2107
	public class BurrowIn : BaseState
	{
		// Token: 0x06002FB3 RID: 12211 RVA: 0x000CC6B4 File Offset: 0x000CA8B4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = BurrowIn.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "BurrowIn", "BurrowIn.playbackRate", this.duration, 0.1f);
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			Util.PlaySound(BurrowIn.burrowInSoundString, base.gameObject);
			EffectManager.SimpleMuzzleFlash(BurrowIn.burrowPrefab, base.gameObject, "BurrowCenter", false);
		}

		// Token: 0x06002FB4 RID: 12212 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002FB5 RID: 12213 RVA: 0x000CC740 File Offset: 0x000CA940
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

		// Token: 0x06002FB6 RID: 12214 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002D7C RID: 11644
		public static GameObject burrowPrefab;

		// Token: 0x04002D7D RID: 11645
		public static float baseDuration;

		// Token: 0x04002D7E RID: 11646
		public static string burrowInSoundString;

		// Token: 0x04002D7F RID: 11647
		private float stopwatch;

		// Token: 0x04002D80 RID: 11648
		private float duration;

		// Token: 0x04002D81 RID: 11649
		private Transform modelTransform;

		// Token: 0x04002D82 RID: 11650
		private ChildLocator childLocator;
	}
}
