using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Drone.DroneWeapon
{
	// Token: 0x020008A0 RID: 2208
	public class StartHealBeam : BaseState
	{
		// Token: 0x06003186 RID: 12678 RVA: 0x000D5564 File Offset: 0x000D3764
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration / this.attackSpeedStat;
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				this.targetHurtBox = this.FindTarget(aimRay);
			}
			if (NetworkServer.active)
			{
				if (HealBeamController.GetHealBeamCountForOwner(base.gameObject) >= this.maxSimultaneousBeams)
				{
					return;
				}
				if (this.targetHurtBox)
				{
					Transform transform = base.FindModelChild(this.muzzleName);
					if (transform)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.healBeamPrefab, transform);
						HealBeamController component = gameObject.GetComponent<HealBeamController>();
						component.healRate = this.healRateCoefficient * this.damageStat * this.attackSpeedStat;
						component.target = this.targetHurtBox;
						component.ownership.ownerObject = base.gameObject;
						gameObject.AddComponent<DestroyOnTimer>().duration = this.duration;
						NetworkServer.Spawn(gameObject);
					}
				}
			}
		}

		// Token: 0x06003187 RID: 12679 RVA: 0x000D5644 File Offset: 0x000D3844
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06003188 RID: 12680 RVA: 0x000D5660 File Offset: 0x000D3860
		private HurtBox FindTarget(Ray aimRay)
		{
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.teamMaskFilter = TeamMask.none;
			if (base.teamComponent)
			{
				bullseyeSearch.teamMaskFilter.AddTeam(base.teamComponent.teamIndex);
			}
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.maxDistanceFilter = this.targetSelectionRange;
			bullseyeSearch.maxAngleFilter = 180f;
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
			bullseyeSearch.RefreshCandidates();
			bullseyeSearch.FilterOutGameObject(base.gameObject);
			return bullseyeSearch.GetResults().Where(new Func<HurtBox, bool>(this.NotAlreadyHealingTarget)).Where(new Func<HurtBox, bool>(StartHealBeam.IsHurt)).FirstOrDefault<HurtBox>();
		}

		// Token: 0x06003189 RID: 12681 RVA: 0x000D571F File Offset: 0x000D391F
		private bool NotAlreadyHealingTarget(HurtBox hurtBox)
		{
			return !HealBeamController.HealBeamAlreadyExists(base.gameObject, hurtBox);
		}

		// Token: 0x0600318A RID: 12682 RVA: 0x00090218 File Offset: 0x0008E418
		private static bool IsHurt(HurtBox hurtBox)
		{
			return hurtBox.healthComponent.alive && hurtBox.healthComponent.health < hurtBox.healthComponent.fullHealth;
		}

		// Token: 0x0600318B RID: 12683 RVA: 0x000D5730 File Offset: 0x000D3930
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(HurtBoxReference.FromHurtBox(this.targetHurtBox));
		}

		// Token: 0x0600318C RID: 12684 RVA: 0x000D574C File Offset: 0x000D394C
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.targetHurtBox = reader.ReadHurtBoxReference().ResolveHurtBox();
		}

		// Token: 0x04002FFE RID: 12286
		[SerializeField]
		public float baseDuration;

		// Token: 0x04002FFF RID: 12287
		[SerializeField]
		public float targetSelectionRange;

		// Token: 0x04003000 RID: 12288
		[SerializeField]
		public float healRateCoefficient;

		// Token: 0x04003001 RID: 12289
		[SerializeField]
		public GameObject healBeamPrefab;

		// Token: 0x04003002 RID: 12290
		[SerializeField]
		public string muzzleName;

		// Token: 0x04003003 RID: 12291
		[SerializeField]
		public int maxSimultaneousBeams;

		// Token: 0x04003004 RID: 12292
		private HurtBox targetHurtBox;

		// Token: 0x04003005 RID: 12293
		private float duration;
	}
}
