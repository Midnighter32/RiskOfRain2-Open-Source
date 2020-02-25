using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Interactables.MSObelisk
{
	// Token: 0x0200080F RID: 2063
	public class EndingGame : BaseState
	{
		// Token: 0x06002EDB RID: 11995 RVA: 0x000C7812 File Offset: 0x000C5A12
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x06002EDC RID: 11996 RVA: 0x000C7828 File Offset: 0x000C5A28
		private void FixedUpdateServer()
		{
			this.destroyTimer -= Time.fixedDeltaTime;
			if (!this.beginEndingGame)
			{
				if (this.destroyTimer <= 0f)
				{
					this.destroyTimer = EndingGame.timeBetweenDestroy;
					ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
					if (teamMembers.Count <= 0)
					{
						this.beginEndingGame = true;
						return;
					}
					GameObject gameObject = teamMembers[0].gameObject;
					CharacterBody component = gameObject.GetComponent<CharacterBody>();
					if (component)
					{
						EffectManager.SpawnEffect(EndingGame.destroyEffectPrefab, new EffectData
						{
							origin = component.corePosition,
							scale = component.radius
						}, true);
						EntityState.Destroy(gameObject.gameObject);
						return;
					}
				}
			}
			else
			{
				this.endGameTimer += Time.fixedDeltaTime;
				if (this.endGameTimer >= EndingGame.timeUntilEndGame && Run.instance)
				{
					this.DoFinalAction();
				}
			}
		}

		// Token: 0x06002EDD RID: 11997 RVA: 0x000C7908 File Offset: 0x000C5B08
		private void DoFinalAction()
		{
			bool flag = false;
			for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
			{
				if (CharacterMaster.readOnlyInstancesList[i].inventory.GetItemCount(ItemIndex.LunarTrinket) > 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this.outer.SetNextState(new TransitionToNextStage());
				return;
			}
			Run.instance.BeginGameOver(GameResultType.Unknown);
			this.outer.SetNextState(new Idle());
		}

		// Token: 0x04002C25 RID: 11301
		public static GameObject destroyEffectPrefab;

		// Token: 0x04002C26 RID: 11302
		public static float timeBetweenDestroy;

		// Token: 0x04002C27 RID: 11303
		public static float timeUntilEndGame;

		// Token: 0x04002C28 RID: 11304
		private float destroyTimer;

		// Token: 0x04002C29 RID: 11305
		private float endGameTimer;

		// Token: 0x04002C2A RID: 11306
		private bool beginEndingGame;
	}
}
