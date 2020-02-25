using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001D4 RID: 468
	public class DebuffZone : MonoBehaviour
	{
		// Token: 0x06000A06 RID: 2566 RVA: 0x0000409B File Offset: 0x0000229B
		private void Awake()
		{
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x0002BEA8 File Offset: 0x0002A0A8
		private void OnTriggerEnter(Collider other)
		{
			if (NetworkServer.active)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					component.AddTimedBuff(this.buffType, this.buffDuration);
					Util.PlaySound(this.buffApplicationSoundString, component.gameObject);
					if (this.buffApplicationEffectPrefab)
					{
						EffectManager.SpawnEffect(this.buffApplicationEffectPrefab, new EffectData
						{
							origin = component.mainHurtBox.transform.position,
							scale = component.radius
						}, true);
					}
				}
			}
		}

		// Token: 0x04000A3F RID: 2623
		[Tooltip("The buff type to grant")]
		public BuffIndex buffType;

		// Token: 0x04000A40 RID: 2624
		[Tooltip("The buff duration")]
		public float buffDuration;

		// Token: 0x04000A41 RID: 2625
		public string buffApplicationSoundString;

		// Token: 0x04000A42 RID: 2626
		public GameObject buffApplicationEffectPrefab;
	}
}
