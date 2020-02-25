using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200015D RID: 349
	public class BlueprintController : MonoBehaviour
	{
		// Token: 0x0600066D RID: 1645 RVA: 0x0001A520 File Offset: 0x00018720
		private void Awake()
		{
			this.transform = base.transform;
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x0001A530 File Offset: 0x00018730
		private void Update()
		{
			Material sharedMaterial = this.ok ? this.okMaterial : this.invalidMaterial;
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.renderers[i].sharedMaterial = sharedMaterial;
			}
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0001A575 File Offset: 0x00018775
		public void PushState(Vector3 position, Quaternion rotation, bool ok)
		{
			this.transform.position = position;
			this.transform.rotation = rotation;
			this.ok = ok;
		}

		// Token: 0x040006BE RID: 1726
		[NonSerialized]
		public bool ok;

		// Token: 0x040006BF RID: 1727
		public Material okMaterial;

		// Token: 0x040006C0 RID: 1728
		public Material invalidMaterial;

		// Token: 0x040006C1 RID: 1729
		public Renderer[] renderers;

		// Token: 0x040006C2 RID: 1730
		private new Transform transform;
	}
}
