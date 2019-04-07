using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002C5 RID: 709
	public class DebuffZone : MonoBehaviour
	{
		// Token: 0x06000E63 RID: 3683 RVA: 0x00004507 File Offset: 0x00002707
		private void Awake()
		{
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x000470DC File Offset: 0x000452DC
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
						EffectManager.instance.SpawnEffect(this.buffApplicationEffectPrefab, new EffectData
						{
							origin = component.mainHurtBox.transform.position,
							scale = component.radius
						}, true);
					}
				}
			}
		}

		// Token: 0x04001255 RID: 4693
		[Tooltip("The buff type to grant")]
		public BuffIndex buffType;

		// Token: 0x04001256 RID: 4694
		[Tooltip("The buff duration")]
		public float buffDuration;

		// Token: 0x04001257 RID: 4695
		public string buffApplicationSoundString;

		// Token: 0x04001258 RID: 4696
		public GameObject buffApplicationEffectPrefab;
	}
}
