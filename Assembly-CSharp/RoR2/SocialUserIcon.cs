using System;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000363 RID: 867
	[RequireComponent(typeof(RawImage))]
	public class SocialUserIcon : UIBehaviour
	{
		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06001512 RID: 5394 RVA: 0x0005A0AB File Offset: 0x000582AB
		private Texture defaultTexture
		{
			get
			{
				return Resources.Load<Texture>("Textures/UI/texDefaultSocialUserIcon");
			}
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x0005A0B8 File Offset: 0x000582B8
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

		// Token: 0x06001514 RID: 5396 RVA: 0x0005A127 File Offset: 0x00058327
		protected override void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.generatedTexture);
			this.generatedTexture = null;
			base.OnDestroy();
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x0005A141 File Offset: 0x00058341
		protected override void Awake()
		{
			base.Awake();
			this.rawImageComponent = base.GetComponent<RawImage>();
			this.rawImageComponent.texture = this.defaultTexture;
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x0005A166 File Offset: 0x00058366
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

		// Token: 0x06001517 RID: 5399 RVA: 0x0005A198 File Offset: 0x00058398
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

		// Token: 0x06001518 RID: 5400 RVA: 0x0005A1EA File Offset: 0x000583EA
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

		// Token: 0x06001519 RID: 5401 RVA: 0x0005A214 File Offset: 0x00058414
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

		// Token: 0x0600151A RID: 5402 RVA: 0x0005A270 File Offset: 0x00058470
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

		// Token: 0x040013B7 RID: 5047
		private RawImage rawImageComponent;

		// Token: 0x040013B8 RID: 5048
		protected Texture2D generatedTexture;

		// Token: 0x040013B9 RID: 5049
		private SocialUserIcon.SourceType sourceType;

		// Token: 0x040013BA RID: 5050
		private ulong userSteamId;

		// Token: 0x040013BB RID: 5051
		public Friends.AvatarSize avatarSize;

		// Token: 0x02000364 RID: 868
		private enum SourceType
		{
			// Token: 0x040013BD RID: 5053
			None,
			// Token: 0x040013BE RID: 5054
			Steam
		}
	}
}
