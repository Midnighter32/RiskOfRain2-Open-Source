using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x020000D7 RID: 215
	public class Throw : BaseState
	{
		// Token: 0x0600043D RID: 1085 RVA: 0x00011898 File Offset: 0x0000FA98
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

		// Token: 0x0600043E RID: 1086 RVA: 0x000119AC File Offset: 0x0000FBAC
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
				EffectManager.instance.SimpleMuzzleFlash(Throw.swingEffectPrefab, base.gameObject, "RightSwingCenter", true);
				this.hasSwung = true;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00011ADC File Offset: 0x0000FCDC
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

		// Token: 0x040003FA RID: 1018
		public static float baseDuration = 3.5f;

		// Token: 0x040003FB RID: 1019
		public static float returnToIdlePercentage;

		// Token: 0x040003FC RID: 1020
		public static float damageCoefficient = 4f;

		// Token: 0x040003FD RID: 1021
		public static float forceMagnitude = 16f;

		// Token: 0x040003FE RID: 1022
		public static float radius = 3f;

		// Token: 0x040003FF RID: 1023
		public static GameObject projectilePrefab;

		// Token: 0x04000400 RID: 1024
		public static GameObject swingEffectPrefab;

		// Token: 0x04000401 RID: 1025
		private Transform rightMuzzleTransform;

		// Token: 0x04000402 RID: 1026
		private Animator modelAnimator;

		// Token: 0x04000403 RID: 1027
		private float duration;

		// Token: 0x04000404 RID: 1028
		private bool hasSwung;
	}
}
