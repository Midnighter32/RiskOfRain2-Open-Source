using System;
using System.Reflection;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000153 RID: 339
	[RequireComponent(typeof(AkGameObj))]
	public class AudioManager : MonoBehaviour
	{
		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x00019869 File Offset: 0x00017A69
		// (set) Token: 0x06000619 RID: 1561 RVA: 0x00019870 File Offset: 0x00017A70
		public static AudioManager instance { get; private set; }

		// Token: 0x0600061A RID: 1562 RVA: 0x00019878 File Offset: 0x00017A78
		private void Awake()
		{
			AudioManager.instance = this;
			this.akGameObj = base.GetComponent<AkGameObj>();
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0001988C File Offset: 0x00017A8C
		static AudioManager()
		{
			RoR2Application.onPauseStartGlobal = (Action)Delegate.Combine(RoR2Application.onPauseStartGlobal, new Action(delegate()
			{
				AkSoundEngine.PostEvent("Pause_All", null);
			}));
			RoR2Application.onPauseEndGlobal = (Action)Delegate.Combine(RoR2Application.onPauseEndGlobal, new Action(delegate()
			{
				AkSoundEngine.PostEvent("Unpause_All", null);
			}));
		}

		// Token: 0x04000698 RID: 1688
		private AkGameObj akGameObj;

		// Token: 0x0400069A RID: 1690
		private static AudioManager.VolumeConVar cvVolumeMaster = new AudioManager.VolumeConVar("volume_master", ConVarFlags.Archive | ConVarFlags.Engine, "100", "The master volume of the game audio, from 0 to 100.", "Volume_Master");

		// Token: 0x0400069B RID: 1691
		private static AudioManager.VolumeConVar cvVolumeSfx = new AudioManager.VolumeConVar("volume_sfx", ConVarFlags.Archive | ConVarFlags.Engine, "100", "The volume of sound effects, from 0 to 100.", "Volume_SFX");

		// Token: 0x0400069C RID: 1692
		private static AudioManager.VolumeConVar cvVolumeMsx = new AudioManager.VolumeConVar("volume_msx", ConVarFlags.Archive | ConVarFlags.Engine, "100", "The music volume, from 0 to 100.", "Volume_MSX");

		// Token: 0x0400069D RID: 1693
		private static readonly FieldInfo akInitializerMsInstanceField = typeof(AkInitializer).GetField("ms_Instance", BindingFlags.Static | BindingFlags.NonPublic);

		// Token: 0x02000154 RID: 340
		private class VolumeConVar : BaseConVar
		{
			// Token: 0x0600061D RID: 1565 RVA: 0x0001995C File Offset: 0x00017B5C
			public VolumeConVar(string name, ConVarFlags flags, string defaultValue, string helpText, string rtpcName) : base(name, flags, defaultValue, helpText)
			{
				this.rtpcName = rtpcName;
			}

			// Token: 0x0600061E RID: 1566 RVA: 0x00019974 File Offset: 0x00017B74
			public override void SetString(string newValue)
			{
				float value;
				if (AkSoundEngine.IsInitialized() && TextSerialization.TryParseInvariant(newValue, out value))
				{
					AkSoundEngine.SetRTPCValue(this.rtpcName, Mathf.Clamp(value, 0f, 100f));
				}
			}

			// Token: 0x0600061F RID: 1567 RVA: 0x000199B0 File Offset: 0x00017BB0
			public override string GetString()
			{
				int num = 1;
				float value;
				AKRESULT rtpcvalue = AkSoundEngine.GetRTPCValue(this.rtpcName, null, 0U, out value, ref num);
				if (rtpcvalue == AKRESULT.AK_Success)
				{
					return TextSerialization.ToStringInvariant(value);
				}
				return "ERROR: " + rtpcvalue;
			}

			// Token: 0x0400069E RID: 1694
			private readonly string rtpcName;
		}

		// Token: 0x02000155 RID: 341
		private class AudioFocusedOnlyConVar : BaseConVar
		{
			// Token: 0x06000620 RID: 1568 RVA: 0x0000972B File Offset: 0x0000792B
			public AudioFocusedOnlyConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000621 RID: 1569 RVA: 0x000199EC File Offset: 0x00017BEC
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					AkSoundEngineController.s_MuteOnFocusLost = (num != 0);
				}
			}

			// Token: 0x06000622 RID: 1570 RVA: 0x00019A0C File Offset: 0x00017C0C
			public override string GetString()
			{
				if (!AkSoundEngineController.s_MuteOnFocusLost)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x0400069F RID: 1695
			private static AudioManager.AudioFocusedOnlyConVar instance = new AudioManager.AudioFocusedOnlyConVar("audio_focused_only", ConVarFlags.Archive | ConVarFlags.Engine, null, "Whether or not audio should mute when focus is lost.");
		}

		// Token: 0x02000156 RID: 342
		private class WwiseLogEnabledConVar : BaseConVar
		{
			// Token: 0x06000624 RID: 1572 RVA: 0x0000972B File Offset: 0x0000792B
			private WwiseLogEnabledConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000625 RID: 1573 RVA: 0x00019A3C File Offset: 0x00017C3C
			public override void SetString(string newValue)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					AkInitializer akInitializer = AudioManager.akInitializerMsInstanceField.GetValue(null) as AkInitializer;
					if (akInitializer)
					{
						AkWwiseInitializationSettings initializationSettings = akInitializer.InitializationSettings;
						AkCallbackManager.InitializationSettings initializationSettings2 = (initializationSettings != null) ? initializationSettings.CallbackManagerInitializationSettings : null;
						if (initializationSettings2 != null)
						{
							initializationSettings2.IsLoggingEnabled = (num != 0);
							return;
						}
						Debug.Log("Cannot set value. callbackManagerInitializationSettings is null.");
					}
				}
			}

			// Token: 0x06000626 RID: 1574 RVA: 0x00019A98 File Offset: 0x00017C98
			public override string GetString()
			{
				AkInitializer akInitializer = AudioManager.akInitializerMsInstanceField.GetValue(null) as AkInitializer;
				if (akInitializer)
				{
					AkWwiseInitializationSettings initializationSettings = akInitializer.InitializationSettings;
					if (((initializationSettings != null) ? initializationSettings.CallbackManagerInitializationSettings : null) != null)
					{
						if (!akInitializer.InitializationSettings.CallbackManagerInitializationSettings.IsLoggingEnabled)
						{
							return "0";
						}
						return "1";
					}
					else
					{
						Debug.Log("Cannot retrieve value. callbackManagerInitializationSettings is null.");
					}
				}
				return "1";
			}

			// Token: 0x040006A0 RID: 1696
			private static AudioManager.WwiseLogEnabledConVar instance = new AudioManager.WwiseLogEnabledConVar("wwise_log_enabled", ConVarFlags.Archive | ConVarFlags.Engine, null, "Wwise logging. 0 = disabled 1 = enabled");
		}
	}
}
