using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;


public class YLayserSystem : MonoBehaviour
{
    public LazerProfile profile;
    
    public uint maxLazerCount=5;
    public float lazerLength;
    public float width;
    public float fxWidth;
    public uint fxMode=0;
    
    private int _kernel;
    private ComputeShader cs;
    
    GraphicsBuffer commandBuf;
    GraphicsBuffer.IndirectDrawIndexedArgs[] commandData;

    const int commandCount = 1;

    private static readonly int BufferID = Shader.PropertyToID("buffers");
    private static readonly int fxModeID = Shader.PropertyToID("fxMode");
    private static readonly int fxWidthID = Shader.PropertyToID("fxWidth");
    private static readonly int LazerCountID = Shader.PropertyToID("lazerCount");
    
    private static readonly int posBuf = Shader.PropertyToID("_Positions");
    void Start()
    {
        //if (!profile.ValidateCheck()) return;
        cs = profile.ComputeShader;
        _kernel = cs.FindKernel("CSMain");
        commandBuf = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, commandCount, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];

        if (commandBuf == null)
        {
            commandBuf = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, commandCount, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        }

        if (commandData == null)
        {
            commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];
        }
    }

    void OnDestroy()
    {
        commandBuf?.Release();
        commandBuf = null;
    }

    void Update()
    {
        if (!profile.ValidateCheck()) return;
                _kernel = cs.FindKernel("CSMain");

        cs.SetInt(LazerCountID,(int)maxLazerCount);
        cs.SetInt(fxModeID, (int)fxMode);
        cs.SetFloat(fxWidthID, fxWidth);
        cs.SetVector("_Time", Shader.GetGlobalVector ("_Time"));
        
        var buffer = new ComputeBuffer((int)maxLazerCount, Marshal.SizeOf(typeof(float3)));
        cs.SetBuffer(_kernel, BufferID, buffer);
        
        uint sizeX, sizeY, sizeZ;
        profile.ComputeShader.GetKernelThreadGroupSizes(
            _kernel,
            out sizeX,
            out sizeY,
            out sizeZ
        );
        var threadGroupX = (int)System.Math.Ceiling(maxLazerCount / (float)sizeX);
        
        cs.Dispatch(_kernel, threadGroupX,1,1);
        
        var result = new float3[maxLazerCount];
        buffer.GetData(result);
        var positions = result;
        var positionBuffer =
          new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)maxLazerCount, Marshal.SizeOf<float3>());
        positionBuffer.SetData(positions);
        
        RenderParams rp = new RenderParams(profile.LazerMaterial);
        rp.worldBounds = new Bounds(Vector3.zero, 10000*Vector3.one); // use tighter bounds for better FOV culling
        
        rp.matProps = new MaterialPropertyBlock();
        
        rp.matProps.SetBuffer(posBuf,positionBuffer);
        rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale));
        rp.matProps.SetFloat("_lazerLength", lazerLength);
        rp.matProps.SetFloat("width", width);
        commandData[0].indexCountPerInstance = profile.LazerMesh.GetIndexCount(0);
        commandData[0].instanceCount = maxLazerCount;
        commandBuf.SetData(commandData);
        Graphics.RenderMeshIndirect(rp, profile.LazerMesh, commandBuf, commandCount);
        buffer.Release();
        positionBuffer.Release();
    }
}
