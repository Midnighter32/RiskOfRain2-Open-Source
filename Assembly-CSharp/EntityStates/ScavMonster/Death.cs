using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.ScavMonster
{
	// Token: 0x0200078B RID: 1931
	public class Death : GenericCharacterDeath
	{
		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06002C55 RID: 11349 RVA: 0x0000AC89 File Offset: 0x00008E89
		protected override bool shouldAutoDestroy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002C56 RID: 11350 RVA: 0x000BB274 File Offset: 0x000B9474
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				CharacterMaster characterMaster = base.characterBody ? base.characterBody.master : null;
				if (characterMaster)
				{
					bool flag = characterMaster.IsExtraLifePendingServer();
					bool flag2 = characterMaster.inventory.GetItemCount(ItemIndex.Ghost) > 0;
					bool flag3 = !Stage.instance || Stage.instance.scavPackDroppedServer;
					this.shouldDropPack = (!flag && !flag2 && !flag3);
					if (this.shouldDropPack)
					{
						Stage.instance.scavPackDroppedServer = true;
					}
				}
			}
		}

		// Token: 0x06002C57 RID: 11351 RVA: 0x000BB30A File Offset: 0x000B950A
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge >= Death.duration)
			{
				this.OnPreDestroyBodyServer();
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x06002C58 RID: 11352 RVA: 0x000BB337 File Offset: 0x000B9537
		public override void OnExit()
		{
			base.DestroyModel();
			base.OnExit();
		}

		// Token: 0x06002C59 RID: 11353 RVA: 0x000BB345 File Offset: 0x000B9545
		protected override void OnPreDestroyBodyServer()
		{
			base.OnPreDestroyBodyServer();
			if (this.shouldDropPack)
			{
				this.AttemptDropPack();
			}
		}

		// Token: 0x06002C5A RID: 11354 RVA: 0x000BB35C File Offset: 0x000B955C
		private void AttemptDropPack()
		{
			DirectorCore instance = DirectorCore.instance;
			if (instance)
			{
				Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
				DirectorPlacementRule placementRule = new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
					position = base.characterBody.footPosition,
					minDistance = 0f,
					maxDistance = float.PositiveInfinity
				};
				instance.TrySpawnObject(new DirectorSpawnRequest(this.spawnCard, placementRule, rng));
			}
		}

		// Token: 0x0400285E RID: 10334
		[SerializeField]
		public SpawnCard spawnCard;

		// Token: 0x0400285F RID: 10335
		public static float duration;

		// Token: 0x04002860 RID: 10336
		private bool shouldDropPack;
	}
}
