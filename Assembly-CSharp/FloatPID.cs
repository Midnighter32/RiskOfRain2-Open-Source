using System;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class FloatPID : MonoBehaviour
{
	// Token: 0x060000F5 RID: 245 RVA: 0x0000409B File Offset: 0x0000229B
	private void Start()
	{
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x000070EC File Offset: 0x000052EC
	private void FixedUpdate()
	{
		this.timer += Time.fixedDeltaTime;
		if (this.automaticallyUpdate && this.timer > this.timeBetweenUpdates)
		{
			this.timer -= this.timeBetweenUpdates;
			this.outputFloat = this.UpdatePID();
		}
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00007140 File Offset: 0x00005340
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

	// Token: 0x04000120 RID: 288
	[Tooltip("PID Constants.")]
	public Vector3 PID = new Vector3(1f, 0f, 0f);

	// Token: 0x04000121 RID: 289
	public float gain = 1f;

	// Token: 0x04000122 RID: 290
	[Tooltip("The value we are currently at.")]
	[HideInInspector]
	public float inputFloat;

	// Token: 0x04000123 RID: 291
	[HideInInspector]
	[Tooltip("The value we want to be at.")]
	public float targetFloat;

	// Token: 0x04000124 RID: 292
	[HideInInspector]
	[Tooltip("Value output from PID controller; what we read.")]
	public float outputFloat;

	// Token: 0x04000125 RID: 293
	public float timeBetweenUpdates;

	// Token: 0x04000126 RID: 294
	private float timer;

	// Token: 0x04000127 RID: 295
	private float errorSum;

	// Token: 0x04000128 RID: 296
	private float deltaError;

	// Token: 0x04000129 RID: 297
	private float lastError;

	// Token: 0x0400012A RID: 298
	private float lastTimer;

	// Token: 0x0400012B RID: 299
	public bool automaticallyUpdate;
}
