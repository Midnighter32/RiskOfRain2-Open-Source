using System;
using RoR2;
using UnityEngine;

namespace EntityStates.NewtMonster
{
	// Token: 0x020007B4 RID: 1972
	public class SpawnState : EntityState
	{
		// Token: 0x06002D15 RID: 11541 RVA: 0x000BE644 File Offset: 0x000BC844
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

		// Token: 0x06002D16 RID: 11542 RVA: 0x000BE6AB File Offset: 0x000BC8AB
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002D17 RID: 11543 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002958 RID: 10584
		public static float duration = 2f;

		// Token: 0x04002959 RID: 10585
		public static string spawnSoundString;

		// Token: 0x0400295A RID: 10586
		public static GameObject spawnEffectPrefab;
	}
}
