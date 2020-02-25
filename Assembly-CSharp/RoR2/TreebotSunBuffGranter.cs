using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200035C RID: 860
	public class TreebotSunBuffGranter : MonoBehaviour
	{
		// Token: 0x060014E7 RID: 5351 RVA: 0x000594F4 File Offset: 0x000576F4
		private void Start()
		{
			this.FindSun();
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x00059508 File Offset: 0x00057708
		private void FixedUpdate()
		{
			this.timer -= Time.fixedDeltaTime;
			if (this.timer <= 0f)
			{
				this.timer = 1f / this.raycastFrequency;
				this.CheckSunlight();
			}
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x00059541 File Offset: 0x00057741
		private void FindSun()
		{
			this.sun = RenderSettings.sun;
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x00059550 File Offset: 0x00057750
		private void CheckSunlight()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (!this.sun)
			{
				this.FindSun();
				if (!this.sun)
				{
					return;
				}
			}
			bool flag = !Physics.Raycast(base.transform.position, -this.sun.transform.forward, 1000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
			if (this.hadSunlight && !flag)
			{
				this.characterBody.RemoveBuff(this.buffIndex);
			}
			else if (flag && !this.hadSunlight)
			{
				this.characterBody.AddBuff(this.buffIndex);
			}
			this.hadSunlight = flag;
		}

		// Token: 0x04001384 RID: 4996
		public Transform referenceTransform;

		// Token: 0x04001385 RID: 4997
		public float raycastFrequency;

		// Token: 0x04001386 RID: 4998
		public BuffIndex buffIndex;

		// Token: 0x04001387 RID: 4999
		private const float raycastLength = 1000f;

		// Token: 0x04001388 RID: 5000
		private float timer;

		// Token: 0x04001389 RID: 5001
		private bool hadSunlight;

		// Token: 0x0400138A RID: 5002
		private Light sun;

		// Token: 0x0400138B RID: 5003
		private CharacterBody characterBody;
	}
}
