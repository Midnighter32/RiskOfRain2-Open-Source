using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class WarningIndicator : MonoBehaviour
{
	// Token: 0x0600015B RID: 347 RVA: 0x000085D3 File Offset: 0x000067D3
	private void Start()
	{
		this.AssembleWarningInfo();
	}

	// Token: 0x0600015C RID: 348 RVA: 0x000085DC File Offset: 0x000067DC
	private void AssembleWarningInfo()
	{
		this.warningInfo = default(WarningIndicator.WarningInfo);
		switch (this.warningDistance)
		{
		case WarningIndicator.WarningDistance.Near:
			this.warningInfo.warningNearDistance = 5f;
			this.warningInfo.warningFarDistance = 25f;
			break;
		case WarningIndicator.WarningDistance.Far:
			this.warningInfo.warningNearDistance = 25f;
			this.warningInfo.warningFarDistance = 60f;
			break;
		case WarningIndicator.WarningDistance.Extreme:
			this.warningInfo.warningNearDistance = 60f;
			this.warningInfo.warningFarDistance = 1000f;
			break;
		}
		WarningIndicator.WarningType warningType = this.warningType;
	}

	// Token: 0x0600015D RID: 349 RVA: 0x0000409B File Offset: 0x0000229B
	private void Update()
	{
	}

	// Token: 0x04000194 RID: 404
	public WarningIndicator.WarningDistance warningDistance;

	// Token: 0x04000195 RID: 405
	public WarningIndicator.WarningType warningType;

	// Token: 0x04000196 RID: 406
	private WarningIndicator.WarningInfo warningInfo;

	// Token: 0x02000057 RID: 87
	public enum WarningType
	{
		// Token: 0x04000198 RID: 408
		Projectile,
		// Token: 0x04000199 RID: 409
		NearbyMonster,
		// Token: 0x0400019A RID: 410
		ChargeAttack
	}

	// Token: 0x02000058 RID: 88
	public enum WarningDistance
	{
		// Token: 0x0400019C RID: 412
		Near,
		// Token: 0x0400019D RID: 413
		Far,
		// Token: 0x0400019E RID: 414
		Extreme
	}

	// Token: 0x02000059 RID: 89
	private struct WarningInfo
	{
		// Token: 0x0400019F RID: 415
		public float warningNearDistance;

		// Token: 0x040001A0 RID: 416
		public float warningFarDistance;

		// Token: 0x040001A1 RID: 417
		public float warningIntensity;

		// Token: 0x040001A2 RID: 418
		public bool enableWarningDuration;

		// Token: 0x040001A3 RID: 419
		public bool warningDuration;
	}
}
