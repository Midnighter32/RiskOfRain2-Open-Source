using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.LaserTurbine
{
	// Token: 0x020007FD RID: 2045
	public class FireMainBeamState : LaserTurbineBaseState
	{
		// Token: 0x06002E84 RID: 11908 RVA: 0x000C5A00 File Offset: 0x000C3C00
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				this.initialAimRay = base.GetAimRay();
			}
			if (NetworkServer.active)
			{
				base.laserTurbineController.ExpendCharge();
				this.isCrit = base.ownerBody.RollCrit();
				this.FireBeamServer(this.initialAimRay, FireMainBeamState.forwardBeamTracerEffect, FireMainBeamState.mainBeamMaxDistance, true);
			}
			base.laserTurbineController.showTurbineDisplay = false;
		}

		// Token: 0x06002E85 RID: 11909 RVA: 0x000C5A6D File Offset: 0x000C3C6D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= FireMainBeamState.baseDuration)
			{
				this.outer.SetNextState(new RechargeState());
			}
		}

		// Token: 0x06002E86 RID: 11910 RVA: 0x000C5A9C File Offset: 0x000C3C9C
		public override void OnExit()
		{
			if (NetworkServer.active && !this.outer.destroying)
			{
				Vector3 direction = this.initialAimRay.origin - this.beamHitPosition;
				Ray aimRay = new Ray(this.beamHitPosition, direction);
				this.FireBeamServer(aimRay, FireMainBeamState.backwardBeamTracerEffect, direction.magnitude, false);
			}
			base.laserTurbineController.showTurbineDisplay = true;
			base.OnExit();
		}

		// Token: 0x06002E87 RID: 11911 RVA: 0x000C5B08 File Offset: 0x000C3D08
		private void FireBeamServer(Ray aimRay, GameObject tracerEffectPrefab, float maxDistance, bool isInitialBeam)
		{
			bool didHit = false;
			BulletAttack bulletAttack = new BulletAttack
			{
				origin = aimRay.origin,
				aimVector = aimRay.direction,
				bulletCount = 1U,
				damage = base.GetDamage() * FireMainBeamState.mainBeamDamageCoefficient,
				damageColorIndex = DamageColorIndex.Item,
				damageType = DamageType.Generic,
				falloffModel = BulletAttack.FalloffModel.None,
				force = FireMainBeamState.mainBeamForce,
				hitEffectPrefab = FireMainBeamState.mainBeamImpactEffect,
				HitEffectNormal = false,
				hitMask = LayerIndex.entityPrecise.mask,
				isCrit = this.isCrit,
				maxDistance = maxDistance,
				minSpread = 0f,
				maxSpread = 0f,
				muzzleName = "",
				owner = base.ownerBody.gameObject,
				procChainMask = default(ProcChainMask),
				procCoefficient = FireMainBeamState.mainBeamProcCoefficient,
				queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
				radius = FireMainBeamState.mainBeamRadius,
				smartCollision = true,
				sniper = false,
				spreadPitchScale = 1f,
				spreadYawScale = 1f,
				stopperMask = LayerIndex.world.mask,
				tracerEffectPrefab = (isInitialBeam ? tracerEffectPrefab : null),
				weapon = base.gameObject
			};
			TeamIndex teamIndex = base.ownerBody.teamComponent.teamIndex;
			bulletAttack.hitCallback = delegate(ref BulletAttack.BulletHit info)
			{
				bool flag = bulletAttack.DefaultHitCallback(ref info);
				if (!isInitialBeam)
				{
					return true;
				}
				if (flag)
				{
					HealthComponent healthComponent = info.hitHurtBox ? info.hitHurtBox.healthComponent : null;
					if (healthComponent && healthComponent.alive && info.hitHurtBox.teamIndex != teamIndex)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					didHit = true;
					this.beamHitPosition = info.point;
				}
				return flag;
			};
			bulletAttack.Fire();
			if (!didHit)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(aimRay, out raycastHit, FireMainBeamState.mainBeamMaxDistance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					didHit = true;
					this.beamHitPosition = raycastHit.point;
				}
				else
				{
					this.beamHitPosition = aimRay.GetPoint(FireMainBeamState.mainBeamMaxDistance);
				}
			}
			if (didHit & isInitialBeam)
			{
				FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
				fireProjectileInfo.projectilePrefab = FireMainBeamState.secondBombPrefab;
				fireProjectileInfo.owner = base.ownerBody.gameObject;
				fireProjectileInfo.position = this.beamHitPosition - aimRay.direction * 0.5f;
				fireProjectileInfo.rotation = Quaternion.identity;
				fireProjectileInfo.damage = base.GetDamage() * FireMainBeamState.secondBombDamageCoefficient;
				fireProjectileInfo.damageColorIndex = DamageColorIndex.Item;
				fireProjectileInfo.crit = this.isCrit;
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
			if (!isInitialBeam)
			{
				EffectData effectData = new EffectData
				{
					origin = aimRay.origin,
					start = base.transform.position
				};
				effectData.SetNetworkedObjectReference(base.gameObject);
				EffectManager.SpawnEffect(tracerEffectPrefab, effectData, true);
			}
		}

		// Token: 0x06002E88 RID: 11912 RVA: 0x000C5DE0 File Offset: 0x000C3FE0
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			Vector3 origin = this.initialAimRay.origin;
			Vector3 direction = this.initialAimRay.direction;
			writer.Write(origin);
			writer.Write(direction);
		}

		// Token: 0x06002E89 RID: 11913 RVA: 0x000C5E1C File Offset: 0x000C401C
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			Vector3 origin = reader.ReadVector3();
			Vector3 direction = reader.ReadVector3();
			this.initialAimRay = new Ray(origin, direction);
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06002E8A RID: 11914 RVA: 0x0000B933 File Offset: 0x00009B33
		protected override bool shouldFollow
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04002B95 RID: 11157
		public static float baseDuration;

		// Token: 0x04002B96 RID: 11158
		public static float mainBeamDamageCoefficient;

		// Token: 0x04002B97 RID: 11159
		public static float mainBeamProcCoefficient;

		// Token: 0x04002B98 RID: 11160
		public static float mainBeamForce;

		// Token: 0x04002B99 RID: 11161
		public static float mainBeamRadius;

		// Token: 0x04002B9A RID: 11162
		public static float mainBeamMaxDistance;

		// Token: 0x04002B9B RID: 11163
		public static GameObject forwardBeamTracerEffect;

		// Token: 0x04002B9C RID: 11164
		public static GameObject backwardBeamTracerEffect;

		// Token: 0x04002B9D RID: 11165
		public static GameObject mainBeamImpactEffect;

		// Token: 0x04002B9E RID: 11166
		public static GameObject secondBombPrefab;

		// Token: 0x04002B9F RID: 11167
		public static float secondBombDamageCoefficient;

		// Token: 0x04002BA0 RID: 11168
		private Ray initialAimRay;

		// Token: 0x04002BA1 RID: 11169
		private Vector3 beamHitPosition;

		// Token: 0x04002BA2 RID: 11170
		private bool isCrit;
	}
}
