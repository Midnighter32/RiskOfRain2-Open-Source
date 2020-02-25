﻿using System;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class RotateItem : MonoBehaviour
{
	// Token: 0x0600013B RID: 315 RVA: 0x00007DE1 File Offset: 0x00005FE1
	private void Start()
	{
		this.initialPosition = base.transform.position;
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00007DF4 File Offset: 0x00005FF4
	private void Update()
	{
		this.counter += Time.deltaTime;
		base.transform.Rotate(new Vector3(0f, this.spinSpeed * Time.deltaTime, 0f), Space.World);
		if (base.transform.parent)
		{
			base.transform.localPosition = this.offsetVector + new Vector3(0f, 0f, Mathf.Sin(this.counter) * this.bobHeight);
			return;
		}
		base.transform.position = this.initialPosition + new Vector3(0f, Mathf.Sin(this.counter) * this.bobHeight, 0f);
	}

	// Token: 0x0400016B RID: 363
	public float spinSpeed = 30f;

	// Token: 0x0400016C RID: 364
	public float bobHeight = 0.3f;

	// Token: 0x0400016D RID: 365
	public Vector3 offsetVector = Vector3.zero;

	// Token: 0x0400016E RID: 366
	private float counter;

	// Token: 0x0400016F RID: 367
	private Vector3 initialPosition;
}
