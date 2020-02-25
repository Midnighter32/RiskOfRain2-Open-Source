using System;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000632 RID: 1586
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SocialUsernameLabel : MonoBehaviour
	{
		// Token: 0x06002569 RID: 9577 RVA: 0x000A2DE6 File Offset: 0x000A0FE6
		private void Awake()
		{
			this.textMeshComponent = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x0600256A RID: 9578 RVA: 0x000A2DF4 File Offset: 0x000A0FF4
		public virtual void Refresh()
		{
			if (this.sourceType == SocialUsernameLabel.SourceType.Steam)
			{
				this.RefreshForSteam();
			}
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x0600256B RID: 9579 RVA: 0x000A2E05 File Offset: 0x000A1005
		// (set) Token: 0x0600256C RID: 9580 RVA: 0x000A2E0D File Offset: 0x000A100D
		public ulong userSteamId
		{
			get
			{
				return this._userSteamId;
			}
			set
			{
				this.sourceType = SocialUsernameLabel.SourceType.Steam;
				this._userSteamId = value;
			}
		}

		// Token: 0x0600256D RID: 9581 RVA: 0x000A2E20 File Offset: 0x000A1020
		public void RefreshForSteam()
		{
			Client instance = Client.Instance;
			if (instance != null)
			{
				this.textMeshComponent.text = instance.Friends.GetName(this.userSteamId);
				if (this.subPlayerIndex != 0)
				{
					TextMeshProUGUI textMeshProUGUI = this.textMeshComponent;
					textMeshProUGUI.text = string.Concat(new object[]
					{
						textMeshProUGUI.text,
						"(",
						this.subPlayerIndex + 1,
						")"
					});
				}
			}
		}

		// Token: 0x04002321 RID: 8993
		protected TextMeshProUGUI textMeshComponent;

		// Token: 0x04002322 RID: 8994
		private SocialUsernameLabel.SourceType sourceType;

		// Token: 0x04002323 RID: 8995
		private ulong _userSteamId;

		// Token: 0x04002324 RID: 8996
		public int subPlayerIndex;

		// Token: 0x02000633 RID: 1587
		private enum SourceType
		{
			// Token: 0x04002326 RID: 8998
			None,
			// Token: 0x04002327 RID: 8999
			Steam
		}
	}
}
