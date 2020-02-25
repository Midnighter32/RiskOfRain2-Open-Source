using System;
using UnityEngine;

namespace EntityStates.Engi.SpiderMine
{
	// Token: 0x0200086F RID: 2159
	public class ChaseTarget : BaseSpiderMineState
	{
		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x060030B0 RID: 12464 RVA: 0x000D1B3F File Offset: 0x000CFD3F
		private Transform target
		{
			get
			{
				return base.projectileTargetComponent.target;
			}
		}

		// Token: 0x060030B1 RID: 12465 RVA: 0x000D1B4C File Offset: 0x000CFD4C
		public override void OnEnter()
		{
			base.OnEnter();
			this.passedDetonationRadius = false;
			this.bestDistance = float.PositiveInfinity;
			base.PlayAnimation("Base", "Chase");
			if (base.isAuthority)
			{
				this.orientationHelper = base.gameObject.AddComponent<ChaseTarget.OrientationHelper>();
			}
		}

		// Token: 0x060030B2 RID: 12466 RVA: 0x000D1B9C File Offset: 0x000CFD9C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				if (!this.target)
				{
					base.rigidbody.AddForce(Vector3.up, ForceMode.VelocityChange);
					this.outer.SetNextState(new WaitForStick());
					return;
				}
				Vector3 position = this.target.position;
				Vector3 position2 = base.transform.position;
				Vector3 a = position - position2;
				float magnitude = a.magnitude;
				float y = base.rigidbody.velocity.y;
				Vector3 velocity = a * (ChaseTarget.speed / magnitude);
				velocity.y = y;
				base.rigidbody.velocity = velocity;
				if (!this.passedDetonationRadius && magnitude <= ChaseTarget.triggerRadius)
				{
					this.passedDetonationRadius = true;
				}
				if (magnitude < this.bestDistance)
				{
					this.bestDistance = magnitude;
					return;
				}
				if (this.passedDetonationRadius)
				{
					this.outer.SetNextState(new PreDetonate());
				}
			}
		}

		// Token: 0x060030B3 RID: 12467 RVA: 0x000D1C84 File Offset: 0x000CFE84
		public override void OnExit()
		{
			base.FindModelChild(this.childLocatorStringToEnable).gameObject.SetActive(false);
			if (this.orientationHelper != null)
			{
				EntityState.Destroy(this.orientationHelper);
				this.orientationHelper = null;
			}
			base.OnExit();
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x060030B4 RID: 12468 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04002EFD RID: 12029
		public static float speed;

		// Token: 0x04002EFE RID: 12030
		public static float triggerRadius;

		// Token: 0x04002EFF RID: 12031
		private bool passedDetonationRadius;

		// Token: 0x04002F00 RID: 12032
		private float bestDistance;

		// Token: 0x04002F01 RID: 12033
		private ChaseTarget.OrientationHelper orientationHelper;

		// Token: 0x02000870 RID: 2160
		private class OrientationHelper : MonoBehaviour
		{
			// Token: 0x060030B6 RID: 12470 RVA: 0x000D1CBD File Offset: 0x000CFEBD
			private void Awake()
			{
				this.rigidbody = base.GetComponent<Rigidbody>();
			}

			// Token: 0x060030B7 RID: 12471 RVA: 0x000D1CCC File Offset: 0x000CFECC
			private void OnCollisionStay(Collision collision)
			{
				int contactCount = collision.contactCount;
				if (contactCount == 0)
				{
					return;
				}
				Vector3 vector = collision.GetContact(0).normal;
				for (int i = 1; i < contactCount; i++)
				{
					Vector3 normal = collision.GetContact(i).normal;
					if (vector.y < normal.y)
					{
						vector = normal;
					}
				}
				this.rigidbody.MoveRotation(Quaternion.LookRotation(vector));
			}

			// Token: 0x04002F02 RID: 12034
			private Rigidbody rigidbody;
		}
	}
}
