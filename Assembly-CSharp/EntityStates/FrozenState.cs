using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000B2 RID: 178
	public class FrozenState : BaseState
	{
		// Token: 0x06000383 RID: 899 RVA: 0x0000E124 File Offset: 0x0000C324
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.sfxLocator && base.sfxLocator.barkSound != "")
			{
				Util.PlaySound(base.sfxLocator.barkSound, base.gameObject);
			}
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				CharacterModel component = modelTransform.GetComponent<CharacterModel>();
				if (component)
				{
					this.temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
					this.temporaryOverlay.duration = this.freezeDuration;
					this.temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matIsFrozen");
					this.temporaryOverlay.destroyComponentOnEnd = true;
					this.temporaryOverlay.animateShaderAlpha = true;
					this.temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
					this.temporaryOverlay.AddToCharacerModel(component);
				}
			}
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				this.modelAnimator.enabled = false;
				this.duration = this.freezeDuration;
				EffectManager.instance.SpawnEffect(FrozenState.frozenEffectPrefab, new EffectData
				{
					origin = base.characterBody.corePosition,
					scale = (base.characterBody ? base.characterBody.radius : 1f)
				}, false);
			}
			if (base.rigidbody && !base.rigidbody.isKinematic)
			{
				base.rigidbody.velocity = Vector3.zero;
				if (base.rigidbodyMotor)
				{
					base.rigidbodyMotor.moveVector = Vector3.zero;
				}
			}
			base.healthComponent.isFrozen = true;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0000E2E4 File Offset: 0x0000C4E4
		public override void OnExit()
		{
			if (this.modelAnimator)
			{
				this.modelAnimator.enabled = true;
			}
			EffectManager.instance.SpawnEffect(FrozenState.frozenEffectPrefab, new EffectData
			{
				origin = base.characterBody.corePosition,
				scale = (base.characterBody ? base.characterBody.radius : 1f)
			}, false);
			base.healthComponent.isFrozen = false;
			base.OnExit();
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0000E367 File Offset: 0x0000C567
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.stopwatch += Time.fixedDeltaTime;
				if (this.stopwatch >= this.duration)
				{
					this.outer.SetNextStateToMain();
				}
			}
		}

		// Token: 0x0400033A RID: 826
		private float stopwatch;

		// Token: 0x0400033B RID: 827
		private float duration;

		// Token: 0x0400033C RID: 828
		private Animator modelAnimator;

		// Token: 0x0400033D RID: 829
		private TemporaryOverlay temporaryOverlay;

		// Token: 0x0400033E RID: 830
		public float freezeDuration = 0.35f;

		// Token: 0x0400033F RID: 831
		public static GameObject frozenEffectPrefab;

		// Token: 0x04000340 RID: 832
		public static GameObject cullEffectPrefab;
	}
}
