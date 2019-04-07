using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005DA RID: 1498
	public class EquipmentIcon : MonoBehaviour
	{
		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06002190 RID: 8592 RVA: 0x0009DCFB File Offset: 0x0009BEFB
		public bool hasEquipment
		{
			get
			{
				return this.currentDisplayData.hasEquipment;
			}
		}

		// Token: 0x06002191 RID: 8593 RVA: 0x0009DD08 File Offset: 0x0009BF08
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
				if (newDisplayData.maxStock > 1)
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

		// Token: 0x06002192 RID: 8594 RVA: 0x0009DF6C File Offset: 0x0009C16C
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

		// Token: 0x06002193 RID: 8595 RVA: 0x0009DFBC File Offset: 0x0009C1BC
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

		// Token: 0x06002194 RID: 8596 RVA: 0x0009E008 File Offset: 0x0009C208
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
				result.cooldownValue = Mathf.CeilToInt(chargeFinishTime - now);
				result.stock = (int)equipmentState.charges;
				result.maxStock = (this.targetEquipmentSlot ? this.targetEquipmentSlot.maxStock : 1);
			}
			result.equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
			return result;
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x0009E0D8 File Offset: 0x0009C2D8
		private void Update()
		{
			this.SetDisplayData(this.GenerateDisplayData());
			this.equipmentReminderTimer -= Time.deltaTime;
			if (this.currentDisplayData.isReady && this.equipmentReminderTimer < 0f && this.currentDisplayData.equipmentDef != null)
			{
				this.DoReminderFlash();
			}
		}

		// Token: 0x0400243E RID: 9278
		public Inventory targetInventory;

		// Token: 0x0400243F RID: 9279
		public EquipmentSlot targetEquipmentSlot;

		// Token: 0x04002440 RID: 9280
		public GameObject displayRoot;

		// Token: 0x04002441 RID: 9281
		public PlayerCharacterMasterController playerCharacterMasterController;

		// Token: 0x04002442 RID: 9282
		public RawImage iconImage;

		// Token: 0x04002443 RID: 9283
		public TextMeshProUGUI cooldownText;

		// Token: 0x04002444 RID: 9284
		public TextMeshProUGUI stockText;

		// Token: 0x04002445 RID: 9285
		public GameObject stockFlashPanelObject;

		// Token: 0x04002446 RID: 9286
		public GameObject reminderFlashPanelObject;

		// Token: 0x04002447 RID: 9287
		public GameObject isReadyPanelObject;

		// Token: 0x04002448 RID: 9288
		public GameObject isAutoCastPanelObject;

		// Token: 0x04002449 RID: 9289
		public TooltipProvider tooltipProvider;

		// Token: 0x0400244A RID: 9290
		public bool displayAlternateEquipment;

		// Token: 0x0400244B RID: 9291
		private int previousStockCount;

		// Token: 0x0400244C RID: 9292
		private float equipmentReminderTimer;

		// Token: 0x0400244D RID: 9293
		private EquipmentIcon.DisplayData currentDisplayData;

		// Token: 0x020005DB RID: 1499
		private struct DisplayData
		{
			// Token: 0x170002EF RID: 751
			// (get) Token: 0x06002197 RID: 8599 RVA: 0x0009E130 File Offset: 0x0009C330
			public bool isReady
			{
				get
				{
					return this.stock > 0;
				}
			}

			// Token: 0x170002F0 RID: 752
			// (get) Token: 0x06002198 RID: 8600 RVA: 0x0009E13B File Offset: 0x0009C33B
			public bool hasEquipment
			{
				get
				{
					return this.equipmentDef != null;
				}
			}

			// Token: 0x170002F1 RID: 753
			// (get) Token: 0x06002199 RID: 8601 RVA: 0x0009E146 File Offset: 0x0009C346
			public bool showCooldown
			{
				get
				{
					return !this.isReady && this.hasEquipment;
				}
			}

			// Token: 0x0400244E RID: 9294
			public EquipmentDef equipmentDef;

			// Token: 0x0400244F RID: 9295
			public int cooldownValue;

			// Token: 0x04002450 RID: 9296
			public int stock;

			// Token: 0x04002451 RID: 9297
			public int maxStock;

			// Token: 0x04002452 RID: 9298
			public bool hideEntireDisplay;
		}
	}
}
