using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200034D RID: 845
	public class TeamManager : NetworkBehaviour
	{
		// Token: 0x06001439 RID: 5177 RVA: 0x00056518 File Offset: 0x00054718
		static TeamManager()
		{
			List<ulong> list = new List<ulong>();
			list.Add(0UL);
			list.Add(0UL);
			TeamManager.naturalLevelCap = 2U;
			for (;;)
			{
				ulong num = (ulong)TeamManager.InitialCalcExperience(TeamManager.naturalLevelCap, 20.0, 1.55);
				if (num <= list[list.Count - 1])
				{
					break;
				}
				list.Add(num);
				TeamManager.naturalLevelCap += 1U;
			}
			TeamManager.naturalLevelCap -= 1U;
			TeamManager.levelToExperienceTable = list.ToArray();
			TeamManager.hardExpCap = TeamManager.levelToExperienceTable[TeamManager.levelToExperienceTable.Length - 1];
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x000565B4 File Offset: 0x000547B4
		private static double InitialCalcLevel(double experience, double experienceForFirstLevelUp = 20.0, double growthRate = 1.55)
		{
			return Math.Max(Math.Log(1.0 - experience / experienceForFirstLevelUp * (1.0 - growthRate), growthRate) + 1.0, 1.0);
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x000565ED File Offset: 0x000547ED
		private static double InitialCalcExperience(double level, double experienceForFirstLevelUp = 20.0, double growthRate = 1.55)
		{
			return Math.Max(experienceForFirstLevelUp * ((1.0 - Math.Pow(growthRate, level - 1.0)) / (1.0 - growthRate)), 0.0);
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x00056628 File Offset: 0x00054828
		private static uint FindLevelForExperience(ulong experience)
		{
			uint num = 1U;
			while ((ulong)num < (ulong)((long)TeamManager.levelToExperienceTable.Length))
			{
				if (TeamManager.levelToExperienceTable[(int)num] > experience)
				{
					return num - 1U;
				}
				num += 1U;
			}
			return TeamManager.naturalLevelCap;
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x0005665C File Offset: 0x0005485C
		private static ulong GetExperienceForLevel(uint level)
		{
			if (level > TeamManager.naturalLevelCap)
			{
				level = TeamManager.naturalLevelCap;
			}
			return TeamManager.levelToExperienceTable[(int)level];
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600143F RID: 5183 RVA: 0x0005667C File Offset: 0x0005487C
		// (set) Token: 0x0600143E RID: 5182 RVA: 0x00056674 File Offset: 0x00054874
		public static TeamManager instance { get; private set; }

		// Token: 0x06001440 RID: 5184 RVA: 0x00056683 File Offset: 0x00054883
		private void OnEnable()
		{
			TeamManager.instance = SingletonHelper.Assign<TeamManager>(TeamManager.instance, this);
		}

		// Token: 0x06001441 RID: 5185 RVA: 0x00056695 File Offset: 0x00054895
		private void OnDisable()
		{
			TeamManager.instance = SingletonHelper.Unassign<TeamManager>(TeamManager.instance, this);
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x000566A8 File Offset: 0x000548A8
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

		// Token: 0x06001443 RID: 5187 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x000566D4 File Offset: 0x000548D4
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

		// Token: 0x06001445 RID: 5189 RVA: 0x00056770 File Offset: 0x00054970
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

		// Token: 0x06001446 RID: 5190 RVA: 0x000567B0 File Offset: 0x000549B0
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
				this.teamNextLevelExperience[(int)teamIndex] = TeamManager.GetExperienceForLevel(num2 + 1U);
				if (num < num2)
				{
					GlobalEventManager.OnTeamLevelUp(teamIndex);
				}
			}
			if (NetworkServer.active)
			{
				base.SetDirtyBit(1U << (int)teamIndex);
			}
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x00056864 File Offset: 0x00054A64
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = initialState ? 7U : base.syncVarDirtyBits;
			writer.WritePackedUInt32(num);
			if (num == 0U)
			{
				return false;
			}
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				if ((num & 1U << (int)teamIndex) != 0U)
				{
					writer.WritePackedUInt64(this.teamExperience[(int)teamIndex]);
				}
			}
			return true;
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x000568B0 File Offset: 0x00054AB0
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			uint num = reader.ReadPackedUInt32();
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				if ((num & 1U << (int)teamIndex) != 0U)
				{
					ulong newExperience = reader.ReadPackedUInt64();
					this.SetTeamExperience(teamIndex, newExperience);
				}
			}
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x000568EA File Offset: 0x00054AEA
		public ulong GetTeamExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamExperience[(int)teamIndex];
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x000568FF File Offset: 0x00054AFF
		public ulong GetTeamCurrentLevelExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamCurrentLevelExperience[(int)teamIndex];
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x00056914 File Offset: 0x00054B14
		public ulong GetTeamNextLevelExperience(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0UL;
			}
			return this.teamNextLevelExperience[(int)teamIndex];
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x00056929 File Offset: 0x00054B29
		public uint GetTeamLevel(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return 0U;
			}
			return this.teamLevels[(int)teamIndex];
		}

		// Token: 0x0600144D RID: 5197 RVA: 0x0005693D File Offset: 0x00054B3D
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

		// Token: 0x0600144E RID: 5198 RVA: 0x00056960 File Offset: 0x00054B60
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

		// Token: 0x0600144F RID: 5199 RVA: 0x0005698F File Offset: 0x00054B8F
		public static string GetTeamLevelUpSoundString(TeamIndex teamIndex)
		{
			switch (teamIndex)
			{
			case TeamIndex.Neutral:
				return null;
			case TeamIndex.Player:
				return "Play_UI_levelUp_player";
			case TeamIndex.Monster:
				return "Play_UI_levelUp_enemy";
			default:
				return null;
			}
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x000569B4 File Offset: 0x00054BB4
		public static bool IsTeamEnemy(TeamIndex teamA, TeamIndex teamB)
		{
			return teamA != teamB;
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x000569C0 File Offset: 0x00054BC0
		[ConCommand(commandName = "team_set_level", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Sets the team specified by the first argument to the level specified by the second argument.")]
		private static void CCTeamSetLevel(ConCommandArgs args)
		{
			TeamIndex argEnum = args.GetArgEnum<TeamIndex>(0);
			ulong argULong = args.GetArgULong(1);
			if (!TeamManager.instance)
			{
				throw new ConCommandException("No TeamManager exists.");
			}
			TeamManager.instance.SetTeamLevel(argEnum, (uint)argULong);
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x040012F4 RID: 4852
		public static readonly uint naturalLevelCap;

		// Token: 0x040012F5 RID: 4853
		private static readonly ulong[] levelToExperienceTable;

		// Token: 0x040012F6 RID: 4854
		public static readonly ulong hardExpCap;

		// Token: 0x040012F8 RID: 4856
		private ulong[] teamExperience = new ulong[3];

		// Token: 0x040012F9 RID: 4857
		private uint[] teamLevels = new uint[3];

		// Token: 0x040012FA RID: 4858
		private ulong[] teamCurrentLevelExperience = new ulong[3];

		// Token: 0x040012FB RID: 4859
		private ulong[] teamNextLevelExperience = new ulong[3];

		// Token: 0x040012FC RID: 4860
		private const uint teamExperienceDirtyBitsMask = 7U;
	}
}
