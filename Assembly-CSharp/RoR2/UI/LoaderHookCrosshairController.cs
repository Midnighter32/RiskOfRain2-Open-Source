using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI
{
	// Token: 0x020005E1 RID: 1505
	[RequireComponent(typeof(HudElement))]
	[RequireComponent(typeof(RectTransform))]
	public class LoaderHookCrosshairController : MonoBehaviour
	{
		// Token: 0x06002394 RID: 9108 RVA: 0x0009B575 File Offset: 0x00099775
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x06002395 RID: 9109 RVA: 0x0009B583 File Offset: 0x00099783
		private void SetAvailable(bool newIsAvailable)
		{
			if (this.isAvailable == newIsAvailable)
			{
				return;
			}
			this.isAvailable = newIsAvailable;
			(this.isAvailable ? this.onAvailable : this.onUnavailable).Invoke();
		}

		// Token: 0x06002396 RID: 9110 RVA: 0x0009B5B1 File Offset: 0x000997B1
		private void SetInRange(bool newInRange)
		{
			if (this.inRange == newInRange)
			{
				return;
			}
			this.inRange = newInRange;
			UnityEvent unityEvent = this.inRange ? this.onEnterRange : this.onExitRange;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06002397 RID: 9111 RVA: 0x0009B5E4 File Offset: 0x000997E4
		private void Update()
		{
			if (!this.hudElement.targetCharacterBody)
			{
				this.SetAvailable(false);
				this.SetInRange(false);
				return;
			}
			this.SetAvailable(this.hudElement.targetCharacterBody.skillLocator.secondary.CanExecute());
			bool flag = false;
			if (this.isAvailable)
			{
				RaycastHit raycastHit;
				flag = this.hudElement.targetCharacterBody.inputBank.GetAimRaycast(this.range, out raycastHit);
			}
			this.SetInRange(flag);
		}

		// Token: 0x04002188 RID: 8584
		public float range;

		// Token: 0x04002189 RID: 8585
		public UnityEvent onAvailable;

		// Token: 0x0400218A RID: 8586
		public UnityEvent onUnavailable;

		// Token: 0x0400218B RID: 8587
		public UnityEvent onEnterRange;

		// Token: 0x0400218C RID: 8588
		public UnityEvent onExitRange;

		// Token: 0x0400218D RID: 8589
		private HudElement hudElement;

		// Token: 0x0400218E RID: 8590
		private bool isAvailable;

		// Token: 0x0400218F RID: 8591
		private bool inRange;
	}
}
