using System;
using UnityEngine;

// Token: 0x02000046 RID: 70
public class PositionFromParentRaycast : MonoBehaviour
{
	// Token: 0x06000128 RID: 296 RVA: 0x00007A28 File Offset: 0x00005C28
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

	// Token: 0x04000155 RID: 341
	public float maxLength;

	// Token: 0x04000156 RID: 342
	public LayerMask mask;
}
