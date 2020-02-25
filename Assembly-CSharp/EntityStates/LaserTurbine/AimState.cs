using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.LaserTurbine
{
	// Token: 0x020007FB RID: 2043
	public class AimState : LaserTurbineBaseState
	{
		// Token: 0x06002E79 RID: 11897 RVA: 0x000C571C File Offset: 0x000C391C
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				TeamMask mask = TeamMask.AllExcept(base.ownerBody.teamComponent.teamIndex);
				HurtBox[] hurtBoxes = new SphereSearch
				{
					radius = AimState.targetAcquisitionRadius,
					mask = LayerIndex.entityPrecise.mask,
					origin = base.transform.position,
					queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
				}.RefreshCandidates().FilterCandidatesByHurtBoxTeam(mask).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();
				float blastRadius = FireMainBeamState.secondBombPrefab.GetComponent<ProjectileImpactExplosion>().blastRadius;
				int num = -1;
				int num2 = 0;
				for (int i = 0; i < hurtBoxes.Length; i++)
				{
					HurtBox[] hurtBoxes2 = new SphereSearch
					{
						radius = blastRadius,
						mask = LayerIndex.entityPrecise.mask,
						origin = hurtBoxes[i].transform.position,
						queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
					}.RefreshCandidates().FilterCandidatesByHurtBoxTeam(mask).FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();
					if (hurtBoxes2.Length > num2)
					{
						num = i;
						num2 = hurtBoxes2.Length;
					}
				}
				if (num != -1)
				{
					base.simpleRotateToDirection.targetRotation = Quaternion.LookRotation(hurtBoxes[num].transform.position - base.transform.position);
					this.foundTarget = true;
				}
			}
		}

		// Token: 0x06002E7A RID: 11898 RVA: 0x000C5869 File Offset: 0x000C3A69
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				if (this.foundTarget)
				{
					this.outer.SetNextState(new ChargeMainBeamState());
					return;
				}
				this.outer.SetNextState(new ReadyState());
			}
		}

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06002E7B RID: 11899 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldFollow
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002B8E RID: 11150
		public static float targetAcquisitionRadius;

		// Token: 0x04002B8F RID: 11151
		private bool foundTarget;
	}
}
