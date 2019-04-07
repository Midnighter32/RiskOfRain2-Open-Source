using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000632 RID: 1586
	public class ScoreboardStrip : MonoBehaviour
	{
		// Token: 0x06002397 RID: 9111 RVA: 0x000A76F0 File Offset: 0x000A58F0
		public void SetMaster(CharacterMaster newMaster)
		{
			if (this.master == newMaster)
			{
				return;
			}
			this.userBody = null;
			this.master = newMaster;
			if (this.master)
			{
				this.userBody = this.master.GetBody();
				this.userPlayerCharacterMasterController = this.master.GetComponent<PlayerCharacterMasterController>();
				this.itemInventoryDisplay.SetSubscribedInventory(this.master.inventory);
				this.equipmentIcon.targetInventory = this.master.inventory;
				this.UpdateMoneyText();
			}
			if (this.userAvatar && this.userAvatar.isActiveAndEnabled)
			{
				this.userAvatar.SetFromMaster(newMaster);
			}
			this.nameLabel.text = Util.GetBestMasterName(this.master);
			this.classIcon.texture = this.FindMasterPortrait();
		}

		// Token: 0x06002398 RID: 9112 RVA: 0x000A77C8 File Offset: 0x000A59C8
		private void UpdateMoneyText()
		{
			if (this.master)
			{
				this.moneyText.text = string.Format("${0}", this.master.money);
			}
		}

		// Token: 0x06002399 RID: 9113 RVA: 0x000A77FC File Offset: 0x000A59FC
		private void Update()
		{
			this.UpdateMoneyText();
		}

		// Token: 0x0600239A RID: 9114 RVA: 0x000A7804 File Offset: 0x000A5A04
		private Texture FindMasterPortrait()
		{
			if (this.userBody)
			{
				return this.userBody.portraitIcon;
			}
			if (this.master)
			{
				GameObject bodyPrefab = this.master.bodyPrefab;
				if (bodyPrefab)
				{
					CharacterBody component = bodyPrefab.GetComponent<CharacterBody>();
					if (component)
					{
						return component.portraitIcon;
					}
				}
			}
			return null;
		}

		// Token: 0x0400268A RID: 9866
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x0400268B RID: 9867
		public EquipmentIcon equipmentIcon;

		// Token: 0x0400268C RID: 9868
		public SocialUserIcon userAvatar;

		// Token: 0x0400268D RID: 9869
		public TextMeshProUGUI nameLabel;

		// Token: 0x0400268E RID: 9870
		public RawImage classIcon;

		// Token: 0x0400268F RID: 9871
		public TextMeshProUGUI moneyText;

		// Token: 0x04002690 RID: 9872
		private CharacterMaster master;

		// Token: 0x04002691 RID: 9873
		private CharacterBody userBody;

		// Token: 0x04002692 RID: 9874
		private PlayerCharacterMasterController userPlayerCharacterMasterController;
	}
}
