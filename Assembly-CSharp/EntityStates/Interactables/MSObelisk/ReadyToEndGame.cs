using System;
using System.Collections.ObjectModel;
using RoR2;

namespace EntityStates.Interactables.MSObelisk
{
	// Token: 0x0200080E RID: 2062
	public class ReadyToEndGame : BaseState
	{
		// Token: 0x06002ED8 RID: 11992 RVA: 0x000C76F4 File Offset: 0x000C58F4
		public override void OnEnter()
		{
			base.OnEnter();
			this.childLocator = base.GetComponent<ChildLocator>();
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
			this.purchaseInteraction.contextToken = "MSOBELISK_CONTEXT_CONFIRMATION";
			this.purchaseInteraction.Networkavailable = false;
			this.childLocator.FindChild(ReadyToEndGame.chargeupChildString).gameObject.SetActive(true);
			Util.PlaySound(ReadyToEndGame.chargeupSoundString, base.gameObject);
			ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
			for (int i = 0; i < instances.Count; i++)
			{
				instances[i].master.preventGameOver = true;
			}
			for (int j = 0; j < CameraRigController.readOnlyInstancesList.Count; j++)
			{
				CameraRigController.readOnlyInstancesList[j].disableSpectating = true;
			}
		}

		// Token: 0x06002ED9 RID: 11993 RVA: 0x000C77B8 File Offset: 0x000C59B8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= ReadyToEndGame.chargeupDuration && !this.ready)
			{
				this.ready = true;
				this.purchaseInteraction.Networkavailable = true;
				base.gameObject.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(EndingGame));
			}
		}

		// Token: 0x04002C1F RID: 11295
		public static string chargeupChildString;

		// Token: 0x04002C20 RID: 11296
		public static string chargeupSoundString;

		// Token: 0x04002C21 RID: 11297
		public static float chargeupDuration;

		// Token: 0x04002C22 RID: 11298
		private ChildLocator childLocator;

		// Token: 0x04002C23 RID: 11299
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x04002C24 RID: 11300
		private bool ready;
	}
}
