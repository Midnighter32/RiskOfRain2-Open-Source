using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Merc
{
	// Token: 0x02000106 RID: 262
	public class EvisDash : BaseState
	{
		// Token: 0x06000515 RID: 1301 RVA: 0x00015FE0 File Offset: 0x000141E0
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(EvisDash.beginSoundString, base.gameObject);
			this.modelTransform = base.GetModelTransform();
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
			if (this.modelTransform)
			{
				this.animator = this.modelTransform.GetComponent<Animator>();
				this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
			}
			if (base.isAuthority)
			{
				base.SmallHop(base.characterMotor, EvisDash.smallHopVelocity);
			}
			base.PlayAnimation("FullBody, Override", "EvisPrep", "EvisPrep.playbackRate", EvisDash.dashPrepDuration);
			this.dashVector = base.inputBank.aimDirection;
			base.characterDirection.forward = this.dashVector;
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x000160B0 File Offset: 0x000142B0
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.dashVector);
			effectData.origin = origin;
			EffectManager.instance.SpawnEffect(EvisDash.blinkPrefab, effectData, false);
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x000160EC File Offset: 0x000142EC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch > EvisDash.dashPrepDuration && !this.isDashing)
			{
				this.isDashing = true;
				this.dashVector = base.inputBank.aimDirection;
				this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
				base.PlayCrossfade("FullBody, Override", "EvisLoop", 0.1f);
				if (this.modelTransform)
				{
					TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
					temporaryOverlay.duration = 0.6f;
					temporaryOverlay.animateShaderAlpha = true;
					temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
					temporaryOverlay.destroyComponentOnEnd = true;
					temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashBright");
					temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
					TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
					temporaryOverlay2.duration = 0.7f;
					temporaryOverlay2.animateShaderAlpha = true;
					temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
					temporaryOverlay2.destroyComponentOnEnd = true;
					temporaryOverlay2.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashExpanded");
					temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
				}
			}
			bool flag = this.stopwatch >= EvisDash.dashDuration + EvisDash.dashPrepDuration;
			if (this.isDashing)
			{
				if (base.characterMotor && base.characterDirection)
				{
					base.characterMotor.velocity = this.dashVector * this.moveSpeedStat * EvisDash.speedCoefficient;
				}
				if (base.isAuthority)
				{
					Collider[] array = Physics.OverlapSphere(base.transform.position, base.characterBody.radius + EvisDash.overlapSphereRadius * (flag ? EvisDash.lollypopFactor : 1f), LayerIndex.entityPrecise.mask);
					for (int i = 0; i < array.Length; i++)
					{
						HurtBox component = array[i].GetComponent<HurtBox>();
						if (component && component.healthComponent != base.healthComponent)
						{
							Evis nextState = new Evis();
							this.outer.SetNextState(nextState);
							return;
						}
					}
				}
			}
			if (flag && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0001635C File Offset: 0x0001455C
		public override void OnExit()
		{
			Util.PlaySound(EvisDash.endSoundString, base.gameObject);
			base.characterMotor.velocity *= 0.1f;
			base.SmallHop(base.characterMotor, EvisDash.smallHopVelocity);
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
			base.PlayAnimation("FullBody, Override", "EvisLoopExit");
			base.OnExit();
		}

		// Token: 0x04000515 RID: 1301
		private Transform modelTransform;

		// Token: 0x04000516 RID: 1302
		public static GameObject blinkPrefab;

		// Token: 0x04000517 RID: 1303
		private float stopwatch;

		// Token: 0x04000518 RID: 1304
		private Vector3 dashVector = Vector3.zero;

		// Token: 0x04000519 RID: 1305
		public static float smallHopVelocity;

		// Token: 0x0400051A RID: 1306
		public static float dashPrepDuration;

		// Token: 0x0400051B RID: 1307
		public static float dashDuration = 0.3f;

		// Token: 0x0400051C RID: 1308
		public static float speedCoefficient = 25f;

		// Token: 0x0400051D RID: 1309
		public static string beginSoundString;

		// Token: 0x0400051E RID: 1310
		public static string endSoundString;

		// Token: 0x0400051F RID: 1311
		public static float overlapSphereRadius;

		// Token: 0x04000520 RID: 1312
		public static float lollypopFactor;

		// Token: 0x04000521 RID: 1313
		private Animator animator;

		// Token: 0x04000522 RID: 1314
		private CharacterModel characterModel;

		// Token: 0x04000523 RID: 1315
		private HurtBoxGroup hurtboxGroup;

		// Token: 0x04000524 RID: 1316
		private bool isDashing;
	}
}
