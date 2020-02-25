using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
[RequireComponent(typeof(Rigidbody))]
public class SetAngularVelocity : MonoBehaviour
{
	// Token: 0x0600014B RID: 331 RVA: 0x00008087 File Offset: 0x00006287
	private void Start()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600014C RID: 332 RVA: 0x00008095 File Offset: 0x00006295
	private void FixedUpdate()
	{
		this.rigidBody.maxAngularVelocity = this.angularVelocity.magnitude;
		this.rigidBody.angularVelocity = base.transform.TransformVector(this.angularVelocity);
	}

	// Token: 0x04000178 RID: 376
	public Vector3 angularVelocity;

	// Token: 0x04000179 RID: 377
	private Rigidbody rigidBody;
}
