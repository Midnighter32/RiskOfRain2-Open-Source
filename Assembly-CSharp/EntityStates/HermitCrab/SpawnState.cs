using System;
using RoR2;
using UnityEngine;

namespace EntityStates.HermitCrab
{
	// Token: 0x0200083E RID: 2110
	public class SpawnState : BaseState
	{
		// Token: 0x06002FC3 RID: 12227 RVA: 0x000CCB5C File Offset: 0x000CAD5C
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			EffectManager.SimpleMuzzleFlash(SpawnState.burrowPrefab, base.gameObject, "BurrowCenter", false);
		}

		// Token: 0x06002FC4 RID: 12228 RVA: 0x000CCBB0 File Offset: 0x000CADB0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002FC5 RID: 12229 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002D96 RID: 11670
		private float stopwatch;

		// Token: 0x04002D97 RID: 11671
		public static GameObject burrowPrefab;

		// Token: 0x04002D98 RID: 11672
		public static float duration = 4f;

		// Token: 0x04002D99 RID: 11673
		public static string spawnSoundString;
	}
}
