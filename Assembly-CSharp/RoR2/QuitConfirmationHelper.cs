using System;
using RoR2.UI;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F3 RID: 1011
	public static class QuitConfirmationHelper
	{
		// Token: 0x06001885 RID: 6277 RVA: 0x00069B7F File Offset: 0x00067D7F
		private static bool IsQuitConfirmationRequired()
		{
			return Run.instance && !GameOverController.instance;
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x00069B9C File Offset: 0x00067D9C
		public static void IssueQuitCommand(NetworkUser sender, string consoleCmd)
		{
			QuitConfirmationHelper.<>c__DisplayClass2_0 CS$<>8__locals1 = new QuitConfirmationHelper.<>c__DisplayClass2_0();
			CS$<>8__locals1.sender = sender;
			CS$<>8__locals1.consoleCmd = consoleCmd;
			QuitConfirmationHelper.IssueQuitCommand(new Action(CS$<>8__locals1.<IssueQuitCommand>g__RunCmd|0));
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x00069BC4 File Offset: 0x00067DC4
		public static void IssueQuitCommand(Action action)
		{
			if (!QuitConfirmationHelper.IsQuitConfirmationRequired())
			{
				action();
				return;
			}
			QuitConfirmationHelper.NetworkStatus networkStatus;
			if (NetworkUser.readOnlyInstancesList.Count <= NetworkUser.readOnlyLocalPlayersList.Count)
			{
				networkStatus = QuitConfirmationHelper.NetworkStatus.SinglePlayer;
			}
			else if (NetworkServer.active)
			{
				networkStatus = QuitConfirmationHelper.NetworkStatus.Host;
			}
			else
			{
				networkStatus = QuitConfirmationHelper.NetworkStatus.Client;
			}
			string token;
			switch (networkStatus)
			{
			case QuitConfirmationHelper.NetworkStatus.None:
				token = "";
				break;
			case QuitConfirmationHelper.NetworkStatus.SinglePlayer:
				token = "QUIT_RUN_CONFIRM_DIALOG_BODY_SINGLEPLAYER";
				break;
			case QuitConfirmationHelper.NetworkStatus.Client:
				token = "QUIT_RUN_CONFIRM_DIALOG_BODY_CLIENT";
				break;
			case QuitConfirmationHelper.NetworkStatus.Host:
				token = "QUIT_RUN_CONFIRM_DIALOG_BODY_HOST";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			SimpleDialogBox simpleDialogBox = SimpleDialogBox.Create(null);
			simpleDialogBox.headerToken = new SimpleDialogBox.TokenParamsPair("QUIT_RUN_CONFIRM_DIALOG_TITLE", Array.Empty<object>());
			simpleDialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair(token, Array.Empty<object>());
			simpleDialogBox.AddActionButton(new UnityAction(action.Invoke), "DIALOG_OPTION_YES", Array.Empty<object>());
			simpleDialogBox.AddCancelButton("CANCEL", Array.Empty<object>());
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x00069CA5 File Offset: 0x00067EA5
		[ConCommand(commandName = "quit_confirmed_command", flags = ConVarFlags.None, helpText = "Runs the command provided in the argument only if the user confirms they want to quit the current game via dialog UI.")]
		private static void CCQuitConfirmedCommand(ConCommandArgs args)
		{
			QuitConfirmationHelper.<>c__DisplayClass4_0 CS$<>8__locals1 = new QuitConfirmationHelper.<>c__DisplayClass4_0();
			CS$<>8__locals1.sender = args.sender;
			CS$<>8__locals1.consoleCmd = args[0];
			QuitConfirmationHelper.IssueQuitCommand(new Action(CS$<>8__locals1.<CCQuitConfirmedCommand>g__RunCmd|0));
		}

		// Token: 0x020003F4 RID: 1012
		private enum NetworkStatus
		{
			// Token: 0x04001706 RID: 5894
			None,
			// Token: 0x04001707 RID: 5895
			SinglePlayer,
			// Token: 0x04001708 RID: 5896
			Client,
			// Token: 0x04001709 RID: 5897
			Host
		}
	}
}
