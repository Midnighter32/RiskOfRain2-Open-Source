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
	// Token: 0x0200018F RID: 399
	[RequireComponent(typeof(MinionOwnership))]
	[RequireComponent(typeof(Inventory))]
	[DisallowMultipleComponent]
	public class CharacterMaster : NetworkBehaviour
	{
		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000811 RID: 2065 RVA: 0x0002312C File Offset: 0x0002132C
		// (set) Token: 0x06000812 RID: 2066 RVA: 0x00023134 File Offset: 0x00021334
		public NetworkIdentity networkIdentity { get; private set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000813 RID: 2067 RVA: 0x0002313D File Offset: 0x0002133D
		// (set) Token: 0x06000814 RID: 2068 RVA: 0x00023145 File Offset: 0x00021345
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06000815 RID: 2069 RVA: 0x0002314E File Offset: 0x0002134E
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.networkIdentity);
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x00023161 File Offset: 0x00021361
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0002316F File Offset: 0x0002136F
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
			base.OnStopAuthority();
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000818 RID: 2072 RVA: 0x00023180 File Offset: 0x00021380
		// (remove) Token: 0x06000819 RID: 2073 RVA: 0x000231B8 File Offset: 0x000213B8
		public event Action<CharacterBody> onBodyStart;

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x000231ED File Offset: 0x000213ED
		// (set) Token: 0x0600081B RID: 2075 RVA: 0x000231F5 File Offset: 0x000213F5
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
					base.SetDirtyBit(8U);
				}
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x0600081C RID: 2076 RVA: 0x00023216 File Offset: 0x00021416
		public static ReadOnlyCollection<CharacterMaster> readOnlyInstancesList
		{
			get
			{
				return CharacterMaster._readOnlyInstancesList;
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600081E RID: 2078 RVA: 0x00023226 File Offset: 0x00021426
		// (set) Token: 0x0600081D RID: 2077 RVA: 0x0002321D File Offset: 0x0002141D
		public Inventory inventory { get; private set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600081F RID: 2079 RVA: 0x0002322E File Offset: 0x0002142E
		// (set) Token: 0x06000820 RID: 2080 RVA: 0x00023236 File Offset: 0x00021436
		public PlayerCharacterMasterController playerCharacterMasterController { get; private set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000821 RID: 2081 RVA: 0x0002323F File Offset: 0x0002143F
		// (set) Token: 0x06000822 RID: 2082 RVA: 0x00023247 File Offset: 0x00021447
		public PlayerStatsComponent playerStatsComponent { get; private set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000823 RID: 2083 RVA: 0x00023250 File Offset: 0x00021450
		// (set) Token: 0x06000824 RID: 2084 RVA: 0x00023258 File Offset: 0x00021458
		public MinionOwnership minionOwnership { get; private set; }

		// Token: 0x06000825 RID: 2085 RVA: 0x00023261 File Offset: 0x00021461
		[Server]
		public void SetLoadoutServer(Loadout newLoadout)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::SetLoadoutServer(RoR2.Loadout)' called on client");
				return;
			}
			newLoadout.Copy(this.loadout);
			base.SetDirtyBit(16U);
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000826 RID: 2086 RVA: 0x0002328C File Offset: 0x0002148C
		// (set) Token: 0x06000827 RID: 2087 RVA: 0x00023294 File Offset: 0x00021494
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
				base.SetDirtyBit(1U);
				this._bodyInstanceId = value;
			}
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x000232B3 File Offset: 0x000214B3
		private void OnSyncBodyInstanceId(NetworkInstanceId value)
		{
			this.resolvedBodyInstance = null;
			this.bodyResolved = (value == NetworkInstanceId.Invalid);
			this._bodyInstanceId = value;
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000829 RID: 2089 RVA: 0x000232D4 File Offset: 0x000214D4
		// (set) Token: 0x0600082A RID: 2090 RVA: 0x0002330C File Offset: 0x0002150C
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

		// Token: 0x0600082B RID: 2091 RVA: 0x00023359 File Offset: 0x00021559
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

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x0600082C RID: 2092 RVA: 0x00023381 File Offset: 0x00021581
		// (set) Token: 0x0600082D RID: 2093 RVA: 0x00023389 File Offset: 0x00021589
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
				base.SetDirtyBit(2U);
				this._money = value;
			}
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x000233A3 File Offset: 0x000215A3
		public void GiveMoney(uint amount)
		{
			this.money += amount;
			StatManager.OnGoldCollected(this, (ulong)amount);
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600082F RID: 2095 RVA: 0x000233BB File Offset: 0x000215BB
		// (set) Token: 0x06000830 RID: 2096 RVA: 0x000233C3 File Offset: 0x000215C3
		public float luck { get; set; }

		// Token: 0x06000831 RID: 2097 RVA: 0x000233CC File Offset: 0x000215CC
		public int GetDeployableSameSlotLimit(DeployableSlot slot)
		{
			int result = 0;
			switch (slot)
			{
			case DeployableSlot.EngiMine:
				result = 4;
				if (this.bodyInstanceObject)
				{
					result = this.bodyInstanceObject.GetComponent<SkillLocator>().secondary.maxStock;
				}
				break;
			case DeployableSlot.EngiTurret:
				result = 2;
				break;
			case DeployableSlot.BeetleGuardAlly:
				result = this.inventory.GetItemCount(ItemIndex.BeetleGland);
				break;
			case DeployableSlot.EngiBubbleShield:
				result = 1;
				break;
			case DeployableSlot.LoaderPylon:
				result = 3;
				break;
			case DeployableSlot.EngiSpiderMine:
				result = 4;
				if (this.bodyInstanceObject)
				{
					result = this.bodyInstanceObject.GetComponent<SkillLocator>().secondary.maxStock;
				}
				break;
			case DeployableSlot.RoboBallMini:
				result = 3;
				break;
			}
			return result;
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0002346C File Offset: 0x0002166C
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
			int deployableSameSlotLimit = this.GetDeployableSameSlotLimit(slot);
			for (int i = this.deployablesList.Count - 1; i >= 0; i--)
			{
				if (this.deployablesList[i].slot == slot)
				{
					num++;
					if (num >= deployableSameSlotLimit)
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

		// Token: 0x06000833 RID: 2099 RVA: 0x00023578 File Offset: 0x00021778
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

		// Token: 0x06000834 RID: 2100 RVA: 0x000235D8 File Offset: 0x000217D8
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

		// Token: 0x06000835 RID: 2101 RVA: 0x00023658 File Offset: 0x00021858
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
			component.teamComponent.teamIndex = this.teamIndex;
			component.SetLoadoutServer(this.loadout);
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

		// Token: 0x06000836 RID: 2102 RVA: 0x00023798 File Offset: 0x00021998
		[Server]
		public void DestroyBody()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterMaster::DestroyBody()' called on client");
				return;
			}
			if (this.bodyInstanceObject)
			{
				CharacterBody body = this.GetBody();
				UnityEngine.Object.Destroy(this.bodyInstanceObject);
				this.OnBodyDestroyed(body);
				this.bodyInstanceObject = null;
			}
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x000237E7 File Offset: 0x000219E7
		public GameObject GetBodyObject()
		{
			return this.bodyInstanceObject;
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000838 RID: 2104 RVA: 0x000237EF File Offset: 0x000219EF
		public bool alive
		{
			get
			{
				return this.bodyInstanceObject;
			}
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x000237FC File Offset: 0x000219FC
		public CharacterBody GetBody()
		{
			GameObject bodyObject = this.GetBodyObject();
			if (!bodyObject)
			{
				return null;
			}
			return bodyObject.GetComponent<CharacterBody>();
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x00023820 File Offset: 0x00021A20
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.inventory = base.GetComponent<Inventory>();
			this.aiComponents = (NetworkServer.active ? base.GetComponents<BaseAI>() : Array.Empty<BaseAI>());
			this.playerCharacterMasterController = base.GetComponent<PlayerCharacterMasterController>();
			this.playerStatsComponent = base.GetComponent<PlayerStatsComponent>();
			this.minionOwnership = base.GetComponent<MinionOwnership>();
			this.inventory.onInventoryChanged += this.OnInventoryChanged;
			this.OnInventoryChanged();
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x000238A0 File Offset: 0x00021AA0
		private void Start()
		{
			this.UpdateAuthority();
			if (NetworkServer.active && this.spawnOnStart && !this.bodyInstanceObject)
			{
				this.SpawnBodyHere();
			}
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x000238CA File Offset: 0x00021ACA
		private void OnInventoryChanged()
		{
			this.luck = (float)this.inventory.GetItemCount(ItemIndex.Clover);
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x000238E0 File Offset: 0x00021AE0
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

		// Token: 0x0600083E RID: 2110 RVA: 0x0002391A File Offset: 0x00021B1A
		private void OnEnable()
		{
			CharacterMaster.instancesList.Add(this);
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x00023927 File Offset: 0x00021B27
		private void OnDisable()
		{
			CharacterMaster.instancesList.Remove(this);
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x00023935 File Offset: 0x00021B35
		private void OnDestroy()
		{
			if (this.isBoss)
			{
				this.isBoss = false;
			}
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x00023948 File Offset: 0x00021B48
		public void OnBodyStart(CharacterBody body)
		{
			this.preventGameOver = true;
			this.killerBodyIndex = -1;
			body.RecalculateStats();
			if (NetworkServer.active)
			{
				BaseAI[] array = this.aiComponents;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnBodyStart(body);
				}
			}
			bool flag = false;
			if (this.playerCharacterMasterController)
			{
				if (this.playerCharacterMasterController.networkUserObject)
				{
					flag = this.playerCharacterMasterController.networkUserObject.GetComponent<NetworkIdentity>().isLocalPlayer;
				}
				this.playerCharacterMasterController.OnBodyStart();
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
				HealthComponent healthComponent = body.healthComponent;
				if (healthComponent)
				{
					healthComponent.Networkhealth = healthComponent.fullHealth;
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

		// Token: 0x06000842 RID: 2114 RVA: 0x00023A49 File Offset: 0x00021C49
		[Server]
		public bool IsExtraLifePendingServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.CharacterMaster::IsExtraLifePendingServer()' called on client");
				return false;
			}
			return base.IsInvoking("RespawnExtraLife");
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x00023A6C File Offset: 0x00021C6C
		public void OnBodyDeath(CharacterBody body)
		{
			if (NetworkServer.active)
			{
				this.deathFootPosition = body.footPosition;
				BaseAI[] array = this.aiComponents;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnBodyDeath(body);
				}
				if (this.playerCharacterMasterController)
				{
					this.playerCharacterMasterController.OnBodyDeath();
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
						UnityEngine.Object.Destroy(base.gameObject, 1f);
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

		// Token: 0x06000844 RID: 2116 RVA: 0x00023B44 File Offset: 0x00021D44
		public void TrueKill()
		{
			int itemCount = this.inventory.GetItemCount(ItemIndex.ExtraLife);
			this.inventory.ResetItem(ItemIndex.ExtraLife);
			this.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, itemCount);
			base.CancelInvoke("RespawnExtraLife");
			base.CancelInvoke("PlayExtraLifeSFX");
			CharacterBody body = this.GetBody();
			if (body)
			{
				body.healthComponent.Suicide(null, null, DamageType.Generic);
			}
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x00023BB0 File Offset: 0x00021DB0
		private void PlayExtraLifeSFX()
		{
			GameObject bodyInstanceObject = this.bodyInstanceObject;
			if (bodyInstanceObject)
			{
				Util.PlaySound("Play_item_proc_extraLife", bodyInstanceObject);
			}
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x00023BD8 File Offset: 0x00021DD8
		public void RespawnExtraLife()
		{
			this.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, 1);
			this.Respawn(this.deathFootPosition, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), false);
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
					EffectManager.SpawnEffect(gameObject, new EffectData
					{
						origin = this.deathFootPosition,
						rotation = this.bodyInstanceObject.transform.rotation
					}, true);
				}
			}
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x00023CA8 File Offset: 0x00021EA8
		public void OnBodyDamaged(DamageReport damageReport)
		{
			BaseAI[] array = this.aiComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnBodyDamaged(damageReport);
			}
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x00023CD4 File Offset: 0x00021ED4
		public void OnBodyDestroyed(CharacterBody characterBody)
		{
			if (characterBody != this.GetBody())
			{
				return;
			}
			if (NetworkServer.active)
			{
				BaseAI[] array = this.aiComponents;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnBodyDestroyed(characterBody);
				}
				this.PauseLifeStopwatch();
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000849 RID: 2121 RVA: 0x00023D16 File Offset: 0x00021F16
		// (set) Token: 0x0600084A RID: 2122 RVA: 0x00023D1E File Offset: 0x00021F1E
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
				base.SetDirtyBit(4U);
				this._internalSurvivalTime = value;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x0600084B RID: 2123 RVA: 0x00023D38 File Offset: 0x00021F38
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
					return Run.instance.GetRunStopwatch() - this.internalSurvivalTime;
				}
				return 0f;
			}
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x00023D72 File Offset: 0x00021F72
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
			this.internalSurvivalTime = Run.instance.GetRunStopwatch() - this.currentLifeStopwatch;
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x00023DAE File Offset: 0x00021FAE
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

		// Token: 0x0600084E RID: 2126 RVA: 0x00023DE0 File Offset: 0x00021FE0
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

		// Token: 0x0600084F RID: 2127 RVA: 0x00023E02 File Offset: 0x00022002
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

		// Token: 0x06000850 RID: 2128 RVA: 0x00023E20 File Offset: 0x00022020
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

		// Token: 0x06000851 RID: 2129 RVA: 0x00023E75 File Offset: 0x00022075
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

		// Token: 0x06000852 RID: 2130 RVA: 0x00023EAC File Offset: 0x000220AC
		[Server]
		public CharacterBody Respawn(Vector3 footPosition, Quaternion rotation, bool tryToGroundSafely = false)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterBody RoR2.CharacterMaster::Respawn(UnityEngine.Vector3,UnityEngine.Quaternion,System.Boolean)' called on client");
				return null;
			}
			this.DestroyBody();
			if (this.bodyPrefab)
			{
				CharacterBody component = this.bodyPrefab.GetComponent<CharacterBody>();
				if (component)
				{
					Vector3 vector = footPosition;
					if (tryToGroundSafely)
					{
						Vector3 vector2 = vector;
						RaycastHit raycastHit = default(RaycastHit);
						Ray ray = new Ray(footPosition + Vector3.up * 1f, Vector3.down);
						float maxDistance = 3f;
						if (Physics.SphereCast(ray, component.radius, out raycastHit, maxDistance, LayerIndex.world.mask))
						{
							vector2.y += 1f - raycastHit.distance;
						}
						vector = vector2;
					}
					Vector3 position = new Vector3(vector.x, vector.y + Util.GetBodyPrefabFootOffset(this.bodyPrefab), vector.z);
					return this.SpawnBody(this.bodyPrefab, position, rotation);
				}
				Debug.LogErrorFormat("Trying to respawn as object {0} who has no Character Body!", new object[]
				{
					this.bodyPrefab
				});
			}
			else
			{
				Debug.LogErrorFormat("CharacterMaster.Respawn failed. {0} does not have a valid body prefab assigned.", new object[]
				{
					base.gameObject.name
				});
			}
			return null;
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x00023FE8 File Offset: 0x000221E8
		private void ToggleGod()
		{
			this.godMode = !this.godMode;
			this.UpdateBodyGodMode();
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x00024000 File Offset: 0x00022200
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

		// Token: 0x06000855 RID: 2133 RVA: 0x0002403C File Offset: 0x0002223C
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 31U;
			}
			bool flag = (num & 1U) > 0U;
			bool flag2 = (num & 2U) > 0U;
			bool flag3 = (num & 4U) > 0U;
			bool flag4 = (num & 8U) > 0U;
			bool flag5 = (num & 16U) > 0U;
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
			if (flag5)
			{
				this.loadout.Serialize(writer);
			}
			return num > 0U;
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x000240D4 File Offset: 0x000222D4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			bool flag = (b & 1) > 0;
			bool flag2 = (b & 2) > 0;
			bool flag3 = (b & 4) > 0;
			bool flag4 = (b & 8) > 0;
			bool flag5 = (b & 16) > 0;
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
			if (flag5)
			{
				this.loadout.Deserialize(reader);
			}
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x000241AC File Offset: 0x000223AC
		static CharacterMaster()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterMaster), CharacterMaster.kCmdCmdRespawn, new NetworkBehaviour.CmdDelegate(CharacterMaster.InvokeCmdCmdRespawn));
			NetworkCRC.RegisterBehaviour("CharacterMaster", 0);
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x0002420B File Offset: 0x0002240B
		protected static void InvokeCmdCmdRespawn(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdRespawn called on client.");
				return;
			}
			((CharacterMaster)obj).CmdRespawn(reader.ReadString());
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x00024234 File Offset: 0x00022434
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

		// Token: 0x0400088D RID: 2189
		[Tooltip("The prefab of this character's body.")]
		public GameObject bodyPrefab;

		// Token: 0x0400088E RID: 2190
		[Tooltip("Whether or not to spawn the body at the position of this manager object as soon as Start runs.")]
		public bool spawnOnStart;

		// Token: 0x0400088F RID: 2191
		[Tooltip("The team of the body.")]
		[SerializeField]
		[FormerlySerializedAs("teamIndex")]
		private TeamIndex _teamIndex;

		// Token: 0x04000893 RID: 2195
		public UnityEvent onBodyDeath;

		// Token: 0x04000894 RID: 2196
		[Tooltip("Whether or not to destroy this master when the body dies.")]
		public bool destroyOnBodyDeath = true;

		// Token: 0x04000895 RID: 2197
		private static List<CharacterMaster> instancesList = new List<CharacterMaster>();

		// Token: 0x04000896 RID: 2198
		private static ReadOnlyCollection<CharacterMaster> _readOnlyInstancesList = new ReadOnlyCollection<CharacterMaster>(CharacterMaster.instancesList);

		// Token: 0x04000898 RID: 2200
		private BaseAI[] aiComponents;

		// Token: 0x0400089C RID: 2204
		private const uint bodyDirtyBit = 1U;

		// Token: 0x0400089D RID: 2205
		private const uint moneyDirtyBit = 2U;

		// Token: 0x0400089E RID: 2206
		private const uint survivalTimeDirtyBit = 4U;

		// Token: 0x0400089F RID: 2207
		private const uint teamDirtyBit = 8U;

		// Token: 0x040008A0 RID: 2208
		private const uint loadoutDirtyBit = 16U;

		// Token: 0x040008A1 RID: 2209
		private const uint allDirtyBits = 31U;

		// Token: 0x040008A2 RID: 2210
		public readonly Loadout loadout = new Loadout();

		// Token: 0x040008A3 RID: 2211
		private NetworkInstanceId _bodyInstanceId = NetworkInstanceId.Invalid;

		// Token: 0x040008A4 RID: 2212
		private GameObject resolvedBodyInstance;

		// Token: 0x040008A5 RID: 2213
		private bool bodyResolved;

		// Token: 0x040008A6 RID: 2214
		private uint _money;

		// Token: 0x040008A8 RID: 2216
		public bool isBoss;

		// Token: 0x040008A9 RID: 2217
		[NonSerialized]
		private List<DeployableInfo> deployablesList;

		// Token: 0x040008AA RID: 2218
		public bool preventGameOver = true;

		// Token: 0x040008AB RID: 2219
		private Vector3 deathFootPosition = Vector3.zero;

		// Token: 0x040008AC RID: 2220
		private Vector3 deathAimVector = Vector3.zero;

		// Token: 0x040008AD RID: 2221
		private const float respawnDelayDuration = 2f;

		// Token: 0x040008AE RID: 2222
		private float _internalSurvivalTime;

		// Token: 0x040008AF RID: 2223
		private int killerBodyIndex = -1;

		// Token: 0x040008B0 RID: 2224
		private bool preventRespawnUntilNextStageServer;

		// Token: 0x040008B1 RID: 2225
		private bool godMode;

		// Token: 0x040008B2 RID: 2226
		private static int kCmdCmdRespawn = 1097984413;
	}
}
