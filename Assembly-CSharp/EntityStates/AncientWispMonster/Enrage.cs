using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x02000733 RID: 1843
	public class Enrage : BaseState
	{
		// Token: 0x06002AD5 RID: 10965 RVA: 0x000B4460 File Offset: 0x000B2660
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Enrage.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayCrossfade("Gesture", "Enrage", "Enrage.playbackRate", this.duration, 0.2f);
			}
		}

		// Token: 0x06002AD6 RID: 10966 RVA: 0x000B44C0 File Offset: 0x000B26C0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Enrage.activate") > 0.5f && !this.hasCastBuff)
			{
				EffectData effectData = new EffectData();
				effectData.origin = base.transform.position;
				effectData.SetNetworkedObjectReference(base.gameObject);
				EffectManager.SpawnEffect(Enrage.enragePrefab, effectData, true);
				this.hasCastBuff = true;
				base.characterBody.AddBuff(BuffIndex.EnrageAncientWisp);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002AD7 RID: 10967 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x06002AD8 RID: 10968 RVA: 0x000B456C File Offset: 0x000B276C
		private static void PullEnemies(Vector3 position, Vector3 direction, float coneAngle, float maxDistance, float force, TeamIndex excludedTeam)
		{
			float num = Mathf.Cos(coneAngle * 0.5f * 0.017453292f);
			foreach (Collider collider in Physics.OverlapSphere(position, maxDistance))
			{
				Vector3 position2 = collider.transform.position;
				Vector3 normalized = (position - position2).normalized;
				if (Vector3.Dot(-normalized, direction) >= num)
				{
					TeamComponent component = collider.GetComponent<TeamComponent>();
					if (component)
					{
						TeamIndex teamIndex = component.teamIndex;
						if (teamIndex != excludedTeam)
						{
							CharacterMotor component2 = collider.GetComponent<CharacterMotor>();
							if (component2)
							{
								component2.ApplyForce(normalized * force, false, false);
							}
							Rigidbody component3 = collider.GetComponent<Rigidbody>();
							if (component3)
							{
								component3.AddForce(normalized * force, ForceMode.Impulse);
							}
						}
					}
				}
			}
		}

		// Token: 0x040026AF RID: 9903
		public static float baseDuration = 3.5f;

		// Token: 0x040026B0 RID: 9904
		public static GameObject enragePrefab;

		// Token: 0x040026B1 RID: 9905
		private Animator modelAnimator;

		// Token: 0x040026B2 RID: 9906
		private float duration;

		// Token: 0x040026B3 RID: 9907
		private bool hasCastBuff;
	}
}
