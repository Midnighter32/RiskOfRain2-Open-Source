using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000162 RID: 354
	public class BoneParticleController : MonoBehaviour
	{
		// Token: 0x0600067E RID: 1662 RVA: 0x0001AA14 File Offset: 0x00018C14
		private void Start()
		{
			this.bonesList = new List<Transform>();
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name.IndexOf("IK", StringComparison.OrdinalIgnoreCase) == -1 && transform.name.IndexOf("Root", StringComparison.OrdinalIgnoreCase) == -1 && transform.name.IndexOf("Base", StringComparison.OrdinalIgnoreCase) == -1)
				{
					Debug.LogFormat("added bone {0}", new object[]
					{
						transform
					});
					this.bonesList.Add(transform);
				}
			}
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0001AAA8 File Offset: 0x00018CA8
		private void Update()
		{
			if (this.skinnedMeshRenderer)
			{
				this.stopwatch += Time.deltaTime;
				if (this.stopwatch > 1f / this.spawnFrequency)
				{
					this.stopwatch -= 1f / this.spawnFrequency;
					int count = this.bonesList.Count;
					Transform transform = this.bonesList[UnityEngine.Random.Range(0, count)];
					if (transform)
					{
						UnityEngine.Object.Instantiate<GameObject>(this.childParticlePrefab, transform.transform.position, Quaternion.identity, transform);
					}
				}
			}
		}

		// Token: 0x040006D6 RID: 1750
		public GameObject childParticlePrefab;

		// Token: 0x040006D7 RID: 1751
		public float spawnFrequency;

		// Token: 0x040006D8 RID: 1752
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x040006D9 RID: 1753
		private float stopwatch;

		// Token: 0x040006DA RID: 1754
		private List<Transform> bonesList;
	}
}
