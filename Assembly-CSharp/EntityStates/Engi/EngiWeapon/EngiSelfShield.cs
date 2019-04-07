using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000184 RID: 388
	internal class EngiSelfShield : BaseState
	{
		// Token: 0x06000777 RID: 1911 RVA: 0x00024AC4 File Offset: 0x00022CC4
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active && base.characterBody)
			{
				base.characterBody.AddBuff(BuffIndex.EngiShield);
				base.characterBody.RecalculateStats();
				if (base.healthComponent)
				{
					base.healthComponent.RechargeShieldFull();
				}
			}
			this.friendLocator = new BullseyeSearch();
			this.friendLocator.teamMaskFilter = TeamMask.none;
			if (base.teamComponent)
			{
				this.friendLocator.teamMaskFilter.AddTeam(base.teamComponent.teamIndex);
			}
			this.friendLocator.maxDistanceFilter = 80f;
			this.friendLocator.maxAngleFilter = 20f;
			this.friendLocator.sortMode = BullseyeSearch.SortMode.Angle;
			this.friendLocator.filterByLoS = false;
			this.indicator = new Indicator(base.gameObject, Resources.Load<GameObject>("Prefabs/ShieldTransferIndicator"));
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x00024BB4 File Offset: 0x00022DB4
		public override void OnExit()
		{
			base.skillLocator.utility = base.skillLocator.FindSkill("RetractShield");
			if (NetworkServer.active)
			{
				base.characterBody.RemoveBuff(BuffIndex.EngiShield);
			}
			if (base.isAuthority)
			{
				base.skillLocator.utility.RemoveAllStocks();
			}
			this.indicator.active = false;
			base.OnExit();
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x00024C1C File Offset: 0x00022E1C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= EngiSelfShield.transferDelay && base.skillLocator.utility.CanExecute())
			{
				if (base.characterBody)
				{
					float num = 0f;
					Ray ray = CameraRigController.ModifyAimRayIfApplicable(base.GetAimRay(), base.gameObject, out num);
					this.friendLocator.searchOrigin = ray.origin;
					this.friendLocator.searchDirection = ray.direction;
					this.friendLocator.maxDistanceFilter += num;
					this.friendLocator.RefreshCandidates();
					this.friendLocator.FilterOutGameObject(base.gameObject);
					this.transferTarget = this.friendLocator.GetResults().FirstOrDefault<HurtBox>();
				}
				HealthComponent healthComponent = this.transferTarget ? this.transferTarget.healthComponent : null;
				if (healthComponent)
				{
					this.indicator.targetTransform = Util.GetCoreTransform(healthComponent.gameObject);
					if (base.inputBank && base.inputBank.skill3.justPressed)
					{
						EngiOtherShield engiOtherShield = new EngiOtherShield();
						engiOtherShield.target = healthComponent.gameObject.GetComponent<CharacterBody>();
						this.outer.SetNextState(engiOtherShield);
						return;
					}
				}
				else
				{
					this.indicator.targetTransform = null;
				}
				this.indicator.active = this.indicator.targetTransform;
			}
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400098B RID: 2443
		public static float transferDelay = 0.1f;

		// Token: 0x0400098C RID: 2444
		private HurtBox transferTarget;

		// Token: 0x0400098D RID: 2445
		private BullseyeSearch friendLocator;

		// Token: 0x0400098E RID: 2446
		private Indicator indicator;
	}
}
