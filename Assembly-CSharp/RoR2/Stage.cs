using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using RoR2.CharacterAI;
using RoR2.ConVar;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033F RID: 831
	public class Stage : NetworkBehaviour
	{
		// Token: 0x17000259 RID: 601
		// (get) Token: 0x060013C5 RID: 5061 RVA: 0x00054906 File Offset: 0x00052B06
		// (set) Token: 0x060013C6 RID: 5062 RVA: 0x0005490D File Offset: 0x00052B0D
		public static Stage instance { get; private set; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x060013C7 RID: 5063 RVA: 0x00054915 File Offset: 0x00052B15
		// (set) Token: 0x060013C8 RID: 5064 RVA: 0x0005491D File Offset: 0x00052B1D
		public SceneDef sceneDef { get; private set; }

		// Token: 0x060013C9 RID: 5065 RVA: 0x00054926 File Offset: 0x00052B26
		private void Start()
		{
			this.sceneDef = SceneCatalog.GetSceneDefForCurrentScene();
			if (NetworkServer.active)
			{
				this.NetworkstartRunTime = Run.instance.fixedTime;
				this.RespawnAllNPCs();
				this.BeginServer();
			}
			if (NetworkClient.active)
			{
				this.RespawnLocalPlayers();
			}
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x00054964 File Offset: 0x00052B64
		private void RespawnAllNPCs()
		{
			if (this.sceneDef.suppressNpcEntry)
			{
				return;
			}
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
							NodeGraph desiredSpawnNodeGraph = component.GetDesiredSpawnNodeGraph();
							if (desiredSpawnNodeGraph)
							{
								List<NodeGraph.NodeIndex> list = desiredSpawnNodeGraph.FindNodesInRange(vector, 10f, 100f, (HullMask)(1 << (int)component2.hullClassification));
								if ((float)list.Count > 0f)
								{
									desiredSpawnNodeGraph.GetNodePosition(list[UnityEngine.Random.Range(0, list.Count)], out vector);
								}
							}
						}
					}
					readOnlyInstancesList[i].Respawn(vector, rotation, false);
				}
			}
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x00054AC0 File Offset: 0x00052CC0
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

		// Token: 0x060013CC RID: 5068 RVA: 0x00054B37 File Offset: 0x00052D37
		private void OnEnable()
		{
			Stage.instance = SingletonHelper.Assign<Stage>(Stage.instance, this);
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x00054B49 File Offset: 0x00052D49
		private void OnDisable()
		{
			Stage.instance = SingletonHelper.Unassign<Stage>(Stage.instance, this);
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x00054B5C File Offset: 0x00052D5C
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

		// Token: 0x060013CF RID: 5071 RVA: 0x00054BA0 File Offset: 0x00052DA0
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
			characterMaster.Respawn(vector, quaternion, true);
			if (characterMaster.GetComponent<PlayerCharacterMasterController>())
			{
				this.spawnedAnyPlayer = true;
			}
			if (this.usePod)
			{
				Run.instance.HandlePlayerFirstEntryAnimation(characterMaster.GetBody(), vector, quaternion);
			}
		}

		// Token: 0x060013D0 RID: 5072 RVA: 0x00054C2C File Offset: 0x00052E2C
		[Server]
		public void BeginAdvanceStage(SceneDef destinationStage)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Stage::BeginAdvanceStage(RoR2.SceneDef)' called on client");
				return;
			}
			this.NetworkstageAdvanceTime = Run.instance.fixedTime + 0.75f;
			this.nextStage = destinationStage;
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x060013D1 RID: 5073 RVA: 0x00054C60 File Offset: 0x00052E60
		// (set) Token: 0x060013D2 RID: 5074 RVA: 0x00054C68 File Offset: 0x00052E68
		public bool completed { get; private set; }

		// Token: 0x060013D3 RID: 5075 RVA: 0x00054C71 File Offset: 0x00052E71
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

		// Token: 0x060013D4 RID: 5076 RVA: 0x00054C98 File Offset: 0x00052E98
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

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x060013D5 RID: 5077 RVA: 0x00054CD0 File Offset: 0x00052ED0
		// (remove) Token: 0x060013D6 RID: 5078 RVA: 0x00054D04 File Offset: 0x00052F04
		public static event Action<Stage> onServerStageBegin;

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x060013D7 RID: 5079 RVA: 0x00054D38 File Offset: 0x00052F38
		// (remove) Token: 0x060013D8 RID: 5080 RVA: 0x00054D6C File Offset: 0x00052F6C
		public static event Action<Stage> onServerStageComplete;

		// Token: 0x060013D9 RID: 5081 RVA: 0x00054DA0 File Offset: 0x00052FA0
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (this.nextStage && this.stageAdvanceTime <= Run.instance.fixedTime)
				{
					SceneDef nextScene = this.nextStage;
					this.nextStage = null;
					Run.instance.AdvanceStage(nextScene);
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

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x060013DA RID: 5082 RVA: 0x00054E56 File Offset: 0x00053056
		// (set) Token: 0x060013DB RID: 5083 RVA: 0x00054E5E File Offset: 0x0005305E
		public bool scavPackDroppedServer { get; set; }

		// Token: 0x060013DE RID: 5086 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060013DF RID: 5087 RVA: 0x00054EC4 File Offset: 0x000530C4
		// (set) Token: 0x060013E0 RID: 5088 RVA: 0x00054ED7 File Offset: 0x000530D7
		public float NetworkstartRunTime
		{
			get
			{
				return this.startRunTime;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.startRunTime, 1U);
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060013E1 RID: 5089 RVA: 0x00054EEC File Offset: 0x000530EC
		// (set) Token: 0x060013E2 RID: 5090 RVA: 0x00054EFF File Offset: 0x000530FF
		public float NetworkstageAdvanceTime
		{
			get
			{
				return this.stageAdvanceTime;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.stageAdvanceTime, 2U);
			}
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x00054F14 File Offset: 0x00053114
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.startRunTime);
				writer.Write(this.stageAdvanceTime);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.startRunTime);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
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

		// Token: 0x060013E4 RID: 5092 RVA: 0x00054FC0 File Offset: 0x000531C0
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

		// Token: 0x04001294 RID: 4756
		[SyncVar]
		public float startRunTime;

		// Token: 0x04001296 RID: 4758
		private bool spawnedAnyPlayer;

		// Token: 0x04001297 RID: 4759
		[NonSerialized]
		public bool usePod = Run.instance && Run.instance.stageClearCount == 0 && Stage.stage1PodConVar.value;

		// Token: 0x04001298 RID: 4760
		private static BoolConVar stage1PodConVar = new BoolConVar("stage1_pod", ConVarFlags.Cheat, "1", "Whether or not to use the pod when spawning on the first stage.");

		// Token: 0x04001299 RID: 4761
		[SyncVar]
		public float stageAdvanceTime = float.PositiveInfinity;

		// Token: 0x0400129A RID: 4762
		public const float stageAdvanceTransitionDuration = 0.5f;

		// Token: 0x0400129B RID: 4763
		public const float stageAdvanceTransitionDelay = 0.75f;

		// Token: 0x0400129C RID: 4764
		private SceneDef nextStage;
	}
}
