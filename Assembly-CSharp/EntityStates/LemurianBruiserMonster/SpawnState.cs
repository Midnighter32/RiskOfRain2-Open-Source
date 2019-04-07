using System;
using RoR2;
using UnityEngine;

namespace EntityStates.LemurianBruiserMonster
{
	// Token: 0x02000121 RID: 289
	public class SpawnState : EntityState
	{
		// Token: 0x06000595 RID: 1429 RVA: 0x000199F0 File Offset: 0x00017BF0
		public override void OnEnter()
		{
			base.OnEnter();
			base.GetModelAnimator();
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			if (SpawnState.spawnEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, "SpawnEffectOrigin", false);
			}
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00019A5C File Offset: 0x00017C5C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400065F RID: 1631
		public static float duration = 2f;

		// Token: 0x04000660 RID: 1632
		public static string spawnSoundString;

		// Token: 0x04000661 RID: 1633
		public static GameObject spawnEffectPrefab;
	}
}
