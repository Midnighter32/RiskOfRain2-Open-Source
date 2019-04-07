using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000658 RID: 1624
	public class ViewableTrigger : MonoBehaviour
	{
		// Token: 0x06002455 RID: 9301 RVA: 0x000AA728 File Offset: 0x000A8928
		private void OnEnable()
		{
			ViewableTrigger.TriggerView(this.viewableName);
		}

		// Token: 0x06002456 RID: 9302 RVA: 0x000AA735 File Offset: 0x000A8935
		public static void TriggerView(string viewableName)
		{
			if (string.IsNullOrEmpty(viewableName))
			{
				return;
			}
			LocalUserManager.readOnlyLocalUsersList[0].userProfile.MarkViewableAsViewed(viewableName);
		}

		// Token: 0x04002750 RID: 10064
		[Tooltip("The name of the viewable to mark as viewed when this component becomes enabled.")]
		public string viewableName;
	}
}
