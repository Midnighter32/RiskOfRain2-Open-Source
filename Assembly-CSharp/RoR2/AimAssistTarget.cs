using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000252 RID: 594
	public class AimAssistTarget : MonoBehaviour
	{
		// Token: 0x06000B16 RID: 2838 RVA: 0x00037113 File Offset: 0x00035313
		private void OnEnable()
		{
			AimAssistTarget.instancesList.Add(this);
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x00037120 File Offset: 0x00035320
		private void OnDisable()
		{
			AimAssistTarget.instancesList.Remove(this);
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x0003712E File Offset: 0x0003532E
		private void FixedUpdate()
		{
			if (this.healthComponent && !this.healthComponent.alive)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x00037158 File Offset: 0x00035358
		private void OnDrawGizmos()
		{
			if (this.point0)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(this.point0.position, 1f * this.assistScale * CameraRigController.aimStickAssistMinSize.value * AimAssistTarget.debugAimAssistVisualCoefficient.value);
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(this.point0.position, 1f * this.assistScale * CameraRigController.aimStickAssistMaxSize.value * CameraRigController.aimStickAssistMinSize.value * AimAssistTarget.debugAimAssistVisualCoefficient.value);
			}
			if (this.point1)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(this.point1.position, 1f * this.assistScale * CameraRigController.aimStickAssistMinSize.value * AimAssistTarget.debugAimAssistVisualCoefficient.value);
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(this.point1.position, 1f * this.assistScale * CameraRigController.aimStickAssistMaxSize.value * CameraRigController.aimStickAssistMinSize.value * AimAssistTarget.debugAimAssistVisualCoefficient.value);
			}
			if (this.point0 && this.point1)
			{
				Gizmos.DrawLine(this.point0.position, this.point1.position);
			}
		}

		// Token: 0x04000F1A RID: 3866
		public Transform point0;

		// Token: 0x04000F1B RID: 3867
		public Transform point1;

		// Token: 0x04000F1C RID: 3868
		public float assistScale = 1f;

		// Token: 0x04000F1D RID: 3869
		public HealthComponent healthComponent;

		// Token: 0x04000F1E RID: 3870
		public TeamComponent teamComponent;

		// Token: 0x04000F1F RID: 3871
		public static List<AimAssistTarget> instancesList = new List<AimAssistTarget>();

		// Token: 0x04000F20 RID: 3872
		public static FloatConVar debugAimAssistVisualCoefficient = new FloatConVar("debug_aim_assist_visual_coefficient", ConVarFlags.None, "2", "Magic for debug visuals. Don't touch.");
	}
}
