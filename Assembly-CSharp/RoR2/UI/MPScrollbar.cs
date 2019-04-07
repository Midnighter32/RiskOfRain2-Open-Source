using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200060F RID: 1551
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPScrollbar : Scrollbar
	{
		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060022F4 RID: 8948 RVA: 0x000A4D11 File Offset: 0x000A2F11
		private EventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x060022F5 RID: 8949 RVA: 0x000A4D1E File Offset: 0x000A2F1E
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x000A4D34 File Offset: 0x000A2F34
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

		// Token: 0x060022F7 RID: 8951 RVA: 0x000A4D74 File Offset: 0x000A2F74
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x060022F8 RID: 8952 RVA: 0x000A4DA8 File Offset: 0x000A2FA8
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x060022F9 RID: 8953 RVA: 0x000A4DB6 File Offset: 0x000A2FB6
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x000A4DC4 File Offset: 0x000A2FC4
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x060022FB RID: 8955 RVA: 0x000A4DD2 File Offset: 0x000A2FD2
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x060022FC RID: 8956 RVA: 0x000A4DE0 File Offset: 0x000A2FE0
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

		// Token: 0x060022FD RID: 8957 RVA: 0x000A4E36 File Offset: 0x000A3036
		public override void OnSelect(BaseEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnSelect(eventData);
		}

		// Token: 0x060022FE RID: 8958 RVA: 0x000A4E4E File Offset: 0x000A304E
		public override void OnDeselect(BaseEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnDeselect(eventData);
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x000A4E66 File Offset: 0x000A3066
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerEnter(eventData);
		}

		// Token: 0x06002300 RID: 8960 RVA: 0x000A4E7E File Offset: 0x000A307E
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerExit(eventData);
		}

		// Token: 0x06002301 RID: 8961 RVA: 0x000A4E98 File Offset: 0x000A3098
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

		// Token: 0x06002302 RID: 8962 RVA: 0x000A4EDF File Offset: 0x000A30DF
		public override void Select()
		{
			if (this.eventSystem.alreadySelecting)
			{
				return;
			}
			this.eventSystem.SetSelectedGameObject(base.gameObject);
		}

		// Token: 0x040025EC RID: 9708
		public bool allowAllEventSystems;

		// Token: 0x040025ED RID: 9709
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040025EE RID: 9710
		private RectTransform viewPortRectTransform;
	}
}
