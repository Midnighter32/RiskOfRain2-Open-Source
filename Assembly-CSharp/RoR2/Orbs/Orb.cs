using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004D3 RID: 1235
	public class Orb
	{
		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06001D85 RID: 7557 RVA: 0x0007E13C File Offset: 0x0007C33C
		// (set) Token: 0x06001D86 RID: 7558 RVA: 0x0007E144 File Offset: 0x0007C344
		public float duration { get; protected set; }

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001D87 RID: 7559 RVA: 0x0007E14D File Offset: 0x0007C34D
		protected float distanceToTarget
		{
			get
			{
				if (this.target)
				{
					return Vector3.Distance(this.target.transform.position, this.origin);
				}
				return 0f;
			}
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void Begin()
		{
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnArrival()
		{
		}

		// Token: 0x04001AB8 RID: 6840
		public Vector3 origin;

		// Token: 0x04001AB9 RID: 6841
		public HurtBox target;

		// Token: 0x04001ABB RID: 6843
		public float arrivalTime;

		// Token: 0x04001ABC RID: 6844
		public Orb nextOrb;
	}
}
