using System;
using RoR2;
using UnityEngine;

namespace EntityStates.HermitCrab
{
	// Token: 0x0200083C RID: 2108
	public class BurrowOut : BaseState
	{
		// Token: 0x06002FB8 RID: 12216 RVA: 0x000CC790 File Offset: 0x000CA990
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = BurrowOut.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Body", "BurrowOut", "BurrowOut.playbackRate", this.duration, 0.1f);
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			Util.PlaySound(BurrowOut.burrowOutSoundString, base.gameObject);
			EffectManager.SimpleMuzzleFlash(BurrowOut.burrowPrefab, base.gameObject, "BurrowCenter", false);
		}

		// Token: 0x06002FB9 RID: 12217 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002FBA RID: 12218 RVA: 0x000CC819 File Offset: 0x000CAA19
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

		// Token: 0x06002FBB RID: 12219 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002D83 RID: 11651
		public static GameObject burrowPrefab;

		// Token: 0x04002D84 RID: 11652
		public static float baseDuration;

		// Token: 0x04002D85 RID: 11653
		public static string burrowOutSoundString;

		// Token: 0x04002D86 RID: 11654
		private float stopwatch;

		// Token: 0x04002D87 RID: 11655
		private Transform modelTransform;

		// Token: 0x04002D88 RID: 11656
		private ChildLocator childLocator;

		// Token: 0x04002D89 RID: 11657
		private float duration;
	}
}
