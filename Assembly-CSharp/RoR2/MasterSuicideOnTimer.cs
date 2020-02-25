using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200027F RID: 639
	public class MasterSuicideOnTimer : MonoBehaviour
	{
		// Token: 0x06000E2D RID: 3629 RVA: 0x0003F347 File Offset: 0x0003D547
		private void Start()
		{
			this.characterMaster = base.GetComponent<CharacterMaster>();
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x0003F358 File Offset: 0x0003D558
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
							component.Suicide(null, null, DamageType.Generic);
							this.hasDied = true;
						}
					}
				}
			}
		}

		// Token: 0x04000E19 RID: 3609
		public float lifeTimer;

		// Token: 0x04000E1A RID: 3610
		private float timer;

		// Token: 0x04000E1B RID: 3611
		private bool hasDied;

		// Token: 0x04000E1C RID: 3612
		private CharacterMaster characterMaster;
	}
}
