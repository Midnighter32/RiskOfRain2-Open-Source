using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000267 RID: 615
	public class BlueprintController : MonoBehaviour
	{
		// Token: 0x06000B8F RID: 2959 RVA: 0x00038818 File Offset: 0x00036A18
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x00038828 File Offset: 0x00036A28
		private void Update()
		{
			Material sharedMaterial = this.ok ? this.okMaterial : this.invalidMaterial;
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.renderers[i].sharedMaterial = sharedMaterial;
			}
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x0003886D File Offset: 0x00036A6D
		public void PushState(Vector3 position, Quaternion rotation, bool ok)
		{
			this.transform.position = position;
			this.transform.rotation = rotation;
			this.ok = ok;
		}

		// Token: 0x04000F76 RID: 3958
		[NonSerialized]
		public bool ok;

		// Token: 0x04000F77 RID: 3959
		public Material okMaterial;

		// Token: 0x04000F78 RID: 3960
		public Material invalidMaterial;

		// Token: 0x04000F79 RID: 3961
		public Renderer[] renderers;

		// Token: 0x04000F7A RID: 3962
		private new Transform transform;
	}
}
