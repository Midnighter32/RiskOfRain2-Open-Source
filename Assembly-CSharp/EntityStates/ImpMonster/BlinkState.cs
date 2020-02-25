using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000821 RID: 2081
	public class BlinkState : BaseState
	{
		// Token: 0x06002F27 RID: 12071 RVA: 0x000C9444 File Offset: 0x000C7644
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

		// Token: 0x06002F28 RID: 12072 RVA: 0x000C95C0 File Offset: 0x000C77C0
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkDestination - this.blinkStart);
			effectData.origin = origin;
			EffectManager.SpawnEffect(BlinkState.blinkPrefab, effectData, false);
		}

		// Token: 0x06002F29 RID: 12073 RVA: 0x000C8384 File Offset: 0x000C6584
		private void SetPosition(Vector3 newPosition)
		{
			if (base.characterMotor)
			{
				base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
			}
		}

		// Token: 0x06002F2A RID: 12074 RVA: 0x000C9604 File Offset: 0x000C7804
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

		// Token: 0x06002F2B RID: 12075 RVA: 0x000C9698 File Offset: 0x000C7898
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

		// Token: 0x04002CA0 RID: 11424
		private Transform modelTransform;

		// Token: 0x04002CA1 RID: 11425
		public static GameObject blinkPrefab;

		// Token: 0x04002CA2 RID: 11426
		public static Material destealthMaterial;

		// Token: 0x04002CA3 RID: 11427
		private float stopwatch;

		// Token: 0x04002CA4 RID: 11428
		private Vector3 blinkDestination = Vector3.zero;

		// Token: 0x04002CA5 RID: 11429
		private Vector3 blinkStart = Vector3.zero;

		// Token: 0x04002CA6 RID: 11430
		public static float duration = 0.3f;

		// Token: 0x04002CA7 RID: 11431
		public static float blinkDistance = 25f;

		// Token: 0x04002CA8 RID: 11432
		public static string beginSoundString;

		// Token: 0x04002CA9 RID: 11433
		public static string endSoundString;

		// Token: 0x04002CAA RID: 11434
		private Animator animator;

		// Token: 0x04002CAB RID: 11435
		private CharacterModel characterModel;

		// Token: 0x04002CAC RID: 11436
		private HurtBoxGroup hurtboxGroup;
	}
}
