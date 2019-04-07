using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000424 RID: 1060
	public class WispAI : MonoBehaviour
	{
		// Token: 0x0600179F RID: 6047 RVA: 0x0006FF79 File Offset: 0x0006E179
		private void Awake()
		{
			this.bodyDirectionComponent = this.body.GetComponent<CharacterDirection>();
			this.bodyMotorComponent = this.body.GetComponent<CharacterMotor>();
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x0006FFA0 File Offset: 0x0006E1A0
		private void FixedUpdate()
		{
			if (!this.body)
			{
				return;
			}
			if (!this.targetTransform)
			{
				this.targetTransform = this.SearchForTarget();
			}
			if (this.targetTransform)
			{
				Vector3 vector = this.targetTransform.position - this.body.transform.position;
				this.bodyMotorComponent.moveDirection = vector;
				this.bodyDirectionComponent.moveVector = Vector3.Lerp(this.bodyDirectionComponent.moveVector, vector, Time.deltaTime);
				if (this.fireSkill && vector.sqrMagnitude < this.fireRange * this.fireRange)
				{
					this.fireSkill.ExecuteIfReady();
				}
			}
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x00070060 File Offset: 0x0006E260
		private Transform SearchForTarget()
		{
			Vector3 position = this.body.transform.position;
			Vector3 forward = this.bodyDirectionComponent.forward;
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
			for (int i = 0; i < teamMembers.Count; i++)
			{
				Transform transform = teamMembers[i].transform;
				Vector3 vector = transform.position - position;
				if (Vector3.Dot(forward, vector) > 0f)
				{
					WispAI.candidateList.Add(new WispAI.TargetSearchCandidate
					{
						transform = transform,
						positionDiff = vector,
						sqrDistance = vector.sqrMagnitude
					});
				}
			}
			WispAI.candidateList.Sort(delegate(WispAI.TargetSearchCandidate a, WispAI.TargetSearchCandidate b)
			{
				if (a.sqrDistance < b.sqrDistance)
				{
					return -1;
				}
				if (a.sqrDistance != b.sqrDistance)
				{
					return 1;
				}
				return 0;
			});
			Transform result = null;
			for (int j = 0; j < WispAI.candidateList.Count; j++)
			{
				if (!Physics.Raycast(position, WispAI.candidateList[j].positionDiff, Mathf.Sqrt(WispAI.candidateList[j].sqrDistance), LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					result = WispAI.candidateList[j].transform;
					break;
				}
			}
			WispAI.candidateList.Clear();
			return result;
		}

		// Token: 0x04001ACF RID: 6863
		[Tooltip("The character to control.")]
		public GameObject body;

		// Token: 0x04001AD0 RID: 6864
		[Tooltip("The enemy to target.")]
		public Transform targetTransform;

		// Token: 0x04001AD1 RID: 6865
		[Tooltip("The skill to activate for a ranged attack.")]
		public GenericSkill fireSkill;

		// Token: 0x04001AD2 RID: 6866
		[Tooltip("How close the character must be to the enemy to use a ranged attack.")]
		public float fireRange;

		// Token: 0x04001AD3 RID: 6867
		private CharacterDirection bodyDirectionComponent;

		// Token: 0x04001AD4 RID: 6868
		private CharacterMotor bodyMotorComponent;

		// Token: 0x04001AD5 RID: 6869
		private static List<WispAI.TargetSearchCandidate> candidateList = new List<WispAI.TargetSearchCandidate>();

		// Token: 0x02000425 RID: 1061
		private struct TargetSearchCandidate
		{
			// Token: 0x04001AD6 RID: 6870
			public Transform transform;

			// Token: 0x04001AD7 RID: 6871
			public Vector3 positionDiff;

			// Token: 0x04001AD8 RID: 6872
			public float sqrDistance;
		}
	}
}
