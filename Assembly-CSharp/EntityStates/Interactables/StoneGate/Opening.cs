using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Interactables.StoneGate
{
	// Token: 0x02000137 RID: 311
	public class Opening : BaseState
	{
		// Token: 0x060005FC RID: 1532 RVA: 0x0001B8DC File Offset: 0x00019ADC
		public override void OnEnter()
		{
			base.OnEnter();
			this.childLocator = base.GetComponent<ChildLocator>();
			this.childLocator.FindChild(Opening.doorBeginOpenEffectChildLocatorEntry).gameObject.SetActive(true);
			Util.PlaySound(Opening.doorBeginOpenSoundString, base.gameObject);
			if (NetworkServer.active)
			{
				Chat.SendBroadcastChat(new Chat.SimpleChatMessage
				{
					baseToken = "STONEGATE_OPEN"
				});
			}
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x0001B943 File Offset: 0x00019B43
		public override void Update()
		{
			base.Update();
			this.UpdateGateTransform(ref this.leftGateTransform, Opening.leftGateChildLocatorEntry);
			this.UpdateGateTransform(ref this.rightGateTransform, Opening.rightGateChildLocatorEntry);
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x0001B970 File Offset: 0x00019B70
		private void UpdateGateTransform(ref Transform gateTransform, string childLocatorString)
		{
			if (!gateTransform)
			{
				gateTransform = this.childLocator.FindChild(childLocatorString);
				return;
			}
			Vector3 localPosition = gateTransform.localPosition;
			gateTransform.localPosition = new Vector3(localPosition.x, localPosition.y, Opening.doorPositionCurve.Evaluate(base.age / Opening.duration));
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x0001B9CC File Offset: 0x00019BCC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= Opening.duration && !this.doorIsOpen)
			{
				this.doorIsOpen = true;
				Util.PlaySound(Opening.doorFinishedOpenSoundString, base.gameObject);
				this.childLocator.FindChild(Opening.doorBeginOpenEffectChildLocatorEntry).gameObject.SetActive(false);
				this.childLocator.FindChild(Opening.doorFinishedOpenEffectChildLocatorEntry).gameObject.SetActive(true);
			}
		}

		// Token: 0x040006EF RID: 1775
		public static string leftGateChildLocatorEntry;

		// Token: 0x040006F0 RID: 1776
		public static string rightGateChildLocatorEntry;

		// Token: 0x040006F1 RID: 1777
		public static AnimationCurve doorPositionCurve;

		// Token: 0x040006F2 RID: 1778
		public static float duration;

		// Token: 0x040006F3 RID: 1779
		public static string doorBeginOpenEffectChildLocatorEntry;

		// Token: 0x040006F4 RID: 1780
		public static string doorBeginOpenSoundString;

		// Token: 0x040006F5 RID: 1781
		public static string doorFinishedOpenEffectChildLocatorEntry;

		// Token: 0x040006F6 RID: 1782
		public static string doorFinishedOpenSoundString;

		// Token: 0x040006F7 RID: 1783
		private ChildLocator childLocator;

		// Token: 0x040006F8 RID: 1784
		private bool doorIsOpen;

		// Token: 0x040006F9 RID: 1785
		private Transform leftGateTransform;

		// Token: 0x040006FA RID: 1786
		private Transform rightGateTransform;
	}
}
