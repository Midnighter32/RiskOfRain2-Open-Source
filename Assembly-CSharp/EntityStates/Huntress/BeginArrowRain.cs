using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x0200014F RID: 335
	public class BeginArrowRain : BaseState
	{
		// Token: 0x06000671 RID: 1649 RVA: 0x0001E3E8 File Offset: 0x0001C5E8
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(BeginArrowRain.blinkSoundString, base.gameObject);
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
				this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
			}
			this.prepDuration = BeginArrowRain.basePrepDuration / this.attackSpeedStat;
			base.PlayAnimation("FullBody, Override", "BeginArrowRain", "BeginArrowRain.playbackRate", BeginArrowRain.basePrepDuration);
			if (base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			this.blinkVector = Vector3.up;
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0001E4B4 File Offset: 0x0001C6B4
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
			effectData.origin = origin;
			EffectManager.instance.SpawnEffect(BeginArrowRain.blinkPrefab, effectData, false);
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x0001E4F0 File Offset: 0x0001C6F0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.prepDuration && !this.beginBlink)
			{
				this.beginBlink = true;
				this.CreateBlinkEffect(base.transform.position);
				if (this.characterModel)
				{
					this.characterModel.invisibilityCount++;
				}
				if (this.hurtboxGroup)
				{
					HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
					int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
					hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
				}
			}
			if (this.beginBlink && base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
				base.characterMotor.rootMotion += this.blinkVector * (base.characterBody.jumpPower * BeginArrowRain.jumpCoefficient * Time.fixedDeltaTime);
			}
			if (this.stopwatch >= BeginArrowRain.blinkDuration + this.prepDuration && base.isAuthority)
			{
				this.outer.SetNextState(new ArrowRain());
			}
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x0001E610 File Offset: 0x0001C810
		public override void OnExit()
		{
			this.CreateBlinkEffect(base.transform.position);
			this.modelTransform = base.GetModelTransform();
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
			if (this.characterModel)
			{
				this.characterModel.invisibilityCount--;
			}
			if (this.hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}
			base.OnExit();
		}

		// Token: 0x040007B6 RID: 1974
		private Transform modelTransform;

		// Token: 0x040007B7 RID: 1975
		public static float basePrepDuration;

		// Token: 0x040007B8 RID: 1976
		public static float blinkDuration = 0.3f;

		// Token: 0x040007B9 RID: 1977
		public static float jumpCoefficient = 25f;

		// Token: 0x040007BA RID: 1978
		public static GameObject blinkPrefab;

		// Token: 0x040007BB RID: 1979
		public static string blinkSoundString;

		// Token: 0x040007BC RID: 1980
		private float stopwatch;

		// Token: 0x040007BD RID: 1981
		private Vector3 blinkVector = Vector3.zero;

		// Token: 0x040007BE RID: 1982
		private float prepDuration;

		// Token: 0x040007BF RID: 1983
		private bool beginBlink;

		// Token: 0x040007C0 RID: 1984
		private CharacterModel characterModel;

		// Token: 0x040007C1 RID: 1985
		private HurtBoxGroup hurtboxGroup;
	}
}
