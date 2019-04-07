using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000534 RID: 1332
	[RequireComponent(typeof(HealthComponent))]
	public class AssignTeamFilterToTeamComponent : MonoBehaviour
	{
		// Token: 0x06001DD5 RID: 7637 RVA: 0x0008C20C File Offset: 0x0008A40C
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
