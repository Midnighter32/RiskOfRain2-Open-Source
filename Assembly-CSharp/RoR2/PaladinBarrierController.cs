using System;
using RoR2.Orbs;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000381 RID: 897
	public class PaladinBarrierController : MonoBehaviour, IBarrier
	{
		// Token: 0x060012B1 RID: 4785 RVA: 0x0005BB98 File Offset: 0x00059D98
		public void BlockedDamage(DamageInfo damageInfo, float actualDamageBlocked)
		{
			this.totalDamageBlocked += actualDamageBlocked;
			LightningOrb lightningOrb = new LightningOrb();
			lightningOrb.teamIndex = this.teamComponent.teamIndex;
			lightningOrb.origin = damageInfo.position;
			lightningOrb.damageValue = actualDamageBlocked * this.blockLaserDamageCoefficient;
			lightningOrb.bouncesRemaining = 0;
			lightningOrb.attacker = damageInfo.attacker;
			lightningOrb.procCoefficient = this.blockLaserProcCoefficient;
			lightningOrb.lightningType = LightningOrb.LightningType.PaladinBarrier;
			HurtBox hurtBox = lightningOrb.PickNextTarget(lightningOrb.origin);
			if (hurtBox)
			{
				lightningOrb.target = hurtBox;
				lightningOrb.isCrit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
				OrbManager.instance.AddOrb(lightningOrb);
			}
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x0005BC52 File Offset: 0x00059E52
		public void EnableBarrier()
		{
			this.barrierPivotTransform.gameObject.SetActive(true);
			this.barrierIsOn = true;
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x0005BC6C File Offset: 0x00059E6C
		public void DisableBarrier()
		{
			this.barrierPivotTransform.gameObject.SetActive(false);
			this.barrierIsOn = false;
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x0005BC86 File Offset: 0x00059E86
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
			this.characterBody = base.GetComponent<CharacterBody>();
			this.teamComponent = base.GetComponent<TeamComponent>();
			this.DisableBarrier();
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x0005BCB2 File Offset: 0x00059EB2
		private void Update()
		{
			if (this.barrierIsOn)
			{
				this.barrierPivotTransform.rotation = Util.QuaternionSafeLookRotation(this.inputBank.aimDirection);
			}
		}

		// Token: 0x0400167E RID: 5758
		public float blockLaserDamageCoefficient;

		// Token: 0x0400167F RID: 5759
		public float blockLaserProcCoefficient;

		// Token: 0x04001680 RID: 5760
		public float blockLaserDistance;

		// Token: 0x04001681 RID: 5761
		private float totalDamageBlocked;

		// Token: 0x04001682 RID: 5762
		private CharacterBody characterBody;

		// Token: 0x04001683 RID: 5763
		private InputBankTest inputBank;

		// Token: 0x04001684 RID: 5764
		private TeamComponent teamComponent;

		// Token: 0x04001685 RID: 5765
		private bool barrierIsOn;

		// Token: 0x04001686 RID: 5766
		public Transform barrierPivotTransform;
	}
}
