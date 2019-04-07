using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Missions.Goldshores
{
	// Token: 0x02000102 RID: 258
	public class Exit : EntityState
	{
		// Token: 0x060004FC RID: 1276 RVA: 0x00015020 File Offset: 0x00013220
		public override void OnEnter()
		{
			base.OnEnter();
			GameObject gameObject = DirectorCore.instance.TrySpawnObject((SpawnCard)Resources.Load("SpawnCards/InteractableSpawnCard/iscGoldshoresPortal"), new DirectorPlacementRule
			{
				maxDistance = float.PositiveInfinity,
				minDistance = 10f,
				placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
				position = base.transform.position,
				spawnOnTarget = GoldshoresMissionController.instance.bossSpawnPosition
			}, Run.instance.stageRng);
			if (gameObject)
			{
				Chat.SendBroadcastChat(new Chat.SimpleChatMessage
				{
					baseToken = "PORTAL_GOLDSHORES_OPEN"
				});
				gameObject.GetComponent<SceneExitController>().useRunNextStageScene = true;
			}
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}
	}
}
