using System;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x0200012F RID: 303
	public class SpawnState : BaseState
	{
		// Token: 0x060005D8 RID: 1496 RVA: 0x0001ABB0 File Offset: 0x00018DB0
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			if (SpawnState.spawnEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, "SpawnOrigin", false);
			}
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				modelTransform.GetComponent<PrintController>().enabled = true;
			}
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x0001AC30 File Offset: 0x00018E30
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040006B4 RID: 1716
		private float stopwatch;

		// Token: 0x040006B5 RID: 1717
		public static float duration = 4f;

		// Token: 0x040006B6 RID: 1718
		public static string spawnSoundString;

		// Token: 0x040006B7 RID: 1719
		public static GameObject spawnEffectPrefab;
	}
}
