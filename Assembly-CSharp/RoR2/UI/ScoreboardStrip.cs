using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000620 RID: 1568
	public class ScoreboardStrip : MonoBehaviour
	{
		// Token: 0x06002514 RID: 9492 RVA: 0x000A1A84 File Offset: 0x0009FC84
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

		// Token: 0x06002515 RID: 9493 RVA: 0x000A1B5C File Offset: 0x0009FD5C
		private void UpdateMoneyText()
		{
			if (this.master)
			{
				this.moneyText.text = string.Format("${0}", this.master.money);
			}
		}

		// Token: 0x06002516 RID: 9494 RVA: 0x000A1B90 File Offset: 0x0009FD90
		private void Update()
		{
			this.UpdateMoneyText();
		}

		// Token: 0x06002517 RID: 9495 RVA: 0x000A1B98 File Offset: 0x0009FD98
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

		// Token: 0x040022D1 RID: 8913
		public ItemInventoryDisplay itemInventoryDisplay;

		// Token: 0x040022D2 RID: 8914
		public EquipmentIcon equipmentIcon;

		// Token: 0x040022D3 RID: 8915
		public SocialUserIcon userAvatar;

		// Token: 0x040022D4 RID: 8916
		public TextMeshProUGUI nameLabel;

		// Token: 0x040022D5 RID: 8917
		public RawImage classIcon;

		// Token: 0x040022D6 RID: 8918
		public TextMeshProUGUI moneyText;

		// Token: 0x040022D7 RID: 8919
		private CharacterMaster master;

		// Token: 0x040022D8 RID: 8920
		private CharacterBody userBody;

		// Token: 0x040022D9 RID: 8921
		private PlayerCharacterMasterController userPlayerCharacterMasterController;
	}
}
