using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200062E RID: 1582
	public class RuleCategoryController : MonoBehaviour
	{
		// Token: 0x17000318 RID: 792
		// (get) Token: 0x0600237B RID: 9083 RVA: 0x000A6F22 File Offset: 0x000A5122
		public bool shouldHide
		{
			get
			{
				return (this.strips.Count == 0 && !this.tipObject) || this.currentCategory == null || this.currentCategory.isHidden;
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x0600237C RID: 9084 RVA: 0x000A6F53 File Offset: 0x000A5153
		public bool isEmpty
		{
			get
			{
				return this.strips.Count == 0;
			}
		}

		// Token: 0x0600237D RID: 9085 RVA: 0x000A6F63 File Offset: 0x000A5163
		private void Awake()
		{
			this.SetCollapsed(this.collapsed);
		}

		// Token: 0x0600237E RID: 9086 RVA: 0x000A6F74 File Offset: 0x000A5174
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

		// Token: 0x0600237F RID: 9087 RVA: 0x000A6FF0 File Offset: 0x000A51F0
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

		// Token: 0x06002380 RID: 9088 RVA: 0x000A7078 File Offset: 0x000A5278
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

		// Token: 0x06002381 RID: 9089 RVA: 0x000A7270 File Offset: 0x000A5470
		public void ToggleCollapsed()
		{
			this.SetCollapsed(!this.collapsed);
		}

		// Token: 0x06002382 RID: 9090 RVA: 0x000A7284 File Offset: 0x000A5484
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

		// Token: 0x04002671 RID: 9841
		public GameObject headerObject;

		// Token: 0x04002672 RID: 9842
		public GameObject collapsedIndicator;

		// Token: 0x04002673 RID: 9843
		public GameObject uncollapsedIndicator;

		// Token: 0x04002674 RID: 9844
		public GameObject stripPrefab;

		// Token: 0x04002675 RID: 9845
		public RectTransform stripContainer;

		// Token: 0x04002676 RID: 9846
		public RectTransform framePanel;

		// Token: 0x04002677 RID: 9847
		public GameObject tipPrefab;

		// Token: 0x04002678 RID: 9848
		public RectTransform tipContainer;

		// Token: 0x04002679 RID: 9849
		private readonly List<RuleDef> rulesToDisplay = new List<RuleDef>(RuleCatalog.ruleCount);

		// Token: 0x0400267A RID: 9850
		private readonly List<GameObject> strips = new List<GameObject>();

		// Token: 0x0400267B RID: 9851
		private GameObject tipObject;

		// Token: 0x0400267C RID: 9852
		private RuleCategoryDef currentCategory;

		// Token: 0x0400267D RID: 9853
		[SerializeField]
		private bool collapsed;
	}
}
