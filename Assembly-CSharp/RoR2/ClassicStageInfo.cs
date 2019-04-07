using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200029B RID: 667
	[RequireComponent(typeof(SceneInfo))]
	public class ClassicStageInfo : MonoBehaviour
	{
		// Token: 0x06000D9B RID: 3483 RVA: 0x00042F4C File Offset: 0x0004114C
		private WeightedSelection<DirectorCard> GenerateDirectorCardWeightedSelection(DirectorCardCategorySelection categorySelection)
		{
			WeightedSelection<DirectorCard> weightedSelection = new WeightedSelection<DirectorCard>(8);
			foreach (DirectorCardCategorySelection.Category category in categorySelection.categories)
			{
				float num = categorySelection.SumAllWeightsInCategory(category);
				foreach (DirectorCard directorCard in category.cards)
				{
					weightedSelection.AddChoice(directorCard, (float)directorCard.selectionWeight / num * category.selectionWeight);
				}
			}
			return weightedSelection;
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000D9C RID: 3484 RVA: 0x00042FC3 File Offset: 0x000411C3
		// (set) Token: 0x06000D9D RID: 3485 RVA: 0x00042FCA File Offset: 0x000411CA
		public static ClassicStageInfo instance { get; private set; }

		// Token: 0x06000D9E RID: 3486 RVA: 0x00042FD4 File Offset: 0x000411D4
		private void Awake()
		{
			this.interactableSelection = this.GenerateDirectorCardWeightedSelection(this.interactableCategories);
			this.monsterSelection = this.GenerateDirectorCardWeightedSelection(this.monsterCategories);
			if (NetworkServer.active && Util.CheckRoll(2f, 0f, null))
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
					ClassicStageInfo.MonsterFamily monsterFamily = weightedSelection.Evaluate(UnityEngine.Random.value);
					this.monsterSelection = this.GenerateDirectorCardWeightedSelection(monsterFamily.monsterFamilyCategories);
					base.StartCoroutine("BroadcastFamilySelection", monsterFamily);
				}
			}
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x000430DE File Offset: 0x000412DE
		private IEnumerator BroadcastFamilySelection(ClassicStageInfo.MonsterFamily selectedFamily)
		{
			yield return new WaitForSeconds(1f);
			Chat.SendBroadcastChat(new Chat.SimpleChatMessage
			{
				baseToken = selectedFamily.familySelectionChatString
			});
			yield break;
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x000430ED File Offset: 0x000412ED
		private void OnEnable()
		{
			ClassicStageInfo.instance = this;
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x000430F5 File Offset: 0x000412F5
		private void OnDisable()
		{
			ClassicStageInfo.instance = null;
		}

		// Token: 0x06000DA2 RID: 3490 RVA: 0x00043100 File Offset: 0x00041300
		private static float CalculateTotalWeight(DirectorCard[] cards)
		{
			float num = 0f;
			foreach (DirectorCard directorCard in cards)
			{
				num += (float)directorCard.selectionWeight;
			}
			return num;
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x00043134 File Offset: 0x00041334
		private static bool CardIsMiniBoss(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name == "GolemMaster" || name == "BisonMaster" || name == "GreaterWispMaster" || name == "BeetleGuardMaster";
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x00043188 File Offset: 0x00041388
		private static bool CardIsChest(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name == "EquipmentBarrel" || name.Contains("Chest") || card.spawnCard.prefab.name.Contains("TripleShop");
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x000431E0 File Offset: 0x000413E0
		private static bool CardIsBarrel(DirectorCard card)
		{
			string name = card.spawnCard.prefab.name;
			return name != "EquipmentBarrel" && name.Contains("Barrel");
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x00043218 File Offset: 0x00041418
		private static bool CardIsChampion(DirectorCard card)
		{
			return card.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().isChampion;
		}

		// Token: 0x0400118D RID: 4493
		[Tooltip("Stages that can be destinations of the teleporter.")]
		public SceneField[] destinations;

		// Token: 0x0400118E RID: 4494
		[SerializeField]
		private DirectorCardCategorySelection interactableCategories;

		// Token: 0x0400118F RID: 4495
		[SerializeField]
		private DirectorCardCategorySelection monsterCategories;

		// Token: 0x04001190 RID: 4496
		public ClassicStageInfo.MonsterFamily[] possibleMonsterFamilies;

		// Token: 0x04001191 RID: 4497
		public WeightedSelection<DirectorCard> interactableSelection;

		// Token: 0x04001192 RID: 4498
		public WeightedSelection<DirectorCard> monsterSelection;

		// Token: 0x04001193 RID: 4499
		[HideInInspector]
		[SerializeField]
		private DirectorCard[] monsterCards;

		// Token: 0x04001194 RID: 4500
		[HideInInspector]
		[SerializeField]
		public DirectorCard[] interactableCards;

		// Token: 0x04001195 RID: 4501
		public int sceneDirectorInteractibleCredits = 200;

		// Token: 0x04001196 RID: 4502
		public int sceneDirectorMonsterCredits = 20;

		// Token: 0x04001198 RID: 4504
		private const float monsterFamilyChance = 2f;

		// Token: 0x0200029C RID: 668
		[Serializable]
		public struct MonsterFamily
		{
			// Token: 0x04001199 RID: 4505
			[SerializeField]
			public DirectorCardCategorySelection monsterFamilyCategories;

			// Token: 0x0400119A RID: 4506
			public string familySelectionChatString;

			// Token: 0x0400119B RID: 4507
			public float selectionWeight;

			// Token: 0x0400119C RID: 4508
			public int minimumStageCompletion;

			// Token: 0x0400119D RID: 4509
			public int maximumStageCompletion;
		}
	}
}
