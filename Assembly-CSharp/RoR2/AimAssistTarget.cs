using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000143 RID: 323
	public class AimAssistTarget : MonoBehaviour
	{
		// Token: 0x060005BE RID: 1470 RVA: 0x00017C3F File Offset: 0x00015E3F
		private void OnEnable()
		{
			AimAssistTarget.instancesList.Add(this);
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00017C4C File Offset: 0x00015E4C
		private void OnDisable()
		{
			AimAssistTarget.instancesList.Remove(this);
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x00017C5A File Offset: 0x00015E5A
		private void FixedUpdate()
		{
			if (this.healthComponent && !this.healthComponent.alive)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00017C84 File Offset: 0x00015E84
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

		// Token: 0x04000637 RID: 1591
		public Transform point0;

		// Token: 0x04000638 RID: 1592
		public Transform point1;

		// Token: 0x04000639 RID: 1593
		public float assistScale = 1f;

		// Token: 0x0400063A RID: 1594
		public HealthComponent healthComponent;

		// Token: 0x0400063B RID: 1595
		public TeamComponent teamComponent;

		// Token: 0x0400063C RID: 1596
		public static List<AimAssistTarget> instancesList = new List<AimAssistTarget>();

		// Token: 0x0400063D RID: 1597
		public static FloatConVar debugAimAssistVisualCoefficient = new FloatConVar("debug_aim_assist_visual_coefficient", ConVarFlags.None, "2", "Magic for debug visuals. Don't touch.");
	}
}
