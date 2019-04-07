using System;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200063F RID: 1599
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SocialUsernameLabel : MonoBehaviour
	{
		// Token: 0x060023D0 RID: 9168 RVA: 0x000A84F2 File Offset: 0x000A66F2
		private void Awake()
		{
			this.textMeshComponent = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x060023D1 RID: 9169 RVA: 0x000A8500 File Offset: 0x000A6700
		public virtual void Refresh()
		{
			if (this.sourceType == SocialUsernameLabel.SourceType.Steam)
			{
				this.RefreshForSteam();
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060023D2 RID: 9170 RVA: 0x000A8511 File Offset: 0x000A6711
		// (set) Token: 0x060023D3 RID: 9171 RVA: 0x000A8519 File Offset: 0x000A6719
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

		// Token: 0x060023D4 RID: 9172 RVA: 0x000A852C File Offset: 0x000A672C
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

		// Token: 0x040026C4 RID: 9924
		protected TextMeshProUGUI textMeshComponent;

		// Token: 0x040026C5 RID: 9925
		private SocialUsernameLabel.SourceType sourceType;

		// Token: 0x040026C6 RID: 9926
		private ulong _userSteamId;

		// Token: 0x040026C7 RID: 9927
		public int subPlayerIndex;

		// Token: 0x02000640 RID: 1600
		private enum SourceType
		{
			// Token: 0x040026C9 RID: 9929
			None,
			// Token: 0x040026CA RID: 9930
			Steam
		}
	}
}
