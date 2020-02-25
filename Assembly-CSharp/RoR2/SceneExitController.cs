using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200031A RID: 794
	public class SceneExitController : MonoBehaviour
	{
		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060012A7 RID: 4775 RVA: 0x000504C1 File Offset: 0x0004E6C1
		// (set) Token: 0x060012A8 RID: 4776 RVA: 0x000504C8 File Offset: 0x0004E6C8
		public static bool isRunning { get; private set; }

		// Token: 0x060012A9 RID: 4777 RVA: 0x000504D0 File Offset: 0x0004E6D0
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

		// Token: 0x060012AA RID: 4778 RVA: 0x00050500 File Offset: 0x0004E700
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
				SceneExitController.isRunning = true;
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

		// Token: 0x060012AB RID: 4779 RVA: 0x00050632 File Offset: 0x0004E832
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateServer();
			}
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x00050644 File Offset: 0x0004E844
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

		// Token: 0x060012AD RID: 4781 RVA: 0x000506B2 File Offset: 0x0004E8B2
		private void OnDestroy()
		{
			SceneExitController.isRunning = false;
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x000506BA File Offset: 0x0004E8BA
		private void OnEnable()
		{
			InstanceTracker.Add<SceneExitController>(this);
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x000506C2 File Offset: 0x0004E8C2
		private void OnDisable()
		{
			InstanceTracker.Remove<SceneExitController>(this);
		}

		// Token: 0x04001184 RID: 4484
		public bool useRunNextStageScene;

		// Token: 0x04001185 RID: 4485
		public SceneDef destinationScene;

		// Token: 0x04001186 RID: 4486
		private const float teleportOutDuration = 4f;

		// Token: 0x04001187 RID: 4487
		private float teleportOutTimer;

		// Token: 0x04001188 RID: 4488
		private SceneExitController.ExitState exitState;

		// Token: 0x04001189 RID: 4489
		private ConvertPlayerMoneyToExperience experienceCollector;

		// Token: 0x0200031B RID: 795
		public enum ExitState
		{
			// Token: 0x0400118B RID: 4491
			Idle,
			// Token: 0x0400118C RID: 4492
			ExtractExp,
			// Token: 0x0400118D RID: 4493
			TeleportOut,
			// Token: 0x0400118E RID: 4494
			Finished
		}
	}
}
