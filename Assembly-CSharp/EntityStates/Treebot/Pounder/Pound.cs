using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Treebot.Pounder
{
	// Token: 0x0200075B RID: 1883
	public class Pound : BaseState
	{
		// Token: 0x06002B8E RID: 11150 RVA: 0x000B7E8D File Offset: 0x000B608D
		public override void OnEnter()
		{
			base.OnEnter();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06002B8F RID: 11151 RVA: 0x000B7EA4 File Offset: 0x000B60A4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.poundTimer -= Time.fixedDeltaTime;
			if (this.poundTimer <= 0f && base.projectileController.owner)
			{
				this.poundTimer += 1f / Pound.blastFrequency;
				if (NetworkServer.active)
				{
					new BlastAttack
					{
						attacker = base.projectileController.owner,
						baseDamage = this.projectileDamage.damage,
						baseForce = Pound.blastForce,
						crit = this.projectileDamage.crit,
						damageType = this.projectileDamage.damageType,
						falloffModel = BlastAttack.FalloffModel.None,
						position = base.transform.position,
						radius = Pound.blastRadius,
						teamIndex = base.projectileController.teamFilter.teamIndex
					}.Fire();
					EffectManager.SpawnEffect(Pound.blastEffectPrefab, new EffectData
					{
						origin = base.transform.position,
						scale = Pound.blastRadius
					}, true);
				}
				base.PlayAnimation("Base", "Pound");
			}
			if (NetworkServer.active && base.fixedAge > Pound.duration)
			{
				EntityState.Destroy(base.gameObject);
			}
		}

		// Token: 0x040027A3 RID: 10147
		public static float blastRadius;

		// Token: 0x040027A4 RID: 10148
		public static float blastProcCoefficient;

		// Token: 0x040027A5 RID: 10149
		public static float blastForce;

		// Token: 0x040027A6 RID: 10150
		public static float blastFrequency;

		// Token: 0x040027A7 RID: 10151
		public static float duration;

		// Token: 0x040027A8 RID: 10152
		public static GameObject blastEffectPrefab;

		// Token: 0x040027A9 RID: 10153
		private ProjectileDamage projectileDamage;

		// Token: 0x040027AA RID: 10154
		private float poundTimer;
	}
}
