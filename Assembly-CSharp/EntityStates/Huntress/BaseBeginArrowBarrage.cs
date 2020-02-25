using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x0200082F RID: 2095
	public class BaseBeginArrowBarrage : BaseState
	{
		// Token: 0x06002F6D RID: 12141 RVA: 0x000CA948 File Offset: 0x000C8B48
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(BaseBeginArrowBarrage.blinkSoundString, base.gameObject);
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
				this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
			}
			this.prepDuration = this.basePrepDuration / this.attackSpeedStat;
			base.PlayAnimation("FullBody, Override", "BeginArrowRain", "BeginArrowRain.playbackRate", this.prepDuration);
			if (base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
			Vector3 direction = base.GetAimRay().direction;
			direction.y = 0f;
			direction.Normalize();
			Vector3 up = Vector3.up;
			this.worldBlinkVector = Matrix4x4.TRS(base.transform.position, Util.QuaternionSafeLookRotation(direction, up), new Vector3(1f, 1f, 1f)).MultiplyPoint3x4(this.blinkVector) - base.transform.position;
			this.worldBlinkVector.Normalize();
		}

		// Token: 0x06002F6E RID: 12142 RVA: 0x000CAA90 File Offset: 0x000C8C90
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.worldBlinkVector);
			effectData.origin = origin;
			EffectManager.SpawnEffect(BaseBeginArrowBarrage.blinkPrefab, effectData, false);
		}

		// Token: 0x06002F6F RID: 12143 RVA: 0x000CAAC8 File Offset: 0x000C8CC8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.prepDuration && !this.beginBlink)
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
				base.characterMotor.rootMotion += this.worldBlinkVector * (base.characterBody.jumpPower * this.jumpCoefficient * Time.fixedDeltaTime);
			}
			if (base.fixedAge >= this.blinkDuration + this.prepDuration && base.isAuthority)
			{
				this.outer.SetNextState(this.InstantiateNextState());
			}
		}

		// Token: 0x06002F70 RID: 12144 RVA: 0x0000AC7F File Offset: 0x00008E7F
		protected virtual EntityState InstantiateNextState()
		{
			return null;
		}

		// Token: 0x06002F71 RID: 12145 RVA: 0x000CABD8 File Offset: 0x000C8DD8
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

		// Token: 0x04002CFA RID: 11514
		private Transform modelTransform;

		// Token: 0x04002CFB RID: 11515
		[SerializeField]
		public float basePrepDuration;

		// Token: 0x04002CFC RID: 11516
		[SerializeField]
		public float blinkDuration = 0.3f;

		// Token: 0x04002CFD RID: 11517
		[SerializeField]
		public float jumpCoefficient = 25f;

		// Token: 0x04002CFE RID: 11518
		public static GameObject blinkPrefab;

		// Token: 0x04002CFF RID: 11519
		public static string blinkSoundString;

		// Token: 0x04002D00 RID: 11520
		[SerializeField]
		public Vector3 blinkVector;

		// Token: 0x04002D01 RID: 11521
		private Vector3 worldBlinkVector;

		// Token: 0x04002D02 RID: 11522
		private float prepDuration;

		// Token: 0x04002D03 RID: 11523
		private bool beginBlink;

		// Token: 0x04002D04 RID: 11524
		private CharacterModel characterModel;

		// Token: 0x04002D05 RID: 11525
		private HurtBoxGroup hurtboxGroup;
	}
}
