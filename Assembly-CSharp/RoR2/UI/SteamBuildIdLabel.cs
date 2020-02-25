using System;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000637 RID: 1591
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SteamBuildIdLabel : MonoBehaviour
	{
		// Token: 0x06002579 RID: 9593 RVA: 0x000A310C File Offset: 0x000A130C
		private void Start()
		{
			if (Client.Instance != null)
			{
				string text = "Steam Build ID " + RoR2Application.GetBuildId();
				string betaName = Client.Instance.BetaName;
				if (!string.IsNullOrEmpty(betaName))
				{
					text = text + "[" + betaName + "]";
				}
				base.GetComponent<TextMeshProUGUI>().text = text;
				return;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
