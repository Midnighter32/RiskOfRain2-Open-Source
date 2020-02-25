using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000322 RID: 802
	[RequireComponent(typeof(CombatSquad))]
	public class ScriptedCombatEncounter : MonoBehaviour
	{
		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060012D5 RID: 4821 RVA: 0x00050E7A File Offset: 0x0004F07A
		// (set) Token: 0x060012D6 RID: 4822 RVA: 0x00050E82 File Offset: 0x0004F082
		public CombatSquad combatSquad { get; private set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x060012D7 RID: 4823 RVA: 0x00050E8B File Offset: 0x0004F08B
		// (set) Token: 0x060012D8 RID: 4824 RVA: 0x00050E93 File Offset: 0x0004F093
		public bool hasSpawnedServer { get; private set; }

		// Token: 0x060012D9 RID: 4825 RVA: 0x00050E9C File Offset: 0x0004F09C
		private void Awake()
		{
			this.combatSquad = base.GetComponent<CombatSquad>();
			this.hasSpawnedServer = false;
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x00050EB4 File Offset: 0x0004F0B4
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(this.randomizeSeed ? Run.instance.stageRng.nextUlong : this.seed);
				if (this.spawnOnStart)
				{
					this.BeginEncounter();
				}
			}
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x00050F00 File Offset: 0x0004F100
		private void Spawn(ref ScriptedCombatEncounter.SpawnInfo spawnInfo)
		{
			Vector3 position = base.transform.position;
			DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
			{
				placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
				minDistance = 0f,
				maxDistance = 1000f,
				position = position
			};
			if (spawnInfo.explicitSpawnPosition)
			{
				directorPlacementRule.placementMode = DirectorPlacementRule.PlacementMode.Direct;
				directorPlacementRule.spawnOnTarget = spawnInfo.explicitSpawnPosition;
			}
			DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnInfo.spawnCard, directorPlacementRule, this.rng);
			directorSpawnRequest.ignoreTeamMemberLimit = true;
			directorSpawnRequest.teamIndexOverride = new TeamIndex?(this.teamIndex);
			GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
			if (gameObject)
			{
				this.hasSpawnedServer = true;
				float num = 1f;
				float num2 = 1f;
				num += Run.instance.difficultyCoefficient / 2.5f;
				num2 += Run.instance.difficultyCoefficient / 30f;
				int livingPlayerCount = Run.instance.livingPlayerCount;
				num *= Mathf.Pow((float)livingPlayerCount, 0.5f);
				CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
				component.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num - 1f) * 10f));
				component.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num2 - 1f) * 10f));
				this.combatSquad.AddMember(component);
			}
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0005105C File Offset: 0x0004F25C
		public void BeginEncounter()
		{
			if (this.hasSpawnedServer || !NetworkServer.active)
			{
				return;
			}
			for (int i = 0; i < this.spawns.Length; i++)
			{
				ref ScriptedCombatEncounter.SpawnInfo spawnInfo = ref this.spawns[i];
				this.Spawn(ref spawnInfo);
			}
		}

		// Token: 0x040011B6 RID: 4534
		public ulong seed;

		// Token: 0x040011B7 RID: 4535
		public bool randomizeSeed;

		// Token: 0x040011B8 RID: 4536
		public TeamIndex teamIndex;

		// Token: 0x040011B9 RID: 4537
		public ScriptedCombatEncounter.SpawnInfo[] spawns;

		// Token: 0x040011BA RID: 4538
		public bool spawnOnStart;

		// Token: 0x040011BD RID: 4541
		private Xoroshiro128Plus rng;

		// Token: 0x02000323 RID: 803
		[Serializable]
		public struct SpawnInfo
		{
			// Token: 0x040011BE RID: 4542
			public SpawnCard spawnCard;

			// Token: 0x040011BF RID: 4543
			public Transform explicitSpawnPosition;
		}
	}
}
