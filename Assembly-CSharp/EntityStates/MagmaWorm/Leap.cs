using System;
using UnityEngine;

namespace EntityStates.MagmaWorm
{
	// Token: 0x02000816 RID: 2070
	public class Leap : BaseState
	{
		// Token: 0x06002EF1 RID: 12017 RVA: 0x000C7CE8 File Offset: 0x000C5EE8
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelBaseTransform = base.GetModelBaseTransform();
			this.leapState = Leap.LeapState.Burrow;
		}

		// Token: 0x06002EF2 RID: 12018 RVA: 0x000C7D04 File Offset: 0x000C5F04
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

		// Token: 0x06002EF3 RID: 12019 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002C33 RID: 11315
		private Transform modelBaseTransform;

		// Token: 0x04002C34 RID: 11316
		private readonly float diveDepth = 200f;

		// Token: 0x04002C35 RID: 11317
		private readonly Vector3 idealDiveVelocity = Vector3.down * 90f;

		// Token: 0x04002C36 RID: 11318
		private readonly Vector3 idealLeapVelocity = Vector3.up * 90f;

		// Token: 0x04002C37 RID: 11319
		private float leapAcceleration = 80f;

		// Token: 0x04002C38 RID: 11320
		private float resurfaceSpeed = 60f;

		// Token: 0x04002C39 RID: 11321
		private Vector3 velocity;

		// Token: 0x04002C3A RID: 11322
		private Leap.LeapState leapState;

		// Token: 0x02000817 RID: 2071
		private enum LeapState
		{
			// Token: 0x04002C3C RID: 11324
			Burrow,
			// Token: 0x04002C3D RID: 11325
			Ascend,
			// Token: 0x04002C3E RID: 11326
			Fall,
			// Token: 0x04002C3F RID: 11327
			Resurface
		}
	}
}
