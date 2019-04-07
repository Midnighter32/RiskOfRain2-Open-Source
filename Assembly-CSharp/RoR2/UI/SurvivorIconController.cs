using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x02000648 RID: 1608
	[RequireComponent(typeof(MPButton))]
	public class SurvivorIconController : MonoBehaviour
	{
		// Token: 0x060023F8 RID: 9208 RVA: 0x000A8EA6 File Offset: 0x000A70A6
		private void Awake()
		{
			this.button = base.GetComponent<MPButton>();
		}

		// Token: 0x060023F9 RID: 9209 RVA: 0x000A8EB4 File Offset: 0x000A70B4
		public void PushSurvivorIndexToCharacterSelect(CharacterSelectController chararcterSelectController)
		{
			if (!PreGameController.instance || !PreGameController.instance.IsCharacterSwitchingCurrentlyAllowed())
			{
				return;
			}
			chararcterSelectController.SelectSurvivor(this.survivorIndex);
			LocalUser localUser = ((MPEventSystem)EventSystem.current).localUser;
			if (localUser.eventSystem == EventSystem.current)
			{
				NetworkUser currentNetworkUser = localUser.currentNetworkUser;
				if (currentNetworkUser == null)
				{
					return;
				}
				currentNetworkUser.CallCmdSetBodyPreference(BodyCatalog.FindBodyIndex(SurvivorCatalog.GetSurvivorDef(this.survivorIndex).bodyPrefab));
			}
		}

		// Token: 0x060023FA RID: 9210 RVA: 0x000A8F30 File Offset: 0x000A7130
		private void Update()
		{
			if (this.shouldRebuild)
			{
				this.shouldRebuild = false;
				this.Rebuild();
			}
			if (this.characterSelectController)
			{
				this.isSelected = false;
				bool flag = SurvivorIconController.SurvivorIsUnlockedOnThisClient(this.survivorIndex);
				this.button.interactable = flag;
				Color color = Color.gray;
				if (!flag)
				{
					color = Color.black;
				}
				if (this.characterSelectController.selectedSurvivorIndex == this.survivorIndex)
				{
					color = Color.white;
					this.isSelected = true;
				}
				this.survivorIcon.color = color;
			}
			this.survivorIsSelectedEffect.SetActive(this.isSelected);
		}

		// Token: 0x060023FB RID: 9211 RVA: 0x000A8FCC File Offset: 0x000A71CC
		private void Rebuild()
		{
			SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(this.survivorIndex);
			if (survivorDef != null)
			{
				GameObject bodyPrefab = survivorDef.bodyPrefab;
				if (bodyPrefab)
				{
					CharacterBody component = bodyPrefab.GetComponent<CharacterBody>();
					if (component)
					{
						if (this.survivorIcon)
						{
							this.survivorIcon.texture = component.portraitIcon;
						}
						string viewableName = string.Format(CultureInfo.InvariantCulture, "/Survivors/{0}", this.survivorIndex.ToString());
						if (this.viewableTag)
						{
							this.viewableTag.viewableName = viewableName;
							this.viewableTag.Refresh();
						}
						if (this.viewableTrigger)
						{
							this.viewableTrigger.viewableName = viewableName;
						}
					}
				}
			}
		}

		// Token: 0x060023FC RID: 9212 RVA: 0x000A9088 File Offset: 0x000A7288
		private static bool SurvivorIsUnlockedOnThisClient(SurvivorIndex survivorIndex)
		{
			return LocalUserManager.readOnlyLocalUsersList.Any((LocalUser localUser) => localUser.userProfile.HasSurvivorUnlocked(survivorIndex));
		}

		// Token: 0x040026E0 RID: 9952
		public CharacterSelectController characterSelectController;

		// Token: 0x040026E1 RID: 9953
		public SurvivorIndex survivorIndex;

		// Token: 0x040026E2 RID: 9954
		public RawImage survivorIcon;

		// Token: 0x040026E3 RID: 9955
		public GameObject survivorIsSelectedEffect;

		// Token: 0x040026E4 RID: 9956
		private bool shouldRebuild = true;

		// Token: 0x040026E5 RID: 9957
		private bool isSelected;

		// Token: 0x040026E6 RID: 9958
		private MPButton button;

		// Token: 0x040026E7 RID: 9959
		public ViewableTag viewableTag;

		// Token: 0x040026E8 RID: 9960
		public ViewableTrigger viewableTrigger;
	}
}
