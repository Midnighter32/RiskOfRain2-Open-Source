using System;
using EntityStates;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005ED RID: 1517
	[RequireComponent(typeof(HUD))]
	public class HUDTutorialController : MonoBehaviour
	{
		// Token: 0x060021FD RID: 8701 RVA: 0x000A0BA0 File Offset: 0x0009EDA0
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

		// Token: 0x060021FE RID: 8702 RVA: 0x000A0C04 File Offset: 0x0009EE04
		private UserProfile GetUserProfile()
		{
			return this.hud.localUserViewer.userProfile;
		}

		// Token: 0x060021FF RID: 8703 RVA: 0x000A0C16 File Offset: 0x0009EE16
		private void HandleTutorial(GameObject tutorialPopup, ref UserProfile.TutorialProgression tutorialProgression, bool dismiss = false, bool progress = true)
		{
			if (tutorialPopup && !dismiss)
			{
				tutorialPopup.SetActive(true);
			}
			tutorialProgression.shouldShow = false;
			if (progress)
			{
				tutorialProgression.showCount += 1u;
			}
		}

		// Token: 0x06002200 RID: 8704 RVA: 0x000A0C40 File Offset: 0x0009EE40
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
					userProfile2.tutorialSprint.showCount = userProfile2.tutorialSprint.showCount + 1u;
				}
			}
		}

		// Token: 0x040024FF RID: 9471
		private HUD hud;

		// Token: 0x04002500 RID: 9472
		[Tooltip("The tutorial popup object.")]
		[Header("Equipment Tutorial")]
		public GameObject equipmentTutorialObject;

		// Token: 0x04002501 RID: 9473
		[Tooltip("The equipment icon to monitor for a change to trigger the tutorial popup.")]
		public EquipmentIcon equipmentIcon;

		// Token: 0x04002502 RID: 9474
		[Header("Difficulty Tutorial")]
		[Tooltip("The tutorial popup object.")]
		public GameObject difficultyTutorialObject;

		// Token: 0x04002503 RID: 9475
		[Tooltip("The time at which to trigger the tutorial popup.")]
		public float difficultyTutorialTriggerTime = 60f;

		// Token: 0x04002504 RID: 9476
		[Header("Sprint Tutorial")]
		[Tooltip("The tutorial popup object.")]
		public GameObject sprintTutorialObject;

		// Token: 0x04002505 RID: 9477
		[Tooltip("How long to wait for the player to sprint before showing the tutorial popup.")]
		public float sprintTutorialTriggerTime = 30f;

		// Token: 0x04002506 RID: 9478
		private float sprintTutorialStopwatch;
	}
}
