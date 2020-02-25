using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x0200070B RID: 1803
	public class GenericCharacterDeath : BaseState
	{
		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06002A0B RID: 10763 RVA: 0x000B0B6A File Offset: 0x000AED6A
		// (set) Token: 0x06002A0C RID: 10764 RVA: 0x000B0B72 File Offset: 0x000AED72
		private protected Transform cachedModelTransform { protected get; private set; }

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06002A0D RID: 10765 RVA: 0x000B0B7B File Offset: 0x000AED7B
		// (set) Token: 0x06002A0E RID: 10766 RVA: 0x000B0B83 File Offset: 0x000AED83
		private protected bool isBrittle { protected get; private set; }

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06002A0F RID: 10767 RVA: 0x000B0B8C File Offset: 0x000AED8C
		// (set) Token: 0x06002A10 RID: 10768 RVA: 0x000B0B94 File Offset: 0x000AED94
		private protected bool isVoidDeath { protected get; private set; }

		// Token: 0x06002A11 RID: 10769 RVA: 0x000B0B9D File Offset: 0x000AED9D
		protected virtual float GetDeathAnimationCrossFadeDuration()
		{
			return 0.1f;
		}

		// Token: 0x06002A12 RID: 10770 RVA: 0x000B0BA4 File Offset: 0x000AEDA4
		public override void OnEnter()
		{
			base.OnEnter();
			this.cachedModelTransform = (base.modelLocator ? base.modelLocator.modelTransform : null);
			this.isBrittle = (base.characterBody && base.characterBody.inventory && base.characterBody.inventory.GetItemCount(ItemIndex.LunarDagger) > 0);
			this.isVoidDeath = (base.healthComponent && (base.healthComponent.killingDamageType & DamageType.VoidDeath) > DamageType.Generic);
			if (this.isVoidDeath)
			{
				if (base.characterBody && base.isAuthority)
				{
					EffectManager.SpawnEffect(GenericCharacterDeath.voidDeathEffect, new EffectData
					{
						origin = base.characterBody.corePosition,
						scale = base.characterBody.bestFitRadius
					}, true);
				}
				if (this.cachedModelTransform)
				{
					EntityState.Destroy(this.cachedModelTransform.gameObject);
					this.cachedModelTransform = null;
				}
			}
			if (this.cachedModelTransform)
			{
				if (this.isBrittle)
				{
					TemporaryOverlay temporaryOverlay = this.cachedModelTransform.gameObject.AddComponent<TemporaryOverlay>();
					temporaryOverlay.duration = 0.5f;
					temporaryOverlay.destroyObjectOnEnd = true;
					temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matShatteredGlass");
					temporaryOverlay.destroyEffectPrefab = (GameObject)Resources.Load("Prefabs/Effects/BrittleDeath");
					temporaryOverlay.destroyEffectChildString = "Chest";
					temporaryOverlay.inspectorCharacterModel = this.cachedModelTransform.gameObject.GetComponent<CharacterModel>();
					temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
					temporaryOverlay.animateShaderAlpha = true;
				}
				if (base.cameraTargetParams)
				{
					ChildLocator component = this.cachedModelTransform.GetComponent<ChildLocator>();
					if (component)
					{
						Transform transform = component.FindChild("Chest");
						if (transform)
						{
							base.cameraTargetParams.cameraPivotTransform = transform;
							base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
							base.cameraTargetParams.dontRaycastToPivot = true;
						}
					}
				}
			}
			this.PlayDeathSound();
			this.PlayDeathAnimation(0.1f);
		}

		// Token: 0x06002A13 RID: 10771 RVA: 0x000B0DC4 File Offset: 0x000AEFC4
		protected virtual void PlayDeathSound()
		{
			if (base.sfxLocator && base.sfxLocator.deathSound != "")
			{
				Util.PlaySound(base.sfxLocator.deathSound, base.gameObject);
			}
		}

		// Token: 0x06002A14 RID: 10772 RVA: 0x000B0E04 File Offset: 0x000AF004
		protected virtual void PlayDeathAnimation(float crossfadeDuration = 0.1f)
		{
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				modelAnimator.CrossFadeInFixedTime("Death", crossfadeDuration);
			}
		}

		// Token: 0x06002A15 RID: 10773 RVA: 0x000B0E2C File Offset: 0x000AF02C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				bool flag = false;
				bool flag2 = true;
				if (base.characterMotor)
				{
					flag = base.characterMotor.isGrounded;
					flag2 = base.characterMotor.atRest;
				}
				else if (base.rigidbodyMotor)
				{
					flag = false;
					flag2 = false;
				}
				this.fallingStopwatch = (flag ? 0f : (this.fallingStopwatch + Time.fixedDeltaTime));
				this.restStopwatch = ((!flag2) ? 0f : (this.restStopwatch + Time.fixedDeltaTime));
				if ((this.restStopwatch >= GenericCharacterDeath.bodyPreservationDuration || this.fallingStopwatch >= GenericCharacterDeath.maxFallDuration || base.fixedAge > GenericCharacterDeath.hardCutoffDuration) && this.shouldAutoDestroy)
				{
					this.OnPreDestroyBodyServer();
					EntityState.Destroy(base.gameObject);
					return;
				}
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06002A16 RID: 10774 RVA: 0x0000B933 File Offset: 0x00009B33
		protected virtual bool shouldAutoDestroy
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002A17 RID: 10775 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void OnPreDestroyBodyServer()
		{
		}

		// Token: 0x06002A18 RID: 10776 RVA: 0x000B0EFE File Offset: 0x000AF0FE
		protected void DestroyModel()
		{
			if (this.cachedModelTransform)
			{
				EntityState.Destroy(this.cachedModelTransform.gameObject);
				this.cachedModelTransform = null;
			}
		}

		// Token: 0x06002A19 RID: 10777 RVA: 0x000B0F24 File Offset: 0x000AF124
		public override void OnExit()
		{
			if (this.shouldAutoDestroy && this.fallingStopwatch >= GenericCharacterDeath.maxFallDuration)
			{
				this.DestroyModel();
			}
			base.OnExit();
		}

		// Token: 0x06002A1A RID: 10778 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040025E9 RID: 9705
		private static readonly float bodyPreservationDuration = 1f;

		// Token: 0x040025EA RID: 9706
		private static readonly float hardCutoffDuration = 10f;

		// Token: 0x040025EB RID: 9707
		private static readonly float maxFallDuration = 4f;

		// Token: 0x040025EC RID: 9708
		public static GameObject voidDeathEffect;

		// Token: 0x040025ED RID: 9709
		private float restStopwatch;

		// Token: 0x040025EE RID: 9710
		private float fallingStopwatch;
	}
}
