using System;
using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x02000819 RID: 2073
	public class BlinkState : BaseState
	{
		// Token: 0x06002EFB RID: 12027 RVA: 0x000C80A8 File Offset: 0x000C62A8
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(this.beginSoundString, base.gameObject);
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				this.animator = this.modelTransform.GetComponent<Animator>();
				this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
				this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
				this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			}
			if (this.disappearWhileBlinking)
			{
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
				if (this.childLocator)
				{
					this.childLocator.FindChild("DustCenter").gameObject.SetActive(false);
				}
			}
			if (base.characterMotor)
			{
				base.characterMotor.enabled = false;
			}
			base.gameObject.layer = LayerIndex.fakeActor.intVal;
			base.characterMotor.Motor.RebuildCollidableLayers();
			this.CalculateBlinkDestination();
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
		}

		// Token: 0x06002EFC RID: 12028 RVA: 0x000C81F4 File Offset: 0x000C63F4
		private void CalculateBlinkDestination()
		{
			Vector3 vector = Vector3.zero;
			Ray aimRay = base.GetAimRay();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.maxDistanceFilter = this.blinkDistance;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.teamMaskFilter.RemoveTeam(TeamComponent.GetObjectTeam(base.gameObject));
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
			bullseyeSearch.RefreshCandidates();
			HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			if (hurtBox)
			{
				vector = hurtBox.transform.position - base.transform.position;
			}
			this.blinkDestination = base.transform.position;
			this.blinkStart = base.transform.position;
			NodeGraph groundNodes = SceneInfo.instance.groundNodes;
			NodeGraph.NodeIndex nodeIndex = groundNodes.FindClosestNode(base.transform.position + vector, base.characterBody.hullClassification);
			groundNodes.GetNodePosition(nodeIndex, out this.blinkDestination);
			this.blinkDestination += base.transform.position - base.characterBody.footPosition;
			base.characterDirection.forward = vector;
		}

		// Token: 0x06002EFD RID: 12029 RVA: 0x000C8334 File Offset: 0x000C6534
		private void CreateBlinkEffect(Vector3 origin)
		{
			if (this.blinkPrefab)
			{
				EffectData effectData = new EffectData();
				effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkDestination - this.blinkStart);
				effectData.origin = origin;
				EffectManager.SpawnEffect(this.blinkPrefab, effectData, false);
			}
		}

		// Token: 0x06002EFE RID: 12030 RVA: 0x000C8384 File Offset: 0x000C6584
		private void SetPosition(Vector3 newPosition)
		{
			if (base.characterMotor)
			{
				base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
			}
		}

		// Token: 0x06002EFF RID: 12031 RVA: 0x000C83AC File Offset: 0x000C65AC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			if (!this.hasBlinked)
			{
				this.SetPosition(Vector3.Lerp(this.blinkStart, this.blinkDestination, base.fixedAge / this.duration));
			}
			if (base.fixedAge >= this.duration - this.destinationAlertDuration && !this.hasBlinked)
			{
				this.hasBlinked = true;
				if (this.blinkDestinationPrefab)
				{
					this.blinkDestinationInstance = UnityEngine.Object.Instantiate<GameObject>(this.blinkDestinationPrefab, this.blinkDestination, Quaternion.identity);
					this.blinkDestinationInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.destinationAlertDuration;
				}
				this.SetPosition(this.blinkDestination);
			}
			if (base.fixedAge >= this.duration)
			{
				this.ExitCleanup();
			}
			if (base.fixedAge >= this.duration + this.exitDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002F00 RID: 12032 RVA: 0x000C84B4 File Offset: 0x000C66B4
		private void ExitCleanup()
		{
			if (this.isExiting)
			{
				return;
			}
			this.isExiting = true;
			base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			base.characterMotor.Motor.RebuildCollidableLayers();
			Util.PlaySound(this.endSoundString, base.gameObject);
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
			this.modelTransform = base.GetModelTransform();
			if (this.blastAttackDamageCoefficient > 0f)
			{
				new BlastAttack
				{
					attacker = base.gameObject,
					inflictor = base.gameObject,
					teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
					baseDamage = this.damageStat * this.blastAttackDamageCoefficient,
					baseForce = this.blastAttackForce,
					position = this.blinkDestination,
					radius = this.blastAttackRadius,
					falloffModel = BlastAttack.FalloffModel.Linear
				}.Fire();
			}
			if (this.disappearWhileBlinking)
			{
				if (this.modelTransform && this.destealthMaterial)
				{
					TemporaryOverlay temporaryOverlay = this.animator.gameObject.AddComponent<TemporaryOverlay>();
					temporaryOverlay.duration = 1f;
					temporaryOverlay.destroyComponentOnEnd = true;
					temporaryOverlay.originalMaterial = this.destealthMaterial;
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
				if (this.childLocator)
				{
					this.childLocator.FindChild("DustCenter").gameObject.SetActive(true);
				}
				base.PlayAnimation("Gesture, Additive", "BlinkEnd", "BlinkEnd.playbackRate", this.exitDuration);
			}
			if (this.blinkDestinationInstance)
			{
				EntityState.Destroy(this.blinkDestinationInstance);
			}
			if (base.characterMotor)
			{
				base.characterMotor.enabled = true;
			}
		}

		// Token: 0x06002F01 RID: 12033 RVA: 0x000C86E8 File Offset: 0x000C68E8
		public override void OnExit()
		{
			base.OnExit();
			this.ExitCleanup();
		}

		// Token: 0x04002C47 RID: 11335
		private Transform modelTransform;

		// Token: 0x04002C48 RID: 11336
		[SerializeField]
		public bool disappearWhileBlinking;

		// Token: 0x04002C49 RID: 11337
		[SerializeField]
		public GameObject blinkPrefab;

		// Token: 0x04002C4A RID: 11338
		[SerializeField]
		public GameObject blinkDestinationPrefab;

		// Token: 0x04002C4B RID: 11339
		[SerializeField]
		public Material destealthMaterial;

		// Token: 0x04002C4C RID: 11340
		private Vector3 blinkDestination = Vector3.zero;

		// Token: 0x04002C4D RID: 11341
		private Vector3 blinkStart = Vector3.zero;

		// Token: 0x04002C4E RID: 11342
		[SerializeField]
		public float duration = 0.3f;

		// Token: 0x04002C4F RID: 11343
		[SerializeField]
		public float exitDuration;

		// Token: 0x04002C50 RID: 11344
		[SerializeField]
		public float destinationAlertDuration;

		// Token: 0x04002C51 RID: 11345
		[SerializeField]
		public float blinkDistance = 25f;

		// Token: 0x04002C52 RID: 11346
		[SerializeField]
		public string beginSoundString;

		// Token: 0x04002C53 RID: 11347
		[SerializeField]
		public string endSoundString;

		// Token: 0x04002C54 RID: 11348
		[SerializeField]
		public float blastAttackRadius;

		// Token: 0x04002C55 RID: 11349
		[SerializeField]
		public float blastAttackDamageCoefficient;

		// Token: 0x04002C56 RID: 11350
		[SerializeField]
		public float blastAttackForce;

		// Token: 0x04002C57 RID: 11351
		[SerializeField]
		public float blastAttackProcCoefficient;

		// Token: 0x04002C58 RID: 11352
		private Animator animator;

		// Token: 0x04002C59 RID: 11353
		private CharacterModel characterModel;

		// Token: 0x04002C5A RID: 11354
		private HurtBoxGroup hurtboxGroup;

		// Token: 0x04002C5B RID: 11355
		private ChildLocator childLocator;

		// Token: 0x04002C5C RID: 11356
		private GameObject blinkDestinationInstance;

		// Token: 0x04002C5D RID: 11357
		private bool isExiting;

		// Token: 0x04002C5E RID: 11358
		private bool hasBlinked;
	}
}
