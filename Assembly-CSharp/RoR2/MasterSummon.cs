using System;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003CF RID: 975
	public class MasterSummon
	{
		// Token: 0x060017BB RID: 6075 RVA: 0x00066F64 File Offset: 0x00065164
		public CharacterMaster Perform()
		{
			TeamIndex teamIndex;
			if (this.teamIndexOverride != null)
			{
				teamIndex = this.teamIndexOverride.Value;
			}
			else
			{
				if (!this.summonerBodyObject)
				{
					Debug.LogErrorFormat("Cannot spawn master {0}: No team specified.", new object[]
					{
						this.masterPrefab
					});
					return null;
				}
				teamIndex = TeamComponent.GetObjectTeam(this.summonerBodyObject);
			}
			if (!this.ignoreTeamMemberLimit)
			{
				TeamDef teamDef = TeamCatalog.GetTeamDef(teamIndex);
				if (teamDef == null)
				{
					Debug.LogErrorFormat("Attempting to spawn master {0} on TeamIndex.None. Is this intentional?", new object[]
					{
						this.masterPrefab
					});
					return null;
				}
				if (teamDef != null && teamDef.softCharacterLimit <= TeamComponent.GetTeamMembers(teamIndex).Count)
				{
					return null;
				}
			}
			CharacterBody characterBody = null;
			CharacterMaster characterMaster = null;
			if (this.summonerBodyObject)
			{
				characterBody = this.summonerBodyObject.GetComponent<CharacterBody>();
			}
			if (characterBody)
			{
				characterMaster = characterBody.master;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.masterPrefab, this.position, this.rotation);
			CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
			component.teamIndex = teamIndex;
			if (this.loadout != null)
			{
				component.SetLoadoutServer(this.loadout);
			}
			CharacterMaster characterMaster2 = characterMaster;
			if (characterMaster2 && characterMaster2.minionOwnership.ownerMaster)
			{
				characterMaster2 = characterMaster2.minionOwnership.ownerMaster;
			}
			component.minionOwnership.SetOwner(characterMaster2);
			if (this.summonerBodyObject)
			{
				AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
				if (component2)
				{
					if (characterMaster)
					{
						component2.ownerMaster = characterMaster;
					}
					CharacterBody component3 = this.summonerBodyObject.GetComponent<CharacterBody>();
					if (component3)
					{
						CharacterMaster master = component3.master;
						if (master)
						{
							component2.ownerMaster = master;
						}
					}
				}
				BaseAI component4 = gameObject.GetComponent<BaseAI>();
				if (component4)
				{
					component4.leader.gameObject = this.summonerBodyObject;
				}
			}
			Action<CharacterMaster> action = this.preSpawnSetupCallback;
			if (action != null)
			{
				action(component);
			}
			NetworkServer.Spawn(gameObject);
			component.Respawn(this.position, this.rotation, false);
			return component;
		}

		// Token: 0x04001658 RID: 5720
		public GameObject masterPrefab;

		// Token: 0x04001659 RID: 5721
		public Vector3 position;

		// Token: 0x0400165A RID: 5722
		public Quaternion rotation;

		// Token: 0x0400165B RID: 5723
		public GameObject summonerBodyObject;

		// Token: 0x0400165C RID: 5724
		public TeamIndex? teamIndexOverride;

		// Token: 0x0400165D RID: 5725
		public bool ignoreTeamMemberLimit;

		// Token: 0x0400165E RID: 5726
		public Action<CharacterMaster> preSpawnSetupCallback;

		// Token: 0x0400165F RID: 5727
		public Loadout loadout;
	}
}
