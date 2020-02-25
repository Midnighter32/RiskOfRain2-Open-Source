using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005F4 RID: 1524
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPButton : Button
	{
		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06002406 RID: 9222 RVA: 0x0009DD6F File Offset: 0x0009BF6F
		protected MPEventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002407 RID: 9223 RVA: 0x0009DD7C File Offset: 0x0009BF7C
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			Navigation navigation = base.navigation;
			navigation.mode = Navigation.Mode.None;
			base.navigation = navigation;
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x0009DDB4 File Offset: 0x0009BFB4
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

		// Token: 0x06002409 RID: 9225 RVA: 0x0009DDF4 File Offset: 0x0009BFF4
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x0600240A RID: 9226 RVA: 0x0009DE02 File Offset: 0x0009C002
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x0600240B RID: 9227 RVA: 0x0009DE10 File Offset: 0x0009C010
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x0600240C RID: 9228 RVA: 0x0009DE1E File Offset: 0x0009C01E
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x0009DE2C File Offset: 0x0009C02C
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
		}

		// Token: 0x0600240E RID: 9230 RVA: 0x0009DE38 File Offset: 0x0009C038
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x0600240F RID: 9231 RVA: 0x0009DE6C File Offset: 0x0009C06C
		private void AttemptSelection(PointerEventData eventData)
		{
			if (this.eventSystem && this.eventSystem.currentInputModule == eventData.currentInputModule)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
		}

		// Token: 0x06002410 RID: 9232 RVA: 0x0009DEA5 File Offset: 0x0009C0A5
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

		// Token: 0x06002411 RID: 9233 RVA: 0x0009DECC File Offset: 0x0009C0CC
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

		// Token: 0x06002412 RID: 9234 RVA: 0x0009DF29 File Offset: 0x0009C129
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerClick(eventData);
		}

		// Token: 0x06002413 RID: 9235 RVA: 0x0009DF41 File Offset: 0x0009C141
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

		// Token: 0x06002414 RID: 9236 RVA: 0x0009DF67 File Offset: 0x0009C167
		public override void OnSubmit(BaseEventData eventData)
		{
			if (this.pointerClickOnly && !this.inPointerUp)
			{
				return;
			}
			base.OnSubmit(eventData);
		}

		// Token: 0x06002415 RID: 9237 RVA: 0x0009DF84 File Offset: 0x0009C184
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

		// Token: 0x06002416 RID: 9238 RVA: 0x0009DFC6 File Offset: 0x0009C1C6
		protected override void OnDisable()
		{
			base.OnDisable();
			this.isPointerInside = false;
		}

		// Token: 0x04002200 RID: 8704
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002201 RID: 8705
		protected bool isPointerInside;

		// Token: 0x04002202 RID: 8706
		public bool allowAllEventSystems;

		// Token: 0x04002203 RID: 8707
		public bool pointerClickOnly;

		// Token: 0x04002204 RID: 8708
		private bool inPointerUp;
	}
}
