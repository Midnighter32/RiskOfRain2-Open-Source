﻿using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005A6 RID: 1446
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ContextManager : MonoBehaviour
	{
		// Token: 0x0600226E RID: 8814 RVA: 0x0009508F File Offset: 0x0009328F
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x0600226F RID: 8815 RVA: 0x0009509D File Offset: 0x0009329D
		private void Start()
		{
			this.Update();
		}

		// Token: 0x06002270 RID: 8816 RVA: 0x000950A8 File Offset: 0x000932A8
		private void Update()
		{
			string text = "";
			string text2 = "";
			bool active = false;
			if (this.hud && this.hud.targetBodyObject)
			{
				InteractionDriver component = this.hud.targetBodyObject.GetComponent<InteractionDriver>();
				if (component)
				{
					GameObject gameObject = component.FindBestInteractableObject();
					if (gameObject)
					{
						PlayerCharacterMasterController component2 = this.hud.targetMaster.GetComponent<PlayerCharacterMasterController>();
						if (component2 && component2.networkUser && component2.networkUser.localUser != null)
						{
							IInteractable component3 = gameObject.GetComponent<IInteractable>();
							if (component3 != null && ((MonoBehaviour)component3).isActiveAndEnabled)
							{
								string text3 = (component3.GetInteractability(component.interactor) == Interactability.Available) ? component3.GetContextString(component.interactor) : null;
								if (text3 != null)
								{
									text2 = text3;
									text = string.Format(CultureInfo.InvariantCulture, "<style=cKeyBinding>{0}</style>", Glyphs.GetGlyphString(this.eventSystemLocator, "Interact"));
									active = true;
								}
							}
						}
					}
				}
			}
			this.glyphTMP.text = text;
			this.descriptionTMP.text = text2;
			this.contextDisplay.SetActive(active);
		}

		// Token: 0x04001FCD RID: 8141
		public TextMeshProUGUI glyphTMP;

		// Token: 0x04001FCE RID: 8142
		public TextMeshProUGUI descriptionTMP;

		// Token: 0x04001FCF RID: 8143
		public GameObject contextDisplay;

		// Token: 0x04001FD0 RID: 8144
		public HUD hud;

		// Token: 0x04001FD1 RID: 8145
		private MPEventSystemLocator eventSystemLocator;
	}
}
