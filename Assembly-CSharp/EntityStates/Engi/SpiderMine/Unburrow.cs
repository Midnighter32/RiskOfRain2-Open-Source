using System;
using System.Linq;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.SpiderMine
{
	// Token: 0x0200086E RID: 2158
	public class Unburrow : BaseSpiderMineState
	{
		// Token: 0x060030A9 RID: 12457 RVA: 0x000D1986 File Offset: 0x000CFB86
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Unburrow.baseDuration;
			Util.PlaySound("Play_beetle_worker_idle", base.gameObject);
			base.PlayAnimation("Base", "ArmedToChase", "ArmedToChase.playbackRate", this.duration);
		}

		// Token: 0x060030AA RID: 12458 RVA: 0x000D19C8 File Offset: 0x000CFBC8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				EntityState entityState = null;
				if (!base.projectileStickOnImpact.stuck)
				{
					entityState = new WaitForStick();
				}
				else if (base.projectileTargetComponent.target)
				{
					if (this.duration <= base.fixedAge)
					{
						this.FindBetterTarget(base.projectileTargetComponent.target);
						entityState = new ChaseTarget();
					}
				}
				else
				{
					entityState = new Burrow();
				}
				if (entityState != null)
				{
					this.outer.SetNextState(entityState);
				}
			}
		}

		// Token: 0x060030AB RID: 12459 RVA: 0x000D1A48 File Offset: 0x000CFC48
		private BullseyeSearch CreateBullseyeSearch(Vector3 origin)
		{
			return new BullseyeSearch
			{
				searchOrigin = origin,
				filterByDistinctEntity = true,
				maxDistanceFilter = Detonate.blastRadius,
				sortMode = BullseyeSearch.SortMode.Distance,
				maxAngleFilter = 360f,
				teamMaskFilter = TeamMask.AllExcept(base.projectileController.teamFilter.teamIndex)
			};
		}

		// Token: 0x060030AC RID: 12460 RVA: 0x000D1AA0 File Offset: 0x000CFCA0
		private void FindBetterTarget(Transform initialTarget)
		{
			BullseyeSearch bullseyeSearch = this.CreateBullseyeSearch(initialTarget.position);
			bullseyeSearch.RefreshCandidates();
			HurtBox[] array = bullseyeSearch.GetResults().ToArray<HurtBox>();
			int num = array.Length;
			int num2 = -1;
			int i = 0;
			int num3 = Math.Min(array.Length, Unburrow.betterTargetSearchLimit);
			while (i < num3)
			{
				HurtBox hurtBox = array[i];
				int num4 = this.CountTargets(hurtBox.transform.position);
				if (num < num4)
				{
					num = num4;
					num2 = i;
				}
				i++;
			}
			if (num2 != -1)
			{
				base.projectileTargetComponent.target = array[num2].transform;
			}
		}

		// Token: 0x060030AD RID: 12461 RVA: 0x000D1B26 File Offset: 0x000CFD26
		private int CountTargets(Vector3 origin)
		{
			BullseyeSearch bullseyeSearch = this.CreateBullseyeSearch(origin);
			bullseyeSearch.RefreshCandidates();
			return bullseyeSearch.GetResults().Count<HurtBox>();
		}

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x060030AE RID: 12462 RVA: 0x0000B933 File Offset: 0x00009B33
		protected override bool shouldStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04002EFA RID: 12026
		public static float baseDuration;

		// Token: 0x04002EFB RID: 12027
		public static int betterTargetSearchLimit;

		// Token: 0x04002EFC RID: 12028
		private float duration;
	}
}
