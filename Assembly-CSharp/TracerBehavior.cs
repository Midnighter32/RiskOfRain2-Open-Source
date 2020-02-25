using System;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class TracerBehavior : MonoBehaviour
{
	// Token: 0x06000151 RID: 337 RVA: 0x000082F5 File Offset: 0x000064F5
	private void Start()
	{
		this.direction = Vector3.Normalize(this.positions[1] - this.positions[0]);
	}

	// Token: 0x06000152 RID: 338 RVA: 0x00008320 File Offset: 0x00006520
	private void Update()
	{
		if (Vector3.Distance(base.transform.position, this.positions[1]) > this.speed * Time.deltaTime)
		{
			base.transform.position += this.direction * this.speed * Time.deltaTime;
		}
	}

	// Token: 0x04000181 RID: 385
	public float speed = 1f;

	// Token: 0x04000182 RID: 386
	public Vector3[] positions;

	// Token: 0x04000183 RID: 387
	private Vector3 direction;
}
