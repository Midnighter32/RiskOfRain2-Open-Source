using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000507 RID: 1287
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileFuse : MonoBehaviour
	{
		// Token: 0x06001E8A RID: 7818 RVA: 0x00083E45 File Offset: 0x00082045
		private void Awake()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06001E8B RID: 7819 RVA: 0x00083E55 File Offset: 0x00082055
		private void FixedUpdate()
		{
			this.fuse -= Time.fixedDeltaTime;
			if (this.fuse <= 0f)
			{
				base.enabled = false;
				this.onFuse.Invoke();
			}
		}

		// Token: 0x04001BEF RID: 7151
		public float fuse;

		// Token: 0x04001BF0 RID: 7152
		public UnityEvent onFuse;
	}
}
