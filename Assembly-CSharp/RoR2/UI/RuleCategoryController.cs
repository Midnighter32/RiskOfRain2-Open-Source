using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200061C RID: 1564
	public class RuleCategoryController : MonoBehaviour
	{
		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x060024F8 RID: 9464 RVA: 0x000A12B2 File Offset: 0x0009F4B2
		public bool shouldHide
		{
			get
			{
				return (this.strips.Count == 0 && !this.tipObject) || this.currentCategory == null || this.currentCategory.isHidden;
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x060024F9 RID: 9465 RVA: 0x000A12E3 File Offset: 0x0009F4E3
		public bool isEmpty
		{
			get
			{
				return this.strips.Count == 0;
			}
		}

		// Token: 0x060024FA RID: 9466 RVA: 0x000A12F3 File Offset: 0x0009F4F3
		private void Awake()
		{
			this.SetCollapsed(this.collapsed);
		}

		// Token: 0x060024FB RID: 9467 RVA: 0x000A1304 File Offset: 0x0009F504
		private void SetTip(string tipToken)
		{
			if (tipToken == null)
			{
				UnityEngine.Object.Destroy(this.tipObject);
				this.tipObject = null;
				this.SetCollapsed(this.collapsed);
				return;
			}
			if (!this.tipObject)
			{
				this.tipObject = UnityEngine.Object.Instantiate<GameObject>(this.tipPrefab, this.tipContainer);
				this.tipObject.SetActive(true);
				this.SetCollapsed(this.collapsed);
			}
			this.tipObject.GetComponentInChildren<LanguageTextMeshController>().token = tipToken;
		}

		// Token: 0x060024FC RID: 9468 RVA: 0x000A1380 File Offset: 0x0009F580
		private void AllocateStrips(int desiredCount)
		{
			while (this.strips.Count > desiredCount)
			{
				int index = this.strips.Count - 1;
				UnityEngine.Object.Destroy(this.strips[index]);
				this.strips.RemoveAt(index);
			}
			while (this.strips.Count < desiredCount)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.stripPrefab, this.stripContainer);
				gameObject.SetActive(true);
				this.strips.Add(gameObject);
			}
			this.framePanel.SetAsLastSibling();
		}

		// Token: 0x060024FD RID: 9469 RVA: 0x000A1408 File Offset: 0x0009F608
		public void SetData(RuleCategoryDef categoryDef, RuleChoiceMask availability, RuleBook ruleBook)
		{
			this.currentCategory = categoryDef;
			this.rulesToDisplay.Clear();
			List<RuleDef> children = categoryDef.children;
			for (int i = 0; i < children.Count; i++)
			{
				RuleDef ruleDef = children[i];
				int num = 0;
				foreach (RuleChoiceDef ruleChoiceDef in ruleDef.choices)
				{
					if (availability[ruleChoiceDef.globalIndex])
					{
						num++;
					}
				}
				bool flag = (!availability[ruleDef.choices[ruleDef.defaultChoiceIndex].globalIndex] && num != 0) || num > 1;
				if (flag)
				{
					this.rulesToDisplay.Add(children[i]);
				}
			}
			this.AllocateStrips(this.rulesToDisplay.Count);
			List<RuleChoiceDef> list = new List<RuleChoiceDef>();
			for (int j = 0; j < this.rulesToDisplay.Count; j++)
			{
				RuleDef ruleDef2 = this.rulesToDisplay[j];
				list.Clear();
				foreach (RuleChoiceDef ruleChoiceDef2 in ruleDef2.choices)
				{
					if (availability[ruleChoiceDef2.globalIndex])
					{
						list.Add(ruleChoiceDef2);
					}
				}
				this.strips[j].GetComponent<RuleBookViewerStrip>().SetData(list, ruleBook.GetRuleChoiceIndex(ruleDef2));
			}
			if (this.headerObject)
			{
				this.headerObject.GetComponent<Image>().color = categoryDef.color;
				this.headerObject.GetComponentInChildren<LanguageTextMeshController>().token = categoryDef.displayToken;
			}
			this.SetTip(this.isEmpty ? categoryDef.emptyTipToken : null);
		}

		// Token: 0x060024FE RID: 9470 RVA: 0x000A1600 File Offset: 0x0009F800
		public void ToggleCollapsed()
		{
			this.SetCollapsed(!this.collapsed);
		}

		// Token: 0x060024FF RID: 9471 RVA: 0x000A1614 File Offset: 0x0009F814
		public void SetCollapsed(bool newCollapsed)
		{
			this.collapsed = newCollapsed;
			this.stripContainer.gameObject.SetActive(!this.collapsed);
			if (this.tipObject)
			{
				this.tipObject.SetActive(!this.collapsed);
			}
			if (this.collapsedIndicator)
			{
				this.collapsedIndicator.SetActive(this.collapsed);
			}
			if (this.uncollapsedIndicator)
			{
				this.uncollapsedIndicator.SetActive(!this.collapsed);
			}
		}

		// Token: 0x040022B8 RID: 8888
		public GameObject headerObject;

		// Token: 0x040022B9 RID: 8889
		public GameObject collapsedIndicator;

		// Token: 0x040022BA RID: 8890
		public GameObject uncollapsedIndicator;

		// Token: 0x040022BB RID: 8891
		public GameObject stripPrefab;

		// Token: 0x040022BC RID: 8892
		public RectTransform stripContainer;

		// Token: 0x040022BD RID: 8893
		public RectTransform framePanel;

		// Token: 0x040022BE RID: 8894
		public GameObject tipPrefab;

		// Token: 0x040022BF RID: 8895
		public RectTransform tipContainer;

		// Token: 0x040022C0 RID: 8896
		private readonly List<RuleDef> rulesToDisplay = new List<RuleDef>(RuleCatalog.ruleCount);

		// Token: 0x040022C1 RID: 8897
		private readonly List<GameObject> strips = new List<GameObject>();

		// Token: 0x040022C2 RID: 8898
		private GameObject tipObject;

		// Token: 0x040022C3 RID: 8899
		private RuleCategoryDef currentCategory;

		// Token: 0x040022C4 RID: 8900
		[SerializeField]
		private bool collapsed;
	}
}
