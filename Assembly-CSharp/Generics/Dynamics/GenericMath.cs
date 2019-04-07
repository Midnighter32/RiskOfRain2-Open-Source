using System;
using RoR2;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x020006DE RID: 1758
	public static class GenericMath
	{
		// Token: 0x06002758 RID: 10072 RVA: 0x000B672C File Offset: 0x000B492C
		public static Quaternion ApplyQuaternion(Quaternion _qA, Quaternion _qB)
		{
			Quaternion identity = Quaternion.identity;
			Vector3 vector = new Vector3(_qA.x, _qA.y, _qA.z);
			Vector3 vector2 = new Vector3(_qB.x, _qB.y, _qB.z);
			identity.w = _qA.w * _qB.w - Vector3.Dot(vector, vector2);
			Vector3 vector3 = Vector3.Cross(vector, vector2) + _qA.w * vector2 + _qB.w * vector;
			identity.x = vector3.x;
			identity.y = vector3.y;
			identity.z = vector3.z;
			return identity;
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x000B67E0 File Offset: 0x000B49E0
		public static Quaternion QuaternionFromAngleAxis(float _angle, Vector3 _axis)
		{
			Quaternion identity = Quaternion.identity;
			_axis.Normalize();
			_angle *= 0.017453292f;
			identity.x = _axis.x * Mathf.Sin(_angle / 2f);
			identity.y = _axis.y * Mathf.Sin(_angle / 2f);
			identity.z = _axis.z * Mathf.Sin(_angle / 2f);
			identity.w = Mathf.Cos(_angle / 2f);
			return identity;
		}

		// Token: 0x0600275A RID: 10074 RVA: 0x000B6868 File Offset: 0x000B4A68
		public static Quaternion QuaternionToAngleAxis(Quaternion quaternion, out float _angle, out Vector3 _axis)
		{
			_angle = 0f;
			_axis = Vector3.zero;
			_angle = 2f * Mathf.Acos(quaternion.w) * 57.29578f;
			_axis.x = quaternion.x / Mathf.Sqrt(1f - Mathf.Pow(quaternion.w, 2f));
			_axis.y = quaternion.y / Mathf.Sqrt(1f - Mathf.Pow(quaternion.w, 2f));
			_axis.z = quaternion.z / Mathf.Sqrt(1f - Mathf.Pow(quaternion.w, 2f));
			return quaternion;
		}

		// Token: 0x0600275B RID: 10075 RVA: 0x000B691C File Offset: 0x000B4B1C
		public static Vector3 Interpolate(Vector3 _from, Vector3 _to, float _weight)
		{
			_weight = Mathf.Clamp(_weight, 0f, 1f);
			return new Vector3((1f - _weight) * _from.x + _weight * _to.x, (1f - _weight) * _from.y + _weight * _to.y, (1f - _weight) * _from.z + _weight * _to.z);
		}

		// Token: 0x0600275C RID: 10076 RVA: 0x000B6985 File Offset: 0x000B4B85
		public static float VectorsAngle(Vector3 _v0, Vector3 _v1)
		{
			_v0.Normalize();
			_v1.Normalize();
			return Mathf.Acos(Mathf.Clamp(Vector3.Dot(_v0, _v1), -1f, 1f)) * 57.29578f;
		}

		// Token: 0x0600275D RID: 10077 RVA: 0x000B69B8 File Offset: 0x000B4BB8
		public static float QuaternionAngle(Quaternion _q1, Quaternion _q2)
		{
			float value = Quaternion.Dot(_q1, _q2);
			return 2f * Mathf.Acos(Mathf.Clamp01(value)) * 57.29578f;
		}

		// Token: 0x0600275E RID: 10078 RVA: 0x000B69E4 File Offset: 0x000B4BE4
		public static Quaternion RotateFromTo(Vector3 _source, Vector3 _target)
		{
			_source.Normalize();
			_target.Normalize();
			return Quaternion.Inverse(GenericMath.QuaternionFromAngleAxis(GenericMath.VectorsAngle(_source, _target), Vector3.Cross(_source, _target).normalized));
		}

		// Token: 0x0600275F RID: 10079 RVA: 0x000B6A20 File Offset: 0x000B4C20
		public static Vector3 TransformVector(Vector3 _v, Quaternion _q)
		{
			Quaternion qB = new Quaternion(_v.x, _v.y, _v.z, 0f);
			Quaternion quaternion = GenericMath.ApplyQuaternion(_q, qB);
			quaternion = GenericMath.ApplyQuaternion(quaternion, Quaternion.Inverse(_q));
			return new Vector3(quaternion.x, quaternion.y, quaternion.z);
		}

		// Token: 0x06002760 RID: 10080 RVA: 0x000B6A77 File Offset: 0x000B4C77
		public static Quaternion RotationLookAt(Vector3 _normal)
		{
			Quaternion identity = Quaternion.identity;
			return Util.QuaternionSafeLookRotation(-_normal);
		}

		// Token: 0x06002761 RID: 10081 RVA: 0x000B6A8C File Offset: 0x000B4C8C
		public static Vector3 GetLocalAxisToTarget(Transform self, Vector3 target)
		{
			Quaternion q = Quaternion.Inverse(self.rotation);
			return GenericMath.TransformVector((target - self.position).normalized, q);
		}

		// Token: 0x06002762 RID: 10082 RVA: 0x000B6AC0 File Offset: 0x000B4CC0
		public static bool ConeBounded(Core.Joint joint, Vector3 obj)
		{
			float num = GenericMath.VectorsAngle(obj - joint.pos, joint.pos + GenericMath.TransformVector(joint.axis, joint.rot));
			return joint.maxAngle >= num;
		}

		// Token: 0x06002763 RID: 10083 RVA: 0x000B6B08 File Offset: 0x000B4D08
		public static Vector3 GetConeNextPoint(Core.Joint joint, Vector3 obj)
		{
			if (GenericMath.ConeBounded(joint, obj))
			{
				return obj;
			}
			Vector3 pos = joint.pos;
			Vector3 v = obj - pos;
			Vector3 vector = GenericMath.TransformVector(joint.axis, joint.rot);
			float num = GenericMath.VectorsAngle(v, pos + vector);
			float num2 = Mathf.Cos(num * 0.017453292f) * v.magnitude;
			float d = num2 * (Mathf.Tan(num * 0.017453292f) - Mathf.Tan(joint.maxAngle * 0.017453292f));
			Vector3 vector2 = joint.joint.position + GenericMath.TransformVector(vector * num2, joint.rot) - obj;
			float f = Vector3.Dot(joint.joint.position + GenericMath.TransformVector(vector, joint.rot), v.normalized);
			return (vector2.normalized * d + obj) * Mathf.Clamp01(Mathf.Sign(f)) + pos * Mathf.Clamp01(-Mathf.Sign(f));
		}

		// Token: 0x06002764 RID: 10084 RVA: 0x000B6C1A File Offset: 0x000B4E1A
		public static float Clamp(float value, float min, float max)
		{
			if (value.CompareTo(min) < 0)
			{
				return min;
			}
			if (value.CompareTo(max) > 0)
			{
				return max;
			}
			return value;
		}
	}
}
