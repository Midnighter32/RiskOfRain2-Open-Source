using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x02000150 RID: 336
	public class BlinkState : BaseState
	{
		// Token: 0x06000677 RID: 1655 RVA: 0x0001E78C File Offset: 0x0001C98C
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(BlinkState.beginSoundString, base.gameObject);
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
			this.blinkVector = base.inputBank.aimDirection;
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x0001E850 File Offset: 0x0001CA50
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
			effectData.origin = origin;
			EffectManager.instance.SpawnEffect(BlinkState.blinkPrefab, effectData, false);
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x0001E88C File Offset: 0x0001CA8C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (base.characterMotor && base.characterDirection)
			{
				base.characterMotor.velocity = Vector3.zero;
				base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * BlinkState.speedCoefficient * Time.fixedDeltaTime);
			}
			if (this.stopwatch >= BlinkState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x0001E930 File Offset: 0x0001CB30
		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				Util.PlaySound(BlinkState.endSoundString, base.gameObject);
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

		// Token: 0x040007C2 RID: 1986
		private Transform modelTransform;

		// Token: 0x040007C3 RID: 1987
		public static GameObject blinkPrefab;

		// Token: 0x040007C4 RID: 1988
		private float stopwatch;

		// Token: 0x040007C5 RID: 1989
		private Vector3 blinkVector = Vector3.zero;

		// Token: 0x040007C6 RID: 1990
		public static float duration = 0.3f;

		// Token: 0x040007C7 RID: 1991
		public static float speedCoefficient = 25f;

		// Token: 0x040007C8 RID: 1992
		public static string beginSoundString;

		// Token: 0x040007C9 RID: 1993
		public static string endSoundString;

		// Token: 0x040007CA RID: 1994
		private CharacterModel characterModel;

		// Token: 0x040007CB RID: 1995
		private HurtBoxGroup hurtboxGroup;
	}
}
