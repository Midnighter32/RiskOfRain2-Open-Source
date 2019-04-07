using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000555 RID: 1365
	public class ProjectileManager : MonoBehaviour
	{
		// Token: 0x06001E60 RID: 7776 RVA: 0x0008F4A4 File Offset: 0x0008D6A4
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			ProjectileManager.projectilePrefabs = Resources.LoadAll<GameObject>("Prefabs/Projectiles/");
			Array.Sort<GameObject>(ProjectileManager.projectilePrefabs, (GameObject a, GameObject b) => string.CompareOrdinal(a.name, b.name));
			ProjectileManager.projectilePrefabProjectileControllers = (from prefab in ProjectileManager.projectilePrefabs
			select prefab.GetComponent<ProjectileController>()).ToArray<ProjectileController>();
			int num = 256;
			if (ProjectileManager.projectilePrefabs.Length > num)
			{
				Debug.LogErrorFormat("Cannot have more than {0} projectile prefabs defined, which is over the limit for {1}. Check comments at error source for details.", new object[]
				{
					num,
					typeof(byte).Name
				});
				for (int i = num; i < ProjectileManager.projectilePrefabs.Length; i++)
				{
					Debug.LogErrorFormat("Could not register projectile [{0}/{1}]=\"{2}\"", new object[]
					{
						i,
						num - 1,
						ProjectileManager.projectilePrefabs[i].name
					});
				}
			}
		}

		// Token: 0x06001E61 RID: 7777 RVA: 0x0008F59C File Offset: 0x0008D79C
		private void Awake()
		{
			this.predictionManager = new ProjectileManager.PredictionManager();
		}

		// Token: 0x06001E62 RID: 7778 RVA: 0x0008F5A9 File Offset: 0x0008D7A9
		private void OnDisable()
		{
			if (ProjectileManager.instance == this)
			{
				ProjectileManager.instance = null;
			}
		}

		// Token: 0x06001E63 RID: 7779 RVA: 0x0008F5BE File Offset: 0x0008D7BE
		private void OnEnable()
		{
			if (ProjectileManager.instance == null)
			{
				ProjectileManager.instance = this;
				return;
			}
			Debug.LogErrorFormat(this, "Duplicate instance of singleton class {0}. Only one should exist at a time", new object[]
			{
				base.GetType().Name
			});
		}

		// Token: 0x06001E64 RID: 7780 RVA: 0x0008F5F3 File Offset: 0x0008D7F3
		[NetworkMessageHandler(msgType = 49, server = true)]
		private static void HandlePlayerFireProjectile(NetworkMessage netMsg)
		{
			if (ProjectileManager.instance)
			{
				ProjectileManager.instance.HandlePlayerFireProjectileInternal(netMsg);
			}
		}

		// Token: 0x06001E65 RID: 7781 RVA: 0x0008F60C File Offset: 0x0008D80C
		[NetworkMessageHandler(msgType = 50, client = true)]
		private static void HandleReleaseProjectilePredictionId(NetworkMessage netMsg)
		{
			if (ProjectileManager.instance)
			{
				ProjectileManager.instance.HandleReleaseProjectilePredictionIdInternal(netMsg);
			}
		}

		// Token: 0x06001E66 RID: 7782 RVA: 0x0008F625 File Offset: 0x0008D825
		private int FindProjectilePrefabIndex(GameObject prefab)
		{
			return Array.IndexOf<GameObject>(ProjectileManager.projectilePrefabs, prefab);
		}

		// Token: 0x06001E67 RID: 7783 RVA: 0x0008F632 File Offset: 0x0008D832
		private GameObject FindProjectilePrefabFromIndex(int projectilePrefabIndex)
		{
			if (projectilePrefabIndex < ProjectileManager.projectilePrefabs.Length)
			{
				return ProjectileManager.projectilePrefabs[projectilePrefabIndex];
			}
			return null;
		}

		// Token: 0x06001E68 RID: 7784 RVA: 0x0008F648 File Offset: 0x0008D848
		[Obsolete("Use the FireProjectileInfo overload of FireProjectile instead.")]
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

		// Token: 0x06001E69 RID: 7785 RVA: 0x0008F6C9 File Offset: 0x0008D8C9
		public void FireProjectile(FireProjectileInfo fireProjectileInfo)
		{
			if (NetworkServer.active)
			{
				this.FireProjectileServer(fireProjectileInfo, null, 0, 0.0);
				return;
			}
			this.FireProjectileClient(fireProjectileInfo, NetworkManager.singleton.client);
		}

		// Token: 0x06001E6A RID: 7786 RVA: 0x0008F6F8 File Offset: 0x0008D8F8
		private void FireProjectileClient(FireProjectileInfo fireProjectileInfo, NetworkClient client)
		{
			int num = this.FindProjectilePrefabIndex(fireProjectileInfo.projectilePrefab);
			if (num == -1)
			{
				Debug.LogErrorFormat(fireProjectileInfo.projectilePrefab, "Prefab {0} is not a registered projectile prefab.", new object[]
				{
					fireProjectileInfo.projectilePrefab
				});
				return;
			}
			bool allowPrediction = ProjectileManager.projectilePrefabProjectileControllers[num].allowPrediction;
			ushort predictionId = 0;
			if (allowPrediction)
			{
				ProjectileController component = UnityEngine.Object.Instantiate<GameObject>(fireProjectileInfo.projectilePrefab, fireProjectileInfo.position, fireProjectileInfo.rotation).GetComponent<ProjectileController>();
				ProjectileManager.InitializeProjectile(component, fireProjectileInfo);
				this.predictionManager.RegisterPrediction(component);
				predictionId = component.predictionId;
			}
			this.fireMsg.sendTime = (double)Run.instance.time;
			this.fireMsg.prefabIndex = (byte)num;
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

		// Token: 0x06001E6B RID: 7787 RVA: 0x0008F888 File Offset: 0x0008DA88
		private static void InitializeProjectile(ProjectileController projectileController, FireProjectileInfo fireProjectileInfo)
		{
			GameObject gameObject = projectileController.gameObject;
			ProjectileDamage component = gameObject.GetComponent<ProjectileDamage>();
			TeamFilter component2 = gameObject.GetComponent<TeamFilter>();
			ProjectileNetworkTransform component3 = gameObject.GetComponent<ProjectileNetworkTransform>();
			MissileController component4 = gameObject.GetComponent<MissileController>();
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

		// Token: 0x06001E6C RID: 7788 RVA: 0x0008F9C8 File Offset: 0x0008DBC8
		private void FireProjectileServer(FireProjectileInfo fireProjectileInfo, NetworkConnection clientAuthorityOwner = null, ushort predictionId = 0, double fastForwardTime = 0.0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fireProjectileInfo.projectilePrefab, fireProjectileInfo.position, fireProjectileInfo.rotation);
			ProjectileController component = gameObject.GetComponent<ProjectileController>();
			component.NetworkpredictionId = predictionId;
			ProjectileManager.InitializeProjectile(component, fireProjectileInfo);
			if (clientAuthorityOwner != null)
			{
				NetworkServer.SpawnWithClientAuthority(gameObject, clientAuthorityOwner);
				return;
			}
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x06001E6D RID: 7789 RVA: 0x0008FA14 File Offset: 0x0008DC14
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

		// Token: 0x06001E6E RID: 7790 RVA: 0x0008FA40 File Offset: 0x0008DC40
		public void OnClientProjectileReceived(ProjectileController projectile)
		{
			if (projectile.predictionId != 0 && projectile.hasAuthority)
			{
				this.predictionManager.OnAuthorityProjectileReceived(projectile);
			}
		}

		// Token: 0x06001E6F RID: 7791 RVA: 0x0008FA60 File Offset: 0x0008DC60
		private void ReleasePredictionId(NetworkConnection owner, ushort predictionId)
		{
			this.releasePredictionIdMsg.predictionId = predictionId;
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(50);
			networkWriter.Write(this.releasePredictionIdMsg);
			networkWriter.FinishMessage();
			owner.SendWriter(networkWriter, 0);
		}

		// Token: 0x06001E70 RID: 7792 RVA: 0x0008FAA4 File Offset: 0x0008DCA4
		private void HandlePlayerFireProjectileInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ProjectileManager.PlayerFireProjectileMessage>(this.fireMsg);
			GameObject gameObject = this.FindProjectilePrefabFromIndex((int)this.fireMsg.prefabIndex);
			if (gameObject == null)
			{
				this.ReleasePredictionId(netMsg.conn, this.fireMsg.predictionId);
				return;
			}
			FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
			fireProjectileInfo.projectilePrefab = gameObject;
			fireProjectileInfo.position = this.fireMsg.position;
			fireProjectileInfo.rotation = this.fireMsg.rotation;
			fireProjectileInfo.owner = this.fireMsg.owner;
			fireProjectileInfo.damage = this.fireMsg.damage;
			fireProjectileInfo.force = this.fireMsg.force;
			fireProjectileInfo.crit = this.fireMsg.crit;
			GameObject gameObject2 = this.fireMsg.target.ResolveGameObject();
			fireProjectileInfo.target = ((gameObject2 != null) ? gameObject2.gameObject : null);
			fireProjectileInfo.damageColorIndex = this.fireMsg.damageColorIndex;
			fireProjectileInfo.speedOverride = this.fireMsg.speedOverride;
			fireProjectileInfo.fuseOverride = this.fireMsg.fuseOverride;
			this.FireProjectileServer(fireProjectileInfo, netMsg.conn, this.fireMsg.predictionId, (double)Run.instance.time - this.fireMsg.sendTime);
		}

		// Token: 0x06001E71 RID: 7793 RVA: 0x0008FBF4 File Offset: 0x0008DDF4
		private void HandleReleaseProjectilePredictionIdInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<ProjectileManager.ReleasePredictionIdMessage>(this.releasePredictionIdMsg);
			this.predictionManager.ReleasePredictionId(this.releasePredictionIdMsg.predictionId);
		}

		// Token: 0x06001E72 RID: 7794 RVA: 0x0008FC18 File Offset: 0x0008DE18
		[ConCommand(commandName = "dump_projectile_map", flags = ConVarFlags.None, helpText = "Dumps the map between indices and projectile prefabs.")]
		private static void DumpProjectileMap(ConCommandArgs args)
		{
			string[] array = new string[ProjectileManager.projectilePrefabs.Length];
			for (int i = 0; i < ProjectileManager.projectilePrefabs.Length; i++)
			{
				array[i] = string.Format("[{0}] = {1}", i, ProjectileManager.projectilePrefabs[i].name);
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x0400210E RID: 8462
		public static ProjectileManager instance;

		// Token: 0x0400210F RID: 8463
		private static GameObject[] projectilePrefabs;

		// Token: 0x04002110 RID: 8464
		private static ProjectileController[] projectilePrefabProjectileControllers;

		// Token: 0x04002111 RID: 8465
		private ProjectileManager.PredictionManager predictionManager;

		// Token: 0x04002112 RID: 8466
		private ProjectileManager.PlayerFireProjectileMessage fireMsg = new ProjectileManager.PlayerFireProjectileMessage();

		// Token: 0x04002113 RID: 8467
		private ProjectileManager.ReleasePredictionIdMessage releasePredictionIdMsg = new ProjectileManager.ReleasePredictionIdMessage();

		// Token: 0x02000556 RID: 1366
		private class PlayerFireProjectileMessage : MessageBase
		{
			// Token: 0x06001E76 RID: 7798 RVA: 0x0008FC94 File Offset: 0x0008DE94
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

			// Token: 0x06001E77 RID: 7799 RVA: 0x0008FD40 File Offset: 0x0008DF40
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

			// Token: 0x04002114 RID: 8468
			public double sendTime;

			// Token: 0x04002115 RID: 8469
			public byte prefabIndex;

			// Token: 0x04002116 RID: 8470
			public Vector3 position;

			// Token: 0x04002117 RID: 8471
			public Quaternion rotation;

			// Token: 0x04002118 RID: 8472
			public GameObject owner;

			// Token: 0x04002119 RID: 8473
			public HurtBoxReference target;

			// Token: 0x0400211A RID: 8474
			public float damage;

			// Token: 0x0400211B RID: 8475
			public float force;

			// Token: 0x0400211C RID: 8476
			public bool crit;

			// Token: 0x0400211D RID: 8477
			public ushort predictionId;

			// Token: 0x0400211E RID: 8478
			public DamageColorIndex damageColorIndex;

			// Token: 0x0400211F RID: 8479
			public float speedOverride;

			// Token: 0x04002120 RID: 8480
			public float fuseOverride;
		}

		// Token: 0x02000557 RID: 1367
		private class ReleasePredictionIdMessage : MessageBase
		{
			// Token: 0x06001E79 RID: 7801 RVA: 0x0008FDE9 File Offset: 0x0008DFE9
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32((uint)this.predictionId);
			}

			// Token: 0x06001E7A RID: 7802 RVA: 0x0008FDF7 File Offset: 0x0008DFF7
			public override void Deserialize(NetworkReader reader)
			{
				this.predictionId = (ushort)reader.ReadPackedUInt32();
			}

			// Token: 0x04002121 RID: 8481
			public ushort predictionId;
		}

		// Token: 0x02000558 RID: 1368
		private class PredictionManager
		{
			// Token: 0x06001E7B RID: 7803 RVA: 0x0008FE05 File Offset: 0x0008E005
			public ProjectileController FindPredictedProjectileController(ushort predictionId)
			{
				return this.predictions[predictionId];
			}

			// Token: 0x06001E7C RID: 7804 RVA: 0x0008FE14 File Offset: 0x0008E014
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

			// Token: 0x06001E7D RID: 7805 RVA: 0x0008FE70 File Offset: 0x0008E070
			public void ReleasePredictionId(ushort predictionId)
			{
				ProjectileController projectileController = this.predictions[predictionId];
				this.predictions.Remove(predictionId);
				if (projectileController && projectileController.gameObject)
				{
					UnityEngine.Object.Destroy(projectileController.gameObject);
				}
			}

			// Token: 0x06001E7E RID: 7806 RVA: 0x0008FEB7 File Offset: 0x0008E0B7
			public void RegisterPrediction(ProjectileController predictedProjectile)
			{
				predictedProjectile.NetworkpredictionId = this.RequestPredictionId();
				this.predictions[predictedProjectile.predictionId] = predictedProjectile;
				predictedProjectile.isPrediction = true;
			}

			// Token: 0x06001E7F RID: 7807 RVA: 0x0008FEE0 File Offset: 0x0008E0E0
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

			// Token: 0x04002122 RID: 8482
			private Dictionary<ushort, ProjectileController> predictions = new Dictionary<ushort, ProjectileController>();
		}
	}
}
