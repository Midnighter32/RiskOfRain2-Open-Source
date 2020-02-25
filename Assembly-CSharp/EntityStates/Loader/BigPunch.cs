using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Loader
{
	// Token: 0x020007E0 RID: 2016
	public class BigPunch : LoaderMeleeAttack
	{
		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06002DE4 RID: 11748 RVA: 0x000C355C File Offset: 0x000C175C
		private Vector3 punchVector
		{
			get
			{
				return base.characterDirection.forward.normalized;
			}
		}

		// Token: 0x06002DE5 RID: 11749 RVA: 0x000C357C File Offset: 0x000C177C
		public override void OnEnter()
		{
			base.OnEnter();
			base.characterMotor.velocity.y = BigPunch.shorthopVelocityOnEnter;
		}

		// Token: 0x06002DE6 RID: 11750 RVA: 0x000C3599 File Offset: 0x000C1799
		protected override void PlayAnimation()
		{
			base.PlayAnimation();
			base.PlayAnimation("FullBody, Override", "BigPunch", "BigPunch.playbackRate", this.duration);
		}

		// Token: 0x06002DE7 RID: 11751 RVA: 0x000C35BC File Offset: 0x000C17BC
		protected override void AuthorityFixedUpdate()
		{
			base.AuthorityFixedUpdate();
			if (this.hasHit && !this.hasKnockbackedSelf && !base.authorityInHitPause)
			{
				this.hasKnockbackedSelf = true;
				base.healthComponent.TakeDamageForce(this.punchVector * -BigPunch.knockbackForce, true, false);
			}
		}

		// Token: 0x06002DE8 RID: 11752 RVA: 0x000C360C File Offset: 0x000C180C
		protected override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
		{
			base.AuthorityModifyOverlapAttack(overlapAttack);
			overlapAttack.maximumOverlapTargets = 1;
		}

		// Token: 0x06002DE9 RID: 11753 RVA: 0x000C361C File Offset: 0x000C181C
		protected override void OnMeleeHitAuthority()
		{
			if (this.hasHit)
			{
				return;
			}
			base.OnMeleeHitAuthority();
			this.hasHit = true;
			if (base.FindModelChild(this.swingEffectMuzzleString))
			{
				FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
				fireProjectileInfo.position = base.GetAimRay().origin;
				fireProjectileInfo.rotation = Quaternion.LookRotation(this.punchVector);
				fireProjectileInfo.crit = base.RollCrit();
				fireProjectileInfo.damage = 1f * this.damageStat;
				fireProjectileInfo.owner = base.gameObject;
				fireProjectileInfo.projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/LoaderZapCone");
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
		}

		// Token: 0x06002DEA RID: 11754 RVA: 0x000C36CC File Offset: 0x000C18CC
		private void FireSecondaryRaysServer()
		{
			Ray aimRay = base.GetAimRay();
			TeamIndex team = base.GetTeam();
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.teamMaskFilter = TeamMask.AllExcept(team);
			bullseyeSearch.maxAngleFilter = BigPunch.maxShockFOV * 0.5f;
			bullseyeSearch.maxDistanceFilter = BigPunch.maxShockDistance;
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = this.punchVector;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.RefreshCandidates();
			List<HurtBox> list = bullseyeSearch.GetResults().Where(new Func<HurtBox, bool>(Util.IsValid)).ToList<HurtBox>();
			Transform transform = base.FindModelChild(this.swingEffectMuzzleString);
			if (transform)
			{
				for (int i = 0; i < Mathf.Min(list.Count, BigPunch.maxShockCount); i++)
				{
					HurtBox hurtBox = list[i];
					if (hurtBox)
					{
						LightningOrb lightningOrb = new LightningOrb();
						lightningOrb.bouncedObjects = new List<HealthComponent>();
						lightningOrb.attacker = base.gameObject;
						lightningOrb.teamIndex = team;
						lightningOrb.damageValue = this.damageStat * BigPunch.shockDamageCoefficient;
						lightningOrb.isCrit = base.RollCrit();
						lightningOrb.origin = transform.position;
						lightningOrb.bouncesRemaining = 0;
						lightningOrb.lightningType = LightningOrb.LightningType.Loader;
						lightningOrb.procCoefficient = BigPunch.shockProcCoefficient;
						lightningOrb.target = hurtBox;
						OrbManager.instance.AddOrb(lightningOrb);
					}
				}
			}
		}

		// Token: 0x04002AEE RID: 10990
		public static int maxShockCount;

		// Token: 0x04002AEF RID: 10991
		public static float maxShockFOV;

		// Token: 0x04002AF0 RID: 10992
		public static float maxShockDistance;

		// Token: 0x04002AF1 RID: 10993
		public static float shockDamageCoefficient;

		// Token: 0x04002AF2 RID: 10994
		public static float shockProcCoefficient;

		// Token: 0x04002AF3 RID: 10995
		public static float knockbackForce;

		// Token: 0x04002AF4 RID: 10996
		public static float shorthopVelocityOnEnter;

		// Token: 0x04002AF5 RID: 10997
		private bool hasHit;

		// Token: 0x04002AF6 RID: 10998
		private bool hasKnockbackedSelf;
	}
}
