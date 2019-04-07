using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000412 RID: 1042
	public class VelocityRandomOnStart : MonoBehaviour
	{
		// Token: 0x0600173C RID: 5948 RVA: 0x0006E5A8 File Offset: 0x0006C7A8
		private void Start()
		{
			if (NetworkServer.active)
			{
				Rigidbody component = base.GetComponent<Rigidbody>();
				if (component)
				{
					float num = (this.minSpeed != this.maxSpeed) ? UnityEngine.Random.Range(this.minSpeed, this.maxSpeed) : this.minSpeed;
					if (num != 0f)
					{
						Vector3 vector = Vector3.zero;
						Vector3 vector2 = this.localDirection ? (base.transform.rotation * this.baseDirection) : this.baseDirection;
						switch (this.directionMode)
						{
						case VelocityRandomOnStart.DirectionMode.Sphere:
							vector = UnityEngine.Random.onUnitSphere;
							break;
						case VelocityRandomOnStart.DirectionMode.Hemisphere:
							vector = UnityEngine.Random.onUnitSphere;
							if (Vector3.Dot(vector, vector2) < 0f)
							{
								vector = -vector;
							}
							break;
						case VelocityRandomOnStart.DirectionMode.Cone:
							vector = Util.ApplySpread(vector2, 0f, this.coneAngle, 1f, 1f, 0f, 0f);
							break;
						}
						component.velocity = vector * num;
					}
					float num2 = (this.minAngularSpeed != this.maxAngularSpeed) ? UnityEngine.Random.Range(this.minAngularSpeed, this.maxAngularSpeed) : this.minAngularSpeed;
					if (num2 != 0f)
					{
						component.angularVelocity = UnityEngine.Random.onUnitSphere * (num2 * 0.017453292f);
					}
				}
			}
		}

		// Token: 0x04001A5F RID: 6751
		public float minSpeed;

		// Token: 0x04001A60 RID: 6752
		public float maxSpeed;

		// Token: 0x04001A61 RID: 6753
		public Vector3 baseDirection = Vector3.up;

		// Token: 0x04001A62 RID: 6754
		public bool localDirection;

		// Token: 0x04001A63 RID: 6755
		public VelocityRandomOnStart.DirectionMode directionMode;

		// Token: 0x04001A64 RID: 6756
		public float coneAngle = 30f;

		// Token: 0x04001A65 RID: 6757
		[Tooltip("Minimum angular speed in degrees/second.")]
		public float minAngularSpeed;

		// Token: 0x04001A66 RID: 6758
		[Tooltip("Maximum angular speed in degrees/second.")]
		public float maxAngularSpeed;

		// Token: 0x02000413 RID: 1043
		public enum DirectionMode
		{
			// Token: 0x04001A68 RID: 6760
			Sphere,
			// Token: 0x04001A69 RID: 6761
			Hemisphere,
			// Token: 0x04001A6A RID: 6762
			Cone
		}
	}
}
