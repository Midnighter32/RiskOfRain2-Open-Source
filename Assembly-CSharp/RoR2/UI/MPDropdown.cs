using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000606 RID: 1542
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPDropdown : TMP_Dropdown
	{
		// Token: 0x17000304 RID: 772
		// (get) Token: 0x060022A8 RID: 8872 RVA: 0x000A3E6D File Offset: 0x000A206D
		protected MPEventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x060022A9 RID: 8873 RVA: 0x000A3E7A File Offset: 0x000A207A
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x060022AA RID: 8874 RVA: 0x000A3E90 File Offset: 0x000A2090
		private Selectable FilterSelectable(Selectable selectable)
		{
			if (selectable)
			{
				MPEventSystemLocator component = selectable.GetComponent<MPEventSystemLocator>();
				if (!component || component.eventSystem != this.eventSystemLocator.eventSystem)
				{
					selectable = null;
				}
			}
			return selectable;
		}

		// Token: 0x060022AB RID: 8875 RVA: 0x000A3ED0 File Offset: 0x000A20D0
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x060022AC RID: 8876 RVA: 0x000A3EDE File Offset: 0x000A20DE
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x060022AD RID: 8877 RVA: 0x000A3EEC File Offset: 0x000A20EC
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x000A3EFA File Offset: 0x000A20FA
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x060022AF RID: 8879 RVA: 0x000A3F08 File Offset: 0x000A2108
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x060022B0 RID: 8880 RVA: 0x000A3F3C File Offset: 0x000A213C
		private void AttemptSelection(PointerEventData eventData)
		{
			if (this.eventSystem && this.eventSystem.currentInputModule == eventData.currentInputModule)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
		}

		// Token: 0x060022B1 RID: 8881 RVA: 0x000A3F75 File Offset: 0x000A2175
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			this.isPointerInside = true;
			base.OnPointerEnter(eventData);
			this.AttemptSelection(eventData);
		}

		// Token: 0x060022B2 RID: 8882 RVA: 0x000A3F9C File Offset: 0x000A219C
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			if (this.eventSystem && base.gameObject == this.eventSystem.currentSelectedGameObject)
			{
				base.enabled = false;
				base.enabled = true;
			}
			this.isPointerInside = false;
			base.OnPointerExit(eventData);
		}

		// Token: 0x060022B3 RID: 8883 RVA: 0x000A3FF9 File Offset: 0x000A21F9
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerClick(eventData);
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x000A4011 File Offset: 0x000A2211
		public override void OnPointerUp(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			this.inPointerUp = true;
			base.OnPointerUp(eventData);
			this.inPointerUp = false;
		}

		// Token: 0x060022B5 RID: 8885 RVA: 0x000A4038 File Offset: 0x000A2238
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			if (this.IsInteractable() && base.navigation.mode != Navigation.Mode.None)
			{
				this.AttemptSelection(eventData);
			}
			base.OnPointerDown(eventData);
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x000A407A File Offset: 0x000A227A
		protected override void OnDisable()
		{
			base.OnDisable();
			this.isPointerInside = false;
		}

		// Token: 0x040025C1 RID: 9665
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040025C2 RID: 9666
		protected bool isPointerInside;

		// Token: 0x040025C3 RID: 9667
		public bool allowAllEventSystems;

		// Token: 0x040025C4 RID: 9668
		private bool inPointerUp;
	}
}
