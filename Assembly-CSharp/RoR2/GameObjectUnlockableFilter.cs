using System;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000206 RID: 518
	public class GameObjectUnlockableFilter : NetworkBehaviour
	{
		// Token: 0x06000B09 RID: 2825 RVA: 0x00031121 File Offset: 0x0002F321
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.Networkactive = this.GameObjectIsValid();
			}
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x00031136 File Offset: 0x0002F336
		private void FixedUpdate()
		{
			base.gameObject.SetActive(this.active);
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x0003114C File Offset: 0x0002F34C
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

		// Token: 0x06000B0D RID: 2829 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000B0E RID: 2830 RVA: 0x000311B0 File Offset: 0x0002F3B0
		// (set) Token: 0x06000B0F RID: 2831 RVA: 0x000311C3 File Offset: 0x0002F3C3
		public bool Networkactive
		{
			get
			{
				return this.active;
			}
			[param: In]
			set
			{
				base.SetSyncVar<bool>(value, ref this.active, 1U);
			}
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x000311D8 File Offset: 0x0002F3D8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.active);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
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

		// Token: 0x06000B11 RID: 2833 RVA: 0x00031244 File Offset: 0x0002F444
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

		// Token: 0x04000B80 RID: 2944
		public string requiredUnlockable;

		// Token: 0x04000B81 RID: 2945
		public string forbiddenUnlockable;

		// Token: 0x04000B82 RID: 2946
		[SyncVar]
		private bool active;
	}
}
