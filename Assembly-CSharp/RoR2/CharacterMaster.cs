using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.CharacterAI;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x0200028C RID: 652
	[RequireComponent(typeof(Inventory))]
	[DisallowMultipleComponent]
	public class CharacterMaster : NetworkBehaviour
	{
		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000CD1 RID: 3281 RVA: 0x0003FB90 File Offset: 0x0003DD90
		// (set) Token: 0x06000CD2 RID: 3282 RVA: 0x0003FB98 File Offset: 0x0003DD98
		public NetworkIdentity networkIdentity { get; private set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x0003FBA1 File Offset: 0x0003DDA1
		// (set) Token: 0x06000CD4 RID: 3284 RVA: 0x0003FBA9 File Offset: 0x0003DDA9
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06000CD5 RID: 3285 RVA: 0x0003FBB2 File Offset: 0x0003DDB2
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.networkIdentity);
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0003FBC5 File Offset: 0x0003DDC5
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x0003FBD3 File Offset: 0x0003DDD3
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
			base.OnStopAuthority();
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000CD8 RID: 3288 RVA: 0x0003FBE4 File Offset: 0x0003DDE4
		// (remove) Token: 0x06000CD9 RID: 3289 RVA: 0x0003FC1C File Offset: 0x0003DE1C
		public event Action<CharacterBody> onBodyStart;

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000CDA RID: 3290 RVA: 0x0003FC51 File Offset: 0x0003DE51
		// (set) Token: 0x06000CDB RID: 3291 RVA: 0x0003FC59 File Offset: 0x0003DE59
		public TeamIndex teamIndex
		{
			get
			{
				return this._teamIndex;
			}
			set
			{
				if (this._teamIndex == value)
				{
					return;
				}
				this._teamIndex = value;
				if (NetworkServer.active)
				{
					base.SetDirtyBit(8u);
				}
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000CDC RID: 3292 RVA: 0x0003FC7A File Offset: 0x0003DE7A
		public static ReadOnlyCollection<CharacterMaster> readOnlyInstancesList
		{
			get
			{
				return CharacterMaster._readOnlyInstancesList;
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000CDE RID: 3294 RVA: 0x0003FC8A File Offset: 0x0003DE8A
		// (set) Token: 0x06000CDD RID: 3293 RVA: 0x0003FC81 File Offset: 0x0003DE81
		public Inventory inventory { get; private set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000CDF RID: 3295 RVA: 0x0003FC92 File Offset: 0x0003DE92
		// (set) Token: 0x06000CE0 RID: 3296 RVA: 0x0003FC9A File Offset: 0x0003DE9A
		private NetworkInstanceId bodyInstanceId
		{
			get
			{
				return this._bodyInstanceId;
			}
			set
			{
				if (value == this._bodyInstanceId)
				{
					return;
				}
				base.SetDirtyBit(1u);
				this._bodyInstanceId = value;
			}
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x0003FCB9 File Offset: 0x0003DEB9
		private void OnSyncBodyInstanceId(NetworkInstanceId value)
		{
			this.resolvedBodyInstance = null;
			this.bodyResolved = (value == NetworkInstanceId.Invalid);
			this._bodyInstanceId = value;
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000CE2 RID: 3298 RVA: 0x0003FCDA File Offset: 0x0003DEDA
		// (set) Token: 0x06000CE3 RID: 3299 RVA: 0x0003FD10 File Offset: 0x0003DF10
		private GameObject bodyInstanceObject
		{
			get
			{
				if (!this.bodyResolved)
				{
					this.resolvedBodyInstance = Util.FindNetworkObject(this.bodyInstanceId);
					if (this.resolvedBodyInstance)
					{
						this.bodyResolved = true;
					}
				}
				return this.resolvedBodyInstance;
			}
			set
			{
				NetworkInstanceId bodyInstanceId = NetworkInstanceId.Invalid;
				this.resolvedBodyInstance = null;
				this.bodyResolved = true;
				if (value)
				{
					NetworkIdentity component = value.GetComponent<NetworkIdentity>();
					if (component)
					{
						bodyInstanceId = component.netId;
						this.resolvedBodyInstance = value;
					}
				}
				this.bodyInstanceId = bodyInstanceId;
			}
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x0003FD5D File Offset: 0x0003DF5D
		[Server]
		public void GiveExperience(ulong amount)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::GiveExperience(System.UInt64)' called on client");
				return;
			}
			TeamManager.instance.GiveTeamExperience(this.teamIndex, amount);
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x0003FD85 File Offset: 0x0003DF85
		// (set) Token: 0x06000CE6 RID: 3302 RVA: 0x0003FD8D File Offset: 0x0003DF8D
		public uint money
		{
			get
			{
				return this._money;
			}
			set
			{
				if (value == this._money)
				{
					return;
				}
				base.SetDirtyBit(2u);
				this._money = value;
			}
		}

		// Token: 0x06000CE7 RID: 3303 RVA: 0x0003FDA7 File Offset: 0x0003DFA7
		public void GiveMoney(uint amount)
		{
			this.money += amount;
			StatManager.OnGoldCollected(this, (ulong)amount);
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000CE8 RID: 3304 RVA: 0x0003FDBF File Offset: 0x0003DFBF
		public float luck
		{
			get
			{
				if (this.inventory)
				{
					return (float)this.inventory.GetItemCount(ItemIndex.Clover);
				}
				return 0f;
			}
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x0003FDE4 File Offset: 0x0003DFE4
		[Server]
		public void AddDeployable(Deployable deployable, DeployableSlot slot)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::AddDeployable(RoR2.Deployable,RoR2.DeployableSlot)' called on client");
				return;
			}
			if (deployable.ownerMaster)
			{
				Debug.LogErrorFormat("Attempted to add deployable {0} which already belongs to master {1} to master {2}.", new object[]
				{
					deployable.gameObject,
					deployable.ownerMaster.gameObject,
					base.gameObject
				});
			}
			if (this.deployablesList == null)
			{
				this.deployablesList = new List<DeployableInfo>();
			}
			int num = 0;
			int num2 = 0;
			switch (slot)
			{
			case DeployableSlot.EngiMine:
				num2 = 10;
				break;
			case DeployableSlot.EngiTurret:
				num2 = 2;
				break;
			case DeployableSlot.BeetleGuardAlly:
				num2 = this.inventory.GetItemCount(ItemIndex.BeetleGland);
				break;
			case DeployableSlot.EngiBubbleShield:
				num2 = 1;
				break;
			}
			for (int i = this.deployablesList.Count - 1; i >= 0; i--)
			{
				if (this.deployablesList[i].slot == slot)
				{
					num++;
					if (num >= num2)
					{
						Deployable deployable2 = this.deployablesList[i].deployable;
						this.deployablesList.RemoveAt(i);
						deployable2.ownerMaster = null;
						deployable2.onUndeploy.Invoke();
					}
				}
			}
			this.deployablesList.Add(new DeployableInfo
			{
				deployable = deployable,
				slot = slot
			});
			deployable.ownerMaster = this;
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x0003FF1C File Offset: 0x0003E11C
		[Server]
		public int GetDeployableCount(DeployableSlot slot)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Int32 RoR2.CharacterMaster::GetDeployableCount(RoR2.DeployableSlot)' called on client");
				return 0;
			}
			if (this.deployablesList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = this.deployablesList.Count - 1; i >= 0; i--)
			{
				if (this.deployablesList[i].slot == slot)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x0003FF7C File Offset: 0x0003E17C
		[Server]
		public void RemoveDeployable(Deployable deployable)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::RemoveDeployable(RoR2.Deployable)' called on client");
				return;
			}
			if (this.deployablesList == null || deployable.ownerMaster != this)
			{
				return;
			}
			deployable.ownerMaster = null;
			for (int i = this.deployablesList.Count - 1; i >= 0; i--)
			{
				if (this.deployablesList[i].deployable == deployable)
				{
					this.deployablesList.RemoveAt(i);
				}
			}
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x0003FFFC File Offset: 0x0003E1FC
		[Server]
		public CharacterBody SpawnBody(GameObject bodyPrefab, Vector3 position, Quaternion rotation)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterBody RoR2.CharacterMaster::SpawnBody(UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
				return null;
			}
			if (this.bodyInstanceObject)
			{
				Debug.LogError("Character cannot have more than one body at this time.");
				return null;
			}
			if (!bodyPrefab)
			{
				Debug.LogErrorFormat("Attempted to spawn body of character master {0} with no body prefab.", new object[]
				{
					base.gameObject
				});
			}
			if (!bodyPrefab.GetComponent<CharacterBody>())
			{
				Debug.LogErrorFormat("Attempted to spawn body of character master {0} with a body prefab that has no {1} component attached.", new object[]
				{
					base.gameObject,
					typeof(CharacterBody).Name
				});
			}
			bool flag = bodyPrefab.GetComponent<CharacterDirection>();
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(bodyPrefab, position, flag ? Quaternion.identity : rotation);
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			component.masterObject = base.gameObject;
			if (flag)
			{
				CharacterDirection component2 = gameObject.GetComponent<CharacterDirection>();
				float y = rotation.eulerAngles.y;
				component2.yaw = y;
			}
			NetworkConnection clientAuthorityOwner = base.GetComponent<NetworkIdentity>().clientAuthorityOwner;
			if (clientAuthorityOwner != null)
			{
				NetworkServer.SpawnWithClientAuthority(gameObject, clientAuthorityOwner);
			}
			else
			{
				NetworkServer.Spawn(gameObject);
			}
			this.bodyInstanceObject = gameObject;
			Run.instance.OnServerCharacterBodySpawned(component);
			return component;
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x0004011E File Offset: 0x0003E31E
		[Server]
		public void DestroyBody()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::DestroyBody()' called on client");
				return;
			}
			if (this.bodyInstanceObject != null)
			{
				UnityEngine.Object.Destroy(this.bodyInstanceObject);
				this.bodyInstanceObject = null;
			}
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x00040155 File Offset: 0x0003E355
		public GameObject GetBodyObject()
		{
			return this.bodyInstanceObject;
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000CEF RID: 3311 RVA: 0x0004015D File Offset: 0x0003E35D
		public bool alive
		{
			get
			{
				return this.bodyInstanceObject;
			}
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x0004016C File Offset: 0x0003E36C
		public CharacterBody GetBody()
		{
			GameObject bodyObject = this.GetBodyObject();
			if (!bodyObject)
			{
				return null;
			}
			return bodyObject.GetComponent<CharacterBody>();
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x00040190 File Offset: 0x0003E390
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.inventory = base.GetComponent<Inventory>();
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x000401AA File Offset: 0x0003E3AA
		private void Start()
		{
			this.UpdateAuthority();
			if (this.spawnOnStart && NetworkServer.active)
			{
				this.SpawnBodyHere();
			}
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x000401C7 File Offset: 0x0003E3C7
		[Server]
		public void SpawnBodyHere()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::SpawnBodyHere()' called on client");
				return;
			}
			this.SpawnBody(this.bodyPrefab, base.transform.position, base.transform.rotation);
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x00040201 File Offset: 0x0003E401
		private void OnEnable()
		{
			CharacterMaster.instancesList.Add(this);
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x0004020E File Offset: 0x0003E40E
		private void OnDisable()
		{
			CharacterMaster.instancesList.Remove(this);
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x0004021C File Offset: 0x0003E41C
		private void OnDestroy()
		{
			if (this.isBoss)
			{
				this.isBoss = false;
			}
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x00040230 File Offset: 0x0003E430
		public void OnBodyStart(CharacterBody body)
		{
			this.preventGameOver = true;
			this.killerBodyIndex = -1;
			TeamComponent component = body.GetComponent<TeamComponent>();
			if (component)
			{
				component.teamIndex = this.teamIndex;
			}
			body.RecalculateStats();
			if (NetworkServer.active)
			{
				BaseAI[] components = base.GetComponents<BaseAI>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnBodyStart(body);
				}
			}
			bool flag = false;
			PlayerCharacterMasterController component2 = base.GetComponent<PlayerCharacterMasterController>();
			if (component2)
			{
				if (component2.networkUserObject)
				{
					flag = component2.networkUserObject.GetComponent<NetworkIdentity>().isLocalPlayer;
				}
				component2.OnBodyStart();
			}
			if (flag)
			{
				GlobalEventManager.instance.OnLocalPlayerBodySpawn(body);
			}
			if (this.inventory.GetItemCount(ItemIndex.Ghost) > 0)
			{
				Util.PlaySound("Play_item_proc_ghostOnKill", body.gameObject);
			}
			if (NetworkServer.active)
			{
				HealthComponent component3 = body.GetComponent<HealthComponent>();
				if (component3)
				{
					component3.Networkhealth = component3.fullHealth;
				}
				this.UpdateBodyGodMode();
				this.StartLifeStopwatch();
				GlobalEventManager.instance.OnCharacterBodySpawn(body);
			}
			Action<CharacterBody> action = this.onBodyStart;
			if (action == null)
			{
				return;
			}
			action(body);
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x00040348 File Offset: 0x0003E548
		public void OnBodyDeath()
		{
			if (NetworkServer.active)
			{
				this.deathFootPosition = this.GetBody().footPosition;
				BaseAI[] components = base.GetComponents<BaseAI>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnBodyDeath();
				}
				PlayerCharacterMasterController component = base.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					component.OnBodyDeath();
				}
				if (this.inventory.GetItemCount(ItemIndex.ExtraLife) > 0)
				{
					this.inventory.RemoveItem(ItemIndex.ExtraLife, 1);
					base.Invoke("RespawnExtraLife", 2f);
					base.Invoke("PlayExtraLifeSFX", 1f);
				}
				else
				{
					if (this.destroyOnBodyDeath)
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
					this.preventGameOver = false;
					this.preventRespawnUntilNextStageServer = true;
				}
				this.ResetLifeStopwatch();
			}
			UnityEvent unityEvent = this.onBodyDeath;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x0004041C File Offset: 0x0003E61C
		public void TrueKill()
		{
			int itemCount = this.inventory.GetItemCount(ItemIndex.ExtraLife);
			this.inventory.ResetItem(ItemIndex.ExtraLife);
			this.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, itemCount);
			CharacterBody body = this.GetBody();
			if (body)
			{
				body.healthComponent.Suicide(null);
			}
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x00040470 File Offset: 0x0003E670
		private void PlayExtraLifeSFX()
		{
			GameObject bodyInstanceObject = this.bodyInstanceObject;
			if (bodyInstanceObject)
			{
				Util.PlaySound("Play_item_proc_extraLife", bodyInstanceObject);
			}
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x00040498 File Offset: 0x0003E698
		public void RespawnExtraLife()
		{
			this.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, 1);
			this.Respawn(this.deathFootPosition, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f));
			this.GetBody().AddTimedBuff(BuffIndex.Immune, 3f);
			GameObject gameObject = Resources.Load<GameObject>("Prefabs/Effects/HippoRezEffect");
			if (this.bodyInstanceObject)
			{
				foreach (EntityStateMachine entityStateMachine in this.bodyInstanceObject.GetComponents<EntityStateMachine>())
				{
					entityStateMachine.initialStateType = entityStateMachine.mainStateType;
				}
				if (gameObject)
				{
					EffectManager.instance.SpawnEffect(gameObject, new EffectData
					{
						origin = this.deathFootPosition,
						rotation = this.bodyInstanceObject.transform.rotation
					}, true);
				}
			}
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x0004056C File Offset: 0x0003E76C
		public void OnBodyDamaged(DamageInfo damageInfo)
		{
			BaseAI[] components = base.GetComponents<BaseAI>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].OnBodyDamaged(damageInfo);
			}
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x00040598 File Offset: 0x0003E798
		public void OnBodyDestroyed()
		{
			if (NetworkServer.active)
			{
				BaseAI[] components = base.GetComponents<BaseAI>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].OnBodyDestroyed();
				}
				this.PauseLifeStopwatch();
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000CFE RID: 3326 RVA: 0x000405CF File Offset: 0x0003E7CF
		// (set) Token: 0x06000CFF RID: 3327 RVA: 0x000405D7 File Offset: 0x0003E7D7
		private float internalSurvivalTime
		{
			get
			{
				return this._internalSurvivalTime;
			}
			set
			{
				if (value == this._internalSurvivalTime)
				{
					return;
				}
				base.SetDirtyBit(4u);
				this._internalSurvivalTime = value;
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000D00 RID: 3328 RVA: 0x000405F1 File Offset: 0x0003E7F1
		public float currentLifeStopwatch
		{
			get
			{
				if (this.internalSurvivalTime <= 0f)
				{
					return -this.internalSurvivalTime;
				}
				if (Run.instance)
				{
					return Run.instance.fixedTime - this.internalSurvivalTime;
				}
				return 0f;
			}
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x0004062B File Offset: 0x0003E82B
		[Server]
		private void StartLifeStopwatch()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::StartLifeStopwatch()' called on client");
				return;
			}
			if (this.internalSurvivalTime > 0f)
			{
				return;
			}
			this.internalSurvivalTime = Run.instance.fixedTime - this.currentLifeStopwatch;
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x00040667 File Offset: 0x0003E867
		[Server]
		private void PauseLifeStopwatch()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::PauseLifeStopwatch()' called on client");
				return;
			}
			if (this.internalSurvivalTime <= 0f)
			{
				return;
			}
			this.internalSurvivalTime = -this.currentLifeStopwatch;
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x00040699 File Offset: 0x0003E899
		[Server]
		private void ResetLifeStopwatch()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::ResetLifeStopwatch()' called on client");
				return;
			}
			this.internalSurvivalTime = 0f;
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x000406BB File Offset: 0x0003E8BB
		[Server]
		public int GetKillerBodyIndex()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Int32 RoR2.CharacterMaster::GetKillerBodyIndex()' called on client");
				return 0;
			}
			return this.killerBodyIndex;
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x000406DC File Offset: 0x0003E8DC
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			GlobalEventManager.onCharacterDeathGlobal += delegate(DamageReport damageReport)
			{
				CharacterMaster victimMaster = damageReport.victimMaster;
				if (victimMaster)
				{
					victimMaster.killerBodyIndex = BodyCatalog.FindBodyIndex(damageReport.damageInfo.attacker);
				}
			};
			Stage.onServerStageBegin += delegate(Stage stage)
			{
				foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
				{
					characterMaster.preventRespawnUntilNextStageServer = false;
				}
			};
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x00040731 File Offset: 0x0003E931
		[Command]
		public void CmdRespawn(string bodyName)
		{
			if (this.preventRespawnUntilNextStageServer)
			{
				return;
			}
			if (!string.IsNullOrEmpty(bodyName))
			{
				this.bodyPrefab = BodyCatalog.FindBodyPrefab(bodyName);
			}
			if (Stage.instance)
			{
				Stage.instance.RespawnCharacter(this);
			}
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x00040768 File Offset: 0x0003E968
		[Server]
		public CharacterBody Respawn(Vector3 footPosition, Quaternion rotation)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterBody RoR2.CharacterMaster::Respawn(UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
				return null;
			}
			this.DestroyBody();
			if (this.bodyPrefab)
			{
				Vector3 position = footPosition;
				position.y += Util.GetBodyPrefabFootOffset(this.bodyPrefab);
				return this.SpawnBody(this.bodyPrefab, position, rotation);
			}
			return null;
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x000407D1 File Offset: 0x0003E9D1
		private void ToggleGod()
		{
			this.godMode = !this.godMode;
			this.UpdateBodyGodMode();
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x000407E8 File Offset: 0x0003E9E8
		private void UpdateBodyGodMode()
		{
			if (this.bodyInstanceObject)
			{
				HealthComponent component = this.bodyInstanceObject.GetComponent<HealthComponent>();
				if (component)
				{
					component.godMode = this.godMode;
				}
			}
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x00040824 File Offset: 0x0003EA24
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 15u;
			}
			bool flag = (num & 1u) > 0u;
			bool flag2 = (num & 2u) > 0u;
			bool flag3 = (num & 4u) > 0u;
			bool flag4 = (num & 8u) > 0u;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write(this._bodyInstanceId);
			}
			if (flag2)
			{
				writer.WritePackedUInt32(this._money);
			}
			if (flag3)
			{
				writer.Write(this._internalSurvivalTime);
			}
			if (flag4)
			{
				writer.Write(this.teamIndex);
			}
			return num > 0u;
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x000408A4 File Offset: 0x0003EAA4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			bool flag = (b & 1) > 0;
			bool flag2 = (b & 2) > 0;
			bool flag3 = (b & 4) > 0;
			bool flag4 = (b & 8) > 0;
			if (flag)
			{
				NetworkInstanceId value = reader.ReadNetworkId();
				this.OnSyncBodyInstanceId(value);
			}
			if (flag2)
			{
				this._money = reader.ReadPackedUInt32();
			}
			if (flag3)
			{
				this._internalSurvivalTime = reader.ReadSingle();
			}
			if (flag4)
			{
				this.teamIndex = reader.ReadTeamIndex();
			}
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x0004094C File Offset: 0x0003EB4C
		static CharacterMaster()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterMaster), CharacterMaster.kCmdCmdRespawn, new NetworkBehaviour.CmdDelegate(CharacterMaster.InvokeCmdCmdRespawn));
			NetworkCRC.RegisterBehaviour("CharacterMaster", 0);
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x000409AB File Offset: 0x0003EBAB
		protected static void InvokeCmdCmdRespawn(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdRespawn called on client.");
				return;
			}
			((CharacterMaster)obj).CmdRespawn(reader.ReadString());
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x000409D4 File Offset: 0x0003EBD4
		public void CallCmdRespawn(string bodyName)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdRespawn called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdRespawn(bodyName);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterMaster.kCmdCmdRespawn);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(bodyName);
			base.SendCommandInternal(networkWriter, 0, "CmdRespawn");
		}

		// Token: 0x040010FB RID: 4347
		[Tooltip("The prefab of this character's body.")]
		public GameObject bodyPrefab;

		// Token: 0x040010FC RID: 4348
		[Tooltip("Whether or not to spawn the body at the position of this manager object as soon as Start runs.")]
		public bool spawnOnStart;

		// Token: 0x040010FD RID: 4349
		[SerializeField]
		[Tooltip("The team of the body.")]
		[FormerlySerializedAs("teamIndex")]
		private TeamIndex _teamIndex;

		// Token: 0x04001101 RID: 4353
		public UnityEvent onBodyDeath;

		// Token: 0x04001102 RID: 4354
		[Tooltip("Whether or not to destroy this master when the body dies.")]
		public bool destroyOnBodyDeath = true;

		// Token: 0x04001103 RID: 4355
		private static List<CharacterMaster> instancesList = new List<CharacterMaster>();

		// Token: 0x04001104 RID: 4356
		private static ReadOnlyCollection<CharacterMaster> _readOnlyInstancesList = new ReadOnlyCollection<CharacterMaster>(CharacterMaster.instancesList);

		// Token: 0x04001106 RID: 4358
		private const uint bodyDirtyBit = 1u;

		// Token: 0x04001107 RID: 4359
		private const uint moneyDirtyBit = 2u;

		// Token: 0x04001108 RID: 4360
		private const uint survivalTimeDirtyBit = 4u;

		// Token: 0x04001109 RID: 4361
		private const uint teamDirtyBit = 8u;

		// Token: 0x0400110A RID: 4362
		private const uint allDirtyBits = 15u;

		// Token: 0x0400110B RID: 4363
		private NetworkInstanceId _bodyInstanceId = NetworkInstanceId.Invalid;

		// Token: 0x0400110C RID: 4364
		private GameObject resolvedBodyInstance;

		// Token: 0x0400110D RID: 4365
		private bool bodyResolved;

		// Token: 0x0400110E RID: 4366
		private uint _money;

		// Token: 0x0400110F RID: 4367
		public bool isBoss;

		// Token: 0x04001110 RID: 4368
		[NonSerialized]
		private List<DeployableInfo> deployablesList;

		// Token: 0x04001111 RID: 4369
		public bool preventGameOver = true;

		// Token: 0x04001112 RID: 4370
		private Vector3 deathFootPosition = Vector3.zero;

		// Token: 0x04001113 RID: 4371
		private Vector3 deathAimVector = Vector3.zero;

		// Token: 0x04001114 RID: 4372
		private const float respawnDelayDuration = 2f;

		// Token: 0x04001115 RID: 4373
		private float _internalSurvivalTime;

		// Token: 0x04001116 RID: 4374
		private int killerBodyIndex = -1;

		// Token: 0x04001117 RID: 4375
		private bool preventRespawnUntilNextStageServer;

		// Token: 0x04001118 RID: 4376
		private bool godMode;

		// Token: 0x04001119 RID: 4377
		private static int kCmdCmdRespawn = 1097984413;
	}
}
