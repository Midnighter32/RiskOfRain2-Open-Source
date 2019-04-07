using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x02000142 RID: 322
	public class SpawnState : BaseState
	{
		// Token: 0x0600062D RID: 1581 RVA: 0x0001CD44 File Offset: 0x0001AF44
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
				EffectManager.instance.SpawnEffect(SpawnState.spawnEffectPrefab, effectData, false);
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

		// Token: 0x0600062E RID: 1582 RVA: 0x0001CE27 File Offset: 0x0001B027
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400074F RID: 1871
		private float stopwatch;

		// Token: 0x04000750 RID: 1872
		public static float duration = 4f;

		// Token: 0x04000751 RID: 1873
		public static string spawnSoundString;

		// Token: 0x04000752 RID: 1874
		public static GameObject spawnEffectPrefab;

		// Token: 0x04000753 RID: 1875
		public static Material destealthMaterial;
	}
}
