using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GeneratorRoom : MonoBehaviour
    {
    [SerializeField]
    public GameObject wallPrefab = null;
    private float individualHeight = 0;

    [SerializeField]
    public int amountY = 0;
    [SerializeField]
    public int amountX = 0;
    [SerializeField]
    public int amountZ = 0;
    private List<GameObject> pieces = new List<GameObject>();

    [SerializeField]
    private bool updateButton = false;

    [SerializeField]
    private GameObject generatorWall4Prefab = null;

    public float width() { if (pieces.Count != 0) { return pieces[0].GetComponent<GeneratorWall4>().width(); } else { return 0; } }
    public float depth() { if (pieces.Count != 0) { return pieces[0].GetComponent<GeneratorWall4>().depth(); } else { return 0; } }
    public float height() { return amountY * individualHeight; }

    private void Update() { if (updateButton) { updateButton = false; rebuild(); } }

    public void rebuild() { clear(); update(); build(); }

    void clear() { foreach (var piece in pieces) { GameObject.DestroyImmediate(piece); } pieces.Clear(); }
    private void update()
        {
        if (wallPrefab != null)
            {
            GameObject tmp = GameObject.Instantiate(wallPrefab);
            individualHeight = tmp.GetComponent<Collider>().bounds.size.y;
            GameObject.DestroyImmediate(tmp);
            }
        else { individualHeight = 0; }
        }
    void build()
        {
        if (wallPrefab != null)
            {
            float initialPosition = -height() / 2 + individualHeight / 2f;
            for (int i = 0; i < amountY; i++)
                {
                GameObject tmp = GameObject.Instantiate(generatorWall4Prefab);
                tmp.transform.parent = gameObject.transform;
                tmp.transform.localPosition = new Vector3(0, i * individualHeight + initialPosition, 0);
                tmp.transform.localRotation = Quaternion.identity;
                GeneratorWall4 gen = tmp.GetComponent<GeneratorWall4>();
                gen.amountX = amountX;
                gen.amountZ = amountZ;
                gen.wallPrefab = wallPrefab;
                gen.rebuild();
                pieces.Add(tmp);
                }
            }
        }
    }
