using System;
using System.Collections.Generic;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200051C RID: 1308
	public class ProjectileManager : MonoBehaviour
	{
		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001ED8 RID: 7896 RVA: 0x0008599D File Offset: 0x00083B9D
		// (set) Token: 0x06001ED9 RID: 7897 RVA: 0x000859A4 File Offset: 0x00083BA4
		public static ProjectileManager instance { get; private set; }

		// Token: 0x06001EDA RID: 7898 RVA: 0x000859AC File Offset: 0x00083BAC
		private void Awake()
		{
			this.predictionManager = new ProjectileManager.PredictionManager();
		}

		// Token: 0x06001EDB RID: 7899 RVA: 0x000859B9 File Offset: 0x00083BB9
		private void OnEnable()
		{
			ProjectileManager.instance = SingletonHelper.Assign<ProjectileManager>(ProjectileManager.instance, this);
		}

		// Token: 0x06001EDC RID: 7900 RVA: 0x000859CB File Offset: 0x00083BCB
		private void OnDisable()
		{
			ProjectileManager.instance = SingletonHelper.Unassign<ProjectileManager>(ProjectileManager.instance, this);
		}

		// Token: 0x06001EDD RID: 7901 RVA: 0x000859DD File Offset: 0x00083BDD
		[NetworkMessageHandler(msgType = 49, server = true)]
		private static void HandlePlayerFireProjectile(NetworkMessage netMsg)
		{
			if (ProjectileManager.instance)
			{
				ProjectileManager.instance.HandlePlayerFireProjectileInternal(netMsg);
			}
		}

		// Token: 0x06001EDE RID: 7902 RVA: 0x000859F6 File Offset: 0x00083BF6
		[NetworkMessageHandler(msgType = 50, client = true)]
		private static void HandleReleaseProjectilePredictionId(NetworkMessage netMsg)
		{
			if (ProjectileManager.instance)
			{
				ProjectileManager.instance.HandleReleaseProjectilePredictionIdInternal(netMsg);
			}
		}

		// Token: 0x06001EDF RID: 7903 RVA: 0x00085A10 File Offset: 0x00083C10
		public void FireProjectile(GameObject prefab, Vector3 position, Quaternion rotation, GameObject owner, float damage, float force, bool crit, DamageColorIndex damageColorIndex = DamageColorIndex.Default, GameObject target = null, float speedOverride = -1f)
		{
			FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
			{
				projectilePrefab = prefab,
				position = position,
				rotation = rotation,
				owner = owner,
				damage = damage,
				force = force,
				crit = crit,
				damageColorIndex = damageColorIndex,
				target = target,
				speedOverride = speedOverride,
				fuseOverride = -1f
			};
			this.FireProjectile(fireProjectileInfo);
		}

		// Token: 0x06001EE0 RID: 7904 RVA: 0x00085A91 File Offset: 0x00083C91
		public void FireProjectile(FireProjectileInfo fireProjectileInfo)
		{
			if (NetworkServer.active)
			{
				this.FireProjectileServer(fireProjectileInfo, null, 0, 0.0);
				return;
			}
			this.FireProjectileClient(fireProjectileInfo, NetworkManager.singleton.client);
		}

		// Token: 0x06001EE1 RID: 7905 RVA: 0x00085AC0 File Offset: 0x00083CC0
		private void FireProjectileClient(FireProjectileInfo fireProjectileInfo, NetworkClient client)
		{
			int projectileIndex = ProjectileCatalog.GetProjectileIndex(fireProjectileInfo.projectilePrefab);
			if (projectileIndex == -1)
			{
				Debug.LogErrorFormat(fireProjectileInfo.projectilePrefab, "Prefab {0} is not a registered projectile prefab.", new object[]
				{
					fireProjectileInfo.projectilePrefab
				});
				return;
			}
			bool allowPrediction = ProjectileCatalog.GetProjectilePrefabProjectileControllerComponent(projectileIndex).allowPrediction;
			ushort predictionId = 0;
			if (allowPrediction)
			{
				ProjectileController component = UnityEngine.Object.Instantiate<GameObject>(fireProjectileInfo.projectilePrefab, fireProjectileInfo.position, fireProjectileInfo.rotation).GetComponent<ProjectileController>();
				ProjectileManager.InitializeProjectile(component, fireProjectileInfo);
				this.predictionManager.RegisterPrediction(component);
				predictionId = component.predictionId;
			}
			this.fireMsg.sendTime = (double)Run.instance.time;
			this.fireMsg.prefabIndex = (byte)projectileIndex;
			this.fireMsg.position = fireProjectileInfo.position;
			this.fireMsg.rotation = fireProjectileInfo.rotation;
			this.fireMsg.owner = fireProjectileInfo.owner;
			this.fireMsg.predictionId = predictionId;
			this.fireMsg.damage = fireProjectileInfo.damage;
			this.fireMsg.force = fireProjectileInfo.force;
			this.fireMsg.crit = fireProjectileInfo.crit;
			this.fireMsg.damageColorIndex = fireProjectileInfo.damageColorIndex;
			this.fireMsg.speedOverride = fireProjectileInfo.speedOverride;
			this.fireMsg.fuseOverride = fireProjectileInfo.fuseOverride;
			this.fireMsg.target = HurtBoxReference.FromRootObject(fireProjectileInfo.target);
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(49);
			networkWriter.Write(this.fireMsg);
			networkWriter.FinishMessage();
			client.SendWriter(networkWriter, 0);
		}

		// Token: 0x06001EE2 RID: 7906 RVA: 0x00085C4C File Offset: 0x00083E4C
		private static void InitializeProjectile(ProjectileController projectileController, FireProjectileInfo fireProjectileInfo)
		{
			GameObject gameObject = projectileController.gameObject;
			ProjectileDamage component = gameObject.GetComponent<ProjectileDamage>();
			TeamFilter component2 = gameObject.GetComponent<TeamFilter>();
			ProjectileNetworkTransform component3 = gameObject.GetComponent<ProjectileNetworkTransform>();
			ProjectileTargetComponent component4 = gameObject.GetComponent<ProjectileTargetComponent>();
			ProjectileSimple component5 = gameObject.GetComponent<ProjectileSimple>();
			projectileController.Networkowner = fireProjectileInfo.owner;
			projectileController.procChainMask = fireProjectileInfo.procChainMask;
			if (component2)
			{
				component2.teamIndex = TeamComponent.GetObjectTeam(fireProjectileInfo.owner);
			}
			if (component3)
			{
				component3.SetValuesFromTransform();
			}
			if (component4)
			{
				component4.target = (fireProjectileInfo.target ? fireProjectileInfo.target.transform : null);
			}
			if (fireProjectileInfo.useSpeedOverride && component5)
			{
				component5.velocity = fireProjectileInfo.speedOverride;
			}
			if (fireProjectileInfo.useFuseOverride)
			{
				ProjectileImpactExplosion component6 = gameObject.GetComponent<ProjectileImpactExplosion>();
				if (component6)
				{
					component6.lifetime = fireProjectileInfo.fuseOverride;
				}
				ProjectileFuse component7 = gameObject.GetComponent<ProjectileFuse>();
				if (component7)
				{
					component7.fuse = fireProjectileInfo.fuseOverride;
				}
			}
			if (component)
			{
				component.damage = fireProjectileInfo.damage;
				component.force = fireProjectileInfo.force;
				component.crit = fireProjectileInfo.crit;
				component.damageColorIndex = fireProjectileInfo.damageColorIndex;
			}
		}

		// Token: 0x06001EE3 RID: 7907 RVA: 0x00085D8C File Offset: 0x00083F8C
		private void FireProjectileServer(FireProjectileInfo fireProjectileInfo, NetworkConnection clientAuthorityOwner = null, ushort predictionId = 0, double fastForwardTime = 0.0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fireProjectileInfo.projectilePrefab, fireProjectileInfo.position, fireProjectileInfo.rotation);
			ProjectileController component = gameObject.GetComponent<ProjectileController>();
			component.NetworkpredictionId = predictionId;
			ProjectileManager.InitializeProjectile(component, fireProjectileInfo);
			NetworkIdentity component2 = gameObject.GetComponent<NetworkIdentity>();
			if (clientAuthorityOwner != null && component2.localPlayerAuthority)
			{
				NetworkServer.SpawnWithClientAuthority(gameObject, clientAuthorityOwner);
				return;
			}
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x06001EE4 RID: 7908 RVA: 0x00085DE8 File Offset: 0x00083FE8
		public void OnServerProjectileDestroyed(ProjectileController projectile)
		{
			if (projectile.predictionId != 0)
			{
				NetworkConnection clientAuthorityOwner = projectile.clientAuthorityOwner;
				if (clientAuthorityOwner != null)
				{
					this.ReleasePredictionId(clientAuthorityOwner, projectile.predictionId);
				}
			}
		}

		// Token: 0x06001EE5 RID: 7909 RVA: 0x00085E14 File Offset: 0x00084014
		public void OnClientProjectileReceived(ProjectileController projectile)
		{
			if (projectile.predictionId != 0 && projectile.hasAuthority)
			{
				this.predictionManager.OnAuthorityProjectileReceived(projectile);
			}
		}

		// Token: 0x06001EE6 RID: 7910 RVA: 0x00085E34 File Offset: 0x00084034
		private void ReleasePredictionId(NetworkConnection owner, ushort predictionId)
		{
			this.releasePredictionIdMsg.predictionId = predictionId;
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(50);
			networkWriter.Write(this.releasePredictionIdMsg);
			networkWriter.FinishMessage();
			owner.SendWriter(networkWriter, 0);
		}

		// Token: 0x06001EE7 RID: 7911 RVA: 0x00085E78 File Offset: 0x00084078
		private void HandlePlayerFireProjectileInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ProjectileManager.PlayerFireProjectileMessage>(this.fireMsg);
			GameObject projectilePrefab = ProjectileCatalog.GetProjectilePrefab((int)this.fireMsg.prefabIndex);
			if (projectilePrefab == null)
			{
				this.ReleasePredictionId(netMsg.conn, this.fireMsg.predictionId);
				return;
			}
			FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
			fireProjectileInfo.projectilePrefab = projectilePrefab;
			fireProjectileInfo.position = this.fireMsg.position;
			fireProjectileInfo.rotation = this.fireMsg.rotation;
			fireProjectileInfo.owner = this.fireMsg.owner;
			fireProjectileInfo.damage = this.fireMsg.damage;
			fireProjectileInfo.force = this.fireMsg.force;
			fireProjectileInfo.crit = this.fireMsg.crit;
			GameObject gameObject = this.fireMsg.target.ResolveGameObject();
			fireProjectileInfo.target = ((gameObject != null) ? gameObject.gameObject : null);
			fireProjectileInfo.damageColorIndex = this.fireMsg.damageColorIndex;
			fireProjectileInfo.speedOverride = this.fireMsg.speedOverride;
			fireProjectileInfo.fuseOverride = this.fireMsg.fuseOverride;
			this.FireProjectileServer(fireProjectileInfo, netMsg.conn, this.fireMsg.predictionId, (double)Run.instance.time - this.fireMsg.sendTime);
		}

		// Token: 0x06001EE8 RID: 7912 RVA: 0x00085FC7 File Offset: 0x000841C7
		private void HandleReleaseProjectilePredictionIdInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ProjectileManager.ReleasePredictionIdMessage>(this.releasePredictionIdMsg);
			this.predictionManager.ReleasePredictionId(this.releasePredictionIdMsg.predictionId);
		}

		// Token: 0x04001C74 RID: 7284
		private ProjectileManager.PredictionManager predictionManager;

		// Token: 0x04001C75 RID: 7285
		private ProjectileManager.PlayerFireProjectileMessage fireMsg = new ProjectileManager.PlayerFireProjectileMessage();

		// Token: 0x04001C76 RID: 7286
		private ProjectileManager.ReleasePredictionIdMessage releasePredictionIdMsg = new ProjectileManager.ReleasePredictionIdMessage();

		// Token: 0x0200051D RID: 1309
		private class PlayerFireProjectileMessage : MessageBase
		{
			// Token: 0x06001EEB RID: 7915 RVA: 0x0008600C File Offset: 0x0008420C
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.sendTime);
				writer.WritePackedUInt32((uint)this.prefabIndex);
				writer.Write(this.position);
				writer.Write(this.rotation);
				writer.Write(this.owner);
				GeneratedNetworkCode._WriteHurtBoxReference_None(writer, this.target);
				writer.Write(this.damage);
				writer.Write(this.force);
				writer.Write(this.crit);
				writer.WritePackedUInt32((uint)this.predictionId);
				writer.Write((int)this.damageColorIndex);
				writer.Write(this.speedOverride);
				writer.Write(this.fuseOverride);
			}

			// Token: 0x06001EEC RID: 7916 RVA: 0x000860B8 File Offset: 0x000842B8
			public override void Deserialize(NetworkReader reader)
			{
				this.sendTime = reader.ReadDouble();
				this.prefabIndex = (byte)reader.ReadPackedUInt32();
				this.position = reader.ReadVector3();
				this.rotation = reader.ReadQuaternion();
				this.owner = reader.ReadGameObject();
				this.target = GeneratedNetworkCode._ReadHurtBoxReference_None(reader);
				this.damage = reader.ReadSingle();
				this.force = reader.ReadSingle();
				this.crit = reader.ReadBoolean();
				this.predictionId = (ushort)reader.ReadPackedUInt32();
				this.damageColorIndex = (DamageColorIndex)reader.ReadInt32();
				this.speedOverride = reader.ReadSingle();
				this.fuseOverride = reader.ReadSingle();
			}

			// Token: 0x04001C77 RID: 7287
			public double sendTime;

			// Token: 0x04001C78 RID: 7288
			public byte prefabIndex;

			// Token: 0x04001C79 RID: 7289
			public Vector3 position;

			// Token: 0x04001C7A RID: 7290
			public Quaternion rotation;

			// Token: 0x04001C7B RID: 7291
			public GameObject owner;

			// Token: 0x04001C7C RID: 7292
			public HurtBoxReference target;

			// Token: 0x04001C7D RID: 7293
			public float damage;

			// Token: 0x04001C7E RID: 7294
			public float force;

			// Token: 0x04001C7F RID: 7295
			public bool crit;

			// Token: 0x04001C80 RID: 7296
			public ushort predictionId;

			// Token: 0x04001C81 RID: 7297
			public DamageColorIndex damageColorIndex;

			// Token: 0x04001C82 RID: 7298
			public float speedOverride;

			// Token: 0x04001C83 RID: 7299
			public float fuseOverride;
		}

		// Token: 0x0200051E RID: 1310
		private class ReleasePredictionIdMessage : MessageBase
		{
			// Token: 0x06001EEE RID: 7918 RVA: 0x00086161 File Offset: 0x00084361
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32((uint)this.predictionId);
			}

			// Token: 0x06001EEF RID: 7919 RVA: 0x0008616F File Offset: 0x0008436F
			public override void Deserialize(NetworkReader reader)
			{
				this.predictionId = (ushort)reader.ReadPackedUInt32();
			}

			// Token: 0x04001C84 RID: 7300
			public ushort predictionId;
		}

		// Token: 0x0200051F RID: 1311
		private class PredictionManager
		{
			// Token: 0x06001EF0 RID: 7920 RVA: 0x0008617D File Offset: 0x0008437D
			public ProjectileController FindPredictedProjectileController(ushort predictionId)
			{
				return this.predictions[predictionId];
			}

			// Token: 0x06001EF1 RID: 7921 RVA: 0x0008618C File Offset: 0x0008438C
			public void OnAuthorityProjectileReceived(ProjectileController authoritativeProjectile)
			{
				ProjectileController projectileController;
				if (authoritativeProjectile.hasAuthority && authoritativeProjectile.predictionId != 0 && this.predictions.TryGetValue(authoritativeProjectile.predictionId, out projectileController))
				{
					authoritativeProjectile.ghost = projectileController.ghost;
					if (authoritativeProjectile.ghost)
					{
						authoritativeProjectile.ghost.authorityTransform = authoritativeProjectile.transform;
					}
				}
			}

			// Token: 0x06001EF2 RID: 7922 RVA: 0x000861E8 File Offset: 0x000843E8
			public void ReleasePredictionId(ushort predictionId)
			{
				ProjectileController projectileController = this.predictions[predictionId];
				this.predictions.Remove(predictionId);
				if (projectileController && projectileController.gameObject)
				{
					UnityEngine.Object.Destroy(projectileController.gameObject);
				}
			}

			// Token: 0x06001EF3 RID: 7923 RVA: 0x0008622F File Offset: 0x0008442F
			public void RegisterPrediction(ProjectileController predictedProjectile)
			{
				predictedProjectile.NetworkpredictionId = this.RequestPredictionId();
				this.predictions[predictedProjectile.predictionId] = predictedProjectile;
				predictedProjectile.isPrediction = true;
			}

			// Token: 0x06001EF4 RID: 7924 RVA: 0x00086258 File Offset: 0x00084458
			private ushort RequestPredictionId()
			{
				for (ushort num = 1; num < 32767; num += 1)
				{
					if (!this.predictions.ContainsKey(num))
					{
						return num;
					}
				}
				return 0;
			}

			// Token: 0x04001C85 RID: 7301
			private Dictionary<ushort, ProjectileController> predictions = new Dictionary<ushort, ProjectileController>();
		}
	}
}
