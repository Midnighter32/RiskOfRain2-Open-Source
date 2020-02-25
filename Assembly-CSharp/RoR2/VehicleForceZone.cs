using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200036A RID: 874
	[RequireComponent(typeof(Collider))]
	public class VehicleForceZone : MonoBehaviour
	{
		// Token: 0x0600153E RID: 5438 RVA: 0x0005A737 File Offset: 0x00058937
		private void Start()
		{
			this.collider = base.GetComponent<Collider>();
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x0005A748 File Offset: 0x00058948
		public void OnTriggerEnter(Collider other)
		{
			CharacterMotor component = other.GetComponent<CharacterMotor>();
			HealthComponent component2 = other.GetComponent<HealthComponent>();
			if (component && component2)
			{
				Vector3 position = base.transform.position;
				Vector3 normalized = this.vehicleRigidbody.velocity.normalized;
				Vector3 pointVelocity = this.vehicleRigidbody.GetPointVelocity(position);
				Vector3 vector = pointVelocity * this.vehicleRigidbody.mass * this.impactMultiplier;
				float mass = this.vehicleRigidbody.mass;
				Mathf.Pow(pointVelocity.magnitude, 2f);
				float num = component.mass / (component.mass + this.vehicleRigidbody.mass);
				this.vehicleRigidbody.AddForceAtPosition(-vector * num, position);
				Debug.LogFormat("Impulse: {0}, Ratio: {1}", new object[]
				{
					vector.magnitude,
					num
				});
				component2.TakeDamageForce(new DamageInfo
				{
					attacker = base.gameObject,
					force = vector,
					position = position
				}, true, false);
			}
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x0005A870 File Offset: 0x00058A70
		public void OnCollisionEnter(Collision collision)
		{
			Debug.LogFormat("Hit {0}", new object[]
			{
				collision.gameObject
			});
			Rigidbody component = collision.collider.GetComponent<Rigidbody>();
			if (component)
			{
				Debug.Log("Hit?");
				HealthComponent component2 = component.GetComponent<HealthComponent>();
				if (component2)
				{
					Vector3 point = collision.contacts[0].point;
					Vector3 normal = collision.contacts[0].normal;
					this.vehicleRigidbody.GetPointVelocity(point);
					Vector3 impulse = collision.impulse;
					float num = 0f;
					this.vehicleRigidbody.AddForceAtPosition(impulse * num, point);
					Debug.LogFormat("Impulse: {0}, Ratio: {1}", new object[]
					{
						impulse,
						num
					});
					component2.TakeDamageForce(new DamageInfo
					{
						attacker = base.gameObject,
						force = -impulse * (1f - num),
						position = point
					}, true, false);
				}
			}
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x0000409B File Offset: 0x0000229B
		private void Update()
		{
		}

		// Token: 0x040013CD RID: 5069
		public Rigidbody vehicleRigidbody;

		// Token: 0x040013CE RID: 5070
		public float impactMultiplier;

		// Token: 0x040013CF RID: 5071
		private Collider collider;
	}
}
