using System;
using UnityEngine;

// Token: 0x02000047 RID: 71
[RequireComponent(typeof(Rigidbody))]
public class PhysicsImpactSpeedModifier : MonoBehaviour
{
	// Token: 0x0600012A RID: 298 RVA: 0x00007AC5 File Offset: 0x00005CC5
	private void Awake()
	{
		this.rigid = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00007AD4 File Offset: 0x00005CD4
	private void OnCollisionEnter(Collision collision)
	{
		Vector3 normal = collision.contacts[0].normal;
		Vector3 velocity = this.rigid.velocity;
		Vector3 vector = Vector3.Project(velocity, normal);
		Vector3 vector2 = velocity - vector;
		vector *= this.normalSpeedModifier;
		vector2 *= this.perpendicularSpeedModifier;
		this.rigid.velocity = vector + vector2;
	}

	// Token: 0x04000157 RID: 343
	public float normalSpeedModifier;

	// Token: 0x04000158 RID: 344
	public float perpendicularSpeedModifier;

	// Token: 0x04000159 RID: 345
	private Rigidbody rigid;
}
