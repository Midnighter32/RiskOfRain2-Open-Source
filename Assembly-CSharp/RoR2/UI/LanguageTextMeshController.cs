using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005F2 RID: 1522
	public class LanguageTextMeshController : MonoBehaviour
	{
		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06002225 RID: 8741 RVA: 0x000A189B File Offset: 0x0009FA9B
		// (set) Token: 0x06002226 RID: 8742 RVA: 0x000A18A3 File Offset: 0x0009FAA3
		public string token
		{
			get
			{
				return this._token;
			}
			set
			{
				if (value != this.previousToken)
				{
					this._token = value;
					this.ResolveString();
					this.UpdateLabel();
				}
			}
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x000A18C6 File Offset: 0x0009FAC6
		public void ResolveString()
		{
			this.previousToken = this._token;
			this.resolvedString = Language.GetString(this._token);
		}

		// Token: 0x06002228 RID: 8744 RVA: 0x000A18E5 File Offset: 0x0009FAE5
		private void CacheComponents()
		{
			this.text = base.GetComponent<Text>();
			this.textMesh = base.GetComponent<TextMesh>();
			this.textMeshPro = base.GetComponent<TextMeshPro>();
			this.textMeshProUGui = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06002229 RID: 8745 RVA: 0x000A1917 File Offset: 0x0009FB17
		private void Awake()
		{
			this.CacheComponents();
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x000A1917 File Offset: 0x0009FB17
		private void OnValidate()
		{
			this.CacheComponents();
		}

		// Token: 0x0600222B RID: 8747 RVA: 0x000A191F File Offset: 0x0009FB1F
		private void Start()
		{
			this.ResolveString();
			this.UpdateLabel();
		}

		// Token: 0x0600222C RID: 8748 RVA: 0x000A1930 File Offset: 0x0009FB30
		private void UpdateLabel()
		{
			if (this.text)
			{
				this.text.text = this.resolvedString;
			}
			if (this.textMesh)
			{
				this.textMesh.text = this.resolvedString;
			}
			if (this.textMeshPro)
			{
				this.textMeshPro.text = this.resolvedString;
			}
			if (this.textMeshProUGui)
			{
				this.textMeshProUGui.text = this.resolvedString;
			}
		}

		// Token: 0x0400252D RID: 9517
		[SerializeField]
		private string _token;

		// Token: 0x0400252E RID: 9518
		private string previousToken;

		// Token: 0x0400252F RID: 9519
		private string resolvedString;

		// Token: 0x04002530 RID: 9520
		private Text text;

		// Token: 0x04002531 RID: 9521
		private TextMesh textMesh;

		// Token: 0x04002532 RID: 9522
		private TextMeshPro textMeshPro;

		// Token: 0x04002533 RID: 9523
		private TextMeshProUGUI textMeshProUGui;
	}
}
