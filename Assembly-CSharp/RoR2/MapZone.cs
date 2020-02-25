using System;
using System.Collections.Generic;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200027A RID: 634
	public class MapZone : MonoBehaviour
	{
		// Token: 0x06000E18 RID: 3608 RVA: 0x0003EEB5 File Offset: 0x0003D0B5
		static MapZone()
		{
			VehicleSeat.onPassengerExitGlobal += MapZone.CheckCharacterOnVehicleExit;
			RoR2Application.onFixedUpdate += MapZone.StaticFixedUpdate;
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x0003EEE4 File Offset: 0x0003D0E4
		private static bool TestColliders(Collider characterCollider, Collider triggerCollider)
		{
			Vector3 vector;
			float num;
			return Physics.ComputePenetration(characterCollider, characterCollider.transform.position, characterCollider.transform.rotation, triggerCollider, triggerCollider.transform.position, triggerCollider.transform.rotation, out vector, out num);
		}

		// Token: 0x06000E1A RID: 3610 RVA: 0x0003EF28 File Offset: 0x0003D128
		private void Awake()
		{
			this.collider = base.GetComponent<Collider>();
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x0003EF36 File Offset: 0x0003D136
		public void OnTriggerEnter(Collider other)
		{
			if (this.triggerType == MapZone.TriggerType.TriggerEnter)
			{
				this.TryZoneStart(other);
				return;
			}
			this.TryZoneEnd(other);
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x0003EF50 File Offset: 0x0003D150
		public void OnTriggerExit(Collider other)
		{
			if (this.triggerType == MapZone.TriggerType.TriggerExit)
			{
				this.TryZoneStart(other);
				return;
			}
			this.TryZoneEnd(other);
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x0003EF6C File Offset: 0x0003D16C
		private void TryZoneStart(Collider other)
		{
			CharacterBody component = other.GetComponent<CharacterBody>();
			if (!component)
			{
				return;
			}
			if (component.currentVehicle)
			{
				this.queuedCollisions.Add(new MapZone.CollisionInfo(this, other));
				return;
			}
			TeamComponent teamComponent = component.teamComponent;
			MapZone.ZoneType zoneType = this.zoneType;
			if (zoneType != MapZone.ZoneType.OutOfBounds)
			{
				if (zoneType != MapZone.ZoneType.KickOutPlayers)
				{
					return;
				}
				if (teamComponent.teamIndex == TeamIndex.Player)
				{
					this.TeleportBody(component);
				}
			}
			else
			{
				if (teamComponent.teamIndex == TeamIndex.Player)
				{
					this.TeleportBody(component);
					return;
				}
				if (NetworkServer.active)
				{
					if (Physics.GetIgnoreLayerCollision(base.gameObject.layer, other.gameObject.layer))
					{
						return;
					}
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						healthComponent.Suicide(healthComponent.lastHitAttacker, base.gameObject, DamageType.Generic);
						return;
					}
				}
			}
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x0003F02D File Offset: 0x0003D22D
		private void TryZoneEnd(Collider other)
		{
			if (this.queuedCollisions.Count == 0)
			{
				return;
			}
			if (this.queuedCollisions.Contains(new MapZone.CollisionInfo(this, other)))
			{
				this.queuedCollisions.Remove(new MapZone.CollisionInfo(this, other));
			}
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x0003F064 File Offset: 0x0003D264
		private void ProcessQueuedCollisionsForCollider(Collider collider)
		{
			for (int i = this.queuedCollisions.Count - 1; i >= 0; i--)
			{
				if (this.queuedCollisions[i].otherCollider == collider)
				{
					this.queuedCollisions.RemoveAt(i);
					this.TryZoneStart(collider);
				}
			}
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x0003F0B0 File Offset: 0x0003D2B0
		private static void StaticFixedUpdate()
		{
			int i = 0;
			int count = MapZone.collidersToCheckInFixedUpdate.Count;
			while (i < count)
			{
				Collider exists = MapZone.collidersToCheckInFixedUpdate.Dequeue();
				if (exists)
				{
					foreach (MapZone mapZone in InstanceTracker.GetInstancesList<MapZone>())
					{
						mapZone.ProcessQueuedCollisionsForCollider(exists);
					}
				}
				i++;
			}
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x0003F12C File Offset: 0x0003D32C
		private static void CheckCharacterOnVehicleExit(VehicleSeat vehicleSeat, GameObject passengerObject)
		{
			Collider component = passengerObject.GetComponent<Collider>();
			MapZone.collidersToCheckInFixedUpdate.Enqueue(component);
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x0003F14B File Offset: 0x0003D34B
		private void OnEnable()
		{
			InstanceTracker.Add<MapZone>(this);
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x0003F153 File Offset: 0x0003D353
		private void OnDisable()
		{
			InstanceTracker.Remove<MapZone>(this);
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x0003F15C File Offset: 0x0003D35C
		private void TeleportBody(CharacterBody characterBody)
		{
			if (!Util.HasEffectiveAuthority(characterBody.gameObject))
			{
				return;
			}
			if (!Physics.GetIgnoreLayerCollision(base.gameObject.layer, characterBody.gameObject.layer))
			{
				SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
				spawnCard.hullSize = characterBody.hullClassification;
				spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
				spawnCard.prefab = Resources.Load<GameObject>("SpawnCards/HelperPrefab");
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
					position = characterBody.transform.position
				}, RoR2Application.rng));
				if (gameObject)
				{
					Debug.Log("tp back");
					TeleportHelper.TeleportBody(characterBody, gameObject.transform.position);
					GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(characterBody.gameObject);
					if (teleportEffectPrefab)
					{
						EffectManager.SimpleEffect(teleportEffectPrefab, gameObject.transform.position, Quaternion.identity, true);
					}
					UnityEngine.Object.Destroy(gameObject);
				}
				UnityEngine.Object.Destroy(spawnCard);
			}
		}

		// Token: 0x04000E0A RID: 3594
		public MapZone.TriggerType triggerType;

		// Token: 0x04000E0B RID: 3595
		public MapZone.ZoneType zoneType;

		// Token: 0x04000E0C RID: 3596
		private Collider collider;

		// Token: 0x04000E0D RID: 3597
		private readonly List<MapZone.CollisionInfo> queuedCollisions = new List<MapZone.CollisionInfo>();

		// Token: 0x04000E0E RID: 3598
		private static readonly Queue<Collider> collidersToCheckInFixedUpdate = new Queue<Collider>();

		// Token: 0x0200027B RID: 635
		public enum TriggerType
		{
			// Token: 0x04000E10 RID: 3600
			TriggerExit,
			// Token: 0x04000E11 RID: 3601
			TriggerEnter
		}

		// Token: 0x0200027C RID: 636
		public enum ZoneType
		{
			// Token: 0x04000E13 RID: 3603
			OutOfBounds,
			// Token: 0x04000E14 RID: 3604
			KickOutPlayers
		}

		// Token: 0x0200027D RID: 637
		private struct CollisionInfo : IEquatable<MapZone.CollisionInfo>
		{
			// Token: 0x06000E26 RID: 3622 RVA: 0x0003F265 File Offset: 0x0003D465
			public CollisionInfo(MapZone mapZone, Collider otherCollider)
			{
				this.mapZone = mapZone;
				this.otherCollider = otherCollider;
			}

			// Token: 0x06000E27 RID: 3623 RVA: 0x0003F275 File Offset: 0x0003D475
			public bool Equals(MapZone.CollisionInfo other)
			{
				return this.mapZone == other.mapZone && this.otherCollider == other.otherCollider;
			}

			// Token: 0x06000E28 RID: 3624 RVA: 0x0003F295 File Offset: 0x0003D495
			public override bool Equals(object obj)
			{
				return obj is MapZone.CollisionInfo && this.Equals((MapZone.CollisionInfo)obj);
			}

			// Token: 0x06000E29 RID: 3625 RVA: 0x0003F2AD File Offset: 0x0003D4AD
			public override int GetHashCode()
			{
				return this.otherCollider.GetHashCode();
			}

			// Token: 0x04000E15 RID: 3605
			public readonly MapZone mapZone;

			// Token: 0x04000E16 RID: 3606
			public readonly Collider otherCollider;
		}
	}
}
