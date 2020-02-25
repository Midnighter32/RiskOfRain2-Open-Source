using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D9 RID: 729
	[RequireComponent(typeof(PurchaseInteraction))]
	public class PurchaseAvailabilityIndicator : MonoBehaviour
	{
		// Token: 0x060010A3 RID: 4259 RVA: 0x00049000 File Offset: 0x00047200
		private void Awake()
		{
			this.purchaseInteraction = base.GetComponent<PurchaseInteraction>();
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x00049010 File Offset: 0x00047210
		private void FixedUpdate()
		{
			if (this.indicatorObject)
			{
				this.indicatorObject.SetActive(this.purchaseInteraction.available);
			}
			if (this.disabledIndicatorObject)
			{
				this.disabledIndicatorObject.SetActive(!this.purchaseInteraction.available);
			}
			if (this.animator)
			{
				this.animator.SetBool(this.mecanimBool, this.purchaseInteraction.available);
			}
		}

		// Token: 0x04001003 RID: 4099
		public GameObject indicatorObject;

		// Token: 0x04001004 RID: 4100
		public GameObject disabledIndicatorObject;

		// Token: 0x04001005 RID: 4101
		public Animator animator;

		// Token: 0x04001006 RID: 4102
		public string mecanimBool;

		// Token: 0x04001007 RID: 4103
		private PurchaseInteraction purchaseInteraction;
	}
}
