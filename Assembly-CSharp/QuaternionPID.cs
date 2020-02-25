using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000048 RID: 72
public class QuaternionPID : MonoBehaviour
{
	// Token: 0x0600012D RID: 301 RVA: 0x0000409B File Offset: 0x0000229B
	private void Start()
	{
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00007B39 File Offset: 0x00005D39
	private void Update()
	{
		this.timer += Time.deltaTime;
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00007B50 File Offset: 0x00005D50
	public Vector3 UpdatePID()
	{
		float num = this.timer - this.lastTimer;
		this.lastTimer = this.timer;
		if (num != 0f)
		{
			Quaternion quaternion = this.targetQuat * Quaternion.Inverse(this.inputQuat);
			if (quaternion.w < 0f)
			{
				quaternion.x *= -1f;
				quaternion.y *= -1f;
				quaternion.z *= -1f;
				quaternion.w *= -1f;
			}
			Vector3 a;
			a.x = quaternion.x;
			a.y = quaternion.y;
			a.z = quaternion.z;
			this.errorSum += a * num;
			this.deltaError = (a - this.lastError) / num;
			this.lastError = a;
			this.outputVector = a * this.PID.x + this.errorSum * this.PID.y + this.deltaError * this.PID.z;
			return this.outputVector * this.gain;
		}
		return Vector3.zero;
	}

	// Token: 0x0400015A RID: 346
	[FormerlySerializedAs("name")]
	[Tooltip("Just a field for user naming. Doesn't do anything.")]
	public string customName;

	// Token: 0x0400015B RID: 347
	[Tooltip("PID Constants.")]
	public Vector3 PID = new Vector3(1f, 0f, 0f);

	// Token: 0x0400015C RID: 348
	[Tooltip("The quaternion we are currently at.")]
	public Quaternion inputQuat = Quaternion.identity;

	// Token: 0x0400015D RID: 349
	[Tooltip("The quaternion we want to be at.")]
	public Quaternion targetQuat = Quaternion.identity;

	// Token: 0x0400015E RID: 350
	[HideInInspector]
	[Tooltip("Vector output from PID controller; what we read.")]
	public Vector3 outputVector = Vector3.zero;

	// Token: 0x0400015F RID: 351
	public float gain = 1f;

	// Token: 0x04000160 RID: 352
	private Vector3 errorSum = Vector3.zero;

	// Token: 0x04000161 RID: 353
	private Vector3 deltaError = Vector3.zero;

	// Token: 0x04000162 RID: 354
	private Vector3 lastError = Vector3.zero;

	// Token: 0x04000163 RID: 355
	private float lastTimer;

	// Token: 0x04000164 RID: 356
	private float timer;
}
