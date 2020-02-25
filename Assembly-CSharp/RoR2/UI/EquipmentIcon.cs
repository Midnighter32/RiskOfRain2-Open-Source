using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005B9 RID: 1465
	public class EquipmentIcon : MonoBehaviour
	{
		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x060022C0 RID: 8896 RVA: 0x00096C6F File Offset: 0x00094E6F
		public bool hasEquipment
		{
			get
			{
				return this.currentDisplayData.hasEquipment;
			}
		}

		// Token: 0x060022C1 RID: 8897 RVA: 0x00096C7C File Offset: 0x00094E7C
		private void SetDisplayData(EquipmentIcon.DisplayData newDisplayData)
		{
			if (!this.currentDisplayData.isReady && newDisplayData.isReady)
			{
				this.DoStockFlash();
			}
			if (this.displayRoot)
			{
				this.displayRoot.SetActive(!newDisplayData.hideEntireDisplay);
			}
			if (newDisplayData.stock > this.currentDisplayData.stock)
			{
				Util.PlaySound("Play_item_proc_equipMag", RoR2Application.instance.gameObject);
				this.DoStockFlash();
			}
			if (this.isReadyPanelObject)
			{
				this.isReadyPanelObject.SetActive(newDisplayData.isReady);
			}
			if (this.isAutoCastPanelObject)
			{
				if (this.targetInventory)
				{
					this.isAutoCastPanelObject.SetActive(this.targetInventory.GetItemCount(ItemIndex.AutoCastEquipment) > 0);
				}
				else
				{
					this.isAutoCastPanelObject.SetActive(false);
				}
			}
			if (this.iconImage)
			{
				Texture texture = null;
				Color color = Color.clear;
				if (newDisplayData.equipmentDef != null)
				{
					color = ((newDisplayData.stock > 0) ? Color.white : Color.gray);
					texture = Resources.Load<Texture>(newDisplayData.equipmentDef.pickupIconPath);
				}
				this.iconImage.texture = texture;
				this.iconImage.color = color;
			}
			if (this.cooldownText)
			{
				this.cooldownText.gameObject.SetActive(newDisplayData.showCooldown);
				if (newDisplayData.cooldownValue != this.currentDisplayData.cooldownValue)
				{
					this.cooldownText.text = newDisplayData.cooldownValue.ToString();
				}
			}
			if (this.stockText)
			{
				if (newDisplayData.hasEquipment && (newDisplayData.maxStock > 1 || newDisplayData.stock > 1))
				{
					this.stockText.gameObject.SetActive(true);
					this.stockText.text = newDisplayData.stock.ToString();
				}
				else
				{
					this.stockText.gameObject.SetActive(false);
				}
			}
			string titleToken = null;
			string bodyToken = null;
			Color titleColor = Color.white;
			Color gray = Color.gray;
			if (newDisplayData.equipmentDef != null)
			{
				titleToken = newDisplayData.equipmentDef.nameToken;
				bodyToken = newDisplayData.equipmentDef.pickupToken;
				titleColor = ColorCatalog.GetColor(newDisplayData.equipmentDef.colorIndex);
			}
			if (this.tooltipProvider)
			{
				this.tooltipProvider.titleToken = titleToken;
				this.tooltipProvider.titleColor = titleColor;
				this.tooltipProvider.bodyToken = bodyToken;
				this.tooltipProvider.bodyColor = gray;
			}
			this.currentDisplayData = newDisplayData;
		}

		// Token: 0x060022C2 RID: 8898 RVA: 0x00096EF4 File Offset: 0x000950F4
		private void DoReminderFlash()
		{
			if (this.reminderFlashPanelObject)
			{
				AnimateUIAlpha component = this.reminderFlashPanelObject.GetComponent<AnimateUIAlpha>();
				if (component)
				{
					component.time = 0f;
				}
				this.reminderFlashPanelObject.SetActive(true);
			}
			this.equipmentReminderTimer = 5f;
		}

		// Token: 0x060022C3 RID: 8899 RVA: 0x00096F44 File Offset: 0x00095144
		private void DoStockFlash()
		{
			this.DoReminderFlash();
			if (this.stockFlashPanelObject)
			{
				AnimateUIAlpha component = this.stockFlashPanelObject.GetComponent<AnimateUIAlpha>();
				if (component)
				{
					component.time = 0f;
				}
				this.stockFlashPanelObject.SetActive(true);
			}
		}

		// Token: 0x060022C4 RID: 8900 RVA: 0x00096F90 File Offset: 0x00095190
		private EquipmentIcon.DisplayData GenerateDisplayData()
		{
			EquipmentIcon.DisplayData result = default(EquipmentIcon.DisplayData);
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			if (this.targetInventory)
			{
				EquipmentState equipmentState;
				if (this.displayAlternateEquipment)
				{
					equipmentState = this.targetInventory.alternateEquipmentState;
					result.hideEntireDisplay = (this.targetInventory.GetEquipmentSlotCount() <= 1);
				}
				else
				{
					equipmentState = this.targetInventory.currentEquipmentState;
					result.hideEntireDisplay = false;
				}
				Run.FixedTimeStamp now = Run.FixedTimeStamp.now;
				Run.FixedTimeStamp chargeFinishTime = equipmentState.chargeFinishTime;
				equipmentIndex = equipmentState.equipmentIndex;
				result.cooldownValue = (chargeFinishTime.isInfinity ? 0 : Mathf.CeilToInt(chargeFinishTime.timeUntilClamped));
				result.stock = (int)equipmentState.charges;
				result.maxStock = (this.targetEquipmentSlot ? this.targetEquipmentSlot.maxStock : 1);
			}
			else if (this.displayAlternateEquipment)
			{
				result.hideEntireDisplay = true;
			}
			result.equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
			return result;
		}

		// Token: 0x060022C5 RID: 8901 RVA: 0x0009707C File Offset: 0x0009527C
		private void Update()
		{
			this.SetDisplayData(this.GenerateDisplayData());
			this.equipmentReminderTimer -= Time.deltaTime;
			if (this.currentDisplayData.isReady && this.equipmentReminderTimer < 0f && this.currentDisplayData.equipmentDef != null)
			{
				this.DoReminderFlash();
			}
		}

		// Token: 0x0400204D RID: 8269
		public Inventory targetInventory;

		// Token: 0x0400204E RID: 8270
		public EquipmentSlot targetEquipmentSlot;

		// Token: 0x0400204F RID: 8271
		public GameObject displayRoot;

		// Token: 0x04002050 RID: 8272
		public PlayerCharacterMasterController playerCharacterMasterController;

		// Token: 0x04002051 RID: 8273
		public RawImage iconImage;

		// Token: 0x04002052 RID: 8274
		public TextMeshProUGUI cooldownText;

		// Token: 0x04002053 RID: 8275
		public TextMeshProUGUI stockText;

		// Token: 0x04002054 RID: 8276
		public GameObject stockFlashPanelObject;

		// Token: 0x04002055 RID: 8277
		public GameObject reminderFlashPanelObject;

		// Token: 0x04002056 RID: 8278
		public GameObject isReadyPanelObject;

		// Token: 0x04002057 RID: 8279
		public GameObject isAutoCastPanelObject;

		// Token: 0x04002058 RID: 8280
		public TooltipProvider tooltipProvider;

		// Token: 0x04002059 RID: 8281
		public bool displayAlternateEquipment;

		// Token: 0x0400205A RID: 8282
		private int previousStockCount;

		// Token: 0x0400205B RID: 8283
		private float equipmentReminderTimer;

		// Token: 0x0400205C RID: 8284
		private EquipmentIcon.DisplayData currentDisplayData;

		// Token: 0x020005BA RID: 1466
		private struct DisplayData
		{
			// Token: 0x170003A3 RID: 931
			// (get) Token: 0x060022C7 RID: 8903 RVA: 0x000970D4 File Offset: 0x000952D4
			public bool isReady
			{
				get
				{
					return this.stock > 0;
				}
			}

			// Token: 0x170003A4 RID: 932
			// (get) Token: 0x060022C8 RID: 8904 RVA: 0x000970DF File Offset: 0x000952DF
			public bool hasEquipment
			{
				get
				{
					return this.equipmentDef != null;
				}
			}

			// Token: 0x170003A5 RID: 933
			// (get) Token: 0x060022C9 RID: 8905 RVA: 0x000970EA File Offset: 0x000952EA
			public bool showCooldown
			{
				get
				{
					return !this.isReady && this.hasEquipment;
				}
			}

			// Token: 0x0400205D RID: 8285
			public EquipmentDef equipmentDef;

			// Token: 0x0400205E RID: 8286
			public int cooldownValue;

			// Token: 0x0400205F RID: 8287
			public int stock;

			// Token: 0x04002060 RID: 8288
			public int maxStock;

			// Token: 0x04002061 RID: 8289
			public bool hideEntireDisplay;
		}
	}
}
