using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000102 RID: 258
	[CreateAssetMenu(menuName = "SpawnCards")]
	public class CharacterSpawnCard : SpawnCard
	{
		// Token: 0x060004E7 RID: 1255 RVA: 0x00013B3D File Offset: 0x00011D3D
		public void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (!this.loadout.isEmpty)
			{
				this._runtimeLoadout = new Loadout();
				this.loadout.Apply(this._runtimeLoadout);
			}
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x00013B70 File Offset: 0x00011D70
		public override GameObject DoSpawn(Vector3 position, Quaternion rotation, DirectorSpawnRequest directorSpawnRequest)
		{
			CharacterMaster characterMaster = new MasterSummon
			{
				masterPrefab = this.prefab,
				position = position,
				rotation = rotation,
				summonerBodyObject = directorSpawnRequest.summonerBodyObject,
				teamIndexOverride = directorSpawnRequest.teamIndexOverride,
				ignoreTeamMemberLimit = directorSpawnRequest.ignoreTeamMemberLimit,
				loadout = this._runtimeLoadout
			}.Perform();
			if (characterMaster == null)
			{
				return null;
			}
			return characterMaster.gameObject;
		}

		// Token: 0x040004AF RID: 1199
		public bool noElites;

		// Token: 0x040004B0 RID: 1200
		public bool forbiddenAsBoss;

		// Token: 0x040004B1 RID: 1201
		public SerializableLoadout loadout;

		// Token: 0x040004B2 RID: 1202
		private Loadout _runtimeLoadout;
	}
}
