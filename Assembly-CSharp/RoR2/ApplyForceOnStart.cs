using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200025A RID: 602
	public class ApplyForceOnStart : MonoBehaviour
	{
		// Token: 0x06000B3C RID: 2876 RVA: 0x00037B74 File Offset: 0x00035D74
		private void Start()
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component)
			{
				component.AddRelativeForce(this.localForce);
			}
		}

		// Token: 0x04000F4B RID: 3915
		public Vector3 localForce;
	}
}
