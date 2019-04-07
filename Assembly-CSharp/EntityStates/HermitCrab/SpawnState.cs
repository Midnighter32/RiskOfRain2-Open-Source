using System;
using RoR2;
using UnityEngine;

namespace EntityStates.HermitCrab
{
	// Token: 0x0200015C RID: 348
	public class SpawnState : BaseState
	{
		// Token: 0x060006C2 RID: 1730 RVA: 0x000204D4 File Offset: 0x0001E6D4
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			EffectManager.instance.SimpleMuzzleFlash(SpawnState.burrowPrefab, base.gameObject, "BurrowCenter", false);
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x0002052D File Offset: 0x0001E72D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000851 RID: 2129
		private float stopwatch;

		// Token: 0x04000852 RID: 2130
		public static GameObject burrowPrefab;

		// Token: 0x04000853 RID: 2131
		public static float duration = 4f;

		// Token: 0x04000854 RID: 2132
		public static string spawnSoundString;
	}
}
