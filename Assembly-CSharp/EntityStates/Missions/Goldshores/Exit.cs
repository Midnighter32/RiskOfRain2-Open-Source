using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Missions.Goldshores
{
	// Token: 0x020007B9 RID: 1977
	public class Exit : EntityState
	{
		// Token: 0x06002D2C RID: 11564 RVA: 0x000BEBAC File Offset: 0x000BCDAC
		public override void OnEnter()
		{
			base.OnEnter();
			if (NetworkServer.active)
			{
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/InteractableSpawnCard/iscGoldshoresPortal"), new DirectorPlacementRule
				{
					maxDistance = float.PositiveInfinity,
					minDistance = 10f,
					placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
					position = base.transform.position,
					spawnOnTarget = GoldshoresMissionController.instance.bossSpawnPosition
				}, Run.instance.stageRng));
				if (gameObject)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "PORTAL_GOLDSHORES_OPEN"
					});
					gameObject.GetComponent<SceneExitController>().useRunNextStageScene = true;
				}
				for (int i = CombatDirector.instancesList.Count - 1; i >= 0; i--)
				{
					CombatDirector.instancesList[i].enabled = false;
				}
			}
		}

		// Token: 0x06002D2D RID: 11565 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}
	}
}
