using System;
using System.Linq;
using RoR2;
using UnityEngine;

namespace EntityStates.Headstompers
{
	// Token: 0x02000160 RID: 352
	public class HeadstompersFall : BaseHeadstompersState
	{
		// Token: 0x060006D2 RID: 1746 RVA: 0x00020868 File Offset: 0x0001EA68
		public override void OnEnter()
		{
			base.OnEnter();
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

		// Token: 0x060006D3 RID: 1747 RVA: 0x0002095C File Offset: 0x0001EB5C
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

		// Token: 0x060006D4 RID: 1748 RVA: 0x000209AE File Offset: 0x0001EBAE
		public override void OnExit()
		{
			this.SetOnHitGroundProvider(null);
			base.OnExit();
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x000209BD File Offset: 0x0001EBBD
		private void OnMotorHitGround(ref CharacterMotor.HitGroundInfo hitGroundInfo)
		{
			this.OnHitGround(-hitGroundInfo.velocity.y, hitGroundInfo.position);
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x000209D7 File Offset: 0x0001EBD7
		private void OnHitGround(float impactSpeed, Vector3 position)
		{
			this.outer.SetNextState(new HeadstompersCooldown
			{
				impactSpeed = impactSpeed,
				impactPosition = position
			});
			this.SetOnHitGroundProvider(null);
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x00020A00 File Offset: 0x0001EC00
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.stopwatch += Time.deltaTime;
				if (base.isGrounded)
				{
					float impactSpeed = this.bodyMotor ? (-this.bodyMotor.velocity.y) : 0f;
					this.OnHitGround(impactSpeed, this.body ? this.body.footPosition : base.gameObject.transform.position);
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

		// Token: 0x04000862 RID: 2146
		private float stopwatch;

		// Token: 0x04000863 RID: 2147
		public static float maxFallDuration = 0f;

		// Token: 0x04000864 RID: 2148
		public static float fallSpeed = 30f;

		// Token: 0x04000865 RID: 2149
		public static float accelerationY = 40f;

		// Token: 0x04000866 RID: 2150
		public static float seekCone = 20f;

		// Token: 0x04000867 RID: 2151
		public static float springboardSpeed = 30f;

		// Token: 0x04000868 RID: 2152
		private Transform seekTransform;

		// Token: 0x04000869 RID: 2153
		private GameObject seekBodyObject;

		// Token: 0x0400086A RID: 2154
		private bool seekLost;

		// Token: 0x0400086B RID: 2155
		private CharacterMotor onHitGroundProvider;
	}
}
