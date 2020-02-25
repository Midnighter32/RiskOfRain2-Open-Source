using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Missions.SuperRoboBallEncounter
{
	// Token: 0x020007B5 RID: 1973
	public class Listening : EntityState
	{
		// Token: 0x06002D1A RID: 11546 RVA: 0x000BE6DF File Offset: 0x000BC8DF
		public override void OnEnter()
		{
			base.OnEnter();
			this.scriptedCombatEncounter = base.GetComponent<ScriptedCombatEncounter>();
		}

		// Token: 0x06002D1B RID: 11547 RVA: 0x000BE6F4 File Offset: 0x000BC8F4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				if (base.fixedAge >= 2f)
				{
					this.RegisterEggs();
				}
				if (this.hasRegisteredEggs)
				{
					int num = 0;
					for (int i = 0; i < this.eggList.Count; i++)
					{
						if (this.eggList[i] == null)
						{
							num++;
						}
					}
					if (this.previousDestroyedEggCount == Listening.eggsDestroyedToTriggerEncounter - 2 && num == Listening.eggsDestroyedToTriggerEncounter - 1)
					{
						Chat.SendBroadcastChat(new Chat.SimpleChatMessage
						{
							baseToken = "VULTURE_EGG_WARNING"
						});
					}
					if (num == Listening.eggsDestroyedToTriggerEncounter && !this.beginEncounterCountdown)
					{
						this.encounterCountdown = Listening.delayBeforeBeginningEncounter;
						this.beginEncounterCountdown = true;
						Chat.SendBroadcastChat(new Chat.SimpleChatMessage
						{
							baseToken = "VULTURE_EGG_BEGIN"
						});
					}
					if (this.beginEncounterCountdown)
					{
						this.encounterCountdown -= Time.fixedDeltaTime;
						if (this.encounterCountdown <= 0f)
						{
							this.scriptedCombatEncounter.BeginEncounter();
							this.outer.SetNextState(new Idle());
						}
					}
					this.previousDestroyedEggCount = num;
				}
			}
		}

		// Token: 0x06002D1C RID: 11548 RVA: 0x000BE810 File Offset: 0x000BCA10
		private void RegisterEggs()
		{
			if (this.hasRegisteredEggs)
			{
				return;
			}
			ReadOnlyCollection<CharacterBody> readOnlyInstancesList = CharacterBody.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				if (readOnlyInstancesList[i].name.Contains("VultureEgg"))
				{
					this.eggList.Add(readOnlyInstancesList[i].gameObject);
					Debug.Log("Found egg!");
				}
			}
			this.hasRegisteredEggs = true;
		}

		// Token: 0x0400295B RID: 10587
		public static float delayBeforeBeginningEncounter;

		// Token: 0x0400295C RID: 10588
		public static int eggsDestroyedToTriggerEncounter;

		// Token: 0x0400295D RID: 10589
		private ScriptedCombatEncounter scriptedCombatEncounter;

		// Token: 0x0400295E RID: 10590
		private List<GameObject> eggList = new List<GameObject>();

		// Token: 0x0400295F RID: 10591
		private const float delayBeforeRegisteringEggs = 2f;

		// Token: 0x04002960 RID: 10592
		private bool hasRegisteredEggs;

		// Token: 0x04002961 RID: 10593
		private int previousDestroyedEggCount;

		// Token: 0x04002962 RID: 10594
		private bool beginEncounterCountdown;

		// Token: 0x04002963 RID: 10595
		private float encounterCountdown;
	}
}
