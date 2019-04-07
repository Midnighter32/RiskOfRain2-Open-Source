using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000535 RID: 1333
	[RequireComponent(typeof(ProjectileController))]
	public class BoomerangProjectile : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001DD7 RID: 7639 RVA: 0x0008C24C File Offset: 0x0008A44C
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

		// Token: 0x06001DD8 RID: 7640 RVA: 0x0008C2B4 File Offset: 0x0008A4B4
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
			EffectManager.instance.SimpleImpactEffect(this.impactSpark, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x0008C47C File Offset: 0x0008A67C
		private bool Reel()
		{
			Vector3 vector = this.projectileController.owner.transform.position - base.transform.position;
			Vector3 normalized = vector.normalized;
			return vector.magnitude <= 2f;
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x0008C4C8 File Offset: 0x0008A6C8
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

		// Token: 0x06001DDB RID: 7643 RVA: 0x0008C61C File Offset: 0x0008A81C
		private Vector3 CalculatePullDirection()
		{
			if (this.projectileController.owner)
			{
				return (this.projectileController.owner.transform.position - base.transform.position).normalized;
			}
			return base.transform.forward;
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06001DDE RID: 7646 RVA: 0x0008C688 File Offset: 0x0008A888
		// (set) Token: 0x06001DDF RID: 7647 RVA: 0x0008C69B File Offset: 0x0008A89B
		public BoomerangProjectile.BoomerangState NetworkboomerangState
		{
			get
			{
				return this.boomerangState;
			}
			set
			{
				base.SetSyncVar<BoomerangProjectile.BoomerangState>(value, ref this.boomerangState, 1u);
			}
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x0008C6B0 File Offset: 0x0008A8B0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.boomerangState);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
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

		// Token: 0x06001DE1 RID: 7649 RVA: 0x0008C71C File Offset: 0x0008A91C
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

		// Token: 0x0400201D RID: 8221
		public float travelSpeed = 40f;

		// Token: 0x0400201E RID: 8222
		public float transitionDuration;

		// Token: 0x0400201F RID: 8223
		public float maxFlyStopwatch;

		// Token: 0x04002020 RID: 8224
		public GameObject impactSpark;

		// Token: 0x04002021 RID: 8225
		public float damageCoefficient;

		// Token: 0x04002022 RID: 8226
		public bool canHitCharacters;

		// Token: 0x04002023 RID: 8227
		public bool canHitWorld;

		// Token: 0x04002024 RID: 8228
		private ProjectileController projectileController;

		// Token: 0x04002025 RID: 8229
		[SyncVar]
		private BoomerangProjectile.BoomerangState boomerangState;

		// Token: 0x04002026 RID: 8230
		private Transform ownerTransform;

		// Token: 0x04002027 RID: 8231
		private ProjectileDamage projectileDamage;

		// Token: 0x04002028 RID: 8232
		private Rigidbody rigidbody;

		// Token: 0x04002029 RID: 8233
		private float stopwatch;

		// Token: 0x02000536 RID: 1334
		private enum BoomerangState
		{
			// Token: 0x0400202B RID: 8235
			FlyOut,
			// Token: 0x0400202C RID: 8236
			Transition,
			// Token: 0x0400202D RID: 8237
			FlyBack
		}
	}
}
