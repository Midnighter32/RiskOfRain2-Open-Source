using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000054 RID: 84
public class VectorPID : MonoBehaviour
{
	// Token: 0x06000154 RID: 340 RVA: 0x0000409B File Offset: 0x0000229B
	private void Start()
	{
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000839B File Offset: 0x0000659B
	private void FixedUpdate()
	{
		this.timer += Time.fixedDeltaTime;
	}

	// Token: 0x06000156 RID: 342 RVA: 0x000083B0 File Offset: 0x000065B0
	public Vector3 UpdatePID()
	{
		float num = this.timer - this.lastTimer;
		this.lastTimer = this.timer;
		if (num != 0f)
		{
			Vector3 a;
			if (this.isAngle)
			{
				a = Vector3.zero;
				a.x = Mathf.DeltaAngle(this.inputVector.x, this.targetVector.x);
				a.y = Mathf.DeltaAngle(this.inputVector.y, this.targetVector.y);
				a.z = Mathf.DeltaAngle(this.inputVector.z, this.targetVector.z);
			}
			else
			{
				a = this.targetVector - this.inputVector;
			}
			this.errorSum += a * num;
			this.deltaError = (a - this.lastError) / num;
			this.lastError = a;
			this.outputVector = a * this.PID.x + this.errorSum * this.PID.y + this.deltaError * this.PID.z;
			return this.outputVector * this.gain;
		}
		return Vector3.zero;
	}

	// Token: 0x04000184 RID: 388
	[FormerlySerializedAs("name")]
	[Tooltip("Just a field for user naming. Doesn't do anything.")]
	public string customName;

	// Token: 0x04000185 RID: 389
	[Tooltip("PID Constants.")]
	public Vector3 PID = new Vector3(1f, 0f, 0f);

	// Token: 0x04000186 RID: 390
	[Tooltip("The vector we are currently at.")]
	[HideInInspector]
	public Vector3 inputVector = Vector3.zero;

	// Token: 0x04000187 RID: 391
	[Tooltip("The vector we want to be at.")]
	[HideInInspector]
	public Vector3 targetVector = Vector3.zero;

	// Token: 0x04000188 RID: 392
	[HideInInspector]
	[Tooltip("Vector output from PID controller; what we read.")]
	public Vector3 outputVector = Vector3.zero;

	// Token: 0x04000189 RID: 393
	[Tooltip("This is an euler angle, so we need to wrap correctly")]
	public bool isAngle;

	// Token: 0x0400018A RID: 394
	public float gain = 1f;

	// Token: 0x0400018B RID: 395
	private Vector3 errorSum = Vector3.zero;

	// Token: 0x0400018C RID: 396
	private Vector3 deltaError = Vector3.zero;

	// Token: 0x0400018D RID: 397
	private Vector3 lastError = Vector3.zero;

	// Token: 0x0400018E RID: 398
	private float lastTimer;

	// Token: 0x0400018F RID: 399
	private float timer;
}
