using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002C4 RID: 708
	internal class DeathZone : MonoBehaviour
	{
		// Token: 0x06000E61 RID: 3681 RVA: 0x0004707C File Offset: 0x0004527C
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
