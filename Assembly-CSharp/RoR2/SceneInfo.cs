using System;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x0200031C RID: 796
	public class SceneInfo : MonoBehaviour
	{
		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060012B1 RID: 4785 RVA: 0x000506CA File Offset: 0x0004E8CA
		public static SceneInfo instance
		{
			get
			{
				return SceneInfo._instance;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060012B2 RID: 4786 RVA: 0x000506D1 File Offset: 0x0004E8D1
		// (set) Token: 0x060012B3 RID: 4787 RVA: 0x000506D9 File Offset: 0x0004E8D9
		public NodeGraph groundNodes { get; private set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060012B4 RID: 4788 RVA: 0x000506E2 File Offset: 0x0004E8E2
		// (set) Token: 0x060012B5 RID: 4789 RVA: 0x000506EA File Offset: 0x0004E8EA
		public NodeGraph airNodes { get; private set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060012B6 RID: 4790 RVA: 0x000506F3 File Offset: 0x0004E8F3
		// (set) Token: 0x060012B7 RID: 4791 RVA: 0x000506FB File Offset: 0x0004E8FB
		public NodeGraph railNodes { get; private set; }

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060012B8 RID: 4792 RVA: 0x00050704 File Offset: 0x0004E904
		// (set) Token: 0x060012B9 RID: 4793 RVA: 0x0005070C File Offset: 0x0004E90C
		public SceneDef sceneDef { get; private set; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060012BA RID: 4794 RVA: 0x00050715 File Offset: 0x0004E915
		public bool countsAsStage
		{
			get
			{
				return this.sceneDef && this.sceneDef.sceneType == SceneType.Stage;
			}
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x00050734 File Offset: 0x0004E934
		private void Awake()
		{
			if (this.groundNodesAsset)
			{
				this.groundNodes = UnityEngine.Object.Instantiate<NodeGraph>(this.groundNodesAsset);
			}
			if (this.airNodesAsset)
			{
				this.airNodes = UnityEngine.Object.Instantiate<NodeGraph>(this.airNodesAsset);
			}
			this.sceneDef = SceneCatalog.GetSceneDefFromSceneName(base.gameObject.scene.name);
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x0005079B File Offset: 0x0004E99B
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.groundNodes);
			UnityEngine.Object.Destroy(this.airNodes);
			UnityEngine.Object.Destroy(this.railNodes);
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x000507BE File Offset: 0x0004E9BE
		public MapNodeGroup GetNodeGroup(MapNodeGroup.GraphType nodeGraphType)
		{
			switch (nodeGraphType)
			{
			case MapNodeGroup.GraphType.Ground:
				return this.groundNodeGroup;
			case MapNodeGroup.GraphType.Air:
				return this.airNodeGroup;
			case MapNodeGroup.GraphType.Rail:
				return this.railNodeGroup;
			default:
				return null;
			}
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x000507EA File Offset: 0x0004E9EA
		public NodeGraph GetNodeGraph(MapNodeGroup.GraphType nodeGraphType)
		{
			switch (nodeGraphType)
			{
			case MapNodeGroup.GraphType.Ground:
				return this.groundNodes;
			case MapNodeGroup.GraphType.Air:
				return this.airNodes;
			case MapNodeGroup.GraphType.Rail:
				return this.railNodes;
			default:
				return null;
			}
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x00050816 File Offset: 0x0004EA16
		public void SetGateState(string gateName, bool gateEnabled)
		{
			this.groundNodes.SetGateState(gateName, gateEnabled);
			this.airNodes.SetGateState(gateName, gateEnabled);
			if (this.railNodes)
			{
				this.railNodes.SetGateState(gateName, gateEnabled);
			}
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x0005084C File Offset: 0x0004EA4C
		private void OnEnable()
		{
			if (!SceneInfo._instance)
			{
				SceneInfo._instance = this;
			}
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x00050860 File Offset: 0x0004EA60
		private void OnDisable()
		{
			if (SceneInfo._instance == this)
			{
				SceneInfo._instance = null;
			}
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x00050875 File Offset: 0x0004EA75
		private void OnValidate()
		{
			if (this.groundNodeGroup)
			{
				this.groundNodesAsset = this.groundNodeGroup.nodeGraph;
			}
			if (this.airNodeGroup)
			{
				this.airNodesAsset = this.airNodeGroup.nodeGraph;
			}
		}

		// Token: 0x0400118F RID: 4495
		private static SceneInfo _instance;

		// Token: 0x04001190 RID: 4496
		[FormerlySerializedAs("groundNodes")]
		public MapNodeGroup groundNodeGroup;

		// Token: 0x04001191 RID: 4497
		[FormerlySerializedAs("airNodes")]
		public MapNodeGroup airNodeGroup;

		// Token: 0x04001192 RID: 4498
		[FormerlySerializedAs("railNodes")]
		public MapNodeGroup railNodeGroup;

		// Token: 0x04001193 RID: 4499
		[SerializeField]
		private NodeGraph groundNodesAsset;

		// Token: 0x04001194 RID: 4500
		[SerializeField]
		private NodeGraph airNodesAsset;
	}
}
