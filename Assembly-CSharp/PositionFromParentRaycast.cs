using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class PositionFromParentRaycast : MonoBehaviour
{
	// Token: 0x06000146 RID: 326 RVA: 0x00007B88 File Offset: 0x00005D88
	private void Update()
	{
		RaycastHit raycastHit = default(RaycastHit);
		if (Physics.Raycast(base.transform.parent.position, base.transform.parent.forward, out raycastHit, this.maxLength, this.mask))
		{
			base.transform.position = raycastHit.point;
			return;
		}
		base.transform.position = base.transform.parent.position + base.transform.parent.forward * this.maxLength;
	}

	// Token: 0x04000158 RID: 344
	public float maxLength;

	// Token: 0x04000159 RID: 345
	public LayerMask mask;
}
