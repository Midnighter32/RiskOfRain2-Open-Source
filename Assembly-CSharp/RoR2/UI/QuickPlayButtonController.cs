using System;
using RoR2.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000615 RID: 1557
	public class QuickPlayButtonController : UIBehaviour
	{
		// Token: 0x060024D3 RID: 9427 RVA: 0x000A09B1 File Offset: 0x0009EBB1
		protected new void Start()
		{
			base.Start();
			this.Update();
		}

		// Token: 0x060024D4 RID: 9428 RVA: 0x000A09C0 File Offset: 0x0009EBC0
		protected void Update()
		{
			bool running = SteamLobbyFinder.running;
			this.quickplayStateText.text = SteamLobbyFinder.GetResolvedStateString();
			if (running)
			{
				this.spinnerRectTransform.gameObject.SetActive(true);
				this.quickplayStateText.gameObject.SetActive(true);
				Vector3 localEulerAngles = this.spinnerRectTransform.localEulerAngles;
				localEulerAngles.z += Time.deltaTime * 360f;
				this.spinnerRectTransform.localEulerAngles = localEulerAngles;
				this.labelController.token = this.stopToken;
				return;
			}
			this.spinnerRectTransform.gameObject.SetActive(false);
			this.quickplayStateText.gameObject.SetActive(false);
			this.labelController.token = this.startToken;
		}

		// Token: 0x060024D5 RID: 9429 RVA: 0x000A0A79 File Offset: 0x0009EC79
		public void ToggleQuickplay()
		{
			Console.instance.SubmitCmd(null, SteamLobbyFinder.running ? "steam_quickplay_stop" : "steam_quickplay_start", false);
		}

		// Token: 0x0400229B RID: 8859
		public LanguageTextMeshController labelController;

		// Token: 0x0400229C RID: 8860
		public string startToken;

		// Token: 0x0400229D RID: 8861
		public string stopToken;

		// Token: 0x0400229E RID: 8862
		public RectTransform spinnerRectTransform;

		// Token: 0x0400229F RID: 8863
		public TextMeshProUGUI quickplayStateText;
	}
}
