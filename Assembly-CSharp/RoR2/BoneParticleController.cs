using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026C RID: 620
	public class BoneParticleController : MonoBehaviour
	{
		// Token: 0x06000BA0 RID: 2976 RVA: 0x00038D14 File Offset: 0x00036F14
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

		// Token: 0x06000BA1 RID: 2977 RVA: 0x00038DA8 File Offset: 0x00036FA8
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

		// Token: 0x04000F8E RID: 3982
		public GameObject childParticlePrefab;

		// Token: 0x04000F8F RID: 3983
		public float spawnFrequency;

		// Token: 0x04000F90 RID: 3984
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04000F91 RID: 3985
		private float stopwatch;

		// Token: 0x04000F92 RID: 3986
		private List<Transform> bonesList;
	}
}
