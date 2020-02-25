using System;
using System.Collections.Generic;
using EntityStates;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000164 RID: 356
	[RequireComponent(typeof(CombatSquad))]
	public class BossGroup : MonoBehaviour
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x0001AB77 File Offset: 0x00018D77
		public float fixedAge
		{
			get
			{
				return this.combatSquad.awakeTime.timeSince;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000685 RID: 1669 RVA: 0x0001AB89 File Offset: 0x00018D89
		public float fixedTimeSinceEnabled
		{
			get
			{
				return this.enabledTime.timeSince;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x0001AB96 File Offset: 0x00018D96
		// (set) Token: 0x06000687 RID: 1671 RVA: 0x0001AB9E File Offset: 0x00018D9E
		public int bonusRewardCount { get; set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x0001ABA7 File Offset: 0x00018DA7
		// (set) Token: 0x06000689 RID: 1673 RVA: 0x0001ABAF File Offset: 0x00018DAF
		public CombatSquad combatSquad { get; private set; }

		// Token: 0x0600068A RID: 1674 RVA: 0x0001ABB8 File Offset: 0x00018DB8
		private void Awake()
		{
			base.enabled = false;
			this.combatSquad = base.GetComponent<CombatSquad>();
			this.combatSquad.onMemberDiscovered += this.OnMemberDiscovered;
			this.combatSquad.onMemberLost += this.OnMemberLost;
			if (NetworkServer.active)
			{
				this.combatSquad.onDefeatedServer += this.OnDefeatedServer;
				this.combatSquad.onMemberAddedServer += this.OnMemberAddedServer;
				this.combatSquad.onMemberDeathServer += this.OnMemberDeathServer;
				this.rng = new Xoroshiro128Plus(Run.instance.bossRewardRng.nextUlong);
				this.bossDrops = new List<PickupIndex>();
			}
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x0001AC77 File Offset: 0x00018E77
		private void Start()
		{
			if (NetworkServer.active)
			{
				Action<BossGroup> action = BossGroup.onBossGroupStartServer;
				if (action == null)
				{
					return;
				}
				action(this);
			}
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x0001AC90 File Offset: 0x00018E90
		private void OnEnable()
		{
			InstanceTracker.Add<BossGroup>(this);
			ObjectivePanelController.collectObjectiveSources += this.ReportObjective;
			this.enabledTime = Run.FixedTimeStamp.now;
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x0001ACB4 File Offset: 0x00018EB4
		private void OnDisable()
		{
			ObjectivePanelController.collectObjectiveSources -= this.ReportObjective;
			InstanceTracker.Remove<BossGroup>(this);
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x0001ACCD File Offset: 0x00018ECD
		private void FixedUpdate()
		{
			this.UpdateBossMemories();
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0001ACD5 File Offset: 0x00018ED5
		private void OnDefeatedServer()
		{
			this.DropRewards();
			Run.instance.OnServerBossDefeated(this);
			Action<BossGroup> action = BossGroup.onBossGroupDefeatedServer;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x0001ACF8 File Offset: 0x00018EF8
		private void OnMemberAddedServer(CharacterMaster memberMaster)
		{
			Run.instance.OnServerBossAdded(this, memberMaster);
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x0001AD08 File Offset: 0x00018F08
		private void OnMemberDeathServer(CharacterMaster memberMaster, DamageReport damageReport)
		{
			GameObject bodyObject = memberMaster.GetBodyObject();
			DeathRewards deathRewards = (bodyObject != null) ? bodyObject.GetComponent<DeathRewards>() : null;
			if (deathRewards)
			{
				PickupIndex pickupIndex = (PickupIndex)deathRewards.bossPickup;
				if (pickupIndex != PickupIndex.none)
				{
					this.bossDrops.Add(pickupIndex);
				}
			}
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x0001AD55 File Offset: 0x00018F55
		private void OnMemberDiscovered(CharacterMaster memberMaster)
		{
			base.enabled = true;
			memberMaster.isBoss = true;
			BossGroup.totalBossCountDirty = true;
			this.RememberBoss(memberMaster);
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0001AD72 File Offset: 0x00018F72
		private void OnMemberLost(CharacterMaster memberMaster)
		{
			memberMaster.isBoss = false;
			BossGroup.totalBossCountDirty = true;
			if (this.combatSquad.memberCount == 0)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x0001AD98 File Offset: 0x00018F98
		private void DropRewards()
		{
			int participatingPlayerCount = Run.instance.participatingPlayerCount;
			if (participatingPlayerCount != 0 && this.dropPosition)
			{
				List<PickupIndex> list = Run.instance.availableTier2DropList;
				if (this.forceTier3Reward)
				{
					list = Run.instance.availableTier3DropList;
				}
				ItemIndex itemIndex = this.rng.NextElementUniform<PickupIndex>(list).itemIndex;
				int num = 1 + this.bonusRewardCount;
				if (this.scaleRewardsByPlayerCount)
				{
					num *= participatingPlayerCount;
				}
				float angle = 360f / (float)num;
				Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
				Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
				int i = 0;
				while (i < num)
				{
					PickupIndex pickupIndex = new PickupIndex(itemIndex);
					if (this.bossDrops.Count > 0 && this.rng.nextNormalizedFloat <= this.bossDropChance)
					{
						pickupIndex = this.rng.NextElementUniform<PickupIndex>(this.bossDrops);
					}
					PickupDropletController.CreatePickupDroplet(pickupIndex, this.dropPosition.position, vector);
					i++;
					vector = rotation * vector;
				}
			}
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000695 RID: 1685 RVA: 0x0001AED8 File Offset: 0x000190D8
		// (remove) Token: 0x06000696 RID: 1686 RVA: 0x0001AF0C File Offset: 0x0001910C
		public static event Action<BossGroup> onBossGroupStartServer;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000697 RID: 1687 RVA: 0x0001AF40 File Offset: 0x00019140
		// (remove) Token: 0x06000698 RID: 1688 RVA: 0x0001AF74 File Offset: 0x00019174
		public static event Action<BossGroup> onBossGroupDefeatedServer;

		// Token: 0x06000699 RID: 1689 RVA: 0x0001AFA8 File Offset: 0x000191A8
		private int FindBossMemoryIndex(NetworkInstanceId id)
		{
			for (int i = 0; i < this.bossMemoryCount; i++)
			{
				if (this.bossMemories[i].masterInstanceId == id)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x0001AFE4 File Offset: 0x000191E4
		private void RememberBoss(CharacterMaster master)
		{
			if (!master)
			{
				return;
			}
			int num = this.FindBossMemoryIndex(master.netId);
			if (num == -1)
			{
				num = this.AddBossMemory(master);
			}
			ref BossGroup.BossMemory ptr = ref this.bossMemories[num];
			ptr.cachedMaster = master;
			ptr.cachedBody = master.GetBody();
			this.UpdateObservations(ref ptr);
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0001B03C File Offset: 0x0001923C
		private void UpdateObservations(ref BossGroup.BossMemory memory)
		{
			memory.lastObservedHealth = 0f;
			if (memory.cachedMaster && !memory.cachedBody)
			{
				memory.cachedBody = memory.cachedMaster.GetBody();
			}
			if (memory.cachedBody)
			{
				if (this.bestObservedName.Length == 0 && this.bestObservedSubtitle.Length == 0 && Time.fixedDeltaTime * 3f < memory.cachedBody.localStartTime.timeSince)
				{
					this.bestObservedName = Util.GetBestBodyName(memory.cachedBody.gameObject);
					this.bestObservedSubtitle = memory.cachedBody.GetSubtitle();
					if (this.bestObservedSubtitle.Length == 0)
					{
						this.bestObservedSubtitle = Language.GetString("NULL_SUBTITLE");
					}
					this.bestObservedSubtitle = "<sprite name=\"CloudLeft\" tint=1> " + this.bestObservedSubtitle + "<sprite name=\"CloudRight\" tint=1>";
				}
				HealthComponent healthComponent = memory.cachedBody.healthComponent;
				memory.maxObservedMaxHealth = Mathf.Max(memory.maxObservedMaxHealth, healthComponent.fullCombinedHealth);
				memory.lastObservedHealth = healthComponent.combinedHealth;
			}
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x0001B160 File Offset: 0x00019360
		private int AddBossMemory(CharacterMaster master)
		{
			BossGroup.BossMemory bossMemory = new BossGroup.BossMemory
			{
				masterInstanceId = master.netId,
				maxObservedMaxHealth = 0f,
				cachedMaster = master
			};
			HGArrayUtilities.ArrayAppend<BossGroup.BossMemory>(ref this.bossMemories, ref this.bossMemoryCount, ref bossMemory);
			return this.bossMemoryCount - 1;
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600069D RID: 1693 RVA: 0x0001B1B3 File Offset: 0x000193B3
		// (set) Token: 0x0600069E RID: 1694 RVA: 0x0001B1BB File Offset: 0x000193BB
		public string bestObservedName { get; private set; } = "";

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x0001B1C4 File Offset: 0x000193C4
		// (set) Token: 0x060006A0 RID: 1696 RVA: 0x0001B1CC File Offset: 0x000193CC
		public string bestObservedSubtitle { get; private set; } = "";

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060006A1 RID: 1697 RVA: 0x0001B1D5 File Offset: 0x000193D5
		// (set) Token: 0x060006A2 RID: 1698 RVA: 0x0001B1DD File Offset: 0x000193DD
		public float totalMaxObservedMaxHealth { get; private set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x0001B1E6 File Offset: 0x000193E6
		// (set) Token: 0x060006A4 RID: 1700 RVA: 0x0001B1EE File Offset: 0x000193EE
		public float totalObservedHealth { get; private set; }

		// Token: 0x060006A5 RID: 1701 RVA: 0x0001B1F8 File Offset: 0x000193F8
		private void UpdateBossMemories()
		{
			this.totalMaxObservedMaxHealth = 0f;
			this.totalObservedHealth = 0f;
			for (int i = 0; i < this.bossMemoryCount; i++)
			{
				ref BossGroup.BossMemory ptr = ref this.bossMemories[i];
				this.UpdateObservations(ref ptr);
				this.totalMaxObservedMaxHealth += ptr.maxObservedMaxHealth;
				this.totalObservedHealth += Mathf.Max(ptr.lastObservedHealth, 0f);
			}
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x0001B270 File Offset: 0x00019470
		public static int GetTotalBossCount()
		{
			if (BossGroup.totalBossCountDirty)
			{
				BossGroup.totalBossCountDirty = false;
				BossGroup.lastTotalBossCount = 0;
				List<BossGroup> instancesList = InstanceTracker.GetInstancesList<BossGroup>();
				for (int i = 0; i < instancesList.Count; i++)
				{
					BossGroup.lastTotalBossCount += instancesList[i].combatSquad.readOnlyMembersList.Count;
				}
			}
			return BossGroup.lastTotalBossCount;
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0001B2D0 File Offset: 0x000194D0
		public static BossGroup FindBossGroup(CharacterBody body)
		{
			if (!body || !body.isBoss)
			{
				return null;
			}
			CharacterMaster master = body.master;
			if (!master)
			{
				return null;
			}
			List<BossGroup> instancesList = InstanceTracker.GetInstancesList<BossGroup>();
			for (int i = 0; i < instancesList.Count; i++)
			{
				BossGroup bossGroup = instancesList[i];
				if (bossGroup.combatSquad.ContainsMember(master))
				{
					return bossGroup;
				}
			}
			return null;
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0001B330 File Offset: 0x00019530
		public void ReportObjective(CharacterMaster master, List<ObjectivePanelController.ObjectiveSourceDescriptor> output)
		{
			if (this.combatSquad.readOnlyMembersList.Count != 0)
			{
				output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
				{
					source = this,
					master = master,
					objectiveType = typeof(BossGroup.DefeatBossObjectiveTracker)
				});
			}
		}

		// Token: 0x040006DC RID: 1756
		public float bossDropChance = 0.15f;

		// Token: 0x040006DD RID: 1757
		public Transform dropPosition;

		// Token: 0x040006DE RID: 1758
		public bool forceTier3Reward;

		// Token: 0x040006DF RID: 1759
		public bool scaleRewardsByPlayerCount = true;

		// Token: 0x040006E0 RID: 1760
		[Tooltip("Whether or not this boss group should display a health bar on the HUD while any of its members are alive. Other scripts can change this at runtime to suppress a health bar until the boss is angered, for example. This field is not networked, so whatever is driving the value should be synchronized over the network.")]
		public bool shouldDisplayHealthBarOnHud = true;

		// Token: 0x040006E3 RID: 1763
		private Xoroshiro128Plus rng;

		// Token: 0x040006E4 RID: 1764
		private List<PickupIndex> bossDrops;

		// Token: 0x040006E5 RID: 1765
		private Run.FixedTimeStamp enabledTime;

		// Token: 0x040006E8 RID: 1768
		private static readonly int initialBossMemoryCapacity = 8;

		// Token: 0x040006E9 RID: 1769
		private BossGroup.BossMemory[] bossMemories = new BossGroup.BossMemory[BossGroup.initialBossMemoryCapacity];

		// Token: 0x040006EA RID: 1770
		private int bossMemoryCount;

		// Token: 0x040006EF RID: 1775
		private static int lastTotalBossCount = 0;

		// Token: 0x040006F0 RID: 1776
		private static bool totalBossCountDirty = false;

		// Token: 0x02000165 RID: 357
		private struct BossMemory
		{
			// Token: 0x040006F1 RID: 1777
			public NetworkInstanceId masterInstanceId;

			// Token: 0x040006F2 RID: 1778
			public float maxObservedMaxHealth;

			// Token: 0x040006F3 RID: 1779
			public float lastObservedHealth;

			// Token: 0x040006F4 RID: 1780
			public CharacterMaster cachedMaster;

			// Token: 0x040006F5 RID: 1781
			public CharacterBody cachedBody;
		}

		// Token: 0x02000166 RID: 358
		private class DefeatBossObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x060006AB RID: 1707 RVA: 0x0001B3E6 File Offset: 0x000195E6
			public DefeatBossObjectiveTracker()
			{
				this.baseToken = "OBJECTIVE_DEFEAT_BOSS";
			}
		}

		// Token: 0x02000167 RID: 359
		public class EnableHudHealthBarState : EntityState
		{
			// Token: 0x060006AC RID: 1708 RVA: 0x0001B3F9 File Offset: 0x000195F9
			public override void OnEnter()
			{
				base.OnEnter();
				this.bossGroup = base.GetComponent<BossGroup>();
				if (this.bossGroup != null)
				{
					this.bossGroup.shouldDisplayHealthBarOnHud = true;
				}
			}

			// Token: 0x060006AD RID: 1709 RVA: 0x0001B421 File Offset: 0x00019621
			public override void OnExit()
			{
				if (this.bossGroup != null)
				{
					this.bossGroup.shouldDisplayHealthBarOnHud = false;
				}
				base.OnExit();
			}

			// Token: 0x040006F6 RID: 1782
			private BossGroup bossGroup;
		}
	}
}
