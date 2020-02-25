using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000508 RID: 1288
	public class ProjectileGhostCluster : MonoBehaviour
	{
		// Token: 0x06001E8D RID: 7821 RVA: 0x00083E88 File Offset: 0x00082088
		private void Start()
		{
			float num = 1f / (Mathf.Log((float)this.clusterCount, 4f) + 1f);
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.clusterCount; i++)
			{
				Vector3 b;
				if (this.distributeEvenly)
				{
					b = Vector3.zero;
				}
				else
				{
					b = UnityEngine.Random.insideUnitSphere * this.clusterDistance;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ghostClusterPrefab, position + b, Quaternion.identity, base.transform);
				gameObject.transform.localScale = Vector3.one / (Mathf.Log((float)this.clusterCount, 4f) + 1f);
				TrailRenderer component = gameObject.GetComponent<TrailRenderer>();
				if (component)
				{
					component.widthMultiplier *= num;
				}
			}
		}

		// Token: 0x06001E8E RID: 7822 RVA: 0x0000409B File Offset: 0x0000229B
		private void Update()
		{
		}

		// Token: 0x04001BF1 RID: 7153
		public GameObject ghostClusterPrefab;

		// Token: 0x04001BF2 RID: 7154
		public int clusterCount;

		// Token: 0x04001BF3 RID: 7155
		public bool distributeEvenly;

		// Token: 0x04001BF4 RID: 7156
		public float clusterDistance;
	}
}
