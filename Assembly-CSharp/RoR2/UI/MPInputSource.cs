using System;
using Rewired.UI;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200060A RID: 1546
	public class MPInputSource : IMouseInputSource
	{
		// Token: 0x060022D4 RID: 8916 RVA: 0x0007E12C File Offset: 0x0007C32C
		public bool GetButtonDown(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060022D5 RID: 8917 RVA: 0x0007E12C File Offset: 0x0007C32C
		public bool GetButtonUp(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060022D6 RID: 8918 RVA: 0x0007E12C File Offset: 0x0007C32C
		public bool GetButton(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060022D7 RID: 8919 RVA: 0x000A4814 File Offset: 0x000A2A14
		public int playerId { get; }

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060022D8 RID: 8920 RVA: 0x000A481C File Offset: 0x000A2A1C
		public bool enabled { get; }

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060022D9 RID: 8921 RVA: 0x000A4824 File Offset: 0x000A2A24
		public bool locked { get; }

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060022DA RID: 8922 RVA: 0x000A482C File Offset: 0x000A2A2C
		public int buttonCount { get; }

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060022DB RID: 8923 RVA: 0x000A4834 File Offset: 0x000A2A34
		public Vector2 screenPosition { get; }

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060022DC RID: 8924 RVA: 0x000A483C File Offset: 0x000A2A3C
		public Vector2 screenPositionDelta { get; }

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060022DD RID: 8925 RVA: 0x000A4844 File Offset: 0x000A2A44
		public Vector2 wheelDelta { get; }
	}
}
