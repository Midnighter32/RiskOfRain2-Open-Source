using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000152 RID: 338
	public class AssignRandomMaterial : MonoBehaviour
	{
		// Token: 0x06000616 RID: 1558 RVA: 0x00019847 File Offset: 0x00017A47
		private void Awake()
		{
			this.rend.material = this.materials[UnityEngine.Random.Range(0, this.materials.Length)];
		}

		// Token: 0x04000696 RID: 1686
		public Renderer rend;

		// Token: 0x04000697 RID: 1687
		public Material[] materials;
	}
}
