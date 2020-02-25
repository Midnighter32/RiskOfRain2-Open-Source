using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace EntityStates.RoboBallBoss.Weapon
{
	// Token: 0x0200079B RID: 1947
	public class DeployMinions : BaseState
	{
		// Token: 0x06002C9A RID: 11418 RVA: 0x000BC214 File Offset: 0x000BA414
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
			this.duration = DeployMinions.baseDuration;
			base.PlayCrossfade("Gesture, Additive", "DeployMinions", "DeployMinions.playbackRate", this.duration, 0.1f);
			Util.PlaySound(DeployMinions.attackSoundString, base.gameObject);
			this.summonInterval = DeployMinions.summonDuration / (float)DeployMinions.maxSummonCount;
		}

		// Token: 0x06002C9B RID: 11419 RVA: 0x000BC2A0 File Offset: 0x000BA4A0
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

		// Token: 0x06002C9C RID: 11420 RVA: 0x000BC304 File Offset: 0x000BA504
		private void SummonMinion()
		{
			if (!base.characterBody || !base.characterBody.master)
			{
				return;
			}
			if (base.characterBody.master.GetDeployableCount(DeployableSlot.RoboBallMini) < base.characterBody.master.GetDeployableSameSlotLimit(DeployableSlot.RoboBallMini))
			{
				Util.PlaySound(DeployMinions.summonSoundString, base.gameObject);
				if (!NetworkServer.active)
				{
					return;
				}
				Vector3 position = base.FindModelChild(DeployMinions.summonMuzzleString).position;
				DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load(string.Format("SpawnCards/CharacterSpawnCards/{0}", DeployMinions.spawnCard)), new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Direct,
					minDistance = 0f,
					maxDistance = 0f,
					position = position
				}, RoR2Application.rng);
				directorSpawnRequest.summonerBodyObject = base.gameObject;
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
				if (gameObject)
				{
					CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
					gameObject.GetComponent<Inventory>().SetEquipmentIndex(base.characterBody.inventory.currentEquipmentIndex);
					Deployable deployable = gameObject.AddComponent<Deployable>();
					deployable.onUndeploy = new UnityEvent();
					deployable.onUndeploy.AddListener(new UnityAction(component.TrueKill));
					base.characterBody.master.AddDeployable(deployable, DeployableSlot.RoboBallMini);
				}
			}
		}

		// Token: 0x06002C9D RID: 11421 RVA: 0x000BC450 File Offset: 0x000BA650
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = this.animator.GetFloat("DeployMinions.active") > 0.9f;
			if (this.isSummoning)
			{
				this.summonTimer += Time.fixedDeltaTime;
				if (NetworkServer.active && this.summonTimer > 0f && this.summonCount < DeployMinions.maxSummonCount)
				{
					this.summonCount++;
					this.summonTimer -= this.summonInterval;
					this.SummonMinion();
				}
			}
			this.isSummoning = flag;
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002C9E RID: 11422 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040028B1 RID: 10417
		public static float baseDuration = 3.5f;

		// Token: 0x040028B2 RID: 10418
		public static string attackSoundString;

		// Token: 0x040028B3 RID: 10419
		public static string summonSoundString;

		// Token: 0x040028B4 RID: 10420
		public static int maxSummonCount = 5;

		// Token: 0x040028B5 RID: 10421
		public static float summonDuration = 3.26f;

		// Token: 0x040028B6 RID: 10422
		public static string summonMuzzleString;

		// Token: 0x040028B7 RID: 10423
		public static string spawnCard;

		// Token: 0x040028B8 RID: 10424
		private Animator animator;

		// Token: 0x040028B9 RID: 10425
		private Transform modelTransform;

		// Token: 0x040028BA RID: 10426
		private ChildLocator childLocator;

		// Token: 0x040028BB RID: 10427
		private float duration;

		// Token: 0x040028BC RID: 10428
		private float summonInterval;

		// Token: 0x040028BD RID: 10429
		private float summonTimer;

		// Token: 0x040028BE RID: 10430
		private int summonCount;

		// Token: 0x040028BF RID: 10431
		private bool isSummoning;
	}
}
