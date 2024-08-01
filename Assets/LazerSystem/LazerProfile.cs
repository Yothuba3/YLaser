using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomLazerProfile", menuName = "ScriptableObjects/LazerProfile", order = 1)]

public class LazerProfile : ScriptableObject
{
    [SerializeField]private Material material;
    [SerializeField]private Mesh lazerMesh;
    [SerializeField] private ComputeShader _computeShader;
    public Material LazerMaterial => material;
    public Mesh LazerMesh => lazerMesh;
    public ComputeShader ComputeShader => _computeShader;

    public bool ValidateCheck()
    {
        return material && lazerMesh && _computeShader;
    }
}
