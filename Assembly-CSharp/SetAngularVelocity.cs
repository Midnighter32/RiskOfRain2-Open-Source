using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
[RequireComponent(typeof(Rigidbody))]
public class SetAngularVelocity : MonoBehaviour
{
	// Token: 0x06000166 RID: 358 RVA: 0x00008173 File Offset: 0x00006373
	private void Start()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000167 RID: 359 RVA: 0x00008181 File Offset: 0x00006381
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
