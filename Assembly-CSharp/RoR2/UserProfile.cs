using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Facepunch.Steamworks;
using JetBrains.Annotations;
using Rewired;
using RoR2.Stats;
using UnityEngine;
using Zio;
using Zio.FileSystems;

namespace RoR2
{
	// Token: 0x02000464 RID: 1124
	public class UserProfile
	{
		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06001B34 RID: 6964 RVA: 0x00074046 File Offset: 0x00072246
		// (set) Token: 0x06001B35 RID: 6965 RVA: 0x0007404E File Offset: 0x0007224E
		public bool isCorrupted { get; private set; }

		// Token: 0x06001B36 RID: 6966 RVA: 0x00074058 File Offset: 0x00072258
		public bool HasUnlockable(string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			return unlockableDef == null || this.HasUnlockable(unlockableDef);
		}

		// Token: 0x06001B37 RID: 6967 RVA: 0x00074078 File Offset: 0x00072278
		public bool HasUnlockable([NotNull] UnlockableDef unlockableDef)
		{
			return this.statSheet.HasUnlockable(unlockableDef);
		}

		// Token: 0x06001B38 RID: 6968 RVA: 0x00074088 File Offset: 0x00072288
		public void AddUnlockToken(string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			if (unlockableDef != null)
			{
				this.GrantUnlockable(unlockableDef);
			}
		}

		// Token: 0x06001B39 RID: 6969 RVA: 0x000740A8 File Offset: 0x000722A8
		public void GrantUnlockable(UnlockableDef unlockableDef)
		{
			if (!this.statSheet.HasUnlockable(unlockableDef))
			{
				this.statSheet.AddUnlockable(unlockableDef);
				Debug.LogFormat("{0} unlocked {1}", new object[]
				{
					this.name,
					unlockableDef.nameToken
				});
				this.RequestSave(false);
				Action<UserProfile, string> action = UserProfile.onUnlockableGranted;
				if (action == null)
				{
					return;
				}
				action(this, unlockableDef.name);
			}
		}

		// Token: 0x06001B3A RID: 6970 RVA: 0x0007410E File Offset: 0x0007230E
		public void RevokeUnlockable(UnlockableDef unlockableDef)
		{
			if (this.statSheet.HasUnlockable(unlockableDef))
			{
				this.statSheet.RemoveUnlockable(unlockableDef.index);
			}
		}

		// Token: 0x06001B3B RID: 6971 RVA: 0x00074130 File Offset: 0x00072330
		public bool HasSurvivorUnlocked(SurvivorIndex survivorIndex)
		{
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(survivorIndex);
			return survivorDef != null && (survivorDef.unlockableName == "" || this.HasUnlockable(survivorDef.unlockableName));
		}

		// Token: 0x06001B3C RID: 6972 RVA: 0x00074169 File Offset: 0x00072369
		public bool HasDiscoveredPickup(PickupIndex pickupIndex)
		{
			return pickupIndex.isValid && this.discoveredPickups[pickupIndex.value];
		}

		// Token: 0x06001B3D RID: 6973 RVA: 0x00074184 File Offset: 0x00072384
		public void DiscoverPickup(PickupIndex pickupIndex)
		{
			if (!pickupIndex.isValid)
			{
				return;
			}
			bool[] array = this.discoveredPickups;
			int value = pickupIndex.value;
			bool flag = !array[value];
			array[value] = true;
			Action<PickupIndex> action = this.onPickupDiscovered;
			if (action != null)
			{
				action(pickupIndex);
			}
			if (flag)
			{
				this.RequestSave(false);
			}
		}

		// Token: 0x14000066 RID: 102
		// (add) Token: 0x06001B3E RID: 6974 RVA: 0x000741D0 File Offset: 0x000723D0
		// (remove) Token: 0x06001B3F RID: 6975 RVA: 0x00074208 File Offset: 0x00072408
		public event Action<PickupIndex> onPickupDiscovered;

		// Token: 0x06001B40 RID: 6976 RVA: 0x0007423D File Offset: 0x0007243D
		public bool HasAchievement(string achievementName)
		{
			return this.achievementsList.Contains(achievementName);
		}

		// Token: 0x06001B41 RID: 6977 RVA: 0x0007424C File Offset: 0x0007244C
		public bool CanSeeAchievement(string achievementName)
		{
			if (this.HasAchievement(achievementName))
			{
				return true;
			}
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(achievementName);
			return achievementDef != null && (string.IsNullOrEmpty(achievementDef.prerequisiteAchievementIdentifier) || this.HasAchievement(achievementDef.prerequisiteAchievementIdentifier));
		}

		// Token: 0x06001B42 RID: 6978 RVA: 0x0007428B File Offset: 0x0007248B
		public void AddAchievement(string achievementName, bool isExternal)
		{
			this.achievementsList.Add(achievementName);
			this.unviewedAchievementsList.Add(achievementName);
			if (isExternal)
			{
				Client.Instance.Achievements.Trigger(achievementName, true);
			}
			this.RequestSave(false);
		}

