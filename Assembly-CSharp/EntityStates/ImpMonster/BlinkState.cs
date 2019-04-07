using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000145 RID: 325
	public class BlinkState : BaseState
	{
		// Token: 0x0600063A RID: 1594 RVA: 0x0001D0BC File Offset: 0x0001B2BC
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(BlinkState.beginSoundString, base.gameObject);
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				this.animator = this.modelTransform.GetComponent<Animator>();
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
			if (base.characterMotor)
			{
				base.characterMotor.enabled = false;
			}
			Vector3 b = base.inputBank.moveVector * BlinkState.blinkDistance;
			this.blinkDestination = base.transform.position;
			this.blinkStart = base.transform.position;
			NodeGraph groundNodes = SceneInfo.instance.groundNodes;
			NodeGraph.NodeIndex nodeIndex = groundNodes.FindClosestNode(base.transform.position + b, base.characterBody.hullClassification);
			groundNodes.GetNodePosition(nodeIndex, out this.blinkDestination);
			this.blinkDestination += base.transform.position - base.characterBody.footPosition;
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0001D238 File Offset: 0x0001B438
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkDestination - this.blinkStart);
			effectData.origin = origin;
			EffectManager.instance.SpawnEffect(BlinkState.blinkPrefab, effectData, false);
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x0001C03B File Offset: 0x0001A23B
		private void SetPosition(Vector3 newPosition)
		{
			if (base.characterMotor)
			{
				base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
			}
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x0001D280 File Offset: 0x0001B480
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (base.characterMotor && base.characterDirection)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			this.SetPosition(Vector3.Lerp(this.blinkStart, this.blinkDestination, this.stopwatch / BlinkState.duration));
			if (this.stopwatch >= BlinkState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x0001D314 File Offset: 0x0001B514
		public override void OnExit()
		{
			Util.PlaySound(BlinkState.endSoundString, base.gameObject);
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform && BlinkState.destealthMaterial)
			{
				TemporaryOverlay temporaryOverlay = this.animator.gameObject.AddComponent<TemporaryOverlay>();
				temporaryOverlay.duration = 1f;
				temporaryOverlay.destroyComponentOnEnd = true;
				temporaryOverlay.originalMaterial = BlinkState.destealthMaterial;
				temporaryOverlay.inspectorCharacterModel = this.animator.gameObject.GetComponent<CharacterModel>();
				temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
				temporaryOverlay.animateShaderAlpha = true;
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
			if (base.characterMotor)
			{
				base.characterMotor.enabled = true;
			}
			base.PlayAnimation("Gesture, Additive", "BlinkEnd");
			base.OnExit();
		}

		// Token: 0x04000760 RID: 1888
		private Transform modelTransform;

		// Token: 0x04000761 RID: 1889
		public static GameObject blinkPrefab;

		// Token: 0x04000762 RID: 1890
		public static Material destealthMaterial;

		// Token: 0x04000763 RID: 1891
		private float stopwatch;

		// Token: 0x04000764 RID: 1892
		private Vector3 blinkDestination = Vector3.zero;

		// Token: 0x04000765 RID: 1893
		private Vector3 blinkStart = Vector3.zero;

		// Token: 0x04000766 RID: 1894
		public static float duration = 0.3f;

		// Token: 0x04000767 RID: 1895
		public static float blinkDistance = 25f;

		// Token: 0x04000768 RID: 1896
		public static string beginSoundString;

		// Token: 0x04000769 RID: 1897
		public static string endSoundString;

		// Token: 0x0400076A RID: 1898
		private Animator animator;

		// Token: 0x0400076B RID: 1899
		private CharacterModel characterModel;

		// Token: 0x0400076C RID: 1900
		private HurtBoxGroup hurtboxGroup;
	}
}
