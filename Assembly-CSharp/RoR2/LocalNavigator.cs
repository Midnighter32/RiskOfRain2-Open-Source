using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000451 RID: 1105
	public class LocalNavigator
	{
		// Token: 0x0600188F RID: 6287 RVA: 0x0007631C File Offset: 0x0007451C
		public void SetBody(CharacterBody newBody)
		{
			this.transform = null;
			this.characterCollider = null;
			this.body = newBody;
			if (this.body)
			{
				this.transform = this.body.GetComponent<Transform>();
				this.motor = this.body.GetComponent<CharacterMotor>();
				this.currentPosition = this.transform.position;
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06001890 RID: 6288 RVA: 0x0007637E File Offset: 0x0007457E
		// (set) Token: 0x06001891 RID: 6289 RVA: 0x00076386 File Offset: 0x00074586
		public Vector3 moveVector { get; private set; }

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06001892 RID: 6290 RVA: 0x0007638F File Offset: 0x0007458F
		// (set) Token: 0x06001893 RID: 6291 RVA: 0x00076397 File Offset: 0x00074597
		public float jumpSpeed { get; private set; }

		// Token: 0x06001894 RID: 6292 RVA: 0x000763A0 File Offset: 0x000745A0
		public void Update(float deltaTime)
		{
			if (this.transform)
			{
				this.currentPosition = this.transform.position;
			}
			Vector3 vector = this.targetPosition - this.currentPosition;
			float magnitude = vector.magnitude;
			if (magnitude == 0f)
			{
				this.moveVector = Vector3.zero;
			}
			else
			{
				float num = this.body ? this.body.moveSpeed : 0f;
				float b = this.body ? this.body.maxJumpHeight : 0f;
				float a = 0f;
				float num2 = 0f;
				if (this.characterCollider)
				{
					Bounds bounds = this.characterCollider.bounds;
					a = (bounds.max.y - this.currentPosition.y) * 0.5f;
					num2 = bounds.min.y;
				}
				Vector3 vector2 = this.currentPosition;
				vector2.y += Mathf.Min(a, b);
				LayerMask mask = LayerIndex.world.mask | LayerIndex.defaultLayer.mask;
				float num3 = this.lookAheadDistance;
				bool flag = !Physics.Raycast(vector2, vector, num3, mask);
				Debug.DrawRay(vector2, vector.normalized * num3, flag ? Color.yellow : Color.red, deltaTime);
				float num4 = 0f;
				if (this.backupRestrictionTimer <= 0f)
				{
					if (!flag)
					{
						float num5 = 45f;
						bool flag2 = !Physics.Raycast(vector2, Quaternion.Euler(0f, num5, 0f) * vector, num3, mask);
						bool flag3 = !Physics.Raycast(vector2, Quaternion.Euler(0f, -num5, 0f) * vector, num3, mask);
						Debug.DrawRay(vector2, (Quaternion.Euler(0f, num5, 0f) * vector).normalized * num3, flag2 ? Color.yellow : Color.red, deltaTime);
						Debug.DrawRay(vector2, (Quaternion.Euler(0f, -num5, 0f) * vector).normalized * num3, flag3 ? Color.yellow : Color.red, deltaTime);
						int num6 = (flag2 ? -1 : 0) + (flag3 ? 1 : 0);
						if (num6 == 0)
						{
							num6 = ((UnityEngine.Random.Range(0, 1) == 1) ? -1 : 1);
						}
						if (this.walkFrustration > 0.5f)
						{
							num4 = vector.y + 1f;
						}
						else
						{
							this.moveVector = Quaternion.Euler(0f, -num5 * (float)num6, 0f) * (vector / magnitude);
						}
					}
					else
					{
						this.moveVector = vector / magnitude;
					}
				}
				if (num != 0f)
				{
					float num7 = 4f;
					float num8 = num * 0.5f;
					float magnitude2 = ((this.currentPosition - this.previousPosition) / deltaTime).magnitude;
					float num9 = num8 - magnitude2;
					this.walkFrustration = Mathf.Clamp01(this.walkFrustration + num7 * num9 / num8 * deltaTime);
				}
				if (this.motor)
				{
					bool isGrounded = this.motor.isGrounded;
				}
				if (this.walkFrustration >= 0.5f && flag)
				{
					float num10 = this.motor ? this.motor.capsuleRadius : 1f;
					Vector3 a2 = vector;
					a2.y = 0f;
					a2.Normalize();
					float num11 = vector2.y - num2;
					RaycastHit raycastHit;
					if (Physics.Raycast(vector2 + a2 * (num10 + 0.25f), Vector3.down, out raycastHit, num11, mask))
					{
						num4 = num11 - raycastHit.distance;
					}
				}
				if (num4 >= (this.motor ? this.motor.stepOffset : 0f))
				{
					this.jumpSpeed = Trajectory.CalculateInitialYSpeed(0.5f, num4 + 0.1f);
				}
				else
				{
					this.jumpSpeed = 0f;
				}
			}
			this.avoidanceTimer -= deltaTime;
			this.walkFrustration -= deltaTime * 0.25f;
			this.backupRestrictionTimer -= deltaTime;
			this.previousPosition = this.currentPosition;
		}

		// Token: 0x04001C2D RID: 7213
		private Vector3 currentPosition;

		// Token: 0x04001C2E RID: 7214
		private Vector3 previousPosition;

		// Token: 0x04001C2F RID: 7215
		public Vector3 targetPosition;

		// Token: 0x04001C30 RID: 7216
		public float lookAheadDistance = 3.5f;

		// Token: 0x04001C31 RID: 7217
		public float avoidanceDuration = 0.5f;

		// Token: 0x04001C32 RID: 7218
		private float avoidanceTimer;

		// Token: 0x04001C33 RID: 7219
		private CharacterBody body;

		// Token: 0x04001C34 RID: 7220
		private Transform transform;

		// Token: 0x04001C35 RID: 7221
		private CharacterMotor motor;

		// Token: 0x04001C36 RID: 7222
		private Collider characterCollider;

		// Token: 0x04001C39 RID: 7225
		private float walkFrustration;

		// Token: 0x04001C3A RID: 7226
		private float backupRestrictionTimer;
	}
}
