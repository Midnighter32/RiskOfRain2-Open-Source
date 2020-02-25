using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x02000761 RID: 1889
	public class FireBuzzsaw : BaseState
	{
		// Token: 0x06002BA3 RID: 11171 RVA: 0x000B8384 File Offset: 0x000B6584
		public override void OnEnter()
		{
			base.OnEnter();
			this.fireFrequency = FireBuzzsaw.baseFireFrequency * this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			Util.PlaySound(FireBuzzsaw.spinUpSoundString, base.gameObject);
			Util.PlaySound(FireBuzzsaw.fireSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive Gun", "SpinBuzzsaw");
			base.PlayAnimation("Gesture, Additive", "EnterBuzzsaw");
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = FireBuzzsaw.damageCoefficientPerSecond * this.damageStat / FireBuzzsaw.baseFireFrequency;
			this.attack.procCoefficient = FireBuzzsaw.procCoefficientPerSecond / FireBuzzsaw.baseFireFrequency;
			if (FireBuzzsaw.impactEffectPrefab)
			{
				this.attack.hitEffectPrefab = FireBuzzsaw.impactEffectPrefab;
			}
			if (modelTransform)
			{
				this.attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Buzzsaw");
			}
			this.muzzleTransform = base.FindModelChild("MuzzleBuzzsaw");
			if (this.muzzleTransform)
			{
				if (FireBuzzsaw.spinEffectPrefab)
				{
					this.spinEffectInstance = UnityEngine.Object.Instantiate<GameObject>(FireBuzzsaw.spinEffectPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
					this.spinEffectInstance.transform.parent = this.muzzleTransform;
					this.spinEffectInstance.transform.localScale = Vector3.one;
				}
				if (FireBuzzsaw.spinImpactEffectPrefab)
				{
					this.spinImpactEffectInstance = UnityEngine.Object.Instantiate<GameObject>(FireBuzzsaw.spinImpactEffectPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
					this.spinImpactEffectInstance.transform.parent = this.muzzleTransform;
					this.spinImpactEffectInstance.transform.localScale = Vector3.one;
					this.spinImpactEffectInstance.gameObject.SetActive(false);
				}
			}
			this.attack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
		}

		// Token: 0x06002BA4 RID: 11172 RVA: 0x000B85D4 File Offset: 0x000B67D4
		public override void OnExit()
		{
			base.OnExit();
			Util.PlaySound(FireBuzzsaw.spinDownSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive Gun", "Empty");
			base.PlayAnimation("Gesture, Additive", "ExitBuzzsaw");
			if (this.spinEffectInstance)
			{
				EntityState.Destroy(this.spinEffectInstance);
			}
			if (this.spinImpactEffectInstance)
			{
				EntityState.Destroy(this.spinImpactEffectInstance);
			}
		}

		// Token: 0x06002BA5 RID: 11173 RVA: 0x000B8648 File Offset: 0x000B6848
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireAge += Time.fixedDeltaTime;
			base.characterBody.SetAimTimer(2f);
			this.attackSpeedStat = base.characterBody.attackSpeed;
			this.fireFrequency = FireBuzzsaw.baseFireFrequency * this.attackSpeedStat;
			if (this.fireAge >= 1f / this.fireFrequency && base.isAuthority)
			{
				this.fireAge = 0f;
				this.attack.ResetIgnoredHealthComponents();
				this.attack.isCrit = base.characterBody.RollCrit();
				this.hitOverlapLastTick = this.attack.Fire(null);
				if (this.hitOverlapLastTick)
				{
					Vector3 normalized = (this.attack.lastFireAverageHitPosition - base.GetAimRay().origin).normalized;
					base.healthComponent.TakeDamageForce(normalized * FireBuzzsaw.selfForceMagnitude, false, false);
					Util.PlaySound(FireBuzzsaw.impactSoundString, base.gameObject);
					base.PlayAnimation("Gesture, Additive", "ImpactBuzzsaw");
				}
				base.characterBody.AddSpreadBloom(FireBuzzsaw.spreadBloomValue);
				if (!base.inputBank.skill1.down)
				{
					this.outer.SetNextStateToMain();
				}
			}
			this.spinImpactEffectInstance.gameObject.SetActive(this.hitOverlapLastTick);
		}

		// Token: 0x06002BA6 RID: 11174 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040027BD RID: 10173
		public static float damageCoefficientPerSecond;

		// Token: 0x040027BE RID: 10174
		public static float procCoefficientPerSecond = 1f;

		// Token: 0x040027BF RID: 10175
		public static string fireSoundString;

		// Token: 0x040027C0 RID: 10176
		public static string impactSoundString;

		// Token: 0x040027C1 RID: 10177
		public static string spinUpSoundString;

		// Token: 0x040027C2 RID: 10178
		public static string spinDownSoundString;

		// Token: 0x040027C3 RID: 10179
		public static float spreadBloomValue = 0.2f;

		// Token: 0x040027C4 RID: 10180
		public static float baseFireFrequency;

		// Token: 0x040027C5 RID: 10181
		public static GameObject spinEffectPrefab;

		// Token: 0x040027C6 RID: 10182
		public static GameObject spinImpactEffectPrefab;

		// Token: 0x040027C7 RID: 10183
		public static GameObject impactEffectPrefab;

		// Token: 0x040027C8 RID: 10184
		public static float selfForceMagnitude;

		// Token: 0x040027C9 RID: 10185
		private OverlapAttack attack;

		// Token: 0x040027CA RID: 10186
		private float fireFrequency;

		// Token: 0x040027CB RID: 10187
		private float fireAge;

		// Token: 0x040027CC RID: 10188
		private GameObject spinEffectInstance;

		// Token: 0x040027CD RID: 10189
		private GameObject spinImpactEffectInstance;

		// Token: 0x040027CE RID: 10190
		private Transform muzzleTransform;

		// Token: 0x040027CF RID: 10191
		private bool hitOverlapLastTick;
	}
}
