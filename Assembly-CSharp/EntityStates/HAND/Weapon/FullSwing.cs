using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.HAND.Weapon
{
	// Token: 0x02000847 RID: 2119
	public class FullSwing : BaseState
	{
		// Token: 0x06002FF4 RID: 12276 RVA: 0x000CD78C File Offset: 0x000CB98C
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

		// Token: 0x06002FF5 RID: 12277 RVA: 0x000CD9C8 File Offset: 0x000CBBC8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Hammer.hitBoxActive") > 0.5f)
			{
				if (!this.hasSwung)
				{
					EffectManager.SimpleMuzzleFlash(FullSwing.swingEffectPrefab, base.gameObject, "SwingCenter", true);
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

		// Token: 0x06002FF6 RID: 12278 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x06002FF7 RID: 12279 RVA: 0x000CDA7C File Offset: 0x000CBC7C
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

		// Token: 0x04002DC3 RID: 11715
		public static float baseDuration = 3.5f;

		// Token: 0x04002DC4 RID: 11716
		public static float returnToIdlePercentage;

		// Token: 0x04002DC5 RID: 11717
		public static float damageCoefficient = 4f;

		// Token: 0x04002DC6 RID: 11718
		public static float forceMagnitude = 16f;

		// Token: 0x04002DC7 RID: 11719
		public static float radius = 3f;

		// Token: 0x04002DC8 RID: 11720
		public static GameObject hitEffectPrefab;

		// Token: 0x04002DC9 RID: 11721
		public static GameObject swingEffectPrefab;

		// Token: 0x04002DCA RID: 11722
		private Transform hammerChildTransform;

		// Token: 0x04002DCB RID: 11723
		private OverlapAttack attack;

		// Token: 0x04002DCC RID: 11724
		private Animator modelAnimator;

		// Token: 0x04002DCD RID: 11725
		private float duration;

		// Token: 0x04002DCE RID: 11726
		private bool hasSwung;
	}
}
