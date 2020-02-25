using System;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class ScaledCamera : MonoBehaviour
{
	// Token: 0x06000141 RID: 321 RVA: 0x0000409B File Offset: 0x0000229B
	private void Start()
	{
	}

	// Token: 0x06000142 RID: 322 RVA: 0x00007F04 File Offset: 0x00006104
	private void LateUpdate()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			if (!this.foundCamera)
			{
				this.foundCamera = true;
				this.offset = main.transform.position - base.transform.position;
			}
			base.transform.eulerAngles = main.transform.eulerAngles;
			base.transform.position = main.transform.position / this.scale - this.offset;
		}
	}

	// Token: 0x04000171 RID: 369
	public float scale = 1f;

	// Token: 0x04000172 RID: 370
	private bool foundCamera;

	// Token: 0x04000173 RID: 371
	private Vector3 offset;
}
