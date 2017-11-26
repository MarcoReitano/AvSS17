using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class UnityObjectSurrogate : ISerializationSurrogate {

    private KeyValuePairListsDictionary<string, UnityEngine.Object> serializedObjects = new KeyValuePairListsDictionary<string, UnityEngine.Object>();

    public UnityObjectSurrogate(KeyValuePairListsDictionary<string, UnityEngine.Object> serializedObjects)
    {
        this.serializedObjects = serializedObjects;
    }

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        string fieldName = context.Context as string;
        serializedObjects[fieldName] = obj as UnityEngine.Object;
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        string fieldName = context.Context as string;
        return serializedObjects[fieldName];
    }

}

/***

Supported types (if it really works):

UnityEngine.AssetBundle
UnityEngine.AssetBundleManifest
UnityEngine.ScriptableObject
UnityEngine.Behaviour
UnityEngine.BillboardAsset
UnityEngine.BillboardRenderer
UnityEngine.Camera
UnityEngine.Component
UnityEngine.ComputeShader
UnityEngine.FlareLayer
UnityEngine.GameObject
UnityEngine.OcclusionArea
UnityEngine.OcclusionPortal
UnityEngine.RenderSettings
UnityEngine.QualitySettings
UnityEngine.MeshFilter
UnityEngine.SkinnedMeshRenderer
UnityEngine.Flare
UnityEngine.LensFlare
UnityEngine.Renderer
UnityEngine.Projector
UnityEngine.Skybox
UnityEngine.TrailRenderer
UnityEngine.LineRenderer
UnityEngine.LightProbes
UnityEngine.LightmapSettings
UnityEngine.MeshRenderer
UnityEngine.GUIElement
UnityEngine.GUITexture
UnityEngine.GUILayer
UnityEngine.Light
UnityEngine.LightProbeGroup
UnityEngine.LightProbeProxyVolume
UnityEngine.LODGroup
UnityEngine.Mesh
UnityEngine.MonoBehaviour
UnityEngine.Motion
UnityEngine.NetworkView
UnityEngine.RectTransform
UnityEngine.ReflectionProbe
UnityEngine.Rendering.GraphicsSettings
UnityEngine.Shader
UnityEngine.Material
UnityEngine.ShaderVariantCollection
UnityEngine.Sprite
UnityEngine.SpriteRenderer
UnityEngine.ProceduralMaterial
UnityEngine.ProceduralTexture
UnityEngine.TextAsset
UnityEngine.Texture
UnityEngine.Texture2D
UnityEngine.Cubemap
UnityEngine.Texture3D
UnityEngine.Texture2DArray
UnityEngine.CubemapArray
UnityEngine.SparseTexture
UnityEngine.RenderTexture
UnityEngine.Transform
UnityEngine.Object
UnityEngine.Experimental.Director.DirectorPlayer
UnityEngine.WindZone
UnityEngine.ParticleSystem
UnityEngine.ParticleSystemRenderer
UnityEngine.Rigidbody
UnityEngine.Joint
UnityEngine.HingeJoint
UnityEngine.SpringJoint
UnityEngine.FixedJoint
UnityEngine.CharacterJoint
UnityEngine.ConfigurableJoint
UnityEngine.ConstantForce
UnityEngine.Collider
UnityEngine.BoxCollider
UnityEngine.SphereCollider
UnityEngine.MeshCollider
UnityEngine.CapsuleCollider
UnityEngine.PhysicMaterial
UnityEngine.CharacterController
UnityEngine.CircleCollider2D
UnityEngine.BoxCollider2D
UnityEngine.Joint2D
UnityEngine.AreaEffector2D
UnityEngine.PlatformEffector2D
UnityEngine.Rigidbody2D
UnityEngine.Collider2D
UnityEngine.EdgeCollider2D
UnityEngine.CapsuleCollider2D
UnityEngine.PolygonCollider2D
UnityEngine.AnchoredJoint2D
UnityEngine.SpringJoint2D
UnityEngine.DistanceJoint2D
UnityEngine.FrictionJoint2D
UnityEngine.HingeJoint2D
UnityEngine.RelativeJoint2D
UnityEngine.SliderJoint2D
UnityEngine.TargetJoint2D
UnityEngine.FixedJoint2D
UnityEngine.WheelJoint2D
UnityEngine.PhysicsMaterial2D
UnityEngine.PhysicsUpdateBehaviour2D
UnityEngine.ConstantForce2D
UnityEngine.Effector2D
UnityEngine.BuoyancyEffector2D
UnityEngine.PointEffector2D
UnityEngine.SurfaceEffector2D
UnityEngine.WheelCollider
UnityEngine.Cloth
UnityEngine.AI.NavMeshAgent
UnityEngine.AI.NavMeshObstacle
UnityEngine.AI.OffMeshLink
UnityEngine.AudioClip
UnityEngine.AudioListener
UnityEngine.AudioSource
UnityEngine.AudioReverbZone
UnityEngine.AudioLowPassFilter
UnityEngine.AudioHighPassFilter
UnityEngine.AudioDistortionFilter
UnityEngine.AudioEchoFilter
UnityEngine.AudioChorusFilter
UnityEngine.AudioReverbFilter
UnityEngine.Audio.AudioMixer
UnityEngine.Audio.AudioMixerSnapshot
UnityEngine.Audio.AudioMixerGroup
UnityEngine.MovieTexture
UnityEngine.WebCamTexture
UnityEditorInternal.Transition
UnityEditorInternal.StateMachine
UnityEditorInternal.State
UnityEditorInternal.AnimatorController
UnityEditorInternal.BlendTree
UnityEngine.AnimatorOverrideController
UnityEngine.AnimationClip
UnityEngine.Animation
UnityEngine.Animator
UnityEngine.RuntimeAnimatorController
UnityEngine.Avatar
UnityEngine.TerrainData
UnityEngine.Terrain
UnityEngine.Tree
UnityEngine.SpeedTreeWindAsset
UnityEngine.GUIText
UnityEngine.TextMesh
UnityEngine.Font
UnityEngine.Canvas
UnityEngine.CanvasGroup
UnityEngine.CanvasRenderer
UnityEngine.TerrainCollider
UnityEngine.GUISkin
UnityEngine.Networking.Match.NetworkMatch
UnityEngine.VR.WSA.WorldAnchor
UnityEngine.ParticleEmitter
UnityEngine.EllipsoidParticleEmitter
UnityEngine.MeshParticleEmitter
UnityEngine.ParticleAnimator
UnityEngine.ParticleRenderer
UnityEngine.StateMachineBehaviour
UnityEngine.UserAuthorizationDialog

**/
