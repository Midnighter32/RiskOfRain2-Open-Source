using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000B0 RID: 176
	[Serializable]
	public struct SerializableEntityStateType
	{
		// Token: 0x06000378 RID: 888 RVA: 0x0000DC8B File Offset: 0x0000BE8B
		public SerializableEntityStateType(string typeName)
		{
			this._typeName = "";
			this.typeName = typeName;
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0000DC9F File Offset: 0x0000BE9F
		public SerializableEntityStateType(Type stateType)
		{
			this._typeName = "";
			this.stateType = stateType;
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600037A RID: 890 RVA: 0x0000DCB3 File Offset: 0x0000BEB3
		// (set) Token: 0x0600037B RID: 891 RVA: 0x0000DCBB File Offset: 0x0000BEBB
		private string typeName
		{
			get
			{
				return this._typeName;
			}
			set
			{
				this.stateType = Type.GetType(value);
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600037C RID: 892 RVA: 0x0000DCCC File Offset: 0x0000BECC
		// (set) Token: 0x0600037D RID: 893 RVA: 0x0000DD0D File Offset: 0x0000BF0D
		public Type stateType
		{
			get
			{
				if (this._typeName == null)
				{
					return null;
				}
				Type type = Type.GetType(this._typeName);
				if (!(type != null) || !type.IsSubclassOf(typeof(EntityState)))
				{
					return null;
				}
				return type;
			}
			set
			{
				this._typeName = ((value != null && value.IsSubclassOf(typeof(EntityState))) ? value.FullName : "");
			}
		}

		// Token: 0x0400032D RID: 813
		[SerializeField]
		private string _typeName;
	}
}
