using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Projectile;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003EA RID: 1002
	public static class ProjectileCatalog
	{
		// Token: 0x14000058 RID: 88
		// (add) Token: 0x06001851 RID: 6225 RVA: 0x000691A4 File Offset: 0x000673A4
		// (remove) Token: 0x06001852 RID: 6226 RVA: 0x000691D8 File Offset: 0x000673D8
		public static event Action<List<GameObject>> getAdditionalEntries;

		// Token: 0x06001853 RID: 6227 RVA: 0x0006920C File Offset: 0x0006740C
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			IEnumerable<GameObject> first = Resources.LoadAll<GameObject>("Prefabs/Projectiles/");
			List<GameObject> list = new List<GameObject>();
			Action<List<GameObject>> action = ProjectileCatalog.getAdditionalEntries;
			if (action != null)
			{
				action(list);
			}
			ProjectileCatalog.SetProjectilePrefabs(first.Concat(list.OrderBy((GameObject v) => v.name, StringComparer.Ordinal)).ToArray<GameObject>());
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x00069274 File Offset: 0x00067474
		private static void SetProjectilePrefabs(GameObject[] newProjectilePrefabs)
		{
			ProjectileCatalog.projectilePrefabs = HGArrayUtilities.Clone<GameObject>(newProjectilePrefabs);
			int num = 256;
			if (ProjectileCatalog.projectilePrefabs.Length > num)
			{
				Debug.LogErrorFormat("Cannot have more than {0} projectile prefabs defined, which is over the limit for {1}. Check comments at error source for details.", new object[]
				{
					num,
					typeof(byte).Name
				});
				for (int i = num; i < ProjectileCatalog.projectilePrefabs.Length; i++)
				{
					Debug.LogErrorFormat("Could not register projectile [{0}/{1}]=\"{2}\"", new object[]
					{
						i,
						num - 1,
						ProjectileCatalog.projectilePrefabs[i].name
					});
				}
			}
			ProjectileCatalog.projectilePrefabProjectileControllerComponents = new ProjectileController[ProjectileCatalog.projectilePrefabs.Length];
			ProjectileCatalog.projectileNames = new string[ProjectileCatalog.projectilePrefabs.Length];
			for (int j = 0; j < ProjectileCatalog.projectilePrefabs.Length; j++)
			{
				GameObject gameObject = ProjectileCatalog.projectilePrefabs[j];
				ProjectileController component = gameObject.GetComponent<ProjectileController>();
				component.catalogIndex = j;
				ProjectileCatalog.projectilePrefabProjectileControllerComponents[j] = component;
				ProjectileCatalog.projectileNames[j] = gameObject.name;
			}
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x0006936E File Offset: 0x0006756E
		public static int GetProjectileIndex(GameObject projectileObject)
		{
			if (projectileObject)
			{
				return ProjectileCatalog.GetProjectileIndex(projectileObject.GetComponent<ProjectileController>());
			}
			return -1;
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x00069385 File Offset: 0x00067585
		public static int GetProjectileIndex(ProjectileController projectileController)
		{
			if (!projectileController)
			{
				return -1;
			}
			return projectileController.catalogIndex;
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x00069397 File Offset: 0x00067597
		public static GameObject GetProjectilePrefab(int projectileIndex)
		{
			return HGArrayUtilities.GetSafe<GameObject>(ProjectileCatalog.projectilePrefabs, projectileIndex);
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x000693A4 File Offset: 0x000675A4
		public static ProjectileController GetProjectilePrefabProjectileControllerComponent(int projectileIndex)
		{
			return HGArrayUtilities.GetSafe<ProjectileController>(ProjectileCatalog.projectilePrefabProjectileControllerComponents, projectileIndex);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x000693B1 File Offset: 0x000675B1
		public static int FindProjectileIndex(string projectileName)
		{
			return Array.IndexOf<string>(ProjectileCatalog.projectileNames, projectileName);
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x000693C0 File Offset: 0x000675C0
		[ConCommand(commandName = "dump_projectile_map", flags = ConVarFlags.None, helpText = "Dumps the map between indices and projectile prefabs.")]
		private static void DumpProjectileMap(ConCommandArgs args)
		{
			string[] array = new string[ProjectileCatalog.projectilePrefabs.Length];
			for (int i = 0; i < ProjectileCatalog.projectilePrefabs.Length; i++)
			{
				array[i] = string.Format("[{0}] = {1}", i, ProjectileCatalog.projectilePrefabs[i].name);
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x040016E5 RID: 5861
		private static GameObject[] projectilePrefabs;

		// Token: 0x040016E6 RID: 5862
		private static ProjectileController[] projectilePrefabProjectileControllerComponents;

		// Token: 0x040016E7 RID: 5863
		private static string[] projectileNames;
	}
}
