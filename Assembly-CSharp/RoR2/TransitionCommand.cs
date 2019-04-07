using System;

namespace RoR2
{
	// Token: 0x0200045C RID: 1116
	public static class TransitionCommand
	{
		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060018F2 RID: 6386 RVA: 0x000779D0 File Offset: 0x00075BD0
		// (set) Token: 0x060018F3 RID: 6387 RVA: 0x000779D7 File Offset: 0x00075BD7
		public static bool requestPending { get; private set; }

		// Token: 0x060018F4 RID: 6388 RVA: 0x000779E0 File Offset: 0x00075BE0
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

		// Token: 0x060018F5 RID: 6389 RVA: 0x00077A30 File Offset: 0x00075C30
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

		// Token: 0x04001C66 RID: 7270
		private static float timer;

		// Token: 0x04001C67 RID: 7271
		private static string commandString;
	}
}
