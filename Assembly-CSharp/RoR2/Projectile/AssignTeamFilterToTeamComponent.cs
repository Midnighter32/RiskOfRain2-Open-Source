using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x020004EE RID: 1262
	[RequireComponent(typeof(HealthComponent))]
	public class AssignTeamFilterToTeamComponent : MonoBehaviour
	{
		// Token: 0x06001E0A RID: 7690 RVA: 0x000814FC File Offset: 0x0007F6FC
		private void Start()
		{
			if (NetworkServer.active)
			{
				TeamComponent component = base.GetComponent<TeamComponent>();
				TeamFilter component2 = base.GetComponent<TeamFilter>();
				if (component2 && component)
				{
					component.teamIndex = component2.teamIndex;
				}
			}
		}
	}
}
