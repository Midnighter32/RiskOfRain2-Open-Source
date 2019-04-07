using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005EA RID: 1514
	[DisallowMultipleComponent]
	public class HudElement : MonoBehaviour
	{
		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060021EC RID: 8684 RVA: 0x000A09DC File Offset: 0x0009EBDC
		// (set) Token: 0x060021ED RID: 8685 RVA: 0x000A09E4 File Offset: 0x0009EBE4
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

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x060021EE RID: 8686 RVA: 0x000A0A13 File Offset: 0x0009EC13
		// (set) Token: 0x060021EF RID: 8687 RVA: 0x000A0A1B File Offset: 0x0009EC1B
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

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x060021F0 RID: 8688 RVA: 0x000A0A42 File Offset: 0x0009EC42
		// (set) Token: 0x060021F1 RID: 8689 RVA: 0x000A0A4A File Offset: 0x0009EC4A
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

		// Token: 0x040024F8 RID: 9464
		private HUD _hud;

		// Token: 0x040024F9 RID: 9465
		private GameObject _targetBodyObject;

		// Token: 0x040024FA RID: 9466
		private CharacterBody _targetCharacterBody;
	}
}
