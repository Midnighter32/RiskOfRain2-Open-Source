using System;

namespace RoR2
{
	// Token: 0x02000200 RID: 512
	public static class ArtifactCatalog
	{
		// Token: 0x06000A02 RID: 2562 RVA: 0x00031BEE File Offset: 0x0002FDEE
		private static void RegisterArtifact(ArtifactIndex artifactIndex, ArtifactDef artifactDef)
		{
			ArtifactCatalog.artifactDefs[(int)artifactIndex] = artifactDef;
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00031BF8 File Offset: 0x0002FDF8
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

		// Token: 0x06000A04 RID: 2564 RVA: 0x00031D23 File Offset: 0x0002FF23
		public static ArtifactDef GetArtifactDef(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return null;
			}
			return ArtifactCatalog.artifactDefs[(int)artifactIndex];
		}

		// Token: 0x04000D45 RID: 3397
		private static ArtifactDef[] artifactDefs = new ArtifactDef[5];
	}
}
