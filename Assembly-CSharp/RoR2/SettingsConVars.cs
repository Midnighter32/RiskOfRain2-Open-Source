using System;
using System.Globalization;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x02000413 RID: 1043
	public static class SettingsConVars
	{
		// Token: 0x0400178B RID: 6027
		public static readonly BoolConVar cvExpAndMoneyEffects = new BoolConVar("exp_and_money_effects", ConVarFlags.Archive, "1", "Whether or not to create effects for experience and money from defeating monsters.");

		// Token: 0x02000414 RID: 1044
		private class VSyncCountConVar : BaseConVar
		{
			// Token: 0x06001934 RID: 6452 RVA: 0x0000972B File Offset: 0x0000792B
			private VSyncCountConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001935 RID: 6453 RVA: 0x0006CE68 File Offset: 0x0006B068
			public override void SetString(string newValue)
			{
				int vSyncCount;
				if (TextSerialization.TryParseInvariant(newValue, out vSyncCount))
				{
					QualitySettings.vSyncCount = vSyncCount;
				}
			}

			// Token: 0x06001936 RID: 6454 RVA: 0x0006CE85 File Offset: 0x0006B085
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.vSyncCount);
			}

			// Token: 0x0400178C RID: 6028
			private static SettingsConVars.VSyncCountConVar instance = new SettingsConVars.VSyncCountConVar("vsync_count", ConVarFlags.Archive | ConVarFlags.Engine, null, "Vsync count.");
		}

		// Token: 0x02000415 RID: 1045
		private class WindowModeConVar : BaseConVar
		{
			// Token: 0x06001938 RID: 6456 RVA: 0x0000972B File Offset: 0x0000792B
			private WindowModeConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001939 RID: 6457 RVA: 0x0006CEAC File Offset: 0x0006B0AC
			public override void SetString(string newValue)
			{
				try
				{
					FullScreenMode fullScreenMode = FullScreenMode.ExclusiveFullScreen;
					switch ((SettingsConVars.WindowModeConVar.WindowMode)Enum.Parse(typeof(SettingsConVars.WindowModeConVar.WindowMode), newValue, true))
					{
					case SettingsConVars.WindowModeConVar.WindowMode.Fullscreen:
						fullScreenMode = FullScreenMode.FullScreenWindow;
						break;
					case SettingsConVars.WindowModeConVar.WindowMode.Window:
						fullScreenMode = FullScreenMode.Windowed;
						break;
					case SettingsConVars.WindowModeConVar.WindowMode.FullscreenExclusive:
						fullScreenMode = FullScreenMode.ExclusiveFullScreen;
						break;
					}
					Screen.fullScreenMode = fullScreenMode;
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x0600193A RID: 6458 RVA: 0x0006CF18 File Offset: 0x0006B118
			public override string GetString()
			{
				SettingsConVars.WindowModeConVar.WindowMode windowMode;
				switch (Screen.fullScreenMode)
				{
				case FullScreenMode.ExclusiveFullScreen:
					windowMode = SettingsConVars.WindowModeConVar.WindowMode.FullscreenExclusive;
					break;
				case FullScreenMode.FullScreenWindow:
					windowMode = SettingsConVars.WindowModeConVar.WindowMode.Fullscreen;
					break;
				case FullScreenMode.MaximizedWindow:
					windowMode = SettingsConVars.WindowModeConVar.WindowMode.Window;
					break;
				case FullScreenMode.Windowed:
					windowMode = SettingsConVars.WindowModeConVar.WindowMode.Window;
					break;
				default:
					windowMode = SettingsConVars.WindowModeConVar.WindowMode.Fullscreen;
					break;
				}
				return windowMode.ToString();
			}

			// Token: 0x0400178D RID: 6029
			private static SettingsConVars.WindowModeConVar instance = new SettingsConVars.WindowModeConVar("window_mode", ConVarFlags.Archive | ConVarFlags.Engine, null, "The window mode. Choices are Fullscreen and Window.");

			// Token: 0x02000416 RID: 1046
			private enum WindowMode
			{
				// Token: 0x0400178F RID: 6031
				Fullscreen,
				// Token: 0x04001790 RID: 6032
				Window,
				// Token: 0x04001791 RID: 6033
				FullscreenExclusive
			}
		}

		// Token: 0x02000417 RID: 1047
		private class ResolutionConVar : BaseConVar
		{
			// Token: 0x0600193C RID: 6460 RVA: 0x0000972B File Offset: 0x0000792B
			private ResolutionConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600193D RID: 6461 RVA: 0x0006CF7C File Offset: 0x0006B17C
			public override void SetString(string newValue)
			{
				string[] array = newValue.Split(new char[]
				{
					'x'
				});
				int width;
				if (array.Length < 1 || !TextSerialization.TryParseInvariant(array[0], out width))
				{
					throw new ConCommandException("Invalid resolution format. No width integer. Example: \"1920x1080x60\".");
				}
				int height;
				if (array.Length < 2 || !TextSerialization.TryParseInvariant(array[1], out height))
				{
					throw new ConCommandException("Invalid resolution format. No height integer. Example: \"1920x1080x60\".");
				}
				int preferredRefreshRate;
				if (array.Length < 3 || !TextSerialization.TryParseInvariant(array[2], out preferredRefreshRate))
				{
					throw new ConCommandException("Invalid resolution format. No refresh rate integer. Example: \"1920x1080x60\".");
				}
				Screen.SetResolution(width, height, Screen.fullScreenMode, preferredRefreshRate);
			}

			// Token: 0x0600193E RID: 6462 RVA: 0x0006D000 File Offset: 0x0006B200
			public override string GetString()
			{
				Resolution currentResolution = Screen.currentResolution;
				return string.Format(CultureInfo.InvariantCulture, "{0}x{1}x{2}", Screen.width, Screen.height, currentResolution.refreshRate);
			}

			// Token: 0x0600193F RID: 6463 RVA: 0x0006D044 File Offset: 0x0006B244
			[ConCommand(commandName = "resolution_list", flags = ConVarFlags.None, helpText = "Prints a list of all possible resolutions for the current display.")]
			private static void CCResolutionList(ConCommandArgs args)
			{
				Resolution[] resolutions = Screen.resolutions;
				string[] array = new string[resolutions.Length];
				for (int i = 0; i < resolutions.Length; i++)
				{
					Resolution resolution = resolutions[i];
					array[i] = string.Format("{0}x{1}x{2}", resolution.width, resolution.height, resolution.refreshRate);
				}
				Debug.Log(string.Join("\n", array));
			}

			// Token: 0x04001792 RID: 6034
			private static SettingsConVars.ResolutionConVar instance = new SettingsConVars.ResolutionConVar("resolution", ConVarFlags.Archive | ConVarFlags.Engine, null, "The resolution of the game window. Format example: 1920x1080x60");
		}

		// Token: 0x02000418 RID: 1048
		private class FpsMaxConVar : BaseConVar
		{
			// Token: 0x06001941 RID: 6465 RVA: 0x0000972B File Offset: 0x0000792B
			private FpsMaxConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001942 RID: 6466 RVA: 0x0006D0D0 File Offset: 0x0006B2D0
			public override void SetString(string newValue)
			{
				int targetFrameRate;
				if (TextSerialization.TryParseInvariant(newValue, out targetFrameRate))
				{
					Application.targetFrameRate = targetFrameRate;
				}
			}

			// Token: 0x06001943 RID: 6467 RVA: 0x0006D0ED File Offset: 0x0006B2ED
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Application.targetFrameRate);
			}

			// Token: 0x04001793 RID: 6035
			private static SettingsConVars.FpsMaxConVar instance = new SettingsConVars.FpsMaxConVar("fps_max", ConVarFlags.Archive, "60", "Maximum FPS. -1 is unlimited.");
		}

		// Token: 0x02000419 RID: 1049
		private class ShadowsConVar : BaseConVar
		{
			// Token: 0x06001945 RID: 6469 RVA: 0x0000972B File Offset: 0x0000792B
			private ShadowsConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001946 RID: 6470 RVA: 0x0006D118 File Offset: 0x0006B318
			public override void SetString(string newValue)
			{
				try
				{
					QualitySettings.shadows = (ShadowQuality)Enum.Parse(typeof(ShadowQuality), newValue, true);
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x06001947 RID: 6471 RVA: 0x0006D160 File Offset: 0x0006B360
			public override string GetString()
			{
				return QualitySettings.shadows.ToString();
			}

			// Token: 0x04001794 RID: 6036
			private static SettingsConVars.ShadowsConVar instance = new SettingsConVars.ShadowsConVar("r_shadows", ConVarFlags.Archive | ConVarFlags.Engine, null, "Shadow quality. Can be \"All\" \"HardOnly\" or \"Disable\"");
		}

		// Token: 0x0200041A RID: 1050
		private class SoftParticlesConVar : BaseConVar
		{
			// Token: 0x06001949 RID: 6473 RVA: 0x0000972B File Offset: 0x0000792B
			private SoftParticlesConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600194A RID: 6474 RVA: 0x0006D19C File Offset: 0x0006B39C
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					QualitySettings.softParticles = (num != 0);
				}
			}

			// Token: 0x0600194B RID: 6475 RVA: 0x0006D1BC File Offset: 0x0006B3BC
			public override string GetString()
			{
				if (!QualitySettings.softParticles)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04001795 RID: 6037
			private static SettingsConVars.SoftParticlesConVar instance = new SettingsConVars.SoftParticlesConVar("r_softparticles", ConVarFlags.Archive | ConVarFlags.Engine, null, "Whether or not soft particles are enabled.");
		}

		// Token: 0x0200041B RID: 1051
		private class FoliageWindConVar : BaseConVar
		{
			// Token: 0x0600194D RID: 6477 RVA: 0x0000972B File Offset: 0x0000792B
			private FoliageWindConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600194E RID: 6478 RVA: 0x0006D1EC File Offset: 0x0006B3EC
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					if (num >= 1)
					{
						Shader.EnableKeyword("ENABLE_WIND");
						return;
					}
					Shader.DisableKeyword("ENABLE_WIND");
				}
			}

			// Token: 0x0600194F RID: 6479 RVA: 0x0006D21C File Offset: 0x0006B41C
			public override string GetString()
			{
				if (!Shader.IsKeywordEnabled("ENABLE_WIND"))
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04001796 RID: 6038
			private static SettingsConVars.FoliageWindConVar instance = new SettingsConVars.FoliageWindConVar("r_foliagewind", ConVarFlags.Archive, "1", "Whether or not foliage has wind.");
		}

		// Token: 0x0200041C RID: 1052
		private class LodBiasConVar : BaseConVar
		{
			// Token: 0x06001951 RID: 6481 RVA: 0x0000972B File Offset: 0x0000792B
			private LodBiasConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001952 RID: 6482 RVA: 0x0006D254 File Offset: 0x0006B454
			public override void SetString(string newValue)
			{
				float lodBias;
				if (TextSerialization.TryParseInvariant(newValue, out lodBias))
				{
					QualitySettings.lodBias = lodBias;
				}
			}

			// Token: 0x06001953 RID: 6483 RVA: 0x0006D271 File Offset: 0x0006B471
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.lodBias);
			}

			// Token: 0x04001797 RID: 6039
			private static SettingsConVars.LodBiasConVar instance = new SettingsConVars.LodBiasConVar("r_lod_bias", ConVarFlags.Archive | ConVarFlags.Engine, null, "LOD bias.");
		}

		// Token: 0x0200041D RID: 1053
		private class MaximumLodConVar : BaseConVar
		{
			// Token: 0x06001955 RID: 6485 RVA: 0x0000972B File Offset: 0x0000792B
			private MaximumLodConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001956 RID: 6486 RVA: 0x0006D298 File Offset: 0x0006B498
			public override void SetString(string newValue)
			{
				int maximumLODLevel;
				if (TextSerialization.TryParseInvariant(newValue, out maximumLODLevel))
				{
					QualitySettings.maximumLODLevel = maximumLODLevel;
				}
			}

			// Token: 0x06001957 RID: 6487 RVA: 0x0006D2B5 File Offset: 0x0006B4B5
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.maximumLODLevel);
			}

			// Token: 0x04001798 RID: 6040
			private static SettingsConVars.MaximumLodConVar instance = new SettingsConVars.MaximumLodConVar("r_lod_max", ConVarFlags.Archive | ConVarFlags.Engine, null, "The maximum allowed LOD level.");
		}

		// Token: 0x0200041E RID: 1054
		private class MasterTextureLimitConVar : BaseConVar
		{
			// Token: 0x06001959 RID: 6489 RVA: 0x0000972B File Offset: 0x0000792B
			private MasterTextureLimitConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600195A RID: 6490 RVA: 0x0006D2DC File Offset: 0x0006B4DC
			public override void SetString(string newValue)
			{
				int masterTextureLimit;
				if (TextSerialization.TryParseInvariant(newValue, out masterTextureLimit))
				{
					QualitySettings.masterTextureLimit = masterTextureLimit;
				}
			}

			// Token: 0x0600195B RID: 6491 RVA: 0x0006D2F9 File Offset: 0x0006B4F9
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.masterTextureLimit);
			}

			// Token: 0x04001799 RID: 6041
			private static SettingsConVars.MasterTextureLimitConVar instance = new SettingsConVars.MasterTextureLimitConVar("master_texture_limit", ConVarFlags.Archive | ConVarFlags.Engine, null, "Reduction in texture quality. 0 is highest quality textures, 1 is half, 2 is quarter, etc.");
		}

		// Token: 0x0200041F RID: 1055
		private class AnisotropicFilteringConVar : BaseConVar
		{
			// Token: 0x0600195D RID: 6493 RVA: 0x0000972B File Offset: 0x0000792B
			private AnisotropicFilteringConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600195E RID: 6494 RVA: 0x0006D320 File Offset: 0x0006B520
			public override void SetString(string newValue)
			{
				try
				{
					QualitySettings.anisotropicFiltering = (AnisotropicFiltering)Enum.Parse(typeof(AnisotropicFiltering), newValue, true);
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x0600195F RID: 6495 RVA: 0x0006D368 File Offset: 0x0006B568
			public override string GetString()
			{
				return QualitySettings.anisotropicFiltering.ToString();
			}

			// Token: 0x0400179A RID: 6042
			private static SettingsConVars.AnisotropicFilteringConVar instance = new SettingsConVars.AnisotropicFilteringConVar("anisotropic_filtering", ConVarFlags.Archive, "Disable", "The anisotropic filtering mode. Can be \"Disable\", \"Enable\" or \"ForceEnable\".");
		}

		// Token: 0x02000420 RID: 1056
		private class ShadowResolutionConVar : BaseConVar
		{
			// Token: 0x06001961 RID: 6497 RVA: 0x0000972B File Offset: 0x0000792B
			private ShadowResolutionConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001962 RID: 6498 RVA: 0x0006D3A4 File Offset: 0x0006B5A4
			public override void SetString(string newValue)
			{
				try
				{
					QualitySettings.shadowResolution = (ShadowResolution)Enum.Parse(typeof(ShadowResolution), newValue, true);
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x06001963 RID: 6499 RVA: 0x0006D3EC File Offset: 0x0006B5EC
			public override string GetString()
			{
				return QualitySettings.shadowResolution.ToString();
			}

			// Token: 0x0400179B RID: 6043
			private static SettingsConVars.ShadowResolutionConVar instance = new SettingsConVars.ShadowResolutionConVar("shadow_resolution", ConVarFlags.Archive | ConVarFlags.Engine, "Medium", "Default shadow resolution. Can be \"Low\", \"Medium\", \"High\" or \"VeryHigh\".");
		}

		// Token: 0x02000421 RID: 1057
		private class ShadowCascadesConVar : BaseConVar
		{
			// Token: 0x06001965 RID: 6501 RVA: 0x0000972B File Offset: 0x0000792B
			private ShadowCascadesConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001966 RID: 6502 RVA: 0x0006D42C File Offset: 0x0006B62C
			public override void SetString(string newValue)
			{
				int shadowCascades;
				if (TextSerialization.TryParseInvariant(newValue, out shadowCascades))
				{
					QualitySettings.shadowCascades = shadowCascades;
				}
			}

			// Token: 0x06001967 RID: 6503 RVA: 0x0006D449 File Offset: 0x0006B649
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.shadowCascades);
			}

			// Token: 0x0400179C RID: 6044
			private static SettingsConVars.ShadowCascadesConVar instance = new SettingsConVars.ShadowCascadesConVar("shadow_cascades", ConVarFlags.Archive | ConVarFlags.Engine, null, "The number of cascades to use for directional light shadows. low=0 high=4");
		}

		// Token: 0x02000422 RID: 1058
		private class ShadowDistanceConvar : BaseConVar
		{
			// Token: 0x06001969 RID: 6505 RVA: 0x0000972B File Offset: 0x0000792B
			private ShadowDistanceConvar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600196A RID: 6506 RVA: 0x0006D470 File Offset: 0x0006B670
			public override void SetString(string newValue)
			{
				float shadowDistance;
				if (TextSerialization.TryParseInvariant(newValue, out shadowDistance))
				{
					QualitySettings.shadowDistance = shadowDistance;
				}
			}

			// Token: 0x0600196B RID: 6507 RVA: 0x0006D48D File Offset: 0x0006B68D
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(QualitySettings.shadowDistance);
			}

			// Token: 0x0400179D RID: 6045
			private static SettingsConVars.ShadowDistanceConvar instance = new SettingsConVars.ShadowDistanceConvar("shadow_distance", ConVarFlags.Archive, "200", "The distance in meters to draw shadows.");
		}

		// Token: 0x02000423 RID: 1059
		private class PpMotionBlurConVar : BaseConVar
		{
			// Token: 0x0600196D RID: 6509 RVA: 0x0006D4B5 File Offset: 0x0006B6B5
			private PpMotionBlurConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<MotionBlur>(out SettingsConVars.PpMotionBlurConVar.settings);
			}

			// Token: 0x0600196E RID: 6510 RVA: 0x0006D4DC File Offset: 0x0006B6DC
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpMotionBlurConVar.settings)
				{
					SettingsConVars.PpMotionBlurConVar.settings.active = (num == 0);
				}
			}

			// Token: 0x0600196F RID: 6511 RVA: 0x0006D50D File Offset: 0x0006B70D
			public override string GetString()
			{
				if (!SettingsConVars.PpMotionBlurConVar.settings)
				{
					return "1";
				}
				if (!SettingsConVars.PpMotionBlurConVar.settings.active)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x0400179E RID: 6046
			private static MotionBlur settings;

			// Token: 0x0400179F RID: 6047
			private static SettingsConVars.PpMotionBlurConVar instance = new SettingsConVars.PpMotionBlurConVar("pp_motionblur", ConVarFlags.Archive, "0", "Motion blur. 0 = disabled 1 = enabled");
		}

		// Token: 0x02000424 RID: 1060
		private class PpSobelOutlineConVar : BaseConVar
		{
			// Token: 0x06001971 RID: 6513 RVA: 0x0006D554 File Offset: 0x0006B754
			private PpSobelOutlineConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<SobelOutline>(out SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings);
			}

			// Token: 0x06001972 RID: 6514 RVA: 0x0006D57C File Offset: 0x0006B77C
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings)
				{
					SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings.active = (num == 0);
				}
			}

			// Token: 0x06001973 RID: 6515 RVA: 0x0006D5AD File Offset: 0x0006B7AD
			public override string GetString()
			{
				if (!SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings)
				{
					return "1";
				}
				if (!SettingsConVars.PpSobelOutlineConVar.sobelOutlineSettings.active)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x040017A0 RID: 6048
			private static SobelOutline sobelOutlineSettings;

			// Token: 0x040017A1 RID: 6049
			private static SettingsConVars.PpSobelOutlineConVar instance = new SettingsConVars.PpSobelOutlineConVar("pp_sobel_outline", ConVarFlags.Archive, "1", "Whether or not to use the sobel rim light effect.");
		}

		// Token: 0x02000425 RID: 1061
		private class PpBloomConVar : BaseConVar
		{
			// Token: 0x06001975 RID: 6517 RVA: 0x0006D5F4 File Offset: 0x0006B7F4
			private PpBloomConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				bool flag = RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<Bloom>(out SettingsConVars.PpBloomConVar.settings);
				Debug.LogFormat("Bloom: {0}", new object[]
				{
					flag
				});
			}

			// Token: 0x06001976 RID: 6518 RVA: 0x0006D640 File Offset: 0x0006B840
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpBloomConVar.settings)
				{
					SettingsConVars.PpBloomConVar.settings.active = (num == 0);
				}
			}

			// Token: 0x06001977 RID: 6519 RVA: 0x0006D671 File Offset: 0x0006B871
			public override string GetString()
			{
				if (!SettingsConVars.PpBloomConVar.settings)
				{
					return "1";
				}
				if (!SettingsConVars.PpBloomConVar.settings.active)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x040017A2 RID: 6050
			private static Bloom settings;

			// Token: 0x040017A3 RID: 6051
			private static SettingsConVars.PpBloomConVar instance = new SettingsConVars.PpBloomConVar("pp_bloom", ConVarFlags.Archive | ConVarFlags.Engine, null, "Bloom postprocessing. 0 = disabled 1 = enabled");
		}

		// Token: 0x02000426 RID: 1062
		private class PpAOConVar : BaseConVar
		{
			// Token: 0x06001979 RID: 6521 RVA: 0x0006D6B5 File Offset: 0x0006B8B5
			private PpAOConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<AmbientOcclusion>(out SettingsConVars.PpAOConVar.settings);
			}

			// Token: 0x0600197A RID: 6522 RVA: 0x0006D6DC File Offset: 0x0006B8DC
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num) && SettingsConVars.PpAOConVar.settings)
				{
					SettingsConVars.PpAOConVar.settings.active = (num == 0);
				}
			}

			// Token: 0x0600197B RID: 6523 RVA: 0x0006D70D File Offset: 0x0006B90D
			public override string GetString()
			{
				if (!SettingsConVars.PpAOConVar.settings)
				{
					return "1";
				}
				if (!SettingsConVars.PpAOConVar.settings.active)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x040017A4 RID: 6052
			private static AmbientOcclusion settings;

			// Token: 0x040017A5 RID: 6053
			private static SettingsConVars.PpAOConVar instance = new SettingsConVars.PpAOConVar("pp_ao", ConVarFlags.Archive | ConVarFlags.Engine, null, "SSAO postprocessing. 0 = disabled 1 = enabled");
		}

		// Token: 0x02000427 RID: 1063
		private class PpGammaConVar : BaseConVar
		{
			// Token: 0x0600197D RID: 6525 RVA: 0x0006D751 File Offset: 0x0006B951
			private PpGammaConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
				RoR2Application.instance.postProcessSettingsController.sharedProfile.TryGetSettings<ColorGrading>(out SettingsConVars.PpGammaConVar.colorGradingSettings);
			}

			// Token: 0x0600197E RID: 6526 RVA: 0x0006D778 File Offset: 0x0006B978
			public override void SetString(string newValue)
			{
				float w;
				if (TextSerialization.TryParseInvariant(newValue, out w) && SettingsConVars.PpGammaConVar.colorGradingSettings)
				{
					SettingsConVars.PpGammaConVar.colorGradingSettings.gamma.value.w = w;
				}
			}

			// Token: 0x0600197F RID: 6527 RVA: 0x0006D7B0 File Offset: 0x0006B9B0
			public override string GetString()
			{
				if (!SettingsConVars.PpGammaConVar.colorGradingSettings)
				{
					return "0";
				}
				return TextSerialization.ToStringInvariant(SettingsConVars.PpGammaConVar.colorGradingSettings.gamma.value.w);
			}

			// Token: 0x040017A6 RID: 6054
			private static ColorGrading colorGradingSettings;

			// Token: 0x040017A7 RID: 6055
			private static SettingsConVars.PpGammaConVar instance = new SettingsConVars.PpGammaConVar("gamma", ConVarFlags.Archive, "0", "Gamma boost, from -inf to inf.");
		}
	}
}
