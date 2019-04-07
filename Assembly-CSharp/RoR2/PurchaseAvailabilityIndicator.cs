using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200039B RID: 923
	[RequireComponent(typeof(PurchaseInteraction))]
	public class PurchaseAvailabilityIndicator : MonoBehaviour
	{
		// Token: 0x0600137E RID: 4990 RVA: 0x0005F380 File Offset: 0x0005D580
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x0600137F RID: 4991 RVA: 0x0005F390 File Offset: 0x0005D590
		private void FixedUpdate()
		{
			this.indicatorObject.SetActive(this.purchaseInteraction.available);
			if (this.animator)
			{
				this.animator.SetBool(this.mecanimBool, this.purchaseInteraction.available);
			}
		}

		// Token: 0x0400172E RID: 5934
		public GameObject indicatorObject;

		// Token: 0x0400172F RID: 5935
		public Animator animator;

		// Token: 0x04001730 RID: 5936
		public string mecanimBool;

		// Token: 0x04001731 RID: 5937
		private PurchaseInteraction purchaseInteraction;
	}
}
