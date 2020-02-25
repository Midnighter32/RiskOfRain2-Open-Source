using System;
using System.Linq;
using RoR2;
using UnityEngine;

namespace EntityStates.Headstompers
{
	// Token: 0x02000842 RID: 2114
	public class HeadstompersFall : BaseHeadstompersState
	{
		// Token: 0x06002FD7 RID: 12247 RVA: 0x000CCF68 File Offset: 0x000CB168
		public override void OnEnter()
		{
			base.OnEnter();
			this.highestFallSpeed = 0f;
			if (base.isAuthority)
			{
				if (this.body)
				{
					TeamMask allButNeutral = TeamMask.allButNeutral;
					TeamIndex objectTeam = TeamComponent.GetObjectTeam(this.bodyGameObject);
					if (objectTeam != TeamIndex.None)
					{
						allButNeutral.RemoveTeam(objectTeam);
					}
					BullseyeSearch bullseyeSearch = new BullseyeSearch();
					bullseyeSearch.filterByLoS = true;
					bullseyeSearch.maxDistanceFilter = 300f;
					bullseyeSearch.maxAngleFilter = HeadstompersFall.seekCone;
					bullseyeSearch.searchOrigin = this.body.footPosition;
					bullseyeSearch.searchDirection = Vector3.down;
					bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
					bullseyeSearch.teamMaskFilter = allButNeutral;
					bullseyeSearch.viewer = this.body;
					bullseyeSearch.RefreshCandidates();
					HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
					this.seekTransform = ((hurtBox != null) ? hurtBox.transform : null);
					GameObject gameObject;
					if (hurtBox == null)
					{
						gameObject = null;
					}
					else
					{
						HealthComponent healthComponent = hurtBox.healthComponent;
						gameObject = ((healthComponent != null) ? healthComponent.gameObject : null);
					}
					this.seekBodyObject = gameObject;
				}
				this.SetOnHitGroundProvider(this.bodyMotor);
			}
		}

		// Token: 0x06002FD8 RID: 12248 RVA: 0x000CD064 File Offset: 0x000CB264
		private void SetOnHitGroundProvider(CharacterMotor newOnHitGroundProvider)
		{
			if (this.onHitGroundProvider != null)
			{
				this.onHitGroundProvider.onHitGround -= this.OnMotorHitGround;
			}
			this.onHitGroundProvider = newOnHitGroundProvider;
			if (this.onHitGroundProvider != null)
			{
				this.onHitGroundProvider.onHitGround += this.OnMotorHitGround;
			}
		}

		// Token: 0x06002FD9 RID: 12249 RVA: 0x000CD0B6 File Offset: 0x000CB2B6
		public override void OnExit()
		{
			this.SetOnHitGroundProvider(null);
			base.OnExit();
		}

		// Token: 0x06002FDA RID: 12250 RVA: 0x000CD0C5 File Offset: 0x000CB2C5
		private void OnMotorHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
		{
			this.highestFallSpeed = Mathf.Max(this.highestFallSpeed, -hitGroundInfo.velocity.y);
			this.OnHitGround(this.highestFallSpeed, hitGroundInfo.position);
		}

		// Token: 0x06002FDB RID: 12251 RVA: 0x000CD0F6 File Offset: 0x000CB2F6
		private void OnHitGround(float impactSpeed, Vector3 position)
		{
			this.outer.SetNextState(new HeadstompersCooldown
			{
				impactSpeed = impactSpeed,
				impactPosition = position
			});
			this.SetOnHitGroundProvider(null);
		}

		// Token: 0x06002FDC RID: 12252 RVA: 0x000CD120 File Offset: 0x000CB320
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.highestFallSpeed = Mathf.Max(this.highestFallSpeed, this.bodyMotor ? (-this.bodyMotor.velocity.y) : 0f);
				this.stopwatch += Time.deltaTime;
				if (base.isGrounded)
				{
					this.OnHitGround(this.highestFallSpeed, this.body ? this.body.footPosition : base.gameObject.transform.position);
					return;
				}
				if (this.stopwatch >= HeadstompersFall.maxFallDuration)
				{
					this.outer.SetNextState(new HeadstompersCooldown());
					return;
				}
				if (this.bodyMotor)
				{
					Vector3 velocity = this.bodyMotor.velocity;
					if (velocity.y > -HeadstompersFall.fallSpeed)
					{
						velocity.y = Mathf.MoveTowards(velocity.y, -HeadstompersFall.fallSpeed, HeadstompersFall.accelerationY * Time.deltaTime);
					}
					if (this.seekTransform && !this.seekLost)
					{
						Vector3 normalized = (this.seekTransform.position - this.body.footPosition).normalized;
						if (Vector3.Dot(Vector3.down, normalized) >= Mathf.Cos(HeadstompersFall.seekCone * 0.017453292f))
						{
							if (velocity.y < 0f)
							{
								Vector3 vector = normalized * -velocity.y;
								vector.y = 0f;
								Vector3 vector2 = velocity;
								vector2.y = 0f;
								vector2 = vector;
								velocity.x = vector2.x;
								velocity.z = vector2.z;
							}
						}
						else
						{
							this.seekLost = true;
						}
					}
					this.bodyMotor.velocity = velocity;
				}
			}
		}

		// Token: 0x04002DA8 RID: 11688
		private float stopwatch;

		// Token: 0x04002DA9 RID: 11689
		public static float maxFallDuration = 0f;

		// Token: 0x04002DAA RID: 11690
		public static float fallSpeed = 30f;

		// Token: 0x04002DAB RID: 11691
		public static float accelerationY = 40f;

		// Token: 0x04002DAC RID: 11692
		public static float seekCone = 20f;

		// Token: 0x04002DAD RID: 11693
		public static float springboardSpeed = 30f;

		// Token: 0x04002DAE RID: 11694
		private Transform seekTransform;

		// Token: 0x04002DAF RID: 11695
		private GameObject seekBodyObject;

		// Token: 0x04002DB0 RID: 11696
		private bool seekLost;

		// Token: 0x04002DB1 RID: 11697
		private CharacterMotor onHitGroundProvider;

		// Token: 0x04002DB2 RID: 11698
		private float highestFallSpeed;
	}
}
