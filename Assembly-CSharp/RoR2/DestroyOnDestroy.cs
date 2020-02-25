using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001DB RID: 475
	public class DestroyOnDestroy : MonoBehaviour
	{
		// Token: 0x06000A12 RID: 2578 RVA: 0x0002C176 File Offset: 0x0002A376
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.target);
		}

		// Token: 0x04000A66 RID: 2662
		[Tooltip("The GameObject to destroy when this object is destroyed.")]
		public GameObject target;
	}
}
