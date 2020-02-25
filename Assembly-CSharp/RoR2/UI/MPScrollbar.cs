using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005FE RID: 1534
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPScrollbar : Scrollbar
	{
		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06002464 RID: 9316 RVA: 0x0009EE81 File Offset: 0x0009D081
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002465 RID: 9317 RVA: 0x0009EE8E File Offset: 0x0009D08E
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002466 RID: 9318 RVA: 0x0009EEA4 File Offset: 0x0009D0A4
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

		// Token: 0x06002467 RID: 9319 RVA: 0x0009EEE4 File Offset: 0x0009D0E4
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x06002468 RID: 9320 RVA: 0x0009EF18 File Offset: 0x0009D118
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x06002469 RID: 9321 RVA: 0x0009EF26 File Offset: 0x0009D126
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x0600246A RID: 9322 RVA: 0x0009EF34 File Offset: 0x0009D134
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x0600246B RID: 9323 RVA: 0x0009EF42 File Offset: 0x0009D142
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x0600246C RID: 9324 RVA: 0x0009EF50 File Offset: 0x0009D150
		public override void OnPointerUp(PointerEventData eventData)
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
			base.OnPointerUp(eventData);
		}

		// Token: 0x0600246D RID: 9325 RVA: 0x0009EFA6 File Offset: 0x0009D1A6
		public override void OnSelect(BaseEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnSelect(eventData);
		}

		// Token: 0x0600246E RID: 9326 RVA: 0x0009EFBE File Offset: 0x0009D1BE
		public override void OnDeselect(BaseEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnDeselect(eventData);
		}

		// Token: 0x0600246F RID: 9327 RVA: 0x0009EFD6 File Offset: 0x0009D1D6
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerEnter(eventData);
		}

		// Token: 0x06002470 RID: 9328 RVA: 0x0009EFEE File Offset: 0x0009D1EE
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerExit(eventData);
		}

		// Token: 0x06002471 RID: 9329 RVA: 0x0009F008 File Offset: 0x0009D208
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if (this.IsInteractable() && base.navigation.mode != Navigation.Mode.None)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
			base.OnPointerDown(eventData);
		}

		// Token: 0x06002472 RID: 9330 RVA: 0x0009F04F File Offset: 0x0009D24F
		public override void Select()
		{
			if (this.eventSystem.alreadySelecting)
			{
				return;
			}
			this.eventSystem.SetSelectedGameObject(base.gameObject);
		}

		// Token: 0x04002230 RID: 8752
		public bool allowAllEventSystems;

		// Token: 0x04002231 RID: 8753
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002232 RID: 8754
		private RectTransform viewPortRectTransform;
	}
}
