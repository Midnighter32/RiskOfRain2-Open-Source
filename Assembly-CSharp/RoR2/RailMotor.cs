using System;
using RoR2.Navigation;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A2 RID: 930
	[RequireComponent(typeof(CharacterBody))]
	public class RailMotor : MonoBehaviour
	{
		// Token: 0x060013B5 RID: 5045 RVA: 0x000603A8 File Offset: 0x0005E5A8
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

		// Token: 0x060013B6 RID: 5046 RVA: 0x00060468 File Offset: 0x0005E668
		private void UpdateNodeAndLinkInfo()
		{
			this.nodeA = this.railGraph.GetLinkStartNode(this.currentLink);
			this.nodeB = this.railGraph.GetLinkEndNode(this.currentLink);
			this.railGraph.GetNodePosition(this.nodeA, out this.nodeAPosition);
			this.railGraph.GetNodePosition(this.nodeB, out this.nodeBPosition);
			this.linkVector = this.nodeBPosition - this.nodeAPosition;
			this.linkLength = this.linkVector.magnitude;
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x000604FC File Offset: 0x0005E6FC
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

		// Token: 0x060013B8 RID: 5048 RVA: 0x000606E4 File Offset: 0x0005E8E4
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

		// Token: 0x060013B9 RID: 5049 RVA: 0x000607F8 File Offset: 0x0005E9F8
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(this.nodeAPosition, 0.5f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(this.nodeBPosition, 0.5f);
			Gizmos.DrawLine(this.nodeAPosition, this.nodeBPosition);
		}

		// Token: 0x0400175A RID: 5978
		public Vector3 inputMoveVector;

		// Token: 0x0400175B RID: 5979
		public Vector3 rootMotion;

		// Token: 0x0400175C RID: 5980
		private Animator modelAnimator;

		// Token: 0x0400175D RID: 5981
		private InputBankTest inputBank;

		// Token: 0x0400175E RID: 5982
		private NodeGraph railGraph;

		// Token: 0x0400175F RID: 5983
		private NodeGraph.NodeIndex nodeA;

		// Token: 0x04001760 RID: 5984
		private NodeGraph.NodeIndex nodeB;

		// Token: 0x04001761 RID: 5985
		private NodeGraph.LinkIndex currentLink;

		// Token: 0x04001762 RID: 5986
		private CharacterBody characterBody;

		// Token: 0x04001763 RID: 5987
		private CharacterDirection characterDirection;

		// Token: 0x04001764 RID: 5988
		private float linkLerp;

		// Token: 0x04001765 RID: 5989
		private Vector3 projectedMoveVector;

		// Token: 0x04001766 RID: 5990
		private Vector3 nodeAPosition;

		// Token: 0x04001767 RID: 5991
		private Vector3 nodeBPosition;

		// Token: 0x04001768 RID: 5992
		private Vector3 linkVector;

		// Token: 0x04001769 RID: 5993
		private float linkLength;

		// Token: 0x0400176A RID: 5994
		private float currentMoveSpeed;

		// Token: 0x0400176B RID: 5995
		private bool useRootMotion;
	}
}
