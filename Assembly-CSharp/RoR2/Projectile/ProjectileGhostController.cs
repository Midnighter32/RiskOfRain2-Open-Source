using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000509 RID: 1289
	public class ProjectileGhostController : MonoBehaviour
	{
		// Token: 0x06001E90 RID: 7824 RVA: 0x00083F5F File Offset: 0x0008215F
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06001E91 RID: 7825 RVA: 0x00083F70 File Offset: 0x00082170
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

		// Token: 0x06001E92 RID: 7826 RVA: 0x00083FFD File Offset: 0x000821FD
		private void LerpTransform(Transform a, Transform b, float t)
		{
			this.transform.position = Vector3.LerpUnclamped(a.position, b.position, t);
			this.transform.rotation = Quaternion.SlerpUnclamped(a.rotation, b.rotation, t);
		}

		// Token: 0x06001E93 RID: 7827 RVA: 0x00084039 File Offset: 0x00082239
		private void CopyTransform(Transform src)
		{
			this.transform.position = src.position;
			this.transform.rotation = src.rotation;
		}

		// Token: 0x04001BF5 RID: 7157
		private new Transform transform;

		// Token: 0x04001BF6 RID: 7158
		private float migration;

		// Token: 0x04001BF7 RID: 7159
		[HideInInspector]
		public Transform authorityTransform;

		// Token: 0x04001BF8 RID: 7160
		[HideInInspector]
		public Transform predictionTransform;
	}
}
