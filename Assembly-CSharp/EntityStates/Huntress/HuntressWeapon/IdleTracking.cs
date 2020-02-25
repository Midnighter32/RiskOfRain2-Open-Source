using System;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000838 RID: 2104
	public class IdleTracking : BaseState
	{
		// Token: 0x06002F9F RID: 12191 RVA: 0x000CBEB8 File Offset: 0x000CA0B8
		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.childLocator = modelTransform.GetComponent<ChildLocator>();
			}
		}

		// Token: 0x06002FA0 RID: 12192 RVA: 0x000CBEE6 File Offset: 0x000CA0E6
		public override void OnExit()
		{
			if (this.trackingIndicatorTransform)
			{
				EntityState.Destroy(this.trackingIndicatorTransform.gameObject);
			}
			base.OnExit();
		}

		// Token: 0x06002FA1 RID: 12193 RVA: 0x000CBF0C File Offset: 0x000CA10C
		private void FireOrbArrow()
		{
			if (!NetworkServer.active)
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
				EffectManager.SimpleMuzzleFlash(IdleTracking.muzzleflashEffectPrefab, base.gameObject, IdleTracking.muzzleString, true);
				arrowOrb.origin = transform.position;
				arrowOrb.target = hurtBox;
				base.PlayAnimation("Gesture, Override", "FireSeekingArrow");
				base.PlayAnimation("Gesture, Additive", "FireSeekingArrow");
				Util.PlaySound(IdleTracking.attackSoundString, base.gameObject);
				OrbManager.instance.AddOrb(arrowOrb);
			}
		}

		// Token: 0x06002FA2 RID: 12194 RVA: 0x000CC024 File Offset: 0x000CA224
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

		// Token: 0x06002FA3 RID: 12195 RVA: 0x0000AC89 File Offset: 0x00008E89
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x04002D58 RID: 11608
		public static float maxTrackingDistance = 20f;

		// Token: 0x04002D59 RID: 11609
		public static float maxTrackingAngle = 20f;

		// Token: 0x04002D5A RID: 11610
		public static float orbDamageCoefficient;

		// Token: 0x04002D5B RID: 11611
		public static float orbProcCoefficient;

		// Token: 0x04002D5C RID: 11612
		public static string muzzleString;

		// Token: 0x04002D5D RID: 11613
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04002D5E RID: 11614
		public static string attackSoundString;

		// Token: 0x04002D5F RID: 11615
		public static float fireFrequency;

		// Token: 0x04002D60 RID: 11616
		private float fireTimer;

		// Token: 0x04002D61 RID: 11617
		private Transform trackingIndicatorTransform;

		// Token: 0x04002D62 RID: 11618
		private HurtBox trackingTarget;

		// Token: 0x04002D63 RID: 11619
		private ChildLocator childLocator;
	}
}
