using System;
using UnityEngine;

// Token: 0x02000049 RID: 73
public class PhysicsController : MonoBehaviour
{
	// Token: 0x0600013F RID: 319 RVA: 0x00007898 File Offset: 0x00005A98
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.TransformPoint(this.centerOfMass), 0.5f);
	}

	// Token: 0x06000140 RID: 320 RVA: 0x000078BF File Offset: 0x00005ABF
	private void Awake()
	{
		this.carRigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00004507 File Offset: 0x00002707
	private void Update()
	{
	}

	// Token: 0x06000142 RID: 322 RVA: 0x000078D0 File Offset: 0x00005AD0
	private void FixedUpdate()
	{
		if (!this.turnOnInput || Input.GetAxis("Vertical") > 0f || Input.GetAxis("Vertical") > 0f)
		{
			this.desiredHeading = this.cameraTransform.forward;
			this.desiredHeading = Vector3.Project(this.desiredHeading, base.transform.forward);
			this.desiredHeading = this.cameraTransform.forward - this.desiredHeading;
			Debug.DrawRay(base.transform.position, this.desiredHeading * 15f, Color.magenta);
		}
		Vector3 vector = -base.transform.up;
		Debug.DrawRay(base.transform.position, vector * 15f, Color.blue);
		Vector3 a = Vector3.Cross(vector, this.desiredHeading);
		Debug.DrawRay(base.transform.position, a * 15f, Color.red);
		a.x = 0f;
		a.z = 0f;
		this.errorSum += a * Time.fixedDeltaTime;
		this.deltaError = (a - this.lastError) / Time.fixedDeltaTime;
		this.lastError = a;
		this.carRigidbody.AddTorque(a * this.PID.x + this.errorSum * this.PID.y + this.deltaError * this.PID.z, ForceMode.Acceleration);
	}

	// Token: 0x0400014E RID: 334
	public Vector3 centerOfMass = Vector3.zero;

	// Token: 0x0400014F RID: 335
	private Rigidbody carRigidbody;

	// Token: 0x04000150 RID: 336
	public Transform cameraTransform;

	// Token: 0x04000151 RID: 337
	public Vector3 PID = new Vector3(1f, 0f, 0f);

	// Token: 0x04000152 RID: 338
	public bool turnOnInput;

	// Token: 0x04000153 RID: 339
	private Vector3 errorSum = Vector3.zero;

	// Token: 0x04000154 RID: 340
	private Vector3 deltaError = Vector3.zero;

	// Token: 0x04000155 RID: 341
	private Vector3 lastError = Vector3.zero;

	// Token: 0x04000156 RID: 342
	private Vector3 desiredHeading;
}
