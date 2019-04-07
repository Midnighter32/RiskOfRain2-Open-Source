using System;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class WarningIndicator : MonoBehaviour
{
	// Token: 0x06000178 RID: 376 RVA: 0x000086E7 File Offset: 0x000068E7
	private void Start()
	{
		this.AssembleWarningInfo();
	}

	// Token: 0x06000179 RID: 377 RVA: 0x000086F0 File Offset: 0x000068F0
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

	// Token: 0x0600017A RID: 378 RVA: 0x00004507 File Offset: 0x00002707
	private void Update()
	{
	}

	// Token: 0x04000195 RID: 405
	public WarningIndicator.WarningDistance warningDistance;

	// Token: 0x04000196 RID: 406
	public WarningIndicator.WarningType warningType;

	// Token: 0x04000197 RID: 407
	private WarningIndicator.WarningInfo warningInfo;

	// Token: 0x0200005C RID: 92
	public enum WarningType
	{
		// Token: 0x04000199 RID: 409
		Projectile,
		// Token: 0x0400019A RID: 410
		NearbyMonster,
		// Token: 0x0400019B RID: 411
		ChargeAttack
	}

	// Token: 0x0200005D RID: 93
	public enum WarningDistance
	{
		// Token: 0x0400019D RID: 413
		Near,
		// Token: 0x0400019E RID: 414
		Far,
		// Token: 0x0400019F RID: 415
		Extreme
	}

	// Token: 0x0200005E RID: 94
	private struct WarningInfo
	{
		// Token: 0x040001A0 RID: 416
		public float warningNearDistance;

		// Token: 0x040001A1 RID: 417
		public float warningFarDistance;

		// Token: 0x040001A2 RID: 418
		public float warningIntensity;

		// Token: 0x040001A3 RID: 419
		public bool enableWarningDuration;

		// Token: 0x040001A4 RID: 420
		public bool warningDuration;
	}
}
