using System;

namespace RoR2
{
	// Token: 0x020003D3 RID: 979
	public static class TransitionCommand
	{
		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x060017CB RID: 6091 RVA: 0x000672D9 File Offset: 0x000654D9
		// (set) Token: 0x060017CC RID: 6092 RVA: 0x000672E0 File Offset: 0x000654E0
		public static bool requestPending { get; private set; }

		// Token: 0x060017CD RID: 6093 RVA: 0x000672E8 File Offset: 0x000654E8
		private static void Update()
		{
			if (FadeToBlackManager.fullyFaded)
			{
				RoR2Application.onUpdate -= TransitionCommand.Update;
				TransitionCommand.requestPending = false;
				FadeToBlackManager.fadeCount--;
				string cmd = TransitionCommand.commandString;
				TransitionCommand.commandString = null;
				Console.instance.SubmitCmd(null, cmd, false);
			}
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x00067338 File Offset: 0x00065538
		[ConCommand(commandName = "transition_command", flags = ConVarFlags.None, helpText = "Fade out and execute a command at the end of the fadeout.")]
		private static void CCTransitionCommand(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			if (TransitionCommand.requestPending)
			{
				return;
			}
			TransitionCommand.requestPending = true;
			TransitionCommand.commandString = args[0];
			FadeToBlackManager.fadeCount++;
			RoR2Application.onUpdate += TransitionCommand.Update;
		}

		// Token: 0x04001668 RID: 5736
		private static float timer;

		// Token: 0x04001669 RID: 5737
		private static string commandString;
	}
}
