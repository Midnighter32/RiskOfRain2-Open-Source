using System;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020003CE RID: 974
	public class SceneInfo : MonoBehaviour
	{
		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x0600151F RID: 5407 RVA: 0x000658CE File Offset: 0x00063ACE
		public static SceneInfo instance
		{
			get
			{
				return SceneInfo._instance;
			}
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06001520 RID: 5408 RVA: 0x000658D5 File Offset: 0x00063AD5
		// (set) Token: 0x06001521 RID: 5409 RVA: 0x000658DD File Offset: 0x00063ADD
		public SceneDef sceneDef { get; private set; }

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06001522 RID: 5410 RVA: 0x000658E6 File Offset: 0x00063AE6
		public bool countsAsStage
		{
			get
			{
				return this.sceneDef && this.sceneDef.sceneType == SceneType.Stage;
			}
		}

		// Token: 0x06001523 RID: 5411 RVA: 0x00065908 File Offset: 0x00063B08
		private void Awake()
		{
			if (this.groundNodeGroup)
			{
				this.groundNodes = UnityEngine.Object.Instantiate<NodeGraph>(this.groundNodeGroup.nodeGraph);
			}
			if (this.airNodeGroup)
			{
				this.airNodes = UnityEngine.Object.Instantiate<NodeGraph>(this.airNodeGroup.nodeGraph);
			}
			if (this.railNodeGroup)
			{
				this.railNodes = UnityEngine.Object.Instantiate<NodeGraph>(this.railNodeGroup.nodeGraph);
			}
			this.sceneDef = SceneCatalog.GetSceneDefFromSceneName(base.gameObject.scene.name);
		}

		// Token: 0x06001524 RID: 5412 RVA: 0x0006599C File Offset: 0x00063B9C
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.groundNodes);
			UnityEngine.Object.Destroy(this.airNodes);
			UnityEngine.Object.Destroy(this.railNodes);
		}

		// Token: 0x06001525 RID: 5413 RVA: 0x000659BF File Offset: 0x00063BBF
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

		// Token: 0x06001526 RID: 5414 RVA: 0x000659EB File Offset: 0x00063BEB
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

		// Token: 0x06001527 RID: 5415 RVA: 0x00065A17 File Offset: 0x00063C17
		public void SetGateState(string gateName, bool gateEnabled)
		{
			this.groundNodes.SetGateState(gateName, gateEnabled);
			this.airNodes.SetGateState(gateName, gateEnabled);
			if (this.railNodes)
			{
				this.railNodes.SetGateState(gateName, gateEnabled);
			}
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x00065A4D File Offset: 0x00063C4D
		private void OnEnable()
		{
			if (!SceneInfo._instance)
			{
				SceneInfo._instance = this;
			}
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x00065A61 File Offset: 0x00063C61
		private void OnDisable()
		{
			if (SceneInfo._instance == this)
			{
				SceneInfo._instance = null;
			}
		}

		// Token: 0x0400186E RID: 6254
		private static SceneInfo _instance;

		// Token: 0x0400186F RID: 6255
		[FormerlySerializedAs("groundNodes")]
		public MapNodeGroup groundNodeGroup;

		// Token: 0x04001870 RID: 6256
		[FormerlySerializedAs("airNodes")]
		public MapNodeGroup airNodeGroup;

		// Token: 0x04001871 RID: 6257
		[FormerlySerializedAs("railNodes")]
		public MapNodeGroup railNodeGroup;

		// Token: 0x04001872 RID: 6258
		[NonSerialized]
		public NodeGraph groundNodes;

		// Token: 0x04001873 RID: 6259
		[NonSerialized]
		public NodeGraph airNodes;

		// Token: 0x04001874 RID: 6260
		[NonSerialized]
		public NodeGraph railNodes;
	}
}
