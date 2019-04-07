using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034F RID: 847
	public class MageLastElementTracker : MonoBehaviour
	{
		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06001185 RID: 4485 RVA: 0x00056F61 File Offset: 0x00055161
		// (set) Token: 0x06001186 RID: 4486 RVA: 0x00056F69 File Offset: 0x00055169
		[HideInInspector]
		public MageElement mageElement { get; private set; }

		// Token: 0x06001187 RID: 4487 RVA: 0x00056F72 File Offset: 0x00055172
		private void Start()
		{
			this.ApplyElement(MageElement.Fire);
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x00056F7C File Offset: 0x0005517C
		public void ApplyElement(MageElement element)
		{
			this.mageElement = element;
			int num = (int)(element - MageElement.Fire);
			this.targetSkill.skillNameToken = this.elementSkillInfos[num].nameToken;
			this.targetSkill.skillDescriptionToken = this.elementSkillInfos[num].descriptionToken;
			this.targetSkill.icon = this.elementSkillInfos[num].sprite;
		}

		// Token: 0x0400158D RID: 5517
		public GenericSkill targetSkill;

		// Token: 0x0400158E RID: 5518
		public MageLastElementTracker.ElementSkillInfo[] elementSkillInfos;

		// Token: 0x02000350 RID: 848
		[Serializable]
		public struct ElementSkillInfo
		{
			// Token: 0x0400158F RID: 5519
			public string nameToken;

			// Token: 0x04001590 RID: 5520
			public string descriptionToken;

			// Token: 0x04001591 RID: 5521
			public Sprite sprite;
		}
	}
}
