using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000549 RID: 1353
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileFuse : MonoBehaviour
	{
		// Token: 0x06001E37 RID: 7735 RVA: 0x0008E60D File Offset: 0x0008C80D
		private void Awake()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06001E38 RID: 7736 RVA: 0x0008E61D File Offset: 0x0008C81D
		private void FixedUpdate()
		{
			this.fuse -= Time.fixedDeltaTime;
			if (this.fuse <= 0f)
			{
				base.enabled = false;
				this.onFuse.Invoke();
			}
		}

		// Token: 0x040020BB RID: 8379
		public float fuse;

		// Token: 0x040020BC RID: 8380
		public UnityEvent onFuse;
	}
}
