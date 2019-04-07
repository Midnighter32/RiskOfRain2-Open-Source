using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200062C RID: 1580
	public class RuleBookViewer : MonoBehaviour
	{
		// Token: 0x0600236E RID: 9070 RVA: 0x000A6B5D File Offset: 0x000A4D5D
		private void Start()
		{
			this.AllocateCategories(RuleCatalog.categoryCount);
		}

		// Token: 0x0600236F RID: 9071 RVA: 0x000A6B6A File Offset: 0x000A4D6A
		private void Update()
		{
			if (PreGameController.instance)
			{
				this.SetData(PreGameController.instance.resolvedRuleChoiceMask, PreGameController.instance.readOnlyRuleBook);
			}
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x000A6B94 File Offset: 0x000A4D94
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

		// Token: 0x06002371 RID: 9073 RVA: 0x000A6C14 File Offset: 0x000A4E14
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

		// Token: 0x04002662 RID: 9826
		[Tooltip("The prefab to instantiate for a rule strip.")]
		public GameObject stripPrefab;

		// Token: 0x04002663 RID: 9827
		[Tooltip("The prefab to use for categories.")]
		public GameObject categoryPrefab;

		// Token: 0x04002664 RID: 9828
		public RectTransform categoryContainer;

		// Token: 0x04002665 RID: 9829
		private readonly List<RuleCategoryController> categoryControllers = new List<RuleCategoryController>();

		// Token: 0x04002666 RID: 9830
		private readonly RuleChoiceMask cachedRuleChoiceMask = new RuleChoiceMask();

		// Token: 0x04002667 RID: 9831
		private readonly RuleBook cachedRuleBook = new RuleBook();
	}
}
