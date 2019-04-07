using System;
using EntityStates.Interactables.GoldBeacon;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Missions.Goldshores
{
	// Token: 0x02000103 RID: 259
	public class GoldshoresBossfight : EntityState
	{
		// Token: 0x060004FF RID: 1279 RVA: 0x000150C3 File Offset: 0x000132C3
		public override void OnEnter()
		{
			base.OnEnter();
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x000150CB File Offset: 0x000132CB
		private void AddImmunityBuff()
		{
			if (this.bossInstanceBody)
			{
				this.bossInstanceBody.AddBuff(BuffIndex.Immune);
			}
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x000150E7 File Offset: 0x000132E7
		private void RemoveImmunityBuff()
		{
			if (this.bossInstanceBody)
			{
				this.bossInstanceBody.RemoveBuff(BuffIndex.Immune);
			}
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00015104 File Offset: 0x00013304
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			this.shieldRemovalAge += Time.fixedDeltaTime;
			if (NetworkServer.active)
			{
				if (GoldshoresMissionController.instance.beaconsActive == GoldshoresMissionController.instance.beaconsToSpawnOnMap && !this.shieldIsOff)
				{
					EffectManager.instance.SpawnEffect(GoldshoresBossfight.shieldRemovalEffectPrefab, new EffectData
					{
						origin = this.bossInstanceBody.coreTransform.position
					}, true);
					this.RemoveImmunityBuff();
					this.shieldRemovalAge = 0f;
					this.shieldIsOff = true;
				}
				if (this.shieldRemovalAge >= GoldshoresBossfight.shieldRemovalDuration && this.shieldIsOff)
				{
					this.shieldIsOff = false;
					foreach (GameObject gameObject in GoldshoresMissionController.instance.beaconInstanceList)
					{
						gameObject.GetComponent<EntityStateMachine>().SetNextState(new NotReady());
					}
					this.AddImmunityBuff();
				}
				if (this.stopwatch >= GoldshoresBossfight.transitionDuration)
				{
					GoldshoresMissionController.instance.ExitTransitionIntoBossfight();
					if (!this.hasSpawnedBoss)
					{
						Vector3 zero = Vector3.zero;
						this.hasSpawnedBoss = true;
						DirectorPlacementRule placementRule = new DirectorPlacementRule
						{
							placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
							minDistance = 0f,
							maxDistance = 1000f,
							position = zero
						};
						GameObject gameObject2 = DirectorCore.instance.TrySpawnObject(Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscTitanGold"), placementRule, GoldshoresMissionController.instance.rng);
						if (gameObject2)
						{
							float num = 1f;
							float num2 = 1f;
							CharacterMaster component = gameObject2.GetComponent<CharacterMaster>();
							this.bossInstanceBody = component.GetBody();
							if (!this.bossGroup)
							{
								GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BossGroup"));
								NetworkServer.Spawn(gameObject3);
								this.bossGroup = gameObject3.GetComponent<BossGroup>();
								this.bossGroup.dropPosition = GoldshoresMissionController.instance.bossSpawnPosition;
							}
							this.AddImmunityBuff();
							num2 += Run.instance.difficultyCoefficient / 8f;
							num += Run.instance.difficultyCoefficient / 2f;
							int livingPlayerCount = Run.instance.livingPlayerCount;
							num *= Mathf.Pow((float)livingPlayerCount, 0.5f);
							component.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num - 1f) * 10f));
							component.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num2 - 1f) * 10f));
							this.bossGroup.bossDropChance = 1f;
							this.bossGroup.AddMember(component);
							return;
						}
					}
					else if (this.bossGroup.readOnlyMembersList.Count == 0)
					{
						this.outer.SetNextState(new Exit());
					}
				}
			}
		}

		// Token: 0x040004DB RID: 1243
		public static float shieldRemovalDuration;

		// Token: 0x040004DC RID: 1244
		public static GameObject shieldRemovalEffectPrefab;

		// Token: 0x040004DD RID: 1245
		public static GameObject shieldRegenerationEffectPrefab;

		// Token: 0x040004DE RID: 1246
		private static float transitionDuration = 3f;

		// Token: 0x040004DF RID: 1247
		private float stopwatch;

		// Token: 0x040004E0 RID: 1248
		private bool hasSpawnedBoss;

		// Token: 0x040004E1 RID: 1249
		private BossGroup bossGroup;

		// Token: 0x040004E2 RID: 1250
		private CharacterBody bossInstanceBody;

		// Token: 0x040004E3 RID: 1251
		private float shieldRemovalAge;

		// Token: 0x040004E4 RID: 1252
		private bool shieldIsOff;
	}
}
