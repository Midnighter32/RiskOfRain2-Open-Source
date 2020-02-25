using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.LockedMage
{
	// Token: 0x020007DE RID: 2014
	public class UnlockingMage : BaseState
	{
		// Token: 0x1400008E RID: 142
		// (add) Token: 0x06002DDD RID: 11741 RVA: 0x000C33D0 File Offset: 0x000C15D0
		// (remove) Token: 0x06002DDE RID: 11742 RVA: 0x000C3404 File Offset: 0x000C1604
		public static event Action<Interactor> onOpened;

		// Token: 0x06002DDF RID: 11743 RVA: 0x000C3438 File Offset: 0x000C1638
		public override void OnEnter()
		{
			base.OnEnter();
			EffectManager.SimpleEffect(UnlockingMage.unlockingMageChargeEffectPrefab, base.transform.position, Util.QuaternionSafeLookRotation(Vector3.up), false);
			Util.PlayScaledSound(UnlockingMage.unlockingChargeSFXString, base.gameObject, UnlockingMage.unlockingChargeSFXStringPitch);
			base.GetModelTransform().GetComponent<ChildLocator>().FindChild("Suspension").gameObject.SetActive(false);
		}

		// Token: 0x06002DE0 RID: 11744 RVA: 0x000C34A4 File Offset: 0x000C16A4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= UnlockingMage.unlockingDuration && !this.unlocked)
			{
				base.gameObject.SetActive(false);
				EffectManager.SimpleEffect(UnlockingMage.unlockingMageExplosionEffectPrefab, base.transform.position, Util.QuaternionSafeLookRotation(Vector3.up), false);
				Util.PlayScaledSound(UnlockingMage.unlockingExplosionSFXString, base.gameObject, UnlockingMage.unlockingExplosionSFXStringPitch);
				this.unlocked = true;
				if (NetworkServer.active)
				{
					Action<Interactor> action = UnlockingMage.onOpened;
					if (action == null)
					{
						return;
					}
					action(base.GetComponent<PurchaseInteraction>().lastActivator);
				}
			}
		}

		// Token: 0x04002AE5 RID: 10981
		public static GameObject unlockingMageChargeEffectPrefab;

		// Token: 0x04002AE6 RID: 10982
		public static GameObject unlockingMageExplosionEffectPrefab;

		// Token: 0x04002AE7 RID: 10983
		public static float unlockingDuration;

		// Token: 0x04002AE8 RID: 10984
		public static string unlockingChargeSFXString;

		// Token: 0x04002AE9 RID: 10985
		public static float unlockingChargeSFXStringPitch;

		// Token: 0x04002AEA RID: 10986
		public static string unlockingExplosionSFXString;

		// Token: 0x04002AEB RID: 10987
		public static float unlockingExplosionSFXStringPitch;

		// Token: 0x04002AED RID: 10989
		private bool unlocked;
	}
}
