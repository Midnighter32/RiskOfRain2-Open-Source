using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000185 RID: 389
	internal class EngiTeamShield : BaseState
	{
		// Token: 0x0600077D RID: 1917 RVA: 0x00024DA4 File Offset: 0x00022FA4
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

		// Token: 0x0600077E RID: 1918 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x0000F633 File Offset: 0x0000D833
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400098F RID: 2447
		public static float duration = 3f;

		// Token: 0x04000990 RID: 2448
		public static float radius;
	}
}
