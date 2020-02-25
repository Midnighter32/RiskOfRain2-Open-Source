using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.VagrantMonster
{
	// Token: 0x02000802 RID: 2050
	public class ExplosionAttack : BaseState
	{
		// Token: 0x06002E9D RID: 11933 RVA: 0x000C634D File Offset: 0x000C454D
		public override void OnEnter()
		{
			base.OnEnter();
			this.explosionInterval = ExplosionAttack.baseDuration / (float)ExplosionAttack.explosionCount;
			this.explosionIndex = 0;
		}

		// Token: 0x06002E9E RID: 11934 RVA: 0x000C6370 File Offset: 0x000C4570
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

		// Token: 0x06002E9F RID: 11935 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002EA0 RID: 11936 RVA: 0x000C63EC File Offset: 0x000C45EC
		private void Explode()
		{
			float t = (float)this.explosionIndex / (float)(ExplosionAttack.explosionCount - 1);
			float num = Mathf.Lerp(ExplosionAttack.minRadius, ExplosionAttack.maxRadius, t);
			EffectManager.SpawnEffect(ExplosionAttack.novaEffectPrefab, new EffectData
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

		// Token: 0x04002BBB RID: 11195
		public static float minRadius;

		// Token: 0x04002BBC RID: 11196
		public static float maxRadius;

		// Token: 0x04002BBD RID: 11197
		public static int explosionCount;

		// Token: 0x04002BBE RID: 11198
		public static float baseDuration;

		// Token: 0x04002BBF RID: 11199
		public static float damageCoefficient;

		// Token: 0x04002BC0 RID: 11200
		public static float force;

		// Token: 0x04002BC1 RID: 11201
		public static float damageScaling;

		// Token: 0x04002BC2 RID: 11202
		public static GameObject novaEffectPrefab;

		// Token: 0x04002BC3 RID: 11203
		private float explosionTimer;

		// Token: 0x04002BC4 RID: 11204
		private float explosionInterval;

		// Token: 0x04002BC5 RID: 11205
		private int explosionIndex;
	}
}
