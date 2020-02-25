using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005F5 RID: 1525
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class MPDropdown : TMP_Dropdown
	{
		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06002418 RID: 9240 RVA: 0x0009DFDD File Offset: 0x0009C1DD
		protected MPEventSystem eventSystem
		{
			get
			{
				return this.eventSystemLocator.eventSystem;
			}
		}

		// Token: 0x06002419 RID: 9241 RVA: 0x0009DFEA File Offset: 0x0009C1EA
		protected override void Awake()
		{
			base.Awake();
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x0600241A RID: 9242 RVA: 0x0009E000 File Offset: 0x0009C200
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

		// Token: 0x0600241B RID: 9243 RVA: 0x0009E040 File Offset: 0x0009C240
		public override Selectable FindSelectableOnDown()
		{
			return this.FilterSelectable(base.FindSelectableOnDown());
		}

		// Token: 0x0600241C RID: 9244 RVA: 0x0009E04E File Offset: 0x0009C24E
		public override Selectable FindSelectableOnLeft()
		{
			return this.FilterSelectable(base.FindSelectableOnLeft());
		}

		// Token: 0x0600241D RID: 9245 RVA: 0x0009E05C File Offset: 0x0009C25C
		public override Selectable FindSelectableOnRight()
		{
			return this.FilterSelectable(base.FindSelectableOnRight());
		}

		// Token: 0x0600241E RID: 9246 RVA: 0x0009E06A File Offset: 0x0009C26A
		public override Selectable FindSelectableOnUp()
		{
			return this.FilterSelectable(base.FindSelectableOnUp());
		}

		// Token: 0x0600241F RID: 9247 RVA: 0x0009E078 File Offset: 0x0009C278
		public bool InputModuleIsAllowed(BaseInputModule inputModule)
		{
			if (this.allowAllEventSystems)
			{
				return true;
			}
			EventSystem eventSystem = this.eventSystem;
			return eventSystem && inputModule == eventSystem.currentInputModule;
		}

		// Token: 0x06002420 RID: 9248 RVA: 0x0009E0AC File Offset: 0x0009C2AC
		private void AttemptSelection(PointerEventData eventData)
		{
			if (this.eventSystem && this.eventSystem.currentInputModule == eventData.currentInputModule)
			{
				this.eventSystem.SetSelectedGameObject(base.gameObject, eventData);
			}
		}

		// Token: 0x06002421 RID: 9249 RVA: 0x0009E0E5 File Offset: 0x0009C2E5
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

		// Token: 0x06002422 RID: 9250 RVA: 0x0009E10C File Offset: 0x0009C30C
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

		// Token: 0x06002423 RID: 9251 RVA: 0x0009E169 File Offset: 0x0009C369
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!this.InputModuleIsAllowed(eventData.currentInputModule))
			{
				return;
			}
			base.OnPointerClick(eventData);
		}

		// Token: 0x06002424 RID: 9252 RVA: 0x0009E181 File Offset: 0x0009C381
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

		// Token: 0x06002425 RID: 9253 RVA: 0x0009E1A8 File Offset: 0x0009C3A8
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

		// Token: 0x06002426 RID: 9254 RVA: 0x0009E1EA File Offset: 0x0009C3EA
		protected override void OnDisable()
		{
			base.OnDisable();
			this.isPointerInside = false;
		}

		// Token: 0x04002205 RID: 8709
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002206 RID: 8710
		protected bool isPointerInside;

		// Token: 0x04002207 RID: 8711
		public bool allowAllEventSystems;

		// Token: 0x04002208 RID: 8712
		private bool inPointerUp;
	}
}
