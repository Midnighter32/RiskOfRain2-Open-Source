using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BeetleQueenMonster
{
	// Token: 0x020001D5 RID: 469
	public class SummonEggs : BaseState
	{
		// Token: 0x06000926 RID: 2342 RVA: 0x0002DF40 File Offset: 0x0002C140
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

		// Token: 0x06000927 RID: 2343 RVA: 0x0002DFC0 File Offset: 0x0002C1C0
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

		// Token: 0x06000928 RID: 2344 RVA: 0x0002E024 File Offset: 0x0002C224
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
				GameObject gameObject = DirectorCore.instance.TrySpawnObject((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetle"), new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Approximate,
					minDistance = 3f,
					maxDistance = 20f,
					spawnOnTarget = transform
				}, RoR2Application.rng);
				if (gameObject)
				{
					gameObject.GetComponent<Inventory>().SetEquipmentIndex(base.characterBody.inventory.currentEquipmentIndex);
				}
			}
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x0002E130 File Offset: 0x0002C330
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

		// Token: 0x0600092A RID: 2346 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000C64 RID: 3172
		public static float baseDuration = 3.5f;

		// Token: 0x04000C65 RID: 3173
		public static string attackSoundString;

		// Token: 0x04000C66 RID: 3174
		public static float randomRadius = 8f;

		// Token: 0x04000C67 RID: 3175
		public static GameObject spitPrefab;

		// Token: 0x04000C68 RID: 3176
		public static int maxSummonCount = 5;

		// Token: 0x04000C69 RID: 3177
		private static float summonDuration = 3.26f;

		// Token: 0x04000C6A RID: 3178
		private Animator animator;

		// Token: 0x04000C6B RID: 3179
		private Transform modelTransform;

		// Token: 0x04000C6C RID: 3180
		private ChildLocator childLocator;

		// Token: 0x04000C6D RID: 3181
		private float duration;

		// Token: 0x04000C6E RID: 3182
		private float summonInterval;

		// Token: 0x04000C6F RID: 3183
		private float summonTimer;

		// Token: 0x04000C70 RID: 3184
		private int summonCount;

		// Token: 0x04000C71 RID: 3185
		private bool isSummoning;
	}
}
