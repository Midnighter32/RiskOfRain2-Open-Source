using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008C2 RID: 2242
	public class FireSweepBarrage : BaseState
	{
		// Token: 0x06003247 RID: 12871 RVA: 0x000D9538 File Offset: 0x000D7738
		public override void OnEnter()
		{
			base.OnEnter();
			this.totalDuration = FireSweepBarrage.baseTotalDuration / this.attackSpeedStat;
			this.firingDuration = FireSweepBarrage.baseFiringDuration / this.attackSpeedStat;
			base.characterBody.SetAimTimer(3f);
			base.PlayAnimation("Gesture, Additive", "FireSweepBarrage", "FireSweepBarrage.playbackRate", this.totalDuration);
			base.PlayAnimation("Gesture, Override", "FireSweepBarrage", "FireSweepBarrage.playbackRate", this.totalDuration);
			Util.PlaySound(FireSweepBarrage.enterSound, base.gameObject);
			Ray aimRay = base.GetAimRay();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.teamMaskFilter = TeamMask.AllExcept(base.GetTeam());
			bullseyeSearch.maxAngleFilter = FireSweepBarrage.fieldOfView * 0.5f;
			bullseyeSearch.maxDistanceFilter = FireSweepBarrage.maxDistance;
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
			bullseyeSearch.filterByLoS = true;
			bullseyeSearch.RefreshCandidates();
			this.targetHurtboxes = bullseyeSearch.GetResults().Where(new Func<HurtBox, bool>(Util.IsValid)).Distinct(default(HurtBox.EntityEqualityComparer)).ToList<HurtBox>();
			this.totalBulletsToFire = Mathf.Max(this.targetHurtboxes.Count, FireSweepBarrage.minimumFireCount);
			this.timeBetweenBullets = this.firingDuration / (float)this.totalBulletsToFire;
			this.childLocator = base.GetModelTransform().GetComponent<ChildLocator>();
			this.muzzleIndex = this.childLocator.FindChildIndex(FireSweepBarrage.muzzle);
			this.muzzleTransform = this.childLocator.FindChild(this.muzzleIndex);
		}

		// Token: 0x06003248 RID: 12872 RVA: 0x000D96D0 File Offset: 0x000D78D0
		private void Fire()
		{
			if (this.totalBulletsFired < this.totalBulletsToFire)
			{
				if (!string.IsNullOrEmpty(FireSweepBarrage.muzzle))
				{
					EffectManager.SimpleMuzzleFlash(FireSweepBarrage.muzzleEffectPrefab, base.gameObject, FireSweepBarrage.muzzle, false);
				}
				Util.PlaySound(FireSweepBarrage.fireSoundString, base.gameObject);
				base.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
				if (NetworkServer.active && this.targetHurtboxes.Count > 0)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = this.damageStat * FireSweepBarrage.damageCoefficient;
					damageInfo.attacker = base.gameObject;
					damageInfo.procCoefficient = FireSweepBarrage.procCoefficient;
					damageInfo.crit = Util.CheckRoll(this.critStat, base.characterBody.master);
					if (this.targetHurtboxIndex >= this.targetHurtboxes.Count)
					{
						this.targetHurtboxIndex = 0;
					}
					HurtBox hurtBox = this.targetHurtboxes[this.targetHurtboxIndex];
					if (hurtBox)
					{
						HealthComponent healthComponent = hurtBox.healthComponent;
						if (healthComponent)
						{
							this.targetHurtboxIndex++;
							Vector3 normalized = (hurtBox.transform.position - base.characterBody.corePosition).normalized;
							damageInfo.force = FireSweepBarrage.force * normalized;
							damageInfo.position = hurtBox.transform.position;
							EffectManager.SimpleImpactEffect(FireSweepBarrage.impactEffectPrefab, hurtBox.transform.position, normalized, true);
							healthComponent.TakeDamage(damageInfo);
							GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
						}
						if (FireSweepBarrage.tracerEffectPrefab && this.childLocator)
						{
							int childIndex = this.childLocator.FindChildIndex(FireSweepBarrage.muzzle);
							this.childLocator.FindChild(childIndex);
							EffectData effectData = new EffectData
							{
								origin = hurtBox.transform.position,
								start = this.muzzleTransform.position
							};
							effectData.SetChildLocatorTransformReference(base.gameObject, childIndex);
							EffectManager.SpawnEffect(FireSweepBarrage.tracerEffectPrefab, effectData, true);
						}
					}
				}
				this.totalBulletsFired++;
			}
		}

		// Token: 0x06003249 RID: 12873 RVA: 0x000D98F0 File Offset: 0x000D7AF0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireTimer -= Time.fixedDeltaTime;
			if (this.fireTimer <= 0f)
			{
				this.Fire();
				this.fireTimer += this.timeBetweenBullets;
			}
			if (base.fixedAge >= this.totalDuration)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600324A RID: 12874 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04003128 RID: 12584
		public static string enterSound;

		// Token: 0x04003129 RID: 12585
		public static string muzzle;

		// Token: 0x0400312A RID: 12586
		public static string fireSoundString;

		// Token: 0x0400312B RID: 12587
		public static GameObject muzzleEffectPrefab;

		// Token: 0x0400312C RID: 12588
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400312D RID: 12589
		public static float baseTotalDuration;

		// Token: 0x0400312E RID: 12590
		public static float baseFiringDuration;

		// Token: 0x0400312F RID: 12591
		public static float fieldOfView;

		// Token: 0x04003130 RID: 12592
		public static float maxDistance;

		// Token: 0x04003131 RID: 12593
		public static float damageCoefficient;

		// Token: 0x04003132 RID: 12594
		public static float procCoefficient;

		// Token: 0x04003133 RID: 12595
		public static float force;

		// Token: 0x04003134 RID: 12596
		public static int minimumFireCount;

		// Token: 0x04003135 RID: 12597
		public static GameObject impactEffectPrefab;

		// Token: 0x04003136 RID: 12598
		private float totalDuration;

		// Token: 0x04003137 RID: 12599
		private float firingDuration;

		// Token: 0x04003138 RID: 12600
		private int totalBulletsToFire;

		// Token: 0x04003139 RID: 12601
		private int totalBulletsFired;

		// Token: 0x0400313A RID: 12602
		private int targetHurtboxIndex;

		// Token: 0x0400313B RID: 12603
		private float timeBetweenBullets;

		// Token: 0x0400313C RID: 12604
		private List<HurtBox> targetHurtboxes = new List<HurtBox>();

		// Token: 0x0400313D RID: 12605
		private float fireTimer;

		// Token: 0x0400313E RID: 12606
		private ChildLocator childLocator;

		// Token: 0x0400313F RID: 12607
		private int muzzleIndex;

		// Token: 0x04003140 RID: 12608
		private Transform muzzleTransform;
	}
}
