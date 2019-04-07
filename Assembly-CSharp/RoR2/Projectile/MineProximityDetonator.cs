using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200053D RID: 1341
	public class MineProximityDetonator : MonoBehaviour
	{
		// Token: 0x06001DF8 RID: 7672 RVA: 0x0008D2B0 File Offset: 0x0008B4B0
		public void OnTriggerEnter(Collider collider)
		{
			if (NetworkServer.active)
			{
				if (collider)
				{
					HurtBox component = collider.GetComponent<HurtBox>();
					if (component)
					{
						TeamComponent component2 = component.healthComponent.GetComponent<TeamComponent>();
						if (component2 && component2.teamIndex == this.myTeamFilter.teamIndex)
						{
							return;
						}
						UnityEvent unityEvent = this.triggerEvents;
						if (unityEvent == null)
						{
							return;
						}
						unityEvent.Invoke();
					}
				}
				return;
			}
		}

		// Token: 0x04002056 RID: 8278
		public TeamFilter myTeamFilter;

		// Token: 0x04002057 RID: 8279
		public UnityEvent triggerEvents;
	}
}
