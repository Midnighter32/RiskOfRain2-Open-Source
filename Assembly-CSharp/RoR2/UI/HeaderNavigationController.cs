using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005E1 RID: 1505
	public class HeaderNavigationController : MonoBehaviour
	{
		// Token: 0x060021B6 RID: 8630 RVA: 0x0009EF15 File Offset: 0x0009D115
		private void Start()
		{
			this.RebuildHeaders();
		}

		// Token: 0x060021B7 RID: 8631 RVA: 0x0009EF20 File Offset: 0x0009D120
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

		// Token: 0x060021B8 RID: 8632 RVA: 0x0009EF78 File Offset: 0x0009D178
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

		// Token: 0x060021B9 RID: 8633 RVA: 0x0009EFC0 File Offset: 0x0009D1C0
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

		// Token: 0x060021BA RID: 8634 RVA: 0x0009F008 File Offset: 0x0009D208
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
						this.buttonSelectionRoot.transform.parent = header.headerButton.transform;
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

		// Token: 0x060021BB RID: 8635 RVA: 0x0009F0F6 File Offset: 0x0009D2F6
		private HeaderNavigationController.Header GetCurrentHeader()
		{
			return this.headers[this.currentHeaderIndex];
		}

		// Token: 0x04002483 RID: 9347
		public HeaderNavigationController.Header[] headers;

		// Token: 0x04002484 RID: 9348
		public GameObject buttonSelectionRoot;

		// Token: 0x04002485 RID: 9349
		public int currentHeaderIndex;

		// Token: 0x020005E2 RID: 1506
		[Serializable]
		public struct Header
		{
			// Token: 0x04002486 RID: 9350
			public MPButton headerButton;

			// Token: 0x04002487 RID: 9351
			public string headerName;

			// Token: 0x04002488 RID: 9352
			public TextMeshProUGUI tmpHeaderText;

			// Token: 0x04002489 RID: 9353
			public GameObject headerRoot;
		}
	}
}
