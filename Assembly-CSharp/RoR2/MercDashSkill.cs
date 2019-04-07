using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000357 RID: 855
	public class MercDashSkill : GenericSkill
	{
		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06001198 RID: 4504 RVA: 0x000572D6 File Offset: 0x000554D6
		// (set) Token: 0x06001199 RID: 4505 RVA: 0x000572DE File Offset: 0x000554DE
		public int currentDashIndex { get; private set; }

		// Token: 0x0600119A RID: 4506 RVA: 0x000572E8 File Offset: 0x000554E8
		public void AddHit()
		{
			int num = this.currentDashIndex + 1;
			this.currentDashIndex = num;
			if (this.currentDashIndex < this.maxDashes)
			{
				num = base.stock + 1;
				base.stock = num;
				return;
			}
			this.currentDashIndex = 0;
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x0005732C File Offset: 0x0005552C
		protected new void FixedUpdate()
		{
			base.FixedUpdate();
			this.timeoutTimer -= Time.fixedDeltaTime;
			if (this.timeoutTimer <= 0f && this.currentDashIndex != 0)
			{
				base.stock = 0;
				this.currentDashIndex = 0;
			}
			int num = this.currentDashIndex;
			if (num >= this.icons.Length)
			{
				num = this.icons.Length - 1;
			}
			this.icon = this.icons[num];
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x0005739F File Offset: 0x0005559F
		protected override void OnExecute()
		{
			base.OnExecute();
			this.timeoutTimer = this.timeoutDuration;
		}

		// Token: 0x040015A3 RID: 5539
		public int maxDashes;

		// Token: 0x040015A4 RID: 5540
		private float timeoutTimer;

		// Token: 0x040015A5 RID: 5541
		public float timeoutDuration;

		// Token: 0x040015A6 RID: 5542
		public Sprite[] icons;
	}
}
