using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020008F0 RID: 2288
	public class SummonEggs : BaseState
	{
		// Token: 0x06003326 RID: 13094 RVA: 0x000DDB94 File Offset: 0x000DBD94
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			this.duration = SummonEggs.baseDuration;
			base.PlayCrossfade("Gesture", "SummonEggs", 0.5f);
			Util.PlaySound(SummonEggs.attackSoundString, base.gameObject);
			this.summonInterval = SummonEggs.summonDuration / (float)SummonEggs.maxSummonCount;
		}

		// Token: 0x06003327 RID: 13095 RVA: 0x000DDC14 File Offset: 0x000DBE14
		private Transform FindTargetClosest(Vector3 point, TeamIndex enemyTeam)
		{
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(enemyTeam);
			float num = 99999f;
			Transform result = null;
			for (int i = 0; i < teamMembers.Count; i++)
			{
				float num2 = Vector3.SqrMagnitude(teamMembers[i].transform.position - point);
				if (num2 < num)
				{
					num = num2;
					result = teamMembers[i].transform;
				}
			}
			return result;
		}

		// Token: 0x06003328 RID: 13096 RVA: 0x000DDC78 File Offset: 0x000DBE78
		private void SummonEgg()
		{
			Vector3 point = Vector3.zero;
			Ray aimRay = base.GetAimRay();
			aimRay.origin += UnityEngine.Random.insideUnitSphere * SummonEggs.randomRadius;
			RaycastHit raycastHit;
			if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
			{
				point = raycastHit.point;
			}
			TeamIndex teamIndex = base.characterBody.GetComponent<TeamComponent>().teamIndex;
			TeamIndex enemyTeam;
			if (teamIndex != TeamIndex.Player)
			{
				if (teamIndex == TeamIndex.Monster)
				{
					enemyTeam = TeamIndex.Player;
				}
				else
				{
					enemyTeam = TeamIndex.Neutral;
				}
			}
			else
			{
				enemyTeam = TeamIndex.Monster;
			}
			Transform transform = this.FindTargetClosest(point, enemyTeam);
			if (transform)
			{
				DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetle"), new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Approximate,
					minDistance = 3f,
					maxDistance = 20f,
					spawnOnTarget = transform
				}, RoR2Application.rng);
				directorSpawnRequest.summonerBodyObject = base.gameObject;
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
				if (gameObject)
				{
					gameObject.GetComponent<Inventory>().SetEquipmentIndex(base.characterBody.inventory.currentEquipmentIndex);
				}
			}
		}

		// Token: 0x06003329 RID: 13097 RVA: 0x000DDD9C File Offset: 0x000DBF9C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = this.animator.GetFloat("SummonEggs.active") > 0.9f;
			if (flag && !this.isSummoning)
			{
				string childName = "Mouth";
				Transform transform = this.childLocator.FindChild(childName);
				if (transform)
				{
					UnityEngine.Object.Instantiate<GameObject>(SummonEggs.spitPrefab, transform);
				}
			}
			if (this.isSummoning)
			{
				this.summonTimer += Time.fixedDeltaTime;
				if (NetworkServer.active && this.summonTimer > 0f && this.summonCount < SummonEggs.maxSummonCount)
				{
					this.summonCount++;
					this.summonTimer -= this.summonInterval;
					this.SummonEgg();
				}
			}
			this.isSummoning = flag;
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600332A RID: 13098 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400328C RID: 12940
		public static float baseDuration = 3.5f;

		// Token: 0x0400328D RID: 12941
		public static string attackSoundString;

		// Token: 0x0400328E RID: 12942
		public static float randomRadius = 8f;

		// Token: 0x0400328F RID: 12943
		public static GameObject spitPrefab;

		// Token: 0x04003290 RID: 12944
		public static int maxSummonCount = 5;

		// Token: 0x04003291 RID: 12945
		private static float summonDuration = 3.26f;

		// Token: 0x04003292 RID: 12946
		private Animator animator;

		// Token: 0x04003293 RID: 12947
		private Transform modelTransform;

		// Token: 0x04003294 RID: 12948
		private ChildLocator childLocator;

		// Token: 0x04003295 RID: 12949
		private float duration;

		// Token: 0x04003296 RID: 12950
		private float summonInterval;

		// Token: 0x04003297 RID: 12951
		private float summonTimer;

		// Token: 0x04003298 RID: 12952
		private int summonCount;

		// Token: 0x04003299 RID: 12953
		private bool isSummoning;
	}
}
