using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000519 RID: 1305
	public class Orb
	{
		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06001D54 RID: 7508 RVA: 0x00088DFC File Offset: 0x00086FFC
		// (set) Token: 0x06001D55 RID: 7509 RVA: 0x00088E04 File Offset: 0x00087004
		public float duration { get; protected set; }

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06001D56 RID: 7510 RVA: 0x00088E0D File Offset: 0x0008700D
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

		// Token: 0x06001D57 RID: 7511 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void Begin()
		{
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnArrival()
		{
		}

		// Token: 0x04001FA0 RID: 8096
		public Vector3 origin;

		// Token: 0x04001FA1 RID: 8097
		public HurtBox target;

		// Token: 0x04001FA3 RID: 8099
		public float arrivalTime;

		// Token: 0x04001FA4 RID: 8100
		public Orb nextOrb;
	}
}
