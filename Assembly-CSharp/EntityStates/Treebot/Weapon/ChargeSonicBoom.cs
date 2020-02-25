using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x0200074E RID: 1870
	public class ChargeSonicBoom : BaseState
	{
		// Token: 0x06002B51 RID: 11089 RVA: 0x000B68E8 File Offset: 0x000B4AE8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeSonicBoom.baseDuration / this.attackSpeedStat;
			Util.PlaySound(this.sound, base.gameObject);
			base.characterBody.SetAimTimer(3f);
			if (this.chargeEffectPrefab)
			{
				Transform transform = base.FindModelChild(ChargeSonicBoom.muzzleName);
				if (transform)
				{
					this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(this.chargeEffectPrefab, transform);
					this.chargeEffect.transform.localPosition = Vector3.zero;
					this.chargeEffect.transform.localRotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x06002B52 RID: 11090 RVA: 0x000B698C File Offset: 0x000B4B8C
		public override void OnExit()
		{
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
				this.chargeEffect = null;
			}
			base.OnExit();
		}

		// Token: 0x06002B53 RID: 11091 RVA: 0x000B69B3 File Offset: 0x000B4BB3
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextState(this.GetNextState());
				return;
			}
		}

		// Token: 0x06002B54 RID: 11092 RVA: 0x000B69DB File Offset: 0x000B4BDB
		protected virtual EntityState GetNextState()
		{
			return new FireSonicBoom();
		}

		// Token: 0x06002B55 RID: 11093 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400273F RID: 10047
		[SerializeField]
		public string sound;

		// Token: 0x04002740 RID: 10048
		[SerializeField]
		public GameObject chargeEffectPrefab;

		// Token: 0x04002741 RID: 10049
		public static string muzzleName;

		// Token: 0x04002742 RID: 10050
		public static float baseDuration;

		// Token: 0x04002743 RID: 10051
		private float duration;

		// Token: 0x04002744 RID: 10052
		private GameObject chargeEffect;
	}
}
