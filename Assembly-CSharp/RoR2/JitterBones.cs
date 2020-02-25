using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026A RID: 618
	public class JitterBones : MonoBehaviour
	{
		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000DC1 RID: 3521 RVA: 0x0003DC3E File Offset: 0x0003BE3E
		// (set) Token: 0x06000DC2 RID: 3522 RVA: 0x0003DC46 File Offset: 0x0003BE46
		public SkinnedMeshRenderer skinnedMeshRenderer
		{
			get
			{
				return this._skinnedMeshRenderer;
			}
			set
			{
				if (this._skinnedMeshRenderer == value)
				{
					return;
				}
				this._skinnedMeshRenderer = value;
				this.RebuildBones();
			}
		}

		// Token: 0x06000DC3 RID: 3523 RVA: 0x0003DC60 File Offset: 0x0003BE60
		private void RebuildBones()
		{
			if (!this._skinnedMeshRenderer)
			{
				this.bones = Array.Empty<JitterBones.BoneInfo>();
				return;
			}
			Transform[] array = this._skinnedMeshRenderer.bones;
			Array.Resize<JitterBones.BoneInfo>(ref this.bones, array.Length);
			for (int i = 0; i < this.bones.Length; i++)
			{
				Transform transform = array[i];
				string text = transform.name.ToLower();
				this.bones[i] = new JitterBones.BoneInfo
				{
					transform = transform,
					isHead = text.Contains("head"),
					isRoot = text.Contains("root")
				};
			}
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x0003DD05 File Offset: 0x0003BF05
		private void Start()
		{
			this.RebuildBones();
		}

		// Token: 0x06000DC5 RID: 3525 RVA: 0x0003DD10 File Offset: 0x0003BF10
		private void LateUpdate()
		{
			if (this.skinnedMeshRenderer)
			{
				this.age += Time.deltaTime;
				for (int i = 0; i < this.bones.Length; i++)
				{
					JitterBones.BoneInfo boneInfo = this.bones[i];
					if (!boneInfo.isRoot)
					{
						float num = this.age * this.perlinNoiseFrequency;
						float num2 = (float)i;
						Vector3 vector = new Vector3(Mathf.PerlinNoise(num, num2), Mathf.PerlinNoise(num + 4f, num2 + 3f), Mathf.PerlinNoise(num + 6f, num2 - 7f));
						vector = HGMath.Remap(vector, this.perlinNoiseMinimumCutoff, this.perlinNoiseMaximumCutoff, -1f, 1f);
						vector = HGMath.Clamp(vector, 0f, 1f);
						vector *= this.perlinNoiseStrength;
						if (this.headBonusStrength >= 0f && boneInfo.isHead)
						{
							vector *= this.headBonusStrength;
						}
						boneInfo.transform.rotation *= Quaternion.Euler(vector);
					}
				}
			}
		}

		// Token: 0x04000DC0 RID: 3520
		[SerializeField]
		private SkinnedMeshRenderer _skinnedMeshRenderer;

		// Token: 0x04000DC1 RID: 3521
		private JitterBones.BoneInfo[] bones = Array.Empty<JitterBones.BoneInfo>();

		// Token: 0x04000DC2 RID: 3522
		public float perlinNoiseFrequency;

		// Token: 0x04000DC3 RID: 3523
		public float perlinNoiseStrength;

		// Token: 0x04000DC4 RID: 3524
		public float perlinNoiseMinimumCutoff;

		// Token: 0x04000DC5 RID: 3525
		public float perlinNoiseMaximumCutoff = 1f;

		// Token: 0x04000DC6 RID: 3526
		public float headBonusStrength;

		// Token: 0x04000DC7 RID: 3527
		private float age;

		// Token: 0x0200026B RID: 619
		private struct BoneInfo
		{
			// Token: 0x04000DC8 RID: 3528
			public Transform transform;

			// Token: 0x04000DC9 RID: 3529
			public bool isHead;

			// Token: 0x04000DCA RID: 3530
			public bool isRoot;
		}
	}
}
