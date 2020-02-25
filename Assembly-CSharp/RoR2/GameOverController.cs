using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000207 RID: 519
	[RequireComponent(typeof(VoteController))]
	public class GameOverController : NetworkBehaviour
	{
		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000B12 RID: 2834 RVA: 0x00031285 File Offset: 0x0002F485
		// (set) Token: 0x06000B13 RID: 2835 RVA: 0x0003128C File Offset: 0x0002F48C
		public static GameOverController instance { get; private set; }

		// Token: 0x06000B14 RID: 2836 RVA: 0x00031294 File Offset: 0x0002F494
		[Server]
		public void SetRunReport([NotNull] RunReport newRunReport)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GameOverController::SetRunReport(RoR2.RunReport)' called on client");
				return;
			}
			base.SetDirtyBit(1U);
			this.runReport = newRunReport;
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x000312BC File Offset: 0x0002F4BC
		private void GenerateReportScreens()
		{
			GameOverController.<>c__DisplayClass11_0 CS$<>8__locals1 = new GameOverController.<>c__DisplayClass11_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.voteControllerGameObject = base.gameObject;
			VoteController component = CS$<>8__locals1.voteControllerGameObject.GetComponent<VoteController>();
			using (IEnumerator<LocalUser> enumerator = LocalUserManager.readOnlyLocalUsersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LocalUser localUser = enumerator.Current;
					CameraRigController cameraRigController = CameraRigController.readOnlyInstancesList.FirstOrDefault((CameraRigController v) => v.viewer == localUser.currentNetworkUser);
					if (cameraRigController && cameraRigController.hud)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameEndReportPanelPrefab, cameraRigController.hud.transform);
						cameraRigController.hud.mainContainer.SetActive(false);
						gameObject.transform.parent = cameraRigController.hud.transform;
						gameObject.GetComponent<MPEventSystemProvider>().eventSystem = localUser.eventSystem;
						GameEndReportPanelController component2 = gameObject.GetComponent<GameEndReportPanelController>();
						GameEndReportPanelController.DisplayData displayData = new GameEndReportPanelController.DisplayData
						{
							runReport = this.runReport,
							playerIndex = CS$<>8__locals1.<GenerateReportScreens>g__FindPlayerIndex|0(localUser)
						};
						component2.SetDisplayData(displayData);
						component2.continueButton.onClick.AddListener(delegate()
						{
							if (localUser.currentNetworkUser)
							{
								localUser.currentNetworkUser.CallCmdSubmitVote(CS$<>8__locals1.voteControllerGameObject, 0);
							}
						});
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/VoteInfoPanel"), (RectTransform)component2.continueButton.transform.parent);
						gameObject2.transform.SetAsFirstSibling();
						gameObject2.GetComponent<VoteInfoPanelController>().voteController = component;
					}
				}
			}
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x00031470 File Offset: 0x0002F670
		private void Awake()
		{
			this.runReport = new RunReport();
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x0003147D File Offset: 0x0002F67D
		private void Start()
		{
			this.appearanceTimer = this.appearanceDelay;
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x0003148B File Offset: 0x0002F68B
		private void OnEnable()
		{
			GameOverController.instance = SingletonHelper.Assign<GameOverController>(GameOverController.instance, this);
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0003149D File Offset: 0x0002F69D
		private void OnDisable()
		{
			GameOverController.instance = SingletonHelper.Unassign<GameOverController>(GameOverController.instance, this);
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x000314AF File Offset: 0x0002F6AF
		private void Update()
		{
			if (!this.reportScreensGenerated)
			{
				this.appearanceTimer -= Time.deltaTime;
				if (this.appearanceTimer <= 0f)
				{
					this.reportScreensGenerated = true;
					this.GenerateReportScreens();
				}
			}
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x000314E5 File Offset: 0x0002F6E5
		[Server]
		private void EndRun()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.GameOverController::EndRun()' called on client");
				return;
			}
			UnityEngine.Object.Destroy(Run.instance);
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x00031508 File Offset: 0x0002F708
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 1U;
			}
			bool flag = (num & 1U) > 0U;
			if (!initialState)
			{
				writer.Write((byte)num);
			}
			if (flag)
			{
				this.runReport.Write(writer);
			}
			return !initialState && num > 0U;
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x00031549 File Offset: 0x0002F749
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (((initialState ? 1 : reader.ReadByte()) & 1) > 0)
			{
				this.runReport.Read(reader);
			}
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x0003156A File Offset: 0x0002F76A
		[ClientRpc]
		public void RpcClientGameOver()
		{
			if (Run.instance)
			{
				Run.instance.OnClientGameOver(this.runReport);
			}
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x0003159B File Offset: 0x0002F79B
		protected static void InvokeRpcRpcClientGameOver(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcClientGameOver called on server.");
				return;
			}
			((GameOverController)obj).RpcClientGameOver();
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x000315C0 File Offset: 0x0002F7C0
		public void CallRpcClientGameOver()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcClientGameOver called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)GameOverController.kRpcRpcClientGameOver);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcClientGameOver");
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x00031629 File Offset: 0x0002F829
		static GameOverController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(GameOverController), GameOverController.kRpcRpcClientGameOver, new NetworkBehaviour.CmdDelegate(GameOverController.InvokeRpcRpcClientGameOver));
			NetworkCRC.RegisterBehaviour("GameOverController", 0);
		}

		// Token: 0x04000B84 RID: 2948
		[Tooltip("How long it takes after the first person has hit the continue button for the game to forcibly end.")]
		public float timeoutDuration;

		// Token: 0x04000B85 RID: 2949
		private const uint runReportDirtyBit = 1U;

		// Token: 0x04000B86 RID: 2950
		private const uint allDirtyBits = 1U;

		// Token: 0x04000B87 RID: 2951
		private RunReport runReport;

		// Token: 0x04000B88 RID: 2952
		public GameObject gameEndReportPanelPrefab;

		// Token: 0x04000B89 RID: 2953
		private bool reportScreensGenerated;

		// Token: 0x04000B8A RID: 2954
		public float appearanceDelay = 1f;

		// Token: 0x04000B8B RID: 2955
		private float appearanceTimer;

		// Token: 0x04000B8C RID: 2956
		private static int kRpcRpcClientGameOver = 1518660169;
	}
}
