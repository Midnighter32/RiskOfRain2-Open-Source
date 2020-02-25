using System;
using EntityStates;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005D2 RID: 1490
	[RequireComponent(typeof(HUD))]
	public class HUDTutorialController : MonoBehaviour
	{
		// Token: 0x06002340 RID: 9024 RVA: 0x00099FF8 File Offset: 0x000981F8
		private void Awake()
		{
			this.hud = base.GetComponent<HUD>();
			if (this.equipmentTutorialObject)
			{
				this.equipmentTutorialObject.SetActive(false);
			}
			if (this.difficultyTutorialObject)
			{
				this.difficultyTutorialObject.SetActive(false);
			}
			if (this.sprintTutorialObject)
			{
				this.sprintTutorialObject.SetActive(false);
			}
		}

		// Token: 0x06002341 RID: 9025 RVA: 0x0009A05C File Offset: 0x0009825C
		private UserProfile GetUserProfile()
		{
			return this.hud.localUserViewer.userProfile;
		}

		// Token: 0x06002342 RID: 9026 RVA: 0x0009A06E File Offset: 0x0009826E
		private void HandleTutorial(GameObject tutorialPopup, ref UserProfile.TutorialProgression tutorialProgression, bool dismiss = false, bool progress = true)
		{
			if (tutorialPopup && !dismiss)
			{
				tutorialPopup.SetActive(true);
			}
			tutorialProgression.shouldShow = false;
			if (progress)
			{
				tutorialProgression.showCount += 1U;
			}
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x0009A098 File Offset: 0x00098298
		private void Update()
		{
			UserProfile userProfile = this.GetUserProfile();
			CharacterBody cachedBody = this.hud.localUserViewer.cachedBody;
			if (userProfile != null && cachedBody)
			{
				if (userProfile.tutorialEquipment.shouldShow && this.equipmentIcon.hasEquipment)
				{
					this.HandleTutorial(this.equipmentTutorialObject, ref userProfile.tutorialEquipment, false, true);
				}
				if (userProfile.tutorialDifficulty.shouldShow && Run.instance && Run.instance.fixedTime >= this.difficultyTutorialTriggerTime)
				{
					this.HandleTutorial(this.difficultyTutorialObject, ref userProfile.tutorialDifficulty, false, true);
				}
				if (userProfile.tutorialSprint.shouldShow)
				{
					if (cachedBody.isSprinting)
					{
						this.HandleTutorial(null, ref userProfile.tutorialSprint, true, true);
						return;
					}
					EntityStateMachine component = cachedBody.GetComponent<EntityStateMachine>();
					if (((component != null) ? component.state : null) is GenericCharacterMain)
					{
						this.sprintTutorialStopwatch += Time.deltaTime;
					}
					if (this.sprintTutorialStopwatch >= this.sprintTutorialTriggerTime)
					{
						this.HandleTutorial(this.sprintTutorialObject, ref userProfile.tutorialSprint, false, false);
						return;
					}
				}
				else if (this.sprintTutorialObject && this.sprintTutorialObject.activeInHierarchy && cachedBody.isSprinting)
				{
					UnityEngine.Object.Destroy(this.sprintTutorialObject);
					this.sprintTutorialObject = null;
					UserProfile userProfile2 = userProfile;
					userProfile2.tutorialSprint.showCount = userProfile2.tutorialSprint.showCount + 1U;
				}
			}
		}

		// Token: 0x04002121 RID: 8481
		private HUD hud;

		// Token: 0x04002122 RID: 8482
		[Tooltip("The tutorial popup object.")]
		[Header("Equipment Tutorial")]
		public GameObject equipmentTutorialObject;

		// Token: 0x04002123 RID: 8483
		[Tooltip("The equipment icon to monitor for a change to trigger the tutorial popup.")]
		public EquipmentIcon equipmentIcon;

		// Token: 0x04002124 RID: 8484
		[Header("Difficulty Tutorial")]
		[Tooltip("The tutorial popup object.")]
		public GameObject difficultyTutorialObject;

		// Token: 0x04002125 RID: 8485
		[Tooltip("The time at which to trigger the tutorial popup.")]
		public float difficultyTutorialTriggerTime = 60f;

		// Token: 0x04002126 RID: 8486
		[Header("Sprint Tutorial")]
		[Tooltip("The tutorial popup object.")]
		public GameObject sprintTutorialObject;

		// Token: 0x04002127 RID: 8487
		[Tooltip("How long to wait for the player to sprint before showing the tutorial popup.")]
		public float sprintTutorialTriggerTime = 30f;

		// Token: 0x04002128 RID: 8488
		private float sprintTutorialStopwatch;
	}
}
