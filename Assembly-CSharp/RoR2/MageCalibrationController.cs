using System;
using System.Collections.Generic;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200034D RID: 845
	[RequireComponent(typeof(CharacterBody))]
	public class MageCalibrationController : MonoBehaviour
	{
		// Token: 0x06001177 RID: 4471 RVA: 0x00056C53 File Offset: 0x00054E53
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.characterBody.onInventoryChanged += this.OnInventoryChanged;
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x00056C89 File Offset: 0x00054E89
		private void Start()
		{
			this.currentElement = this.GetAwardedElementFromInventory();
			this.RefreshCalibrationElement(this.currentElement);
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x00056CA3 File Offset: 0x00054EA3
		private void OnDestroy()
		{
			this.characterBody.onInventoryChanged -= this.OnInventoryChanged;
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x0600117A RID: 4474 RVA: 0x00056CBC File Offset: 0x00054EBC
		// (set) Token: 0x0600117B RID: 4475 RVA: 0x00056CC4 File Offset: 0x00054EC4
		private MageElement currentElement
		{
			get
			{
				return this._currentElement;
			}
			set
			{
				if (value == this._currentElement)
				{
					return;
				}
				this._currentElement = value;
				this.RefreshCalibrationElement(this._currentElement);
			}
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x00056CE3 File Offset: 0x00054EE3
		private void OnInventoryChanged()
		{
			base.enabled = true;
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x00056CEC File Offset: 0x00054EEC
		private void FixedUpdate()
		{
			base.enabled = false;
			this.currentElement = this.GetAwardedElementFromInventory();
			if (this.hasEffectiveAuthority && this.currentElement == MageElement.None)
			{
				MageElement mageElement = this.CalcElementToAward();
				if (mageElement != MageElement.None && !(this.stateMachine.state is MageCalibrate))
				{
					MageCalibrate mageCalibrate = new MageCalibrate();
					mageCalibrate.element = mageElement;
					this.stateMachine.SetInterruptState(mageCalibrate, this.calibrationStateInterruptPriority);
				}
			}
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x00056D58 File Offset: 0x00054F58
		private MageElement GetAwardedElementFromInventory()
		{
			Inventory inventory = this.characterBody.inventory;
			if (inventory)
			{
				MageElement mageElement = (MageElement)inventory.GetItemCount(ItemIndex.MageAttunement);
				if (mageElement >= MageElement.None && mageElement < MageElement.Count)
				{
					return mageElement;
				}
			}
			return MageElement.None;
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x00056D90 File Offset: 0x00054F90
		private MageElement CalcElementToAward()
		{
			for (int i = 0; i < MageCalibrationController.elementCounter.Length; i++)
			{
				MageCalibrationController.elementCounter[i] = 0;
			}
			Inventory inventory = this.characterBody.inventory;
			if (!inventory)
			{
				return MageElement.None;
			}
			List<ItemIndex> itemAcquisitionOrder = inventory.itemAcquisitionOrder;
			for (int j = 0; j < itemAcquisitionOrder.Count; j++)
			{
				ItemIndex itemIndex = itemAcquisitionOrder[j];
				ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
				if (itemDef.mageElement != MageElement.None)
				{
					int num = 0;
					switch (itemDef.tier)
					{
					case ItemTier.Tier1:
						num = 1;
						break;
					case ItemTier.Tier2:
						num = 2;
						break;
					case ItemTier.Tier3:
						num = 3;
						break;
					case ItemTier.Lunar:
						num = 3;
						break;
					}
					MageCalibrationController.elementCounter[(int)itemDef.mageElement] += num * inventory.GetItemCount(itemIndex);
				}
			}
			EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(inventory.currentEquipmentIndex);
			if (equipmentDef != null && equipmentDef.mageElement != MageElement.None)
			{
				MageCalibrationController.elementCounter[(int)equipmentDef.mageElement] += 2;
			}
			MageElement result = MageElement.None;
			int num2 = 0;
			for (MageElement mageElement = MageElement.Fire; mageElement < MageElement.Count; mageElement += 1)
			{
				int num3 = MageCalibrationController.elementCounter[(int)mageElement];
				if (num3 > num2)
				{
					result = mageElement;
					num2 = num3;
				}
			}
			if (num2 >= 5)
			{
				return result;
			}
			return MageElement.None;
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x00056EC5 File Offset: 0x000550C5
		public MageElement GetActiveCalibrationElement()
		{
			return this.currentElement;
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x00056ED0 File Offset: 0x000550D0
		public void SetElement(MageElement newElement)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			Inventory inventory = this.characterBody.inventory;
			if (inventory)
			{
				MageElement mageElement = (MageElement)inventory.GetItemCount(ItemIndex.MageAttunement);
				if (mageElement != newElement)
				{
					int count = (int)(newElement - mageElement);
					inventory.GiveItem(ItemIndex.MageAttunement, count);
				}
			}
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x00056F18 File Offset: 0x00055118
		public void RefreshCalibrationElement(MageElement targetElement)
		{
			MageCalibrationController.CalibrationInfo calibrationInfo = this.calibrationInfos[(int)targetElement];
			this.calibrationOverlayRenderer.enabled = calibrationInfo.enableCalibrationOverlay;
			this.calibrationOverlayRenderer.material = calibrationInfo.calibrationOverlayMaterial;
		}

		// Token: 0x04001582 RID: 5506
		public MageCalibrationController.CalibrationInfo[] calibrationInfos;

		// Token: 0x04001583 RID: 5507
		public SkinnedMeshRenderer calibrationOverlayRenderer;

		// Token: 0x04001584 RID: 5508
		[Tooltip("The state machine upon which to perform the calibration state.")]
		public EntityStateMachine stateMachine;

		// Token: 0x04001585 RID: 5509
		[Tooltip("The priority with which the calibration state will try to interrupt the current state.")]
		public InterruptPriority calibrationStateInterruptPriority;

		// Token: 0x04001586 RID: 5510
		private CharacterBody characterBody;

		// Token: 0x04001587 RID: 5511
		private bool hasEffectiveAuthority;

		// Token: 0x04001588 RID: 5512
		private MageElement _currentElement;

		// Token: 0x04001589 RID: 5513
		private static readonly int[] elementCounter = new int[4];

		// Token: 0x0200034E RID: 846
		[Serializable]
		public struct CalibrationInfo
		{
			// Token: 0x0400158A RID: 5514
			public bool enableCalibrationOverlay;

			// Token: 0x0400158B RID: 5515
			public Material calibrationOverlayMaterial;
		}
	}
}
