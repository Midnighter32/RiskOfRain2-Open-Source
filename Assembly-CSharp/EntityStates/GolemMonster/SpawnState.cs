using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GolemMonster
{
	// Token: 0x02000860 RID: 2144
	public class SpawnState : GenericCharacterSpawnState
	{
		// Token: 0x06003066 RID: 12390 RVA: 0x000D07AC File Offset: 0x000CE9AC
		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				modelTransform.GetComponent<PrintController>().enabled = true;
			}
			Transform transform = base.FindModelChild("Eye");
			this.eyeGameObject = ((transform != null) ? transform.gameObject : null);
			if (this.eyeGameObject)
			{
				this.eyeGameObject.SetActive(false);
			}
		}

		// Token: 0x06003067 RID: 12391 RVA: 0x000D0810 File Offset: 0x000CEA10
		public override void OnExit()
		{
			if (!this.outer.destroying && this.eyeGameObject)
			{
				this.eyeGameObject.SetActive(true);
			}
		}

		// Token: 0x04002EA1 RID: 11937
		private GameObject eyeGameObject;
	}
}
