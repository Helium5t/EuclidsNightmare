using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
    {
    [SerializeField]
    private uint bounces = 100;
    [SerializeField]
    Color color = Color.red;
    [SerializeField]
    private float thickness = .5f;
    [SerializeField]
    private float alpha = .8f;

    [SerializeField]
    private Material laserMaterial;
    private Transform oldTransform = null; //to only update when transform changes
    private HashSet<LaserSensor> sensorsHit = new HashSet<LaserSensor>(); //to enter-leave sensors
    private List<Vector3> points = new List<Vector3>(); //to change line renderer coordinates

    private GameObject lrObject = null;
    private LineRenderer lr = null;
    private void Awake()
        {
        lrObject = new GameObject();
        lrObject.AddComponent<LineRenderer>();

        lr = lrObject.GetComponent<LineRenderer>();
        if(!laserMaterial){
            lr.material = Resources.Load<Material>("Materials/Laser");
        }
        else{
            lr.material = laserMaterial;
        }
        
        lr.startWidth = thickness;
        lr.endWidth = thickness;

        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = color;
        colorKeys[1].color = color;
        colorKeys[0].time = 0f;
        colorKeys[1].time = 1f;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = alpha;
        alphaKeys[1].alpha = alpha;
        alphaKeys[0].time = 0f;
        alphaKeys[1].time = 1f;
        lr.material.color = Color.red;
        lr.colorGradient.SetKeys(colorKeys, alphaKeys);
        }
    private void OnDestroy() { GameObject.Destroy(lrObject); }

    //In case someone needs to know the objects that have been hit from outside the class.
    public List<GameObject> objectsHit = new List<GameObject>();

    void Update()
        {
        //TODO understand why movement checks don't work
        /*if (oldTransform == null) { Debug.Log("a = false"); }
        else
            {
            Debug.Log(
         "b = " + ((transform.position - oldTransform.position).sqrMagnitude > float.Epsilon) +
         "\nc = " + (Quaternion.Angle(transform.rotation, oldTransform.rotation) > float.Epsilon));
            }*/

        /*if (oldTransform == null || 
            ((transform.position - oldTransform.position).sqrMagnitude > float.Epsilon) ||
            (Quaternion.Angle(transform.rotation, oldTransform.rotation) > float.Epsilon))*/
            {
            castRay(transform.position, transform.forward); 
            oldTransform = transform; 
            }
        }


	private void castRay(Vector3 position, Vector3 direction)
        {
        HashSet<LaserSensor> sensorsPrevious = new HashSet<LaserSensor>(sensorsHit);
        objectsHit.Clear();
        sensorsHit.Clear();
        points.Clear();
        points.Add(transform.position);

        for (int i = 0; i < bounces; i++)
			{
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, ~LayerMask.GetMask("Glass")))
                {
                GameObject obj = hit.collider.gameObject;
                objectsHit.Add(obj);
                points.Add(hit.point);

                //Sensors
                LaserSensor sensor = obj.GetComponent<LaserSensor>();
                if (sensor != null) { sensorsHit.Add(sensor); }

                if (obj.tag == "Mirror")
                    {
                    position = hit.point;
                    direction = Vector3.Reflect(direction, hit.normal);
                    }
                /*
                    //Now Deprecated since we use layers
                    else if (obj.tag == "Glass")
                    {
                    position = hit.point;
                    }
                */
                else { break; }
                }
            else { points.Add(position + direction * 100); break; }
            }

        //Sensors
        HashSet<LaserSensor> sensorsNew = new HashSet<LaserSensor>(sensorsHit);
        sensorsNew.ExceptWith(sensorsPrevious);
        sensorsPrevious.ExceptWith(sensorsHit);
        foreach (LaserSensor sensor in sensorsNew) { sensor.enter(); }
        foreach (LaserSensor sensor in sensorsPrevious) { sensor.leave(); }

        //Draw
        lr.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++) { lr.SetPosition(i, points[i]); }
        }

    }