using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x020004EF RID: 1263
	[RequireComponent(typeof(ProjectileController))]
	public class BoomerangProjectile : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E0C RID: 7692 RVA: 0x0008153C File Offset: 0x0007F73C
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			if (this.projectileController && this.projectileController.owner)
			{
				this.ownerTransform = this.projectileController.owner.transform;
			}
		}

		// Token: 0x06001E0D RID: 7693 RVA: 0x000815A4 File Offset: 0x0007F7A4
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			HurtBox component = impactInfo.collider.GetComponent<HurtBox>();
			if (component)
			{
				HealthComponent healthComponent = component.healthComponent;
				if (healthComponent)
				{
					if (!this.canHitCharacters)
					{
						return;
					}
					TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
					TeamFilter component3 = base.GetComponent<TeamFilter>();
					if (healthComponent.gameObject == this.projectileController.owner || component2.teamIndex == component3.teamIndex)
					{
						return;
					}
					GameObject gameObject = healthComponent.gameObject;
					DamageInfo damageInfo = new DamageInfo();
					if (this.projectileDamage)
					{
						damageInfo.damage = this.projectileDamage.damage * this.damageCoefficient;
						damageInfo.crit = this.projectileDamage.crit;
						damageInfo.attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null);
						damageInfo.inflictor = base.gameObject;
						damageInfo.position = impactInfo.estimatedPointOfImpact;
						damageInfo.force = this.projectileDamage.force * base.transform.forward;
						damageInfo.procChainMask = this.projectileController.procChainMask;
						damageInfo.procCoefficient = this.projectileController.procCoefficient;
						damageInfo.damageColorIndex = this.projectileDamage.damageColorIndex;
						damageInfo.damageType = this.projectileDamage.damageType;
					}
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					this.NetworkboomerangState = BoomerangProjectile.BoomerangState.FlyBack;
				}
			}
			if (!this.canHitWorld)
			{
				return;
			}
			this.NetworkboomerangState = BoomerangProjectile.BoomerangState.FlyBack;
			EffectManager.SimpleImpactEffect(this.impactSpark, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
		}

		// Token: 0x06001E0E RID: 7694 RVA: 0x00081764 File Offset: 0x0007F964
		private bool Reel()
		{
			Vector3 vector = this.projectileController.owner.transform.position - base.transform.position;
			Vector3 normalized = vector.normalized;
			return vector.magnitude <= 2f;
		}

		// Token: 0x06001E0F RID: 7695 RVA: 0x000817B0 File Offset: 0x0007F9B0
		public void FixedUpdate()
		{
			if (NetworkServer.active && !this.projectileController.owner)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			switch (this.boomerangState)
			{
			case BoomerangProjectile.BoomerangState.FlyOut:
				if (NetworkServer.active)
				{
					this.rigidbody.velocity = this.travelSpeed * base.transform.forward;
					this.stopwatch += Time.fixedDeltaTime;
					if (this.stopwatch >= this.maxFlyStopwatch)
					{
						this.stopwatch = 0f;
						this.NetworkboomerangState = BoomerangProjectile.BoomerangState.Transition;
						return;
					}
				}
				break;
			case BoomerangProjectile.BoomerangState.Transition:
			{
				this.stopwatch += Time.fixedDeltaTime;
				float num = this.stopwatch / this.transitionDuration;
				Vector3 a = this.CalculatePullDirection();
				this.rigidbody.velocity = Vector3.Lerp(this.travelSpeed * base.transform.forward, this.travelSpeed * a, num);
				if (num >= 1f)
				{
					this.NetworkboomerangState = BoomerangProjectile.BoomerangState.FlyBack;
					return;
				}
				break;
			}
			case BoomerangProjectile.BoomerangState.FlyBack:
			{
				bool flag = this.Reel();
				if (NetworkServer.active)
				{
					Vector3 a2 = this.CalculatePullDirection();
					this.rigidbody.velocity = this.travelSpeed * a2;
					if (flag)
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x00081904 File Offset: 0x0007FB04
		private Vector3 CalculatePullDirection()
		{
			if (this.projectileController.owner)
			{
				return (this.projectileController.owner.transform.position - base.transform.position).normalized;
			}
			return base.transform.forward;
		}

		// Token: 0x06001E12 RID: 7698 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06001E13 RID: 7699 RVA: 0x00081970 File Offset: 0x0007FB70
		// (set) Token: 0x06001E14 RID: 7700 RVA: 0x00081983 File Offset: 0x0007FB83
		public BoomerangProjectile.BoomerangState NetworkboomerangState
		{
			get
			{
				return this.boomerangState;
			}
			[param: In]
			set
			{
				base.SetSyncVar<BoomerangProjectile.BoomerangState>(value, ref this.boomerangState, 1U);
			}
		}

		// Token: 0x06001E15 RID: 7701 RVA: 0x00081998 File Offset: 0x0007FB98
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.boomerangState);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write((int)this.boomerangState);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001E16 RID: 7702 RVA: 0x00081A04 File Offset: 0x0007FC04
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.boomerangState = (BoomerangProjectile.BoomerangState)reader.ReadInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.boomerangState = (BoomerangProjectile.BoomerangState)reader.ReadInt32();
			}
		}

		// Token: 0x04001B36 RID: 6966
		public float travelSpeed = 40f;

		// Token: 0x04001B37 RID: 6967
		public float transitionDuration;

		// Token: 0x04001B38 RID: 6968
		public float maxFlyStopwatch;

		// Token: 0x04001B39 RID: 6969
		public GameObject impactSpark;

		// Token: 0x04001B3A RID: 6970
		public float damageCoefficient;

		// Token: 0x04001B3B RID: 6971
		public bool canHitCharacters;

		// Token: 0x04001B3C RID: 6972
		public bool canHitWorld;

		// Token: 0x04001B3D RID: 6973
		private ProjectileController projectileController;

		// Token: 0x04001B3E RID: 6974
		[SyncVar]
		private BoomerangProjectile.BoomerangState boomerangState;

		// Token: 0x04001B3F RID: 6975
		private Transform ownerTransform;

		// Token: 0x04001B40 RID: 6976
		private ProjectileDamage projectileDamage;

		// Token: 0x04001B41 RID: 6977
		private Rigidbody rigidbody;

		// Token: 0x04001B42 RID: 6978
		private float stopwatch;

		// Token: 0x020004F0 RID: 1264
		private enum BoomerangState
		{
			// Token: 0x04001B44 RID: 6980
			FlyOut,
			// Token: 0x04001B45 RID: 6981
			Transition,
			// Token: 0x04001B46 RID: 6982
			FlyBack
		}
	}
}
