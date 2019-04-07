using System;
using RoR2;
using UnityEngine;

namespace EntityStates.AI.Walker
{
	// Token: 0x020001E5 RID: 485
	public class LookBusy : BaseAIState
	{
		// Token: 0x06000976 RID: 2422 RVA: 0x0002F954 File Offset: 0x0002DB54
		private void PickNewTargetLookDirection()
		{
			if (base.bodyInputBank)
			{
				float num = 0f;
				Vector3 aimOrigin = base.bodyInputBank.aimOrigin;
				for (int i = 0; i < 4; i++)
				{
					Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
					float num2 = 25f;
					RaycastHit raycastHit;
					if (Physics.Raycast(new Ray(aimOrigin, onUnitSphere), out raycastHit, 25f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
					{
						num2 = raycastHit.distance;
					}
					if (num2 > num)
					{
						num = num2;
						base.ai.desiredAimDirection = onUnitSphere;
					}
				}
			}
			this.lookTimer = UnityEngine.Random.Range(0.5f, 4f);
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0002F9F7 File Offset: 0x0002DBF7
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = UnityEngine.Random.Range(2f, 7f);
			this.PickNewTargetLookDirection();
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x0002F301 File Offset: 0x0002D501
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0002FA1C File Offset: 0x0002DC1C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.ai && base.body)
			{
				if (base.ai.skillDriverEvaluation.dominantSkillDriver)
				{
					this.outer.SetNextState(new Combat());
				}
				if (Vector3.Dot(base.bodyInputBank.aimDirection, base.ai.desiredAimDirection) >= 0.95f)
				{
					this.lookTimer -= Time.fixedDeltaTime;
					if (this.lookTimer <= 0f)
					{
						this.PickNewTargetLookDirection();
					}
				}
				if (base.fixedAge >= this.duration)
				{
					this.outer.SetNextState(new Wander());
					return;
				}
			}
		}

		// Token: 0x04000CCB RID: 3275
		private const float minDuration = 2f;

		// Token: 0x04000CCC RID: 3276
		private const float maxDuration = 7f;

		// Token: 0x04000CCD RID: 3277
		private Vector3 targetPosition;

		// Token: 0x04000CCE RID: 3278
		private float duration;

		// Token: 0x04000CCF RID: 3279
		private float lookTimer;

		// Token: 0x04000CD0 RID: 3280
		private const float minLookDuration = 0.5f;

		// Token: 0x04000CD1 RID: 3281
		private const float maxLookDuration = 4f;

		// Token: 0x04000CD2 RID: 3282
		private const int lookTries = 4;

		// Token: 0x04000CD3 RID: 3283
		private const float lookRaycastLength = 25f;
	}
}
