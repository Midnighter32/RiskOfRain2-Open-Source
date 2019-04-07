using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002CC RID: 716
	public class DestroyOnTimer : MonoBehaviour
	{
		// Token: 0x06000E6F RID: 3695 RVA: 0x0004739A File Offset: 0x0004559A
		private void FixedUpdate()
		{
			this.age += Time.fixedDeltaTime;
			if (this.age > this.duration)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000E70 RID: 3696 RVA: 0x000473C7 File Offset: 0x000455C7
		private void OnDisable()
		{
			if (this.resetAgeOnDisable)
			{
				this.age = 0f;
			}
		}

		// Token: 0x04001278 RID: 4728
		public float duration;

		// Token: 0x04001279 RID: 4729
		public bool resetAgeOnDisable;

		// Token: 0x0400127A RID: 4730
		private float age;
	}
}
