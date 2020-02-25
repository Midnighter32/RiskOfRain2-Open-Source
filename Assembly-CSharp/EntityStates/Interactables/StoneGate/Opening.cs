using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Interactables.StoneGate
{
	// Token: 0x0200080D RID: 2061
	public class Opening : BaseState
	{
		// Token: 0x06002ED3 RID: 11987 RVA: 0x000C758C File Offset: 0x000C578C
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

		// Token: 0x06002ED4 RID: 11988 RVA: 0x000C75F3 File Offset: 0x000C57F3
		public override void Update()
		{
			base.Update();
			this.UpdateGateTransform(ref this.leftGateTransform, Opening.leftGateChildLocatorEntry);
			this.UpdateGateTransform(ref this.rightGateTransform, Opening.rightGateChildLocatorEntry);
		}

		// Token: 0x06002ED5 RID: 11989 RVA: 0x000C7620 File Offset: 0x000C5820
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

		// Token: 0x06002ED6 RID: 11990 RVA: 0x000C767C File Offset: 0x000C587C
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

		// Token: 0x04002C13 RID: 11283
		public static string leftGateChildLocatorEntry;

		// Token: 0x04002C14 RID: 11284
		public static string rightGateChildLocatorEntry;

		// Token: 0x04002C15 RID: 11285
		public static AnimationCurve doorPositionCurve;

		// Token: 0x04002C16 RID: 11286
		public static float duration;

		// Token: 0x04002C17 RID: 11287
		public static string doorBeginOpenEffectChildLocatorEntry;

		// Token: 0x04002C18 RID: 11288
		public static string doorBeginOpenSoundString;

		// Token: 0x04002C19 RID: 11289
		public static string doorFinishedOpenEffectChildLocatorEntry;

		// Token: 0x04002C1A RID: 11290
		public static string doorFinishedOpenSoundString;

		// Token: 0x04002C1B RID: 11291
		private ChildLocator childLocator;

		// Token: 0x04002C1C RID: 11292
		private bool doorIsOpen;

		// Token: 0x04002C1D RID: 11293
		private Transform leftGateTransform;

		// Token: 0x04002C1E RID: 11294
		private Transform rightGateTransform;
	}
}
