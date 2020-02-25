using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000884 RID: 2180
	public class EngiTeamShield : BaseState
	{
		// Token: 0x0600310D RID: 12557 RVA: 0x000D300C File Offset: 0x000D120C
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.teamComponent && NetworkServer.active)
			{
				ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(base.teamComponent.teamIndex);
				float num = EngiTeamShield.radius * EngiTeamShield.radius;
				Vector3 position = base.transform.position;
				for (int i = 0; i < teamMembers.Count; i++)
				{
					if ((teamMembers[i].transform.position - position).sqrMagnitude <= num)
					{
						CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
						if (component)
						{
							component.AddTimedBuff(BuffIndex.EngiTeamShield, EngiTeamShield.duration);
							HealthComponent component2 = component.GetComponent<HealthComponent>();
							if (component2)
							{
								component2.RechargeShieldFull();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600310E RID: 12558 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600310F RID: 12559 RVA: 0x000B23CF File Offset: 0x000B05CF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		// Token: 0x06003110 RID: 12560 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002F3F RID: 12095
		public static float duration = 3f;

		// Token: 0x04002F40 RID: 12096
		public static float radius;
	}
}
