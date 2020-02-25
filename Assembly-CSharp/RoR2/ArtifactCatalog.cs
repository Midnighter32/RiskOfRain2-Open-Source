using System;

namespace RoR2
{
	// Token: 0x020000C1 RID: 193
	public static class ArtifactCatalog
	{
		// Token: 0x060003C7 RID: 967 RVA: 0x0000E8BA File Offset: 0x0000CABA
		private static void RegisterArtifact(ArtifactIndex artifactIndex, ArtifactDef artifactDef)
		{
			ArtifactCatalog.artifactDefs[(int)artifactIndex] = artifactDef;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0000E8C4 File Offset: 0x0000CAC4
		static ArtifactCatalog()
		{
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Command, new ArtifactDef
			{
				nameToken = "ARTIFACT_COMMAND_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texCommandSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texCommandSmallDeselected",
				unlockableName = "artifact_command"
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Bomb, new ArtifactDef
			{
				nameToken = "ARTIFACT_BOMB_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texSpiteSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texSpiteSmallDeselected",
				unlockableName = "artifact_bomb"
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Sacrifice, new ArtifactDef
			{
				nameToken = "ARTIFACT_SACRIFICE_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texSacrificeSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texSacrificeSmallDeselected",
				unlockableName = "artifact_sacrifice"
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Enigma, new ArtifactDef
			{
				nameToken = "ARTIFACT_ENIGMA_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texEnigmaSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texEnigmaSmallDeselected",
				unlockableName = "artifact_enigma"
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Spirit, new ArtifactDef
			{
				nameToken = "ARTIFACT_SPIRIT_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texSpiritSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texSpiritSmallDeselected",
				unlockableName = "artifact_spirit"
			});
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0000E9EF File Offset: 0x0000CBEF
		public static ArtifactDef GetArtifactDef(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return null;
			}
			return ArtifactCatalog.artifactDefs[(int)artifactIndex];
		}

		// Token: 0x04000355 RID: 853
		private static ArtifactDef[] artifactDefs = new ArtifactDef[5];
	}
}
