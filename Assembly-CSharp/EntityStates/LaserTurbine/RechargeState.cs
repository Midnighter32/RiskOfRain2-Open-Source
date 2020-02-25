using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.LaserTurbine
{
	// Token: 0x020007F9 RID: 2041
	public class RechargeState : LaserTurbineBaseState
	{
		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06002E6B RID: 11883 RVA: 0x000C55D9 File Offset: 0x000C37D9
		// (set) Token: 0x06002E6C RID: 11884 RVA: 0x000C55E1 File Offset: 0x000C37E1
		public Run.FixedTimeStamp startTime { get; private set; }

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06002E6D RID: 11885 RVA: 0x000C55EA File Offset: 0x000C37EA
		// (set) Token: 0x06002E6E RID: 11886 RVA: 0x000C55F2 File Offset: 0x000C37F2
		public Run.FixedTimeStamp readyTime { get; private set; }

		// Token: 0x06002E6F RID: 11887 RVA: 0x000C55FB File Offset: 0x000C37FB
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				this.startTime = Run.FixedTimeStamp.now;
				this.readyTime = this.startTime + RechargeState.baseDuration;
			}
		}

		// Token: 0x06002E70 RID: 11888 RVA: 0x000C562C File Offset: 0x000C382C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.laserTurbineController.charge >= 1f)
			{
				this.outer.SetNextState(new ReadyState());
			}
		}

		// Token: 0x06002E71 RID: 11889 RVA: 0x000C565E File Offset: 0x000C385E
		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.startTime);
			writer.Write(this.readyTime);
		}

		// Token: 0x06002E72 RID: 11890 RVA: 0x000C567F File Offset: 0x000C387F
		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.startTime = reader.ReadFixedTimeStamp();
			this.readyTime = reader.ReadFixedTimeStamp();
		}

		// Token: 0x06002E73 RID: 11891 RVA: 0x000C56A0 File Offset: 0x000C38A0
		public override float GetChargeFraction()
		{
			return Mathf.Clamp01(this.startTime.timeSince / (this.readyTime - this.startTime));
		}

		// Token: 0x04002B8A RID: 11146
		public static float baseDuration = 60f;
	}
}
