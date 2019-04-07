using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F4 RID: 1012
	[DisallowMultipleComponent]
	public class TeamComponent : NetworkBehaviour, ILifeBehavior
	{
		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06001629 RID: 5673 RVA: 0x0006A1E5 File Offset: 0x000683E5
		// (set) Token: 0x06001628 RID: 5672 RVA: 0x0006A1BD File Offset: 0x000683BD
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
				base.SetDirtyBit(1u);
				if (Application.isPlaying)
				{
					this.OnChangeTeam(value);
				}
			}
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x0006A1ED File Offset: 0x000683ED
		private static bool TeamIsValid(TeamIndex teamIndex)
		{
			return teamIndex >= TeamIndex.Neutral && teamIndex < TeamIndex.Count;
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x0006A1F9 File Offset: 0x000683F9
		private void OnChangeTeam(TeamIndex newTeamIndex)
		{
			this.OnLeaveTeam(this.oldTeamIndex);
			this.OnJoinTeam(newTeamIndex);
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x0006A20E File Offset: 0x0006840E
		private void OnLeaveTeam(TeamIndex oldTeamIndex)
		{
			if (TeamComponent.TeamIsValid(oldTeamIndex))
			{
				TeamComponent.teamsList[(int)oldTeamIndex].Remove(this);
			}
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x0006A228 File Offset: 0x00068428
		private void OnJoinTeam(TeamIndex newTeamIndex)
		{
			if (TeamComponent.TeamIsValid(newTeamIndex))
			{
				TeamComponent.teamsList[(int)newTeamIndex].Add(this);
			}
			this.SetupIndicator();
			HurtBoxGroup hurtBoxGroup = this.hurtBoxGroup;
			HurtBox[] array = (hurtBoxGroup != null) ? hurtBoxGroup.hurtBoxes : null;
			if (array != null)
			{
				HurtBox[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].teamIndex = newTeamIndex;
				}
			}
			this.oldTeamIndex = newTeamIndex;
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x0006A288 File Offset: 0x00068488
		private void SetupIndicator()
		{
			if (this.indicator)
			{
				return;
			}
			CharacterBody component = base.GetComponent<CharacterBody>();
			if (component)
			{
				TeamComponent component2 = component.GetComponent<TeamComponent>();
				if (component2)
				{
					CharacterMaster master = component.master;
					bool flag = master && master.isBoss;
					GameObject gameObject = null;
					if (master && component2.teamIndex == TeamIndex.Player)
					{
						bool flag2 = false;
						PlayerCharacterMasterController component3 = master.GetComponent<PlayerCharacterMasterController>();
						if (component3)
						{
							flag2 = true;
							GameObject networkUserObject = component3.networkUserObject;
							if (networkUserObject)
							{
								NetworkIdentity component4 = networkUserObject.GetComponent<NetworkIdentity>();
								if (component4)
								{
									bool isLocalPlayer = component4.isLocalPlayer;
								}
							}
						}
						Vector3 position = component.transform.position;
						component.GetComponent<Collider>();
						if (flag2)
						{
							gameObject = Resources.Load<GameObject>("Prefabs/PositionIndicators/PlayerPositionIndicator");
						}
						else
						{
							gameObject = Resources.Load<GameObject>("Prefabs/PositionIndicators/NPCPositionIndicator");
						}
						this.indicator = UnityEngine.Object.Instantiate<GameObject>(gameObject, position, Quaternion.identity, component.transform);
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
						this.indicator.GetComponent<PositionIndicator>().targetTransform = component.coreTransform;
						Nameplate component5 = this.indicator.GetComponent<Nameplate>();
						if (component5)
						{
							component5.SetBody(component);
						}
					}
				}
			}
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x0006A410 File Offset: 0x00068610
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

		// Token: 0x06001630 RID: 5680 RVA: 0x0006A479 File Offset: 0x00068679
		private void Awake()
		{
			ModelLocator component = base.GetComponent<ModelLocator>();
			HurtBoxGroup hurtBoxGroup;
			if (component == null)
			{
				hurtBoxGroup = null;
			}
			else
			{
				Transform modelTransform = component.modelTransform;
				hurtBoxGroup = ((modelTransform != null) ? modelTransform.GetComponent<HurtBoxGroup>() : null);
			}
			this.hurtBoxGroup = hurtBoxGroup;
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x0006A49F File Offset: 0x0006869F
		public void Start()
		{
			this.SetupIndicator();
			if (this.oldTeamIndex != this.teamIndex)
			{
				this.OnChangeTeam(this.teamIndex);
			}
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x0006A4C1 File Offset: 0x000686C1
		private void OnDestroy()
		{
			this.teamIndex = TeamIndex.None;
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x0003F5D8 File Offset: 0x0003D7D8
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x0006A4CA File Offset: 0x000686CA
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(this.teamIndex);
			return initialState || base.syncVarDirtyBits > 0u;
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x0006A4E6 File Offset: 0x000686E6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			this.teamIndex = reader.ReadTeamIndex();
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x0006A4F4 File Offset: 0x000686F4
		public static ReadOnlyCollection<TeamComponent> GetTeamMembers(TeamIndex teamIndex)
		{
			if (!TeamComponent.TeamIsValid(teamIndex))
			{
				return TeamComponent.emptyTeamMembers;
			}
			return TeamComponent.readonlyTeamsList[(int)teamIndex];
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x0006A50C File Offset: 0x0006870C
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
			return TeamIndex.Neutral;
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x0400198D RID: 6541
		[SerializeField]
		private TeamIndex _teamIndex = TeamIndex.None;

		// Token: 0x0400198E RID: 6542
		private TeamIndex oldTeamIndex = TeamIndex.None;

		// Token: 0x0400198F RID: 6543
		private GameObject indicator;

		// Token: 0x04001990 RID: 6544
		private static List<TeamComponent>[] teamsList;

		// Token: 0x04001991 RID: 6545
		private static ReadOnlyCollection<TeamComponent>[] readonlyTeamsList;

		// Token: 0x04001992 RID: 6546
		private HurtBoxGroup hurtBoxGroup;

		// Token: 0x04001993 RID: 6547
		private static ReadOnlyCollection<TeamComponent> emptyTeamMembers = new List<TeamComponent>().AsReadOnly();
	}
}
