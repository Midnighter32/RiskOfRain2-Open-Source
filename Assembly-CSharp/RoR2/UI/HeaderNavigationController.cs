using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005C0 RID: 1472
	public class HeaderNavigationController : MonoBehaviour
	{
		// Token: 0x060022E6 RID: 8934 RVA: 0x00097F59 File Offset: 0x00096159
		private void Start()
		{
			this.RebuildHeaders();
		}

		// Token: 0x060022E7 RID: 8935 RVA: 0x00097F64 File Offset: 0x00096164
		private void LateUpdate()
		{
			for (int i = 0; i < this.headers.Length; i++)
			{
				HeaderNavigationController.Header header = this.headers[i];
				if (i == this.currentHeaderIndex)
				{
					header.tmpHeaderText.color = Color.white;
				}
				else
				{
					header.tmpHeaderText.color = Color.gray;
				}
			}
		}

		// Token: 0x060022E8 RID: 8936 RVA: 0x00097FBC File Offset: 0x000961BC
		public void ChooseHeader(string headerName)
		{
			for (int i = 0; i < this.headers.Length; i++)
			{
				if (this.headers[i].headerName == headerName)
				{
					this.currentHeaderIndex = i;
					this.RebuildHeaders();
					return;
				}
			}
		}

		// Token: 0x060022E9 RID: 8937 RVA: 0x00098004 File Offset: 0x00096204
		public void ChooseHeaderByButton(MPButton mpButton)
		{
			for (int i = 0; i < this.headers.Length; i++)
			{
				if (this.headers[i].headerButton == mpButton)
				{
					this.currentHeaderIndex = i;
					this.RebuildHeaders();
					return;
				}
			}
		}

		// Token: 0x060022EA RID: 8938 RVA: 0x0009804C File Offset: 0x0009624C
		private void RebuildHeaders()
		{
			for (int i = 0; i < this.headers.Length; i++)
			{
				HeaderNavigationController.Header header = this.headers[i];
				if (i == this.currentHeaderIndex)
				{
					if (header.headerRoot)
					{
						header.headerRoot.SetActive(true);
					}
					if (header.headerButton && this.buttonSelectionRoot)
					{
						this.buttonSelectionRoot.transform.SetParent(header.headerButton.transform, false);
						this.buttonSelectionRoot.SetActive(false);
						this.buttonSelectionRoot.SetActive(true);
						RectTransform component = this.buttonSelectionRoot.GetComponent<RectTransform>();
						component.offsetMin = Vector2.zero;
						component.offsetMax = Vector2.zero;
						component.localScale = Vector3.one;
					}
				}
				else if (header.headerRoot)
				{
					header.headerRoot.SetActive(false);
				}
			}
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x0009813B File Offset: 0x0009633B
		private HeaderNavigationController.Header GetCurrentHeader()
		{
			return this.headers[this.currentHeaderIndex];
		}

		// Token: 0x04002094 RID: 8340
		public HeaderNavigationController.Header[] headers;

		// Token: 0x04002095 RID: 8341
		public GameObject buttonSelectionRoot;

		// Token: 0x04002096 RID: 8342
		public int currentHeaderIndex;

		// Token: 0x020005C1 RID: 1473
		[Serializable]
		public struct Header
		{
			// Token: 0x04002097 RID: 8343
			public MPButton headerButton;

			// Token: 0x04002098 RID: 8344
			public string headerName;

			// Token: 0x04002099 RID: 8345
			public TextMeshProUGUI tmpHeaderText;

			// Token: 0x0400209A RID: 8346
			public GameObject headerRoot;
		}
	}
}
