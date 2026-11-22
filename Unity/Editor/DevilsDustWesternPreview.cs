// Devil's Dust - isolated Unity 6 / URP prototype generator.
// Place under Assets/Editor. Menu: Tools > Devil's Dust > Create Western Preview.
// Generates an additive scene; never changes existing game scenes.
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class DevilsDustWesternPreview
{
    static Material sand, wood, darkWood, roof, rock, glass, cactus, road;
    static Material Mat(string name, Color color)
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (!shader) shader = Shader.Find("Standard");
        var m = new Material(shader) { name = "DD_" + name, color = color };
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
        return m;
    }
    static GameObject Box(Transform parent, string name, Vector3 pos, Vector3 size, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name; go.transform.SetParent(parent, false);
        go.transform.localPosition = pos; go.transform.localScale = size;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }
    static GameObject Root(string name, Transform parent = null)
    {
        var go = new GameObject(name);
        if (parent) go.transform.SetParent(parent, false);
        return go;
    }
    static void Building(Transform parent, string name, float x, float z, float width, float depth, float height, Material facade)
    {
        var root = Root(name, parent).transform;
        root.localPosition = new Vector3(x, 0, z);
        float front = x < 0 ? 1 : -1;
        Box(root, "Walls", new Vector3(0,height*.5f,0),new Vector3(width,height,depth),facade);
        Box(root, "Roof", new Vector3(0,height+.12f,0),new Vector3(width+1,.3f,depth+1),roof);
        Box(root, "False front",new Vector3(0,height+.9f,front*depth*.5f),new Vector3(width+1,1.7f,.35f),facade);
        Box(root, "Signboard",new Vector3(0,height+1,front*(depth*.5f+.2f)),new Vector3(width*.65f,.8f,.14f),darkWood);
        Box(root, "Door",new Vector3(0,1.3f,front*(depth*.5f+.13f)),new Vector3(1.7f,2.6f,.17f),darkWood);
        for (int i=-1;i<=1;i+=2)
        {
            Box(root,"Window glass",new Vector3(i*width*.31f,2.6f,front*(depth*.5f+.13f)),new Vector3(1.35f,1.35f,.18f),glass);
            Box(root,"Window mullion",new Vector3(i*width*.31f,2.6f,front*(depth*.5f+.25f)),new Vector3(.1f,1.4f,.1f),darkWood);
            Box(root,"Porch post",new Vector3(i*width*.44f,1.5f,front*(depth*.5f+2.3f)),new Vector3(.24f,3,.24f),darkWood);
        }
        Box(root,"Porch canopy",new Vector3(0,3.05f,front*(depth*.5f+1.2f)),new Vector3(width,.25f,2.7f),roof);
        Box(root,"Boardwalk",new Vector3(0,.17f,front*(depth*.5f+1.1f)),new Vector3(width+.5f,.3f,3.2f),wood);
        for(int i=0;i<11;i++)
            Box(root,"Plank seam",new Vector3(-width*.46f+i*width*.092f,.33f,front*(depth*.5f+1.1f)),new Vector3(.035f,.012f,3.05f),darkWood);
    }
    static void Cactus(Transform parent, float x, float z, float scale)
    {
        var t=Root("Saguaro",parent).transform; t.localPosition=new Vector3(x,0,z);
        Box(t,"Trunk",new Vector3(0,1.7f*scale,0),new Vector3(.55f,3.4f,.55f)*scale,cactus);
        Box(t,"Left arm",new Vector3(-.65f*scale,1.8f*scale,0),new Vector3(1.4f,.38f,.42f)*scale,cactus);
        Box(t,"Left tip",new Vector3(-1.15f*scale,2.25f*scale,0),new Vector3(.4f,1.2f,.4f)*scale,cactus);
        Box(t,"Right arm",new Vector3(.7f*scale,2.4f*scale,0),new Vector3(1.4f,.38f,.42f)*scale,cactus);
        Box(t,"Right tip",new Vector3(1.2f*scale,2.85f*scale,0),new Vector3(.4f,1.2f,.4f)*scale,cactus);
    }
    static void Mesa(Transform parent, Vector3 pos, float width, float height)
    {
        var t=Root("Layered mesa",parent).transform; t.localPosition=pos;
        Box(t,"Base",new Vector3(0,height*.28f,0),new Vector3(width*1.15f,height*.56f,width*.75f),rock);
        Box(t,"Upper strata",new Vector3(0,height*.68f,0),new Vector3(width,height*.25f,width*.65f),roof);
        Box(t,"Cap",new Vector3(0,height*.86f,0),new Vector3(width*.92f,height*.12f,width*.61f),rock);
    }
    [MenuItem("Tools/Devil's Dust/Create Western Preview")]
    public static void Create()
    {
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
        scene.name="DD_Western_Preview_Unsaved";
        sand=Mat("Sand",new Color(.64f,.41f,.23f));
        road=Mat("Road",new Color(.46f,.29f,.17f));
        wood=Mat("Sunbleached wood",new Color(.56f,.36f,.22f));
        darkWood=Mat("Dark wood",new Color(.22f,.13f,.09f));
        roof=Mat("Roof",new Color(.32f,.19f,.15f));
        rock=Mat("Red rock",new Color(.48f,.26f,.17f));
        glass=Mat("Dusty windows",new Color(.16f,.25f,.26f));
        cactus=Mat("Cactus",new Color(.16f,.31f,.18f));
        var world=Root("DevilsDust_WesternPreview").transform;
        SceneManager.MoveGameObjectToScene(world.gameObject,scene);
        Box(world,"Desert floor",new Vector3(0,-.45f,0),new Vector3(260,.8f,260),sand);
        Box(world,"Main street",new Vector3(0,-.035f,0),new Vector3(12,.04f,112),road);
        var names=new[]{"SALOON","GENERAL STORE","SHERIFF","HOTEL","STABLE","BANK"};
        for(int i=0;i<6;i++)
        {
            float x=i%2==0?-12:12,z=(i/2-1)*25;
            Building(world,names[i],x,z,10,9,6+i%3, i%2==0?wood:sand);
        }
        var rng=new System.Random(2607);
        for(int i=0;i<70;i++)
        {
            float x=(float)(rng.NextDouble()-.5)*210,z=(float)(rng.NextDouble()-.5)*210;
            if(Mathf.Abs(x)<26&&Mathf.Abs(z)<49)continue;
            if(i%3==0)Cactus(world,x,z,.7f+(float)rng.NextDouble()*.7f);
            else Box(world,"Desert boulder",new Vector3(x,.35f,z),new Vector3(1.3f,.7f,1.5f),rock);
        }
        for(int i=0;i<16;i++)
        {
            float a=i*Mathf.PI*2/16,r=92+i%3*7;
            Mesa(world,new Vector3(Mathf.Cos(a)*r,0,Mathf.Sin(a)*r),10+i%4*3,12+i%5*3);
        }
        var sun=Root("Golden hour sun",world);
        var light=sun.AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.4f;
        light.color=new Color(1f,.78f,.56f);light.shadows=LightShadows.Soft;
        sun.transform.rotation=Quaternion.Euler(32,-48,0);
        var cameraGo=Root("Preview camera",world);
        var camera=cameraGo.AddComponent<Camera>();camera.clearFlags=CameraClearFlags.Skybox;
        cameraGo.transform.position=new Vector3(44,32,62);
        cameraGo.transform.LookAt(new Vector3(0,3,0));
        camera.fieldOfView=55;camera.farClipPlane=350;
        RenderSettings.ambientLight=new Color(.53f,.45f,.39f);
        Selection.activeGameObject=world.gameObject;
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Devil's Dust preview created in a separate UNSAVED additive scene. Save it separately if approved.");
    }
}
#endif