using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001D3 RID: 467
	internal class DeathZone : MonoBehaviour
	{
		// Token: 0x06000A04 RID: 2564 RVA: 0x0002BE48 File Offset: 0x0002A048
		public void OnTriggerEnter(Collider other)
		{
			if (NetworkServer.active)
			{
				HealthComponent component = other.GetComponent<HealthComponent>();
				if (component)
				{
					component.TakeDamage(new DamageInfo
					{
						position = other.transform.position,
						attacker = null,
						inflictor = base.gameObject,
						damage = 999999f
					});
				}
			}
		}
	}
}
