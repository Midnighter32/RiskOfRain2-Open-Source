using System;
using RoR2;
using UnityEngine;

namespace EntityStates.AI.Walker
{
	// Token: 0x02000900 RID: 2304
	public class LookBusy : BaseAIState
	{
		// Token: 0x06003376 RID: 13174 RVA: 0x000DF558 File Offset: 0x000DD758
		protected virtual void PickNewTargetLookDirection()
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

		// Token: 0x06003377 RID: 13175 RVA: 0x000DF5FC File Offset: 0x000DD7FC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = UnityEngine.Random.Range(2f, 7f);
			base.bodyInputBank.moveVector = Vector3.zero;
			base.bodyInputBank.jump.PushState(false);
			this.PickNewTargetLookDirection();
		}

		// Token: 0x06003378 RID: 13176 RVA: 0x000DEF05 File Offset: 0x000DD105
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003379 RID: 13177 RVA: 0x000DF64C File Offset: 0x000DD84C
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

		// Token: 0x040032F4 RID: 13044
		private const float minDuration = 2f;

		// Token: 0x040032F5 RID: 13045
		private const float maxDuration = 7f;

		// Token: 0x040032F6 RID: 13046
		private Vector3 targetPosition;

		// Token: 0x040032F7 RID: 13047
		private float duration;

		// Token: 0x040032F8 RID: 13048
		private float lookTimer;

		// Token: 0x040032F9 RID: 13049
		private const float minLookDuration = 0.5f;

		// Token: 0x040032FA RID: 13050
		private const float maxLookDuration = 4f;

		// Token: 0x040032FB RID: 13051
		private const int lookTries = 4;

		// Token: 0x040032FC RID: 13052
		private const float lookRaycastLength = 25f;
	}
}
