using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200054B RID: 1355
	public class ProjectileGhostController : MonoBehaviour
	{
		// Token: 0x06001E3D RID: 7741 RVA: 0x0008E727 File Offset: 0x0008C927
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06001E3E RID: 7742 RVA: 0x0008E738 File Offset: 0x0008C938
		private void Update()
		{
			if (this.authorityTransform ^ this.predictionTransform)
			{
				this.CopyTransform(this.authorityTransform ? this.authorityTransform : this.predictionTransform);
				return;
			}
			if (this.authorityTransform)
			{
				this.LerpTransform(this.predictionTransform, this.authorityTransform, this.migration);
				if (this.migration == 1f)
				{
					this.predictionTransform = null;
					return;
				}
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06001E3F RID: 7743 RVA: 0x0008E7C5 File Offset: 0x0008C9C5
		private void LerpTransform(Transform a, Transform b, float t)
		{
			this.transform.position = Vector3.LerpUnclamped(a.position, b.position, t);
			this.transform.rotation = Quaternion.SlerpUnclamped(a.rotation, b.rotation, t);
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x0008E801 File Offset: 0x0008CA01
		private void CopyTransform(Transform src)
		{
			this.transform.position = src.position;
			this.transform.rotation = src.rotation;
		}

		// Token: 0x040020C1 RID: 8385
		private new Transform transform;

		// Token: 0x040020C2 RID: 8386
		private float migration;

		// Token: 0x040020C3 RID: 8387
		[HideInInspector]
		public Transform authorityTransform;

		// Token: 0x040020C4 RID: 8388
		[HideInInspector]
		public Transform predictionTransform;
	}
}
