using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200063D RID: 1597
	[RequireComponent(typeof(MPButton))]
	public class SurvivorIconController : MonoBehaviour
	{
		// Token: 0x06002597 RID: 9623 RVA: 0x000A38B6 File Offset: 0x000A1AB6
		private void Awake()
		{
			this.button = base.GetComponent<MPButton>();
		}

		// Token: 0x06002598 RID: 9624 RVA: 0x000A38C4 File Offset: 0x000A1AC4
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

		// Token: 0x06002599 RID: 9625 RVA: 0x000A3940 File Offset: 0x000A1B40
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
					this.tooltipProvider.enabled = true;
				}
				else
				{
					this.tooltipProvider.enabled = false;
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

		// Token: 0x0600259A RID: 9626 RVA: 0x000A39F4 File Offset: 0x000A1BF4
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
						if (this.loadoutViewableTag)
						{
							this.loadoutViewableTag.viewableName = string.Format(CultureInfo.InvariantCulture, "/Loadout/Bodies/{0}/", BodyCatalog.GetBodyName(SurvivorCatalog.GetBodyIndexFromSurvivorIndex(this.survivorIndex)));
							this.loadoutViewableTag.Refresh();
						}
						if (this.viewableTrigger)
						{
							this.viewableTrigger.viewableName = viewableName;
						}
						if (this.tooltipProvider)
						{
							UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(survivorDef.unlockableName);
							if (unlockableDef != null)
							{
								this.tooltipProvider.titleToken = "UNIDENTIFIED";
								this.tooltipProvider.titleColor = Color.gray;
								this.tooltipProvider.overrideBodyText = unlockableDef.getHowToUnlockString();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600259B RID: 9627 RVA: 0x000A3B4C File Offset: 0x000A1D4C
		private static bool SurvivorIsUnlockedOnThisClient(SurvivorIndex survivorIndex)
		{
			return LocalUserManager.readOnlyLocalUsersList.Any((LocalUser localUser) => localUser.userProfile.HasSurvivorUnlocked(survivorIndex));
		}

		// Token: 0x04002343 RID: 9027
		public CharacterSelectController characterSelectController;

		// Token: 0x04002344 RID: 9028
		public SurvivorIndex survivorIndex;

		// Token: 0x04002345 RID: 9029
		public RawImage survivorIcon;

		// Token: 0x04002346 RID: 9030
		public GameObject survivorIsSelectedEffect;

		// Token: 0x04002347 RID: 9031
		private bool shouldRebuild = true;

		// Token: 0x04002348 RID: 9032
		private bool isSelected;

		// Token: 0x04002349 RID: 9033
		private MPButton button;

		// Token: 0x0400234A RID: 9034
		public ViewableTag viewableTag;

		// Token: 0x0400234B RID: 9035
		public ViewableTag loadoutViewableTag;

		// Token: 0x0400234C RID: 9036
		public ViewableTrigger viewableTrigger;

		// Token: 0x0400234D RID: 9037
		public TooltipProvider tooltipProvider;
	}
}
