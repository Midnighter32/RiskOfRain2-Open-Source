using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005D7 RID: 1495
	public class LanguageTextMeshController : MonoBehaviour
	{
		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06002368 RID: 9064 RVA: 0x0009ACF3 File Offset: 0x00098EF3
		// (set) Token: 0x06002369 RID: 9065 RVA: 0x0009ACFB File Offset: 0x00098EFB
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

		// Token: 0x0600236A RID: 9066 RVA: 0x0009AD1E File Offset: 0x00098F1E
		public void ResolveString()
		{
			this.previousToken = this._token;
			this.resolvedString = Language.GetString(this._token);
		}

		// Token: 0x0600236B RID: 9067 RVA: 0x0009AD3D File Offset: 0x00098F3D
		private void CacheComponents()
		{
			this.textMeshPro = base.GetComponent<TMP_Text>();
		}

		// Token: 0x0600236C RID: 9068 RVA: 0x0009AD4B File Offset: 0x00098F4B
		private void Awake()
		{
			this.CacheComponents();
		}

		// Token: 0x0600236D RID: 9069 RVA: 0x0009AD4B File Offset: 0x00098F4B
		private void OnValidate()
		{
			this.CacheComponents();
		}

		// Token: 0x0600236E RID: 9070 RVA: 0x0009AD53 File Offset: 0x00098F53
		private void Start()
		{
			this.ResolveString();
			this.UpdateLabel();
		}

		// Token: 0x0600236F RID: 9071 RVA: 0x0009AD61 File Offset: 0x00098F61
		private void UpdateLabel()
		{
			if (this.textMeshPro)
			{
				this.textMeshPro.text = this.resolvedString;
			}
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x0009AD81 File Offset: 0x00098F81
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Language.onCurrentLanguageChanged += LanguageTextMeshController.OnCurrentLanguageChanged;
		}

		// Token: 0x06002371 RID: 9073 RVA: 0x0009AD94 File Offset: 0x00098F94
		private static void OnCurrentLanguageChanged()
		{
			LanguageTextMeshController[] array = UnityEngine.Object.FindObjectsOfType<LanguageTextMeshController>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResolveString();
			}
		}

		// Token: 0x0400214F RID: 8527
		[SerializeField]
		private string _token;

		// Token: 0x04002150 RID: 8528
		private string previousToken;

		// Token: 0x04002151 RID: 8529
		private string resolvedString;

		// Token: 0x04002152 RID: 8530
		private TMP_Text textMeshPro;
	}
}
