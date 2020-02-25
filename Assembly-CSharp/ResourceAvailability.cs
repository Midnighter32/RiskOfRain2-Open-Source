using System;
using JetBrains.Annotations;

// Token: 0x02000071 RID: 113
public struct ResourceAvailability
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060001D0 RID: 464 RVA: 0x00009B64 File Offset: 0x00007D64
	// (remove) Token: 0x060001D1 RID: 465 RVA: 0x00009B9C File Offset: 0x00007D9C
	private event Action onAvailable;

	// Token: 0x060001D2 RID: 466 RVA: 0x00009BD1 File Offset: 0x00007DD1
	public void MakeAvailable()
	{
		if (this.available)
		{
			return;
		}
		this.available = true;
		Action action = this.onAvailable;
		if (action != null)
		{
			action();
		}
		this.onAvailable = null;
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x00009BFB File Offset: 0x00007DFB
	public void CallWhenAvailable([NotNull] Action callback)
	{
		if (this.available)
		{
			callback();
			return;
		}
		this.onAvailable += callback;
	}

	// Token: 0x040001F0 RID: 496
	private bool available;
}
