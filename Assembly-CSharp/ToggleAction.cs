using System;

// Token: 0x02000079 RID: 121
public class ToggleAction : IDisposable
{
	// Token: 0x060001FF RID: 511 RVA: 0x0000A135 File Offset: 0x00008335
	public ToggleAction(Action activationAction, Action deactivationAction)
	{
		this.active = false;
		this.activationAction = activationAction;
		this.deactivationAction = deactivationAction;
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000200 RID: 512 RVA: 0x0000A152 File Offset: 0x00008352
	// (set) Token: 0x06000201 RID: 513 RVA: 0x0000A15A File Offset: 0x0000835A
	public bool active { get; private set; }

	// Token: 0x06000202 RID: 514 RVA: 0x0000A163 File Offset: 0x00008363
	public void SetActive(bool newActive)
	{
		if (this.active == newActive)
		{
			return;
		}
		this.active = newActive;
		if (this.active)
		{
			Action action = this.activationAction;
			if (action == null)
			{
				return;
			}
			action();
			return;
		}
		else
		{
			Action action2 = this.deactivationAction;
			if (action2 == null)
			{
				return;
			}
			action2();
			return;
		}
	}

	// Token: 0x06000203 RID: 515 RVA: 0x0000A19F File Offset: 0x0000839F
	public void Dispose()
	{
		this.SetActive(false);
	}

	// Token: 0x040001FC RID: 508
	private readonly Action activationAction;

	// Token: 0x040001FD RID: 509
	private readonly Action deactivationAction;
}
