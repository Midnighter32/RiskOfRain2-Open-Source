using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x020000CF RID: 207
	public class ChannelRain : BaseState
	{
		// Token: 0x06000409 RID: 1033 RVA: 0x00010994 File Offset: 0x0000EB94
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChannelRain.baseDuration;
			this.durationBetweenCast = ChannelRain.baseDuration / (float)ChannelRain.explosionCount / this.attackSpeedStat;
			base.PlayCrossfade("Body", "ChannelRain", 0.3f);
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x000109E0 File Offset: 0x0000EBE0
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

		// Token: 0x0600040B RID: 1035 RVA: 0x00010C18 File Offset: 0x0000EE18
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

		// Token: 0x0600040C RID: 1036 RVA: 0x00010C8C File Offset: 0x0000EE8C
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

		// Token: 0x0600040D RID: 1037 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040003C7 RID: 967
		private float castTimer;

		// Token: 0x040003C8 RID: 968
		public static float baseDuration = 4f;

		// Token: 0x040003C9 RID: 969
		public static float explosionDelay = 2f;

		// Token: 0x040003CA RID: 970
		public static int explosionCount = 10;

		// Token: 0x040003CB RID: 971
		public static float damageCoefficient;

		// Token: 0x040003CC RID: 972
		public static float randomRadius;

		// Token: 0x040003CD RID: 973
		public static float radius;

		// Token: 0x040003CE RID: 974
		public static GameObject delayPrefab;

		// Token: 0x040003CF RID: 975
		private float duration;

		// Token: 0x040003D0 RID: 976
		private float durationBetweenCast;
	}
}
