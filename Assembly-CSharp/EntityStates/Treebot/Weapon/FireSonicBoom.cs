using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x02000753 RID: 1875
	public class FireSonicBoom : BaseState
	{
		// Token: 0x06002B69 RID: 11113 RVA: 0x000B6F38 File Offset: 0x000B5138
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireSonicBoom");
			Util.PlaySound(this.sound, base.gameObject);
			Ray aimRay = base.GetAimRay();
			if (!string.IsNullOrEmpty(this.muzzle))
			{
				EffectManager.SimpleMuzzleFlash(this.fireEffectPrefab, base.gameObject, this.muzzle, false);
			}
			else
			{
				EffectManager.SpawnEffect(this.fireEffectPrefab, new EffectData
				{
					origin = aimRay.origin,
					rotation = Quaternion.LookRotation(aimRay.direction)
				}, false);
			}
			aimRay.origin -= aimRay.direction * this.backupDistance;
			if (NetworkServer.active)
			{
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.teamMaskFilter = TeamMask.AllExcept(base.GetTeam());
				bullseyeSearch.maxAngleFilter = this.fieldOfView * 0.5f;
				bullseyeSearch.maxDistanceFilter = this.maxDistance;
				bullseyeSearch.searchOrigin = aimRay.origin;
				bullseyeSearch.searchDirection = aimRay.direction;
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
				bullseyeSearch.filterByLoS = false;
				bullseyeSearch.RefreshCandidates();
				foreach (HurtBox hurtBox in bullseyeSearch.GetResults().Where(new Func<HurtBox, bool>(Util.IsValid)).Distinct(default(HurtBox.EntityEqualityComparer)))
				{
					Vector3 vector = hurtBox.transform.position - aimRay.origin;
					float magnitude = vector.magnitude;
					float magnitude2 = new Vector2(vector.x, vector.z).magnitude;
					Vector3 vector2 = vector / magnitude;
					float num = 1f;
					CharacterBody body = hurtBox.healthComponent.body;
					if (body.characterMotor)
					{
						num = body.characterMotor.mass;
					}
					else if (hurtBox.healthComponent.GetComponent<Rigidbody>())
					{
						num = base.rigidbody.mass;
					}
					float num2 = FireSonicBoom.shoveSuitabilityCurve.Evaluate(num);
					this.AddDebuff(body);
					body.RecalculateStats();
					float acceleration = body.acceleration;
					Vector3 a = vector2;
					float d = Trajectory.CalculateInitialYSpeedForHeight(Mathf.Abs(this.idealDistanceToPlaceTargets - magnitude), -acceleration) * Mathf.Sign(this.idealDistanceToPlaceTargets - magnitude);
					a *= d;
					a.y = this.liftVelocity;
					hurtBox.healthComponent.TakeDamageForce(a * (num * num2), true, true);
					hurtBox.healthComponent.TakeDamage(new DamageInfo
					{
						attacker = base.gameObject,
						damage = this.CalculateDamage(),
						position = hurtBox.transform.position,
						procCoefficient = this.CalculateProcCoefficient()
					});
				}
			}
			if (base.isAuthority && base.characterBody && base.characterBody.characterMotor)
			{
				float height = base.characterBody.characterMotor.isGrounded ? this.groundKnockbackDistance : this.airKnockbackDistance;
				float num3 = base.characterBody.characterMotor ? base.characterBody.characterMotor.mass : 1f;
				float acceleration2 = base.characterBody.acceleration;
				float num4 = Trajectory.CalculateInitialYSpeedForHeight(height, -acceleration2);
				base.characterBody.characterMotor.ApplyForce(-num4 * num3 * aimRay.direction, false, false);
			}
		}

		// Token: 0x06002B6A RID: 11114 RVA: 0x000B72F0 File Offset: 0x000B54F0
		protected virtual void AddDebuff(CharacterBody body)
		{
			body.AddTimedBuff(BuffIndex.Weak, this.slowDuration);
			SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
			if (component == null)
			{
				return;
			}
			component.SetStun(-1f);
		}

		// Token: 0x06002B6B RID: 11115 RVA: 0x000AA35A File Offset: 0x000A855A
		protected virtual float CalculateDamage()
		{
			return 0f;
		}

		// Token: 0x06002B6C RID: 11116 RVA: 0x000AA35A File Offset: 0x000A855A
		protected virtual float CalculateProcCoefficient()
		{
			return 0f;
		}

		// Token: 0x06002B6D RID: 11117 RVA: 0x000B731A File Offset: 0x000B551A
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002B6E RID: 11118 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002763 RID: 10083
		[SerializeField]
		public string sound;

		// Token: 0x04002764 RID: 10084
		[SerializeField]
		public string muzzle;

		// Token: 0x04002765 RID: 10085
		[SerializeField]
		public GameObject fireEffectPrefab;

		// Token: 0x04002766 RID: 10086
		[SerializeField]
		public float baseDuration;

		// Token: 0x04002767 RID: 10087
		[SerializeField]
		public float fieldOfView;

		// Token: 0x04002768 RID: 10088
		[SerializeField]
		public float backupDistance;

		// Token: 0x04002769 RID: 10089
		[SerializeField]
		public float maxDistance;

		// Token: 0x0400276A RID: 10090
		[SerializeField]
		public float idealDistanceToPlaceTargets;

		// Token: 0x0400276B RID: 10091
		[SerializeField]
		public float liftVelocity;

		// Token: 0x0400276C RID: 10092
		[SerializeField]
		public float slowDuration;

		// Token: 0x0400276D RID: 10093
		[SerializeField]
		public float groundKnockbackDistance;

		// Token: 0x0400276E RID: 10094
		[SerializeField]
		public float airKnockbackDistance;

		// Token: 0x0400276F RID: 10095
		public static AnimationCurve shoveSuitabilityCurve;

		// Token: 0x04002770 RID: 10096
		private float duration;
	}
}
