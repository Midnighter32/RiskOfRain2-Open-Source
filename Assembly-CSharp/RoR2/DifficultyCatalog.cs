using System;

namespace RoR2
{
	// Token: 0x0200023C RID: 572
	public static class DifficultyCatalog
	{
		// Token: 0x06000ACB RID: 2763 RVA: 0x00035540 File Offset: 0x00033740
		static DifficultyCatalog()
		{
			DifficultyCatalog.difficultyDefs[0] = new DifficultyDef(1f, "DIFFICULTY_EASY_NAME", "Textures/DifficultyIcons/texDifficultyEasyIcon", "DIFFICULTY_EASY_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.EasyDifficulty));
			DifficultyCatalog.difficultyDefs[1] = new DifficultyDef(2f, "DIFFICULTY_NORMAL_NAME", "Textures/DifficultyIcons/texDifficultyNormalIcon", "DIFFICULTY_NORMAL_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.NormalDifficulty));
			DifficultyCatalog.difficultyDefs[2] = new DifficultyDef(3f, "DIFFICULTY_HARD_NAME", "Textures/DifficultyIcons/texDifficultyHardIcon", "DIFFICULTY_HARD_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.HardDifficulty));
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x000355DC File Offset: 0x000337DC
		public static DifficultyDef GetDifficultyDef(DifficultyIndex difficultyIndex)
		{
			return DifficultyCatalog.difficultyDefs[(int)difficultyIndex];
		}

		// Token: 0x04000E93 RID: 3731
		private static readonly DifficultyDef[] difficultyDefs = new DifficultyDef[3];
	}
}
