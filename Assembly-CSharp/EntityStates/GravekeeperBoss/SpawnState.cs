using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GravekeeperBoss
{
	// Token: 0x0200084E RID: 2126
	public class SpawnState : BaseState
	{
		// Token: 0x06003016 RID: 12310 RVA: 0x000CE4C4 File Offset: 0x000CC6C4
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			EffectManager.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, "Root", false);
		}

		// Token: 0x06003017 RID: 12311 RVA: 0x000CE518 File Offset: 0x000CC718
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06003018 RID: 12312 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002E04 RID: 11780
		public static float duration = 4f;

		// Token: 0x04002E05 RID: 11781
		public static string spawnSoundString;

		// Token: 0x04002E06 RID: 11782
		public static GameObject spawnEffectPrefab;
	}
}
