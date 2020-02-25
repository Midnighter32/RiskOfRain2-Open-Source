using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x02000736 RID: 1846
	public class Throw : BaseState
	{
		// Token: 0x06002AE6 RID: 10982 RVA: 0x000B48DC File Offset: 0x000B2ADC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Throw.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					this.rightMuzzleTransform = component.FindChild("MuzzleRight");
				}
			}
			if (this.modelAnimator)
			{
				int layerIndex = this.modelAnimator.GetLayerIndex("Gesture");
				if (this.modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Throw1"))
				{
					base.PlayCrossfade("Gesture", "Throw2", "Throw.playbackRate", this.duration / (1f - Throw.returnToIdlePercentage), 0.2f);
				}
				else
				{
					base.PlayCrossfade("Gesture", "Throw1", "Throw.playbackRate", this.duration / (1f - Throw.returnToIdlePercentage), 0.2f);
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x06002AE7 RID: 10983 RVA: 0x000B49F0 File Offset: 0x000B2BF0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && this.modelAnimator && this.modelAnimator.GetFloat("Throw.activate") > 0f && !this.hasSwung)
			{
				Ray aimRay = base.GetAimRay();
				Vector3 forward = aimRay.direction;
				RaycastHit raycastHit;
				if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
				{
					forward = raycastHit.point - this.rightMuzzleTransform.position;
				}
				ProjectileManager.instance.FireProjectile(Throw.projectilePrefab, this.rightMuzzleTransform.position, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * Throw.damageCoefficient, Throw.forceMagnitude, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				EffectManager.SimpleMuzzleFlash(Throw.swingEffectPrefab, base.gameObject, "RightSwingCenter", true);
				this.hasSwung = true;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002AE8 RID: 10984 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x06002AE9 RID: 10985 RVA: 0x000B4B1C File Offset: 0x000B2D1C
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

		// Token: 0x040026C0 RID: 9920
		public static float baseDuration = 3.5f;

		// Token: 0x040026C1 RID: 9921
		public static float returnToIdlePercentage;

		// Token: 0x040026C2 RID: 9922
		public static float damageCoefficient = 4f;

		// Token: 0x040026C3 RID: 9923
		public static float forceMagnitude = 16f;

		// Token: 0x040026C4 RID: 9924
		public static float radius = 3f;

		// Token: 0x040026C5 RID: 9925
		public static GameObject projectilePrefab;

		// Token: 0x040026C6 RID: 9926
		public static GameObject swingEffectPrefab;

		// Token: 0x040026C7 RID: 9927
		private Transform rightMuzzleTransform;

		// Token: 0x040026C8 RID: 9928
		private Animator modelAnimator;

		// Token: 0x040026C9 RID: 9929
		private float duration;

		// Token: 0x040026CA RID: 9930
		private bool hasSwung;
	}
}
