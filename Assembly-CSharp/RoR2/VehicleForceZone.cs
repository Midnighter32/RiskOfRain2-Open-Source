using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000411 RID: 1041
	[RequireComponent(typeof(Collider))]
	public class VehicleForceZone : MonoBehaviour
	{
		// Token: 0x06001737 RID: 5943 RVA: 0x0006E35F File Offset: 0x0006C55F
		private void Start()
		{
			this.collider = base.GetComponent<Collider>();
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x0006E370 File Offset: 0x0006C570
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
				}, true);
			}
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x0006E498 File Offset: 0x0006C698
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
					}, true);
				}
			}
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x00004507 File Offset: 0x00002707
		private void Update()
		{
		}

		// Token: 0x04001A5C RID: 6748
		public Rigidbody vehicleRigidbody;

		// Token: 0x04001A5D RID: 6749
		public float impactMultiplier;

		// Token: 0x04001A5E RID: 6750
		private Collider collider;
	}
}
