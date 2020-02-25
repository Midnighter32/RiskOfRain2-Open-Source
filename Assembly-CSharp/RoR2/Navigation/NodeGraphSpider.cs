using System;
using System.Collections;
using System.Collections.Generic;

namespace RoR2.Navigation
{
	// Token: 0x020004EC RID: 1260
	public class NodeGraphSpider
	{
		// Token: 0x06001E06 RID: 7686 RVA: 0x0008137F File Offset: 0x0007F57F
		public NodeGraphSpider(NodeGraph nodeGraph, HullMask hullMask)
		{
			this.nodeGraph = nodeGraph;
			this.hullMask = hullMask;
			this.collectedSteps = new List<NodeGraphSpider.StepInfo>();
			this.uncheckedSteps = new List<NodeGraphSpider.StepInfo>();
			this.visitedNodes = new BitArray(nodeGraph.GetNodeCount());
		}

		// Token: 0x06001E07 RID: 7687 RVA: 0x000813BC File Offset: 0x0007F5BC
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

		// Token: 0x06001E08 RID: 7688 RVA: 0x000814AC File Offset: 0x0007F6AC
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

		// Token: 0x04001B2F RID: 6959
		private NodeGraph nodeGraph;

		// Token: 0x04001B30 RID: 6960
		public List<NodeGraphSpider.StepInfo> collectedSteps;

		// Token: 0x04001B31 RID: 6961
		private List<NodeGraphSpider.StepInfo> uncheckedSteps;

		// Token: 0x04001B32 RID: 6962
		private BitArray visitedNodes;

		// Token: 0x04001B33 RID: 6963
		public HullMask hullMask;

		// Token: 0x020004ED RID: 1261
		public class StepInfo
		{
			// Token: 0x04001B34 RID: 6964
			public NodeGraph.NodeIndex node;

			// Token: 0x04001B35 RID: 6965
			public NodeGraphSpider.StepInfo previousStep;
		}
	}
}
