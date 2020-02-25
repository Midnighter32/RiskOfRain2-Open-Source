using System;
using RoR2.Audio;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200010D RID: 269
	[CreateAssetMenu(menuName = "RoR2/NetworkSoundEventDef")]
	public class NetworkSoundEventDef : ScriptableObject
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000507 RID: 1287 RVA: 0x0001435D File Offset: 0x0001255D
		// (set) Token: 0x06000508 RID: 1288 RVA: 0x00014365 File Offset: 0x00012565
		public NetworkSoundEventIndex index { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000509 RID: 1289 RVA: 0x0001436E File Offset: 0x0001256E
		// (set) Token: 0x0600050A RID: 1290 RVA: 0x00014376 File Offset: 0x00012576
		public uint akId { get; set; }

		// Token: 0x040004D8 RID: 1240
		public string eventName;
	}
}
