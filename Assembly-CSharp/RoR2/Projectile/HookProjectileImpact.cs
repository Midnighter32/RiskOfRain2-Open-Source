using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000539 RID: 1337
	[RequireComponent(typeof(ProjectileController))]
	public class HookProjectileImpact : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001DEA RID: 7658 RVA: 0x0008C9F8 File Offset: 0x0008ABF8
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			if (this.projectileController && this.projectileController.owner)
			{
				this.ownerTransform = this.projectileController.owner.transform;
			}
			this.liveTimer = this.maxDistance / this.reelSpeed;
		}

		// Token: 0x06001DEB RID: 7659 RVA: 0x0008CA74 File Offset: 0x0008AC74
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			EffectManager.instance.SimpleImpactEffect(this.impactSpark, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
			if (this.hookState != HookProjectileImpact.HookState.Flying)
			{
				return;
			}
			HurtBox component = impactInfo.collider.GetComponent<HurtBox>();
			if (component)
			{
				HealthComponent healthComponent = component.healthComponent;
				if (healthComponent)
				{
					TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
					TeamFilter component3 = base.GetComponent<TeamFilter>();
					if (healthComponent.gameObject == this.projectileController.owner || component2.teamIndex == component3.teamIndex)
					{
						return;
					}
					this.Networkvictim = healthComponent.gameObject;
					this.victimSetStateOnHurt = this.victim.GetComponent<SetStateOnHurt>();
					if (this.victimSetStateOnHurt)
					{
						this.victimSetStateOnHurt.SetPain();
					}
					DamageInfo damageInfo = new DamageInfo();
					if (this.projectileDamage)
					{
						damageInfo.damage = this.projectileDamage.damage;
						damageInfo.crit = this.projectileDamage.crit;
						damageInfo.attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null);
						damageInfo.inflictor = base.gameObject;
						damageInfo.position = impactInfo.estimatedPointOfImpact;
						damageInfo.force = this.projectileDamage.force * base.transform.forward;
						damageInfo.procChainMask = this.projectileController.procChainMask;
						damageInfo.procCoefficient = this.projectileController.procCoefficient;
						damageInfo.damageColorIndex = this.projectileDamage.damageColorIndex;
					}
					else
					{
						Debug.Log("No projectile damage component!");
					}
					Debug.Log(damageInfo.damage);
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					this.NetworkhookState = HookProjectileImpact.HookState.HitDelay;
					EffectManager.instance.SimpleImpactEffect(this.impactSuccess, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
				}
			}
			if (!this.victim)
			{
				this.NetworkhookState = HookProjectileImpact.HookState.ReelFail;
			}
		}

		// Token: 0x06001DEC RID: 7660 RVA: 0x0008CC98 File Offset: 0x0008AE98
		private bool Reel()
		{
			Vector3 vector = this.projectileController.owner.transform.position - this.victim.transform.position;
			Vector3 normalized = vector.normalized;
			float num = vector.magnitude;
			Collider component = this.projectileController.owner.GetComponent<Collider>();
			Collider component2 = this.victim.GetComponent<Collider>();
			if (component && component2)
			{
				num = Util.EstimateSurfaceDistance(component, component2);
			}
			bool flag = num <= 2f;
			Rigidbody rigidbody = null;
			float num2 = -1f;
			CharacterMotor component3 = this.projectileController.owner.GetComponent<CharacterMotor>();
			if (component3)
			{
				num2 = component3.mass;
			}
			else
			{
				rigidbody = this.projectileController.owner.GetComponent<Rigidbody>();
				if (rigidbody)
				{
					num2 = rigidbody.mass;
				}
			}
			Rigidbody rigidbody2 = null;
			float num3 = -1f;
			CharacterMotor component4 = this.victim.GetComponent<CharacterMotor>();
			if (component4)
			{
				num3 = component4.mass;
			}
			else
			{
				rigidbody2 = this.victim.GetComponent<Rigidbody>();
				if (rigidbody2)
				{
					num3 = rigidbody2.mass;
				}
			}
			float num4 = 0f;
			float num5 = 0f;
			if (num2 > 0f && num3 > 0f)
			{
				num4 = 1f - num2 / (num2 + num3);
				num5 = 1f - num4;
			}
			else if (num2 > 0f)
			{
				num4 = 1f;
			}
			else if (num3 > 0f)
			{
				num5 = 1f;
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				num4 = 0f;
				num5 = 0f;
			}
			Vector3 velocity = normalized * (num4 * this.ownerPullFactor * -this.reelSpeed);
			Vector3 velocity2 = normalized * (num5 * this.victimPullFactor * this.reelSpeed);
			if (component3)
			{
				component3.velocity = velocity;
			}
			if (rigidbody)
			{
				rigidbody.velocity = velocity;
			}
			if (component4)
			{
				component4.velocity = velocity2;
			}
			if (rigidbody2)
			{
				rigidbody2.velocity = velocity2;
			}
			CharacterDirection component5 = this.projectileController.owner.GetComponent<CharacterDirection>();
			CharacterDirection component6 = this.victim.GetComponent<CharacterDirection>();
			if (component5)
			{
				component5.forward = -normalized;
			}
			if (component6)
			{
				component6.forward = normalized;
			}
			return flag;
		}

		// Token: 0x06001DED RID: 7661 RVA: 0x0008CF00 File Offset: 0x0008B100
		public void FixedUpdate()
		{
			if (NetworkServer.active && !this.projectileController.owner)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (this.victim)
			{
				base.transform.position = this.victim.transform.position;
			}
			switch (this.hookState)
			{
			case HookProjectileImpact.HookState.Flying:
				if (NetworkServer.active)
				{
					this.flyTimer += Time.fixedDeltaTime;
					if (this.flyTimer >= this.liveTimer)
					{
						this.NetworkhookState = HookProjectileImpact.HookState.ReelFail;
						return;
					}
				}
				break;
			case HookProjectileImpact.HookState.HitDelay:
				if (NetworkServer.active)
				{
					if (!this.victim)
					{
						this.NetworkhookState = HookProjectileImpact.HookState.Reel;
						return;
					}
					this.delayTimer += Time.fixedDeltaTime;
					if (this.delayTimer >= this.reelDelayTime)
					{
						this.NetworkhookState = HookProjectileImpact.HookState.Reel;
						return;
					}
				}
				break;
			case HookProjectileImpact.HookState.Reel:
			{
				bool flag = true;
				if (this.victim)
				{
					flag = this.Reel();
				}
				if (NetworkServer.active)
				{
					if (!this.victim)
					{
						this.NetworkhookState = HookProjectileImpact.HookState.ReelFail;
					}
					if (flag)
					{
						if (this.victimSetStateOnHurt)
						{
							this.victimSetStateOnHurt.SetPain();
						}
						UnityEngine.Object.Destroy(base.gameObject);
						return;
					}
				}
				break;
			}
			case HookProjectileImpact.HookState.ReelFail:
				if (NetworkServer.active)
				{
					if (this.rigidbody)
					{
						this.rigidbody.isKinematic = true;
					}
					this.ownerTransform = this.projectileController.owner.transform;
					if (this.ownerTransform)
					{
						base.transform.position = Vector3.MoveTowards(base.transform.position, this.ownerTransform.position, this.reelSpeed * Time.fixedDeltaTime);
						if (base.transform.position == this.ownerTransform.position)
						{
							UnityEngine.Object.Destroy(base.gameObject);
						}
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06001DEF RID: 7663 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06001DF0 RID: 7664 RVA: 0x0008D120 File Offset: 0x0008B320
		// (set) Token: 0x06001DF1 RID: 7665 RVA: 0x0008D133 File Offset: 0x0008B333
		public HookProjectileImpact.HookState NetworkhookState
		{
			get
			{
				return this.hookState;
			}
			set
			{
				base.SetSyncVar<HookProjectileImpact.HookState>(value, ref this.hookState, 1u);
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06001DF2 RID: 7666 RVA: 0x0008D148 File Offset: 0x0008B348
		// (set) Token: 0x06001DF3 RID: 7667 RVA: 0x0008D15B File Offset: 0x0008B35B
		public GameObject Networkvictim
		{
			get
			{
				return this.victim;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.victim, 2u, ref this.___victimNetId);
			}
		}

		// Token: 0x06001DF4 RID: 7668 RVA: 0x0008D178 File Offset: 0x0008B378
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.hookState);
				writer.Write(this.victim);
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
				writer.Write((int)this.hookState);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.victim);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001DF5 RID: 7669 RVA: 0x0008D224 File Offset: 0x0008B424
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.hookState = (HookProjectileImpact.HookState)reader.ReadInt32();
				this.___victimNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.hookState = (HookProjectileImpact.HookState)reader.ReadInt32();
			}
			if ((num & 2) != 0)
			{
				this.victim = reader.ReadGameObject();
			}
		}

		// Token: 0x06001DF6 RID: 7670 RVA: 0x0008D28A File Offset: 0x0008B48A
		public override void PreStartClient()
		{
			if (!this.___victimNetId.IsEmpty())
			{
				this.Networkvictim = ClientScene.FindLocalObject(this.___victimNetId);
			}
		}

		// Token: 0x0400203C RID: 8252
		private ProjectileController projectileController;

		// Token: 0x0400203D RID: 8253
		public float reelDelayTime;

		// Token: 0x0400203E RID: 8254
		public float reelSpeed = 40f;

		// Token: 0x0400203F RID: 8255
		public float ownerPullFactor = 1f;

		// Token: 0x04002040 RID: 8256
		public float victimPullFactor = 1f;

		// Token: 0x04002041 RID: 8257
		public float maxDistance;

		// Token: 0x04002042 RID: 8258
		public GameObject impactSpark;

		// Token: 0x04002043 RID: 8259
		public GameObject impactSuccess;

		// Token: 0x04002044 RID: 8260
		[SyncVar]
		private HookProjectileImpact.HookState hookState;

		// Token: 0x04002045 RID: 8261
		[SyncVar]
		private GameObject victim;

		// Token: 0x04002046 RID: 8262
		private SetStateOnHurt victimSetStateOnHurt;

		// Token: 0x04002047 RID: 8263
		private Transform ownerTransform;

		// Token: 0x04002048 RID: 8264
		private ProjectileDamage projectileDamage;

		// Token: 0x04002049 RID: 8265
		private Rigidbody rigidbody;

		// Token: 0x0400204A RID: 8266
		private float liveTimer;

		// Token: 0x0400204B RID: 8267
		private float delayTimer;

		// Token: 0x0400204C RID: 8268
		private float flyTimer;

		// Token: 0x0400204D RID: 8269
		private NetworkInstanceId ___victimNetId;

		// Token: 0x0200053A RID: 1338
		private enum HookState
		{
			// Token: 0x0400204F RID: 8271
			Flying,
			// Token: 0x04002050 RID: 8272
			HitDelay,
			// Token: 0x04002051 RID: 8273
			Reel,
			// Token: 0x04002052 RID: 8274
			ReelFail
		}
	}
}
