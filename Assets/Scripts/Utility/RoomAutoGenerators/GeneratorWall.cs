using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GeneratorWall : MonoBehaviour
    {
    [SerializeField]
    public GameObject wallPrefab = null;
    private float individualWidth = 0;

    [SerializeField]
    public int amount = 0;
    private List<GameObject> pieces = new List<GameObject>();

    [SerializeField]
    private bool updateButton = false;

    public float width() { return amount * individualWidth; }

	private void Update() { if (updateButton) { updateButton = false; rebuild(); } }

    public void rebuild() { clear(); update(); build(); }

    void clear() { foreach (var piece in pieces) { GameObject.DestroyImmediate(piece); } pieces.Clear(); }
    private void update()
        {
        if (wallPrefab != null) 
            {
            GameObject tmp = GameObject.Instantiate(wallPrefab);
            individualWidth = tmp.GetComponent<Collider>().bounds.size.x;
            GameObject.DestroyImmediate(tmp);
            }
        else { individualWidth = 0; }
        }
	void build()
        {
        if (wallPrefab != null)
            {
            float initialPosition = -width()/2 + individualWidth / 2f;
            for (int i = 0; i < amount; i++)
                {
                GameObject tmp = GameObject.Instantiate(wallPrefab);
                tmp.transform.parent = gameObject.transform;
                tmp.transform.localPosition = new Vector3(i * individualWidth + initialPosition, 0, 0);
                tmp.transform.localRotation = Quaternion.identity;
                pieces.Add(tmp);
                }
            }
        }
    }