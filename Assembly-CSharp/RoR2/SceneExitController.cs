using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003CC RID: 972
	public class SceneExitController : MonoBehaviour
	{
		// Token: 0x0600151A RID: 5402 RVA: 0x000656E9 File Offset: 0x000638E9
		public void Begin()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (this.exitState == SceneExitController.ExitState.Idle)
			{
				this.SetState(Run.instance.ruleBook.keepMoneyBetweenStages ? SceneExitController.ExitState.TeleportOut : SceneExitController.ExitState.ExtractExp);
			}
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x00065718 File Offset: 0x00063918
		public void SetState(SceneExitController.ExitState newState)
		{
			if (newState == this.exitState)
			{
				return;
			}
			this.exitState = newState;
			switch (this.exitState)
			{
			case SceneExitController.ExitState.Idle:
				break;
			case SceneExitController.ExitState.ExtractExp:
				this.experienceCollector = base.gameObject.AddComponent<ConvertPlayerMoneyToExperience>();
				return;
			case SceneExitController.ExitState.TeleportOut:
			{
				ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
				for (int i = 0; i < readOnlyInstancesList.Count; i++)
				{
					CharacterMaster component = readOnlyInstancesList[i].GetComponent<CharacterMaster>();
					if (component.GetComponent<SetDontDestroyOnLoad>())
					{
						GameObject bodyObject = component.GetBodyObject();
						if (bodyObject)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/TeleportOutController"), bodyObject.transform.position, Quaternion.identity);
							gameObject.GetComponent<TeleportOutController>().Networktarget = bodyObject;
							NetworkServer.Spawn(gameObject);
						}
					}
				}
				this.teleportOutTimer = 4f;
				return;
			}
			case SceneExitController.ExitState.Finished:
				if (Run.instance && Run.instance.isGameOverServer)
				{
					return;
				}
				if (this.useRunNextStageScene)
				{
					Stage.instance.BeginAdvanceStage(Run.instance.nextStageScene);
					return;
				}
				if (this.destinationScene)
				{
					Stage.instance.BeginAdvanceStage(this.destinationScene);
					return;
				}
				Debug.Log("SceneExitController: destinationScene not set!");
				break;
			default:
				return;
			}
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x0006584E File Offset: 0x00063A4E
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateServer();
			}
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x00065860 File Offset: 0x00063A60
		private void UpdateServer()
		{
			switch (this.exitState)
			{
			case SceneExitController.ExitState.Idle:
			case SceneExitController.ExitState.Finished:
				break;
			case SceneExitController.ExitState.ExtractExp:
				if (!this.experienceCollector)
				{
					this.SetState(SceneExitController.ExitState.TeleportOut);
					return;
				}
				break;
			case SceneExitController.ExitState.TeleportOut:
				this.teleportOutTimer -= Time.fixedDeltaTime;
				if (this.teleportOutTimer <= 0f)
				{
					this.SetState(SceneExitController.ExitState.Finished);
					return;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x04001863 RID: 6243
		public bool useRunNextStageScene;

		// Token: 0x04001864 RID: 6244
		public SceneField destinationScene;

		// Token: 0x04001865 RID: 6245
		private const float teleportOutDuration = 4f;

		// Token: 0x04001866 RID: 6246
		private float teleportOutTimer;

		// Token: 0x04001867 RID: 6247
		private SceneExitController.ExitState exitState;

		// Token: 0x04001868 RID: 6248
		private ConvertPlayerMoneyToExperience experienceCollector;

		// Token: 0x020003CD RID: 973
		public enum ExitState
		{
			// Token: 0x0400186A RID: 6250
			Idle,
			// Token: 0x0400186B RID: 6251
			ExtractExp,
			// Token: 0x0400186C RID: 6252
			TeleportOut,
			// Token: 0x0400186D RID: 6253
			Finished
		}
	}
}
