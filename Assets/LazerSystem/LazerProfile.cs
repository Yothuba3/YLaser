using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomLazerProfile", menuName = "ScriptableObjects/LazerProfile", order = 1)]

public class LazerProfile : ScriptableObject
{
    [SerializeField]private Material material;
    [SerializeField]private Mesh lazerMesh;

    public Material LazerMaterial => material;
    public Mesh LazerMesh => lazerMesh;
}
