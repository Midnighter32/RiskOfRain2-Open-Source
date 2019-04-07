using System;
using UnityEngine;

namespace EntityStates.MagmaWorm
{
	// Token: 0x0200010F RID: 271
	public class Leap : BaseState
	{
		// Token: 0x06000532 RID: 1330 RVA: 0x00016ECD File Offset: 0x000150CD
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelBaseTransform = base.GetModelBaseTransform();
			this.leapState = Leap.LeapState.Burrow;
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x00016EE8 File Offset: 0x000150E8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			switch (this.leapState)
			{
			case Leap.LeapState.Burrow:
				if (this.modelBaseTransform)
				{
					if (this.modelBaseTransform.position.y >= base.transform.position.y - this.diveDepth)
					{
						this.velocity = Vector3.MoveTowards(this.velocity, this.idealDiveVelocity, this.leapAcceleration * Time.fixedDeltaTime);
						this.modelBaseTransform.position += this.velocity * Time.fixedDeltaTime;
						return;
					}
					this.leapState = Leap.LeapState.Ascend;
					return;
				}
				break;
			case Leap.LeapState.Ascend:
				if (this.modelBaseTransform)
				{
					if (this.modelBaseTransform.position.y <= base.transform.position.y)
					{
						this.velocity = Vector3.MoveTowards(this.velocity, this.idealLeapVelocity, this.leapAcceleration * Time.fixedDeltaTime);
						this.modelBaseTransform.position += this.velocity * Time.fixedDeltaTime;
						return;
					}
					this.leapState = Leap.LeapState.Fall;
					return;
				}
				break;
			case Leap.LeapState.Fall:
				if (this.modelBaseTransform)
				{
					if (this.modelBaseTransform.position.y >= base.transform.position.y - this.diveDepth)
					{
						this.velocity += Physics.gravity * Time.fixedDeltaTime;
						this.modelBaseTransform.position += this.velocity * Time.fixedDeltaTime;
						return;
					}
					this.leapState = Leap.LeapState.Resurface;
					return;
				}
				break;
			case Leap.LeapState.Resurface:
				this.velocity = Vector3.zero;
				this.modelBaseTransform.position = Vector3.MoveTowards(this.modelBaseTransform.position, base.transform.position, this.resurfaceSpeed * Time.fixedDeltaTime);
				if (this.modelBaseTransform.position.y >= base.transform.position.y)
				{
					this.outer.SetNextStateToMain();
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000565 RID: 1381
		private Transform modelBaseTransform;

		// Token: 0x04000566 RID: 1382
		private readonly float diveDepth = 200f;

		// Token: 0x04000567 RID: 1383
		private readonly Vector3 idealDiveVelocity = Vector3.down * 90f;

		// Token: 0x04000568 RID: 1384
		private readonly Vector3 idealLeapVelocity = Vector3.up * 90f;

		// Token: 0x04000569 RID: 1385
		private float leapAcceleration = 80f;

		// Token: 0x0400056A RID: 1386
		private float resurfaceSpeed = 60f;

		// Token: 0x0400056B RID: 1387
		private Vector3 velocity;

		// Token: 0x0400056C RID: 1388
		private Leap.LeapState leapState;

		// Token: 0x02000110 RID: 272
		private enum LeapState
		{
			// Token: 0x0400056E RID: 1390
			Burrow,
			// Token: 0x0400056F RID: 1391
			Ascend,
			// Token: 0x04000570 RID: 1392
			Fall,
			// Token: 0x04000571 RID: 1393
			Resurface
		}
	}
}
