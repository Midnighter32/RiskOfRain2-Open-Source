using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.CharacterAI;
using RoR2.ConVar;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003EA RID: 1002
	public class Stage : NetworkBehaviour
	{
		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060015D1 RID: 5585 RVA: 0x0006895B File Offset: 0x00066B5B
		// (set) Token: 0x060015D2 RID: 5586 RVA: 0x00068962 File Offset: 0x00066B62
		public static Stage instance { get; private set; }

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060015D3 RID: 5587 RVA: 0x0006896A File Offset: 0x00066B6A
		// (set) Token: 0x060015D4 RID: 5588 RVA: 0x00068972 File Offset: 0x00066B72
		public SceneDef sceneDef { get; private set; }

		// Token: 0x060015D5 RID: 5589 RVA: 0x0006897C File Offset: 0x00066B7C
		private void Start()
		{
			this.sceneDef = SceneCatalog.GetSceneDefForCurrentScene();
			if (NetworkServer.active)
			{
				this.NetworkstartRunTime = Run.instance.fixedTime;
				this.stageSpawnPosition = this.SampleNodeGraphForSpawnPosition();
				ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
				Transform playerSpawnTransform = this.GetPlayerSpawnTransform();
				for (int i = 0; i < readOnlyInstancesList.Count; i++)
				{
					CharacterMaster characterMaster = readOnlyInstancesList[i];
					if (characterMaster && !characterMaster.GetComponent<PlayerCharacterMasterController>() && !characterMaster.GetBodyObject() && characterMaster.gameObject.scene.buildIndex == -1)
					{
						Vector3 vector = Vector3.zero;
						Quaternion rotation = Quaternion.identity;
						if (playerSpawnTransform)
						{
							vector = playerSpawnTransform.position;
							rotation = playerSpawnTransform.rotation;
							BaseAI component = readOnlyInstancesList[i].GetComponent<BaseAI>();
							CharacterBody component2 = readOnlyInstancesList[i].bodyPrefab.GetComponent<CharacterBody>();
							if (component && component2)
							{
								NodeGraph nodeGraph = component.GetNodeGraph();
								if (nodeGraph)
								{
									List<NodeGraph.NodeIndex> list = nodeGraph.FindNodesInRange(vector, 10f, 100f, (HullMask)(1 << (int)component2.hullClassification));
									if ((float)list.Count > 0f)
									{
										nodeGraph.GetNodePosition(list[UnityEngine.Random.Range(0, list.Count)], out vector);
									}
								}
							}
						}
						readOnlyInstancesList[i].Respawn(vector, rotation);
					}
				}
				this.BeginServer();
			}
			if (NetworkClient.active)
			{
				this.RespawnLocalPlayers();
			}
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x00068B0C File Offset: 0x00066D0C
		[Client]
		public void RespawnLocalPlayers()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.Stage::RespawnLocalPlayers()' called on server");
				return;
			}
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				NetworkUser networkUser = readOnlyInstancesList[i];
				CharacterMaster characterMaster = null;
				if (networkUser.isLocalPlayer && networkUser.masterObject)
				{
					characterMaster = networkUser.masterObject.GetComponent<CharacterMaster>();
				}
				if (characterMaster)
				{
					characterMaster.CallCmdRespawn("");
				}
			}
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x00068B83 File Offset: 0x00066D83
		private void OnEnable()
		{
			Stage.instance = SingletonHelper.Assign<Stage>(Stage.instance, this);
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x00068B95 File Offset: 0x00066D95
		private void OnDisable()
		{
			Stage.instance = SingletonHelper.Unassign<Stage>(Stage.instance, this);
		}

		// Token: 0x060015D9 RID: 5593 RVA: 0x00068BA8 File Offset: 0x00066DA8
		private Vector3 SampleNodeGraphForSpawnPosition()
		{
			Vector3 zero = Vector3.zero;
			NodeGraph groundNodes = SceneInfo.instance.groundNodes;
			NodeFlags requiredFlags = this.usePod ? NodeFlags.NoCeiling : NodeFlags.None;
			List<NodeGraph.NodeIndex> activeNodesForHullMaskWithFlagConditions = groundNodes.GetActiveNodesForHullMaskWithFlagConditions(HullMask.BeetleQueen, requiredFlags, NodeFlags.None);
			if (activeNodesForHullMaskWithFlagConditions.Count < 0)
			{
				Debug.LogWarning("No spawn points available in scene!");
				return Vector3.zero;
			}
			NodeGraph.NodeIndex nodeIndex = activeNodesForHullMaskWithFlagConditions[Run.instance.spawnRng.RangeInt(0, activeNodesForHullMaskWithFlagConditions.Count)];
			groundNodes.GetNodePosition(nodeIndex, out zero);
			return zero;
		}

		// Token: 0x060015DA RID: 5594 RVA: 0x00068C20 File Offset: 0x00066E20
		[Server]
		public Transform GetPlayerSpawnTransform()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'UnityEngine.Transform RoR2.Stage::GetPlayerSpawnTransform()' called on client");
				return null;
			}
			SpawnPoint spawnPoint = SpawnPoint.ConsumeSpawnPoint();
			if (spawnPoint)
			{
				return spawnPoint.transform;
			}
			return null;
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x00068C64 File Offset: 0x00066E64
		[Server]
		public void RespawnCharacter(CharacterMaster characterMaster)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stage::RespawnCharacter(RoR2.CharacterMaster)' called on client");
				return;
			}
			if (!characterMaster)
			{
				return;
			}
			Transform playerSpawnTransform = this.GetPlayerSpawnTransform();
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			if (playerSpawnTransform)
			{
				vector = playerSpawnTransform.position;
				quaternion = playerSpawnTransform.rotation;
			}
			characterMaster.Respawn(vector, quaternion);
			if (characterMaster.GetComponent<PlayerCharacterMasterController>())
			{
				this.spawnedAnyPlayer = true;
			}
			if (this.usePod)
			{
				Run.instance.HandlePlayerFirstEntryAnimation(characterMaster.GetBody(), vector, quaternion);
			}
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x00068CEF File Offset: 0x00066EEF
		[Server]
		public void BeginAdvanceStage(string destinationStage)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stage::BeginAdvanceStage(System.String)' called on client");
				return;
			}
			this.NetworkstageAdvanceTime = Run.instance.fixedTime + 0.75f;
			this.nextStage = destinationStage;
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060015DD RID: 5597 RVA: 0x00068D23 File Offset: 0x00066F23
		// (set) Token: 0x060015DE RID: 5598 RVA: 0x00068D2B File Offset: 0x00066F2B
		public bool completed { get; private set; }

		// Token: 0x060015DF RID: 5599 RVA: 0x00068D34 File Offset: 0x00066F34
		[Server]
		private void BeginServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stage::BeginServer()' called on client");
				return;
			}
			Action<Stage> action = Stage.onServerStageBegin;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x00068D5B File Offset: 0x00066F5B
		[Server]
		public void CompleteServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stage::CompleteServer()' called on client");
				return;
			}
			if (this.completed)
			{
				return;
			}
			this.completed = true;
			Action<Stage> action = Stage.onServerStageComplete;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x060015E1 RID: 5601 RVA: 0x00068D94 File Offset: 0x00066F94
		// (remove) Token: 0x060015E2 RID: 5602 RVA: 0x00068DC8 File Offset: 0x00066FC8
		public static event Action<Stage> onServerStageBegin;

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x060015E3 RID: 5603 RVA: 0x00068DFC File Offset: 0x00066FFC
		// (remove) Token: 0x060015E4 RID: 5604 RVA: 0x00068E30 File Offset: 0x00067030
		public static event Action<Stage> onServerStageComplete;

		// Token: 0x060015E5 RID: 5605 RVA: 0x00068E64 File Offset: 0x00067064
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (!string.IsNullOrEmpty(this.nextStage) && this.stageAdvanceTime <= Run.instance.fixedTime)
				{
					string nextSceneName = this.nextStage;
					this.nextStage = null;
					Run.instance.AdvanceStage(nextSceneName);
				}
				if (this.spawnedAnyPlayer && float.IsInfinity(this.stageAdvanceTime) && !Run.instance.isGameOverServer)
				{
					ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
					bool flag = false;
					for (int i = 0; i < instances.Count; i++)
					{
						PlayerCharacterMasterController playerCharacterMasterController = instances[i];
						if (playerCharacterMasterController.isConnected && playerCharacterMasterController.preventGameOver)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						Run.instance.BeginGameOver(GameResultType.Lost);
					}
				}
			}
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060015E9 RID: 5609 RVA: 0x00068F98 File Offset: 0x00067198
		// (set) Token: 0x060015EA RID: 5610 RVA: 0x00068FAB File Offset: 0x000671AB
		public float NetworkstartRunTime
		{
			get
			{
				return this.startRunTime;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.startRunTime, 1u);
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060015EB RID: 5611 RVA: 0x00068FC0 File Offset: 0x000671C0
		// (set) Token: 0x060015EC RID: 5612 RVA: 0x00068FD3 File Offset: 0x000671D3
		public float NetworkstageAdvanceTime
		{
			get
			{
				return this.stageAdvanceTime;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.stageAdvanceTime, 2u);
			}
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x00068FE8 File Offset: 0x000671E8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.startRunTime);
				writer.Write(this.stageAdvanceTime);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.startRunTime);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.stageAdvanceTime);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x00069094 File Offset: 0x00067294
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.startRunTime = reader.ReadSingle();
				this.stageAdvanceTime = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.startRunTime = reader.ReadSingle();
			}
			if ((num & 2) != 0)
			{
				this.stageAdvanceTime = reader.ReadSingle();
			}
		}

		// Token: 0x04001939 RID: 6457
		[SyncVar]
		public float startRunTime;

		// Token: 0x0400193B RID: 6459
		private Vector3 stageSpawnPosition = Vector3.zero;

		// Token: 0x0400193C RID: 6460
		private bool spawnedAnyPlayer;

		// Token: 0x0400193D RID: 6461
		[NonSerialized]
		public bool usePod = Run.instance && Run.instance.stageClearCount == 0 && Stage.stage1PodConVar.value;

		// Token: 0x0400193E RID: 6462
		private static BoolConVar stage1PodConVar = new BoolConVar("stage1_pod", ConVarFlags.Cheat, "1", "Whether or not to use the pod when spawning on the first stage.");

		// Token: 0x0400193F RID: 6463
		[SyncVar]
		public float stageAdvanceTime = float.PositiveInfinity;

		// Token: 0x04001940 RID: 6464
		public const float stageAdvanceTransitionDuration = 0.5f;

		// Token: 0x04001941 RID: 6465
		public const float stageAdvanceTransitionDelay = 0.75f;

		// Token: 0x04001942 RID: 6466
		private string nextStage = "";
	}
}
