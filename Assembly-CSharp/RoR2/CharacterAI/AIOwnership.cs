using System;
using UnityEngine;

namespace RoR2.CharacterAI
{
	// Token: 0x02000568 RID: 1384
	[RequireComponent(typeof(BaseAI))]
	[DisallowMultipleComponent]
	public class AIOwnership : MonoBehaviour
	{
		// Token: 0x06002100 RID: 8448 RVA: 0x0008E9C2 File Offset: 0x0008CBC2
		private void Awake()
		{
			this.baseAI = base.GetComponent<BaseAI>();
		}

		// Token: 0x06002101 RID: 8449 RVA: 0x0008E9D0 File Offset: 0x0008CBD0
		private void FixedUpdate()
		{
			if (this.ownerMaster)
			{
				this.baseAI.leader.gameObject = this.ownerMaster.GetBodyObject();
			}
		}

		// Token: 0x04001E27 RID: 7719
		public CharacterMaster ownerMaster;

		// Token: 0x04001E28 RID: 7720
		private BaseAI baseAI;
	}
}
