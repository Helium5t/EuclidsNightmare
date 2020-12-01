using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GeneratorWall4 : MonoBehaviour
    {
    [SerializeField]
    public GameObject wallPrefab = null;

    [SerializeField]
    public int amountX = 0;
    [SerializeField]
    public int amountZ = 0;

    [SerializeField]
    private bool updateButton = false;

    [SerializeField]
    private GameObject objectWallRR = null;
    [SerializeField]
    private GameObject objectWallLL = null;
    [SerializeField]
    private GameObject objectWallBK = null;
    [SerializeField]
    private GameObject objectWallFD = null;

    public float width() { return objectWallBK.GetComponent<GeneratorWall>().width(); }
    public float depth() { return objectWallLL.GetComponent<GeneratorWall>().width(); }

    private void Update() { if (updateButton) { updateButton = false; rebuild(); } }

    public void rebuild() 
        { 
        GeneratorWall wallRR = objectWallRR.GetComponent<GeneratorWall>();
        GeneratorWall wallLL = objectWallLL.GetComponent<GeneratorWall>();
        GeneratorWall wallBK = objectWallBK.GetComponent<GeneratorWall>();
        GeneratorWall wallFD = objectWallFD.GetComponent<GeneratorWall>();
        wallRR.wallPrefab = wallPrefab;
        wallLL.wallPrefab = wallPrefab;
        wallBK.wallPrefab = wallPrefab;
        wallFD.wallPrefab = wallPrefab;
        wallRR.amount = amountZ;
        wallLL.amount = amountZ;
        wallBK.amount = amountX;
        wallFD.amount = amountX;
        wallRR.rebuild();
        wallLL.rebuild();
        wallBK.rebuild();
        wallFD.rebuild();

        objectWallRR.transform.localPosition = new Vector3(width()/2, 0, 0);
        objectWallRR.transform.localRotation = Quaternion.Euler(0, 270, 0);
        objectWallLL.transform.localPosition = new Vector3(-width()/2, 0, 0);
        objectWallLL.transform.localRotation = Quaternion.Euler(0, 90, 0);
        objectWallFD.transform.localPosition = new Vector3(0, 0, -depth()/2);
        objectWallFD.transform.localRotation = Quaternion.Euler(0, 0, 0);
        objectWallBK.transform.localPosition = new Vector3(0, 0, depth()/2);
        objectWallBK.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
