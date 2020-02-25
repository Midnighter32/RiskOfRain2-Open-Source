using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200033E RID: 830
	public class SprintEffectController : MonoBehaviour
	{
		// Token: 0x060013C2 RID: 5058 RVA: 0x000547F1 File Offset: 0x000529F1
		private void Awake()
		{
			if (this.characterBody && Util.IsPrefab(this.characterBody.gameObject) && !Util.IsPrefab(base.gameObject))
			{
				this.characterBody = null;
			}
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x00054828 File Offset: 0x00052A28
		private void FixedUpdate()
		{
			if (this.characterBody)
			{
				if (this.characterBody.healthComponent.alive && this.characterBody.isSprinting && (!this.mustBeGrounded || this.characterBody.characterMotor.isGrounded))
				{
					GameObject gameObject = this.loopRootObject;
					if (gameObject != null)
					{
						gameObject.SetActive(true);
					}
					for (int i = 0; i < this.loopSystems.Length; i++)
					{
						ParticleSystem particleSystem = this.loopSystems[i];
						particleSystem.main.loop = true;
						if (!particleSystem.isPlaying)
						{
							particleSystem.Play();
						}
					}
					return;
				}
				GameObject gameObject2 = this.loopRootObject;
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
				for (int j = 0; j < this.loopSystems.Length; j++)
				{
					this.loopSystems[j].main.loop = false;
				}
			}
		}

		// Token: 0x0400128F RID: 4751
		public ParticleSystem[] loopSystems;

		// Token: 0x04001290 RID: 4752
		public GameObject loopRootObject;

		// Token: 0x04001291 RID: 4753
		public bool mustBeGrounded;

		// Token: 0x04001292 RID: 4754
		public CharacterBody characterBody;
	}
}
