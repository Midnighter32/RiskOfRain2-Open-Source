using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.CharacterAI
{
	// Token: 0x02000598 RID: 1432
	public class AISkillDriver : MonoBehaviour
	{
		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06002039 RID: 8249 RVA: 0x00097446 File Offset: 0x00095646
		public float minDistanceSqr
		{
			get
			{
				return this.minDistance * this.minDistance;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x0600203A RID: 8250 RVA: 0x00097455 File Offset: 0x00095655
		public float maxDistanceSqr
		{
			get
			{
				return this.maxDistance * this.maxDistance;
			}
		}

		// Token: 0x0400226E RID: 8814
		[Tooltip("The name of this skill driver for reference purposes.")]
		public string customName;

		// Token: 0x0400226F RID: 8815
		[Tooltip("The slot of the associated skill. Set to None to allow this behavior to run regardless of skill availability.")]
		public SkillSlot skillSlot;

		// Token: 0x04002270 RID: 8816
		[Tooltip("If set, this cannot be the dominant driver while the skill is on cooldown or out of stock.")]
		public bool requireSkillReady;

		// Token: 0x04002271 RID: 8817
		[Tooltip("The type of object targeted for movement.")]
		[FormerlySerializedAs("targetType")]
		public AISkillDriver.TargetType moveTargetType;

		// Token: 0x04002272 RID: 8818
		[Tooltip("The minimum health fraction required of the user for this behavior.")]
		public float minUserHealthFraction = float.NegativeInfinity;

		// Token: 0x04002273 RID: 8819
		[Tooltip("The maximum health fraction required of the user for this behavior.")]
		public float maxUserHealthFraction = float.PositiveInfinity;

		// Token: 0x04002274 RID: 8820
		[Tooltip("The minimum health fraction required of the target for this behavior.")]
		public float minTargetHealthFraction = float.NegativeInfinity;

		// Token: 0x04002275 RID: 8821
		[Tooltip("The maximum health fraction required of the target for this behavior.")]
		public float maxTargetHealthFraction = float.PositiveInfinity;

		// Token: 0x04002276 RID: 8822
		[Tooltip("The minimum distance from the target required for this behavior.")]
		public float minDistance;

		// Token: 0x04002277 RID: 8823
		[Tooltip("The maximum distance from the target required for this behavior.")]
		public float maxDistance = float.PositiveInfinity;

		// Token: 0x04002278 RID: 8824
		public bool selectionRequiresTargetLoS;

		// Token: 0x04002279 RID: 8825
		[Tooltip("If set, this skill will not be activated unless there is LoS to the target.")]
		public bool activationRequiresTargetLoS;

		// Token: 0x0400227A RID: 8826
		[Tooltip("If set, this skill will not be activated unless the aim vector is pointing close to the target.")]
		public bool activationRequiresAimConfirmation;

		// Token: 0x0400227B RID: 8827
		[Tooltip("The movement type to use while this is the dominant skill driver.")]
		public AISkillDriver.MovementType movementType = AISkillDriver.MovementType.ChaseMoveTarget;

		// Token: 0x0400227C RID: 8828
		public float moveInputScale = 1f;

		// Token: 0x0400227D RID: 8829
		[Tooltip("Where to look while this is the dominant skill driver")]
		public AISkillDriver.AimType aimType = AISkillDriver.AimType.AtMoveTarget;

		// Token: 0x0400227E RID: 8830
		[Tooltip("If set, the nodegraph will not be used to direct the local navigator while this is the dominant skill driver. Direction toward the target will be used instead.")]
		public bool ignoreNodeGraph;

		// Token: 0x0400227F RID: 8831
		[Tooltip("If non-negative, this value will be used for the driver evaluation timer while this is the dominant skill driver.")]
		public float driverUpdateTimerOverride = -1f;

		// Token: 0x04002280 RID: 8832
		[Tooltip("If set and this is the dominant skill driver, the current enemy will be reset at the time of the next evaluation.")]
		public bool resetCurrentEnemyOnNextDriverSelection;

		// Token: 0x04002281 RID: 8833
		[Tooltip("If true, this skill driver cannot be chosen twice in a row.")]
		public bool noRepeat;

		// Token: 0x04002282 RID: 8834
		[Tooltip("If true, the AI will attempt to sprint while this is the dominant skill driver.")]
		public bool shouldSprint;

		// Token: 0x02000599 RID: 1433
		public enum TargetType
		{
			// Token: 0x04002284 RID: 8836
			CurrentEnemy,
			// Token: 0x04002285 RID: 8837
			NearestFriendlyInSkillRange,
			// Token: 0x04002286 RID: 8838
			CurrentLeader
		}

		// Token: 0x0200059A RID: 1434
		public enum AimType
		{
			// Token: 0x04002288 RID: 8840
			None,
			// Token: 0x04002289 RID: 8841
			AtMoveTarget,
			// Token: 0x0400228A RID: 8842
			AtCurrentEnemy,
			// Token: 0x0400228B RID: 8843
			AtCurrentLeader,
			// Token: 0x0400228C RID: 8844
			MoveDirection
		}

		// Token: 0x0200059B RID: 1435
		public enum MovementType
		{
			// Token: 0x0400228E RID: 8846
			Stop,
			// Token: 0x0400228F RID: 8847
			ChaseMoveTarget,
			// Token: 0x04002290 RID: 8848
			StrafeMovetarget,
			// Token: 0x04002291 RID: 8849
			FleeMoveTarget
		}
	}
}
