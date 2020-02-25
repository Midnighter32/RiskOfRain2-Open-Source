using System;

namespace RoR2.Navigation
{
	// Token: 0x020004E3 RID: 1251
	public class PathTask
	{
		// Token: 0x06001DD4 RID: 7636 RVA: 0x0007FBBA File Offset: 0x0007DDBA
		public PathTask(Path path)
		{
			this.path = path;
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x0000409B File Offset: 0x0000229B
		public void Wait()
		{
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06001DD6 RID: 7638 RVA: 0x0007FBC9 File Offset: 0x0007DDC9
		// (set) Token: 0x06001DD7 RID: 7639 RVA: 0x0007FBD1 File Offset: 0x0007DDD1
		public Path path { get; private set; }

		// Token: 0x04001B09 RID: 6921
		public PathTask.TaskStatus status;

		// Token: 0x020004E4 RID: 1252
		public enum TaskStatus
		{
			// Token: 0x04001B0C RID: 6924
			NotStarted,
			// Token: 0x04001B0D RID: 6925
			Running,
			// Token: 0x04001B0E RID: 6926
			Complete
		}
	}
}
