using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005AE RID: 1454
	public class AllyCardManager : MonoBehaviour
	{
		// Token: 0x06002098 RID: 8344 RVA: 0x000996CB File Offset: 0x000978CB
		private void Awake()
		{
			this.cardAllocator = new UIElementAllocator<AllyCardController>((RectTransform)base.transform, Resources.Load<GameObject>("Prefabs/UI/AllyCard"));
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x000996F0 File Offset: 0x000978F0
		private void Update()
		{
			TeamIndex teamIndex = TeamIndex.None;
			TeamComponent teamComponent = null;
			if (this.sourceGameObject)
			{
				teamComponent = this.sourceGameObject.GetComponent<TeamComponent>();
				if (teamComponent)
				{
					teamIndex = teamComponent.teamIndex;
				}
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
			int num = teamMembers.Count;
			if (teamComponent && teamMembers.Contains(teamComponent))
			{
				num--;
			}
			this.cardAllocator.AllocateElements(num);
			int i = 0;
			int num2 = 0;
			while (i < teamMembers.Count)
			{
				GameObject gameObject = teamMembers[i].gameObject;
				if (gameObject != this.sourceGameObject)
				{
					this.cardAllocator.elements[num2++].sourceGameObject = gameObject;
				}
				i++;
			}
		}

		// Token: 0x04002322 RID: 8994
		public GameObject sourceGameObject;

		// Token: 0x04002323 RID: 8995
		private UIElementAllocator<AllyCardController> cardAllocator;
	}
}
