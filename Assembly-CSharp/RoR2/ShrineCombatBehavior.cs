using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DF RID: 991
	[RequireComponent(typeof(PurchaseInteraction))]
	[RequireComponent(typeof(CombatDirector))]
	public class ShrineCombatBehavior : NetworkBehaviour
	{
		// Token: 0x0600158F RID: 5519 RVA: 0x00037FB6 File Offset: 0x000361B6
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06001590 RID: 5520 RVA: 0x0006757A File Offset: 0x0006577A
		private float monsterCredit
		{
			get
			{
				return (float)this.baseMonsterCredit * this.cachedDifficultyCoefficient * (1f + (float)this.purchaseCount * (this.monsterCreditCoefficientPerPurchase - 1f));
			}
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x000675A8 File Offset: 0x000657A8
		private void Start()
		{
			this.cachedDifficultyCoefficient = Run.instance.difficultyCoefficient;
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
			this.combatDirector = base.GetComponent<CombatDirector>();
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
				this.InitCombatShrineValues();
			}
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x00067604 File Offset: 0x00065804
		private void InitCombatShrineValues()
		{
			WeightedSelection<DirectorCard> monsterSelection = ClassicStageInfo.instance.monsterSelection;
			WeightedSelection<DirectorCard> weightedSelection = new WeightedSelection<DirectorCard>(8);
			for (int i = 0; i < monsterSelection.Count; i++)
			{
				DirectorCard value = monsterSelection.choices[i].value;
				if ((float)value.cost <= this.monsterCredit && (float)value.cost * CombatDirector.maximumNumberToSpawnBeforeSkipping * CombatDirector.eliteMultiplierCost / 2f > this.monsterCredit && value.CardIsValid())
				{
					weightedSelection.AddChoice(value, monsterSelection.choices[i].weight);
				}
			}
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

		// Token: 0x06001593 RID: 5523 RVA: 0x000676D4 File Offset: 0x000658D4
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

		// Token: 0x06001594 RID: 5524 RVA: 0x0006772C File Offset: 0x0006592C
		[Server]
		public void AddShrineStack(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShrineCombatBehavior::AddShrineStack(RoR2.Interactor)' called on client");
				return;
			}
			this.waitingForRefresh = true;
			if (this.combatDirector)
			{
				this.combatDirector.OverrideCurrentMonsterCard(this.chosenDirectorCard);
				this.combatDirector.enabled = true;
				this.combatDirector.monsterCredit += this.monsterCredit;
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
						subjectCharacterBodyGameObject = interactor.gameObject,
						baseToken = "SHRINE_COMBAT_USE_MESSAGE",
						paramTokens = new string[]
						{
							component2.baseNameToken
						}
					});
				}
			}
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
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

		// Token: 0x06001596 RID: 5526 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x0006789C File Offset: 0x00065A9C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040018E0 RID: 6368
		public Color shrineEffectColor;

		// Token: 0x040018E1 RID: 6369
		public int maxPurchaseCount;

		// Token: 0x040018E2 RID: 6370
		public int baseMonsterCredit;

		// Token: 0x040018E3 RID: 6371
		public float monsterCreditCoefficientPerPurchase;

		// Token: 0x040018E4 RID: 6372
		public Transform symbolTransform;

		// Token: 0x040018E5 RID: 6373
		public GameObject spawnPositionEffectPrefab;

		// Token: 0x040018E6 RID: 6374
		private CombatDirector combatDirector;

		// Token: 0x040018E7 RID: 6375
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x040018E8 RID: 6376
		private int purchaseCount;

		// Token: 0x040018E9 RID: 6377
		private float refreshTimer;

		// Token: 0x040018EA RID: 6378
		private const float refreshDuration = 2f;

		// Token: 0x040018EB RID: 6379
		private bool waitingForRefresh;

		// Token: 0x040018EC RID: 6380
		private Xoroshiro128Plus rng;

		// Token: 0x040018ED RID: 6381
		private DirectorCard chosenDirectorCard;

		// Token: 0x040018EE RID: 6382
		private float cachedDifficultyCoefficient;
	}
}
