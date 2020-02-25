using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000331 RID: 817
	[RequireComponent(typeof(CombatDirector))]
	[RequireComponent(typeof(CombatSquad))]
	[RequireComponent(typeof(PurchaseInteraction))]
	public class ShrineCombatBehavior : NetworkBehaviour
	{
		// Token: 0x0600135F RID: 4959 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06001360 RID: 4960 RVA: 0x00052F18 File Offset: 0x00051118
		private float monsterCredit
		{
			get
			{
				return (float)this.baseMonsterCredit * this.cachedDifficultyCoefficient * (1f + (float)this.purchaseCount * (this.monsterCreditCoefficientPerPurchase - 1f));
			}
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x00052F43 File Offset: 0x00051143
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
				this.combatDirector = base.GetComponent<CombatDirector>();
				this.combatDirector.combatSquad.onDefeatedServer += this.OnDefeatedServer;
			}
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x00052F80 File Offset: 0x00051180
		private void OnDefeatedServer()
		{
			Action<ShrineCombatBehavior> action = ShrineCombatBehavior.onDefeatedServerGlobal;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x00052F92 File Offset: 0x00051192
		private void Start()
		{
			this.cachedDifficultyCoefficient = Run.instance.difficultyCoefficient;
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
				this.SelectMonsterCard();
			}
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x00052FCC File Offset: 0x000511CC
		public void SelectMonsterCard()
		{
			WeightedSelection<DirectorCard> weightedSelection = Util.CreateReasonableDirectorCardSpawnList(this.monsterCredit, this.combatDirector.maximumNumberToSpawnBeforeSkipping, 1);
			if (weightedSelection.Count == 0)
			{
				if (this.chosenDirectorCard == null)
				{
					Debug.Log("Could not find appropriate spawn card for Combat Shrine");
					this.purchaseInteraction.SetAvailable(false);
				}
				return;
			}
			this.chosenDirectorCard = weightedSelection.Evaluate(this.rng.nextNormalizedFloat);
		}

		// Token: 0x06001365 RID: 4965 RVA: 0x00053030 File Offset: 0x00051230
		public void FixedUpdate()
		{
			if (this.waitingForRefresh)
			{
				this.refreshTimer -= Time.fixedDeltaTime;
				if (this.refreshTimer <= 0f && this.purchaseCount < this.maxPurchaseCount)
				{
					this.purchaseInteraction.SetAvailable(true);
					this.waitingForRefresh = false;
				}
			}
		}

		// Token: 0x06001366 RID: 4966 RVA: 0x00053088 File Offset: 0x00051288
		[Server]
		public void AddShrineStack(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineCombatBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			new List<CharacterBody>();
			this.waitingForRefresh = true;
			if (this.combatDirector)
			{
				this.combatDirector.enabled = true;
				this.combatDirector.monsterCredit += this.monsterCredit;
				this.combatDirector.OverrideCurrentMonsterCard(this.chosenDirectorCard);
				this.combatDirector.monsterSpawnTimer = 0f;
			}
			CharacterMaster component = this.chosenDirectorCard.spawnCard.prefab.GetComponent<CharacterMaster>();
			if (component)
			{
				CharacterBody component2 = component.bodyPrefab.GetComponent<CharacterBody>();
				if (component2)
				{
					Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
					{
						subjectAsCharacterBody = interactor.GetComponent<CharacterBody>(),
						baseToken = "SHRINE_COMBAT_USE_MESSAGE",
						paramTokens = new string[]
						{
							component2.baseNameToken
						}
					});
				}
			}
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity,
				scale = 1f,
				color = this.shrineEffectColor
			}, true);
			this.purchaseCount++;
			this.refreshTimer = 2f;
			if (this.purchaseCount >= this.maxPurchaseCount)
			{
				this.symbolTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x06001367 RID: 4967 RVA: 0x000531F8 File Offset: 0x000513F8
		// (remove) Token: 0x06001368 RID: 4968 RVA: 0x0005322C File Offset: 0x0005142C
		public static event Action<ShrineCombatBehavior> onDefeatedServerGlobal;

		// Token: 0x06001369 RID: 4969 RVA: 0x0005325F File Offset: 0x0005145F
		private void OnValidate()
		{
			if (!base.GetComponent<CombatDirector>().combatSquad)
			{
				Debug.LogError("ShrineCombatBehavior's sibling CombatDirector must use a CombatSquad.", base.gameObject);
			}
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x0600136C RID: 4972 RVA: 0x00053284 File Offset: 0x00051484
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600136D RID: 4973 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001222 RID: 4642
		public Color shrineEffectColor;

		// Token: 0x04001223 RID: 4643
		public int maxPurchaseCount;

		// Token: 0x04001224 RID: 4644
		public int baseMonsterCredit;

		// Token: 0x04001225 RID: 4645
		public float monsterCreditCoefficientPerPurchase;

		// Token: 0x04001226 RID: 4646
		public Transform symbolTransform;

		// Token: 0x04001227 RID: 4647
		public GameObject spawnPositionEffectPrefab;

		// Token: 0x04001228 RID: 4648
		private CombatDirector combatDirector;

		// Token: 0x04001229 RID: 4649
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x0400122A RID: 4650
		private int purchaseCount;

		// Token: 0x0400122B RID: 4651
		private float refreshTimer;

		// Token: 0x0400122C RID: 4652
		private const float refreshDuration = 2f;

		// Token: 0x0400122D RID: 4653
		private bool waitingForRefresh;

		// Token: 0x0400122E RID: 4654
		private Xoroshiro128Plus rng;

		// Token: 0x0400122F RID: 4655
		private DirectorCard chosenDirectorCard;

		// Token: 0x04001230 RID: 4656
		private float cachedDifficultyCoefficient;
	}
}
