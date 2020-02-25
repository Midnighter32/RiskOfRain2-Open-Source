using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using RoR2.CharacterAI;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200034E RID: 846
	[RequireComponent(typeof(SceneExitController))]
	public sealed class TeleporterInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider
	{
		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06001454 RID: 5204 RVA: 0x00056A3B File Offset: 0x00054C3B
		// (set) Token: 0x06001455 RID: 5205 RVA: 0x00056A43 File Offset: 0x00054C43
		private TeleporterInteraction.ActivationState activationState
		{
			get
			{
				return (TeleporterInteraction.ActivationState)this.activationStateInternal;
			}
			set
			{
				this.NetworkactivationStateInternal = (uint)value;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06001456 RID: 5206 RVA: 0x00056A4C File Offset: 0x00054C4C
		public bool isIdle
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.Idle;
			}
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06001457 RID: 5207 RVA: 0x00056A57 File Offset: 0x00054C57
		public bool isIdleToCharging
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.IdleToCharging;
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06001458 RID: 5208 RVA: 0x00056A62 File Offset: 0x00054C62
		public bool isInFinalSequence
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.Finished;
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06001459 RID: 5209 RVA: 0x00056A6D File Offset: 0x00054C6D
		public bool isCharging
		{
			get
			{
				return this.activationState == TeleporterInteraction.ActivationState.Charging;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x0600145A RID: 5210 RVA: 0x00056A78 File Offset: 0x00054C78
		public bool isCharged
		{
			get
			{
				return this.activationState >= TeleporterInteraction.ActivationState.Charged;
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x0600145B RID: 5211 RVA: 0x00056A86 File Offset: 0x00054C86
		public float chargeFraction
		{
			get
			{
				return this.chargePercent * 0.01f;
			}
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x0600145C RID: 5212 RVA: 0x00056A96 File Offset: 0x00054C96
		// (set) Token: 0x0600145D RID: 5213 RVA: 0x00056A9D File Offset: 0x00054C9D
		public static TeleporterInteraction instance { get; private set; }

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x0600145E RID: 5214 RVA: 0x00056AA5 File Offset: 0x00054CA5
		// (set) Token: 0x0600145F RID: 5215 RVA: 0x00056AAD File Offset: 0x00054CAD
		public bool shouldAttemptToSpawnShopPortal
		{
			get
			{
				return this._shouldAttemptToSpawnShopPortal;
			}
			set
			{
				if (this._shouldAttemptToSpawnShopPortal == value)
				{
					return;
				}
				this.Network_shouldAttemptToSpawnShopPortal = value;
				if (this._shouldAttemptToSpawnShopPortal && NetworkServer.active)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "PORTAL_SHOP_WILL_OPEN"
					});
				}
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06001460 RID: 5216 RVA: 0x00056AE4 File Offset: 0x00054CE4
		// (set) Token: 0x06001461 RID: 5217 RVA: 0x00056AEC File Offset: 0x00054CEC
		public bool shouldAttemptToSpawnGoldshoresPortal
		{
			get
			{
				return this._shouldAttemptToSpawnGoldshoresPortal;
			}
			set
			{
				if (this._shouldAttemptToSpawnGoldshoresPortal == value)
				{
					return;
				}
				this.Network_shouldAttemptToSpawnGoldshoresPortal = value;
				if (this._shouldAttemptToSpawnGoldshoresPortal && NetworkServer.active)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "PORTAL_GOLDSHORES_WILL_OPEN"
					});
				}
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06001462 RID: 5218 RVA: 0x00056B23 File Offset: 0x00054D23
		// (set) Token: 0x06001463 RID: 5219 RVA: 0x00056B2B File Offset: 0x00054D2B
		public bool shouldAttemptToSpawnMSPortal
		{
			get
			{
				return this._shouldAttemptToSpawnMSPortal;
			}
			set
			{
				if (this._shouldAttemptToSpawnMSPortal == value)
				{
					return;
				}
				this.Network_shouldAttemptToSpawnMSPortal = value;
				if (this._shouldAttemptToSpawnMSPortal && NetworkServer.active)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "PORTAL_MS_WILL_OPEN"
					});
				}
			}
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x00056B62 File Offset: 0x00054D62
		private void OnSyncShouldAttemptToSpawnShopPortal(bool newValue)
		{
			this.Network_shouldAttemptToSpawnShopPortal = newValue;
			if (this.childLocator)
			{
				this.childLocator.FindChild("ShopPortalIndicator").gameObject.SetActive(newValue);
			}
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x00056B93 File Offset: 0x00054D93
		private void OnSyncShouldAttemptToSpawnGoldshoresPortal(bool newValue)
		{
			this.Network_shouldAttemptToSpawnGoldshoresPortal = newValue;
			if (this.childLocator)
			{
				this.childLocator.FindChild("GoldshoresPortalIndicator").gameObject.SetActive(newValue);
			}
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x00056BC4 File Offset: 0x00054DC4
		private void OnSyncShouldAttemptToSpawnMSPortal(bool newValue)
		{
			this.Network_shouldAttemptToSpawnMSPortal = newValue;
			if (this.childLocator)
			{
				this.childLocator.FindChild("MSPortalIndicator").gameObject.SetActive(newValue);
			}
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x00056BF8 File Offset: 0x00054DF8
		private void Awake()
		{
			this.remainingChargeTimer = this.chargeDuration;
			this.monsterCheckTimer = 0f;
			this.childLocator = base.GetComponent<ModelLocator>().modelTransform.GetComponent<ChildLocator>();
			this.bossShrineIndicator = this.childLocator.FindChild("BossShrineSymbol").gameObject;
			this.bossGroup = base.GetComponent<BossGroup>();
			if (NetworkServer.active && this.bossDirector)
			{
				this.bossDirector.onSpawnedServer.AddListener(new UnityAction<GameObject>(this.OnBossDirectorSpawnedMonsterServer));
			}
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x00056C89 File Offset: 0x00054E89
		private void OnEnable()
		{
			TeleporterInteraction.instance = SingletonHelper.Assign<TeleporterInteraction>(TeleporterInteraction.instance, this);
			InstanceTracker.Add<TeleporterInteraction>(this);
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x00056CA1 File Offset: 0x00054EA1
		private void OnDisable()
		{
			InstanceTracker.Remove<TeleporterInteraction>(this);
			TeleporterInteraction.instance = SingletonHelper.Unassign<TeleporterInteraction>(TeleporterInteraction.instance, this);
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x00056CBC File Offset: 0x00054EBC
		private void Start()
		{
			if (this.clearRadiusIndicator)
			{
				float num = this.clearRadius * 2f;
				this.clearRadiusIndicator.transform.localScale = new Vector3(num, num, num);
			}
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
				SceneDef[] destinations = SceneCatalog.mostRecentSceneDef.destinations;
				if (destinations.Length != 0)
				{
					Run.instance.PickNextStageScene(destinations);
				}
				float nextNormalizedFloat = this.rng.nextNormalizedFloat;
				float num2 = this.baseShopSpawnChance / (float)(Run.instance.shopPortalCount + 1);
				this.shouldAttemptToSpawnShopPortal = (nextNormalizedFloat < num2);
				int num3 = 4;
				int stageClearCount = Run.instance.stageClearCount;
				if ((stageClearCount + 1) % num3 == 3 && stageClearCount > num3)
				{
					this.shouldAttemptToSpawnMSPortal = true;
				}
			}
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x00056D8A File Offset: 0x00054F8A
		public string GetContextString(Interactor activator)
		{
			if (this.activationState == TeleporterInteraction.ActivationState.Idle)
			{
				return Language.GetString(this.beginContextString);
			}
			if (this.activationState == TeleporterInteraction.ActivationState.Charged)
			{
				return Language.GetString(this.exitContextString);
			}
			return null;
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x00056DB6 File Offset: 0x00054FB6
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.locked)
			{
				return Interactability.Disabled;
			}
			if (this.activationState == TeleporterInteraction.ActivationState.Idle)
			{
				return Interactability.Available;
			}
			if (this.activationState == TeleporterInteraction.ActivationState.Charged)
			{
				if (!this.monstersCleared)
				{
					return Interactability.ConditionsNotMet;
				}
				return Interactability.Available;
			}
			else
			{
				if (this.activationState == TeleporterInteraction.ActivationState.Charging)
				{
					return Interactability.ConditionsNotMet;
				}
				return Interactability.Disabled;
			}
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x00056DF0 File Offset: 0x00054FF0
		public void OnInteractionBegin(Interactor activator)
		{
			this.CallRpcClientOnActivated(activator.gameObject);
			if (this.activationState == TeleporterInteraction.ActivationState.Idle)
			{
				Chat.SendBroadcastChat(new Chat.SubjectChatMessage
				{
					subjectAsCharacterBody = activator.GetComponent<CharacterBody>(),
					baseToken = "PLAYER_ACTIVATED_TELEPORTER"
				});
				if (this.showBossIndicator)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "SHRINE_BOSS_BEGIN_TRIAL"
					});
				}
				this.activationState = TeleporterInteraction.ActivationState.IdleToCharging;
				this.chargeActivatorServer = activator.gameObject;
				return;
			}
			if (this.activationState == TeleporterInteraction.ActivationState.Charged)
			{
				this.activationState = TeleporterInteraction.ActivationState.Finished;
				base.GetComponent<SceneExitController>().Begin();
			}
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x0000B933 File Offset: 0x00009B33
		public bool ShouldShowOnScanner()
		{
			return true;
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x00056E7E File Offset: 0x0005507E
		[ClientRpc]
		private void RpcClientOnActivated(GameObject activatorObject)
		{
			Util.PlaySound("Play_env_teleporter_active_button", base.gameObject);
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x00056E91 File Offset: 0x00055091
		private void UpdateMonstersClear()
		{
			this.monstersCleared = !this.bossGroup.enabled;
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x00056EA8 File Offset: 0x000550A8
		private int GetPlayerCountInRadius()
		{
			int num = 0;
			Vector3 position = base.transform.position;
			float num2 = this.clearRadius * this.clearRadius;
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject) && (teamMembers[i].transform.position - position).sqrMagnitude <= num2)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x000407BB File Offset: 0x0003E9BB
		private int GetMonsterCount()
		{
			return TeamComponent.GetTeamMembers(TeamIndex.Monster).Count;
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x00056F2F File Offset: 0x0005512F
		private float DiminishingReturns(int i)
		{
			return (1f - Mathf.Pow(0.5f, (float)i)) * 2f;
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x00056F4C File Offset: 0x0005514C
		[Server]
		public void AddShrineStack()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::AddShrineStack()' called on client");
				return;
			}
			if (this.activationState <= TeleporterInteraction.ActivationState.IdleToCharging)
			{
				BossGroup bossGroup = this.bossGroup;
				int bonusRewardCount = bossGroup.bonusRewardCount + 1;
				bossGroup.bonusRewardCount = bonusRewardCount;
				this.shrineBonusStacks++;
				this.NetworkshowBossIndicator = true;
			}
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x00056FA4 File Offset: 0x000551A4
		public bool IsInChargingRange(GameObject gameObject)
		{
			return (gameObject.transform.position - base.transform.position).sqrMagnitude <= this.clearRadius * this.clearRadius;
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x00056FE6 File Offset: 0x000551E6
		public bool IsInChargingRange(CharacterBody characterBody)
		{
			return this.IsInChargingRange(characterBody.gameObject);
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x00056FF4 File Offset: 0x000551F4
		public void FixedUpdate()
		{
			this.bossShrineIndicator.SetActive(this.showBossIndicator);
			if (this.previousActivationState != this.activationState)
			{
				this.OnStateChanged(this.previousActivationState, this.activationState);
			}
			this.previousActivationState = this.activationState;
			this.StateFixedUpdate();
			this.UpdateHealingNovas();
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x00057050 File Offset: 0x00055250
		private void StateFixedUpdate()
		{
			switch (this.activationState)
			{
			case TeleporterInteraction.ActivationState.IdleToCharging:
				this.idleToChargingStopwatch += Time.fixedDeltaTime;
				if (this.idleToChargingStopwatch > 3f)
				{
					this.activationState = TeleporterInteraction.ActivationState.Charging;
					this.TrySpawnTitanGold();
				}
				break;
			case TeleporterInteraction.ActivationState.Charging:
			{
				int num = Run.instance ? Run.instance.livingPlayerCount : 0;
				float num2 = (num != 0) ? ((float)this.GetPlayerCountInRadius() / (float)num * Time.fixedDeltaTime) : 0f;
				bool isCharging = num2 > 0f;
				this.remainingChargeTimer = Mathf.Max(this.remainingChargeTimer - num2, 0f);
				if (NetworkServer.active)
				{
					this.NetworkchargePercent = (uint)((byte)Mathf.RoundToInt(99f * (1f - this.remainingChargeTimer / this.chargeDuration)));
				}
				if (SceneWeatherController.instance)
				{
					SceneWeatherController.instance.weatherLerp = SceneWeatherController.instance.weatherLerpOverChargeTime.Evaluate(1f - this.remainingChargeTimer / this.chargeDuration);
				}
				if (!this.teleporterPositionIndicator)
				{
					this.teleporterPositionIndicator = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PositionIndicators/TeleporterChargingPositionIndicator"), base.transform.position, Quaternion.identity);
					this.teleporterPositionIndicator.GetComponent<PositionIndicator>().targetTransform = base.transform;
				}
				else
				{
					ChargeIndicatorController component = this.teleporterPositionIndicator.GetComponent<ChargeIndicatorController>();
					component.isCharging = isCharging;
					component.chargingText.text = this.chargePercent.ToString() + "%";
				}
				this.UpdateMonstersClear();
				if (this.remainingChargeTimer <= 0f && NetworkServer.active)
				{
					if (this.bonusDirector)
					{
						this.bonusDirector.enabled = false;
					}
					if (this.monstersCleared)
					{
						if (this.bossDirector)
						{
							this.bossDirector.enabled = false;
						}
						this.activationState = TeleporterInteraction.ActivationState.Charged;
						this.OnChargingFinished();
					}
				}
				break;
			}
			case TeleporterInteraction.ActivationState.Charged:
				this.monsterCheckTimer -= Time.fixedDeltaTime;
				if (this.monsterCheckTimer <= 0f)
				{
					this.monsterCheckTimer = 1f;
					this.UpdateMonstersClear();
				}
				this.NetworkshowBossIndicator = false;
				break;
			}
			if (this.clearRadiusIndicator)
			{
				this.clearRadiusIndicator.SetActive(this.activationState >= TeleporterInteraction.ActivationState.Charging);
			}
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x000572B0 File Offset: 0x000554B0
		private void OnBossDirectorSpawnedMonsterServer(GameObject masterObject)
		{
			if (this.chargeActivatorServer)
			{
				TeleporterInteraction.<>c__DisplayClass76_0 CS$<>8__locals1 = new TeleporterInteraction.<>c__DisplayClass76_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.ai = masterObject.GetComponent<BaseAI>();
				if (!CS$<>8__locals1.ai)
				{
					return;
				}
				CS$<>8__locals1.ai.onBodyDiscovered += CS$<>8__locals1.<OnBossDirectorSpawnedMonsterServer>g__AiOnBodyDiscovered|0;
			}
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x00057308 File Offset: 0x00055508
		private void OnStateChanged(TeleporterInteraction.ActivationState oldActivationState, TeleporterInteraction.ActivationState newActivationState)
		{
			switch (newActivationState)
			{
			case TeleporterInteraction.ActivationState.Idle:
				return;
			case TeleporterInteraction.ActivationState.IdleToCharging:
				this.childLocator.FindChild("IdleToChargingEffect").gameObject.SetActive(true);
				this.childLocator.FindChild("PPVolume").gameObject.SetActive(true);
				return;
			case TeleporterInteraction.ActivationState.Charging:
			{
				Action<TeleporterInteraction> action = TeleporterInteraction.onTeleporterBeginChargingGlobal;
				if (action != null)
				{
					action(this);
				}
				if (NetworkServer.active)
				{
					if (this.bonusDirector)
					{
						this.bonusDirector.enabled = true;
					}
					if (this.bossDirector)
					{
						this.bossDirector.enabled = true;
						this.bossDirector.monsterCredit += (float)((int)(600f * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f) * (float)(1 + this.shrineBonusStacks)));
						this.bossDirector.currentSpawnTarget = base.gameObject;
						this.bossDirector.SetNextSpawnAsBoss();
					}
					if (DirectorCore.instance)
					{
						CombatDirector[] components = DirectorCore.instance.GetComponents<CombatDirector>();
						if (components.Length != 0)
						{
							CombatDirector[] array = components;
							for (int i = 0; i < array.Length; i++)
							{
								array[i].enabled = false;
							}
						}
					}
					if (this.chestLockCoroutine == null)
					{
						this.chestLockCoroutine = base.StartCoroutine(this.ChestLockCoroutine());
					}
				}
				this.childLocator.FindChild("IdleToChargingEffect").gameObject.SetActive(false);
				this.childLocator.FindChild("ChargingEffect").gameObject.SetActive(true);
				return;
			}
			case TeleporterInteraction.ActivationState.Charged:
			{
				this.teleporterPositionIndicator.GetComponent<ChargeIndicatorController>().isCharged = true;
				this.childLocator.FindChild("ChargingEffect").gameObject.SetActive(false);
				this.childLocator.FindChild("ChargedEffect").gameObject.SetActive(true);
				this.childLocator.FindChild("BossShrineSymbol").gameObject.SetActive(false);
				Action<TeleporterInteraction> action2 = TeleporterInteraction.onTeleporterChargedGlobal;
				if (action2 == null)
				{
					return;
				}
				action2(this);
				return;
			}
			case TeleporterInteraction.ActivationState.Finished:
			{
				this.childLocator.FindChild("ChargedEffect").gameObject.SetActive(false);
				Action<TeleporterInteraction> action3 = TeleporterInteraction.onTeleporterFinishGlobal;
				if (action3 == null)
				{
					return;
				}
				action3(this);
				return;
			}
			default:
				throw new ArgumentOutOfRangeException("newActivationState", newActivationState, null);
			}
		}

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x0600147B RID: 5243 RVA: 0x00057544 File Offset: 0x00055744
		// (remove) Token: 0x0600147C RID: 5244 RVA: 0x00057578 File Offset: 0x00055778
		public static event Action<TeleporterInteraction> onTeleporterBeginChargingGlobal;

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x0600147D RID: 5245 RVA: 0x000575AC File Offset: 0x000557AC
		// (remove) Token: 0x0600147E RID: 5246 RVA: 0x000575E0 File Offset: 0x000557E0
		public static event Action<TeleporterInteraction> onTeleporterChargedGlobal;

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x0600147F RID: 5247 RVA: 0x00057614 File Offset: 0x00055814
		// (remove) Token: 0x06001480 RID: 5248 RVA: 0x00057648 File Offset: 0x00055848
		public static event Action<TeleporterInteraction> onTeleporterFinishGlobal;

		// Token: 0x06001481 RID: 5249 RVA: 0x0005767C File Offset: 0x0005587C
		[Server]
		private void OnChargingFinished()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::OnChargingFinished()' called on client");
				return;
			}
			if (this.shouldAttemptToSpawnShopPortal)
			{
				this.AttemptToSpawnShopPortal();
			}
			if (this.shouldAttemptToSpawnGoldshoresPortal)
			{
				this.AttemptToSpawnGoldshoresPortal();
			}
			if (this.shouldAttemptToSpawnMSPortal)
			{
				this.AttemptToSpawnMSPortal();
			}
			if (this.titanGoldBossBody)
			{
				this.titanGoldBossBody.healthComponent.Suicide(null, null, DamageType.Generic);
			}
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x000576E8 File Offset: 0x000558E8
		[Server]
		private void AttemptToSpawnShopPortal()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::AttemptToSpawnShopPortal()' called on client");
				return;
			}
			Debug.Log("Submitting request for shop portal");
			if (DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(this.shopPortalSpawnCard, new DirectorPlacementRule
			{
				maxDistance = 30f,
				minDistance = 0f,
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				position = base.transform.position,
				spawnOnTarget = base.transform
			}, this.rng)))
			{
				Debug.Log("Succeeded in creating shop portal!");
				Run.instance.shopPortalCount++;
				Chat.SendBroadcastChat(new Chat.SimpleChatMessage
				{
					baseToken = "PORTAL_SHOP_OPEN"
				});
			}
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x000577A8 File Offset: 0x000559A8
		[Server]
		private void AttemptToSpawnGoldshoresPortal()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::AttemptToSpawnGoldshoresPortal()' called on client");
				return;
			}
			if (DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(this.goldshoresPortalSpawnCard, new DirectorPlacementRule
			{
				maxDistance = 40f,
				minDistance = 10f,
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				position = base.transform.position,
				spawnOnTarget = base.transform
			}, this.rng)))
			{
				Chat.SendBroadcastChat(new Chat.SimpleChatMessage
				{
					baseToken = "PORTAL_GOLDSHORES_OPEN"
				});
			}
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x00057840 File Offset: 0x00055A40
		[Server]
		private void AttemptToSpawnMSPortal()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeleporterInteraction::AttemptToSpawnMSPortal()' called on client");
				return;
			}
			if (DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(this.msPortalSpawnCard, new DirectorPlacementRule
			{
				maxDistance = 40f,
				minDistance = 10f,
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				position = base.transform.position,
				spawnOnTarget = base.transform
			}, this.rng)))
			{
				Chat.SendBroadcastChat(new Chat.SimpleChatMessage
				{
					baseToken = "PORTAL_MS_OPEN"
				});
			}
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x00056A6D File Offset: 0x00054C6D
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.activationState == TeleporterInteraction.ActivationState.Charging;
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x000578D8 File Offset: 0x00055AD8
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/TimerHologramContent");
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x000578E4 File Offset: 0x00055AE4
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			TimerHologramContent component = hologramContentObject.GetComponent<TimerHologramContent>();
			if (component)
			{
				component.displayValue = this.remainingChargeTimer;
			}
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x0005790C File Offset: 0x00055B0C
		private IEnumerator ChestLockCoroutine()
		{
			List<GameObject> lockInstances = new List<GameObject>();
			Vector3 myPosition = base.transform.position;
			float maxDistanceSqr = this.clearRadius * this.clearRadius;
			PurchaseInteraction[] purchasables = UnityEngine.Object.FindObjectsOfType<PurchaseInteraction>();
			int num;
			for (int i = 0; i < purchasables.Length; i = num)
			{
				if (purchasables[i] && purchasables[i].available)
				{
					Vector3 position = purchasables[i].transform.position;
					if ((position - myPosition).sqrMagnitude > maxDistanceSqr && !purchasables[i].lockGameObject)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.lockPrefab, position, Quaternion.identity);
						NetworkServer.Spawn(gameObject);
						purchasables[i].NetworklockGameObject = gameObject;
						lockInstances.Add(gameObject);
						yield return new WaitForSeconds(0.1f);
					}
				}
				num = i + 1;
			}
			while (this.activationState == TeleporterInteraction.ActivationState.Charging)
			{
				yield return new WaitForSeconds(1f);
			}
			for (int i = 0; i < lockInstances.Count; i = num)
			{
				UnityEngine.Object.Destroy(lockInstances[i]);
				yield return new WaitForSeconds(0.1f);
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x0005791C File Offset: 0x00055B1C
		private void TrySpawnTitanGold()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			int num = 0;
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject))
				{
					CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
					if (component && component.inventory)
					{
						num += component.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP);
					}
				}
			}
			if (num > 0)
			{
				DirectorPlacementRule placementRule = new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
					minDistance = 20f,
					maxDistance = 130f,
					position = base.transform.position
				};
				DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscTitanGoldAlly"), placementRule, this.rng);
				directorSpawnRequest.ignoreTeamMemberLimit = true;
				directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Player);
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
				if (gameObject)
				{
					float num2 = 1f;
					float num3 = 1f;
					num3 += Run.instance.difficultyCoefficient / 8f;
					num2 += Run.instance.difficultyCoefficient / 2f;
					CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
					this.titanGoldBossBody = component2.GetBody();
					int livingPlayerCount = Run.instance.livingPlayerCount;
					num2 *= Mathf.Pow((float)num, 1f);
					num3 *= Mathf.Pow((float)num, 0.5f);
					component2.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num2 - 1f) * 10f));
					component2.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num3 - 1f) * 10f));
				}
			}
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x00057AD8 File Offset: 0x00055CD8
		private void UpdateHealingNovas()
		{
			bool flag = false;
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				bool flag2 = Util.GetItemCountForTeam(teamIndex, ItemIndex.TPHealingNova, false, true) > 0 && this.isCharging;
				flag = (flag || flag2);
				if (NetworkServer.active)
				{
					ref GameObject ptr = ref this.healingNovaGeneratorsByTeam[(int)teamIndex];
					if (flag2 != ptr)
					{
						if (flag2)
						{
							ptr = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/TeleporterHealNovaGenerator"), base.transform);
							ptr.GetComponent<TeamFilter>().teamIndex = teamIndex;
							NetworkServer.Spawn(ptr);
						}
						else
						{
							UnityEngine.Object.Destroy(ptr);
							ptr = null;
						}
					}
				}
			}
			this.healingNovaItemEffect.SetActive(flag);
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x0600148E RID: 5262 RVA: 0x00057BA8 File Offset: 0x00055DA8
		// (set) Token: 0x0600148F RID: 5263 RVA: 0x00057BBB File Offset: 0x00055DBB
		public uint NetworkactivationStateInternal
		{
			get
			{
				return this.activationStateInternal;
			}
			[param: In]
			set
			{
				base.SetSyncVar<uint>(value, ref this.activationStateInternal, 1U);
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06001490 RID: 5264 RVA: 0x00057BD0 File Offset: 0x00055DD0
		// (set) Token: 0x06001491 RID: 5265 RVA: 0x00057BE3 File Offset: 0x00055DE3
		public bool Networklocked
		{
			get
			{
				return this.locked;
			}
			[param: In]
			set
			{
				base.SetSyncVar<bool>(value, ref this.locked, 2U);
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06001492 RID: 5266 RVA: 0x00057BF8 File Offset: 0x00055DF8
		// (set) Token: 0x06001493 RID: 5267 RVA: 0x00057C0B File Offset: 0x00055E0B
		public uint NetworkchargePercent
		{
			get
			{
				return this.chargePercent;
			}
			[param: In]
			set
			{
				base.SetSyncVar<uint>(value, ref this.chargePercent, 4U);
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06001494 RID: 5268 RVA: 0x00057C20 File Offset: 0x00055E20
		// (set) Token: 0x06001495 RID: 5269 RVA: 0x00057C33 File Offset: 0x00055E33
		public bool Network_shouldAttemptToSpawnShopPortal
		{
			get
			{
				return this._shouldAttemptToSpawnShopPortal;
			}
			[param: In]
			set
			{
				uint dirtyBit = 8U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncShouldAttemptToSpawnShopPortal(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this._shouldAttemptToSpawnShopPortal, dirtyBit);
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06001496 RID: 5270 RVA: 0x00057C74 File Offset: 0x00055E74
		// (set) Token: 0x06001497 RID: 5271 RVA: 0x00057C87 File Offset: 0x00055E87
		public bool Network_shouldAttemptToSpawnGoldshoresPortal
		{
			get
			{
				return this._shouldAttemptToSpawnGoldshoresPortal;
			}
			[param: In]
			set
			{
				uint dirtyBit = 16U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncShouldAttemptToSpawnGoldshoresPortal(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this._shouldAttemptToSpawnGoldshoresPortal, dirtyBit);
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06001498 RID: 5272 RVA: 0x00057CC8 File Offset: 0x00055EC8
		// (set) Token: 0x06001499 RID: 5273 RVA: 0x00057CDB File Offset: 0x00055EDB
		public bool Network_shouldAttemptToSpawnMSPortal
		{
			get
			{
				return this._shouldAttemptToSpawnMSPortal;
			}
			[param: In]
			set
			{
				uint dirtyBit = 32U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncShouldAttemptToSpawnMSPortal(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this._shouldAttemptToSpawnMSPortal, dirtyBit);
			}
		}

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x0600149A RID: 5274 RVA: 0x00057D1C File Offset: 0x00055F1C
		// (set) Token: 0x0600149B RID: 5275 RVA: 0x00057D2F File Offset: 0x00055F2F
		public bool NetworkshowBossIndicator
		{
			get
			{
				return this.showBossIndicator;
			}
			[param: In]
			set
			{
				base.SetSyncVar<bool>(value, ref this.showBossIndicator, 64U);
			}
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x00057D43 File Offset: 0x00055F43
		protected static void InvokeRpcRpcClientOnActivated(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcClientOnActivated called on server.");
				return;
			}
			((TeleporterInteraction)obj).RpcClientOnActivated(reader.ReadGameObject());
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x00057D6C File Offset: 0x00055F6C
		public void CallRpcClientOnActivated(GameObject activatorObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcClientOnActivated called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)TeleporterInteraction.kRpcRpcClientOnActivated);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(activatorObject);
			this.SendRPCInternal(networkWriter, 0, "RpcClientOnActivated");
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x00057DDF File Offset: 0x00055FDF
		static TeleporterInteraction()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(TeleporterInteraction), TeleporterInteraction.kRpcRpcClientOnActivated, new NetworkBehaviour.CmdDelegate(TeleporterInteraction.InvokeRpcRpcClientOnActivated));
			NetworkCRC.RegisterBehaviour("TeleporterInteraction", 0);
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x00057E1C File Offset: 0x0005601C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32(this.activationStateInternal);
				writer.Write(this.locked);
				writer.WritePackedUInt32(this.chargePercent);
				writer.Write(this._shouldAttemptToSpawnShopPortal);
				writer.Write(this._shouldAttemptToSpawnGoldshoresPortal);
				writer.Write(this._shouldAttemptToSpawnMSPortal);
				writer.Write(this.showBossIndicator);
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
				writer.WritePackedUInt32(this.activationStateInternal);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.locked);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32(this.chargePercent);
			}
			if ((base.syncVarDirtyBits & 8U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this._shouldAttemptToSpawnShopPortal);
			}
			if ((base.syncVarDirtyBits & 16U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this._shouldAttemptToSpawnGoldshoresPortal);
			}
			if ((base.syncVarDirtyBits & 32U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this._shouldAttemptToSpawnMSPortal);
			}
			if ((base.syncVarDirtyBits & 64U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.showBossIndicator);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x00058004 File Offset: 0x00056204
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.activationStateInternal = reader.ReadPackedUInt32();
				this.locked = reader.ReadBoolean();
				this.chargePercent = reader.ReadPackedUInt32();
				this._shouldAttemptToSpawnShopPortal = reader.ReadBoolean();
				this._shouldAttemptToSpawnGoldshoresPortal = reader.ReadBoolean();
				this._shouldAttemptToSpawnMSPortal = reader.ReadBoolean();
				this.showBossIndicator = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.activationStateInternal = reader.ReadPackedUInt32();
			}
			if ((num & 2) != 0)
			{
				this.locked = reader.ReadBoolean();
			}
			if ((num & 4) != 0)
			{
				this.chargePercent = reader.ReadPackedUInt32();
			}
			if ((num & 8) != 0)
			{
				this.OnSyncShouldAttemptToSpawnShopPortal(reader.ReadBoolean());
			}
			if ((num & 16) != 0)
			{
				this.OnSyncShouldAttemptToSpawnGoldshoresPortal(reader.ReadBoolean());
			}
			if ((num & 32) != 0)
			{
				this.OnSyncShouldAttemptToSpawnMSPortal(reader.ReadBoolean());
			}
			if ((num & 64) != 0)
			{
				this.showBossIndicator = reader.ReadBoolean();
			}
		}

		// Token: 0x040012FD RID: 4861
		[SyncVar]
		private uint activationStateInternal;

		// Token: 0x040012FE RID: 4862
		private TeleporterInteraction.ActivationState previousActivationState;

		// Token: 0x040012FF RID: 4863
		[SyncVar]
		public bool locked;

		// Token: 0x04001300 RID: 4864
		[Tooltip("How long it takes for this teleporter to finish activating.")]
		public float chargeDuration = 90f;

		// Token: 0x04001301 RID: 4865
		[Tooltip("The radius within which no monsters must exist for the teleporter event to conclude. Changing at runtime will not currently update the indicator scale.")]
		public float clearRadius = 40f;

		// Token: 0x04001302 RID: 4866
		[Tooltip("An object which will be used to represent the clear radius.")]
		public GameObject clearRadiusIndicator;

		// Token: 0x04001303 RID: 4867
		[HideInInspector]
		public float remainingChargeTimer;

		// Token: 0x04001304 RID: 4868
		public int shrineBonusStacks;

		// Token: 0x04001305 RID: 4869
		[SyncVar]
		private uint chargePercent;

		// Token: 0x04001306 RID: 4870
		private float idleToChargingStopwatch;

		// Token: 0x04001307 RID: 4871
		private float monsterCheckTimer;

		// Token: 0x04001308 RID: 4872
		private GameObject teleporterPositionIndicator;

		// Token: 0x04001309 RID: 4873
		public string beginContextString;

		// Token: 0x0400130A RID: 4874
		public string exitContextString;

		// Token: 0x0400130B RID: 4875
		private ChildLocator childLocator;

		// Token: 0x0400130C RID: 4876
		public CombatDirector bonusDirector;

		// Token: 0x0400130D RID: 4877
		public CombatDirector bossDirector;

		// Token: 0x0400130F RID: 4879
		private GameObject bossShrineIndicator;

		// Token: 0x04001310 RID: 4880
		[SyncVar(hook = "OnSyncShouldAttemptToSpawnShopPortal")]
		private bool _shouldAttemptToSpawnShopPortal;

		// Token: 0x04001311 RID: 4881
		[SyncVar(hook = "OnSyncShouldAttemptToSpawnGoldshoresPortal")]
		private bool _shouldAttemptToSpawnGoldshoresPortal;

		// Token: 0x04001312 RID: 4882
		[SyncVar(hook = "OnSyncShouldAttemptToSpawnMSPortal")]
		private bool _shouldAttemptToSpawnMSPortal;

		// Token: 0x04001313 RID: 4883
		private Xoroshiro128Plus rng;

		// Token: 0x04001314 RID: 4884
		private BossGroup bossGroup;

		// Token: 0x04001315 RID: 4885
		private GameObject chargeActivatorServer;

		// Token: 0x04001316 RID: 4886
		private bool monstersCleared;

		// Token: 0x04001317 RID: 4887
		[SyncVar]
		private bool showBossIndicator;

		// Token: 0x0400131B RID: 4891
		public SpawnCard shopPortalSpawnCard;

		// Token: 0x0400131C RID: 4892
		public SpawnCard goldshoresPortalSpawnCard;

		// Token: 0x0400131D RID: 4893
		public SpawnCard msPortalSpawnCard;

		// Token: 0x0400131E RID: 4894
		public float baseShopSpawnChance = 0.25f;

		// Token: 0x0400131F RID: 4895
		[Tooltip("The networked object which will be instantiated to lock purchasables.")]
		public GameObject lockPrefab;

		// Token: 0x04001320 RID: 4896
		[Tooltip("The child object to enable when healing nova should be active.")]
		public GameObject healingNovaItemEffect;

		// Token: 0x04001321 RID: 4897
		private Coroutine chestLockCoroutine;

		// Token: 0x04001322 RID: 4898
		private CharacterBody titanGoldBossBody;

		// Token: 0x04001323 RID: 4899
		private GameObject[] healingNovaGeneratorsByTeam = new GameObject[3];

		// Token: 0x04001324 RID: 4900
		private static int kRpcRpcClientOnActivated = 1157394167;

		// Token: 0x0200034F RID: 847
		private enum ActivationState
		{
			// Token: 0x04001326 RID: 4902
			Idle,
			// Token: 0x04001327 RID: 4903
			IdleToCharging,
			// Token: 0x04001328 RID: 4904
			Charging,
			// Token: 0x04001329 RID: 4905
			Charged,
			// Token: 0x0400132A RID: 4906
			Finished
		}
	}
}
