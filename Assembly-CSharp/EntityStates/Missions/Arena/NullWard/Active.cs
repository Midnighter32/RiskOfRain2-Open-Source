using System;
using System.Collections.Generic;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Missions.Arena.NullWard
{
	// Token: 0x020007BE RID: 1982
	public class Active : NullWardBaseState
	{
		// Token: 0x06002D43 RID: 11587 RVA: 0x000BF0C8 File Offset: 0x000BD2C8
		public override void OnEnter()
		{
			base.OnEnter();
			this.buffWard.Networkradius = NullWardBaseState.wardRadiusOn;
			this.purchaseInteraction.SetAvailable(false);
			base.arenaMissionController.rewardSpawnPosition = this.childLocator.FindChild("RewardSpawn").gameObject;
			base.arenaMissionController.monsterSpawnPosition = this.childLocator.FindChild("MonsterSpawn").gameObject;
			this.childLocator.FindChild("ActiveEffect").gameObject.SetActive(true);
			if (NetworkServer.active)
			{
				base.arenaMissionController.BeginRound();
			}
			if (base.isAuthority)
			{
				Active.startTime = Run.FixedTimeStamp.now;
			}
			ObjectivePanelController.collectObjectiveSources += this.ReportObjective;
			Util.PlaySound(Active.soundEntryEvent, base.gameObject);
			Util.PlaySound(Active.soundLoopStartEvent, base.gameObject);
		}

		// Token: 0x06002D44 RID: 11588 RVA: 0x000BF1AC File Offset: 0x000BD3AC
		private void ReportObjective(CharacterMaster master, List<ObjectivePanelController.ObjectiveSourceDescriptor> objectivesList)
		{
			objectivesList.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
			{
				source = this.outer,
				master = master,
				objectiveType = typeof(Active.ChargingObjectiveTracker)
			});
		}

		// Token: 0x06002D45 RID: 11589 RVA: 0x000BF1EE File Offset: 0x000BD3EE
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= Active.duration && base.isAuthority)
			{
				this.outer.SetNextState(new Complete());
			}
		}

		// Token: 0x06002D46 RID: 11590 RVA: 0x000BF21C File Offset: 0x000BD41C
		public override void OnExit()
		{
			Util.PlaySound(Active.soundLoopEndEvent, base.gameObject);
			ObjectivePanelController.collectObjectiveSources -= this.ReportObjective;
			this.childLocator.FindChild("ActiveEffect").gameObject.SetActive(false);
			this.childLocator.FindChild("WardOnEffect").gameObject.SetActive(false);
			base.OnExit();
		}

		// Token: 0x06002D47 RID: 11591 RVA: 0x000BF287 File Offset: 0x000BD487
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(Active.startTime);
		}

		// Token: 0x06002D48 RID: 11592 RVA: 0x000BF29B File Offset: 0x000BD49B
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			Active.startTime = reader.ReadFixedTimeStamp();
		}

		// Token: 0x04002983 RID: 10627
		public static float duration;

		// Token: 0x04002984 RID: 10628
		public static string soundEntryEvent;

		// Token: 0x04002985 RID: 10629
		public static string soundLoopStartEvent;

		// Token: 0x04002986 RID: 10630
		public static string soundLoopEndEvent;

		// Token: 0x04002987 RID: 10631
		private static Run.FixedTimeStamp startTime;

		// Token: 0x020007BF RID: 1983
		private class ChargingObjectiveTracker : ObjectivePanelController.ObjectiveTracker
		{
			// Token: 0x06002D4A RID: 11594 RVA: 0x000BF2AF File Offset: 0x000BD4AF
			private Active FindState()
			{
				return ((EntityStateMachine)this.sourceDescriptor.source).state as Active;
			}

			// Token: 0x06002D4B RID: 11595 RVA: 0x000BF2CB File Offset: 0x000BD4CB
			private static int CalcStateProgress(Active state)
			{
				return Mathf.FloorToInt(Mathf.Clamp01(Active.startTime.timeSince / Active.duration) * 100f);
			}

			// Token: 0x06002D4C RID: 11596 RVA: 0x000BF2ED File Offset: 0x000BD4ED
			protected override bool IsDirty()
			{
				return Active.ChargingObjectiveTracker.CalcStateProgress(this.FindState()) != this.lastValue;
			}

			// Token: 0x06002D4D RID: 11597 RVA: 0x000BF308 File Offset: 0x000BD508
			protected override string GenerateString()
			{
				int num = Active.ChargingObjectiveTracker.CalcStateProgress(this.FindState());
				this.lastValue = num;
				return Language.GetStringFormatted("OBJECTIVE_ARENA_CHARGE_CELL", new object[]
				{
					num
				});
			}

			// Token: 0x04002988 RID: 10632
			private int lastValue = -1;
		}
	}
}
