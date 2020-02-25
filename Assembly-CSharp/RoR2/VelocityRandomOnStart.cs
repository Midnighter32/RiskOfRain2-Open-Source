using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036E RID: 878
	public class VelocityRandomOnStart : MonoBehaviour
	{
		// Token: 0x06001573 RID: 5491 RVA: 0x0005B878 File Offset: 0x00059A78
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

		// Token: 0x040013F5 RID: 5109
		public float minSpeed;

		// Token: 0x040013F6 RID: 5110
		public float maxSpeed;

		// Token: 0x040013F7 RID: 5111
		public Vector3 baseDirection = Vector3.up;

		// Token: 0x040013F8 RID: 5112
		public bool localDirection;

		// Token: 0x040013F9 RID: 5113
		public VelocityRandomOnStart.DirectionMode directionMode;

		// Token: 0x040013FA RID: 5114
		public float coneAngle = 30f;

		// Token: 0x040013FB RID: 5115
		[Tooltip("Minimum angular speed in degrees/second.")]
		public float minAngularSpeed;

		// Token: 0x040013FC RID: 5116
		[Tooltip("Maximum angular speed in degrees/second.")]
		public float maxAngularSpeed;

		// Token: 0x0200036F RID: 879
		public enum DirectionMode
		{
			// Token: 0x040013FE RID: 5118
			Sphere,
			// Token: 0x040013FF RID: 5119
			Hemisphere,
			// Token: 0x04001400 RID: 5120
			Cone
		}
	}
}
