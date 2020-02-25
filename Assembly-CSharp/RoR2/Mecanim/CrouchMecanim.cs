using System;
using UnityEngine;

namespace RoR2.Mecanim
{
	// Token: 0x02000531 RID: 1329
	public class CrouchMecanim : MonoBehaviour
	{
		// Token: 0x06001F65 RID: 8037 RVA: 0x00088584 File Offset: 0x00086784
		private void FixedUpdate()
		{
			this.crouchStopwatch -= Time.fixedDeltaTime;
			if (this.crouchStopwatch <= 0f)
			{
				this.crouchStopwatch = 0.5f;
				Transform transform = this.crouchOriginOverride ? this.crouchOriginOverride : base.transform;
				Vector3 up = transform.up;
				RaycastHit raycastHit;
				this.crouchCycle = (Physics.Raycast(new Ray(transform.position - up * this.initialVerticalOffset, up), out raycastHit, this.duckHeight + this.initialVerticalOffset, LayerIndex.world.mask, QueryTriggerInteraction.Ignore) ? Mathf.Clamp01(1f - (raycastHit.distance - this.initialVerticalOffset) / this.duckHeight) : 0f);
			}
		}

		// Token: 0x06001F66 RID: 8038 RVA: 0x00088657 File Offset: 0x00086857
		private void Update()
		{
			if (this.animator)
			{
				this.animator.SetFloat(CrouchMecanim.crouchCycleOffsetParamNameHash, this.crouchCycle, this.smoothdamp, Time.deltaTime);
			}
		}

		// Token: 0x06001F67 RID: 8039 RVA: 0x00088688 File Offset: 0x00086888
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.up * this.duckHeight);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, base.transform.position + -base.transform.up * this.initialVerticalOffset);
		}

		// Token: 0x04001D13 RID: 7443
		public float duckHeight;

		// Token: 0x04001D14 RID: 7444
		public Animator animator;

		// Token: 0x04001D15 RID: 7445
		public float smoothdamp;

		// Token: 0x04001D16 RID: 7446
		public float initialVerticalOffset;

		// Token: 0x04001D17 RID: 7447
		public Transform crouchOriginOverride;

		// Token: 0x04001D18 RID: 7448
		private float crouchCycle;

		// Token: 0x04001D19 RID: 7449
		private const float crouchRaycastFrequency = 2f;

		// Token: 0x04001D1A RID: 7450
		private float crouchStopwatch;

		// Token: 0x04001D1B RID: 7451
		private static readonly int crouchCycleOffsetParamNameHash = Animator.StringToHash("crouchCycleOffset");
	}
}
