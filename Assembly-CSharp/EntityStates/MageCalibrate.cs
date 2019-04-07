using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x020000BD RID: 189
	public class MageCalibrate : BaseState
	{
		// Token: 0x060003B1 RID: 945 RVA: 0x0000F4A4 File Offset: 0x0000D6A4
		public override void OnEnter()
		{
			this.calibrationController = base.GetComponent<MageCalibrationController>();
			this.shouldApply = NetworkServer.active;
			base.OnEnter();
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0000F4C3 File Offset: 0x0000D6C3
		public override void OnExit()
		{
			this.ApplyElement();
			base.OnExit();
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0000F4D1 File Offset: 0x0000D6D1
		public override void FixedUpdate()
		{
			this.outer.SetNextStateToMain();
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0000F4DE File Offset: 0x0000D6DE
		private void ApplyElement()
		{
			Debug.Log("MageCalibrate.ApplyElement()");
			if (this.shouldApply && this.calibrationController)
			{
				this.shouldApply = false;
				this.calibrationController.SetElement(this.element);
			}
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0000F517 File Offset: 0x0000D717
		public override void OnSerialize(NetworkWriter writer)
		{
			writer.Write((byte)this.element);
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0000F525 File Offset: 0x0000D725
		public override void OnDeserialize(NetworkReader reader)
		{
			this.element = (MageElement)reader.ReadByte();
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x0400036D RID: 877
		public MageElement element;

		// Token: 0x0400036E RID: 878
		public MageCalibrationController calibrationController;

		// Token: 0x0400036F RID: 879
		private bool shouldApply;
	}
}
