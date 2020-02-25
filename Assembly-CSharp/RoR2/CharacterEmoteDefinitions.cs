using System;
using EntityStates;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200018D RID: 397
	public class CharacterEmoteDefinitions : MonoBehaviour
	{
		// Token: 0x0600080F RID: 2063 RVA: 0x000230F0 File Offset: 0x000212F0
		public int FindEmoteIndex(string name)
		{
			for (int i = 0; i < this.emoteDefinitions.Length; i++)
			{
				if (this.emoteDefinitions[i].name == name)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x04000888 RID: 2184
		public CharacterEmoteDefinitions.EmoteDef[] emoteDefinitions;

		// Token: 0x0200018E RID: 398
		[Serializable]
		public struct EmoteDef
		{
			// Token: 0x04000889 RID: 2185
			public string name;

			// Token: 0x0400088A RID: 2186
			public string displayName;

			// Token: 0x0400088B RID: 2187
			public EntityStateMachine targetStateMachine;

			// Token: 0x0400088C RID: 2188
			public SerializableEntityStateType state;
		}
	}
}
