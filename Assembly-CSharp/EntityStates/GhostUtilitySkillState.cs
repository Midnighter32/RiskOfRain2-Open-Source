using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x02000711 RID: 1809
	public class GhostUtilitySkillState : GenericCharacterMain
	{
		// Token: 0x06002A3B RID: 10811 RVA: 0x000B1A74 File Offset: 0x000AFC74
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = GhostUtilitySkillState.baseDuration;
			if (base.characterBody)
			{
				if (base.characterBody.inventory)
				{
					this.duration *= (float)base.characterBody.inventory.GetItemCount(ItemIndex.LunarUtilityReplacement);
				}
				this.hurtBoxGroup = base.characterBody.hurtBoxGroup;
				if (this.hurtBoxGroup)
				{
					HurtBoxGroup hurtBoxGroup = this.hurtBoxGroup;
					int i = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
					hurtBoxGroup.hurtBoxesDeactivatorCounter = i;
				}
				if (GhostUtilitySkillState.coreVfxPrefab)
				{
					this.coreVfxInstance = UnityEngine.Object.Instantiate<GameObject>(GhostUtilitySkillState.coreVfxPrefab);
				}
				if (GhostUtilitySkillState.footVfxPrefab)
				{
					this.footVfxInstance = UnityEngine.Object.Instantiate<GameObject>(GhostUtilitySkillState.footVfxPrefab);
				}
				this.UpdateVfxPositions();
				if (GhostUtilitySkillState.entryEffectPrefab)
				{
					Ray aimRay = base.GetAimRay();
					EffectManager.SimpleEffect(GhostUtilitySkillState.entryEffectPrefab, aimRay.origin, Quaternion.LookRotation(aimRay.direction), false);
				}
			}
			Transform modelTransform = base.GetModelTransform();
			this.characterModel = ((modelTransform != null) ? modelTransform.GetComponent<CharacterModel>() : null);
			if (base.modelAnimator)
			{
				base.modelAnimator.enabled = false;
			}
			if (base.characterMotor)
			{
				base.characterMotor.walkSpeedPenaltyCoefficient = GhostUtilitySkillState.moveSpeedCoefficient;
				base.characterMotor.useGravity = false;
				base.characterMotor.isFlying = true;
			}
			if (this.characterModel)
			{
				this.characterModel.invisibilityCount++;
			}
			foreach (EntityStateMachine entityStateMachine in base.gameObject.GetComponents<EntityStateMachine>())
			{
				if (entityStateMachine.customName == "Weapon")
				{
					entityStateMachine.SetNextStateToMain();
				}
			}
		}

		// Token: 0x06002A3C RID: 10812 RVA: 0x000B1C34 File Offset: 0x000AFE34
		private void UpdateVfxPositions()
		{
			if (base.characterBody)
			{
				if (this.coreVfxInstance)
				{
					this.coreVfxInstance.transform.position = base.characterBody.corePosition;
				}
				if (this.footVfxInstance)
				{
					this.footVfxInstance.transform.position = base.characterBody.footPosition;
				}
			}
		}

		// Token: 0x06002A3D RID: 10813 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool CanExecuteSkill(GenericSkill skillSlot)
		{
			return false;
		}

		// Token: 0x06002A3E RID: 10814 RVA: 0x000B1C9E File Offset: 0x000AFE9E
		public override void Update()
		{
			base.Update();
			this.UpdateVfxPositions();
		}

		// Token: 0x06002A3F RID: 10815 RVA: 0x000B1CAC File Offset: 0x000AFEAC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.healTimer -= Time.fixedDeltaTime;
			if (this.healTimer <= 0f)
			{
				if (NetworkServer.active)
				{
					base.healthComponent.HealFraction(GhostUtilitySkillState.healFractionPerTick, default(ProcChainMask));
				}
				this.healTimer = 1f / GhostUtilitySkillState.healFrequency;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002A40 RID: 10816 RVA: 0x000B1D34 File Offset: 0x000AFF34
		public override void OnExit()
		{
			if (GhostUtilitySkillState.exitEffectPrefab && !this.outer.destroying)
			{
				Ray aimRay = base.GetAimRay();
				EffectManager.SimpleEffect(GhostUtilitySkillState.exitEffectPrefab, aimRay.origin, Quaternion.LookRotation(aimRay.direction), false);
			}
			if (this.coreVfxInstance)
			{
				EntityState.Destroy(this.coreVfxInstance);
			}
			if (this.footVfxInstance)
			{
				EntityState.Destroy(this.footVfxInstance);
			}
			if (this.characterModel)
			{
				this.characterModel.invisibilityCount--;
			}
			if (this.hurtBoxGroup)
			{
				HurtBoxGroup hurtBoxGroup = this.hurtBoxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}
			if (base.modelAnimator)
			{
				base.modelAnimator.enabled = true;
			}
			if (base.characterMotor)
			{
				base.characterMotor.walkSpeedPenaltyCoefficient = 1f;
				base.characterMotor.useGravity = true;
				base.characterMotor.isFlying = false;
			}
			base.OnExit();
		}

		// Token: 0x04002610 RID: 9744
		public static float baseDuration;

		// Token: 0x04002611 RID: 9745
		public static GameObject coreVfxPrefab;

		// Token: 0x04002612 RID: 9746
		public static GameObject footVfxPrefab;

		// Token: 0x04002613 RID: 9747
		public static GameObject entryEffectPrefab;

		// Token: 0x04002614 RID: 9748
		public static GameObject exitEffectPrefab;

		// Token: 0x04002615 RID: 9749
		public static float moveSpeedCoefficient;

		// Token: 0x04002616 RID: 9750
		public static float healFractionPerTick;

		// Token: 0x04002617 RID: 9751
		public static float healFrequency;

		// Token: 0x04002618 RID: 9752
		private HurtBoxGroup hurtBoxGroup;

		// Token: 0x04002619 RID: 9753
		private CharacterModel characterModel;

		// Token: 0x0400261A RID: 9754
		private GameObject coreVfxInstance;

		// Token: 0x0400261B RID: 9755
		private GameObject footVfxInstance;

		// Token: 0x0400261C RID: 9756
		private float healTimer;

		// Token: 0x0400261D RID: 9757
		private float duration;
	}
}
