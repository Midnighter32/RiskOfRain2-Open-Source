using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x020004F8 RID: 1272
	public class MineProximityDetonator : MonoBehaviour
	{
		// Token: 0x06001E3D RID: 7741 RVA: 0x0008274C File Offset: 0x0008094C
		public void OnTriggerEnter(Collider collider)
		{
			if (NetworkServer.active)
			{
				if (collider)
				{
					HurtBox component = collider.GetComponent<HurtBox>();
					if (component)
					{
						HealthComponent healthComponent = component.healthComponent;
						if (healthComponent)
						{
							TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
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
				}
				return;
			}
		}

		// Token: 0x04001B73 RID: 7027
		public TeamFilter myTeamFilter;

		// Token: 0x04001B74 RID: 7028
		public UnityEvent triggerEvents;
	}
}
