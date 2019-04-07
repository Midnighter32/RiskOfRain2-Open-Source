using System;
using EntityStates;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200028A RID: 650
	public class CharacterEmoteDefinitions : MonoBehaviour
	{
		// Token: 0x06000CCF RID: 3279 RVA: 0x0003FB54 File Offset: 0x0003DD54
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

		// Token: 0x040010F6 RID: 4342
		public CharacterEmoteDefinitions.EmoteDef[] emoteDefinitions;

		// Token: 0x0200028B RID: 651
		[Serializable]
		public struct EmoteDef
		{
			// Token: 0x040010F7 RID: 4343
			public string name;

			// Token: 0x040010F8 RID: 4344
			public string displayName;

			// Token: 0x040010F9 RID: 4345
			public EntityStateMachine targetStateMachine;

			// Token: 0x040010FA RID: 4346
			public SerializableEntityStateType state;
		}
	}
}
