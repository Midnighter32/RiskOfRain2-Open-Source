using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200014B RID: 331
	public class ApplyForceOnStart : MonoBehaviour
	{
		// Token: 0x060005E5 RID: 1509 RVA: 0x000186E8 File Offset: 0x000168E8
		private void Start()
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component)
			{
				component.AddRelativeForce(this.localForce);
			}
		}

		// Token: 0x04000668 RID: 1640
		public Vector3 localForce;
	}
}
