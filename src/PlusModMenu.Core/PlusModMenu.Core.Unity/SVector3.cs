using System.Runtime.Serialization;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

[DataContract]
public class SVector3
{
	[DataMember(Name = "values")]
	private float[] _vector3 = new float[3];

	public Vector3 Vector3
	{
		get
		{
			return new Vector3(_vector3[0], _vector3[1], _vector3[2]);
		}
		set
		{
			_vector3 = new float[3] { value.x, value.y, value.z };
		}
	}

	public SVector3(Vector3 vector3)
	{
		Vector3 = vector3;
	}
}
