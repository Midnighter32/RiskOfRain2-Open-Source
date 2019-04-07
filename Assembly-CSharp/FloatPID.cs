using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class FloatPID : MonoBehaviour
{
	// Token: 0x06000111 RID: 273 RVA: 0x00004507 File Offset: 0x00002707
	private void Start()
	{
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00007194 File Offset: 0x00005394
	private void FixedUpdate()
	{
		this.timer += Time.fixedDeltaTime;
		if (this.automaticallyUpdate && this.timer > this.timeBetweenUpdates)
		{
			this.timer -= this.timeBetweenUpdates;
			this.outputFloat = this.UpdatePID();
		}
	}

	// Token: 0x06000113 RID: 275 RVA: 0x000071E8 File Offset: 0x000053E8
	public float UpdatePID()
	{
		float num = this.timer - this.lastTimer;
		this.lastTimer = this.timer;
		float num2 = this.targetFloat - this.inputFloat;
		this.errorSum += num2 * num;
		this.deltaError = (num2 - this.lastError) / num;
		this.lastError = num2;
		return (num2 * this.PID.x + this.errorSum * this.PID.y + this.deltaError * this.PID.z) * this.gain;
	}

	// Token: 0x0400011B RID: 283
	[Tooltip("PID Constants.")]
	public Vector3 PID = new Vector3(1f, 0f, 0f);

	// Token: 0x0400011C RID: 284
	public float gain = 1f;

	// Token: 0x0400011D RID: 285
	[HideInInspector]
	[Tooltip("The value we are currently at.")]
	public float inputFloat;

	// Token: 0x0400011E RID: 286
	[Tooltip("The value we want to be at.")]
	[HideInInspector]
	public float targetFloat;

	// Token: 0x0400011F RID: 287
	[Tooltip("Value output from PID controller; what we read.")]
	[HideInInspector]
	public float outputFloat;

	// Token: 0x04000120 RID: 288
	public float timeBetweenUpdates;

	// Token: 0x04000121 RID: 289
	private float timer;

	// Token: 0x04000122 RID: 290
	private float errorSum;

	// Token: 0x04000123 RID: 291
	private float deltaError;

	// Token: 0x04000124 RID: 292
	private float lastError;

	// Token: 0x04000125 RID: 293
	private float lastTimer;

	// Token: 0x04000126 RID: 294
	public bool automaticallyUpdate;
}
