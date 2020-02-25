using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x0200074A RID: 1866
	public class AimFlower : AimThrowableBase
	{
		// Token: 0x06002B42 RID: 11074 RVA: 0x000B6628 File Offset: 0x000B4828
		public override void Update()
		{
			base.Update();
			this.keyDown &= !base.inputBank.skill1.down;
		}

		// Token: 0x06002B43 RID: 11075 RVA: 0x000B6650 File Offset: 0x000B4850
		protected override bool KeyIsDown()
		{
			return this.keyDown;
		}

		// Token: 0x06002B44 RID: 11076 RVA: 0x0000C68F File Offset: 0x0000A88F
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		// Token: 0x06002B45 RID: 11077 RVA: 0x000B6658 File Offset: 0x000B4858
		protected override void FireProjectile()
		{
			if (base.healthComponent)
			{
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = base.healthComponent.combinedHealth * AimFlower.healthCostFraction;
				damageInfo.position = base.characterBody.corePosition;
				damageInfo.force = Vector3.zero;
				damageInfo.damageColorIndex = DamageColorIndex.Default;
				damageInfo.crit = false;
				damageInfo.attacker = null;
				damageInfo.inflictor = null;
				damageInfo.damageType = DamageType.NonLethal;
				damageInfo.procCoefficient = 0f;
				damageInfo.procChainMask = default(ProcChainMask);
				base.healthComponent.TakeDamage(damageInfo);
			}
			base.FireProjectile();
		}

		// Token: 0x04002736 RID: 10038
		public static float healthCostFraction;

		// Token: 0x04002737 RID: 10039
		private bool keyDown = true;
	}
}
