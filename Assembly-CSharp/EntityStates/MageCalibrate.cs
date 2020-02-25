using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates
{
	// Token: 0x02000717 RID: 1815
	public class MageCalibrate : BaseState
	{
		// Token: 0x06002A4F RID: 10831 RVA: 0x000B224C File Offset: 0x000B044C
		public override void OnEnter()
		{
			this.calibrationController = base.GetComponent<MageCalibrationController>();
			this.shouldApply = NetworkServer.active;
			base.OnEnter();
		}

		// Token: 0x06002A50 RID: 10832 RVA: 0x000B226B File Offset: 0x000B046B
		public override void OnExit()
		{
			this.ApplyElement();
			base.OnExit();
		}

		// Token: 0x06002A51 RID: 10833 RVA: 0x000AF84B File Offset: 0x000ADA4B
		public override void FixedUpdate()
		{
			this.outer.SetNextStateToMain();
		}

		// Token: 0x06002A52 RID: 10834 RVA: 0x000B2279 File Offset: 0x000B0479
		private void ApplyElement()
		{
			Debug.Log("MageCalibrate.ApplyElement()");
			if (this.shouldApply && this.calibrationController)
			{
				this.shouldApply = false;
				this.calibrationController.SetElement(this.element);
			}
		}

		// Token: 0x06002A53 RID: 10835 RVA: 0x000B22B2 File Offset: 0x000B04B2
		public override void OnSerialize(NetworkWriter writer)
		{
			writer.Write((byte)this.element);
		}

		// Token: 0x06002A54 RID: 10836 RVA: 0x000B22C0 File Offset: 0x000B04C0
		public override void OnDeserialize(NetworkReader reader)
		{
			this.element = (MageElement)reader.ReadByte();
		}

		// Token: 0x06002A55 RID: 10837 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002627 RID: 9767
		public MageElement element;

		// Token: 0x04002628 RID: 9768
		public MageCalibrationController calibrationController;

		// Token: 0x04002629 RID: 9769
		private bool shouldApply;
	}
}
