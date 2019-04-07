using System;
using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EntityStates.ImpBossMonster
{
	// Token: 0x0200013D RID: 317
	public class BlinkState : BaseState
	{
		// Token: 0x0600060D RID: 1549 RVA: 0x0001BD8C File Offset: 0x00019F8C
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
				this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
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
			if (this.childLocator)
			{
				this.childLocator.FindChild("DustCenter").gameObject.SetActive(false);
			}
			if (base.characterMotor)
			{
				base.characterMotor.enabled = false;
			}
			this.CalculateBlinkDestination();
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x0001BEA8 File Offset: 0x0001A0A8
		private void CalculateBlinkDestination()
		{
			Vector3 vector = base.inputBank.aimDirection * BlinkState.blinkDistance;
			Ray aimRay = base.GetAimRay();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.maxDistanceFilter = BlinkState.blinkDistance;
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

		// Token: 0x0600060F RID: 1551 RVA: 0x0001BFF4 File Offset: 0x0001A1F4
		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkDestination - this.blinkStart);
			effectData.origin = origin;
			EffectManager.instance.SpawnEffect(BlinkState.blinkPrefab, effectData, false);
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x0001C03B File Offset: 0x0001A23B
		private void SetPosition(Vector3 newPosition)
		{
			if (base.characterMotor)
			{
				base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
			}
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x0001C064 File Offset: 0x0001A264
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.characterMotor)
			{
				base.characterMotor.velocity = Vector3.zero;
			}
			if (!this.hasBlinked)
			{
				this.SetPosition(Vector3.Lerp(this.blinkStart, this.blinkDestination, base.fixedAge / BlinkState.duration));
			}
			if (base.fixedAge >= BlinkState.duration - BlinkState.destinationAlertDuration && !this.hasBlinked)
			{
				this.hasBlinked = true;
				this.blinkDestinationInstance = UnityEngine.Object.Instantiate<GameObject>(BlinkState.blinkDestinationPrefab, this.blinkDestination, Quaternion.identity);
				this.blinkDestinationInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = BlinkState.destinationAlertDuration;
				this.SetPosition(this.blinkDestination);
			}
			if (base.fixedAge >= BlinkState.duration)
			{
				this.ExitCleanup();
			}
			if (base.fixedAge >= BlinkState.duration + BlinkState.exitDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x0001C158 File Offset: 0x0001A358
		private void ExitCleanup()
		{
			if (this.isExiting)
			{
				return;
			}
			this.isExiting = true;
			Util.PlaySound(BlinkState.endSoundString, base.gameObject);
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
			this.modelTransform = base.GetModelTransform();
			new BlastAttack
			{
				attacker = base.gameObject,
				inflictor = base.gameObject,
				teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
				baseDamage = this.damageStat * BlinkState.blastAttackDamageCoefficient,
				baseForce = BlinkState.blastAttackForce,
				position = this.blinkDestination,
				radius = BlinkState.blastAttackRadius,
				falloffModel = BlastAttack.FalloffModel.Linear
			}.Fire();
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
			if (this.childLocator)
			{
				this.childLocator.FindChild("DustCenter").gameObject.SetActive(true);
			}
			if (this.blinkDestinationInstance)
			{
				EntityState.Destroy(this.blinkDestinationInstance);
			}
			if (base.characterMotor)
			{
				base.characterMotor.enabled = true;
			}
			base.PlayAnimation("Gesture, Additive", "BlinkEnd", "BlinkEnd.playbackRate", BlinkState.exitDuration);
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x0001C347 File Offset: 0x0001A547
		public override void OnExit()
		{
			base.OnExit();
			this.ExitCleanup();
		}

		// Token: 0x04000708 RID: 1800
		private Transform modelTransform;

		// Token: 0x04000709 RID: 1801
		public static GameObject blinkPrefab;

		// Token: 0x0400070A RID: 1802
		public static GameObject blinkDestinationPrefab;

		// Token: 0x0400070B RID: 1803
		public static Material destealthMaterial;

		// Token: 0x0400070C RID: 1804
		private Vector3 blinkDestination = Vector3.zero;

		// Token: 0x0400070D RID: 1805
		private Vector3 blinkStart = Vector3.zero;

		// Token: 0x0400070E RID: 1806
		public static float duration = 0.3f;

		// Token: 0x0400070F RID: 1807
		public static float exitDuration;

		// Token: 0x04000710 RID: 1808
		public static float destinationAlertDuration;

		// Token: 0x04000711 RID: 1809
		public static float blinkDistance = 25f;

		// Token: 0x04000712 RID: 1810
		public static string beginSoundString;

		// Token: 0x04000713 RID: 1811
		public static string endSoundString;

		// Token: 0x04000714 RID: 1812
		public static float blastAttackRadius;

		// Token: 0x04000715 RID: 1813
		public static float blastAttackDamageCoefficient;

		// Token: 0x04000716 RID: 1814
		public static float blastAttackForce;

		// Token: 0x04000717 RID: 1815
		public static float blastAttackProcCoefficient;

		// Token: 0x04000718 RID: 1816
		private Animator animator;

		// Token: 0x04000719 RID: 1817
		private CharacterModel characterModel;

		// Token: 0x0400071A RID: 1818
		private HurtBoxGroup hurtboxGroup;

		// Token: 0x0400071B RID: 1819
		private ChildLocator childLocator;

		// Token: 0x0400071C RID: 1820
		private GameObject blinkDestinationInstance;

		// Token: 0x0400071D RID: 1821
		private bool isExiting;

		// Token: 0x0400071E RID: 1822
		private bool hasBlinked;
	}
}
