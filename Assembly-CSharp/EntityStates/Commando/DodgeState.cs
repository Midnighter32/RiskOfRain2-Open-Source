using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando
{
	// Token: 0x020001A0 RID: 416
	public class DodgeState : BaseState
	{
		// Token: 0x0600080E RID: 2062 RVA: 0x00027ED0 File Offset: 0x000260D0
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
				base.PlayAnimation("Body", (num > 0f) ? "DodgeForward" : "DodgeBackward", "Dodge.playbackRate", DodgeState.duration);
			}
			else
			{
				base.PlayAnimation("Body", (num2 > 0f) ? "DodgeRight" : "DodgeLeft", "Dodge.playbackRate", DodgeState.duration);
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

		// Token: 0x0600080F RID: 2063 RVA: 0x0002812B File Offset: 0x0002632B
		private void RecalculateRollSpeed()
		{
			this.rollSpeed = this.moveSpeedStat * Mathf.Lerp(DodgeState.initialSpeedCoefficient, DodgeState.finalSpeedCoefficient, base.fixedAge / DodgeState.duration);
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x00028158 File Offset: 0x00026358
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.RecalculateRollSpeed();
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.fovOverride = Mathf.Lerp(DodgeState.dodgeFOV, 60f, base.fixedAge / DodgeState.duration);
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
			if (base.fixedAge >= DodgeState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x00028287 File Offset: 0x00026487
		public override void OnExit()
		{
			base.OnExit();
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.fovOverride = -1f;
			}
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x000282AC File Offset: 0x000264AC
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.forwardDirection);
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x000282C1 File Offset: 0x000264C1
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.forwardDirection = reader.ReadVector3();
		}

		// Token: 0x04000A88 RID: 2696
		public static float duration = 0.9f;

		// Token: 0x04000A89 RID: 2697
		public static float initialSpeedCoefficient;

		// Token: 0x04000A8A RID: 2698
		public static float finalSpeedCoefficient;

		// Token: 0x04000A8B RID: 2699
		public static string dodgeSoundString;

		// Token: 0x04000A8C RID: 2700
		public static GameObject jetEffect;

		// Token: 0x04000A8D RID: 2701
		public static float dodgeFOV;

		// Token: 0x04000A8E RID: 2702
		public static float commandoBoostBuffDuration;

		// Token: 0x04000A8F RID: 2703
		private float rollSpeed;

		// Token: 0x04000A90 RID: 2704
		private Vector3 forwardDirection;

		// Token: 0x04000A91 RID: 2705
		private Animator animator;

		// Token: 0x04000A92 RID: 2706
		private Vector3 previousPosition;
	}
}
