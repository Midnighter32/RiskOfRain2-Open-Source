using System;

namespace RoR2
{
	// Token: 0x0200045B RID: 1115
	public class TimerQueue
	{
		// Token: 0x06001AFE RID: 6910 RVA: 0x00072854 File Offset: 0x00070A54
		public TimerQueue.TimerHandle CreateTimer(float time, Action action)
		{
			time += this.internalTime;
			int position = this.count;
			for (int i = 0; i < this.count; i++)
			{
				if (time < this.timers[i].time)
				{
					position = i;
					break;
				}
			}
			TimerQueue.TimerHandle timerHandle = new TimerQueue.TimerHandle(this.indexAllocator.RequestIndex());
			TimerQueue.Timer timer = new TimerQueue.Timer
			{
				time = time,
				action = action,
				handle = timerHandle
			};
			HGArrayUtilities.ArrayInsert<TimerQueue.Timer>(ref this.timers, ref this.count, position, ref timer);
			return timerHandle;
		}

		// Token: 0x06001AFF RID: 6911 RVA: 0x000728E8 File Offset: 0x00070AE8
		public void RemoveTimer(TimerQueue.TimerHandle timerHandle)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.timers[i].handle.Equals(timerHandle))
				{
					this.RemoveTimerAt(i);
					return;
				}
			}
		}

		// Token: 0x06001B00 RID: 6912 RVA: 0x00072927 File Offset: 0x00070B27
		private void RemoveTimerAt(int i)
		{
			this.indexAllocator.FreeIndex(this.timers[i].handle.uid);
			HGArrayUtilities.ArrayRemoveAt<TimerQueue.Timer>(ref this.timers, ref this.count, i, 1);
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x00072960 File Offset: 0x00070B60
		public void Update(float deltaTime)
		{
			this.internalTime += deltaTime;
			int num = 0;
			while (num < this.count && this.timers[num].time <= this.internalTime)
			{
				HGArrayUtilities.ArrayInsert<Action>(ref this.actionsToCall, ref this.actionsToCallCount, this.actionsToCallCount, ref this.timers[num].action);
				num++;
			}
			for (int i = this.actionsToCallCount - 1; i >= 0; i--)
			{
				this.RemoveTimerAt(i);
			}
			for (int j = 0; j < this.actionsToCallCount; j++)
			{
				this.actionsToCall[j]();
				this.actionsToCall[j] = null;
			}
			this.actionsToCallCount = 0;
		}

		// Token: 0x04001881 RID: 6273
		private float internalTime;

		// Token: 0x04001882 RID: 6274
		private int count;

		// Token: 0x04001883 RID: 6275
		private TimerQueue.Timer[] timers = Array.Empty<TimerQueue.Timer>();

		// Token: 0x04001884 RID: 6276
		private readonly IndexAllocator indexAllocator = new IndexAllocator();

		// Token: 0x04001885 RID: 6277
		private Action[] actionsToCall = Array.Empty<Action>();

		// Token: 0x04001886 RID: 6278
		private int actionsToCallCount;

		// Token: 0x0200045C RID: 1116
		public struct TimerHandle : IEquatable<TimerQueue.TimerHandle>
		{
			// Token: 0x06001B03 RID: 6915 RVA: 0x00072A3E File Offset: 0x00070C3E
			public TimerHandle(int uid)
			{
				this.uid = uid;
			}

			// Token: 0x06001B04 RID: 6916 RVA: 0x00072A47 File Offset: 0x00070C47
			public bool Equals(TimerQueue.TimerHandle other)
			{
				return this.uid == other.uid;
			}

			// Token: 0x06001B05 RID: 6917 RVA: 0x00072A57 File Offset: 0x00070C57
			public override bool Equals(object obj)
			{
				return obj != null && obj is TimerQueue.TimerHandle && this.Equals((TimerQueue.TimerHandle)obj);
			}

			// Token: 0x06001B06 RID: 6918 RVA: 0x00072A74 File Offset: 0x00070C74
			public override int GetHashCode()
			{
				return this.uid;
			}

			// Token: 0x04001887 RID: 6279
			public static readonly TimerQueue.TimerHandle invalid = new TimerQueue.TimerHandle(-1);

			// Token: 0x04001888 RID: 6280
			public readonly int uid;
		}

		// Token: 0x0200045D RID: 1117
		private struct Timer
		{
			// Token: 0x04001889 RID: 6281
			public float time;

			// Token: 0x0400188A RID: 6282
			public Action action;

			// Token: 0x0400188B RID: 6283
			public TimerQueue.TimerHandle handle;
		}
	}
}
