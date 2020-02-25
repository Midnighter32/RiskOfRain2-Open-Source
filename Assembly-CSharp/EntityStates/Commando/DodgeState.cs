using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando
{
	// Token: 0x020008B0 RID: 2224
	public class DodgeState : BaseState
	{
		// Token: 0x060031DB RID: 12763 RVA: 0x000D6B2C File Offset: 0x000D4D2C
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(DodgeState.dodgeSoundString, base.gameObject);
			this.animator = base.GetModelAnimator();
			ChildLocator component = this.animator.GetComponent<ChildLocator>();
			if (base.isAuthority && base.inputBank && base.characterDirection)
			{
				this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
			}
			Vector3 rhs = base.characterDirection ? base.characterDirection.forward : this.forwardDirection;
			Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);
			float num = Vector3.Dot(this.forwardDirection, rhs);
			float num2 = Vector3.Dot(this.forwardDirection, rhs2);
			this.animator.SetFloat("forwardSpeed", num, 0.1f, Time.fixedDeltaTime);
			this.animator.SetFloat("rightSpeed", num2, 0.1f, Time.fixedDeltaTime);
			if (Mathf.Abs(num) > Mathf.Abs(num2))
			{
				base.PlayAnimation("Body", (num > 0f) ? "DodgeForward" : "DodgeBackward", "Dodge.playbackRate", this.duration);
			}
			else
			{
				base.PlayAnimation("Body", (num2 > 0f) ? "DodgeRight" : "DodgeLeft", "Dodge.playbackRate", this.duration);
			}
			if (DodgeState.jetEffect)
			{
				Transform transform = component.FindChild("LeftJet");
				Transform transform2 = component.FindChild("RightJet");
				if (transform)
				{
					UnityEngine.Object.Instantiate<GameObject>(DodgeState.jetEffect, transform);
				}
				if (transform2)
				{
					UnityEngine.Object.Instantiate<GameObject>(DodgeState.jetEffect, transform2);
				}
			}
			this.RecalculateRollSpeed();
			if (base.characterMotor && base.characterDirection)
			{
				base.characterMotor.velocity.y = 0f;
				base.characterMotor.velocity = this.forwardDirection * this.rollSpeed;
			}
			Vector3 b = base.characterMotor ? base.characterMotor.velocity : Vector3.zero;
			this.previousPosition = base.transform.position - b;
		}

		// Token: 0x060031DC RID: 12764 RVA: 0x000D6D89 File Offset: 0x000D4F89
		private void RecalculateRollSpeed()
		{
			this.rollSpeed = this.moveSpeedStat * Mathf.Lerp(this.initialSpeedCoefficient, this.finalSpeedCoefficient, base.fixedAge / this.duration);
		}

		// Token: 0x060031DD RID: 12765 RVA: 0x000D6DB8 File Offset: 0x000D4FB8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.RecalculateRollSpeed();
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.fovOverride = Mathf.Lerp(DodgeState.dodgeFOV, 60f, base.fixedAge / this.duration);
			}
			Vector3 normalized = (base.transform.position - this.previousPosition).normalized;
			if (base.characterMotor && base.characterDirection && normalized != Vector3.zero)
			{
				Vector3 vector = normalized * this.rollSpeed;
				float y = vector.y;
				vector.y = 0f;
				float d = Mathf.Max(Vector3.Dot(vector, this.forwardDirection), 0f);
				vector = this.forwardDirection * d;
				vector.y += Mathf.Max(y, 0f);
				base.characterMotor.velocity = vector;
			}
			this.previousPosition = base.transform.position;
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060031DE RID: 12766 RVA: 0x000D6EE9 File Offset: 0x000D50E9
		public override void OnExit()
		{
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.fovOverride = -1f;
			}
			base.OnExit();
		}

		// Token: 0x060031DF RID: 12767 RVA: 0x000D6F0E File Offset: 0x000D510E
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.forwardDirection);
		}

		// Token: 0x060031E0 RID: 12768 RVA: 0x000D6F23 File Offset: 0x000D5123
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.forwardDirection = reader.ReadVector3();
		}

		// Token: 0x04003065 RID: 12389
		[SerializeField]
		public float duration = 0.9f;

		// Token: 0x04003066 RID: 12390
		[SerializeField]
		public float initialSpeedCoefficient;

		// Token: 0x04003067 RID: 12391
		[SerializeField]
		public float finalSpeedCoefficient;

		// Token: 0x04003068 RID: 12392
		public static string dodgeSoundString;

		// Token: 0x04003069 RID: 12393
		public static GameObject jetEffect;

		// Token: 0x0400306A RID: 12394
		public static float dodgeFOV;

		// Token: 0x0400306B RID: 12395
		private float rollSpeed;

		// Token: 0x0400306C RID: 12396
		private Vector3 forwardDirection;

		// Token: 0x0400306D RID: 12397
		private Animator animator;

		// Token: 0x0400306E RID: 12398
		private Vector3 previousPosition;
	}
}
