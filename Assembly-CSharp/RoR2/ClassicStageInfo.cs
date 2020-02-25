using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001A6 RID: 422
	[RequireComponent(typeof(SceneInfo))]
	public class ClassicStageInfo : MonoBehaviour
	{
		// Token: 0x0600090A RID: 2314 RVA: 0x000271E0 File Offset: 0x000253E0
		private WeightedSelection<DirectorCard> GenerateDirectorCardWeightedSelection(DirectorCardCategorySelection categorySelection)
		{
			WeightedSelection<DirectorCard> weightedSelection = new WeightedSelection<DirectorCard>(8);
			foreach (DirectorCardCategorySelection.Category category in categorySelection.categories)
			{
				float num = categorySelection.SumAllWeightsInCategory(category);
				foreach (DirectorCard directorCard in category.cards)
				{
					if (directorCard.CardIsValid())
					{
						weightedSelection.AddChoice(directorCard, (float)directorCard.selectionWeight / num * category.selectionWeight);
					}
				}
			}
			return weightedSelection;
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x0600090B RID: 2315 RVA: 0x00027260 File Offset: 0x00025460
		// (set) Token: 0x0600090C RID: 2316 RVA: 0x00027267 File Offset: 0x00025467
		public static ClassicStageInfo instance { get; private set; }

		// Token: 0x0600090D RID: 2317 RVA: 0x00027270 File Offset: 0x00025470
		private void Awake()
		{
			this.interactableSelection = this.GenerateDirectorCardWeightedSelection(this.interactableCategories);
			this.monsterSelection = this.GenerateDirectorCardWeightedSelection(this.monsterCategories);
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
				if (this.rng.nextNormalizedFloat <= 0.02f)
				{
					Debug.Log("Trying to find family selection...");
					WeightedSelection<ClassicStageInfo.MonsterFamily> weightedSelection = new WeightedSelection<ClassicStageInfo.MonsterFamily>(8);
					for (int i = 0; i < this.possibleMonsterFamilies.Length; i++)
					{
						if (this.possibleMonsterFamilies[i].minimumStageCompletion <= Run.instance.stageClearCount && this.possibleMonsterFamilies[i].maximumStageCompletion > Run.instance.stageClearCount)
						{
							weightedSelection.AddChoice(this.possibleMonsterFamilies[i], this.possibleMonsterFamilies[i].selectionWeight);
						}
					}
					if (weightedSelection.Count > 0)
					{
						ClassicStageInfo.MonsterFamily monsterFamily = weightedSelection.Evaluate(this.rng.nextNormalizedFloat);
						this.monsterSelection = this.GenerateDirectorCardWeightedSelection(monsterFamily.monsterFamilyCategories);
						base.StartCoroutine("BroadcastFamilySelection", monsterFamily);
					}
				}
			}
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0002739A File Offset: 0x0002559A
		private IEnumerator BroadcastFamilySelection(ClassicStageInfo.MonsterFamily selectedFamily)
		{
			yield return new WaitForSeconds(1f);
			Chat.SendBroadcastChat(new Chat.SimpleChatMessage
			{
				baseToken = selectedFamily.familySelectionChatString
			});
			yield break;
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x000273A9 File Offset: 0x000255A9
		private void OnEnable()
		{
			ClassicStageInfo.instance = this;
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x000273B1 File Offset: 0x000255B1
		private void OnDisable()
		{
			ClassicStageInfo.instance = null;
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x000273BC File Offset: 0x000255BC
		private static float CalculateTotalWeight(DirectorCard[] cards)
		{
			float num = 0f;
			foreach (DirectorCard directorCard in cards)
			{
				num += (float)directorCard.selectionWeight;
			}
			return num;
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x000273F0 File Offset: 0x000255F0
		private static bool CardIsMiniBoss(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name == "GolemMaster" || name == "BisonMaster" || name == "GreaterWispMaster" || name == "BeetleGuardMaster";
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x00027444 File Offset: 0x00025644
		private static bool CardIsChest(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name == "EquipmentBarrel" || name.Contains("Chest") || card.spawnCard.prefab.name.Contains("TripleShop");
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x0002749C File Offset: 0x0002569C
		private static bool CardIsBarrel(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name != "EquipmentBarrel" && name.Contains("Barrel");
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x000274D4 File Offset: 0x000256D4
		private static bool CardIsChampion(DirectorCard card)
		{
			return card.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().isChampion;
		}

		// Token: 0x0400095C RID: 2396
		[SerializeField]
		private DirectorCardCategorySelection interactableCategories;

		// Token: 0x0400095D RID: 2397
		[SerializeField]
		private DirectorCardCategorySelection monsterCategories;

		// Token: 0x0400095E RID: 2398
		public ClassicStageInfo.MonsterFamily[] possibleMonsterFamilies;

		// Token: 0x0400095F RID: 2399
		private Xoroshiro128Plus rng;

		// Token: 0x04000960 RID: 2400
		public WeightedSelection<DirectorCard> interactableSelection;

		// Token: 0x04000961 RID: 2401
		public WeightedSelection<DirectorCard> monsterSelection;

		// Token: 0x04000962 RID: 2402
		[HideInInspector]
		[SerializeField]
		private DirectorCard[] monsterCards;

		// Token: 0x04000963 RID: 2403
		[HideInInspector]
		[SerializeField]
		public DirectorCard[] interactableCards;

		// Token: 0x04000964 RID: 2404
		public int sceneDirectorInteractibleCredits = 200;

		// Token: 0x04000965 RID: 2405
		public int sceneDirectorMonsterCredits = 20;

		// Token: 0x04000966 RID: 2406
		public ClassicStageInfo.BonusInteractibleCreditObject[] bonusInteractibleCreditObjects;

		// Token: 0x04000968 RID: 2408
		private const float monsterFamilyChance = 0.02f;

		// Token: 0x020001A7 RID: 423
		[Serializable]
		public struct MonsterFamily
		{
			// Token: 0x04000969 RID: 2409
			[SerializeField]
			public DirectorCardCategorySelection monsterFamilyCategories;

			// Token: 0x0400096A RID: 2410
			public string familySelectionChatString;

			// Token: 0x0400096B RID: 2411
			public float selectionWeight;

			// Token: 0x0400096C RID: 2412
			public int minimumStageCompletion;

			// Token: 0x0400096D RID: 2413
			public int maximumStageCompletion;
		}

		// Token: 0x020001A8 RID: 424
		[Serializable]
		public struct BonusInteractibleCreditObject
		{
			// Token: 0x0400096E RID: 2414
			public GameObject objectThatGrantsPointsIfEnabled;

			// Token: 0x0400096F RID: 2415
			public int points;
		}
	}
}
