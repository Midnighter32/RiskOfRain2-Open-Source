using System;
using UnityEngine;

// Token: 0x02000047 RID: 71
[ExecuteInEditMode]
public class MaintainRotation : MonoBehaviour
{
	// Token: 0x06000136 RID: 310 RVA: 0x00004507 File Offset: 0x00002707
	private void Start()
	{
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00007762 File Offset: 0x00005962
	private void LateUpdate()
	{
		base.transform.eulerAngles = this.eulerAngles;
	}

	// Token: 0x04000145 RID: 325
	public Vector3 eulerAngles;
}
