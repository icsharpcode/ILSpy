using System;
[Serializable]
public class Material
{
	public static implicit operator bool(Material m)
	{
		return m == null;
	}
}
[Serializable]
public class SequenceOfNestedIfs
{
	public bool _clear;
	public Material _material;
	public virtual bool CheckShader()
	{
		return false;
	}
	public virtual void CreateMaterials()
	{
		if (!_clear)
		{
			if (!CheckShader())
			{
				return;
			}
			_material = new Material();
		}
		if (!_material)
		{
			if (!CheckShader())
			{
				return;
			}
			_material = new Material();
		}
		if (!_material)
		{
			if (!CheckShader())
			{
				return;
			}
			_material = new Material();
		}
		if (!_material && CheckShader())
		{
			_material = new Material();
		}
	}
}
