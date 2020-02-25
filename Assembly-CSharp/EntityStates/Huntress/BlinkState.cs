using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x02000830 RID: 2096
	public class BlinkState : BaseState
	{
		// Token: 0x06002F73 RID: 12147 RVA: 0x000CAD48 File Offset: 0x000C8F48
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(this.beginSoundString, base.gameObject);
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
				this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
			}
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
			this.blinkVector = this.GetBlinkVector();
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
		}

		// Token: 0x06002F74 RID: 12148 RVA: 0x000CAE07 File Offset: 0x000C9007
		protected virtual Vector3 GetBlinkVector()
		{
			return base.inputBank.aimDirection;
		}

		// Token: 0x06002F75 RID: 12149 RVA: 0x000CAE14 File Offset: 0x000C9014
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
			effectData.origin = origin;
			EffectManager.SpawnEffect(BlinkState.blinkPrefab, effectData, false);
		}

		// Token: 0x06002F76 RID: 12150 RVA: 0x000CAE4C File Offset: 0x000C904C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (base.characterMotor && base.characterDirection)
			{
				base.characterMotor.velocity = Vector3.zero;
				base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * this.speedCoefficient * Time.fixedDeltaTime);
			}
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002F77 RID: 12151 RVA: 0x000CAEF0 File Offset: 0x000C90F0
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				Util.PlaySound(this.endSoundString, base.gameObject);
				this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
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

		// Token: 0x04002D06 RID: 11526
		private Transform modelTransform;

		// Token: 0x04002D07 RID: 11527
		public static GameObject blinkPrefab;

		// Token: 0x04002D08 RID: 11528
		private float stopwatch;

		// Token: 0x04002D09 RID: 11529
		private Vector3 blinkVector = Vector3.zero;

		// Token: 0x04002D0A RID: 11530
		[SerializeField]
		public float duration = 0.3f;

		// Token: 0x04002D0B RID: 11531
		[SerializeField]
		public float speedCoefficient = 25f;

		// Token: 0x04002D0C RID: 11532
		[SerializeField]
		public string beginSoundString;

		// Token: 0x04002D0D RID: 11533
		[SerializeField]
		public string endSoundString;

		// Token: 0x04002D0E RID: 11534
		private CharacterModel characterModel;

		// Token: 0x04002D0F RID: 11535
		private HurtBoxGroup hurtboxGroup;
	}
}
