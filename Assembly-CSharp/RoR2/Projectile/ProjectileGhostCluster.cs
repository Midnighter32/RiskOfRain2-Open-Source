using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200054A RID: 1354
	public class ProjectileGhostCluster : MonoBehaviour
	{
		// Token: 0x06001E3A RID: 7738 RVA: 0x0008E650 File Offset: 0x0008C850
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

		// Token: 0x06001E3B RID: 7739 RVA: 0x00004507 File Offset: 0x00002707
		private void Update()
		{
		}

		// Token: 0x040020BD RID: 8381
		public GameObject ghostClusterPrefab;

		// Token: 0x040020BE RID: 8382
		public int clusterCount;

		// Token: 0x040020BF RID: 8383
		public bool distributeEvenly;

		// Token: 0x040020C0 RID: 8384
		public float clusterDistance;
	}
}
