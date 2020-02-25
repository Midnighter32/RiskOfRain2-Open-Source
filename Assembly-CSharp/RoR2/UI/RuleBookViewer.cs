using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200061A RID: 1562
	public class RuleBookViewer : MonoBehaviour
	{
		// Token: 0x060024EA RID: 9450 RVA: 0x000A0EE9 File Offset: 0x0009F0E9
		private void Awake()
		{
			this.cachedRuleBook = new RuleBook();
			this.cachedRuleChoiceMask = new RuleChoiceMask();
		}

		// Token: 0x060024EB RID: 9451 RVA: 0x000A0F01 File Offset: 0x0009F101
		private void Start()
		{
			this.AllocateCategories(RuleCatalog.categoryCount);
		}

		// Token: 0x060024EC RID: 9452 RVA: 0x000A0F0E File Offset: 0x0009F10E
		private void Update()
		{
			if (PreGameController.instance)
			{
				this.SetData(PreGameController.instance.resolvedRuleChoiceMask, PreGameController.instance.readOnlyRuleBook);
			}
		}

		// Token: 0x060024ED RID: 9453 RVA: 0x000A0F38 File Offset: 0x0009F138
		private void AllocateCategories(int desiredCount)
		{
			while (this.categoryControllers.Count > desiredCount)
			{
				int index = this.categoryControllers.Count - 1;
				UnityEngine.Object.Destroy(this.categoryControllers[index].gameObject);
				this.categoryControllers.RemoveAt(index);
			}
			while (this.categoryControllers.Count < desiredCount)
			{
				RuleCategoryController component = UnityEngine.Object.Instantiate<GameObject>(this.categoryPrefab, this.categoryContainer).GetComponent<RuleCategoryController>();
				this.categoryControllers.Add(component);
			}
		}

		// Token: 0x060024EE RID: 9454 RVA: 0x000A0FB8 File Offset: 0x0009F1B8
		private void SetData(RuleChoiceMask choiceAvailability, RuleBook ruleBook)
		{
			if (choiceAvailability.Equals(this.cachedRuleChoiceMask) && ruleBook.Equals(this.cachedRuleBook))
			{
				return;
			}
			this.cachedRuleChoiceMask.Copy(choiceAvailability);
			this.cachedRuleBook.Copy(ruleBook);
			for (int i = 0; i < RuleCatalog.categoryCount; i++)
			{
				this.categoryControllers[i].SetData(RuleCatalog.GetCategoryDef(i), this.cachedRuleChoiceMask, this.cachedRuleBook);
				this.categoryControllers[i].gameObject.SetActive(!this.categoryControllers[i].shouldHide);
			}
		}

		// Token: 0x040022A9 RID: 8873
		[Tooltip("The prefab to instantiate for a rule strip.")]
		public GameObject stripPrefab;

		// Token: 0x040022AA RID: 8874
		[Tooltip("The prefab to use for categories.")]
		public GameObject categoryPrefab;

		// Token: 0x040022AB RID: 8875
		public RectTransform categoryContainer;

		// Token: 0x040022AC RID: 8876
		private readonly List<RuleCategoryController> categoryControllers = new List<RuleCategoryController>();

		// Token: 0x040022AD RID: 8877
		private RuleChoiceMask cachedRuleChoiceMask;

		// Token: 0x040022AE RID: 8878
		private RuleBook cachedRuleBook;
	}
}