		// Token: 0x06001B43 RID: 6979 RVA: 0x000742C1 File Offset: 0x000724C1
		public void RevokeAchievement(string achievementName)
		{
			this.achievementsList.Remove(achievementName);
			this.unviewedAchievementsList.Remove(achievementName);
			this.RequestSave(false);
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06001B44 RID: 6980 RVA: 0x000742E4 File Offset: 0x000724E4
		public bool hasUnviewedAchievement
		{
			get
			{
				return this.unviewedAchievementsList.Count > 0;
			}
		}

		// Token: 0x06001B45 RID: 6981 RVA: 0x000742F4 File Offset: 0x000724F4
		public string PopNextUnviewedAchievementName()
		{
			if (this.unviewedAchievementsList.Count == 0)
			{
				return null;
			}
			string result = this.unviewedAchievementsList[0];
			this.unviewedAchievementsList.RemoveAt(0);
			return result;
		}

		// Token: 0x06001B46 RID: 6982 RVA: 0x00074320 File Offset: 0x00072520
		private static void GenerateSaveFieldFunctions()
		{
			UserProfile.nameToSaveFieldMap.Clear();
			foreach (FieldInfo fieldInfo in typeof(UserProfile).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				UserProfile.SaveFieldAttribute customAttribute = fieldInfo.GetCustomAttribute<UserProfile.SaveFieldAttribute>();
				if (customAttribute != null)
				{
					customAttribute.Setup(fieldInfo);
					UserProfile.nameToSaveFieldMap[fieldInfo.Name] = customAttribute;
				}
			}
			UserProfile.saveFieldNames = UserProfile.nameToSaveFieldMap.Keys.ToArray<string>();
			Array.Sort<string>(UserProfile.saveFieldNames);
			UserProfile.saveFields = (from name in UserProfile.saveFieldNames
			select UserProfile.nameToSaveFieldMap[name]).ToArray<UserProfile.SaveFieldAttribute>();
		}

		// Token: 0x06001B47 RID: 6983 RVA: 0x000743D0 File Offset: 0x000725D0
		public void SetSaveFieldString([NotNull] string fieldName, [NotNull] string value)
		{
			UserProfile.SaveFieldAttribute saveFieldAttribute;
			if (UserProfile.nameToSaveFieldMap.TryGetValue(fieldName, out saveFieldAttribute))
			{
				saveFieldAttribute.setter(this, value);
				return;
			}
			Debug.LogErrorFormat("Save field {0} is not defined.", new object[]
			{
				fieldName
			});
		}

		// Token: 0x06001B48 RID: 6984 RVA: 0x00074410 File Offset: 0x00072610
		public string GetSaveFieldString([NotNull] string fieldName)
		{
			UserProfile.SaveFieldAttribute saveFieldAttribute;
			if (UserProfile.nameToSaveFieldMap.TryGetValue(fieldName, out saveFieldAttribute))
			{
				return saveFieldAttribute.getter(this);
			}
			Debug.LogErrorFormat("Save field {0} is not defined.", new object[]
			{
				fieldName
			});
			return string.Empty;
		}

		// Token: 0x06001B49 RID: 6985 RVA: 0x00074454 File Offset: 0x00072654
		public void ApplyDeltaStatSheet(StatSheet deltaStatSheet)
		{
			int i = 0;
			int unlockableCount = deltaStatSheet.GetUnlockableCount();
			while (i < unlockableCount)
			{
				this.GrantUnlockable(deltaStatSheet.GetUnlockable(i));
				i++;
			}
			this.statSheet.ApplyDelta(deltaStatSheet);
			Action action = this.onStatsReceived;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x14000067 RID: 103
		// (add) Token: 0x06001B4A RID: 6986 RVA: 0x000744A0 File Offset: 0x000726A0
		// (remove) Token: 0x06001B4B RID: 6987 RVA: 0x000744D8 File Offset: 0x000726D8
		public event Action onStatsReceived;

		// Token: 0x06001B4C RID: 6988 RVA: 0x0007450D File Offset: 0x0007270D
		private void ResetShouldShowTutorial(ref UserProfile.TutorialProgression tutorialProgression)
		{
			tutorialProgression.shouldShow = (tutorialProgression.showCount < 3U);
		}

		// Token: 0x06001B4D RID: 6989 RVA: 0x0007451E File Offset: 0x0007271E
		private void RebuildTutorialProgressions()
		{
			this.ResetShouldShowTutorial(ref this.tutorialDifficulty);
			this.ResetShouldShowTutorial(ref this.tutorialSprint);
			this.ResetShouldShowTutorial(ref this.tutorialEquipment);
		}

		// Token: 0x06001B4E RID: 6990 RVA: 0x00074544 File Offset: 0x00072744
		private void OnLoadoutChanged()
		{
			Action action = this.onLoadoutChanged;
			if (action != null)
			{
				action();
			}
			Action<UserProfile> action2 = UserProfile.onLoadoutChangedGlobal;
			if (action2 != null)
			{
				action2(this);
			}
			this.RequestSave(false);
		}

		// Token: 0x06001B4F RID: 6991 RVA: 0x0007456F File Offset: 0x0007276F
		public void CopyLoadout(Loadout dest)
		{
			this.loadout.Copy(dest);
		}

		// Token: 0x06001B50 RID: 6992 RVA: 0x0007457D File Offset: 0x0007277D
		public void SetLoadout(Loadout newLoadout)
		{
			if (this.loadout.ValueEquals(newLoadout))
			{
				return;
			}
			newLoadout.Copy(this.loadout);
			this.OnLoadoutChanged();
		}

		// Token: 0x14000068 RID: 104
		// (add) Token: 0x06001B51 RID: 6993 RVA: 0x000745A0 File Offset: 0x000727A0
		// (remove) Token: 0x06001B52 RID: 6994 RVA: 0x000745D8 File Offset: 0x000727D8
		public event Action onLoadoutChanged;

		// Token: 0x14000069 RID: 105
		// (add) Token: 0x06001B53 RID: 6995 RVA: 0x00074610 File Offset: 0x00072810
		// (remove) Token: 0x06001B54 RID: 6996 RVA: 0x00074644 File Offset: 0x00072844
		public static event Action<UserProfile> onLoadoutChangedGlobal;

		// Token: 0x06001B55 RID: 6997 RVA: 0x00074678 File Offset: 0x00072878
		[ConCommand(commandName = "loadout_set_skill_variant", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "loadout_set_skill_variant [body_name] [skill_slot_index] [skill_variant_index]\nSets the skill variant for the sender's user profile.")]
		private static void CCLoadoutSetSkillVariant(ConCommandArgs args)
		{
			int bodyIndex = args.GetBodyIndex(0);
			int argInt = args.GetArgInt(1);
			int argInt2 = args.GetArgInt(2);
			UserProfile userProfile = args.GetSenderLocalUser().userProfile;
			Loadout loadout = new Loadout();
			userProfile.loadout.Copy(loadout);
			loadout.bodyLoadoutManager.SetSkillVariant(bodyIndex, argInt, (uint)argInt2);
			userProfile.SetLoadout(loadout);
			if (args.senderMaster)
			{
				args.senderMaster.SetLoadoutServer(loadout);
			}
			if (args.senderBody)
			{
				args.senderBody.SetLoadoutServer(loadout);
			}
		}

		// Token: 0x06001B56 RID: 6998 RVA: 0x00074707 File Offset: 0x00072907
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			UserProfile.GenerateSaveFieldFunctions();
			RoR2Application.onUpdate += UserProfile.StaticUpdate;
		}

		// Token: 0x06001B57 RID: 6999 RVA: 0x00074720 File Offset: 0x00072920
		private static void StaticUpdate()
		{
			UserProfile.secondAccumulator += Time.unscaledDeltaTime;
			if (UserProfile.secondAccumulator > 1f)
			{
				UserProfile.secondAccumulator -= 1f;
				foreach (UserProfile userProfile in UserProfile.loggedInProfiles)
				{
					userProfile.totalLoginSeconds += 1U;
				}
			}
			foreach (UserProfile userProfile2 in UserProfile.loggedInProfiles)
			{
				if (userProfile2.saveRequestPending && userProfile2.Save(false))
				{
					userProfile2.saveRequestPending = false;
				}
			}
			UserProfile.SaveHelper.ProcessFileOutputQueue();
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06001B58 RID: 7000 RVA: 0x000747FC File Offset: 0x000729FC
		// (set) Token: 0x06001B59 RID: 7001 RVA: 0x00074804 File Offset: 0x00072A04
		public bool loggedIn { get; private set; }

		// Token: 0x06001B5A RID: 7002 RVA: 0x00074810 File Offset: 0x00072A10
		public void OnLogin()
		{
			if (this.loggedIn)
			{
				Debug.LogErrorFormat("Profile {0} is already logged in!", new object[]
				{
					this.fileName
				});
				return;
			}
			this.loggedIn = true;
			UserProfile.loggedInProfiles.Add(this);
			this.RebuildTutorialProgressions();
			foreach (string identifier in this.achievementsList)
			{
				Client.Instance.Achievements.Trigger(identifier, true);
			}
			this.loadout.EnforceUnlockables(this);
		}

		// Token: 0x06001B5B RID: 7003 RVA: 0x000748B4 File Offset: 0x00072AB4
		public void OnLogout()
		{
			if (!this.loggedIn)
			{
				Debug.LogErrorFormat("Profile {0} is already logged out!", new object[]
				{
					this.fileName
				});
				return;
			}
			UserProfile.loggedInProfiles.Remove(this);
			this.loggedIn = false;
			this.RequestSave(true);
		}

		// Token: 0x06001B5C RID: 7004 RVA: 0x000748F4 File Offset: 0x00072AF4
		public static void HandleShutDown()
		{
			foreach (UserProfile userProfile in UserProfile.loggedInProfiles)
			{
				userProfile.RequestSave(true);
			}
		}

		// Token: 0x06001B5D RID: 7005 RVA: 0x00074944 File Offset: 0x00072B44
		private static void OutputBadFileResults()
		{
			if (UserProfile.badFileResults.Count == 0)
			{
				return;
			}
			try
			{
				using (Stream stream = RoR2Application.fileSystem.CreateFile(new UPath("/bad_profiles.log")))
				{
					using (TextWriter textWriter = new StreamWriter(stream))
					{
						foreach (UserProfile.LoadUserProfileOperationResult loadUserProfileOperationResult in UserProfile.badFileResults)
						{
							textWriter.WriteLine("Failed to load file \"{0}\" ({1}B)", loadUserProfileOperationResult.fileName, loadUserProfileOperationResult.fileLength);
							textWriter.WriteLine("Exception: {0}", loadUserProfileOperationResult.exception);
							textWriter.Write("Base64 Contents: ");
							textWriter.WriteLine(loadUserProfileOperationResult.failureContents ?? string.Empty);
							textWriter.WriteLine(string.Empty);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogFormat("Could not write bad UserProfile load log! Reason: {0}", new object[]
				{
					ex.Message
				});
			}
		}

		// Token: 0x06001B5E RID: 7006 RVA: 0x00074A70 File Offset: 0x00072C70
		public static void LoadUserProfiles()
		{
			UserProfile.badFileResults.Clear();
			UserProfile.loadedUserProfiles.Clear();
			UserProfile.LoadDefaultProfile();
			FileSystem cloudStorage = RoR2Application.cloudStorage;
			if (!cloudStorage.DirectoryExists(UserProfile.userProfilesFolder))
			{
				cloudStorage.CreateDirectory(UserProfile.userProfilesFolder);
			}
			foreach (UPath path in cloudStorage.EnumeratePaths(UserProfile.userProfilesFolder))
			{
				if (cloudStorage.FileExists(path) && string.CompareOrdinal(path.GetExtensionWithDot(), ".xml") == 0)
				{
					UserProfile.LoadUserProfileOperationResult loadUserProfileOperationResult = UserProfile.LoadUserProfileFromDisk(cloudStorage, path);
					UserProfile userProfile = loadUserProfileOperationResult.userProfile;
					if (userProfile != null)
					{
						UserProfile.loadedUserProfiles[userProfile.fileName] = userProfile;
					}
					if (loadUserProfileOperationResult.exception != null)
					{
						UserProfile.badFileResults.Add(loadUserProfileOperationResult);
					}
				}
			}
			UserProfile.OutputBadFileResults();
		}

		// Token: 0x06001B5F RID: 7007 RVA: 0x00074B4C File Offset: 0x00072D4C
		public static List<string> GetAvailableProfileNames()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, UserProfile> keyValuePair in UserProfile.loadedUserProfiles)
			{
				if (!keyValuePair.Value.isClaimed)
				{
					list.Add(keyValuePair.Key);
				}
			}
			list.Sort();
			return list;
		}

		// Token: 0x06001B60 RID: 7008 RVA: 0x00074BC0 File Offset: 0x00072DC0
		public static UserProfile GetProfile(string profileName)
		{
			profileName = profileName.ToLower(CultureInfo.InvariantCulture);
			UserProfile result;
			if (UserProfile.loadedUserProfiles.TryGetValue(profileName, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06001B61 RID: 7009 RVA: 0x00074BEC File Offset: 0x00072DEC
		public void RequestSave(bool immediate = false)
		{
			if (!this.canSave)
			{
				return;
			}
			if (immediate)
			{
				this.Save(true);
				return;
			}
			this.saveRequestPending = true;
		}

		// Token: 0x06001B62 RID: 7010 RVA: 0x00074C0C File Offset: 0x00072E0C
		private bool Save(bool blocking)
		{
			bool result;
			try
			{
				UserProfile.SaveHelper.StartSave(this, blocking);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06001B63 RID: 7011 RVA: 0x00074C3C File Offset: 0x00072E3C
		private static void SkipBOM(Stream stream)
		{
			long position = stream.Position;
			if (stream.Length - position < 3L)
			{
				return;
			}
			int num = stream.ReadByte();
			int num2 = stream.ReadByte();
			if (num == 255 && num2 == 254)
			{
				Debug.Log("Skipping UTF-8 BOM");
				return;
			}
			int num3 = stream.ReadByte();
			if (num == 239 && num2 == 187 && num3 == 191)
			{
				Debug.Log("Skipping UTF-16 BOM");
				return;
			}
			stream.Position = position;
		}

		// Token: 0x06001B64 RID: 7012 RVA: 0x00074CB8 File Offset: 0x00072EB8
		private static UserProfile.LoadUserProfileOperationResult LoadUserProfileFromDisk(IFileSystem fileSystem, UPath path)
		{
			Debug.LogFormat("Attempting to load user profile {0}", new object[]
			{
				path
			});
			UserProfile.LoadUserProfileOperationResult loadUserProfileOperationResult = new UserProfile.LoadUserProfileOperationResult
			{
				fileName = path.FullName,
				fileLength = 0L,
				userProfile = null,
				exception = null,
				failureContents = null
			};
			UserProfile.LoadUserProfileOperationResult result = loadUserProfileOperationResult;
			try
			{
				using (Stream stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					UserProfile.SkipBOM(stream);
					result.fileLength = stream.Length;
					using (TextReader textReader = new StreamReader(stream, Encoding.UTF8))
					{
						Debug.LogFormat("stream.Length={0}", new object[]
						{
							stream.Length
						});
						try
						{
							UserProfile userProfile = UserProfile.XmlUtility.FromXml(XDocument.Load(textReader));
							userProfile.fileName = path.GetNameWithoutExtension();
							userProfile.canSave = true;
							userProfile.fileSystem = fileSystem;
							userProfile.filePath = path;
							result.userProfile = userProfile;
							return result;
						}
						catch (XmlException ex)
						{
							stream.Position = 0L;
							byte[] array = new byte[stream.Length];
							stream.Read(array, 0, (int)stream.Length);
							result.failureContents = Convert.ToBase64String(array);
							UserProfile userProfile2 = UserProfile.CreateGuestProfile();
							userProfile2.fileSystem = fileSystem;
							userProfile2.filePath = path;
							userProfile2.fileName = path.GetNameWithoutExtension();
							userProfile2.name = string.Format("<color=#FF7F7FFF>Corrupted Profile: {0}</color>", userProfile2.fileName);
							userProfile2.canSave = false;
							userProfile2.isCorrupted = true;
							result.userProfile = userProfile2;
							throw ex;
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Debug.LogFormat("Failed to load user profile {0}: {1}\nStack Trace:\n{2}", new object[]
				{
					path,
					ex2.Message,
					ex2.StackTrace
				});
				result.exception = ex2;
			}
			return result;
		}

		// Token: 0x06001B65 RID: 7013 RVA: 0x00074EE4 File Offset: 0x000730E4
		private static void Copy(UserProfile src, UserProfile dest)
		{
			dest.fileSystem = src.fileSystem;
			dest.filePath = src.filePath;
			StatSheet.Copy(src.statSheet, dest.statSheet);
			src.loadout.Copy(dest.loadout);
			dest.tutorialSprint = src.tutorialSprint;
			dest.tutorialDifficulty = src.tutorialDifficulty;
			dest.tutorialEquipment = src.tutorialEquipment;
			UserProfile.SaveFieldAttribute[] array = UserProfile.saveFields;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].copier(src, dest);
			}
			dest.isClaimed = false;
			dest.canSave = false;
			dest.fileName = src.fileName;
			dest.onPickupDiscovered = null;
			dest.onStatsReceived = null;
			dest.loggedIn = false;
		}

		// Token: 0x06001B66 RID: 7014 RVA: 0x00074FA4 File Offset: 0x000731A4
		private static void DeleteUserProfile(string fileName)
		{
			fileName = fileName.ToLower(CultureInfo.InvariantCulture);
			UserProfile profile = UserProfile.GetProfile(fileName);
			if (UserProfile.loadedUserProfiles.ContainsKey(fileName))
			{
				UserProfile.loadedUserProfiles.Remove(fileName);
			}
			if (profile != null && profile.fileSystem != null)
			{
				profile.fileSystem.DeleteFile(profile.filePath);
			}
			Action action = UserProfile.onAvailableUserProfilesChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001B67 RID: 7015 RVA: 0x00075009 File Offset: 0x00073209
		public static XDocument ToXml(UserProfile userProfile)
		{
			return UserProfile.XmlUtility.ToXml(userProfile);
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x00075011 File Offset: 0x00073211
		private static UserProfile FromXml(XDocument doc)
		{
			return UserProfile.XmlUtility.FromXml(doc);
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x0007501C File Offset: 0x0007321C
		public static UserProfile CreateProfile(IFileSystem fileSystem, string name)
		{
			UserProfile userProfile = UserProfile.FromXml(UserProfile.ToXml(UserProfile.defaultProfile));
			userProfile.fileName = Guid.NewGuid().ToString();
			userProfile.fileSystem = fileSystem;
			userProfile.filePath = UserProfile.userProfilesFolder / (userProfile.fileName + ".xml");
			userProfile.name = name;
			userProfile.canSave = true;
			UserProfile.loadedUserProfiles.Add(userProfile.fileName, userProfile);
			userProfile.Save(true);
			Action action = UserProfile.onAvailableUserProfilesChanged;
			if (action != null)
			{
				action();
			}
			return userProfile;
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x000750B8 File Offset: 0x000732B8
		public static UserProfile CreateGuestProfile()
		{
			UserProfile userProfile = new UserProfile();
			UserProfile.Copy(UserProfile.defaultProfile, userProfile);
			userProfile.name = "Guest";
			return userProfile;
		}

		// Token: 0x06001B6B RID: 7019 RVA: 0x000750E4 File Offset: 0x000732E4
		[ConCommand(commandName = "user_profile_save", flags = ConVarFlags.None, helpText = "Saves the named profile to disk, if it exists.")]
		private static void CCUserProfileSave(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			string text = args[0].ToLower(CultureInfo.InvariantCulture);
			if (text == "default")
			{
				Debug.Log("Cannot save profile \"default\", it is a reserved profile.");
				return;
			}
			UserProfile profile = UserProfile.GetProfile(text);
			if (profile == null)
			{
				Debug.LogFormat("Could not find profile \"{0}\" to save.", new object[]
				{
					text
				});
				return;
			}
			profile.Save(true);
		}

		// Token: 0x06001B6C RID: 7020 RVA: 0x0007514C File Offset: 0x0007334C
		[ConCommand(commandName = "user_profile_copy", flags = ConVarFlags.None, helpText = "Copies the profile named by the first argument to a new profile named by the second argument. This does not save the profile.")]
		private static void CCUserProfileCopy(ConCommandArgs args)
		{
			args.CheckArgumentCount(2);
			string text = args[0].ToLower(CultureInfo.InvariantCulture);
			string text2 = args[1].ToLower(CultureInfo.InvariantCulture);
			UserProfile profile = UserProfile.GetProfile(text);
			if (profile == null)
			{
				Debug.LogFormat("Profile {0} does not exist, so it cannot be copied.", new object[]
				{
					text
				});
				return;
			}
			if (UserProfile.GetProfile(text2) != null)
			{
				Debug.LogFormat("Profile {0} already exists, and cannot be copied to.", new object[]
				{
					text2
				});
				return;
			}
			UserProfile userProfile = new UserProfile();
			UserProfile.Copy(profile, userProfile);
			userProfile.fileSystem = (profile.fileSystem ?? RoR2Application.cloudStorage);
			userProfile.filePath = UserProfile.userProfilesFolder / (text2 + ".xml");
			userProfile.fileName = text2;
			userProfile.canSave = true;
			UserProfile.loadedUserProfiles[text2] = userProfile;
			Action action = UserProfile.onAvailableUserProfilesChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001B6D RID: 7021 RVA: 0x00075230 File Offset: 0x00073430
		[ConCommand(commandName = "user_profile_delete", flags = ConVarFlags.None, helpText = "Unloads the named user profile and deletes it from the disk if it exists.")]
		private static void CCUserProfileDelete(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			string a = args[0].ToLower(CultureInfo.InvariantCulture);
			if (a == "default")
			{
				Debug.Log("Cannot delete profile \"default\", it is a reserved profile.");
				return;
			}
			UserProfile.DeleteUserProfile(a);
		}

		// Token: 0x06001B6E RID: 7022 RVA: 0x00075278 File Offset: 0x00073478
		[ConCommand(commandName = "create_corrupted_profiles", flags = ConVarFlags.None, helpText = "Creates corrupted user profiles.")]
		private static void CCCreateCorruptedProfiles(ConCommandArgs args)
		{
			UserProfile.<>c__DisplayClass121_0 CS$<>8__locals1;
			CS$<>8__locals1.fileSystem = RoR2Application.cloudStorage;
			UserProfile.<CCCreateCorruptedProfiles>g__WriteFile|121_0("empty", "", ref CS$<>8__locals1);
			UserProfile.<CCCreateCorruptedProfiles>g__WriteFile|121_0("truncated", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<UserProfile>\r\n", ref CS$<>8__locals1);
			UserProfile.<CCCreateCorruptedProfiles>g__WriteFile|121_0("multiroot", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<UserProfile>\r\n</UserProfile>\r\n<UserProfile>\r\n</UserProfile>", ref CS$<>8__locals1);
			UserProfile.<CCCreateCorruptedProfiles>g__WriteFile|121_0("outoforder", "<?xml version=\"1.0\" encodi=\"utf-8\"ng?>\r\n<Userrofile>\r\n<UserProfile>\r\n</UserProfileProfile>\r\n</UserP>", ref CS$<>8__locals1);
		}

		// Token: 0x06001B6F RID: 7023 RVA: 0x000752D8 File Offset: 0x000734D8
		[ConCommand(commandName = "userprofile_test_buffer_overflow", flags = ConVarFlags.None, helpText = "")]
		private static void CCUserProfileTestBufferOverflow(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			int num = 128;
			FileSystem cloudStorage = RoR2Application.cloudStorage;
			RemoteFile remoteFile = Client.Instance.RemoteStorage.OpenFile(args[0]);
			int sizeInBytes = remoteFile.SizeInBytes;
			FieldInfo field = remoteFile.GetType().GetField("_sizeInBytes", BindingFlags.Instance | BindingFlags.NonPublic);
			int num2 = (int)field.GetValue(remoteFile);
			field.SetValue(remoteFile, num2 + num);
			byte[] array = remoteFile.ReadAllBytes();
			byte[] array2 = new byte[num];
			for (int i = 0; i < num; i++)
			{
				Debug.Log(array[num2 + i]);
				array2[i] = array[num2 + i];
			}
			GUIUtility.systemCopyBuffer = Encoding.UTF8.GetString(array2);
			field.SetValue(remoteFile, num2);
		}

		// Token: 0x1400006A RID: 106
		// (add) Token: 0x06001B70 RID: 7024 RVA: 0x000753A4 File Offset: 0x000735A4
		// (remove) Token: 0x06001B71 RID: 7025 RVA: 0x000753D8 File Offset: 0x000735D8
		public static event Action onAvailableUserProfilesChanged;

		// Token: 0x1400006B RID: 107
		// (add) Token: 0x06001B72 RID: 7026 RVA: 0x0007540C File Offset: 0x0007360C
		// (remove) Token: 0x06001B73 RID: 7027 RVA: 0x00075440 File Offset: 0x00073640
		public static event Action<UserProfile, string> onUnlockableGranted;

		// Token: 0x06001B74 RID: 7028 RVA: 0x00075473 File Offset: 0x00073673
		private static void LoadDefaultProfile()
		{
			UserProfile.defaultProfile = UserProfile.XmlUtility.FromXml(XDocument.Parse("<UserProfile>\r\n  <name>Survivor</name>\r\n  <mouseLookSensitivity>0.2</mouseLookSensitivity>\r\n  <mouseLookScaleX>1</mouseLookScaleX>\r\n  <mouseLookScaleY>1</mouseLookScaleY>\r\n  <stickLookSensitivity>5</stickLookSensitivity>\r\n  <stickLookScaleX>1</stickLookScaleX>\r\n  <stickLookScaleY>1</stickLookScaleY>\r\n</UserProfile>"));
			UserProfile.defaultProfile.canSave = false;
		}

		// Token: 0x06001B75 RID: 7029 RVA: 0x00075494 File Offset: 0x00073694
		public bool HasViewedViewable(string viewableName)
		{
			return this.viewedViewables.Contains(viewableName);
		}

		// Token: 0x06001B76 RID: 7030 RVA: 0x000754A2 File Offset: 0x000736A2
		public void MarkViewableAsViewed(string viewableName)
		{
			if (this.HasViewedViewable(viewableName))
			{
				return;
			}
			this.viewedViewables.Add(viewableName);
			Action<UserProfile> action = UserProfile.onUserProfileViewedViewablesChanged;
			if (action != null)
			{
				action(this);
			}
			this.RequestSave(false);
		}

		// Token: 0x1400006C RID: 108
		// (add) Token: 0x06001B77 RID: 7031 RVA: 0x000754D4 File Offset: 0x000736D4
		// (remove) Token: 0x06001B78 RID: 7032 RVA: 0x00075508 File Offset: 0x00073708
		public static event Action<UserProfile> onUserProfileViewedViewablesChanged;

		// Token: 0x06001B7B RID: 7035 RVA: 0x0007569C File Offset: 0x0007389C
		[CompilerGenerated]
		internal static void <CCCreateCorruptedProfiles>g__WriteFile|121_0(string fileName, string contents, ref UserProfile.<>c__DisplayClass121_0 A_2)
		{
			using (Stream stream = A_2.fileSystem.OpenFile(UserProfile.userProfilesFolder / (fileName + ".xml"), FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (TextWriter textWriter = new StreamWriter(stream))
				{
					textWriter.Write(contents.ToCharArray());
					textWriter.Flush();
				}
				stream.Flush();
			}
		}

		// Token: 0x0400189C RID: 6300
		public bool isClaimed;

		// Token: 0x0400189D RID: 6301
		public bool canSave;

		// Token: 0x0400189E RID: 6302
		public string fileName;

		// Token: 0x0400189F RID: 6303
		public IFileSystem fileSystem;

		// Token: 0x040018A0 RID: 6304
		public UPath filePath = UPath.Empty;

		// Token: 0x040018A1 RID: 6305
		[UserProfile.SaveFieldAttribute]
		public string name;

		// Token: 0x040018A2 RID: 6306
		[UserProfile.SaveFieldAttribute]
		public uint coins;

		// Token: 0x040018A3 RID: 6307
		[UserProfile.SaveFieldAttribute]
		public uint totalCollectedCoins;

		// Token: 0x040018A5 RID: 6309
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupTokenList")]
		public List<string> viewedUnlockablesList = new List<string>();

		// Token: 0x040018A6 RID: 6310
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupPickupsSet")]
		private readonly bool[] discoveredPickups = PickupCatalog.GetPerPickupBuffer<bool>();

		// Token: 0x040018A8 RID: 6312
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupTokenList")]
		private List<string> achievementsList = new List<string>();

		// Token: 0x040018A9 RID: 6313
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupTokenList")]
		private List<string> unviewedAchievementsList = new List<string>();

		// Token: 0x040018AA RID: 6314
		[UserProfile.SaveFieldAttribute]
		public string version = "2";

		// Token: 0x040018AB RID: 6315
		[UserProfile.SaveFieldAttribute]
		public float screenShakeScale = 1f;

		// Token: 0x040018AC RID: 6316
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupKeyboardMap")]
		public KeyboardMap keyboardMap = new KeyboardMap(DefaultControllerMaps.defaultKeyboardMap);

		// Token: 0x040018AD RID: 6317
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupMouseMap")]
		public MouseMap mouseMap = new MouseMap(DefaultControllerMaps.defaultMouseMap);

		// Token: 0x040018AE RID: 6318
		[UserProfile.SaveFieldAttribute(explicitSetupMethod = "SetupJoystickMap")]
		public JoystickMap joystickMap = new JoystickMap(DefaultControllerMaps.defaultJoystickMap);

		// Token: 0x040018AF RID: 6319
		[UserProfile.SaveFieldAttribute]
		public float mouseLookSensitivity = 0.25f;

		// Token: 0x040018B0 RID: 6320
		[UserProfile.SaveFieldAttribute]
		public float mouseLookScaleX = 1f;

		// Token: 0x040018B1 RID: 6321
		[UserProfile.SaveFieldAttribute]
		public float mouseLookScaleY = 1f;

		// Token: 0x040018B2 RID: 6322
		[UserProfile.SaveFieldAttribute]
		public bool mouseLookInvertX;

		// Token: 0x040018B3 RID: 6323
		[UserProfile.SaveFieldAttribute]
		public bool mouseLookInvertY;

		// Token: 0x040018B4 RID: 6324
		[UserProfile.SaveFieldAttribute]
		public float stickLookSensitivity = 4f;

		// Token: 0x040018B5 RID: 6325
		[UserProfile.SaveFieldAttribute]
		public float stickLookScaleX = 1f;

		// Token: 0x040018B6 RID: 6326
		[UserProfile.SaveFieldAttribute]
		public float stickLookScaleY = 1f;

		// Token: 0x040018B7 RID: 6327
		[UserProfile.SaveFieldAttribute]
		public bool stickLookInvertX;

		// Token: 0x040018B8 RID: 6328
		[UserProfile.SaveFieldAttribute]
		public bool stickLookInvertY;

		// Token: 0x040018B9 RID: 6329
		[UserProfile.SaveFieldAttribute]
		public float gamepadVibrationScale = 1f;

		// Token: 0x040018BA RID: 6330
		private static string[] saveFieldNames;

		// Token: 0x040018BB RID: 6331
		private static UserProfile.SaveFieldAttribute[] saveFields;

		// Token: 0x040018BC RID: 6332
		private static readonly Dictionary<string, UserProfile.SaveFieldAttribute> nameToSaveFieldMap = new Dictionary<string, UserProfile.SaveFieldAttribute>();

		// Token: 0x040018BD RID: 6333
		public StatSheet statSheet = StatSheet.New();

		// Token: 0x040018BF RID: 6335
		private const uint maxShowCount = 3U;

		// Token: 0x040018C0 RID: 6336
		public UserProfile.TutorialProgression tutorialDifficulty;

		// Token: 0x040018C1 RID: 6337
		public UserProfile.TutorialProgression tutorialSprint;

		// Token: 0x040018C2 RID: 6338
		public UserProfile.TutorialProgression tutorialEquipment;

		// Token: 0x040018C3 RID: 6339
		private readonly Loadout loadout = new Loadout();

		// Token: 0x040018C6 RID: 6342
		[UserProfile.SaveFieldAttribute]
		public uint totalLoginSeconds;

		// Token: 0x040018C7 RID: 6343
		[UserProfile.SaveFieldAttribute]
		public uint totalRunSeconds;

		// Token: 0x040018C8 RID: 6344
		[UserProfile.SaveFieldAttribute]
		public uint totalAliveSeconds;

		// Token: 0x040018C9 RID: 6345
		[UserProfile.SaveFieldAttribute]
		public uint totalRunCount;

		// Token: 0x040018CA RID: 6346
		private static float secondAccumulator;

		// Token: 0x040018CB RID: 6347
		private static readonly List<UserProfile> loggedInProfiles = new List<UserProfile>();

		// Token: 0x040018CD RID: 6349
		private static UPath userProfilesFolder = "/UserProfiles";

		// Token: 0x040018CE RID: 6350
		private static readonly Dictionary<string, UserProfile> loadedUserProfiles = new Dictionary<string, UserProfile>();

		// Token: 0x040018CF RID: 6351
		private static readonly List<UserProfile.LoadUserProfileOperationResult> badFileResults = new List<UserProfile.LoadUserProfileOperationResult>();

		// Token: 0x040018D0 RID: 6352
		private bool saveRequestPending;

		// Token: 0x040018D1 RID: 6353
		private static readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

		// Token: 0x040018D2 RID: 6354
		public static UserProfile defaultProfile;

		// Token: 0x040018D5 RID: 6357
		private const string defaultProfileContents = "<UserProfile>\r\n  <name>Survivor</name>\r\n  <mouseLookSensitivity>0.2</mouseLookSensitivity>\r\n  <mouseLookScaleX>1</mouseLookScaleX>\r\n  <mouseLookScaleY>1</mouseLookScaleY>\r\n  <stickLookSensitivity>5</stickLookSensitivity>\r\n  <stickLookScaleX>1</stickLookScaleX>\r\n  <stickLookScaleY>1</stickLookScaleY>\r\n</UserProfile>";

		// Token: 0x040018D6 RID: 6358
		[UserProfile.SaveFieldAttribute(defaultValue = "", explicitSetupMethod = "SetupTokenList", fieldName = "viewedViewables")]
		private readonly List<string> viewedViewables = new List<string>();

		// Token: 0x02000465 RID: 1125
		public class SaveFieldAttribute : Attribute
		{
			// Token: 0x06001B7C RID: 7036 RVA: 0x00075728 File Offset: 0x00073928
			public void Setup(FieldInfo fieldInfo)
			{
				this.fieldInfo = fieldInfo;
				Type fieldType = fieldInfo.FieldType;
				this.fieldName = fieldInfo.Name;
				if (this.explicitSetupMethod != null)
				{
					MethodInfo method = typeof(UserProfile.SaveFieldAttribute).GetMethod(this.explicitSetupMethod, BindingFlags.Instance | BindingFlags.Public);
					if (method == null)
					{
						Debug.LogErrorFormat("Explicit setup {0} specified by field {1} could not be found. Use the nameof() operator to ensure the method exists.", Array.Empty<object>());
						return;
					}
					if (method.GetParameters().Length > 1)
					{
						Debug.LogErrorFormat("Explicit setup method {0} for field {1} must have one parameter.", new object[]
						{
							this.explicitSetupMethod,
							fieldInfo.Name
						});
						return;
					}
					method.Invoke(this, new object[]
					{
						fieldInfo
					});
					return;
				}
				else
				{
					if (fieldType == typeof(string))
					{
						this.SetupString(fieldInfo);
						return;
					}
					if (fieldType == typeof(int))
					{
						this.SetupInt(fieldInfo);
						return;
					}
					if (fieldType == typeof(uint))
					{
						this.SetupUint(fieldInfo);
						return;
					}
					if (fieldType == typeof(float))
					{
						this.SetupFloat(fieldInfo);
						return;
					}
					if (fieldType == typeof(bool))
					{
						this.SetupBool(fieldInfo);
						return;
					}
					Debug.LogErrorFormat("No explicit setup method or supported type for save field {0}", new object[]
					{
						fieldInfo.Name
					});
					return;
				}
			}

			// Token: 0x06001B7D RID: 7037 RVA: 0x00075864 File Offset: 0x00073A64
			public void SetupString(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => (string)fieldInfo.GetValue(userProfile));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					fieldInfo.SetValue(userProfile, valueString);
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001B7E RID: 7038 RVA: 0x000758B4 File Offset: 0x00073AB4
			public void SetupFloat(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => TextSerialization.ToStringInvariant((float)fieldInfo.GetValue(userProfile)));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					float num;
					if (TextSerialization.TryParseInvariant(valueString, out num))
					{
						fieldInfo.SetValue(userProfile, num);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001B7F RID: 7039 RVA: 0x00075904 File Offset: 0x00073B04
			public void SetupInt(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => TextSerialization.ToStringInvariant((int)fieldInfo.GetValue(userProfile)));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					int num;
					if (TextSerialization.TryParseInvariant(valueString, out num))
					{
						fieldInfo.SetValue(userProfile, num);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001B80 RID: 7040 RVA: 0x00075954 File Offset: 0x00073B54
			public void SetupUint(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => TextSerialization.ToStringInvariant((uint)fieldInfo.GetValue(userProfile)));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					uint num;
					if (TextSerialization.TryParseInvariant(valueString, out num))
					{
						fieldInfo.SetValue(userProfile, num);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001B81 RID: 7041 RVA: 0x000759A4 File Offset: 0x00073BA4
			public void SetupBool(FieldInfo fieldInfo)
			{
				this.getter = delegate(UserProfile userProfile)
				{
					if (!(bool)fieldInfo.GetValue(userProfile))
					{
						return "0";
					}
					return "1";
				};
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					int num;
					if (TextSerialization.TryParseInvariant(valueString, out num))
					{
						fieldInfo.SetValue(userProfile, num > 0);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					fieldInfo.SetValue(destProfile, fieldInfo.GetValue(srcProfile));
				};
			}

			// Token: 0x06001B82 RID: 7042 RVA: 0x000759F4 File Offset: 0x00073BF4
			public void SetupTokenList(FieldInfo fieldInfo)
			{
				this.getter = ((UserProfile userProfile) => string.Join(" ", (List<string>)fieldInfo.GetValue(userProfile)));
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					List<string> list = (List<string>)fieldInfo.GetValue(userProfile);
					list.Clear();
					foreach (string item in valueString.Split(new char[]
					{
						' '
					}))
					{
						list.Add(item);
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					List<string> src = (List<string>)fieldInfo.GetValue(srcProfile);
					List<string> dest = (List<string>)fieldInfo.GetValue(destProfile);
					Util.CopyList<string>(src, dest);
				};
			}

			// Token: 0x06001B83 RID: 7043 RVA: 0x00075A44 File Offset: 0x00073C44
			public void SetupPickupsSet(FieldInfo fieldInfo)
			{
				this.getter = delegate(UserProfile userProfile)
				{
					bool[] pickupsSet = (bool[])fieldInfo.GetValue(userProfile);
					return string.Join(" ", from pickupIndex in PickupIndex.allPickups
					where pickupsSet[pickupIndex.value]
					select pickupIndex.ToString());
				};
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					bool[] array = (bool[])fieldInfo.GetValue(userProfile);
					Array.Clear(array, 0, 0);
					string[] array2 = valueString.Split(new char[]
					{
						' '
					});
					for (int i = 0; i < array2.Length; i++)
					{
						PickupIndex pickupIndex = PickupIndex.Find(array2[i]);
						if (pickupIndex.isValid)
						{
							array[pickupIndex.value] = true;
						}
					}
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					Array sourceArray = (bool[])fieldInfo.GetValue(srcProfile);
					bool[] array = (bool[])fieldInfo.GetValue(destProfile);
					Array.Copy(sourceArray, array, array.Length);
				};
			}

			// Token: 0x06001B84 RID: 7044 RVA: 0x00075A94 File Offset: 0x00073C94
			public void SetupKeyboardMap(FieldInfo fieldInfo)
			{
				this.SetupControllerMap(fieldInfo, ControllerType.Keyboard);
			}

			// Token: 0x06001B85 RID: 7045 RVA: 0x00075A9E File Offset: 0x00073C9E
			public void SetupMouseMap(FieldInfo fieldInfo)
			{
				this.SetupControllerMap(fieldInfo, ControllerType.Mouse);
			}

			// Token: 0x06001B86 RID: 7046 RVA: 0x00075AA8 File Offset: 0x00073CA8
			public void SetupJoystickMap(FieldInfo fieldInfo)
			{
				this.SetupControllerMap(fieldInfo, ControllerType.Joystick);
			}

			// Token: 0x06001B87 RID: 7047 RVA: 0x00075AB4 File Offset: 0x00073CB4
			private void SetupControllerMap(FieldInfo fieldInfo, ControllerType controllerType)
			{
				this.getter = delegate(UserProfile userProfile)
				{
					ControllerMap controllerMap = (ControllerMap)fieldInfo.GetValue(userProfile);
					return ((controllerMap != null) ? controllerMap.ToXmlString() : null) ?? string.Empty;
				};
				this.setter = delegate(UserProfile userProfile, string valueString)
				{
					fieldInfo.SetValue(userProfile, ControllerMap.CreateFromXml(controllerType, valueString));
				};
				this.copier = delegate(UserProfile srcProfile, UserProfile destProfile)
				{
					switch (controllerType)
					{
					case ControllerType.Keyboard:
						fieldInfo.SetValue(destProfile, new KeyboardMap((KeyboardMap)fieldInfo.GetValue(srcProfile)));
						return;
					case ControllerType.Mouse:
						fieldInfo.SetValue(destProfile, new MouseMap((MouseMap)fieldInfo.GetValue(srcProfile)));
						return;
					case ControllerType.Joystick:
						fieldInfo.SetValue(destProfile, new JoystickMap((JoystickMap)fieldInfo.GetValue(srcProfile)));
						return;
					default:
						throw new NotImplementedException();
					}
				};
			}

			// Token: 0x040018D8 RID: 6360
			public Action<UserProfile, string> setter;

			// Token: 0x040018D9 RID: 6361
			public Func<UserProfile, string> getter;

			// Token: 0x040018DA RID: 6362
			public Action<UserProfile, UserProfile> copier;

			// Token: 0x040018DB RID: 6363
			public string defaultValue = string.Empty;

			// Token: 0x040018DC RID: 6364
			public string fieldName;

			// Token: 0x040018DD RID: 6365
			public string explicitSetupMethod;

			// Token: 0x040018DE RID: 6366
			private FieldInfo fieldInfo;
		}

		// Token: 0x02000470 RID: 1136
		public struct TutorialProgression
		{
			// Token: 0x040018EB RID: 6379
			public uint showCount;

			// Token: 0x040018EC RID: 6380
			public bool shouldShow;
		}

		// Token: 0x02000471 RID: 1137
		private static class SaveHelper
		{
			// Token: 0x06001BAE RID: 7086 RVA: 0x00075F94 File Offset: 0x00074194
			public static void StartSave(UserProfile userProfile, bool blocking)
			{
				UserProfile.SaveHelper.<>c__DisplayClass0_0 CS$<>8__locals1 = new UserProfile.SaveHelper.<>c__DisplayClass0_0();
				CS$<>8__locals1.tempCopy = new UserProfile();
				UserProfile.Copy(userProfile, CS$<>8__locals1.tempCopy);
				CS$<>8__locals1.fileOutput = new UserProfile.SaveHelper.FileOutput
				{
					fileReference = new UserProfile.SaveHelper.FileReference
					{
						path = CS$<>8__locals1.tempCopy.filePath,
						fileSystem = CS$<>8__locals1.tempCopy.fileSystem
					},
					requestTime = DateTime.UtcNow,
					contents = Array.Empty<byte>()
				};
				CS$<>8__locals1.task = null;
				CS$<>8__locals1.task = new Task(new Action(CS$<>8__locals1.<StartSave>g__PayloadGeneratorAction|0));
				UserProfile.SaveHelper.AddActiveTask(CS$<>8__locals1.task);
				CS$<>8__locals1.task.Start(TaskScheduler.Default);
				if (blocking)
				{
					CS$<>8__locals1.task.Wait();
					UserProfile.SaveHelper.ProcessFileOutputQueue();
				}
			}

			// Token: 0x06001BAF RID: 7087 RVA: 0x00076060 File Offset: 0x00074260
			public static void WaitAll()
			{
				for (;;)
				{
					Task task = null;
					List<Task> obj = UserProfile.SaveHelper.activeTasks;
					lock (obj)
					{
						if (UserProfile.SaveHelper.activeTasks.Count == 0)
						{
							break;
						}
						task = UserProfile.SaveHelper.activeTasks[0];
					}
					if (task != null)
					{
						task.Wait();
					}
				}
			}

			// Token: 0x06001BB0 RID: 7088 RVA: 0x000760C0 File Offset: 0x000742C0
			private static void AddActiveTask(Task task)
			{
				List<Task> obj = UserProfile.SaveHelper.activeTasks;
				lock (obj)
				{
					UserProfile.SaveHelper.activeTasks.Add(task);
				}
			}

			// Token: 0x06001BB1 RID: 7089 RVA: 0x00076104 File Offset: 0x00074304
			private static void RemoveActiveTask(Task task)
			{
				List<Task> obj = UserProfile.SaveHelper.activeTasks;
				lock (obj)
				{
					UserProfile.SaveHelper.activeTasks.Remove(task);
				}
			}

			// Token: 0x06001BB2 RID: 7090 RVA: 0x0007614C File Offset: 0x0007434C
			private static void EnqueueFileOutput(UserProfile.SaveHelper.FileOutput fileOutput)
			{
				Queue<UserProfile.SaveHelper.FileOutput> obj = UserProfile.SaveHelper.pendingOutputQueue;
				lock (obj)
				{
					UserProfile.SaveHelper.pendingOutputQueue.Enqueue(fileOutput);
				}
			}

			// Token: 0x06001BB3 RID: 7091 RVA: 0x00076190 File Offset: 0x00074390
			public static void ProcessFileOutputQueue()
			{
				Queue<UserProfile.SaveHelper.FileOutput> obj = UserProfile.SaveHelper.pendingOutputQueue;
				lock (obj)
				{
					while (UserProfile.SaveHelper.pendingOutputQueue.Count > 0)
					{
						UserProfile.SaveHelper.FileOutput fileOutput = UserProfile.SaveHelper.pendingOutputQueue.Dequeue();
						if (UserProfile.SaveHelper.CanWrite(fileOutput))
						{
							UserProfile.SaveHelper.WriteToDisk(fileOutput);
						}
					}
				}
			}

			// Token: 0x06001BB4 RID: 7092 RVA: 0x000761F4 File Offset: 0x000743F4
			private static bool CanWrite(UserProfile.SaveHelper.FileOutput fileOutput)
			{
				if (fileOutput.contents.Length == 0)
				{
					Debug.LogErrorFormat("Cannot write UserProfile \"{0}\" with zero-length contents. This would erase the file.", Array.Empty<object>());
					return false;
				}
				DateTime t;
				return !UserProfile.SaveHelper.latestWrittenRequestTimesByFile.TryGetValue(fileOutput.fileReference, out t) || t < fileOutput.requestTime;
			}

			// Token: 0x06001BB5 RID: 7093 RVA: 0x00076240 File Offset: 0x00074440
			private static void WriteToDisk(UserProfile.SaveHelper.FileOutput fileOutput)
			{
				RoR2Application.IncrementActiveWriteCount();
				try
				{
					using (Stream stream = fileOutput.fileReference.fileSystem.OpenFile(fileOutput.fileReference.path, FileMode.Create, FileAccess.Write, FileShare.None))
					{
						stream.Write(fileOutput.contents, 0, fileOutput.contents.Length);
						stream.Flush();
						stream.Close();
						Debug.LogFormat("Saved file \"{0}\" ({1} bytes)", new object[]
						{
							fileOutput.fileReference.path.GetName(),
							fileOutput.contents.Length
						});
					}
					UserProfile.SaveHelper.latestWrittenRequestTimesByFile[fileOutput.fileReference] = fileOutput.requestTime;
				}
				catch (Exception message)
				{
					Debug.Log(message);
				}
				finally
				{
					RoR2Application.DecrementActiveWriteCount();
				}
			}

			// Token: 0x040018ED RID: 6381
			private static readonly List<Task> activeTasks = new List<Task>();

			// Token: 0x040018EE RID: 6382
			private static readonly Dictionary<UserProfile.SaveHelper.FileReference, DateTime> latestWrittenRequestTimesByFile = new Dictionary<UserProfile.SaveHelper.FileReference, DateTime>();

			// Token: 0x040018EF RID: 6383
			private static readonly Queue<UserProfile.SaveHelper.FileOutput> pendingOutputQueue = new Queue<UserProfile.SaveHelper.FileOutput>();

			// Token: 0x02000472 RID: 1138
			private struct FileReference : IEquatable<UserProfile.SaveHelper.FileReference>
			{
				// Token: 0x06001BB7 RID: 7095 RVA: 0x00076340 File Offset: 0x00074540
				public bool Equals(UserProfile.SaveHelper.FileReference other)
				{
					return this.fileSystem.Equals(other.fileSystem) && this.path.Equals(other.path);
				}

				// Token: 0x06001BB8 RID: 7096 RVA: 0x00076368 File Offset: 0x00074568
				public override bool Equals(object other)
				{
					return other is UserProfile.SaveHelper.FileReference && this.Equals((UserProfile.SaveHelper.FileReference)other);
				}

				// Token: 0x06001BB9 RID: 7097 RVA: 0x00076380 File Offset: 0x00074580
				public override int GetHashCode()
				{
					return (-990633296 * -1521134295 + EqualityComparer<IFileSystem>.Default.GetHashCode(this.fileSystem)) * -1521134295 + EqualityComparer<UPath>.Default.GetHashCode(this.path);
				}

				// Token: 0x040018F0 RID: 6384
				public IFileSystem fileSystem;

				// Token: 0x040018F1 RID: 6385
				public UPath path;
			}

			// Token: 0x02000473 RID: 1139
			private class FileOutput
			{
				// Token: 0x040018F2 RID: 6386
				public UserProfile.SaveHelper.FileReference fileReference;

				// Token: 0x040018F3 RID: 6387
				public DateTime requestTime;

				// Token: 0x040018F4 RID: 6388
				public byte[] contents;
			}
		}

		// Token: 0x02000476 RID: 1142
		private static class XmlUtility
		{
			// Token: 0x06001BBF RID: 7103 RVA: 0x000764D9 File Offset: 0x000746D9
			private static XElement CreateStringField(string name, string value)
			{
				return new XElement(name, value);
			}

			// Token: 0x06001BC0 RID: 7104 RVA: 0x000764E7 File Offset: 0x000746E7
			private static XElement CreateUintField(string name, uint value)
			{
				return new XElement(name, TextSerialization.ToStringInvariant(value));
			}

			// Token: 0x06001BC1 RID: 7105 RVA: 0x000764FC File Offset: 0x000746FC
			private static XElement CreateStatsField(string name, StatSheet statSheet)
			{
				XElement xelement = new XElement(name);
				for (int i = 0; i < statSheet.fields.Length; i++)
				{
					XElement xelement2 = new XElement("stat", new XText(statSheet.fields[i].ToString()));
					xelement2.SetAttributeValue("name", statSheet.fields[i].name);
					xelement.Add(xelement2);
				}
				int unlockableCount = statSheet.GetUnlockableCount();
				for (int j = 0; j < unlockableCount; j++)
				{
					UnlockableDef unlockable = statSheet.GetUnlockable(j);
					XElement content = new XElement("unlock", new XText(unlockable.name));
					xelement.Add(content);
				}
				return xelement;
			}

			// Token: 0x06001BC2 RID: 7106 RVA: 0x000765C3 File Offset: 0x000747C3
			private static XElement CreateLoadoutField(string name, Loadout loadout)
			{
				return loadout.ToXml(name);
			}

			// Token: 0x06001BC3 RID: 7107 RVA: 0x000765CC File Offset: 0x000747CC
			private static uint GetUintField(XElement container, string fieldName, uint defaultValue)
			{
				XElement xelement = container.Element(fieldName);
				if (xelement != null)
				{
					XNode firstNode = xelement.FirstNode;
					if (firstNode != null && firstNode.NodeType == XmlNodeType.Text)
					{
						uint result;
						if (!TextSerialization.TryParseInvariant(((XText)firstNode).Value, out result))
						{
							return defaultValue;
						}
						return result;
					}
				}
				return defaultValue;
			}

			// Token: 0x06001BC4 RID: 7108 RVA: 0x00076618 File Offset: 0x00074818
			private static string GetStringField(XElement container, string fieldName, string defaultValue)
			{
				XElement xelement = container.Element(fieldName);
				if (xelement != null)
				{
					XNode firstNode = xelement.FirstNode;
					if (firstNode != null && firstNode.NodeType == XmlNodeType.Text)
					{
						return ((XText)firstNode).Value;
					}
				}
				return defaultValue;
			}

			// Token: 0x06001BC5 RID: 7109 RVA: 0x00076658 File Offset: 0x00074858
			private static void GetStatsField(XElement container, string fieldName, StatSheet dest)
			{
				XElement xelement = container.Element(fieldName);
				if (xelement == null)
				{
					return;
				}
				foreach (XElement xelement2 in from element in xelement.Elements()
				where element.Name == "stat"
				select element)
				{
					XAttribute xattribute = xelement2.Attributes().FirstOrDefault((XAttribute attribute) => attribute.Name == "name");
					string statName = (xattribute != null) ? xattribute.Value : null;
					XText xtext = xelement2.Nodes().FirstOrDefault((XNode node) => node.NodeType == XmlNodeType.Text) as XText;
					string value = (xtext != null) ? xtext.Value : null;
					dest.SetStatValueFromString(StatDef.Find(statName), value);
				}
				foreach (XElement xelement3 in from element in xelement.Elements()
				where element.Name == "unlock"
				select element)
				{
					XText xtext2 = xelement3.Nodes().FirstOrDefault((XNode node) => node.NodeType == XmlNodeType.Text) as XText;
					UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef((xtext2 != null) ? xtext2.Value : null);
					if (unlockableDef != null)
					{
						dest.AddUnlockable(unlockableDef);
					}
				}
			}

			// Token: 0x06001BC6 RID: 7110 RVA: 0x000767FC File Offset: 0x000749FC
			private static void GetLoadoutField(XElement container, string fieldName, Loadout dest)
			{
				XElement xelement = container.Element(fieldName);
				if (xelement == null)
				{
					return;
				}
				Loadout loadout = new Loadout();
				if (!loadout.FromXml(xelement))
				{
					return;
				}
				loadout.Copy(dest);
			}

			// Token: 0x06001BC7 RID: 7111 RVA: 0x00076834 File Offset: 0x00074A34
			public static XDocument ToXml(UserProfile userProfile)
			{
				object[] array = new object[UserProfile.saveFields.Length];
				for (int i = 0; i < UserProfile.saveFields.Length; i++)
				{
					UserProfile.SaveFieldAttribute saveFieldAttribute = UserProfile.saveFields[i];
					array[i] = UserProfile.XmlUtility.CreateStringField(saveFieldAttribute.fieldName, saveFieldAttribute.getter(userProfile));
				}
				object[] element = new object[]
				{
					UserProfile.XmlUtility.CreateStatsField("stats", userProfile.statSheet),
					UserProfile.XmlUtility.CreateUintField("tutorialDifficulty", userProfile.tutorialDifficulty.showCount),
					UserProfile.XmlUtility.CreateUintField("tutorialEquipment", userProfile.tutorialEquipment.showCount),
					UserProfile.XmlUtility.CreateUintField("tutorialSprint", userProfile.tutorialSprint.showCount),
					UserProfile.XmlUtility.CreateLoadoutField("loadout", userProfile.loadout)
				};
				return new XDocument(new object[]
				{
					new XElement("UserProfile", array.Append(element).ToArray<object>())
				});
			}

			// Token: 0x06001BC8 RID: 7112 RVA: 0x00076920 File Offset: 0x00074B20
			public static UserProfile FromXml(XDocument doc)
			{
				UserProfile userProfile = new UserProfile();
				XElement root = doc.Root;
				foreach (UserProfile.SaveFieldAttribute saveFieldAttribute in UserProfile.saveFields)
				{
					string stringField = UserProfile.XmlUtility.GetStringField(root, saveFieldAttribute.fieldName, null);
					if (stringField != null)
					{
						saveFieldAttribute.setter(userProfile, stringField);
					}
				}
				UserProfile.XmlUtility.GetStatsField(root, "stats", userProfile.statSheet);
				UserProfile.XmlUtility.GetLoadoutField(root, "loadout", userProfile.loadout);
				userProfile.tutorialDifficulty.showCount = UserProfile.XmlUtility.GetUintField(root, "tutorialDifficulty", userProfile.tutorialDifficulty.showCount);
				userProfile.tutorialEquipment.showCount = UserProfile.XmlUtility.GetUintField(root, "tutorialEquipment", userProfile.tutorialEquipment.showCount);
				userProfile.tutorialSprint.showCount = UserProfile.XmlUtility.GetUintField(root, "tutorialSprint", userProfile.tutorialSprint.showCount);
				return userProfile;
			}
		}

		// Token: 0x02000478 RID: 1144
		private struct LoadUserProfileOperationResult
		{
			// Token: 0x040018FF RID: 6399
			public string fileName;

			// Token: 0x04001900 RID: 6400
			public long fileLength;

			// Token: 0x04001901 RID: 6401
			public UserProfile userProfile;

			// Token: 0x04001902 RID: 6402
			public Exception exception;

			// Token: 0x04001903 RID: 6403
			public string failureContents;
		}
	}
}
