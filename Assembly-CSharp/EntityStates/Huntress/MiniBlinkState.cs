using System;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x02000831 RID: 2097
	public class MiniBlinkState : BlinkState
	{
		// Token: 0x06002F79 RID: 12153 RVA: 0x000CB090 File Offset: 0x000C9290
		protected override Vector3 GetBlinkVector()
		{
			return ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
		}
	}
}
