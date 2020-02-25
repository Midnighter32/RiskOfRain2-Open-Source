using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x0200089F RID: 2207
	public class HealBeam : BaseState
	{
		// Token: 0x0600317E RID: 12670 RVA: 0x000D52FC File Offset: 0x000D34FC
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Gesture", "Heal", 0.2f);
			this.duration = HealBeam.baseDuration / this.attackSpeedStat;
			float healRate = HealBeam.healCoefficient * this.damageStat / this.duration;
			Ray aimRay = base.GetAimRay();
			Transform transform = base.FindModelChild("Muzzle");
			if (NetworkServer.active)
			{
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.teamMaskFilter = TeamMask.none;
				if (base.teamComponent)
				{
					bullseyeSearch.teamMaskFilter.AddTeam(base.teamComponent.teamIndex);
				}
				bullseyeSearch.filterByLoS = false;
				bullseyeSearch.maxDistanceFilter = 50f;
				bullseyeSearch.maxAngleFilter = 180f;
				bullseyeSearch.searchOrigin = aimRay.origin;
				bullseyeSearch.searchDirection = aimRay.direction;
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
				bullseyeSearch.RefreshCandidates();
				bullseyeSearch.FilterOutGameObject(base.gameObject);
				this.target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
				if (transform && this.target)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(HealBeam.healBeamPrefab, transform);
					this.healBeamController = gameObject.GetComponent<HealBeamController>();
					this.healBeamController.healRate = healRate;
					this.healBeamController.target = this.target;
					this.healBeamController.ownership.ownerObject = base.gameObject;
					NetworkServer.Spawn(gameObject);
				}
			}
		}

		// Token: 0x0600317F RID: 12671 RVA: 0x000D5465 File Offset: 0x000D3665
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if ((base.fixedAge >= this.duration || !this.target) && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003180 RID: 12672 RVA: 0x000D549C File Offset: 0x000D369C
		public override void OnExit()
		{
			base.PlayCrossfade("Gesture", "Empty", 0.2f);
			if (this.healBeamController)
			{
				this.healBeamController.BreakServer();
			}
			base.OnExit();
		}

		// Token: 0x06003181 RID: 12673 RVA: 0x0000AC89 File Offset: 0x00008E89
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		// Token: 0x06003182 RID: 12674 RVA: 0x000D54D4 File Offset: 0x000D36D4
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			HurtBoxReference.FromHurtBox(this.target).Write(writer);
		}

		// Token: 0x06003183 RID: 12675 RVA: 0x000D54FC File Offset: 0x000D36FC
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			HurtBoxReference hurtBoxReference = default(HurtBoxReference);
			hurtBoxReference.Read(reader);
			GameObject gameObject = hurtBoxReference.ResolveGameObject();
			this.target = ((gameObject != null) ? gameObject.GetComponent<HurtBox>() : null);
		}

		// Token: 0x04002FF5 RID: 12277
		public static float baseDuration;

		// Token: 0x04002FF6 RID: 12278
		public static float healCoefficient = 5f;

		// Token: 0x04002FF7 RID: 12279
		public static GameObject healBeamPrefab;

		// Token: 0x04002FF8 RID: 12280
		public HurtBox target;

		// Token: 0x04002FF9 RID: 12281
		private HealBeamController healBeamController;

		// Token: 0x04002FFA RID: 12282
		private float duration;

		// Token: 0x04002FFB RID: 12283
		private float lineWidthRefVelocity;

		// Token: 0x04002FFC RID: 12284
		private float maxLineWidth = 0.3f;

		// Token: 0x04002FFD RID: 12285
		private float smoothTime = 0.1f;
	}
}
