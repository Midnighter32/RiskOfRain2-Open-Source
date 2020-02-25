using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D1 RID: 977
	[RequireComponent(typeof(CharacterModel))]
	[DisallowMultipleComponent]
	public class ModelSkinController : MonoBehaviour
	{
		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x060017BE RID: 6078 RVA: 0x000671AB File Offset: 0x000653AB
		// (set) Token: 0x060017BF RID: 6079 RVA: 0x000671B3 File Offset: 0x000653B3
		public int currentSkinIndex { get; private set; } = -1;

		// Token: 0x060017C0 RID: 6080 RVA: 0x000671BC File Offset: 0x000653BC
		private void Awake()
		{
			this.characterModel = base.GetComponent<CharacterModel>();
		}

		// Token: 0x060017C1 RID: 6081 RVA: 0x000671CA File Offset: 0x000653CA
		private void Start()
		{
			if (this.characterModel.body)
			{
				this.ApplySkin((int)this.characterModel.body.skinIndex);
			}
		}

		// Token: 0x060017C2 RID: 6082 RVA: 0x000671F4 File Offset: 0x000653F4
		public void ApplySkin(int skinIndex)
		{
			if (skinIndex == this.currentSkinIndex || (ulong)skinIndex >= (ulong)((long)this.skins.Length))
			{
				return;
			}
			this.skins[skinIndex].Apply(base.gameObject);
			this.currentSkinIndex = skinIndex;
		}

		// Token: 0x04001662 RID: 5730
		public SkinDef[] skins;

		// Token: 0x04001664 RID: 5732
		private CharacterModel characterModel;
	}
}
