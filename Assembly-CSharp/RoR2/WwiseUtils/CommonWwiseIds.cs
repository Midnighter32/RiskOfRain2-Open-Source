using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RoR2.WwiseUtils
{
	// Token: 0x02000493 RID: 1171
	public static class CommonWwiseIds
	{
		// Token: 0x06001C6D RID: 7277 RVA: 0x000798DC File Offset: 0x00077ADC
		[RuntimeInitializeOnLoadMethod]
		public static void Init()
		{
			CommonWwiseIds.<Init>g__Assign|9_0(ref CommonWwiseIds.none, "None");
			CommonWwiseIds.<Init>g__Assign|9_0(ref CommonWwiseIds.alive, "alive");
			CommonWwiseIds.<Init>g__Assign|9_0(ref CommonWwiseIds.dead, "dead");
			CommonWwiseIds.<Init>g__Assign|9_0(ref CommonWwiseIds.bossfight, "Bossfight");
			CommonWwiseIds.<Init>g__Assign|9_0(ref CommonWwiseIds.gameplay, "Gameplay");
			CommonWwiseIds.<Init>g__Assign|9_0(ref CommonWwiseIds.menu, "Menu");
			CommonWwiseIds.<Init>g__Assign|9_0(ref CommonWwiseIds.main, "Main");
			CommonWwiseIds.<Init>g__Assign|9_0(ref CommonWwiseIds.logbook, "Logbook");
			CommonWwiseIds.<Init>g__Assign|9_0(ref CommonWwiseIds.secretLevel, "SecretLevel");
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x00079970 File Offset: 0x00077B70
		[CompilerGenerated]
		internal static void <Init>g__Assign|9_0(ref uint field, string name)
		{
			field = AkSoundEngine.GetIDFromString(name);
		}

		// Token: 0x04001961 RID: 6497
		public static uint none;

		// Token: 0x04001962 RID: 6498
		public static uint alive;

		// Token: 0x04001963 RID: 6499
		public static uint bossfight;

		// Token: 0x04001964 RID: 6500
		public static uint dead;

		// Token: 0x04001965 RID: 6501
		public static uint gameplay;

		// Token: 0x04001966 RID: 6502
		public static uint menu;

		// Token: 0x04001967 RID: 6503
		public static uint main;

		// Token: 0x04001968 RID: 6504
		public static uint logbook;

		// Token: 0x04001969 RID: 6505
		public static uint secretLevel;
	}
}
