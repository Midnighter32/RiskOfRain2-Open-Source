using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000324 RID: 804
	public class SeerStationController : NetworkBehaviour
	{
		// Token: 0x060012DE RID: 4830 RVA: 0x000510A0 File Offset: 0x0004F2A0
		private void SetTargetSceneDefIndex(int newTargetSceneDefIndex)
		{
			this.NetworktargetSceneDefIndex = newTargetSceneDefIndex;
			this.OnTargetSceneChanged(SceneCatalog.GetSceneDef(this.targetSceneDefIndex));
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x000510BA File Offset: 0x0004F2BA
		[Server]
		public void SetTargetScene(SceneDef sceneDef)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SeerStationController::SetTargetScene(RoR2.SceneDef)' called on client");
				return;
			}
			this.NetworktargetSceneDefIndex = sceneDef.sceneDefIndex;
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x000510E0 File Offset: 0x0004F2E0
		public override void OnStartClient()
		{
			base.OnStartClient();
			SceneDef targetScene = null;
			if ((ulong)this.targetSceneDefIndex < (ulong)((long)SceneCatalog.sceneDefCount))
			{
				targetScene = SceneCatalog.GetSceneDef(this.targetSceneDefIndex);
			}
			this.OnTargetSceneChanged(targetScene);
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x00051118 File Offset: 0x0004F318
		private void OnTargetSceneChanged(SceneDef targetScene)
		{
			Material portalMaterial = null;
			if (targetScene)
			{
				portalMaterial = targetScene.portalMaterial;
			}
			this.SetPortalMaterial(portalMaterial);
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0005113D File Offset: 0x0004F33D
		private void SetPortalMaterial(Material portalMaterial)
		{
			this.targetRenderer.GetSharedMaterials(SeerStationController.sharedSharedMaterialsList);
			SeerStationController.sharedSharedMaterialsList[this.materialIndexToAssign] = portalMaterial;
			this.targetRenderer.SetSharedMaterials(SeerStationController.sharedSharedMaterialsList);
			SeerStationController.sharedSharedMaterialsList.Clear();
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0005117C File Offset: 0x0004F37C
		[Server]
		public void SetRunNextStageToTarget()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SeerStationController::SetRunNextStageToTarget()' called on client");
				return;
			}
			SceneDef sceneDef = SceneCatalog.GetSceneDef(this.targetSceneDefIndex);
			if (sceneDef)
			{
				SceneExitController sceneExitController = this.explicitTargetSceneExitController;
				if (!sceneExitController && this.fallBackToFirstActiveExitController)
				{
					sceneExitController = InstanceTracker.FirstOrNull<SceneExitController>();
				}
				if (sceneExitController)
				{
					sceneExitController.destinationScene = sceneDef;
					sceneExitController.useRunNextStageScene = false;
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = sceneDef.portalSelectionMessageString
					});
				}
			}
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060012E7 RID: 4839 RVA: 0x00051218 File Offset: 0x0004F418
		// (set) Token: 0x060012E8 RID: 4840 RVA: 0x0005122B File Offset: 0x0004F42B
		public int NetworktargetSceneDefIndex
		{
			get
			{
				return this.targetSceneDefIndex;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetTargetSceneDefIndex(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<int>(value, ref this.targetSceneDefIndex, dirtyBit);
			}
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x0005126C File Offset: 0x0004F46C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.targetSceneDefIndex);
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
				writer.WritePackedUInt32((uint)this.targetSceneDefIndex);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x000512D8 File Offset: 0x0004F4D8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.targetSceneDefIndex = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetTargetSceneDefIndex((int)reader.ReadPackedUInt32());
			}
		}

		// Token: 0x040011C0 RID: 4544
		[SyncVar(hook = "SetTargetSceneDefIndex")]
		private int targetSceneDefIndex = -1;

		// Token: 0x040011C1 RID: 4545
		public SceneExitController explicitTargetSceneExitController;

		// Token: 0x040011C2 RID: 4546
		public bool fallBackToFirstActiveExitController;

		// Token: 0x040011C3 RID: 4547
		public Renderer targetRenderer;

		// Token: 0x040011C4 RID: 4548
		public int materialIndexToAssign;

		// Token: 0x040011C5 RID: 4549
		private static List<Material> sharedSharedMaterialsList = new List<Material>();
	}
}
