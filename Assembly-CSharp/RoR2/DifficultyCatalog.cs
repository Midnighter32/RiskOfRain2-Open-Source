using System;

namespace RoR2
{
	// Token: 0x02000122 RID: 290
	public static class DifficultyCatalog
	{
		// Token: 0x06000536 RID: 1334 RVA: 0x00015120 File Offset: 0x00013320
		static DifficultyCatalog()
		{
			DifficultyCatalog.difficultyDefs[0] = new DifficultyDef(1f, "DIFFICULTY_EASY_NAME", "Textures/DifficultyIcons/texDifficultyEasyIcon", "DIFFICULTY_EASY_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.EasyDifficulty));
			DifficultyCatalog.difficultyDefs[1] = new DifficultyDef(2f, "DIFFICULTY_NORMAL_NAME", "Textures/DifficultyIcons/texDifficultyNormalIcon", "DIFFICULTY_NORMAL_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.NormalDifficulty));
			DifficultyCatalog.difficultyDefs[2] = new DifficultyDef(3f, "DIFFICULTY_HARD_NAME", "Textures/DifficultyIcons/texDifficultyHardIcon", "DIFFICULTY_HARD_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.HardDifficulty));
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x000151BC File Offset: 0x000133BC
		public static DifficultyDef GetDifficultyDef(DifficultyIndex difficultyIndex)
		{
			return HGArrayUtilities.GetSafe<DifficultyDef>(DifficultyCatalog.difficultyDefs, (int)difficultyIndex);
		}

		// Token: 0x0400056E RID: 1390
		private static readonly DifficultyDef[] difficultyDefs = new DifficultyDef[3];
	}
}
