using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000605 RID: 1541
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPButton : Button
	{
		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06002296 RID: 8854 RVA: 0x000A3BFE File Offset: 0x000A1DFE
		protected MPEventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002297 RID: 8855 RVA: 0x000A3C0C File Offset: 0x000A1E0C
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			Navigation navigation = base.navigation;
			navigation.mode = Navigation.Mode.None;
			base.navigation = navigation;
		}

		// Token: 0x06002298 RID: 8856 RVA: 0x000A3C44 File Offset: 0x000A1E44
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

		// Token: 0x06002299 RID: 8857 RVA: 0x000A3C84 File Offset: 0x000A1E84
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x0600229A RID: 8858 RVA: 0x000A3C92 File Offset: 0x000A1E92
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x0600229B RID: 8859 RVA: 0x000A3CA0 File Offset: 0x000A1EA0
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x0600229C RID: 8860 RVA: 0x000A3CAE File Offset: 0x000A1EAE
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x0600229D RID: 8861 RVA: 0x000A3CBC File Offset: 0x000A1EBC
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
		}

		// Token: 0x0600229E RID: 8862 RVA: 0x000A3CC8 File Offset: 0x000A1EC8
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x0600229F RID: 8863 RVA: 0x000A3CFC File Offset: 0x000A1EFC
		private void AttemptSelection(PointerEventData eventData)
		{
			if (this.eventSystem && this.eventSystem.currentInputModule == eventData.currentInputModule)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
		}

		// Token: 0x060022A0 RID: 8864 RVA: 0x000A3D35 File Offset: 0x000A1F35
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

		// Token: 0x060022A1 RID: 8865 RVA: 0x000A3D5C File Offset: 0x000A1F5C
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

		// Token: 0x060022A2 RID: 8866 RVA: 0x000A3DB9 File Offset: 0x000A1FB9
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerClick(eventData);
		}

		// Token: 0x060022A3 RID: 8867 RVA: 0x000A3DD1 File Offset: 0x000A1FD1
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

		// Token: 0x060022A4 RID: 8868 RVA: 0x000A3DF7 File Offset: 0x000A1FF7
		public override void OnSubmit(BaseEventData eventData)
		{
			if (this.pointerClickOnly && !this.inPointerUp)
			{
				return;
			}
			base.OnSubmit(eventData);
		}

		// Token: 0x060022A5 RID: 8869 RVA: 0x000A3E14 File Offset: 0x000A2014
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

		// Token: 0x060022A6 RID: 8870 RVA: 0x000A3E56 File Offset: 0x000A2056
		protected override void OnDisable()
		{
			base.OnDisable();
			this.isPointerInside = false;
		}

		// Token: 0x040025BC RID: 9660
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040025BD RID: 9661
		protected bool isPointerInside;

		// Token: 0x040025BE RID: 9662
		public bool allowAllEventSystems;

		// Token: 0x040025BF RID: 9663
		public bool pointerClickOnly;

		// Token: 0x040025C0 RID: 9664
		private bool inPointerUp;
	}
}
