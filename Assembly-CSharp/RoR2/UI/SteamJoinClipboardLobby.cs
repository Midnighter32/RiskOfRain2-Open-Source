using System;
using System.Globalization;
using Facepunch.Steamworks;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000638 RID: 1592
	public class SteamJoinClipboardLobby : MonoBehaviour
	{
		// Token: 0x170003DD RID: 989
		// (get) Token: 0x0600257B RID: 9595 RVA: 0x000A316D File Offset: 0x000A136D
		// (set) Token: 0x0600257C RID: 9596 RVA: 0x000A3175 File Offset: 0x000A1375
		public bool validClipboardLobbyID { get; private set; }

		// Token: 0x0600257D RID: 9597 RVA: 0x000A317E File Offset: 0x000A137E
		private void OnEnable()
		{
			SingletonHelper.Assign<SteamJoinClipboardLobby>(ref SteamJoinClipboardLobby.instance, this);
		}

		// Token: 0x0600257E RID: 9598 RVA: 0x000A318B File Offset: 0x000A138B
		private void OnDisable()
		{
			SingletonHelper.Unassign<SteamJoinClipboardLobby>(ref SteamJoinClipboardLobby.instance, this);
		}

		// Token: 0x0600257F RID: 9599 RVA: 0x000A3198 File Offset: 0x000A1398
		static SteamJoinClipboardLobby()
		{
			SteamworksLobbyManager.onLobbyJoined += SteamJoinClipboardLobby.OnLobbyJoined;
		}

		// Token: 0x06002580 RID: 9600 RVA: 0x000A31AC File Offset: 0x000A13AC
		private static void OnLobbyJoined(bool success)
		{
			if (SteamJoinClipboardLobby.instance && SteamJoinClipboardLobby.instance.resultTextComponent)
			{
				SteamJoinClipboardLobby.instance.resultTextTimer = 4f;
				SteamJoinClipboardLobby.instance.resultTextComponent.text = Language.GetString(success ? "STEAM_JOIN_LOBBY_CLIPBOARD_SUCCESS" : "STEAM_JOIN_LOBBY_CLIPBOARD_FAIL");
			}
		}

		// Token: 0x06002581 RID: 9601 RVA: 0x000A3208 File Offset: 0x000A1408
		private void FixedUpdate()
		{
			Client client = Client.Instance;
			this.validClipboardLobbyID = false;
			if (client != null)
			{
				string systemCopyBuffer = GUIUtility.systemCopyBuffer;
				this.validClipboardLobbyID = (CSteamID.TryParse(systemCopyBuffer, out this.clipboardLobbyID) && this.clipboardLobbyID.isLobby && this.clipboardLobbyID.value != client.Lobby.CurrentLobby);
			}
			this.buttonText.text = string.Format(Language.GetString("STEAM_JOIN_LOBBY_ON_CLIPBOARD"), Array.Empty<object>());
			if (this.resultTextTimer > 0f)
			{
				this.resultTextTimer -= Time.fixedDeltaTime;
				this.resultTextComponent.enabled = true;
				return;
			}
			this.resultTextComponent.enabled = false;
		}

		// Token: 0x06002582 RID: 9602 RVA: 0x000A32C1 File Offset: 0x000A14C1
		public void TryToJoinClipboardLobby()
		{
			Console.instance.SubmitCmd(null, string.Format(CultureInfo.InvariantCulture, "steam_lobby_join {0}", this.clipboardLobbyID.ToString()), true);
		}

		// Token: 0x04002331 RID: 9009
		public TextMeshProUGUI buttonText;

		// Token: 0x04002332 RID: 9010
		public TextMeshProUGUI resultTextComponent;

		// Token: 0x04002333 RID: 9011
		public MPButton mpButton;

		// Token: 0x04002334 RID: 9012
		private CSteamID clipboardLobbyID;

		// Token: 0x04002336 RID: 9014
		private const float resultTextDuration = 4f;

		// Token: 0x04002337 RID: 9015
		protected float resultTextTimer;

		// Token: 0x04002338 RID: 9016
		private static SteamJoinClipboardLobby instance;
	}
}
