using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200064D RID: 1613
	public class ViewableTrigger : MonoBehaviour
	{
		// Token: 0x060025F9 RID: 9721 RVA: 0x000A5264 File Offset: 0x000A3464
		private void OnEnable()
		{
			ViewableTrigger.TriggerView(this.viewableName);
		}

		// Token: 0x060025FA RID: 9722 RVA: 0x000A5271 File Offset: 0x000A3471
		public static void TriggerView(string viewableName)
		{
			if (string.IsNullOrEmpty(viewableName))
			{
				return;
			}
			LocalUserManager.readOnlyLocalUsersList[0].userProfile.MarkViewableAsViewed(viewableName);
		}

		// Token: 0x040023B7 RID: 9143
		[Tooltip("The name of the viewable to mark as viewed when this component becomes enabled.")]
		public string viewableName;
	}
}
