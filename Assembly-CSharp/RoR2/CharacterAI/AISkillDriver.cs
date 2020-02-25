using System;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.CharacterAI
{
	// Token: 0x02000569 RID: 1385
	public class AISkillDriver : MonoBehaviour
	{
		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06002103 RID: 8451 RVA: 0x0008E9FA File Offset: 0x0008CBFA
		public float minDistanceSqr
		{
			get
			{
				return this.minDistance * this.minDistance;
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06002104 RID: 8452 RVA: 0x0008EA09 File Offset: 0x0008CC09
		public float maxDistanceSqr
		{
			get
			{
				return this.maxDistance * this.maxDistance;
			}
		}

		// Token: 0x04001E29 RID: 7721
		[Tooltip("The name of this skill driver for reference purposes.")]
		public string customName;

		// Token: 0x04001E2A RID: 7722
		[Tooltip("The slot of the associated skill. Set to None to allow this behavior to run regardless of skill availability.")]
		public SkillSlot skillSlot;

		// Token: 0x04001E2B RID: 7723
		[Tooltip("The skill that the specified slot must have for this behavior to run. Set to none to allow any skill.")]
		public SkillDef requiredSkill;

		// Token: 0x04001E2C RID: 7724
		[Tooltip("If set, this cannot be the dominant driver while the skill is on cooldown or out of stock.")]
		public bool requireSkillReady;

		// Token: 0x04001E2D RID: 7725
		[Tooltip("If set, this cannot be the dominant driver while the equipment is on cooldown or out of stock.")]
		public bool requireEquipmentReady;

		// Token: 0x04001E2E RID: 7726
		[Tooltip("The type of object targeted for movement.")]
		[FormerlySerializedAs("targetType")]
		public AISkillDriver.TargetType moveTargetType;

		// Token: 0x04001E2F RID: 7727
		[Tooltip("The minimum health fraction required of the user for this behavior.")]
		public float minUserHealthFraction = float.NegativeInfinity;

		// Token: 0x04001E30 RID: 7728
		[Tooltip("The maximum health fraction required of the user for this behavior.")]
		public float maxUserHealthFraction = float.PositiveInfinity;

		// Token: 0x04001E31 RID: 7729
		[Tooltip("The minimum health fraction required of the target for this behavior.")]
		public float minTargetHealthFraction = float.NegativeInfinity;

		// Token: 0x04001E32 RID: 7730
		[Tooltip("The maximum health fraction required of the target for this behavior.")]
		public float maxTargetHealthFraction = float.PositiveInfinity;

		// Token: 0x04001E33 RID: 7731
		[Tooltip("The minimum distance from the target required for this behavior.")]
		public float minDistance;

		// Token: 0x04001E34 RID: 7732
		[Tooltip("The maximum distance from the target required for this behavior.")]
		public float maxDistance = float.PositiveInfinity;

		// Token: 0x04001E35 RID: 7733
		public bool selectionRequiresTargetLoS;

		// Token: 0x04001E36 RID: 7734
		[Tooltip("If set, this skill will not be activated unless there is LoS to the target.")]
		public bool activationRequiresTargetLoS;

		// Token: 0x04001E37 RID: 7735
		[Tooltip("If set, this skill will not be activated unless the aim vector is pointing close to the target.")]
		public bool activationRequiresAimConfirmation;

		// Token: 0x04001E38 RID: 7736
		[Tooltip("The movement type to use while this is the dominant skill driver.")]
		public AISkillDriver.MovementType movementType = AISkillDriver.MovementType.ChaseMoveTarget;

		// Token: 0x04001E39 RID: 7737
		public float moveInputScale = 1f;

		// Token: 0x04001E3A RID: 7738
		[Tooltip("Where to look while this is the dominant skill driver")]
		public AISkillDriver.AimType aimType = AISkillDriver.AimType.AtMoveTarget;

		// Token: 0x04001E3B RID: 7739
		[Tooltip("If set, the nodegraph will not be used to direct the local navigator while this is the dominant skill driver. Direction toward the target will be used instead.")]
		public bool ignoreNodeGraph;

		// Token: 0x04001E3C RID: 7740
		[Tooltip("If non-negative, this value will be used for the driver evaluation timer while this is the dominant skill driver.")]
		public float driverUpdateTimerOverride = -1f;

		// Token: 0x04001E3D RID: 7741
		[Tooltip("If set and this is the dominant skill driver, the current enemy will be reset at the time of the next evaluation.")]
		public bool resetCurrentEnemyOnNextDriverSelection;

		// Token: 0x04001E3E RID: 7742
		[Tooltip("If true, this skill driver cannot be chosen twice in a row.")]
		public bool noRepeat;

		// Token: 0x04001E3F RID: 7743
		[Tooltip("If true, the AI will attempt to sprint while this is the dominant skill driver.")]
		public bool shouldSprint;

		// Token: 0x04001E40 RID: 7744
		public bool shouldFireEquipment;

		// Token: 0x0200056A RID: 1386
		public enum TargetType
		{
			// Token: 0x04001E42 RID: 7746
			CurrentEnemy,
			// Token: 0x04001E43 RID: 7747
			NearestFriendlyInSkillRange,
			// Token: 0x04001E44 RID: 7748
			CurrentLeader,
			// Token: 0x04001E45 RID: 7749
			Custom
		}

		// Token: 0x0200056B RID: 1387
		public enum AimType
		{
			// Token: 0x04001E47 RID: 7751
			None,
			// Token: 0x04001E48 RID: 7752
			AtMoveTarget,
			// Token: 0x04001E49 RID: 7753
			AtCurrentEnemy,
			// Token: 0x04001E4A RID: 7754
			AtCurrentLeader,
			// Token: 0x04001E4B RID: 7755
			MoveDirection
		}

		// Token: 0x0200056C RID: 1388
		public enum MovementType
		{
			// Token: 0x04001E4D RID: 7757
			Stop,
			// Token: 0x04001E4E RID: 7758
			ChaseMoveTarget,
			// Token: 0x04001E4F RID: 7759
			StrafeMovetarget,
			// Token: 0x04001E50 RID: 7760
			FleeMoveTarget
		}
	}
}
