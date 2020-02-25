using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x0200072E RID: 1838
	public class ChannelRain : BaseState
	{
		// Token: 0x06002AB2 RID: 10930 RVA: 0x000B39E0 File Offset: 0x000B1BE0
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChannelRain.baseDuration;
			this.durationBetweenCast = ChannelRain.baseDuration / (float)ChannelRain.explosionCount / this.attackSpeedStat;
			base.PlayCrossfade("Body", "ChannelRain", 0.3f);
		}

		// Token: 0x06002AB3 RID: 10931 RVA: 0x000B3A2C File Offset: 0x000B1C2C
		private void PlaceRain()
		{
			Vector3 vector = Vector3.zero;
			Ray aimRay = base.GetAimRay();
			aimRay.origin += UnityEngine.Random.insideUnitSphere * ChannelRain.randomRadius;
			RaycastHit raycastHit;
			if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
			{
				vector = raycastHit.point;
			}
			if (vector != Vector3.zero)
			{
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
				Transform transform = this.FindTargetClosest(vector, enemyTeam);
				Vector3 a = vector;
				if (transform)
				{
					a = transform.transform.position;
				}
				a += UnityEngine.Random.insideUnitSphere * ChannelRain.randomRadius;
				if (Physics.Raycast(new Ray
				{
					origin = a + Vector3.up * ChannelRain.randomRadius,
					direction = Vector3.down
				}, out raycastHit, 500f, LayerIndex.world.mask))
				{
					Vector3 point = raycastHit.point;
					Quaternion rotation = Util.QuaternionSafeLookRotation(raycastHit.normal);
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ChannelRain.delayPrefab, point, rotation);
					DelayBlast component = gameObject.GetComponent<DelayBlast>();
					component.position = point;
					component.baseDamage = base.characterBody.damage * ChannelRain.damageCoefficient;
					component.baseForce = 2000f;
					component.bonusForce = Vector3.up * 1000f;
					component.radius = ChannelRain.radius;
					component.attacker = base.gameObject;
					component.inflictor = null;
					component.crit = Util.CheckRoll(this.critStat, base.characterBody.master);
					component.maxTimer = ChannelRain.explosionDelay;
					gameObject.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(component.attacker);
					gameObject.transform.localScale = new Vector3(ChannelRain.radius, ChannelRain.radius, 1f);
					ScaleParticleSystemDuration component2 = gameObject.GetComponent<ScaleParticleSystemDuration>();
					if (component2)
					{
						component2.newDuration = ChannelRain.explosionDelay;
					}
				}
			}
		}

		// Token: 0x06002AB4 RID: 10932 RVA: 0x000B3C64 File Offset: 0x000B1E64
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.castTimer += Time.fixedDeltaTime;
			if (this.castTimer >= this.durationBetweenCast)
			{
				this.PlaceRain();
				this.castTimer -= this.durationBetweenCast;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new EndRain());
			}
		}

		// Token: 0x06002AB5 RID: 10933 RVA: 0x000B3CD8 File Offset: 0x000B1ED8
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

		// Token: 0x06002AB6 RID: 10934 RVA: 0x0000C68F File Offset: 0x0000A88F
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		// Token: 0x0400268D RID: 9869
		private float castTimer;

		// Token: 0x0400268E RID: 9870
		public static float baseDuration = 4f;

		// Token: 0x0400268F RID: 9871
		public static float explosionDelay = 2f;

		// Token: 0x04002690 RID: 9872
		public static int explosionCount = 10;

		// Token: 0x04002691 RID: 9873
		public static float damageCoefficient;

		// Token: 0x04002692 RID: 9874
		public static float randomRadius;

		// Token: 0x04002693 RID: 9875
		public static float radius;

		// Token: 0x04002694 RID: 9876
		public static GameObject delayPrefab;

		// Token: 0x04002695 RID: 9877
		private float duration;

		// Token: 0x04002696 RID: 9878
		private float durationBetweenCast;
	}
}
