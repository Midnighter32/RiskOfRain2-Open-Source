using System;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000156 RID: 342
	internal class IdleTracking : BaseState
	{
		// Token: 0x0600069E RID: 1694 RVA: 0x0001F810 File Offset: 0x0001DA10
		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
			}
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x0001F83E File Offset: 0x0001DA3E
		public override void OnExit()
		{
			if (this.trackingIndicatorTransform)
			{
				EntityState.Destroy(this.trackingIndicatorTransform.gameObject);
			}
			base.OnExit();
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x0001F864 File Offset: 0x0001DA64
		private void FireOrbArrow()
		{
			if (!base.isServer)
			{
				return;
			}
			ArrowOrb arrowOrb = new ArrowOrb();
			arrowOrb.damageValue = base.characterBody.damage * IdleTracking.orbDamageCoefficient;
			arrowOrb.isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
			arrowOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
			arrowOrb.attacker = base.gameObject;
			arrowOrb.damageColorIndex = DamageColorIndex.Poison;
			arrowOrb.procChainMask.AddProc(ProcType.HealOnHit);
			arrowOrb.procCoefficient = IdleTracking.orbProcCoefficient;
			HurtBox hurtBox = this.trackingTarget;
			if (hurtBox)
			{
				Transform transform = this.childLocator.FindChild(IdleTracking.muzzleString).transform;
				EffectManager.instance.SimpleMuzzleFlash(IdleTracking.muzzleflashEffectPrefab, base.gameObject, IdleTracking.muzzleString, true);
				arrowOrb.origin = transform.position;
				arrowOrb.target = hurtBox;
				base.PlayAnimation("Gesture, Override", "FireSeekingArrow");
				base.PlayAnimation("Gesture, Additive", "FireSeekingArrow");
				Util.PlaySound(IdleTracking.attackSoundString, base.gameObject);
				OrbManager.instance.AddOrb(arrowOrb);
			}
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x0001F988 File Offset: 0x0001DB88
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.fireTimer -= Time.fixedDeltaTime;
				if (base.characterBody)
				{
					float num = 0f;
					Ray ray = CameraRigController.ModifyAimRayIfApplicable(base.GetAimRay(), base.gameObject, out num);
					BullseyeSearch bullseyeSearch = new BullseyeSearch();
					bullseyeSearch.searchOrigin = ray.origin;
					bullseyeSearch.searchDirection = ray.direction;
					bullseyeSearch.maxDistanceFilter = IdleTracking.maxTrackingDistance + num;
					bullseyeSearch.maxAngleFilter = IdleTracking.maxTrackingAngle;
					bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
					bullseyeSearch.teamMaskFilter.RemoveTeam(TeamComponent.GetObjectTeam(base.gameObject));
					bullseyeSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
					bullseyeSearch.RefreshCandidates();
					this.trackingTarget = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
				}
				if (this.trackingTarget)
				{
					if (!this.trackingIndicatorTransform)
					{
						this.trackingIndicatorTransform = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ShieldTransferIndicator"), this.trackingTarget.transform.position, Quaternion.identity).transform;
					}
					this.trackingIndicatorTransform.position = this.trackingTarget.transform.position;
					if (base.inputBank && base.inputBank.skill1.down && this.fireTimer <= 0f)
					{
						this.fireTimer = 1f / IdleTracking.fireFrequency / this.attackSpeedStat;
						this.FireOrbArrow();
						return;
					}
				}
				else if (this.trackingIndicatorTransform)
				{
					EntityState.Destroy(this.trackingIndicatorTransform.gameObject);
					this.trackingIndicatorTransform = null;
				}
			}
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x0000A1ED File Offset: 0x000083ED
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x04000812 RID: 2066
		public static float maxTrackingDistance = 20f;

		// Token: 0x04000813 RID: 2067
		public static float maxTrackingAngle = 20f;

		// Token: 0x04000814 RID: 2068
		public static float orbDamageCoefficient;

		// Token: 0x04000815 RID: 2069
		public static float orbProcCoefficient;

		// Token: 0x04000816 RID: 2070
		public static string muzzleString;

		// Token: 0x04000817 RID: 2071
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04000818 RID: 2072
		public static string attackSoundString;

		// Token: 0x04000819 RID: 2073
		public static float fireFrequency;

		// Token: 0x0400081A RID: 2074
		private float fireTimer;

		// Token: 0x0400081B RID: 2075
		private Transform trackingIndicatorTransform;

		// Token: 0x0400081C RID: 2076
		private HurtBox trackingTarget;

		// Token: 0x0400081D RID: 2077
		private ChildLocator childLocator;
	}
}
