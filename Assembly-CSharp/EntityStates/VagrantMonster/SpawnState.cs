using System;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantMonster
{
	// Token: 0x02000805 RID: 2053
	public class SpawnState : BaseState
	{
		// Token: 0x06002EAF RID: 11951 RVA: 0x000C685C File Offset: 0x000C4A5C
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			if (SpawnState.spawnEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, "SpawnOrigin", false);
			}
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				modelTransform.GetComponent<PrintController>().enabled = true;
			}
		}

		// Token: 0x06002EB0 RID: 11952 RVA: 0x000C68D7 File Offset: 0x000C4AD7
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002EB1 RID: 11953 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002BD8 RID: 11224
		private float stopwatch;

		// Token: 0x04002BD9 RID: 11225
		public static float duration = 4f;

		// Token: 0x04002BDA RID: 11226
		public static string spawnSoundString;

		// Token: 0x04002BDB RID: 11227
		public static GameObject spawnEffectPrefab;
	}
}
