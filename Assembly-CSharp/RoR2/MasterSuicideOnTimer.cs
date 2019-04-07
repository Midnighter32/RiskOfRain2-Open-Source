using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000355 RID: 853
	public class MasterSuicideOnTimer : MonoBehaviour
	{
		// Token: 0x06001192 RID: 4498 RVA: 0x0005720F File Offset: 0x0005540F
		private void Start()
		{
			this.characterMaster = base.GetComponent<CharacterMaster>();
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x00057220 File Offset: 0x00055420
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.timer += Time.fixedDeltaTime;
				if (this.timer >= this.lifeTimer && !this.hasDied)
				{
					GameObject bodyObject = this.characterMaster.GetBodyObject();
					if (bodyObject)
					{
						HealthComponent component = bodyObject.GetComponent<HealthComponent>();
						if (component)
						{
							component.Suicide(null);
							this.hasDied = true;
						}
					}
				}
			}
		}

		// Token: 0x0400159C RID: 5532
		public float lifeTimer;

		// Token: 0x0400159D RID: 5533
		private float timer;

		// Token: 0x0400159E RID: 5534
		private bool hasDied;

		// Token: 0x0400159F RID: 5535
		private CharacterMaster characterMaster;
	}
}
