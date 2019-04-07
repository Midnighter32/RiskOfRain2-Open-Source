using System;
using TMPro;
using UnityEngine;

namespace Assets.RoR2.Scripts.GameBehaviors.UI
{
	// Token: 0x020000A3 RID: 163
	public class HUDSpeedometer : MonoBehaviour
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0000C1F0 File Offset: 0x0000A3F0
		// (set) Token: 0x06000305 RID: 773 RVA: 0x0000C1F8 File Offset: 0x0000A3F8
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

		// Token: 0x06000306 RID: 774 RVA: 0x0000C24C File Offset: 0x0000A44C
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

		// Token: 0x06000307 RID: 775 RVA: 0x0000C288 File Offset: 0x0000A488
		private void Update()
		{
			if (this._targetTransform)
			{
				float num = this.EstimateSpeed(ref this.lastTargetPosition, this._targetTransform.position, Time.deltaTime);
				this.label.text = string.Format("{0:0.00} m/s", num);
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0000C2DC File Offset: 0x0000A4DC
		private void FixedUpdate()
		{
			if (this._targetTransform)
			{
				float num = this.EstimateSpeed(ref this.lastTargetPositionFixedUpdate, this._targetTransform.position, Time.deltaTime);
				this.fixedUpdateLabel.text = string.Format("{0:0.00} m/s", num);
			}
		}

		// Token: 0x040002E2 RID: 738
		public TextMeshProUGUI label;

		// Token: 0x040002E3 RID: 739
		public TextMeshProUGUI fixedUpdateLabel;

		// Token: 0x040002E4 RID: 740
		private Transform _targetTransform;

		// Token: 0x040002E5 RID: 741
		private Vector3 lastTargetPosition;

		// Token: 0x040002E6 RID: 742
		private Vector3 lastTargetPositionFixedUpdate;
	}
}
