using System;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class ScaledCamera : MonoBehaviour
{
	// Token: 0x0600015C RID: 348 RVA: 0x00004507 File Offset: 0x00002707
	private void Start()
	{
	}

	// Token: 0x0600015D RID: 349 RVA: 0x00007FF0 File Offset: 0x000061F0
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
