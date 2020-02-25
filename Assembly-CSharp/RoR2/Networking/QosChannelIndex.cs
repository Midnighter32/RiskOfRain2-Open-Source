using System;

namespace RoR2.Networking
{
	// Token: 0x02000557 RID: 1367
	public struct QosChannelIndex
	{
		// Token: 0x04001DD7 RID: 7639
		public int intVal;

		// Token: 0x04001DD8 RID: 7640
		public static QosChannelIndex defaultReliable = new QosChannelIndex
		{
			intVal = 0
		};

		// Token: 0x04001DD9 RID: 7641
		public static QosChannelIndex defaultUnreliable = new QosChannelIndex
		{
			intVal = 1
		};

		// Token: 0x04001DDA RID: 7642
		public static QosChannelIndex characterTransformUnreliable = new QosChannelIndex
		{
			intVal = 2
		};

		// Token: 0x04001DDB RID: 7643
		public static QosChannelIndex time = new QosChannelIndex
		{
			intVal = 3
		};

		// Token: 0x04001DDC RID: 7644
		public static QosChannelIndex chat = new QosChannelIndex
		{
			intVal = 4
		};

		// Token: 0x04001DDD RID: 7645
		public const int viewAnglesChannel = 5;

		// Token: 0x04001DDE RID: 7646
		public static QosChannelIndex viewAngles = new QosChannelIndex
		{
			intVal = 5
		};

		// Token: 0x04001DDF RID: 7647
		public static QosChannelIndex ping = new QosChannelIndex
		{
			intVal = 6
		};

		// Token: 0x04001DE0 RID: 7648
		public static QosChannelIndex effects = new QosChannelIndex
		{
			intVal = 7
		};
	}
}
