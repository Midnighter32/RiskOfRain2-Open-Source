using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000827 RID: 2087
	public class SpawnState : BaseState
	{
		// Token: 0x06002F48 RID: 12104 RVA: 0x000C9EF4 File Offset: 0x000C80F4
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			if (SpawnState.spawnEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, "Base", false);
			}
		}

		// Token: 0x06002F49 RID: 12105 RVA: 0x000C9F54 File Offset: 0x000C8154
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002F4A RID: 12106 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002CD1 RID: 11473
		private float stopwatch;

		// Token: 0x04002CD2 RID: 11474
		public static float duration = 4f;

		// Token: 0x04002CD3 RID: 11475
		public static string spawnSoundString;

		// Token: 0x04002CD4 RID: 11476
		public static GameObject spawnEffectPrefab;
	}
}
