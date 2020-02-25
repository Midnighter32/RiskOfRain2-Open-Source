using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class RotateObject : MonoBehaviour
{
	// Token: 0x0600013E RID: 318 RVA: 0x0000409B File Offset: 0x0000229B
	private void Start()
	{
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00007EE4 File Offset: 0x000060E4
	private void Update()
	{
		base.transform.Rotate(this.rotationSpeed * Time.deltaTime);
	}

	// Token: 0x04000170 RID: 368
	public Vector3 rotationSpeed;
}
