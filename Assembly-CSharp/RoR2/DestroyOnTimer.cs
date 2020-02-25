using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001DC RID: 476
	public class DestroyOnTimer : MonoBehaviour
	{
		// Token: 0x06000A14 RID: 2580 RVA: 0x0002C183 File Offset: 0x0002A383
		private void FixedUpdate()
		{
			this.age += Time.fixedDeltaTime;
			if (this.age > this.duration)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x0002C1B0 File Offset: 0x0002A3B0
		private void OnDisable()
		{
			if (this.resetAgeOnDisable)
			{
				this.age = 0f;
			}
		}

		// Token: 0x04000A67 RID: 2663
		public float duration;

		// Token: 0x04000A68 RID: 2664
		public bool resetAgeOnDisable;

		// Token: 0x04000A69 RID: 2665
		private float age;
	}
}
