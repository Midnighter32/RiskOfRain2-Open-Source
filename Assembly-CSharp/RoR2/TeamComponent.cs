using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200034B RID: 843
	[DisallowMultipleComponent]
	public class TeamComponent : NetworkBehaviour, ILifeBehavior
	{
		// Token: 0x17000263 RID: 611
		// (get) Token: 0x0600141D RID: 5149 RVA: 0x00056119 File Offset: 0x00054319
		// (set) Token: 0x0600141E RID: 5150 RVA: 0x00056121 File Offset: 0x00054321
		public CharacterBody body { get; private set; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06001420 RID: 5152 RVA: 0x00056152 File Offset: 0x00054352
		// (set) Token: 0x0600141F RID: 5151 RVA: 0x0005612A File Offset: 0x0005432A
		public TeamIndex teamIndex
		{
			get
			{
				return this._teamIndex;
			}
			set
			{
				if (this._teamIndex == value)
				{
					return;
				}
				this._teamIndex = value;
				if (Application.isPlaying)
				{
					base.SetDirtyBit(1U);
					this.OnChangeTeam(value);
				}
			}
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x0005615A File Offset: 0x0005435A
		private static bool TeamIsValid(TeamIndex teamIndex)
		{
			return teamIndex >= TeamIndex.Neutral && teamIndex < TeamIndex.Count;
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x00056166 File Offset: 0x00054366
		private void OnChangeTeam(TeamIndex newTeamIndex)
		{
			this.OnLeaveTeam(this.oldTeamIndex);
			this.OnJoinTeam(newTeamIndex);
		}

		// Token: 0x06001423 RID: 5155 RVA: 0x0005617B File Offset: 0x0005437B
		private void OnLeaveTeam(TeamIndex oldTeamIndex)
		{
			if (TeamComponent.TeamIsValid(oldTeamIndex))
			{
				TeamComponent.teamsList[(int)oldTeamIndex].Remove(this);
			}
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x00056194 File Offset: 0x00054394
		private void OnJoinTeam(TeamIndex newTeamIndex)
		{
			if (TeamComponent.TeamIsValid(newTeamIndex))
			{
				TeamComponent.teamsList[(int)newTeamIndex].Add(this);
			}
			this.SetupIndicator();
			HurtBox[] array;
			if (!this.body)
			{
				array = null;
			}
			else
			{
				HurtBoxGroup hurtBoxGroup = this.body.hurtBoxGroup;
				array = ((hurtBoxGroup != null) ? hurtBoxGroup.hurtBoxes : null);
			}
			HurtBox[] array2 = array;
			if (array2 != null)
			{
				HurtBox[] array3 = array2;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].teamIndex = newTeamIndex;
				}
			}
			this.oldTeamIndex = newTeamIndex;
		}

		// Token: 0x06001425 RID: 5157 RVA: 0x00056208 File Offset: 0x00054408
		private void SetupIndicator()
		{
			if (this.indicator)
			{
				return;
			}
			if (this.body)
			{
				CharacterMaster master = this.body.master;
				bool flag = master && master.isBoss;
				GameObject gameObject = null;
				if (master && this.teamIndex == TeamIndex.Player)
				{
					gameObject = Resources.Load<GameObject>(this.body.isPlayerControlled ? "Prefabs/PositionIndicators/PlayerPositionIndicator" : "Prefabs/PositionIndicators/NPCPositionIndicator");
				}
				else if (flag)
				{
					gameObject = Resources.Load<GameObject>("Prefabs/PositionIndicators/BossPositionIndicator");
				}
				if (this.indicator)
				{
					UnityEngine.Object.Destroy(this.indicator);
					this.indicator = null;
				}
				if (gameObject)
				{
					this.indicator = UnityEngine.Object.Instantiate<GameObject>(gameObject, base.transform);
					this.indicator.GetComponent<PositionIndicator>().targetTransform = this.body.coreTransform;
					Nameplate component = this.indicator.GetComponent<Nameplate>();
					if (component)
					{
						component.SetBody(this.body);
					}
				}
			}
		}

		// Token: 0x06001426 RID: 5158 RVA: 0x00056308 File Offset: 0x00054508
		static TeamComponent()
		{
			TeamComponent.teamsList = new List<TeamComponent>[3];
			TeamComponent.readonlyTeamsList = new ReadOnlyCollection<TeamComponent>[TeamComponent.teamsList.Length];
			for (int i = 0; i < TeamComponent.teamsList.Length; i++)
			{
				TeamComponent.teamsList[i] = new List<TeamComponent>();
				TeamComponent.readonlyTeamsList[i] = TeamComponent.teamsList[i].AsReadOnly();
			}
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x00056371 File Offset: 0x00054571
		private void Awake()
		{
			this.body = base.GetComponent<CharacterBody>();
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0005637F File Offset: 0x0005457F
		public void Start()
		{
			this.SetupIndicator();
			if (this.oldTeamIndex != this.teamIndex)
			{
				this.OnChangeTeam(this.teamIndex);
			}
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x000563A1 File Offset: 0x000545A1
		private void OnDestroy()
		{
			this.teamIndex = TeamIndex.None;
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x00022B74 File Offset: 0x00020D74
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x000563AA File Offset: 0x000545AA
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(this.teamIndex);
			return initialState || base.syncVarDirtyBits > 0U;
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x000563C6 File Offset: 0x000545C6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			this.teamIndex = reader.ReadTeamIndex();
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x000563D4 File Offset: 0x000545D4
		public static ReadOnlyCollection<TeamComponent> GetTeamMembers(TeamIndex teamIndex)
		{
			if (!TeamComponent.TeamIsValid(teamIndex))
			{
				return TeamComponent.emptyTeamMembers;
			}
			return TeamComponent.readonlyTeamsList[(int)teamIndex];
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x000563EC File Offset: 0x000545EC
		public static TeamIndex GetObjectTeam(GameObject gameObject)
		{
			if (gameObject)
			{
				TeamComponent component = gameObject.GetComponent<TeamComponent>();
				if (component)
				{
					return component.teamIndex;
				}
			}
			return TeamIndex.None;
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x040012EB RID: 4843
		public bool hideAllyCardDisplay;

		// Token: 0x040012EC RID: 4844
		[SerializeField]
		private TeamIndex _teamIndex = TeamIndex.None;

		// Token: 0x040012EE RID: 4846
		private TeamIndex oldTeamIndex = TeamIndex.None;

		// Token: 0x040012EF RID: 4847
		private GameObject indicator;

		// Token: 0x040012F0 RID: 4848
		private static List<TeamComponent>[] teamsList;

		// Token: 0x040012F1 RID: 4849
		private static ReadOnlyCollection<TeamComponent>[] readonlyTeamsList;

		// Token: 0x040012F2 RID: 4850
		private static ReadOnlyCollection<TeamComponent> emptyTeamMembers = new List<TeamComponent>().AsReadOnly();
	}
}
