using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005D1 RID: 1489
	public class HUDSpeedometer : MonoBehaviour
	{
		// Token: 0x170003AE RID: 942
		// (get) Token: 0x0600233A RID: 9018 RVA: 0x00099EB5 File Offset: 0x000980B5
		// (set) Token: 0x0600233B RID: 9019 RVA: 0x00099EC0 File Offset: 0x000980C0
		public Transform targetTransform
		{
			get
			{
				return this._targetTransform;
			}
			set
			{
				if (this._targetTransform == value)
				{
					return;
				}
				this._targetTransform = value;
				if (this._targetTransform)
				{
					this.lastTargetPosition = this._targetTransform.position;
					this.lastTargetPositionFixedUpdate = this._targetTransform.position;
				}
			}
		}

		// Token: 0x0600233C RID: 9020 RVA: 0x00099F14 File Offset: 0x00098114
		private float EstimateSpeed(ref Vector3 oldPosition, Vector3 newPosition, float deltaTime)
		{
			float result = 0f;
			if (deltaTime != 0f)
			{
				result = (newPosition - oldPosition).magnitude / deltaTime;
			}
			oldPosition = newPosition;
			return result;
		}

		// Token: 0x0600233D RID: 9021 RVA: 0x00099F50 File Offset: 0x00098150
		private void Update()
		{
			if (this._targetTransform)
			{
				float num = this.EstimateSpeed(ref this.lastTargetPosition, this._targetTransform.position, Time.deltaTime);
				this.label.text = string.Format("{0:0.00} m/s", num);
			}
		}

		// Token: 0x0600233E RID: 9022 RVA: 0x00099FA4 File Offset: 0x000981A4
		private void FixedUpdate()
		{
			if (this._targetTransform)
			{
				float num = this.EstimateSpeed(ref this.lastTargetPositionFixedUpdate, this._targetTransform.position, Time.deltaTime);
				this.fixedUpdateLabel.text = string.Format("{0:0.00} m/s", num);
			}
		}

		// Token: 0x0400211C RID: 8476
		public TextMeshProUGUI label;

		// Token: 0x0400211D RID: 8477
		public TextMeshProUGUI fixedUpdateLabel;

		// Token: 0x0400211E RID: 8478
		private Transform _targetTransform;

		// Token: 0x0400211F RID: 8479
		private Vector3 lastTargetPosition;

		// Token: 0x04002120 RID: 8480
		private Vector3 lastTargetPositionFixedUpdate;
	}
}
