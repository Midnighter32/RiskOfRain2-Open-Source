using System;

namespace RoR2
{
	// Token: 0x020004C3 RID: 1219
	public class TimerQueue
	{
		// Token: 0x06001B67 RID: 7015 RVA: 0x00080158 File Offset: 0x0007E358
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

		// Token: 0x06001B68 RID: 7016 RVA: 0x000801EC File Offset: 0x0007E3EC
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

		// Token: 0x06001B69 RID: 7017 RVA: 0x0008022B File Offset: 0x0007E42B
		private void RemoveTimerAt(int i)
		{
			this.indexAllocator.FreeIndex(this.timers[i].handle.uid);
			HGArrayUtilities.ArrayRemoveAt<TimerQueue.Timer>(ref this.timers, ref this.count, i, 1);
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x00080264 File Offset: 0x0007E464
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

		// Token: 0x04001E03 RID: 7683
		private float internalTime;

		// Token: 0x04001E04 RID: 7684
		private int count;

		// Token: 0x04001E05 RID: 7685
		private TimerQueue.Timer[] timers = Array.Empty<TimerQueue.Timer>();

		// Token: 0x04001E06 RID: 7686
		private readonly IndexAllocator indexAllocator = new IndexAllocator();

		// Token: 0x04001E07 RID: 7687
		private Action[] actionsToCall = Array.Empty<Action>();

		// Token: 0x04001E08 RID: 7688
		private int actionsToCallCount;

		// Token: 0x020004C4 RID: 1220
		public struct TimerHandle : IEquatable<TimerQueue.TimerHandle>
		{
			// Token: 0x06001B6C RID: 7020 RVA: 0x00080342 File Offset: 0x0007E542
			public TimerHandle(int uid)
			{
				this.uid = uid;
			}

			// Token: 0x06001B6D RID: 7021 RVA: 0x0008034B File Offset: 0x0007E54B
			public bool Equals(TimerQueue.TimerHandle other)
			{
				return this.uid == other.uid;
			}

			// Token: 0x06001B6E RID: 7022 RVA: 0x0008035B File Offset: 0x0007E55B
			public override bool Equals(object obj)
			{
				return obj != null && obj is TimerQueue.TimerHandle && this.Equals((TimerQueue.TimerHandle)obj);
			}

			// Token: 0x06001B6F RID: 7023 RVA: 0x00080378 File Offset: 0x0007E578
			public override int GetHashCode()
			{
				return this.uid;
			}

			// Token: 0x04001E09 RID: 7689
			public static readonly TimerQueue.TimerHandle invalid = new TimerQueue.TimerHandle(-1);

			// Token: 0x04001E0A RID: 7690
			public readonly int uid;
		}

		// Token: 0x020004C5 RID: 1221
		private struct Timer
		{
			// Token: 0x04001E0B RID: 7691
			public float time;

			// Token: 0x04001E0C RID: 7692
			public Action action;

			// Token: 0x04001E0D RID: 7693
			public TimerQueue.TimerHandle handle;
		}
	}
}
