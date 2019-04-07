using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.VagrantMonster
{
	// Token: 0x0200012C RID: 300
	public class ExplosionAttack : BaseState
	{
		// Token: 0x060005C6 RID: 1478 RVA: 0x0001A60E File Offset: 0x0001880E
		public override void OnEnter()
		{
			base.OnEnter();
			this.explosionInterval = ExplosionAttack.baseDuration / (float)ExplosionAttack.explosionCount;
			this.explosionIndex = 0;
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x0001A630 File Offset: 0x00018830
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.explosionTimer -= Time.fixedDeltaTime;
			if (this.explosionTimer <= 0f)
			{
				if (this.explosionIndex >= ExplosionAttack.explosionCount)
				{
					if (base.isAuthority)
					{
						this.outer.SetNextStateToMain();
						return;
					}
				}
				else
				{
					this.explosionTimer += this.explosionInterval;
					this.Explode();
					this.explosionIndex++;
				}
			}
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x0001A6AC File Offset: 0x000188AC
		private void Explode()
		{
			float t = (float)this.explosionIndex / (float)(ExplosionAttack.explosionCount - 1);
			float num = Mathf.Lerp(ExplosionAttack.minRadius, ExplosionAttack.maxRadius, t);
			EffectManager.instance.SpawnEffect(ExplosionAttack.novaEffectPrefab, new EffectData
			{
				origin = base.transform.position,
				scale = num
			}, false);
			if (NetworkServer.active)
			{
				new BlastAttack
				{
					attacker = base.gameObject,
					inflictor = base.gameObject,
					teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
					baseDamage = this.damageStat * ExplosionAttack.damageCoefficient * Mathf.Pow(ExplosionAttack.damageScaling, (float)this.explosionIndex),
					baseForce = ExplosionAttack.force,
					position = base.transform.position,
					radius = num,
					falloffModel = BlastAttack.FalloffModel.None
				}.Fire();
			}
		}

		// Token: 0x04000697 RID: 1687
		public static float minRadius;

		// Token: 0x04000698 RID: 1688
		public static float maxRadius;

		// Token: 0x04000699 RID: 1689
		public static int explosionCount;

		// Token: 0x0400069A RID: 1690
		public static float baseDuration;

		// Token: 0x0400069B RID: 1691
		public static float damageCoefficient;

		// Token: 0x0400069C RID: 1692
		public static float force;

		// Token: 0x0400069D RID: 1693
		public static float damageScaling;

		// Token: 0x0400069E RID: 1694
		public static GameObject novaEffectPrefab;

		// Token: 0x0400069F RID: 1695
		private float explosionTimer;

		// Token: 0x040006A0 RID: 1696
		private float explosionInterval;

		// Token: 0x040006A1 RID: 1697
		private int explosionIndex;
	}
}
