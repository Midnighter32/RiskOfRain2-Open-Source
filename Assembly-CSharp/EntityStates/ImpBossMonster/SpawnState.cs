using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x0200081E RID: 2078
	public class SpawnState : BaseState
	{
		// Token: 0x06002F1A RID: 12058 RVA: 0x000C90D8 File Offset: 0x000C72D8
		public override void OnEnter()
		{
			base.OnEnter();
			Animator modelAnimator = base.GetModelAnimator();
			base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound(SpawnState.spawnSoundString, base.gameObject);
			if (SpawnState.spawnEffectPrefab)
			{
				EffectData effectData = new EffectData();
				effectData.origin = base.transform.position;
				EffectManager.SpawnEffect(SpawnState.spawnEffectPrefab, effectData, false);
			}
			if (SpawnState.destealthMaterial)
			{
				TemporaryOverlay temporaryOverlay = modelAnimator.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay.duration = 1f;
				temporaryOverlay.destroyComponentOnEnd = true;
				temporaryOverlay.originalMaterial = SpawnState.destealthMaterial;
				temporaryOverlay.inspectorCharacterModel = modelAnimator.gameObject.GetComponent<CharacterModel>();
				temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
				temporaryOverlay.animateShaderAlpha = true;
			}
		}

		// Token: 0x06002F1B RID: 12059 RVA: 0x000C91B6 File Offset: 0x000C73B6
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002F1C RID: 12060 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002C8F RID: 11407
		private float stopwatch;

		// Token: 0x04002C90 RID: 11408
		public static float duration = 4f;

		// Token: 0x04002C91 RID: 11409
		public static string spawnSoundString;

		// Token: 0x04002C92 RID: 11410
		public static GameObject spawnEffectPrefab;

		// Token: 0x04002C93 RID: 11411
		public static Material destealthMaterial;
	}
}
