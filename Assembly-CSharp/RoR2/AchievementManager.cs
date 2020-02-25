using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using RoR2.Achievements;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000B1 RID: 177
	public static class AchievementManager
	{
		// Token: 0x06000385 RID: 901 RVA: 0x0000D9E0 File Offset: 0x0000BBE0
		public static UserAchievementManager GetUserAchievementManager([NotNull] LocalUser user)
		{
			UserAchievementManager result;
			AchievementManager.userToManagerMap.TryGetValue(user, out result);
			return result;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0000D9FC File Offset: 0x0000BBFC
		[SystemInitializer(new Type[]
		{
			typeof(UnlockableCatalog)
		})]
		private static void DoInit()
		{
			AchievementManager.CollectAchievementDefs(AchievementManager.achievementNamesToDefs);
			LocalUserManager.onUserSignIn += delegate(LocalUser localUser)
			{
				if (!localUser.userProfile.canSave)
				{
					return;
				}
				UserAchievementManager userAchievementManager = new UserAchievementManager();
				userAchievementManager.OnInstall(localUser);
				AchievementManager.userToManagerMap[localUser] = userAchievementManager;
			};
			LocalUserManager.onUserSignOut += delegate(LocalUser localUser)
			{
				UserAchievementManager userAchievementManager;
				if (AchievementManager.userToManagerMap.TryGetValue(localUser, out userAchievementManager))
				{
					userAchievementManager.OnUninstall();
					AchievementManager.userToManagerMap.Remove(localUser);
				}
			};
			RoR2Application.onUpdate += delegate()
			{
				foreach (KeyValuePair<LocalUser, UserAchievementManager> keyValuePair in AchievementManager.userToManagerMap)
				{
					keyValuePair.Value.Update();
				}
			};
			AchievementManager.availability.MakeAvailable();
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0000DA89 File Offset: 0x0000BC89
		public static void AddTask(Action action)
		{
			AchievementManager.taskQueue.Enqueue(action);
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0000DA96 File Offset: 0x0000BC96
		public static void ProcessTasks()
		{
			while (AchievementManager.taskQueue.Count > 0)
			{
				AchievementManager.taskQueue.Dequeue()();
			}
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0000DAB8 File Offset: 0x0000BCB8
		public static AchievementDef GetAchievementDef(string achievementIdentifier)
		{
			AchievementDef result;
			if (AchievementManager.achievementNamesToDefs.TryGetValue(achievementIdentifier, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0000DAD7 File Offset: 0x0000BCD7
		public static AchievementDef GetAchievementDef(AchievementIndex index)
		{
			if (index.intValue >= 0 && index.intValue < AchievementManager.achievementDefs.Length)
			{
				return AchievementManager.achievementDefs[index.intValue];
			}
			return null;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0000DAFF File Offset: 0x0000BCFF
		public static AchievementDef GetAchievementDef(ServerAchievementIndex index)
		{
			if (index.intValue >= 0 && index.intValue < AchievementManager.serverAchievementDefs.Length)
			{
				return AchievementManager.serverAchievementDefs[index.intValue];
			}
			return null;
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0000DB28 File Offset: 0x0000BD28
		public static AchievementDef GetAchievementDefFromUnlockable(string unlockableRewardIdentifier)
		{
			for (int i = 0; i < AchievementManager.achievementDefs.Length; i++)
			{
				if (AchievementManager.achievementDefs[i].unlockableRewardIdentifier == unlockableRewardIdentifier)
				{
					return AchievementManager.achievementDefs[i];
				}
			}
			return null;
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600038D RID: 909 RVA: 0x0000DB64 File Offset: 0x0000BD64
		public static int achievementCount
		{
			get
			{
				return AchievementManager.achievementDefs.Length;
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600038E RID: 910 RVA: 0x0000DB6D File Offset: 0x0000BD6D
		public static int serverAchievementCount
		{
			get
			{
				return AchievementManager.serverAchievementDefs.Length;
			}
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0000DB78 File Offset: 0x0000BD78
		public static void CollectAchievementDefs(Dictionary<string, AchievementDef> map)
		{
			List<AchievementDef> list = new List<AchievementDef>();
			map.Clear();
			foreach (Type type2 in from type in typeof(BaseAchievement).Assembly.GetTypes()
			where type.IsSubclassOf(typeof(BaseAchievement))
			orderby type.Name
			select type)
			{
				RegisterAchievementAttribute registerAchievementAttribute = (RegisterAchievementAttribute)type2.GetCustomAttributes(false).FirstOrDefault((object v) => v is RegisterAchievementAttribute);
				if (registerAchievementAttribute != null)
				{
					if (map.ContainsKey(registerAchievementAttribute.identifier))
					{
						Debug.LogErrorFormat("Class {0} attempted to register as achievement {1}, but class {2} has already registered as that achievement.", new object[]
						{
							type2.FullName,
							registerAchievementAttribute.identifier,
							AchievementManager.achievementNamesToDefs[registerAchievementAttribute.identifier].type.FullName
						});
					}
					else
					{
						AchievementDef achievementDef = new AchievementDef
						{
							identifier = registerAchievementAttribute.identifier,
							unlockableRewardIdentifier = registerAchievementAttribute.unlockableRewardIdentifier,
							prerequisiteAchievementIdentifier = registerAchievementAttribute.prerequisiteAchievementIdentifier,
							nameToken = "ACHIEVEMENT_" + registerAchievementAttribute.identifier.ToUpper(CultureInfo.InvariantCulture) + "_NAME",
							descriptionToken = "ACHIEVEMENT_" + registerAchievementAttribute.identifier.ToUpper(CultureInfo.InvariantCulture) + "_DESCRIPTION",
							iconPath = "Textures/AchievementIcons/tex" + registerAchievementAttribute.identifier + "Icon",
							type = type2,
							serverTrackerType = registerAchievementAttribute.serverTrackerType
						};
						AchievementManager.achievementIdentifiers.Add(registerAchievementAttribute.identifier);
						map.Add(registerAchievementAttribute.identifier, achievementDef);
						list.Add(achievementDef);
						UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(achievementDef.unlockableRewardIdentifier);
						if (unlockableDef != null)
						{
							unlockableDef.getHowToUnlockString = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
							{
								Language.GetString(achievementDef.nameToken),
								Language.GetString(achievementDef.descriptionToken)
							}));
							unlockableDef.getUnlockedString = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
							{
								unlockableDef.getHowToUnlockString()
							}));
						}
					}
				}
			}
			AchievementManager.achievementDefs = list.ToArray();
			AchievementManager.SortAchievements(AchievementManager.achievementDefs);
			AchievementManager.serverAchievementDefs = (from achievementDef in AchievementManager.achievementDefs
			where achievementDef.serverTrackerType != null
			select achievementDef).ToArray<AchievementDef>();
			for (int i = 0; i < AchievementManager.achievementDefs.Length; i++)
			{
				AchievementManager.achievementDefs[i].index = new AchievementIndex
				{
					intValue = i
				};
			}
			for (int j = 0; j < AchievementManager.serverAchievementDefs.Length; j++)
			{
				AchievementManager.serverAchievementDefs[j].serverIndex = new ServerAchievementIndex
				{
					intValue = j
				};
			}
			for (int k = 0; k < AchievementManager.achievementIdentifiers.Count; k++)
			{
				string currentAchievementIdentifier = AchievementManager.achievementIdentifiers[k];
				map[currentAchievementIdentifier].childAchievementIdentifiers = (from v in AchievementManager.achievementIdentifiers
				where map[v].prerequisiteAchievementIdentifier == currentAchievementIdentifier
				select v).ToArray<string>();
			}
			Action action = AchievementManager.onAchievementsRegistered;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0000DF50 File Offset: 0x0000C150
		private static void SortAchievements(AchievementDef[] achievementDefsArray)
		{
			AchievementManager.AchievementSortPair[] array = new AchievementManager.AchievementSortPair[achievementDefsArray.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new AchievementManager.AchievementSortPair
				{
					score = UnlockableCatalog.GetUnlockableSortScore(achievementDefsArray[i].unlockableRewardIdentifier),
					achievementDef = achievementDefsArray[i]
				};
			}
			Array.Sort<AchievementManager.AchievementSortPair>(array, (AchievementManager.AchievementSortPair a, AchievementManager.AchievementSortPair b) => a.score - b.score);
			for (int j = 0; j < array.Length; j++)
			{
				achievementDefsArray[j] = array[j].achievementDef;
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000391 RID: 913 RVA: 0x0000DFE4 File Offset: 0x0000C1E4
		// (remove) Token: 0x06000392 RID: 914 RVA: 0x0000E018 File Offset: 0x0000C218
		public static event Action onAchievementsRegistered;

		// Token: 0x06000393 RID: 915 RVA: 0x0000E04C File Offset: 0x0000C24C
		public static AchievementManager.Enumerator GetEnumerator()
		{
			return default(AchievementManager.Enumerator);
		}

		// Token: 0x04000306 RID: 774
		private static readonly Dictionary<LocalUser, UserAchievementManager> userToManagerMap = new Dictionary<LocalUser, UserAchievementManager>();

		// Token: 0x04000307 RID: 775
		public static ResourceAvailability availability;

		// Token: 0x04000308 RID: 776
		private static readonly Queue<Action> taskQueue = new Queue<Action>();

		// Token: 0x04000309 RID: 777
		private static readonly Dictionary<string, AchievementDef> achievementNamesToDefs = new Dictionary<string, AchievementDef>();

		// Token: 0x0400030A RID: 778
		private static readonly List<string> achievementIdentifiers = new List<string>();

		// Token: 0x0400030B RID: 779
		public static readonly ReadOnlyCollection<string> readOnlyAchievementIdentifiers = AchievementManager.achievementIdentifiers.AsReadOnly();

		// Token: 0x0400030C RID: 780
		private static AchievementDef[] achievementDefs;

		// Token: 0x0400030D RID: 781
		private static AchievementDef[] serverAchievementDefs;

		// Token: 0x0400030F RID: 783
		public static readonly GenericStaticEnumerable<AchievementDef, AchievementManager.Enumerator> allAchievementDefs;

		// Token: 0x020000B2 RID: 178
		private struct AchievementSortPair
		{
			// Token: 0x04000310 RID: 784
			public int score;

			// Token: 0x04000311 RID: 785
			public AchievementDef achievementDef;
		}

		// Token: 0x020000B3 RID: 179
		public struct Enumerator : IEnumerator<AchievementDef>, IEnumerator, IDisposable
		{
			// Token: 0x06000395 RID: 917 RVA: 0x0000E09B File Offset: 0x0000C29B
			public bool MoveNext()
			{
				this.position++;
				return this.position < AchievementManager.achievementDefs.Length;
			}

			// Token: 0x06000396 RID: 918 RVA: 0x0000E0BA File Offset: 0x0000C2BA
			public void Reset()
			{
				this.position = -1;
			}

			// Token: 0x1700007F RID: 127
			// (get) Token: 0x06000397 RID: 919 RVA: 0x0000E0C3 File Offset: 0x0000C2C3
			public AchievementDef Current
			{
				get
				{
					return AchievementManager.achievementDefs[this.position];
				}
			}

			// Token: 0x17000080 RID: 128
			// (get) Token: 0x06000398 RID: 920 RVA: 0x0000E0D1 File Offset: 0x0000C2D1
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x06000399 RID: 921 RVA: 0x0000409B File Offset: 0x0000229B
			void IDisposable.Dispose()
			{
			}

			// Token: 0x04000312 RID: 786
			private int position;
		}
	}
}
