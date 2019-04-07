using System;

namespace RoR2.Navigation
{
	// Token: 0x02000529 RID: 1321
	public class PathTask
	{
		// Token: 0x06001DA1 RID: 7585 RVA: 0x0008A87E File Offset: 0x00088A7E
		public PathTask(Path path)
		{
			this.path = path;
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x00004507 File Offset: 0x00002707
		public void Wait()
		{
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06001DA3 RID: 7587 RVA: 0x0008A88D File Offset: 0x00088A8D
		// (set) Token: 0x06001DA4 RID: 7588 RVA: 0x0008A895 File Offset: 0x00088A95
		public Path path { get; private set; }

		// Token: 0x04001FF0 RID: 8176
		public PathTask.TaskStatus status;

		// Token: 0x0200052A RID: 1322
		public enum TaskStatus
		{
			// Token: 0x04001FF3 RID: 8179
			NotStarted,
			// Token: 0x04001FF4 RID: 8180
			Running,
			// Token: 0x04001FF5 RID: 8181
			Complete
		}
	}
}
