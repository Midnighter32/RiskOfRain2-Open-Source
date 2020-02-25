using System;
using RoR2.Orbs;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002B9 RID: 697
	public class PaladinBarrierController : MonoBehaviour, IBarrier
	{
		// Token: 0x06000FBD RID: 4029 RVA: 0x000451FC File Offset: 0x000433FC
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
			lightningOrb.lightningType = LightningOrb.LightningType.TreePoisonDart;
			HurtBox hurtBox = lightningOrb.PickNextTarget(lightningOrb.origin);
			if (hurtBox)
			{
				lightningOrb.target = hurtBox;
				lightningOrb.isCrit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
				OrbManager.instance.AddOrb(lightningOrb);
			}
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x000452B6 File Offset: 0x000434B6
		public void EnableBarrier()
		{
			this.barrierPivotTransform.gameObject.SetActive(true);
			this.barrierIsOn = true;
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x000452D0 File Offset: 0x000434D0
		public void DisableBarrier()
		{
			this.barrierPivotTransform.gameObject.SetActive(false);
			this.barrierIsOn = false;
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x000452EA File Offset: 0x000434EA
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
			this.characterBody = base.GetComponent<CharacterBody>();
			this.teamComponent = base.GetComponent<TeamComponent>();
			this.DisableBarrier();
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x00045316 File Offset: 0x00043516
		private void Update()
		{
			if (this.barrierIsOn)
			{
				this.barrierPivotTransform.rotation = Util.QuaternionSafeLookRotation(this.inputBank.aimDirection);
			}
		}

		// Token: 0x04000F3B RID: 3899
		public float blockLaserDamageCoefficient;

		// Token: 0x04000F3C RID: 3900
		public float blockLaserProcCoefficient;

		// Token: 0x04000F3D RID: 3901
		public float blockLaserDistance;

		// Token: 0x04000F3E RID: 3902
		private float totalDamageBlocked;

		// Token: 0x04000F3F RID: 3903
		private CharacterBody characterBody;

		// Token: 0x04000F40 RID: 3904
		private InputBankTest inputBank;

		// Token: 0x04000F41 RID: 3905
		private TeamComponent teamComponent;

		// Token: 0x04000F42 RID: 3906
		private bool barrierIsOn;

		// Token: 0x04000F43 RID: 3907
		public Transform barrierPivotTransform;
	}
}
