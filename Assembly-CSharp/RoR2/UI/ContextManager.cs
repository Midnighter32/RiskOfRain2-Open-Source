using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005C8 RID: 1480
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ContextManager : MonoBehaviour
	{
		// Token: 0x0600213F RID: 8511 RVA: 0x0009C2B8 File Offset: 0x0009A4B8
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x06002140 RID: 8512 RVA: 0x0009C2C6 File Offset: 0x0009A4C6
		private void Start()
		{
			this.Update();
		}

		// Token: 0x06002141 RID: 8513 RVA: 0x0009C2D0 File Offset: 0x0009A4D0
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
							if (component3 != null)
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

		// Token: 0x040023C5 RID: 9157
		public TextMeshProUGUI glyphTMP;

		// Token: 0x040023C6 RID: 9158
		public TextMeshProUGUI descriptionTMP;

		// Token: 0x040023C7 RID: 9159
		public GameObject contextDisplay;

		// Token: 0x040023C8 RID: 9160
		public HUD hud;

		// Token: 0x040023C9 RID: 9161
		private MPEventSystemLocator eventSystemLocator;
	}
}
