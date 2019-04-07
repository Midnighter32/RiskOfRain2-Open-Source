using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005AB RID: 1451
	public class AchievementNotificationPanel : MonoBehaviour
	{
		// Token: 0x06002088 RID: 8328 RVA: 0x00099479 File Offset: 0x00097679
		private void Awake()
		{
			AchievementNotificationPanel.instancesList.Add(this);
			this.onStart.Invoke();
		}

		// Token: 0x06002089 RID: 8329 RVA: 0x00099491 File Offset: 0x00097691
		private void OnDestroy()
		{
			AchievementNotificationPanel.instancesList.Remove(this);
		}

		// Token: 0x0600208A RID: 8330 RVA: 0x00004507 File Offset: 0x00002707
		private void Update()
		{
		}

		// Token: 0x0600208B RID: 8331 RVA: 0x0009949F File Offset: 0x0009769F
		public void SetAchievementDef(AchievementDef achievementDef)
		{
			this.achievementIconImage.sprite = achievementDef.GetAchievedIcon();
			this.achievementName.text = Language.GetString(achievementDef.nameToken);
			this.achievementDescription.text = Language.GetString(achievementDef.descriptionToken);
		}

		// Token: 0x0600208C RID: 8332 RVA: 0x000994DE File Offset: 0x000976DE
		private static Canvas GetUserCanvas(LocalUser localUser)
		{
			if (Run.instance)
			{
				return localUser.cameraRigController.hud.GetComponent<Canvas>();
			}
			return RoR2Application.instance.mainCanvas;
		}

		// Token: 0x0600208D RID: 8333 RVA: 0x00099507 File Offset: 0x00097707
		private static bool IsAppropriateTimeToDisplayUserAchievementNotification(LocalUser localUser)
		{
			return !GameOverController.instance;
		}

		// Token: 0x0600208E RID: 8334 RVA: 0x00099516 File Offset: 0x00097716
		private static void DispatchAchievementNotification(Canvas canvas, AchievementDef achievementDef)
		{
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/AchievementNotificationPanel"), canvas.transform).GetComponent<AchievementNotificationPanel>().SetAchievementDef(achievementDef);
			Util.PlaySound(achievementDef.GetAchievementSoundString(), RoR2Application.instance.gameObject);
		}

		// Token: 0x0600208F RID: 8335 RVA: 0x0009954E File Offset: 0x0009774E
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onFixedUpdate += AchievementNotificationPanel.StaticFixedUpdate;
		}

		// Token: 0x06002090 RID: 8336 RVA: 0x00099564 File Offset: 0x00097764
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

		// Token: 0x04002318 RID: 8984
		private static readonly List<AchievementNotificationPanel> instancesList = new List<AchievementNotificationPanel>();

		// Token: 0x04002319 RID: 8985
		public Image achievementIconImage;

		// Token: 0x0400231A RID: 8986
		public TextMeshProUGUI achievementName;

		// Token: 0x0400231B RID: 8987
		public TextMeshProUGUI achievementDescription;

		// Token: 0x0400231C RID: 8988
		public UnityEvent onStart;
	}
}
