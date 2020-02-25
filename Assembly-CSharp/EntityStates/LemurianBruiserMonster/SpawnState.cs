using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianBruiserMonster
{
	// Token: 0x020007F0 RID: 2032
	public class SpawnState : EntityState
	{
		// Token: 0x06002E3B RID: 11835 RVA: 0x000C4CF0 File Offset: 0x000C2EF0
		public override void OnEnter()
		{
			base.OnEnter();
			base.GetModelAnimator();
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			if (SpawnState.spawnEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, "SpawnEffectOrigin", false);
			}
		}

		// Token: 0x06002E3C RID: 11836 RVA: 0x000C4D57 File Offset: 0x000C2F57
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002E3D RID: 11837 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002B60 RID: 11104
		public static float duration = 2f;

		// Token: 0x04002B61 RID: 11105
		public static string spawnSoundString;

		// Token: 0x04002B62 RID: 11106
		public static GameObject spawnEffectPrefab;
	}
}
