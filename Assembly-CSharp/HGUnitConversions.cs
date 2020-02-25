using System;

// Token: 0x0200005D RID: 93
public static class HGUnitConversions
{
	// Token: 0x040001AA RID: 426
	public static readonly double milesToKilometers = 1.609344;

	// Token: 0x040001AB RID: 427
	public static readonly double kilometersToMeters = 1000.0;

	// Token: 0x040001AC RID: 428
	public static readonly double milesToMeters = HGUnitConversions.milesToKilometers * HGUnitConversions.kilometersToMeters;

	// Token: 0x040001AD RID: 429
	public static readonly double hoursToMinutes = 60.0;

	// Token: 0x040001AE RID: 430
	public static readonly double minutesToSeconds = 60.0;

	// Token: 0x040001AF RID: 431
	public static readonly double hoursToSeconds = HGUnitConversions.hoursToMinutes * HGUnitConversions.minutesToSeconds;
}
