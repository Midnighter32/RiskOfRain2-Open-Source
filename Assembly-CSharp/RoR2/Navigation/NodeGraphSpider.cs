using System;
using System.Collections;
using System.Collections.Generic;

namespace RoR2.Navigation
{
	// Token: 0x02000532 RID: 1330
	public class NodeGraphSpider
	{
		// Token: 0x06001DD1 RID: 7633 RVA: 0x0008C08F File Offset: 0x0008A28F
		public NodeGraphSpider(NodeGraph nodeGraph, HullMask hullMask)
		{
			this.nodeGraph = nodeGraph;
			this.hullMask = hullMask;
			this.collectedSteps = new List<NodeGraphSpider.StepInfo>();
			this.uncheckedSteps = new List<NodeGraphSpider.StepInfo>();
			this.visitedNodes = new BitArray(nodeGraph.GetNodeCount());
		}

		// Token: 0x06001DD2 RID: 7634 RVA: 0x0008C0CC File Offset: 0x0008A2CC
		public bool PerformStep()
		{
			List<NodeGraphSpider.StepInfo> list = this.uncheckedSteps;
			this.uncheckedSteps = new List<NodeGraphSpider.StepInfo>();
			for (int i = 0; i < list.Count; i++)
			{
				NodeGraphSpider.StepInfo stepInfo = list[i];
				foreach (NodeGraph.LinkIndex linkIndex in this.nodeGraph.GetActiveNodeLinks(stepInfo.node))
				{
					if (this.nodeGraph.IsLinkSuitableForHull(linkIndex, this.hullMask))
					{
						NodeGraph.NodeIndex linkEndNode = this.nodeGraph.GetLinkEndNode(linkIndex);
						if (!this.visitedNodes[linkEndNode.nodeIndex])
						{
							this.uncheckedSteps.Add(new NodeGraphSpider.StepInfo
							{
								node = linkEndNode,
								previousStep = stepInfo
							});
							this.visitedNodes[linkEndNode.nodeIndex] = true;
						}
					}
				}
				this.collectedSteps.Add(stepInfo);
			}
			return list.Count > 0;
		}

		// Token: 0x06001DD3 RID: 7635 RVA: 0x0008C1BC File Offset: 0x0008A3BC
		public void AddNodeForNextStep(NodeGraph.NodeIndex nodeIndex)
		{
			if (!this.visitedNodes[nodeIndex.nodeIndex])
			{
				this.uncheckedSteps.Add(new NodeGraphSpider.StepInfo
				{
					node = nodeIndex,
					previousStep = null
				});
				this.visitedNodes[nodeIndex.nodeIndex] = true;
			}
		}

		// Token: 0x04002016 RID: 8214
		private NodeGraph nodeGraph;

		// Token: 0x04002017 RID: 8215
		public List<NodeGraphSpider.StepInfo> collectedSteps;

		// Token: 0x04002018 RID: 8216
		private List<NodeGraphSpider.StepInfo> uncheckedSteps;

		// Token: 0x04002019 RID: 8217
		private BitArray visitedNodes;

		// Token: 0x0400201A RID: 8218
		public HullMask hullMask;

		// Token: 0x02000533 RID: 1331
		public class StepInfo
		{
			// Token: 0x0400201B RID: 8219
			public NodeGraph.NodeIndex node;

			// Token: 0x0400201C RID: 8220
			public NodeGraphSpider.StepInfo previousStep;
		}
	}
}
