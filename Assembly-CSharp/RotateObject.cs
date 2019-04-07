using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class RotateObject : MonoBehaviour
{
	// Token: 0x06000159 RID: 345 RVA: 0x00004507 File Offset: 0x00002707
	private void Start()
	{
	}

	// Token: 0x0600015A RID: 346 RVA: 0x00007FD0 File Offset: 0x000061D0
	private void Update()
	{
		base.transform.Rotate(this.rotationSpeed * Time.deltaTime);
	}

	// Token: 0x04000170 RID: 368
	public Vector3 rotationSpeed;
}
