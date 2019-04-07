using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200039F RID: 927
	public class RadialForce : MonoBehaviour
	{
		// Token: 0x060013A4 RID: 5028 RVA: 0x0005FE1F File Offset: 0x0005E01F
		private void Start()
		{
			this._transform = base.GetComponent<Transform>();
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x0005FE3C File Offset: 0x0005E03C
		private void AddToList(GameObject affectedObject)
		{
			if (this.tetherPrefab && !this.affectedObjects.Contains(affectedObject))
			{
				TetherEffect component = UnityEngine.Object.Instantiate<GameObject>(this.tetherPrefab, affectedObject.transform).GetComponent<TetherEffect>();
				component.tetherEndTransform = base.transform;
				component.tetherMaxDistance = this.radius + 1f;
				this.affectedObjects.Add(affectedObject);
			}
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x0005FEA4 File Offset: 0x0005E0A4
		private void FixedUpdate()
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, this.radius, LayerIndex.defaultLayer.mask);
			for (int i = 0; i < array.Length; i++)
			{
				HealthComponent component = array[i].GetComponent<HealthComponent>();
				CharacterMotor component2 = array[i].GetComponent<CharacterMotor>();
				if (component)
				{
					TeamComponent component3 = component.GetComponent<TeamComponent>();
					bool flag = false;
					if (component3 && this.teamFilter)
					{
						flag = (component3.teamIndex == this.teamFilter.teamIndex);
					}
					if (!flag)
					{
						this.AddToList(component.gameObject);
						if (NetworkServer.active)
						{
							Vector3 a = array[i].transform.position - this._transform.position;
							float num = 1f - Mathf.Clamp(a.magnitude / this.radius, 0f, 1f - this.forceCoefficientAtEdge);
							a = a.normalized * this.forceMagnitude * (1f - num);
							Vector3 velocity;
							float mass;
							if (component2)
							{
								velocity = component2.velocity;
								mass = component2.mass;
							}
							else
							{
								Rigidbody component4 = component.GetComponent<Rigidbody>();
								velocity = component4.velocity;
								mass = component4.mass;
							}
							velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
							component.TakeDamageForce(a - velocity * (this.damping * mass * num), true);
						}
					}
				}
			}
		}

		// Token: 0x0400174B RID: 5963
		public GameObject tetherPrefab;

		// Token: 0x0400174C RID: 5964
		public float radius;

		// Token: 0x0400174D RID: 5965
		public float damping = 0.2f;

		// Token: 0x0400174E RID: 5966
		public float forceMagnitude;

		// Token: 0x0400174F RID: 5967
		public float forceCoefficientAtEdge = 0.5f;

		// Token: 0x04001750 RID: 5968
		private Transform _transform;

		// Token: 0x04001751 RID: 5969
		private TeamFilter teamFilter;

		// Token: 0x04001752 RID: 5970
		private List<GameObject> affectedObjects = new List<GameObject>();
	}
}
