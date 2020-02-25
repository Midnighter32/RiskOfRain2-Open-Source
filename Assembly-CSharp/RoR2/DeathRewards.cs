using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001D2 RID: 466
	[RequireComponent(typeof(CharacterBody))]
	public class DeathRewards : MonoBehaviour, IOnKilledServerReceiver
	{
		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060009FC RID: 2556 RVA: 0x0002BC74 File Offset: 0x00029E74
		// (set) Token: 0x060009FD RID: 2557 RVA: 0x0002BC9F File Offset: 0x00029E9F
		public uint goldReward
		{
			get
			{
				if (!this.characterBody.master)
				{
					return this.fallbackGold;
				}
				return this.characterBody.master.money;
			}
			set
			{
				if (this.characterBody.master)
				{
					this.characterBody.master.money = value;
					return;
				}
				this.fallbackGold = value;
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060009FE RID: 2558 RVA: 0x0002BCCC File Offset: 0x00029ECC
		// (set) Token: 0x060009FF RID: 2559 RVA: 0x0002BCD4 File Offset: 0x00029ED4
		public uint expReward { get; set; }

		// Token: 0x06000A00 RID: 2560 RVA: 0x0002BCDD File Offset: 0x00029EDD
		[RuntimeInitializeOnLoadMethod]
		private static void LoadAssets()
		{
			DeathRewards.coinEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/CoinEmitter");
			DeathRewards.logbookPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/LogPickup");
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x0002BCFD File Offset: 0x00029EFD
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x0002BD0C File Offset: 0x00029F0C
		public void OnKilledServer(DamageReport damageReport)
		{
			CharacterBody attackerBody = damageReport.attackerBody;
			if (attackerBody)
			{
				Vector3 corePosition = this.characterBody.corePosition;
				TeamManager.instance.GiveTeamMoney(damageReport.attackerTeamIndex, this.goldReward);
				EffectManager.SpawnEffect(DeathRewards.coinEffectPrefab, new EffectData
				{
					origin = corePosition,
					genericFloat = this.goldReward,
					scale = this.characterBody.radius
				}, true);
				float num = 1f + (this.characterBody.level - 1f) * 0.3f;
				ExperienceManager.instance.AwardExperience(corePosition, attackerBody, (ulong)((uint)(this.expReward * num)));
				if (this.logUnlockableName != "" && Run.instance.CanUnlockableBeGrantedThisRun(this.logUnlockableName) && Util.CheckRoll(this.characterBody.isChampion ? 3f : 1f, damageReport.attackerMaster))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(DeathRewards.logbookPrefab, corePosition, UnityEngine.Random.rotation);
					gameObject.GetComponentInChildren<UnlockPickup>().unlockableName = this.logUnlockableName;
					gameObject.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
					NetworkServer.Spawn(gameObject);
				}
			}
		}

		// Token: 0x04000A38 RID: 2616
		public string logUnlockableName = "";

		// Token: 0x04000A39 RID: 2617
		public SerializablePickupIndex bossPickup;

		// Token: 0x04000A3A RID: 2618
		private uint fallbackGold;

		// Token: 0x04000A3C RID: 2620
		private CharacterBody characterBody;

		// Token: 0x04000A3D RID: 2621
		private static GameObject coinEffectPrefab;

		// Token: 0x04000A3E RID: 2622
		private static GameObject logbookPrefab;
	}
}
