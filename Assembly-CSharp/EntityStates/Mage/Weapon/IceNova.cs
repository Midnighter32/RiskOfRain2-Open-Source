using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x020007DC RID: 2012
	public class IceNova : BaseState
	{
		// Token: 0x06002DD0 RID: 11728 RVA: 0x000C2E6C File Offset: 0x000C106C
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.endDuration = IceNova.baseEndDuration / this.attackSpeedStat;
			this.startDuration = IceNova.baseStartDuration / this.attackSpeedStat;
		}

		// Token: 0x06002DD1 RID: 11729 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002DD2 RID: 11730 RVA: 0x000C2EA4 File Offset: 0x000C10A4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.startDuration && !this.hasCastNova)
			{
				this.hasCastNova = true;
				EffectManager.SpawnEffect(IceNova.novaEffectPrefab, new EffectData
				{
					origin = base.transform.position,
					scale = IceNova.novaRadius
				}, true);
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.radius = IceNova.novaRadius;
				blastAttack.procCoefficient = IceNova.procCoefficient;
				blastAttack.position = base.transform.position;
				blastAttack.attacker = base.gameObject;
				blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
				blastAttack.baseDamage = base.characterBody.damage * IceNova.damageCoefficient;
				blastAttack.falloffModel = BlastAttack.FalloffModel.None;
				blastAttack.damageType = DamageType.Freeze2s;
				blastAttack.baseForce = IceNova.force;
				blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
				blastAttack.Fire();
			}
			if (this.stopwatch >= this.startDuration + this.endDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002DD3 RID: 11731 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002AC8 RID: 10952
		public static GameObject impactEffectPrefab;

		// Token: 0x04002AC9 RID: 10953
		public static GameObject novaEffectPrefab;

		// Token: 0x04002ACA RID: 10954
		public static float baseStartDuration;

		// Token: 0x04002ACB RID: 10955
		public static float baseEndDuration = 2f;

		// Token: 0x04002ACC RID: 10956
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002ACD RID: 10957
		public static float procCoefficient;

		// Token: 0x04002ACE RID: 10958
		public static float force = 20f;

		// Token: 0x04002ACF RID: 10959
		public static float novaRadius;

		// Token: 0x04002AD0 RID: 10960
		public static string attackString;

		// Token: 0x04002AD1 RID: 10961
		private float stopwatch;

		// Token: 0x04002AD2 RID: 10962
		private float startDuration;

		// Token: 0x04002AD3 RID: 10963
		private float endDuration;

		// Token: 0x04002AD4 RID: 10964
		private bool hasCastNova;
	}
}
