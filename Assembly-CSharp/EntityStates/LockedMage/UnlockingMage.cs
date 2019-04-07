using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.LockedMage
{
	// Token: 0x0200011D RID: 285
	public class UnlockingMage : BaseState
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600057C RID: 1404 RVA: 0x00019090 File Offset: 0x00017290
		// (remove) Token: 0x0600057D RID: 1405 RVA: 0x000190C4 File Offset: 0x000172C4
		public static event Action<Interactor> onOpened;

		// Token: 0x0600057E RID: 1406 RVA: 0x000190F8 File Offset: 0x000172F8
		public override void OnEnter()
		{
			base.OnEnter();
			EffectManager.instance.SimpleEffect(UnlockingMage.unlockingMageChargeEffectPrefab, base.transform.position, Util.QuaternionSafeLookRotation(Vector3.up), false);
			Util.PlayScaledSound(UnlockingMage.unlockingChargeSFXString, base.gameObject, UnlockingMage.unlockingChargeSFXStringPitch);
			base.GetModelTransform().GetComponent<ChildLocator>().FindChild("Suspension").gameObject.SetActive(false);
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x00019168 File Offset: 0x00017368
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= UnlockingMage.unlockingDuration && !this.unlocked)
			{
				base.gameObject.SetActive(false);
				EffectManager.instance.SimpleEffect(UnlockingMage.unlockingMageExplosionEffectPrefab, base.transform.position, Util.QuaternionSafeLookRotation(Vector3.up), false);
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

		// Token: 0x04000628 RID: 1576
		public static GameObject unlockingMageChargeEffectPrefab;

		// Token: 0x04000629 RID: 1577
		public static GameObject unlockingMageExplosionEffectPrefab;

		// Token: 0x0400062A RID: 1578
		public static float unlockingDuration;

		// Token: 0x0400062B RID: 1579
		public static string unlockingChargeSFXString;

		// Token: 0x0400062C RID: 1580
		public static float unlockingChargeSFXStringPitch;

		// Token: 0x0400062D RID: 1581
		public static string unlockingExplosionSFXString;

		// Token: 0x0400062E RID: 1582
		public static float unlockingExplosionSFXStringPitch;

		// Token: 0x04000630 RID: 1584
		private bool unlocked;
	}
}
