using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x020000D4 RID: 212
	public class Enrage : BaseState
	{
		// Token: 0x0600042C RID: 1068 RVA: 0x00011414 File Offset: 0x0000F614
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

		// Token: 0x0600042D RID: 1069 RVA: 0x00011474 File Offset: 0x0000F674
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Enrage.activate") > 0.5f && !this.hasCastBuff)
			{
				EffectData effectData = new EffectData();
				effectData.origin = base.transform.position;
				effectData.SetNetworkedObjectReference(base.gameObject);
				EffectManager.instance.SpawnEffect(Enrage.enragePrefab, effectData, true);
				this.hasCastBuff = true;
				base.characterBody.AddBuff(BuffIndex.EnrageAncientWisp);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00011524 File Offset: 0x0000F724
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
								component2.ApplyForce(normalized * force, false);
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

		// Token: 0x040003E9 RID: 1001
		public static float baseDuration = 3.5f;

		// Token: 0x040003EA RID: 1002
		public static GameObject enragePrefab;

		// Token: 0x040003EB RID: 1003
		private Animator modelAnimator;

		// Token: 0x040003EC RID: 1004
		private float duration;

		// Token: 0x040003ED RID: 1005
		private bool hasCastBuff;
	}
}
