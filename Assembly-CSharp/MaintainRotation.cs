using System;
using UnityEngine;

// Token: 0x02000043 RID: 67
[ExecuteAlways]
public class MaintainRotation : MonoBehaviour
{
	// Token: 0x0600011E RID: 286 RVA: 0x0000409B File Offset: 0x0000229B
	private void Start()
	{
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00007725 File Offset: 0x00005925
	private void LateUpdate()
	{
		base.transform.eulerAngles = this.eulerAngles;
	}

	// Token: 0x0400014A RID: 330
	public Vector3 eulerAngles;
}
