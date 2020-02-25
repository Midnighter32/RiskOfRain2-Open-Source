using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClayBruiserMonster
{
	// Token: 0x020008D1 RID: 2257
	public class SpawnState : BaseState
	{
		// Token: 0x06003291 RID: 12945 RVA: 0x000DABD0 File Offset: 0x000D8DD0
		public override void OnEnter()
		{
			base.OnEnter();
			EffectManager.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, base.gameObject, SpawnState.spawnEffectChildString, false);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			PrintController printController = base.GetModelTransform().gameObject.AddComponent<PrintController>();
			printController.printTime = SpawnState.printDuration;
			printController.enabled = true;
			printController.startingPrintHeight = 0.3f;
			printController.maxPrintHeight = 0.3f;
			printController.startingPrintBias = SpawnState.startingPrintBias;
			printController.maxPrintBias = SpawnState.maxPrintBias;
			printController.disableWhenFinished = true;
			printController.printCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}

		// Token: 0x06003292 RID: 12946 RVA: 0x000DAC97 File Offset: 0x000D8E97
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003293 RID: 12947 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400319B RID: 12699
		public static float duration;

		// Token: 0x0400319C RID: 12700
		public static string spawnSoundString;

		// Token: 0x0400319D RID: 12701
		public static GameObject spawnEffectPrefab;

		// Token: 0x0400319E RID: 12702
		public static string spawnEffectChildString;

		// Token: 0x0400319F RID: 12703
		public static float startingPrintBias;

		// Token: 0x040031A0 RID: 12704
		public static float maxPrintBias;

		// Token: 0x040031A1 RID: 12705
		public static float printDuration;
	}
}
