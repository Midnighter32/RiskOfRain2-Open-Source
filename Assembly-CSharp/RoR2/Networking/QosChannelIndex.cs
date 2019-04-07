using System;

namespace RoR2.Networking
{
	// Token: 0x02000589 RID: 1417
	public struct QosChannelIndex
	{
		// Token: 0x0400222E RID: 8750
		public int intVal;

		// Token: 0x0400222F RID: 8751
		public static QosChannelIndex defaultReliable = new QosChannelIndex
		{
			intVal = 0
		};

		// Token: 0x04002230 RID: 8752
		public static QosChannelIndex defaultUnreliable = new QosChannelIndex
		{
			intVal = 1
		};

		// Token: 0x04002231 RID: 8753
		public static QosChannelIndex characterTransformUnreliable = new QosChannelIndex
		{
			intVal = 2
		};

		// Token: 0x04002232 RID: 8754
		public static QosChannelIndex time = new QosChannelIndex
		{
			intVal = 3
		};

		// Token: 0x04002233 RID: 8755
		public static QosChannelIndex chat = new QosChannelIndex
		{
			intVal = 4
		};

		// Token: 0x04002234 RID: 8756
		public const int viewAnglesChannel = 5;

		// Token: 0x04002235 RID: 8757
		public static QosChannelIndex viewAngles = new QosChannelIndex
		{
			intVal = 5
		};

		// Token: 0x04002236 RID: 8758
		public static QosChannelIndex ping = new QosChannelIndex
		{
			intVal = 6
		};

		// Token: 0x04002237 RID: 8759
		public static QosChannelIndex effects = new QosChannelIndex
		{
			intVal = 7
		};
	}
}
