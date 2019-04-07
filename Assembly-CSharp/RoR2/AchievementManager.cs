using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using RoR2.Achievements;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F0 RID: 496
	public static class AchievementManager
	{
		// Token: 0x060009C0 RID: 2496 RVA: 0x00030D60 File Offset: 0x0002EF60
		public static UserAchievementManager GetUserAchievementManager([NotNull] LocalUser user)
		{
			UserAchievementManager result;
			AchievementManager.userToManagerMap.TryGetValue(user, out result);
			return result;
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x00030D7C File Offset: 0x0002EF7C
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

		// Token: 0x060009C2 RID: 2498 RVA: 0x00030E09 File Offset: 0x0002F009
		public static void AddTask(Action action)
		{
			AchievementManager.taskQueue.Enqueue(action);
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x00030E16 File Offset: 0x0002F016
		public static void ProcessTasks()
		{
			while (AchievementManager.taskQueue.Count > 0)
			{
				AchievementManager.taskQueue.Dequeue()();
			}
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00030E38 File Offset: 0x0002F038
		public static AchievementDef GetAchievementDef(string achievementIdentifier)
		{
			AchievementDef result;
			if (AchievementManager.achievementNamesToDefs.TryGetValue(achievementIdentifier, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x00030E57 File Offset: 0x0002F057
		public static AchievementDef GetAchievementDef(AchievementIndex index)
		{
			if (index.intValue >= 0 && index.intValue < AchievementManager.achievementDefs.Length)
			{
				return AchievementManager.achievementDefs[index.intValue];
			}
			return null;
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x00030E7F File Offset: 0x0002F07F
		public static AchievementDef GetAchievementDef(ServerAchievementIndex index)
		{
			if (index.intValue >= 0 && index.intValue < AchievementManager.serverAchievementDefs.Length)
			{
				return AchievementManager.serverAchievementDefs[index.intValue];
			}
			return null;
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x00030EA8 File Offset: 0x0002F0A8
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

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060009C8 RID: 2504 RVA: 0x00030EE4 File Offset: 0x0002F0E4
		public static int achievementCount
		{
			get
			{
				return AchievementManager.achievementDefs.Length;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060009C9 RID: 2505 RVA: 0x00030EED File Offset: 0x0002F0ED
		public static int serverAchievementCount
		{
			get
			{
				return AchievementManager.serverAchievementDefs.Length;
			}
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x00030EF8 File Offset: 0x0002F0F8
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
							nameToken = "ACHIEVEMENT_" + registerAchievementAttribute.identifier.ToUpper() + "_NAME",
							descriptionToken = "ACHIEVEMENT_" + registerAchievementAttribute.identifier.ToUpper() + "_DESCRIPTION",
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

		// Token: 0x060009CB RID: 2507 RVA: 0x000312C4 File Offset: 0x0002F4C4
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

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060009CC RID: 2508 RVA: 0x00031358 File Offset: 0x0002F558
		// (remove) Token: 0x060009CD RID: 2509 RVA: 0x0003138C File Offset: 0x0002F58C
		public static event Action onAchievementsRegistered;

		// Token: 0x060009CE RID: 2510 RVA: 0x000313C0 File Offset: 0x0002F5C0
		public static AchievementManager.Enumerator GetEnumerator()
		{
			return default(AchievementManager.Enumerator);
		}

		// Token: 0x04000CF7 RID: 3319
		private static readonly Dictionary<LocalUser, UserAchievementManager> userToManagerMap = new Dictionary<LocalUser, UserAchievementManager>();

		// Token: 0x04000CF8 RID: 3320
		public static ResourceAvailability availability;

		// Token: 0x04000CF9 RID: 3321
		private static readonly Queue<Action> taskQueue = new Queue<Action>();

		// Token: 0x04000CFA RID: 3322
		private static readonly Dictionary<string, AchievementDef> achievementNamesToDefs = new Dictionary<string, AchievementDef>();

		// Token: 0x04000CFB RID: 3323
		private static readonly List<string> achievementIdentifiers = new List<string>();

		// Token: 0x04000CFC RID: 3324
		public static readonly ReadOnlyCollection<string> readOnlyAchievementIdentifiers = AchievementManager.achievementIdentifiers.AsReadOnly();

		// Token: 0x04000CFD RID: 3325
		private static AchievementDef[] achievementDefs;

		// Token: 0x04000CFE RID: 3326
		private static AchievementDef[] serverAchievementDefs;

		// Token: 0x04000D00 RID: 3328
		public static readonly GenericStaticEnumerable<AchievementDef, AchievementManager.Enumerator> allAchievementDefs;

		// Token: 0x020001F1 RID: 497
		private struct AchievementSortPair
		{
			// Token: 0x04000D01 RID: 3329
			public int score;

			// Token: 0x04000D02 RID: 3330
			public AchievementDef achievementDef;
		}

		// Token: 0x020001F2 RID: 498
		public struct Enumerator : IEnumerator<AchievementDef>, IEnumerator, IDisposable
		{
			// Token: 0x060009D0 RID: 2512 RVA: 0x0003140F File Offset: 0x0002F60F
			public bool MoveNext()
			{
				this.position++;
				return this.position < AchievementManager.achievementDefs.Length;
			}

			// Token: 0x060009D1 RID: 2513 RVA: 0x0003142E File Offset: 0x0002F62E
			public void Reset()
			{
				this.position = -1;
			}

			// Token: 0x170000A7 RID: 167
			// (get) Token: 0x060009D2 RID: 2514 RVA: 0x00031437 File Offset: 0x0002F637
			public AchievementDef Current
			{
				get
				{
					return AchievementManager.achievementDefs[this.position];
				}
			}

			// Token: 0x170000A8 RID: 168
			// (get) Token: 0x060009D3 RID: 2515 RVA: 0x00031445 File Offset: 0x0002F645
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x060009D4 RID: 2516 RVA: 0x00004507 File Offset: 0x00002707
			void IDisposable.Dispose()
			{
			}

			// Token: 0x04000D03 RID: 3331
			private int position;
		}
	}
}
