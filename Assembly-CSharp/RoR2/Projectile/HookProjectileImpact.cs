using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x020004F4 RID: 1268
	[RequireComponent(typeof(ProjectileController))]
	public class HookProjectileImpact : NetworkBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E2F RID: 7727 RVA: 0x00081F68 File Offset: 0x00080168
		private void Start()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.ownerTransform = this.projectileController.owner.transform;
			if (this.ownerTransform)
			{
				ModelLocator component = this.ownerTransform.GetComponent<ModelLocator>();
				if (component)
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform)
					{
						ChildLocator component2 = modelTransform.GetComponent<ChildLocator>();
						if (component2)
						{
							this.ownerTransform = component2.FindChild(this.attachmentString);
						}
					}
				}
			}
		}

		// Token: 0x06001E30 RID: 7728 RVA: 0x00082000 File Offset: 0x00080200
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			EffectManager.SimpleImpactEffect(this.impactSpark, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
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
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					this.NetworkhookState = HookProjectileImpact.HookState.HitDelay;
					EffectManager.SimpleImpactEffect(this.impactSuccess, impactInfo.estimatedPointOfImpact, -base.transform.forward, true);
					base.gameObject.layer = LayerIndex.noCollision.intVal;
				}
			}
			if (!this.victim)
			{
				this.NetworkhookState = HookProjectileImpact.HookState.ReelFail;
			}
		}

		// Token: 0x06001E31 RID: 7729 RVA: 0x000821E8 File Offset: 0x000803E8
		private bool Reel()
		{
			Vector3 vector = this.ownerTransform.position - this.victim.transform.position;
			Vector3 normalized = vector.normalized;
			float num = vector.magnitude;
			Collider component = this.projectileController.owner.GetComponent<Collider>();
			Collider component2 = this.victim.GetComponent<Collider>();
			if (component && component2)
			{
				num = Util.EstimateSurfaceDistance(component, component2);
			}
			bool flag = num <= this.pullMinimumDistance;
			float num2 = -1f;
			CharacterMotor component3 = this.projectileController.owner.GetComponent<CharacterMotor>();
			if (component3)
			{
				num2 = component3.mass;
			}
			else
			{
				Rigidbody component4 = this.projectileController.owner.GetComponent<Rigidbody>();
				if (component4)
				{
					num2 = component4.mass;
				}
			}
			Rigidbody rigidbody = null;
			float num3 = -1f;
			CharacterMotor component5 = this.victim.GetComponent<CharacterMotor>();
			if (component5)
			{
				num3 = component5.mass;
			}
			else
			{
				rigidbody = this.victim.GetComponent<Rigidbody>();
				if (rigidbody)
				{
					num3 = rigidbody.mass;
				}
			}
			float num4 = 0f;
			if (num2 > 0f && num3 > 0f)
			{
				float num5 = 1f - num2 / (num2 + num3);
				num4 = 1f - num5;
			}
			else if (num2 <= 0f)
			{
				if (num3 > 0f)
				{
					num4 = 1f;
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				num4 = 0f;
			}
			Vector3 velocity = normalized * (num4 * this.victimPullFactor * this.reelSpeed);
			if (component5)
			{
				component5.velocity = velocity;
			}
			if (rigidbody)
			{
				rigidbody.velocity = velocity;
			}
			return flag;
		}

		// Token: 0x06001E32 RID: 7730 RVA: 0x000823C0 File Offset: 0x000805C0
		public void FixedUpdate()
		{
			if (NetworkServer.active && !this.projectileController.owner)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (this.victim)
			{
				this.rigidbody.MovePosition(this.victim.transform.position);
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
						this.rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
						this.rigidbody.isKinematic = true;
					}
					if (this.ownerTransform)
					{
						this.rigidbody.MovePosition(Vector3.MoveTowards(base.transform.position, this.ownerTransform.position, this.reelSpeed * Time.fixedDeltaTime));
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

		// Token: 0x06001E34 RID: 7732 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06001E35 RID: 7733 RVA: 0x000825BC File Offset: 0x000807BC
		// (set) Token: 0x06001E36 RID: 7734 RVA: 0x000825CF File Offset: 0x000807CF
		public HookProjectileImpact.HookState NetworkhookState
		{
			get
			{
				return this.hookState;
			}
			[param: In]
			set
			{
				base.SetSyncVar<HookProjectileImpact.HookState>(value, ref this.hookState, 1U);
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06001E37 RID: 7735 RVA: 0x000825E4 File Offset: 0x000807E4
		// (set) Token: 0x06001E38 RID: 7736 RVA: 0x000825F7 File Offset: 0x000807F7
		public GameObject Networkvictim
		{
			get
			{
				return this.victim;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.victim, 2U, ref this.___victimNetId);
			}
		}

		// Token: 0x06001E39 RID: 7737 RVA: 0x00082614 File Offset: 0x00080814
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write((int)this.hookState);
				writer.Write(this.victim);
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
				writer.Write((int)this.hookState);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
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

		// Token: 0x06001E3A RID: 7738 RVA: 0x000826C0 File Offset: 0x000808C0
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

		// Token: 0x06001E3B RID: 7739 RVA: 0x00082726 File Offset: 0x00080926
		public override void PreStartClient()
		{
			if (!this.___victimNetId.IsEmpty())
			{
				this.Networkvictim = ClientScene.FindLocalObject(this.___victimNetId);
			}
		}

		// Token: 0x04001B5A RID: 7002
		private ProjectileController projectileController;

		// Token: 0x04001B5B RID: 7003
		public float reelDelayTime;

		// Token: 0x04001B5C RID: 7004
		public float reelSpeed = 40f;

		// Token: 0x04001B5D RID: 7005
		public string attachmentString;

		// Token: 0x04001B5E RID: 7006
		public float victimPullFactor = 1f;

		// Token: 0x04001B5F RID: 7007
		public float pullMinimumDistance = 10f;

		// Token: 0x04001B60 RID: 7008
		public GameObject impactSpark;

		// Token: 0x04001B61 RID: 7009
		public GameObject impactSuccess;

		// Token: 0x04001B62 RID: 7010
		[SyncVar]
		private HookProjectileImpact.HookState hookState;

		// Token: 0x04001B63 RID: 7011
		[SyncVar]
		private GameObject victim;

		// Token: 0x04001B64 RID: 7012
		private Transform ownerTransform;

		// Token: 0x04001B65 RID: 7013
		private ProjectileDamage projectileDamage;

		// Token: 0x04001B66 RID: 7014
		private Rigidbody rigidbody;

		// Token: 0x04001B67 RID: 7015
		public float liveTimer;

		// Token: 0x04001B68 RID: 7016
		private float delayTimer;

		// Token: 0x04001B69 RID: 7017
		private float flyTimer;

		// Token: 0x04001B6A RID: 7018
		private NetworkInstanceId ___victimNetId;

		// Token: 0x020004F5 RID: 1269
		private enum HookState
		{
			// Token: 0x04001B6C RID: 7020
			Flying,
			// Token: 0x04001B6D RID: 7021
			HitDelay,
			// Token: 0x04001B6E RID: 7022
			Reel,
			// Token: 0x04001B6F RID: 7023
			ReelFail
		}
	}
}
