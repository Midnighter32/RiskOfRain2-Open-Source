using System;
using Rewired.UI;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F9 RID: 1529
	public class MPInputSource : IMouseInputSource
	{
		// Token: 0x06002444 RID: 9284 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		public bool GetButtonDown(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002445 RID: 9285 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		public bool GetButtonUp(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002446 RID: 9286 RVA: 0x0006FBB0 File Offset: 0x0006DDB0
		public bool GetButton(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06002447 RID: 9287 RVA: 0x0009E984 File Offset: 0x0009CB84
		public int playerId { get; }

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06002448 RID: 9288 RVA: 0x0009E98C File Offset: 0x0009CB8C
		public bool enabled { get; }

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06002449 RID: 9289 RVA: 0x0009E994 File Offset: 0x0009CB94
		public bool locked { get; }

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x0600244A RID: 9290 RVA: 0x0009E99C File Offset: 0x0009CB9C
		public int buttonCount { get; }

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x0600244B RID: 9291 RVA: 0x0009E9A4 File Offset: 0x0009CBA4
		public Vector2 screenPosition { get; }

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x0600244C RID: 9292 RVA: 0x0009E9AC File Offset: 0x0009CBAC
		public Vector2 screenPositionDelta { get; }

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x0600244D RID: 9293 RVA: 0x0009E9B4 File Offset: 0x0009CBB4
		public Vector2 wheelDelta { get; }
	}
}
