using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002DC RID: 732
	public class RadialForce : MonoBehaviour
	{
		// Token: 0x060010CB RID: 4299 RVA: 0x000497F7 File Offset: 0x000479F7
		private void Start()
		{
			this._transform = base.GetComponent<Transform>();
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x00049814 File Offset: 0x00047A14
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

		// Token: 0x060010CD RID: 4301 RVA: 0x0004987C File Offset: 0x00047A7C
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
							component.TakeDamageForce(a - velocity * (this.damping * mass * num), true, false);
						}
					}
				}
			}
		}

		// Token: 0x0400101A RID: 4122
		public GameObject tetherPrefab;

		// Token: 0x0400101B RID: 4123
		public float radius;

		// Token: 0x0400101C RID: 4124
		public float damping = 0.2f;

		// Token: 0x0400101D RID: 4125
		public float forceMagnitude;

		// Token: 0x0400101E RID: 4126
		public float forceCoefficientAtEdge = 0.5f;

		// Token: 0x0400101F RID: 4127
		private Transform _transform;

		// Token: 0x04001020 RID: 4128
		private TeamFilter teamFilter;

		// Token: 0x04001021 RID: 4129
		private List<GameObject> affectedObjects = new List<GameObject>();
	}
}
