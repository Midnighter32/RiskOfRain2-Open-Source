using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200057E RID: 1406
	public class AchievementNotificationPanel : MonoBehaviour
	{
		// Token: 0x06002178 RID: 8568 RVA: 0x00091031 File Offset: 0x0008F231
		private void Awake()
		{
			AchievementNotificationPanel.instancesList.Add(this);
			this.onStart.Invoke();
		}

		// Token: 0x06002179 RID: 8569 RVA: 0x00091049 File Offset: 0x0008F249
		private void OnDestroy()
		{
			AchievementNotificationPanel.instancesList.Remove(this);
		}

		// Token: 0x0600217A RID: 8570 RVA: 0x0000409B File Offset: 0x0000229B
		private void Update()
		{
		}

		// Token: 0x0600217B RID: 8571 RVA: 0x00091057 File Offset: 0x0008F257
		public void SetAchievementDef(AchievementDef achievementDef)
		{
			this.achievementIconImage.sprite = achievementDef.GetAchievedIcon();
			this.achievementName.text = Language.GetString(achievementDef.nameToken);
			this.achievementDescription.text = Language.GetString(achievementDef.descriptionToken);
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x00091096 File Offset: 0x0008F296
		private static Canvas GetUserCanvas(LocalUser localUser)
		{
			if (Run.instance)
			{
				return localUser.cameraRigController.hud.GetComponent<Canvas>();
			}
			return RoR2Application.instance.mainCanvas;
		}

		// Token: 0x0600217D RID: 8573 RVA: 0x000910BF File Offset: 0x0008F2BF
		private static bool IsAppropriateTimeToDisplayUserAchievementNotification(LocalUser localUser)
		{
			return !GameOverController.instance;
		}

		// Token: 0x0600217E RID: 8574 RVA: 0x000910CE File Offset: 0x0008F2CE
		private static void DispatchAchievementNotification(Canvas canvas, AchievementDef achievementDef)
		{
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/AchievementNotificationPanel"), canvas.transform).GetComponent<AchievementNotificationPanel>().SetAchievementDef(achievementDef);
			Util.PlaySound(achievementDef.GetAchievementSoundString(), RoR2Application.instance.gameObject);
		}

		// Token: 0x0600217F RID: 8575 RVA: 0x00091106 File Offset: 0x0008F306
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onFixedUpdate += AchievementNotificationPanel.StaticFixedUpdate;
		}

		// Token: 0x06002180 RID: 8576 RVA: 0x0009111C File Offset: 0x0008F31C
		private static void StaticFixedUpdate()
		{
			foreach (LocalUser localUser in LocalUserManager.readOnlyLocalUsersList)
			{
				if (localUser != null && localUser.userProfile.hasUnviewedAchievement)
				{
					Canvas canvas = AchievementNotificationPanel.GetUserCanvas(localUser);
					if (!AchievementNotificationPanel.instancesList.Any((AchievementNotificationPanel instance) => instance.transform.parent == canvas.transform) && AchievementNotificationPanel.IsAppropriateTimeToDisplayUserAchievementNotification(localUser))
					{
						string text = (localUser != null) ? localUser.userProfile.PopNextUnviewedAchievementName() : null;
						if (text != null)
						{
							AchievementDef achievementDef = AchievementManager.GetAchievementDef(text);
							if (achievementDef != null)
							{
								AchievementNotificationPanel.DispatchAchievementNotification(AchievementNotificationPanel.GetUserCanvas(localUser), achievementDef);
							}
						}
					}
				}
			}
		}

		// Token: 0x04001EEB RID: 7915
		private static readonly List<AchievementNotificationPanel> instancesList = new List<AchievementNotificationPanel>();

		// Token: 0x04001EEC RID: 7916
		public Image achievementIconImage;

		// Token: 0x04001EED RID: 7917
		public TextMeshProUGUI achievementName;

		// Token: 0x04001EEE RID: 7918
		public TextMeshProUGUI achievementDescription;

		// Token: 0x04001EEF RID: 7919
		public UnityEvent onStart;
	}
}
