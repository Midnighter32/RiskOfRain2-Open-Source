using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000709 RID: 1801
	public class FrozenState : BaseState
	{
		// Token: 0x060029FD RID: 10749 RVA: 0x000B064C File Offset: 0x000AE84C
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
					this.temporaryOverlay.AddToCharacerModel(component);
				}
			}
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				this.modelAnimator.enabled = false;
				this.duration = this.freezeDuration;
				EffectManager.SpawnEffect(FrozenState.frozenEffectPrefab, new EffectData
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
			base.healthComponent.isInFrozenState = true;
		}

		// Token: 0x060029FE RID: 10750 RVA: 0x000B07C8 File Offset: 0x000AE9C8
		public override void OnExit()
		{
			if (this.modelAnimator)
			{
				this.modelAnimator.enabled = true;
			}
			if (this.temporaryOverlay)
			{
				EntityState.Destroy(this.temporaryOverlay);
			}
			EffectManager.SpawnEffect(FrozenState.frozenEffectPrefab, new EffectData
			{
				origin = base.characterBody.corePosition,
				scale = (base.characterBody ? base.characterBody.radius : 1f)
			}, false);
			base.healthComponent.isInFrozenState = false;
			base.OnExit();
		}

		// Token: 0x060029FF RID: 10751 RVA: 0x000B085E File Offset: 0x000AEA5E
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002A00 RID: 10752 RVA: 0x0000C68F File Offset: 0x0000A88F
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		// Token: 0x040025CD RID: 9677
		private float duration;

		// Token: 0x040025CE RID: 9678
		private Animator modelAnimator;

		// Token: 0x040025CF RID: 9679
		private TemporaryOverlay temporaryOverlay;

		// Token: 0x040025D0 RID: 9680
		public float freezeDuration = 0.35f;

		// Token: 0x040025D1 RID: 9681
		public static GameObject frozenEffectPrefab;

		// Token: 0x040025D2 RID: 9682
		public static GameObject executeEffectPrefab;
	}
}
