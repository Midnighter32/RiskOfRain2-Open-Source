using System;
using System.Collections.Generic;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000278 RID: 632
	[RequireComponent(typeof(CharacterBody))]
	public class MageCalibrationController : MonoBehaviour
	{
		// Token: 0x06000E0A RID: 3594 RVA: 0x0003EC0C File Offset: 0x0003CE0C
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.characterBody.onInventoryChanged += this.OnInventoryChanged;
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06000E0B RID: 3595 RVA: 0x0003EC42 File Offset: 0x0003CE42
		private void Start()
		{
			this.currentElement = this.GetAwardedElementFromInventory();
			this.RefreshCalibrationElement(this.currentElement);
		}

		// Token: 0x06000E0C RID: 3596 RVA: 0x0003EC5C File Offset: 0x0003CE5C
		private void OnDestroy()
		{
			this.characterBody.onInventoryChanged -= this.OnInventoryChanged;
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000E0D RID: 3597 RVA: 0x0003EC75 File Offset: 0x0003CE75
		// (set) Token: 0x06000E0E RID: 3598 RVA: 0x0003EC7D File Offset: 0x0003CE7D
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

		// Token: 0x06000E0F RID: 3599 RVA: 0x0003EC9C File Offset: 0x0003CE9C
		private void OnInventoryChanged()
		{
			base.enabled = true;
		}

		// Token: 0x06000E10 RID: 3600 RVA: 0x0003ECA8 File Offset: 0x0003CEA8
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

		// Token: 0x06000E11 RID: 3601 RVA: 0x0003ED14 File Offset: 0x0003CF14
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

		// Token: 0x06000E12 RID: 3602 RVA: 0x0003ED4C File Offset: 0x0003CF4C
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
				ItemCatalog.GetItemDef(itemAcquisitionOrder[j]);
			}
			EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(inventory.currentEquipmentIndex);
			if (equipmentDef != null && equipmentDef.mageElement != MageElement.None)
			{
				MageCalibrationController.elementCounter[(int)equipmentDef.mageElement] += 2;
			}
			MageElement result = MageElement.None;
			int num = 0;
			for (MageElement mageElement = MageElement.Fire; mageElement < MageElement.Count; mageElement += 1)
			{
				int num2 = MageCalibrationController.elementCounter[(int)mageElement];
				if (num2 > num)
				{
					result = mageElement;
					num = num2;
				}
			}
			if (num >= 5)
			{
				return result;
			}
			return MageElement.None;
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x0003EE19 File Offset: 0x0003D019
		public MageElement GetActiveCalibrationElement()
		{
			return this.currentElement;
		}

		// Token: 0x06000E14 RID: 3604 RVA: 0x0003EE24 File Offset: 0x0003D024
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

		// Token: 0x06000E15 RID: 3605 RVA: 0x0003EE6C File Offset: 0x0003D06C
		public void RefreshCalibrationElement(MageElement targetElement)
		{
			MageCalibrationController.CalibrationInfo calibrationInfo = this.calibrationInfos[(int)targetElement];
			this.calibrationOverlayRenderer.enabled = calibrationInfo.enableCalibrationOverlay;
			this.calibrationOverlayRenderer.material = calibrationInfo.calibrationOverlayMaterial;
		}

		// Token: 0x04000E00 RID: 3584
		public MageCalibrationController.CalibrationInfo[] calibrationInfos;

		// Token: 0x04000E01 RID: 3585
		public SkinnedMeshRenderer calibrationOverlayRenderer;

		// Token: 0x04000E02 RID: 3586
		[Tooltip("The state machine upon which to perform the calibration state.")]
		public EntityStateMachine stateMachine;

		// Token: 0x04000E03 RID: 3587
		[Tooltip("The priority with which the calibration state will try to interrupt the current state.")]
		public InterruptPriority calibrationStateInterruptPriority;

		// Token: 0x04000E04 RID: 3588
		private CharacterBody characterBody;

		// Token: 0x04000E05 RID: 3589
		private bool hasEffectiveAuthority;

		// Token: 0x04000E06 RID: 3590
		private MageElement _currentElement;

		// Token: 0x04000E07 RID: 3591
		private static readonly int[] elementCounter = new int[4];

		// Token: 0x02000279 RID: 633
		[Serializable]
		public struct CalibrationInfo
		{
			// Token: 0x04000E08 RID: 3592
			public bool enableCalibrationOverlay;

			// Token: 0x04000E09 RID: 3593
			public Material calibrationOverlayMaterial;
		}
	}
}
