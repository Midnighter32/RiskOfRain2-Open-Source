using System;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x0200040A RID: 1034
	[RequireComponent(typeof(RawImage))]
	public class SocialUserIcon : UIBehaviour
	{
		// Token: 0x1700021F RID: 543
		// (get) Token: 0x0600170B RID: 5899 RVA: 0x0006DCD8 File Offset: 0x0006BED8
		private Texture defaultTexture
		{
			get
			{
				return Resources.Load<Texture>("Textures/UI/texDefaultSocialUserIcon");
			}
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x0006DCE4 File Offset: 0x0006BEE4
		protected void BuildTexture(int width, int height)
		{
			if (this.generatedTexture && (this.generatedTexture.width != width || this.generatedTexture.height != height))
			{
				this.generatedTexture.Resize(width, height);
			}
			if (!this.generatedTexture)
			{
				this.generatedTexture = new Texture2D(width, height);
				this.rawImageComponent.texture = this.generatedTexture;
			}
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x0006DD53 File Offset: 0x0006BF53
		protected override void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.generatedTexture);
			this.generatedTexture = null;
			base.OnDestroy();
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x0006DD6D File Offset: 0x0006BF6D
		protected override void Awake()
		{
			base.Awake();
			this.rawImageComponent = base.GetComponent<RawImage>();
			this.rawImageComponent.texture = this.defaultTexture;
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x0006DD92 File Offset: 0x0006BF92
		public virtual void Refresh()
		{
			if (this.sourceType == SocialUserIcon.SourceType.Steam)
			{
				this.RefreshForSteam();
			}
			if (!this.generatedTexture)
			{
				this.rawImageComponent.texture = this.defaultTexture;
			}
		}

		// Token: 0x06001710 RID: 5904 RVA: 0x0006DDC4 File Offset: 0x0006BFC4
		public virtual void SetFromMaster(CharacterMaster master)
		{
			if (master)
			{
				PlayerCharacterMasterController component = master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					NetworkUser networkUser = component.networkUser;
					this.SetFromSteamId(networkUser.id.value);
					return;
				}
			}
			this.userSteamId = 0UL;
			this.sourceType = SocialUserIcon.SourceType.None;
			this.Refresh();
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x0006DE16 File Offset: 0x0006C016
		public void SetFromSteamId(ulong steamId)
		{
			if (this.sourceType == SocialUserIcon.SourceType.Steam && steamId == this.userSteamId)
			{
				return;
			}
			this.sourceType = SocialUserIcon.SourceType.Steam;
			this.userSteamId = steamId;
			this.Refresh();
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x0006DE40 File Offset: 0x0006C040
		private void RefreshForSteam()
		{
			Client instance = Client.Instance;
			if (instance != null)
			{
				Facepunch.Steamworks.Image cachedAvatar = instance.Friends.GetCachedAvatar(this.avatarSize, this.userSteamId);
				if (cachedAvatar != null)
				{
					this.OnSteamAvatarReceived(cachedAvatar);
					return;
				}
				instance.Friends.GetAvatar(this.avatarSize, this.userSteamId, new Action<Facepunch.Steamworks.Image>(this.OnSteamAvatarReceived));
			}
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x0006DE9C File Offset: 0x0006C09C
		private void OnSteamAvatarReceived(Facepunch.Steamworks.Image image)
		{
			if (!this)
			{
				return;
			}
			if (image == null)
			{
				return;
			}
			int width = image.Width;
			int height = image.Height;
			this.BuildTexture(width, height);
			byte[] data = image.Data;
			Color32[] array = new Color32[data.Length / 4];
			for (int i = 0; i < height; i++)
			{
				int num = height - 1 - i;
				for (int j = 0; j < width; j++)
				{
					int num2 = (i * width + j) * 4;
					array[num * width + j] = new Color32(data[num2], data[num2 + 1], data[num2 + 2], data[num2 + 3]);
				}
			}
			if (this.generatedTexture)
			{
				this.generatedTexture.SetPixels32(array);
				this.generatedTexture.Apply();
			}
		}

		// Token: 0x04001A46 RID: 6726
		private RawImage rawImageComponent;

		// Token: 0x04001A47 RID: 6727
		protected Texture2D generatedTexture;

		// Token: 0x04001A48 RID: 6728
		private SocialUserIcon.SourceType sourceType;

		// Token: 0x04001A49 RID: 6729
		private ulong userSteamId;

		// Token: 0x04001A4A RID: 6730
		public Friends.AvatarSize avatarSize;

		// Token: 0x0200040B RID: 1035
		private enum SourceType
		{
			// Token: 0x04001A4C RID: 6732
			None,
			// Token: 0x04001A4D RID: 6733
			Steam
		}
	}
}
