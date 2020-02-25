using System;
using RoR2;
using RoR2.Mecanim;
using UnityEngine.Networking;

namespace EntityStates.Treebot.UnlockInteractable
{
	// Token: 0x02000757 RID: 1879
	public class Unlock : BaseState
	{
		// Token: 0x1400008B RID: 139
		// (add) Token: 0x06002B7C RID: 11132 RVA: 0x000B76EC File Offset: 0x000B58EC
		// (remove) Token: 0x06002B7D RID: 11133 RVA: 0x000B7720 File Offset: 0x000B5920
		public static event Action<Interactor> onActivated;

		// Token: 0x06002B7E RID: 11134 RVA: 0x000B7754 File Offset: 0x000B5954
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				Action<Interactor> action = Unlock.onActivated;
				if (action != null)
				{
					action(base.GetComponent<PurchaseInteraction>().lastActivator);
				}
			}
			base.GetModelAnimator().SetBool("Revive", true);
			base.GetModelTransform().GetComponent<RandomBlinkController>().enabled = true;
		}
	}
}
