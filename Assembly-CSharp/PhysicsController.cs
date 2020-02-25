using System;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class PhysicsController : MonoBehaviour
{
	// Token: 0x06000121 RID: 289 RVA: 0x00007738 File Offset: 0x00005938
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.TransformPoint(this.centerOfMass), 0.5f);
	}

	// Token: 0x06000122 RID: 290 RVA: 0x0000775F File Offset: 0x0000595F
	private void Awake()
	{
		this.carRigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000123 RID: 291 RVA: 0x0000409B File Offset: 0x0000229B
	private void Update()
	{
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00007770 File Offset: 0x00005970
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

	// Token: 0x0400014B RID: 331
	public Vector3 centerOfMass = Vector3.zero;

	// Token: 0x0400014C RID: 332
	private Rigidbody carRigidbody;

	// Token: 0x0400014D RID: 333
	public Transform cameraTransform;

	// Token: 0x0400014E RID: 334
	public Vector3 PID = new Vector3(1f, 0f, 0f);

	// Token: 0x0400014F RID: 335
	public bool turnOnInput;

	// Token: 0x04000150 RID: 336
	private Vector3 errorSum = Vector3.zero;

	// Token: 0x04000151 RID: 337
	private Vector3 deltaError = Vector3.zero;

	// Token: 0x04000152 RID: 338
	private Vector3 lastError = Vector3.zero;

	// Token: 0x04000153 RID: 339
	private Vector3 desiredHeading;
}
