using System;
using RoR2;
using UnityEngine;

namespace EntityStates.NullifierMonster
{
	// Token: 0x020007B1 RID: 1969
	public class SpawnState : BaseState
	{
		// Token: 0x06002D07 RID: 11527 RVA: 0x000BE1E4 File Offset: 0x000BC3E4
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			if (SpawnState.spawnEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, "PortalEffect", false);
			}
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				modelTransform.GetComponent<PrintController>().enabled = true;
			}
		}

		// Token: 0x06002D08 RID: 11528 RVA: 0x000BE25F File Offset: 0x000BC45F
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002D09 RID: 11529 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002940 RID: 10560
		private float stopwatch;

		// Token: 0x04002941 RID: 10561
		public static float duration = 4f;

		// Token: 0x04002942 RID: 10562
		public static string spawnSoundString;

		// Token: 0x04002943 RID: 10563
		public static GameObject spawnEffectPrefab;
	}
}
