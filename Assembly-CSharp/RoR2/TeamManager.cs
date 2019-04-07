using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F6 RID: 1014
	public class TeamManager : NetworkBehaviour
	{
		// Token: 0x06001642 RID: 5698 RVA: 0x0006A638 File Offset: 0x00068838
		static TeamManager()
		{
			List<ulong> list = new List<ulong>();
			list.Add(0UL);
			list.Add(0UL);
			TeamManager.naturalLevelCap = 2u;
			for (;;)
			{
				ulong num = (ulong)TeamManager.InitialCalcExperience(TeamManager.naturalLevelCap, 20.0, 1.55);
				if (num <= list[list.Count - 1])
				{
					break;
				}
				list.Add(num);
				TeamManager.naturalLevelCap += 1u;
			}
			TeamManager.naturalLevelCap -= 1u;
			TeamManager.levelToExperienceTable = list.ToArray();
			TeamManager.hardExpCap = TeamManager.levelToExperienceTable[TeamManager.levelToExperienceTable.Length - 1];
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x0006A6D4 File Offset: 0x000688D4
		private static double InitialCalcLevel(double experience, double experienceForFirstLevelUp = 20.0, double growthRate = 1.55)
		{
			return Math.Max(Math.Log(1.0 - experience / experienceForFirstLevelUp * (1.0 - growthRate), growthRate) + 1.0, 1.0);
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x0006A70D File Offset: 0x0006890D
		private static double InitialCalcExperience(double level, double experienceForFirstLevelUp = 20.0, double growthRate = 1.55)
		{
			return Math.Max(experienceForFirstLevelUp * ((1.0 - Math.Pow(growthRate, level - 1.0)) / (1.0 - growthRate)), 0.0);
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x0006A748 File Offset: 0x00068948
		private static uint FindLevelForExperience(ulong experience)
		{
			uint num = 1u;
			while ((ulong)num < (ulong)((long)TeamManager.levelToExperienceTable.Length))
			{
				if (TeamManager.levelToExperienceTable[(int)num] > experience)
				{
					return num - 1u;
				}
				num += 1u;
			}
			return TeamManager.naturalLevelCap;
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0006A77C File Offset: 0x0006897C
		private static ulong GetExperienceForLevel(uint level)
		{
			if (level > TeamManager.naturalLevelCap)
			{
				level = TeamManager.naturalLevelCap;
			}
			return TeamManager.levelToExperienceTable[(int)level];
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06001648 RID: 5704 RVA: 0x0006A79C File Offset: 0x0006899C
		// (set) Token: 0x06001647 RID: 5703 RVA: 0x0006A794 File Offset: 0x00068994
		public static TeamManager instance { get; private set; }

		// Token: 0x06001649 RID: 5705 RVA: 0x0006A7A3 File Offset: 0x000689A3
		private void OnEnable()
		{
			if (TeamManager.instance)
			{
				Debug.LogErrorFormat(this, "Only one {0} may exist at a time.", new object[]
				{
					typeof(TeamManager).Name
				});
				return;
			}
			TeamManager.instance = this;
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x0006A7DB File Offset: 0x000689DB
		private void OnDisable()
		{
			if (TeamManager.instance == this)
			{
				TeamManager.instance = null;
			}
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x0006A7F0 File Offset: 0x000689F0
		private void Start()
		{
			if (NetworkServer.active)
			{
				for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
				{
					this.SetTeamExperience(teamIndex, 0UL);
				}
			}
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x00037FB6 File Offset: 0x000361B6
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x0006A81C File Offset: 0x00068A1C
		[Server]
		public void GiveTeamMoney(TeamIndex teamIndex, uint money)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeamManager::GiveTeamMoney(RoR2.TeamIndex,System.UInt32)' called on client");
				return;
			}
			int num = Run.instance ? Run.instance.livingPlayerCount : 0;
			if (num != 0)
			{
				money = (uint)Mathf.CeilToInt(money / (float)num);
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
				if (component && component.isPlayerControlled)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						master.GiveMoney(money);
					}
				}
			}
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x0006A8B8 File Offset: 0x00068AB8
		[Server]
		public void GiveTeamExperience(TeamIndex teamIndex, ulong experience)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.TeamManager::GiveTeamExperience(RoR2.TeamIndex,System.UInt64)' called on client");
				return;
			}
			ulong num = this.teamExperience[(int)teamIndex];
			ulong num2 = num + experience;
			if (num2 < num)
			{
				num2 = ulong.MaxValue;
			}
			this.SetTeamExperience(teamIndex, num2);
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x0006A8F8 File Offset: 0x00068AF8
		private void SetTeamExperience(TeamIndex teamIndex, ulong newExperience)
		{
			if (newExperience > TeamManager.hardExpCap)
			{
				newExperience = TeamManager.hardExpCap;
			}
			this.teamExperience[(int)teamIndex] = newExperience;
			uint num = this.teamLevels[(int)teamIndex];
			uint num2 = TeamManager.FindLevelForExperience(newExperience);
			if (num != num2)
			{
				ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
				for (int i = 0; i < teamMembers.Count; i++)
				{
					CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
					if (component)
					{
						component.OnLevelChanged();
					}
				}
				this.teamLevels[(int)teamIndex] = num2;
				this.teamCurrentLevelExperience[(int)teamIndex] = TeamManager.GetExperienceForLevel(num2);
				this.teamNextLevelExperience[(int)teamIndex] = TeamManager.GetExperienceForLevel(num2 + 1u);
				if (num < num2)
				{
					GlobalEventManager.OnTeamLevelUp(teamIndex);
				}
			}
			if (NetworkServer.active)
			{
				base.SetDirtyBit(1u << (int)teamIndex);
			}
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x0006A9AC File Offset: 0x00068BAC
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = initialState ? 7u : base.syncVarDirtyBits;
			writer.WritePackedUInt32(num);
			if (num == 0u)
			{
				return false;
			}
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				if ((num & 1u << (int)teamIndex) != 0u)
				{
					writer.WritePackedUInt64(this.teamExperience[(int)teamIndex]);
				}
			}
			return true;
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x0006A9F8 File Offset: 0x00068BF8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			uint num = reader.ReadPackedUInt32();
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				if ((num & 1u << (int)teamIndex) != 0u)
				{
					ulong newExperience = reader.ReadPackedUInt64();
					this.SetTeamExperience(teamIndex, newExperience);
				}
			}
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x0006AA32 File Offset: 0x00068C32
		public ulong GetTeamExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamExperience[(int)teamIndex];
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x0006AA47 File Offset: 0x00068C47
		public ulong GetTeamCurrentLevelExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamCurrentLevelExperience[(int)teamIndex];
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x0006AA5C File Offset: 0x00068C5C
		public ulong GetTeamNextLevelExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamNextLevelExperience[(int)teamIndex];
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x0006AA71 File Offset: 0x00068C71
		public uint GetTeamLevel(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0u;
			}
			return this.teamLevels[(int)teamIndex];
		}

		// Token: 0x06001656 RID: 5718 RVA: 0x0006AA85 File Offset: 0x00068C85
		public void SetTeamLevel(TeamIndex teamIndex, uint newLevel)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			if (this.GetTeamLevel(teamIndex) == newLevel)
			{
				return;
			}
			this.SetTeamExperience(teamIndex, TeamManager.GetExperienceForLevel(newLevel));
		}

		// Token: 0x06001657 RID: 5719 RVA: 0x0006AAA8 File Offset: 0x00068CA8
		public static GameObject GetTeamLevelUpEffect(TeamIndex teamIndex)
		{
			switch (teamIndex)
			{
			case TeamIndex.Neutral:
				return null;
			case TeamIndex.Player:
				return Resources.Load<GameObject>("Prefabs/Effects/LevelUpEffect");
			case TeamIndex.Monster:
				return Resources.Load<GameObject>("Prefabs/Effects/LevelUpEffectEnemy");
			default:
				return null;
			}
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x0006AAD7 File Offset: 0x00068CD7
		public static bool IsTeamEnemy(TeamIndex teamA, TeamIndex teamB)
		{
			return teamA != teamB;
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x04001995 RID: 6549
		public static readonly uint naturalLevelCap;

		// Token: 0x04001996 RID: 6550
		private static readonly ulong[] levelToExperienceTable;

		// Token: 0x04001997 RID: 6551
		public static readonly ulong hardExpCap;

		// Token: 0x04001999 RID: 6553
		private ulong[] teamExperience = new ulong[3];

		// Token: 0x0400199A RID: 6554
		private uint[] teamLevels = new uint[3];

		// Token: 0x0400199B RID: 6555
		private ulong[] teamCurrentLevelExperience = new ulong[3];

		// Token: 0x0400199C RID: 6556
		private ulong[] teamNextLevelExperience = new ulong[3];

		// Token: 0x0400199D RID: 6557
		private const uint teamExperienceDirtyBitsMask = 7u;
	}
}
