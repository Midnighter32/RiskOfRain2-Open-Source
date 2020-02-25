using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000883 RID: 2179
	public class EngiSelfShield : BaseState
	{
		// Token: 0x06003107 RID: 12551 RVA: 0x000D2D2C File Offset: 0x000D0F2C
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

		// Token: 0x06003108 RID: 12552 RVA: 0x000D2E1C File Offset: 0x000D101C
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

		// Token: 0x06003109 RID: 12553 RVA: 0x000D2E84 File Offset: 0x000D1084
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= EngiSelfShield.transferDelay && base.skillLocator.utility.IsReady())
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

		// Token: 0x0600310A RID: 12554 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002F3B RID: 12091
		public static float transferDelay = 0.1f;

		// Token: 0x04002F3C RID: 12092
		private HurtBox transferTarget;

		// Token: 0x04002F3D RID: 12093
		private BullseyeSearch friendLocator;

		// Token: 0x04002F3E RID: 12094
		private Indicator indicator;
	}
}
