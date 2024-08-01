using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YLayserSystem : MonoBehaviour
{
    public LazerProfile profile;
    
    public uint maxLazerCount=5;
    public float lazerLength;
    public float width;
    
    GraphicsBuffer commandBuf;
    GraphicsBuffer.IndirectDrawIndexedArgs[] commandData;
    const int commandCount = 1;

    void Start()
    {
        commandBuf = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, commandCount, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];
    }

    void OnDestroy()
    {
        commandBuf?.Release();
        commandBuf = null;
    }

    void Update()
    {
        RenderParams rp = new RenderParams(profile.LazerMaterial);
        rp.worldBounds = new Bounds(Vector3.zero, 10000*Vector3.one); // use tighter bounds for better FOV culling
        rp.matProps = new MaterialPropertyBlock();
        rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale));
        rp.matProps.SetFloat("_lazerLength", lazerLength);
        rp.matProps.SetFloat("width", width);
        commandData[0].indexCountPerInstance = profile.LazerMesh.GetIndexCount(0);
        commandData[0].instanceCount = maxLazerCount;
        commandBuf.SetData(commandData);
        Graphics.RenderMeshIndirect(rp, profile.LazerMesh, commandBuf, commandCount);
    }
}
