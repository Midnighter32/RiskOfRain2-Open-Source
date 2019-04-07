using System;
using JetBrains.Annotations;

// Token: 0x0200006D RID: 109
public struct ResourceAvailability
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060001A2 RID: 418 RVA: 0x00009258 File Offset: 0x00007458
	// (remove) Token: 0x060001A3 RID: 419 RVA: 0x00009290 File Offset: 0x00007490
	private event Action onAvailable;

	// Token: 0x060001A4 RID: 420 RVA: 0x000092C5 File Offset: 0x000074C5
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

	// Token: 0x060001A5 RID: 421 RVA: 0x000092EF File Offset: 0x000074EF
	public void CallWhenAvailable([NotNull] Action callback)
	{
		if (this.available)
		{
			callback();
			return;
		}
		this.onAvailable += callback;
	}

	// Token: 0x040001E9 RID: 489
	private bool available;
}
