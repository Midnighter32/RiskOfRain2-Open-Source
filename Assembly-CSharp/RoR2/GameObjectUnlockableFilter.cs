using System;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002F7 RID: 759
	public class GameObjectUnlockableFilter : NetworkBehaviour
	{
		// Token: 0x06000F52 RID: 3922 RVA: 0x0004BCC8 File Offset: 0x00049EC8
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.Networkactive = this.GameObjectIsValid();
			}
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x0004BCDD File Offset: 0x00049EDD
		private void FixedUpdate()
		{
			base.gameObject.SetActive(this.active);
		}

		// Token: 0x06000F54 RID: 3924 RVA: 0x0004BCF0 File Offset: 0x00049EF0
		private bool GameObjectIsValid()
		{
			if (Run.instance)
			{
				bool flag = string.IsNullOrEmpty(this.requiredUnlockable) || Run.instance.IsUnlockableUnlocked(this.requiredUnlockable);
				bool flag2 = !string.IsNullOrEmpty(this.forbiddenUnlockable) && Run.instance.DoesEveryoneHaveThisUnlockableUnlocked(this.forbiddenUnlockable);
				return flag && !flag2;
			}
			return true;
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000F57 RID: 3927 RVA: 0x0004BD54 File Offset: 0x00049F54
		// (set) Token: 0x06000F58 RID: 3928 RVA: 0x0004BD67 File Offset: 0x00049F67
		public bool Networkactive
		{
			get
			{
				return this.active;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.active, 1u);
			}
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x0004BD7C File Offset: 0x00049F7C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.active);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.active);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x0004BDE8 File Offset: 0x00049FE8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.active = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.active = reader.ReadBoolean();
			}
		}

		// Token: 0x04001380 RID: 4992
		public string requiredUnlockable;

		// Token: 0x04001381 RID: 4993
		public string forbiddenUnlockable;

		// Token: 0x04001382 RID: 4994
		[SyncVar]
		private bool active;
	}
}
