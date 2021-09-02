using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class TetrisBlock : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime = 0.8f;
    public static int height = 30;
    public static int width = 40;
    public GameObject square1;
    public GameObject square2;
    public GameObject square3;
    public GameObject square4;
    private static Transform[,] grid = new Transform[width, height];
    public int rowsDeleted = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //private void FixedUpdate()
    //{
    //    if(Input.GetKey(KeyCode.LeftArrow))
    //    {
    //        transform.position += new Vector3(-1, 0, 0);
    //        if (!ValidMove())
    //            transform.position -= new Vector3(-1, 0, 0);
    //    }
    //    else if (Input.GetKey(KeyCode.RightArrow))
    //    {
    //        transform.position += new Vector3(1, 0, 0);
    //        if (!ValidMove())
    //            transform.position -= new Vector3(1, 0, 0);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!ValidMove())
                transform.position -= new Vector3(-1, 0, 0);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!ValidMove())
                transform.position -= new Vector3(1, 0, 0);
        }
        else
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Rotate piece, keep orientation of squares the same
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            square1.transform.eulerAngles += new Vector3(0, 0, -90);
            square2.transform.eulerAngles += new Vector3(0, 0, -90);
            square3.transform.eulerAngles += new Vector3(0, 0, -90);
            square4.transform.eulerAngles += new Vector3(0, 0, -90);
            if (!ValidMove())
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
                square1.transform.eulerAngles += new Vector3(0, 0, 90);
                square2.transform.eulerAngles += new Vector3(0, 0, 90);
                square3.transform.eulerAngles += new Vector3(0, 0, 90);
                square4.transform.eulerAngles += new Vector3(0, 0, 90);
            }
        }

        if(Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
        {
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLines();
                this.enabled = false;
                FindObjectOfType<SpawnTetromino>().NewTetromino();
            }
            previousTime = Time.time;
        }
    }

    void CheckForLines()
    {
        for (int i = height-1; i >= 0; i--)
        {
            if(HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
                ScoreScript.scoreValue += rowsDeleted * 10;
            }
        }
        rowsDeleted = 0;
    }

    bool HasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null)
                return false;
        }
        return true;
    }

    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }
        rowsDeleted++;
    }

    void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j,y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX, roundedY] = children;
        }
    }

    bool ValidMove()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if(roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
                return false;
        }
        return true;
    }
}