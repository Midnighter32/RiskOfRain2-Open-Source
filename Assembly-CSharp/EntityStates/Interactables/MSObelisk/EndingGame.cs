using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Interactables.MSObelisk
{
	// Token: 0x02000139 RID: 313
	public class EndingGame : BaseState
	{
		// Token: 0x06000604 RID: 1540 RVA: 0x0001BB64 File Offset: 0x00019D64
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!NetworkServer.active)
			{
				return;
			}
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
						EffectManager.instance.SpawnEffect(EndingGame.destroyEffectPrefab, new EffectData
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
					Run.instance.BeginGameOver(GameResultType.Unknown);
				}
			}
		}

		// Token: 0x04000701 RID: 1793
		public static GameObject destroyEffectPrefab;

		// Token: 0x04000702 RID: 1794
		public static float timeBetweenDestroy;

		// Token: 0x04000703 RID: 1795
		public static float timeUntilEndGame;

		// Token: 0x04000704 RID: 1796
		private float destroyTimer;

		// Token: 0x04000705 RID: 1797
		private float endGameTimer;

		// Token: 0x04000706 RID: 1798
		private bool beginEndingGame;
	}
}
