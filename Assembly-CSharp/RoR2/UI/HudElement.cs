using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005CE RID: 1486
	[DisallowMultipleComponent]
	public class HudElement : MonoBehaviour
	{
		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06002329 RID: 9001 RVA: 0x00099CF3 File Offset: 0x00097EF3
		// (set) Token: 0x0600232A RID: 9002 RVA: 0x00099CFB File Offset: 0x00097EFB
		public HUD hud
		{
			get
			{
				return this._hud;
			}
			set
			{
				this._hud = value;
				if (this._hud)
				{
					this.targetBodyObject = this._hud.targetBodyObject;
					return;
				}
				this.targetBodyObject = null;
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x0600232B RID: 9003 RVA: 0x00099D2A File Offset: 0x00097F2A
		// (set) Token: 0x0600232C RID: 9004 RVA: 0x00099D32 File Offset: 0x00097F32
		public GameObject targetBodyObject
		{
			get
			{
				return this._targetBodyObject;
			}
			set
			{
				this._targetBodyObject = value;
				if (this._targetBodyObject)
				{
					this._targetCharacterBody = this._targetBodyObject.GetComponent<CharacterBody>();
				}
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x0600232D RID: 9005 RVA: 0x00099D59 File Offset: 0x00097F59
		// (set) Token: 0x0600232E RID: 9006 RVA: 0x00099D61 File Offset: 0x00097F61
		public CharacterBody targetCharacterBody
		{
			get
			{
				return this._targetCharacterBody;
			}
			set
			{
				this._targetCharacterBody = value;
				if (this.targetCharacterBody)
				{
					this._targetBodyObject = this.targetCharacterBody.gameObject;
				}
			}
		}

		// Token: 0x04002115 RID: 8469
		private HUD _hud;

		// Token: 0x04002116 RID: 8470
		private GameObject _targetBodyObject;

		// Token: 0x04002117 RID: 8471
		private CharacterBody _targetCharacterBody;
	}
}
