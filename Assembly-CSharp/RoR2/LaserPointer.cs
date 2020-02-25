using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200026D RID: 621
	[RequireComponent(typeof(LineRenderer))]
	public class LaserPointer : MonoBehaviour
	{
		// Token: 0x06000DCA RID: 3530 RVA: 0x0003DF2E File Offset: 0x0003C12E
		private void Start()
		{
			this.line = base.GetComponent<LineRenderer>();
		}

		// Token: 0x06000DCB RID: 3531 RVA: 0x0003DF3C File Offset: 0x0003C13C
		private void Update()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, this.laserDistance, LayerIndex.world.mask))
			{
				this.line.SetPosition(0, base.transform.position);
				this.line.SetPosition(1, raycastHit.point);
			}
		}

		// Token: 0x04000DCF RID: 3535
		public float laserDistance;

		// Token: 0x04000DD0 RID: 3536
		private LineRenderer line;
	}
}
