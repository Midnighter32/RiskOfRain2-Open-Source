using System;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000707 RID: 1799
	[Serializable]
	public struct SerializableEntityStateType
	{
		// Token: 0x060029F2 RID: 10738 RVA: 0x000B01B3 File Offset: 0x000AE3B3
		public SerializableEntityStateType(string typeName)
		{
			this._typeName = "";
			this.typeName = typeName;
		}

		// Token: 0x060029F3 RID: 10739 RVA: 0x000B01C7 File Offset: 0x000AE3C7
		public SerializableEntityStateType(Type stateType)
		{
			this._typeName = "";
			this.stateType = stateType;
		}

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x060029F4 RID: 10740 RVA: 0x000B01DB File Offset: 0x000AE3DB
		// (set) Token: 0x060029F5 RID: 10741 RVA: 0x000B01E3 File Offset: 0x000AE3E3
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

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x060029F6 RID: 10742 RVA: 0x000B01F4 File Offset: 0x000AE3F4
		// (set) Token: 0x060029F7 RID: 10743 RVA: 0x000B0235 File Offset: 0x000AE435
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

		// Token: 0x040025C0 RID: 9664
		[SerializeField]
		private string _typeName;
	}
}
