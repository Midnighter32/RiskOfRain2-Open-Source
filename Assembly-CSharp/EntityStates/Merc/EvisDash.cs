using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Merc
{
	// Token: 0x020007C3 RID: 1987
	public class EvisDash : BaseState
	{
		// Token: 0x06002D65 RID: 11621 RVA: 0x000BFF84 File Offset: 0x000BE184
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
			if (NetworkServer.active)
			{
				base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);
			}
			base.PlayAnimation("FullBody, Override", "EvisPrep", "EvisPrep.playbackRate", EvisDash.dashPrepDuration);
			this.dashVector = base.inputBank.aimDirection;
			base.characterDirection.forward = this.dashVector;
		}

		// Token: 0x06002D66 RID: 11622 RVA: 0x000C0064 File Offset: 0x000BE264
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.dashVector);
			effectData.origin = origin;
			EffectManager.SpawnEffect(EvisDash.blinkPrefab, effectData, false);
		}

		// Token: 0x06002D67 RID: 11623 RVA: 0x000C009C File Offset: 0x000BE29C
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
					base.characterMotor.rootMotion += this.dashVector * (this.moveSpeedStat * EvisDash.speedCoefficient * Time.fixedDeltaTime);
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

		// Token: 0x06002D68 RID: 11624 RVA: 0x000C0318 File Offset: 0x000BE518
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
			if (NetworkServer.active)
			{
				base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
			}
			base.OnExit();
		}

		// Token: 0x040029B9 RID: 10681
		private Transform modelTransform;

		// Token: 0x040029BA RID: 10682
		public static GameObject blinkPrefab;

		// Token: 0x040029BB RID: 10683
		private float stopwatch;

		// Token: 0x040029BC RID: 10684
		private Vector3 dashVector = Vector3.zero;

		// Token: 0x040029BD RID: 10685
		public static float smallHopVelocity;

		// Token: 0x040029BE RID: 10686
		public static float dashPrepDuration;

		// Token: 0x040029BF RID: 10687
		public static float dashDuration = 0.3f;

		// Token: 0x040029C0 RID: 10688
		public static float speedCoefficient = 25f;

		// Token: 0x040029C1 RID: 10689
		public static string beginSoundString;

		// Token: 0x040029C2 RID: 10690
		public static string endSoundString;

		// Token: 0x040029C3 RID: 10691
		public static float overlapSphereRadius;

		// Token: 0x040029C4 RID: 10692
		public static float lollypopFactor;

		// Token: 0x040029C5 RID: 10693
		private Animator animator;

		// Token: 0x040029C6 RID: 10694
		private CharacterModel characterModel;

		// Token: 0x040029C7 RID: 10695
		private HurtBoxGroup hurtboxGroup;

		// Token: 0x040029C8 RID: 10696
		private bool isDashing;
	}
}
