using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000347 RID: 839
	[RequireComponent(typeof(LineRenderer))]
	public class LaserPointer : MonoBehaviour
	{
		// Token: 0x0600116A RID: 4458 RVA: 0x000568EA File Offset: 0x00054AEA
		private void Start()
		{
			this.line = base.GetComponent<LineRenderer>();
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x000568F8 File Offset: 0x00054AF8
		private void Update()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, this.laserDistance, LayerIndex.world.mask))
			{
				this.line.SetPosition(0, base.transform.position);
				this.line.SetPosition(1, raycastHit.point);
			}
		}

		// Token: 0x04001573 RID: 5491
		public float laserDistance;

		// Token: 0x04001574 RID: 5492
		private LineRenderer line;
	}
}
