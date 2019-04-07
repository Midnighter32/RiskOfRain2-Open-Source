using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class TracerBehavior : MonoBehaviour
{
	// Token: 0x0600016E RID: 366 RVA: 0x00008408 File Offset: 0x00006608
	private void Start()
	{
		this.direction = Vector3.Normalize(this.positions[1] - this.positions[0]);
	}

	// Token: 0x0600016F RID: 367 RVA: 0x00008434 File Offset: 0x00006634
	private void Update()
	{
		if (Vector3.Distance(base.transform.position, this.positions[1]) > this.speed * Time.deltaTime)
		{
			base.transform.position += this.direction * this.speed * Time.deltaTime;
		}
	}

	// Token: 0x04000182 RID: 386
	public float speed = 1f;

	// Token: 0x04000183 RID: 387
	public Vector3[] positions;

	// Token: 0x04000184 RID: 388
	private Vector3 direction;
}
