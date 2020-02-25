using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000380 RID: 896
	public class WispAI : MonoBehaviour
	{
		// Token: 0x060015D9 RID: 5593 RVA: 0x0005D295 File Offset: 0x0005B495
		private void Awake()
		{
			this.bodyDirectionComponent = this.body.GetComponent<CharacterDirection>();
			this.bodyMotorComponent = this.body.GetComponent<CharacterMotor>();
		}

		// Token: 0x060015DA RID: 5594 RVA: 0x0005D2BC File Offset: 0x0005B4BC
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

		// Token: 0x060015DB RID: 5595 RVA: 0x0005D37C File Offset: 0x0005B57C
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

		// Token: 0x04001465 RID: 5221
		[Tooltip("The character to control.")]
		public GameObject body;

		// Token: 0x04001466 RID: 5222
		[Tooltip("The enemy to target.")]
		public Transform targetTransform;

		// Token: 0x04001467 RID: 5223
		[Tooltip("The skill to activate for a ranged attack.")]
		public GenericSkill fireSkill;

		// Token: 0x04001468 RID: 5224
		[Tooltip("How close the character must be to the enemy to use a ranged attack.")]
		public float fireRange;

		// Token: 0x04001469 RID: 5225
		private CharacterDirection bodyDirectionComponent;

		// Token: 0x0400146A RID: 5226
		private CharacterMotor bodyMotorComponent;

		// Token: 0x0400146B RID: 5227
		private static List<WispAI.TargetSearchCandidate> candidateList = new List<WispAI.TargetSearchCandidate>();

		// Token: 0x02000381 RID: 897
		private struct TargetSearchCandidate
		{
			// Token: 0x0400146C RID: 5228
			public Transform transform;

			// Token: 0x0400146D RID: 5229
			public Vector3 positionDiff;

			// Token: 0x0400146E RID: 5230
			public float sqrDistance;
		}
	}
}
