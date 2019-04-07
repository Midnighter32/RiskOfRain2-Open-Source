using System;
using System.Collections.ObjectModel;
using RoR2;

namespace EntityStates.Interactables.MSObelisk
{
	// Token: 0x02000138 RID: 312
	public class ReadyToEndGame : BaseState
	{
		// Token: 0x06000601 RID: 1537 RVA: 0x0001BA44 File Offset: 0x00019C44
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

		// Token: 0x06000602 RID: 1538 RVA: 0x0001BB08 File Offset: 0x00019D08
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

		// Token: 0x040006FB RID: 1787
		public static string chargeupChildString;

		// Token: 0x040006FC RID: 1788
		public static string chargeupSoundString;

		// Token: 0x040006FD RID: 1789
		public static float chargeupDuration;

		// Token: 0x040006FE RID: 1790
		private ChildLocator childLocator;

		// Token: 0x040006FF RID: 1791
		private PurchaseInteraction purchaseInteraction;

		// Token: 0x04000700 RID: 1792
		private bool ready;
	}
}
