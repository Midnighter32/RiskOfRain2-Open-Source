using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020000D4 RID: 212
	public class BulletAttack
	{
		// Token: 0x06000421 RID: 1057 RVA: 0x00010A44 File Offset: 0x0000EC44
		public BulletAttack()
		{
			this.filterCallback = new BulletAttack.FilterCallback(this.DefaultFilterCallback);
			this.hitCallback = new BulletAttack.HitCallback(this.DefaultHitCallback);
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000422 RID: 1058 RVA: 0x00010AFA File Offset: 0x0000ECFA
		// (set) Token: 0x06000423 RID: 1059 RVA: 0x00010B02 File Offset: 0x0000ED02
		public Vector3 aimVector
		{
			get
			{
				return this._aimVector;
			}
			set
			{
				this._aimVector = value;
				this._aimVector.Normalize();
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000424 RID: 1060 RVA: 0x00010B16 File Offset: 0x0000ED16
		// (set) Token: 0x06000425 RID: 1061 RVA: 0x00010B1E File Offset: 0x0000ED1E
		public float maxDistance
		{
			get
			{
				return this._maxDistance;
			}
			set
			{
				if (!float.IsInfinity(value) && !float.IsNaN(value))
				{
					this._maxDistance = value;
					return;
				}
				Debug.LogFormat("BulletAttack.maxDistance was assigned a value other than a finite number. value={0}", new object[]
				{
					value
				});
			}
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00010B54 File Offset: 0x0000ED54
		public bool DefaultHitCallback(ref BulletAttack.BulletHit hitInfo)
		{
			bool result = false;
			if (hitInfo.collider)
			{
				result = ((1 << hitInfo.collider.gameObject.layer & this.stopperMask) == 0);
			}
			if (this.hitEffectPrefab)
			{
				EffectManager.SimpleImpactEffect(this.hitEffectPrefab, hitInfo.point, this.HitEffectNormal ? hitInfo.surfaceNormal : (-hitInfo.direction), true);
			}
			if (hitInfo.collider)
			{
				SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(hitInfo.collider, hitInfo.point);
				if (objectSurfaceDef && objectSurfaceDef.impactEffectPrefab)
				{
					EffectData effectData = new EffectData
					{
						origin = hitInfo.point,
						rotation = Quaternion.LookRotation(hitInfo.surfaceNormal),
						color = objectSurfaceDef.approximateColor,
						surfaceDefIndex = objectSurfaceDef.surfaceDefIndex
					};
					EffectManager.SpawnEffect(objectSurfaceDef.impactEffectPrefab, effectData, true);
				}
			}
			if (this.isCrit)
			{
				EffectManager.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/Critspark"), hitInfo.point, this.HitEffectNormal ? hitInfo.surfaceNormal : (-hitInfo.direction), true);
			}
			GameObject entityObject = hitInfo.entityObject;
			if (entityObject)
			{
				float num = 1f;
				switch (this.falloffModel)
				{
				case BulletAttack.FalloffModel.None:
					num = 1f;
					break;
				case BulletAttack.FalloffModel.DefaultBullet:
					num = 0.5f + Mathf.Clamp01(Mathf.InverseLerp(60f, 25f, hitInfo.distance)) * 0.5f;
					break;
				case BulletAttack.FalloffModel.Buckshot:
					num = 0.25f + Mathf.Clamp01(Mathf.InverseLerp(25f, 7f, hitInfo.distance)) * 0.75f;
					break;
				}
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = this.damage * num;
				damageInfo.crit = this.isCrit;
				damageInfo.attacker = this.owner;
				damageInfo.inflictor = this.weapon;
				damageInfo.position = hitInfo.point;
				damageInfo.force = hitInfo.direction * (this.force * num);
				damageInfo.procChainMask = this.procChainMask;
				damageInfo.procCoefficient = this.procCoefficient;
				damageInfo.damageType = this.damageType;
				damageInfo.damageColorIndex = this.damageColorIndex;
				damageInfo.ModifyDamageInfo(hitInfo.damageModifier);
				TeamIndex teamIndex = TeamIndex.Neutral;
				if (this.owner)
				{
					TeamComponent component = this.owner.GetComponent<TeamComponent>();
					if (component)
					{
						teamIndex = component.teamIndex;
					}
				}
				TeamIndex teamIndex2 = TeamIndex.Neutral;
				TeamComponent component2 = hitInfo.entityObject.GetComponent<TeamComponent>();
				if (component2)
				{
					teamIndex2 = component2.teamIndex;
				}
				bool flag = teamIndex == teamIndex2;
				HealthComponent healthComponent = null;
				if (!flag)
				{
					healthComponent = entityObject.GetComponent<HealthComponent>();
				}
				if (NetworkServer.active)
				{
					if (healthComponent)
					{
						healthComponent.TakeDamage(damageInfo);
						GlobalEventManager.instance.OnHitEnemy(damageInfo, hitInfo.entityObject);
					}
					GlobalEventManager.instance.OnHitAll(damageInfo, hitInfo.entityObject);
				}
				else if (ClientScene.ready)
				{
					BulletAttack.messageWriter.StartMessage(53);
					BulletAttack.messageWriter.Write(entityObject);
					BulletAttack.messageWriter.Write(damageInfo);
					BulletAttack.messageWriter.Write(healthComponent != null);
					BulletAttack.messageWriter.FinishMessage();
					ClientScene.readyConnection.SendWriter(BulletAttack.messageWriter, QosChannelIndex.defaultReliable.intVal);
				}
			}
			return result;
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00010EC8 File Offset: 0x0000F0C8
		public bool DefaultFilterCallback(ref BulletAttack.BulletHit hitInfo)
		{
			HurtBox component = hitInfo.collider.GetComponent<HurtBox>();
			return (!component || !component.healthComponent || !(component.healthComponent.gameObject == this.weapon)) && hitInfo.entityObject != this.weapon;
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00010F24 File Offset: 0x0000F124
		private void InitBulletHitFromOriginHit(ref BulletAttack.BulletHit bulletHit, Vector3 direction, Collider hitCollider)
		{
			bulletHit.direction = direction;
			bulletHit.point = this.origin;
			bulletHit.surfaceNormal = -direction;
			bulletHit.distance = 0f;
			bulletHit.collider = hitCollider;
			HurtBox component = bulletHit.collider.GetComponent<HurtBox>();
			bulletHit.hitHurtBox = component;
			bulletHit.entityObject = ((component && component.healthComponent) ? component.healthComponent.gameObject : bulletHit.collider.gameObject);
			bulletHit.damageModifier = (component ? component.damageModifier : HurtBox.DamageModifier.Normal);
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x00010FC0 File Offset: 0x0000F1C0
		private void InitBulletHitFromRaycastHit(ref BulletAttack.BulletHit bulletHit, Vector3 origin, Vector3 direction, ref RaycastHit raycastHit)
		{
			bulletHit.direction = direction;
			bulletHit.point = raycastHit.point;
			bulletHit.surfaceNormal = raycastHit.normal;
			bulletHit.distance = raycastHit.distance;
			bulletHit.collider = raycastHit.collider;
			bulletHit.point = ((bulletHit.distance == 0f) ? origin : raycastHit.point);
			HurtBox component = bulletHit.collider.GetComponent<HurtBox>();
			bulletHit.hitHurtBox = component;
			bulletHit.entityObject = ((component && component.healthComponent) ? component.healthComponent.gameObject : bulletHit.collider.gameObject);
			bulletHit.damageModifier = (component ? component.damageModifier : HurtBox.DamageModifier.Normal);
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00011082 File Offset: 0x0000F282
		private bool ProcessHit(ref BulletAttack.BulletHit hitInfo)
		{
			if (this.sniper && hitInfo.damageModifier == HurtBox.DamageModifier.SniperTarget)
			{
				hitInfo.damageModifier = HurtBox.DamageModifier.Weak;
			}
			return !this.filterCallback(ref hitInfo) || this.hitCallback(ref hitInfo);
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x000110B8 File Offset: 0x0000F2B8
		private GameObject ProcessHitList(List<BulletAttack.BulletHit> hits, ref Vector3 endPosition, List<GameObject> ignoreList)
		{
			int count = hits.Count;
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = i;
			}
			for (int j = 0; j < count; j++)
			{
				float num = this.maxDistance;
				int num2 = j;
				for (int k = j; k < count; k++)
				{
					int index = array[k];
					if (hits[index].distance < num)
					{
						num = hits[index].distance;
						num2 = k;
					}
				}
				GameObject entityObject = hits[array[num2]].entityObject;
				if (!ignoreList.Contains(entityObject))
				{
					ignoreList.Add(entityObject);
					BulletAttack.BulletHit bulletHit = hits[array[num2]];
					if (!this.ProcessHit(ref bulletHit))
					{
						endPosition = hits[array[num2]].point;
						return entityObject;
					}
				}
				array[num2] = array[j];
			}
			return null;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00011194 File Offset: 0x0000F394
		private static GameObject LookUpColliderEntityObject(Collider collider)
		{
			HurtBox component = collider.GetComponent<HurtBox>();
			if (!component || !component.healthComponent)
			{
				return collider.gameObject;
			}
			return component.healthComponent.gameObject;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x000111CF File Offset: 0x0000F3CF
		private static Collider[] PhysicsOverlapPoint(Vector3 point, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
		{
			return Physics.OverlapBox(point, Vector3.zero, Quaternion.identity, layerMask, queryTriggerInteraction);
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x000111E4 File Offset: 0x0000F3E4
		public void Fire()
		{
			Vector3[] array = new Vector3[this.bulletCount];
			Vector3 up = Vector3.up;
			Vector3 axis = Vector3.Cross(up, this.aimVector);
			int num = 0;
			while ((long)num < (long)((ulong)this.bulletCount))
			{
				float x = UnityEngine.Random.Range(this.minSpread, this.maxSpread);
				float z = UnityEngine.Random.Range(0f, 360f);
				Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
				float y = vector.y;
				vector.y = 0f;
				float angle = (Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f) * this.spreadYawScale;
				float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f * this.spreadPitchScale;
				array[num] = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.aimVector);
				num++;
			}
			int muzzleIndex = -1;
			Vector3 vector2 = this.origin;
			if (!this.weapon)
			{
				this.weapon = this.owner;
			}
			if (this.weapon)
			{
				ModelLocator component = this.weapon.GetComponent<ModelLocator>();
				if (component && component.modelTransform)
				{
					ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
					if (component2)
					{
						muzzleIndex = component2.FindChildIndex(this.muzzleName);
					}
				}
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this.bulletCount))
			{
				this.FireSingle(array[num2], muzzleIndex);
				num2++;
			}
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x000113A0 File Offset: 0x0000F5A0
		private void FireSingle(Vector3 normal, int muzzleIndex)
		{
			float num = this.maxDistance;
			Vector3 vector = this.origin + normal * this.maxDistance;
			List<BulletAttack.BulletHit> list = new List<BulletAttack.BulletHit>();
			bool flag = this.radius == 0f || this.smartCollision;
			bool flag2 = this.radius != 0f;
			HashSet<GameObject> hashSet = null;
			if (this.smartCollision)
			{
				hashSet = new HashSet<GameObject>();
			}
			if (flag)
			{
				RaycastHit[] array = Physics.RaycastAll(this.origin, normal, num, this.hitMask, this.queryTriggerInteraction);
				for (int i = 0; i < array.Length; i++)
				{
					BulletAttack.BulletHit bulletHit = default(BulletAttack.BulletHit);
					this.InitBulletHitFromRaycastHit(ref bulletHit, this.origin, normal, ref array[i]);
					list.Add(bulletHit);
					if (this.smartCollision)
					{
						hashSet.Add(bulletHit.entityObject);
					}
					if (bulletHit.distance < num)
					{
						num = bulletHit.distance;
					}
				}
			}
			if (flag2)
			{
				LayerMask mask = this.hitMask;
				if (this.smartCollision)
				{
					mask &= ~LayerIndex.world.mask;
				}
				RaycastHit[] array2 = Physics.SphereCastAll(this.origin, this.radius, normal, num, mask, this.queryTriggerInteraction);
				for (int j = 0; j < array2.Length; j++)
				{
					BulletAttack.BulletHit bulletHit2 = default(BulletAttack.BulletHit);
					this.InitBulletHitFromRaycastHit(ref bulletHit2, this.origin, normal, ref array2[j]);
					if (!this.smartCollision || !hashSet.Contains(bulletHit2.entityObject))
					{
						list.Add(bulletHit2);
					}
				}
			}
			this.ProcessHitList(list, ref vector, new List<GameObject>());
			if (this.tracerEffectPrefab)
			{
				EffectData effectData = new EffectData
				{
					origin = vector,
					start = this.origin
				};
				effectData.SetChildLocatorTransformReference(this.weapon, muzzleIndex);
				EffectManager.SpawnEffect(this.tracerEffectPrefab, effectData, true);
			}
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x00011594 File Offset: 0x0000F794
		[NetworkMessageHandler(msgType = 53, server = true)]
		private static void HandleBulletDamage(NetworkMessage netMsg)
		{
			NetworkReader reader = netMsg.reader;
			GameObject gameObject = reader.ReadGameObject();
			DamageInfo damageInfo = reader.ReadDamageInfo();
			if (reader.ReadBoolean() && gameObject)
			{
				HealthComponent component = gameObject.GetComponent<HealthComponent>();
				if (component)
				{
					component.TakeDamage(damageInfo);
				}
				GlobalEventManager.instance.OnHitEnemy(damageInfo, gameObject);
			}
			GlobalEventManager.instance.OnHitAll(damageInfo, gameObject);
		}

		// Token: 0x040003E5 RID: 997
		public GameObject owner;

		// Token: 0x040003E6 RID: 998
		public GameObject weapon;

		// Token: 0x040003E7 RID: 999
		public float damage = 1f;

		// Token: 0x040003E8 RID: 1000
		public bool isCrit;

		// Token: 0x040003E9 RID: 1001
		public float force = 1f;

		// Token: 0x040003EA RID: 1002
		public ProcChainMask procChainMask;

		// Token: 0x040003EB RID: 1003
		public float procCoefficient = 1f;

		// Token: 0x040003EC RID: 1004
		public DamageType damageType;

		// Token: 0x040003ED RID: 1005
		public DamageColorIndex damageColorIndex;

		// Token: 0x040003EE RID: 1006
		public bool sniper;

		// Token: 0x040003EF RID: 1007
		public BulletAttack.FalloffModel falloffModel = BulletAttack.FalloffModel.DefaultBullet;

		// Token: 0x040003F0 RID: 1008
		public GameObject tracerEffectPrefab;

		// Token: 0x040003F1 RID: 1009
		public GameObject hitEffectPrefab;

		// Token: 0x040003F2 RID: 1010
		public string muzzleName = "";

		// Token: 0x040003F3 RID: 1011
		public bool HitEffectNormal = true;

		// Token: 0x040003F4 RID: 1012
		public Vector3 origin;

		// Token: 0x040003F5 RID: 1013
		private Vector3 _aimVector;

		// Token: 0x040003F6 RID: 1014
		private float _maxDistance = 200f;

		// Token: 0x040003F7 RID: 1015
		public float radius;

		// Token: 0x040003F8 RID: 1016
		public uint bulletCount = 1U;

		// Token: 0x040003F9 RID: 1017
		public float minSpread;

		// Token: 0x040003FA RID: 1018
		public float maxSpread;

		// Token: 0x040003FB RID: 1019
		public float spreadPitchScale = 1f;

		// Token: 0x040003FC RID: 1020
		public float spreadYawScale = 1f;

		// Token: 0x040003FD RID: 1021
		public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore;

		// Token: 0x040003FE RID: 1022
		private static readonly LayerMask defaultHitMask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask;

		// Token: 0x040003FF RID: 1023
		public LayerMask hitMask = BulletAttack.defaultHitMask;

		// Token: 0x04000400 RID: 1024
		private static readonly LayerMask defaultStopperMask = BulletAttack.defaultHitMask;

		// Token: 0x04000401 RID: 1025
		public LayerMask stopperMask = BulletAttack.defaultStopperMask;

		// Token: 0x04000402 RID: 1026
		public bool smartCollision;

		// Token: 0x04000403 RID: 1027
		public BulletAttack.HitCallback hitCallback;

		// Token: 0x04000404 RID: 1028
		private static NetworkWriter messageWriter = new NetworkWriter();

		// Token: 0x04000405 RID: 1029
		public BulletAttack.FilterCallback filterCallback;

		// Token: 0x020000D5 RID: 213
		public enum FalloffModel
		{
			// Token: 0x04000407 RID: 1031
			None,
			// Token: 0x04000408 RID: 1032
			DefaultBullet,
			// Token: 0x04000409 RID: 1033
			Buckshot
		}

		// Token: 0x020000D6 RID: 214
		// (Invoke) Token: 0x06000433 RID: 1075
		public delegate bool HitCallback(ref BulletAttack.BulletHit hitInfo);

		// Token: 0x020000D7 RID: 215
		// (Invoke) Token: 0x06000437 RID: 1079
		public delegate bool FilterCallback(ref BulletAttack.BulletHit hitInfo);

		// Token: 0x020000D8 RID: 216
		public struct BulletHit
		{
			// Token: 0x0400040A RID: 1034
			public Vector3 direction;

			// Token: 0x0400040B RID: 1035
			public Vector3 point;

			// Token: 0x0400040C RID: 1036
			public Vector3 surfaceNormal;

			// Token: 0x0400040D RID: 1037
			public float distance;

			// Token: 0x0400040E RID: 1038
			public Collider collider;

			// Token: 0x0400040F RID: 1039
			public HurtBox hitHurtBox;

			// Token: 0x04000410 RID: 1040
			public GameObject entityObject;

			// Token: 0x04000411 RID: 1041
			public HurtBox.DamageModifier damageModifier;
		}
	}
}
