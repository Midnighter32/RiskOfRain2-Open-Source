using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.HAND.Weapon
{
	// Token: 0x02000165 RID: 357
	public class FullSwing : BaseState
	{
		// Token: 0x060006EF RID: 1775 RVA: 0x00021080 File Offset: 0x0001F280
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FullSwing.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = FullSwing.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = FullSwing.hitEffectPrefab;
			this.attack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Hammer");
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.hammerChildTransform = component.FindChild("SwingCenter");
				}
			}
			if (this.modelAnimator)
			{
				int layerIndex = this.modelAnimator.GetLayerIndex("Gesture");
				if (this.modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName("FullSwing3") || this.modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName("FullSwing1"))
				{
					base.PlayCrossfade("Gesture", "FullSwing2", "FullSwing.playbackRate", this.duration / (1f - FullSwing.returnToIdlePercentage), 0.2f);
				}
				else if (this.modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName("FullSwing2"))
				{
					base.PlayCrossfade("Gesture", "FullSwing3", "FullSwing.playbackRate", this.duration / (1f - FullSwing.returnToIdlePercentage), 0.2f);
				}
				else
				{
					base.PlayCrossfade("Gesture", "FullSwing1", "FullSwing.playbackRate", this.duration / (1f - FullSwing.returnToIdlePercentage), 0.2f);
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x000212BC File Offset: 0x0001F4BC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Hammer.hitBoxActive") > 0.5f)
			{
				if (!this.hasSwung)
				{
					EffectManager.instance.SimpleMuzzleFlash(FullSwing.swingEffectPrefab, base.gameObject, "SwingCenter", true);
					this.hasSwung = true;
				}
				this.attack.forceVector = this.hammerChildTransform.right * -FullSwing.forceMagnitude;
				this.attack.Fire(null);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x00021374 File Offset: 0x0001F574
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

		// Token: 0x0400087D RID: 2173
		public static float baseDuration = 3.5f;

		// Token: 0x0400087E RID: 2174
		public static float returnToIdlePercentage;

		// Token: 0x0400087F RID: 2175
		public static float damageCoefficient = 4f;

		// Token: 0x04000880 RID: 2176
		public static float forceMagnitude = 16f;

		// Token: 0x04000881 RID: 2177
		public static float radius = 3f;

		// Token: 0x04000882 RID: 2178
		public static GameObject hitEffectPrefab;

		// Token: 0x04000883 RID: 2179
		public static GameObject swingEffectPrefab;

		// Token: 0x04000884 RID: 2180
		private Transform hammerChildTransform;

		// Token: 0x04000885 RID: 2181
		private OverlapAttack attack;

		// Token: 0x04000886 RID: 2182
		private Animator modelAnimator;

		// Token: 0x04000887 RID: 2183
		private float duration;

		// Token: 0x04000888 RID: 2184
		private bool hasSwung;
	}
}
