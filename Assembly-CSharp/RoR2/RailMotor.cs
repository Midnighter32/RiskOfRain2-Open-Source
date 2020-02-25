using System;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002DF RID: 735
	[RequireComponent(typeof(CharacterBody))]
	public class RailMotor : MonoBehaviour
	{
		// Token: 0x060010DC RID: 4316 RVA: 0x00049D8C File Offset: 0x00047F8C
		private void Start()
		{
			this.characterDirection = base.GetComponent<CharacterDirection>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.characterBody = base.GetComponent<CharacterBody>();
			this.railGraph = SceneInfo.instance.railNodes;
			ModelLocator component = base.GetComponent<ModelLocator>();
			if (component)
			{
				this.modelAnimator = component.modelTransform.GetComponent<Animator>();
			}
			this.nodeA = this.railGraph.FindClosestNode(base.transform.position, this.characterBody.hullClassification);
			NodeGraph.LinkIndex[] activeNodeLinks = this.railGraph.GetActiveNodeLinks(this.nodeA);
			this.currentLink = activeNodeLinks[0];
			this.UpdateNodeAndLinkInfo();
			this.useRootMotion = this.characterBody.rootMotionInMainState;
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x00049E4C File Offset: 0x0004804C
		private void UpdateNodeAndLinkInfo()
		{
			this.nodeA = this.railGraph.GetLinkStartNode(this.currentLink);
			this.nodeB = this.railGraph.GetLinkEndNode(this.currentLink);
			this.railGraph.GetNodePosition(this.nodeA, out this.nodeAPosition);
			this.railGraph.GetNodePosition(this.nodeB, out this.nodeBPosition);
			this.linkVector = this.nodeBPosition - this.nodeAPosition;
			this.linkLength = this.linkVector.magnitude;
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x00049EE0 File Offset: 0x000480E0
		private void FixedUpdate()
		{
			this.UpdateNodeAndLinkInfo();
			if (this.inputBank)
			{
				bool value = false;
				if (this.inputMoveVector.sqrMagnitude > 0f)
				{
					value = true;
					this.characterDirection.moveVector = this.linkVector;
					if (this.linkLerp == 0f || this.linkLerp == 1f)
					{
						NodeGraph.NodeIndex nodeIndex;
						if (this.linkLerp == 0f)
						{
							nodeIndex = this.nodeA;
						}
						else
						{
							nodeIndex = this.nodeB;
						}
						NodeGraph.LinkIndex[] activeNodeLinks = this.railGraph.GetActiveNodeLinks(nodeIndex);
						float num = -1f;
						NodeGraph.LinkIndex lhs = this.currentLink;
						Debug.DrawRay(base.transform.position, this.inputMoveVector, Color.green);
						foreach (NodeGraph.LinkIndex linkIndex in activeNodeLinks)
						{
							NodeGraph.NodeIndex linkStartNode = this.railGraph.GetLinkStartNode(linkIndex);
							NodeGraph.NodeIndex linkEndNode = this.railGraph.GetLinkEndNode(linkIndex);
							if (!(linkStartNode != nodeIndex))
							{
								Vector3 vector;
								this.railGraph.GetNodePosition(linkStartNode, out vector);
								Vector3 a;
								this.railGraph.GetNodePosition(linkEndNode, out a);
								Vector3 vector2 = a - vector;
								Vector3 rhs = new Vector3(vector2.x, 0f, vector2.z);
								Debug.DrawRay(vector, vector2, Color.red);
								float num2 = Vector3.Dot(this.inputMoveVector, rhs);
								if (num2 > num)
								{
									num = num2;
									lhs = linkIndex;
								}
							}
						}
						if (lhs != this.currentLink)
						{
							this.currentLink = lhs;
							this.UpdateNodeAndLinkInfo();
							this.linkLerp = 0f;
						}
					}
				}
				this.modelAnimator.SetBool("isMoving", value);
				if (this.useRootMotion)
				{
					this.TravelLink();
				}
				else
				{
					this.TravelLink();
				}
			}
			base.transform.position = Vector3.Lerp(this.nodeAPosition, this.nodeBPosition, this.linkLerp);
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x0004A0C8 File Offset: 0x000482C8
		private void TravelLink()
		{
			this.projectedMoveVector = Vector3.Project(this.inputMoveVector, this.linkVector);
			this.projectedMoveVector = this.projectedMoveVector.normalized;
			if (this.characterBody.rootMotionInMainState)
			{
				this.currentMoveSpeed = this.rootMotion.magnitude / Time.fixedDeltaTime;
				this.rootMotion = Vector3.zero;
			}
			else
			{
				float target;
				if (this.projectedMoveVector.sqrMagnitude > 0f)
				{
					target = this.characterBody.moveSpeed * this.inputMoveVector.magnitude;
				}
				else
				{
					target = 0f;
				}
				this.currentMoveSpeed = Mathf.MoveTowards(this.currentMoveSpeed, target, this.characterBody.acceleration * Time.fixedDeltaTime);
			}
			if (this.currentMoveSpeed > 0f)
			{
				Vector3 lhs = this.projectedMoveVector * this.currentMoveSpeed;
				float num = this.currentMoveSpeed / this.linkLength * Mathf.Sign(Vector3.Dot(lhs, this.linkVector)) * Time.fixedDeltaTime;
				this.linkLerp = Mathf.Clamp01(this.linkLerp + num);
			}
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x0004A1DC File Offset: 0x000483DC
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(this.nodeAPosition, 0.5f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(this.nodeBPosition, 0.5f);
			Gizmos.DrawLine(this.nodeAPosition, this.nodeBPosition);
		}

		// Token: 0x04001029 RID: 4137
		public Vector3 inputMoveVector;

		// Token: 0x0400102A RID: 4138
		public Vector3 rootMotion;

		// Token: 0x0400102B RID: 4139
		private Animator modelAnimator;

		// Token: 0x0400102C RID: 4140
		private InputBankTest inputBank;

		// Token: 0x0400102D RID: 4141
		private NodeGraph railGraph;

		// Token: 0x0400102E RID: 4142
		private NodeGraph.NodeIndex nodeA;

		// Token: 0x0400102F RID: 4143
		private NodeGraph.NodeIndex nodeB;

		// Token: 0x04001030 RID: 4144
		private NodeGraph.LinkIndex currentLink;

		// Token: 0x04001031 RID: 4145
		private CharacterBody characterBody;

		// Token: 0x04001032 RID: 4146
		private CharacterDirection characterDirection;

		// Token: 0x04001033 RID: 4147
		private float linkLerp;

		// Token: 0x04001034 RID: 4148
		private Vector3 projectedMoveVector;

		// Token: 0x04001035 RID: 4149
		private Vector3 nodeAPosition;

		// Token: 0x04001036 RID: 4150
		private Vector3 nodeBPosition;

		// Token: 0x04001037 RID: 4151
		private Vector3 linkVector;

		// Token: 0x04001038 RID: 4152
		private float linkLength;

		// Token: 0x04001039 RID: 4153
		private float currentMoveSpeed;

		// Token: 0x0400103A RID: 4154
		private bool useRootMotion;
	}
}
