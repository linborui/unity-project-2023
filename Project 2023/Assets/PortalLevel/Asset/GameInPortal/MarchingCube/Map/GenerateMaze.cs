using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//----CONSTANTS-------------------------------------------------------

//----GLOBAL VARIABLES------------------------------------------------

public class GenerateMaze : MonoBehaviour
{
    int GRID_WIDTH = 23;
    int GRID_HEIGHT =  23;
    int NORTH =0;
    int EAST =1;
    int SOUTH =2;
    int WEST =3;
    char []grid;
    public GameObject wall;
    public GameObject tunnel;
    // Start is called before the first frame update
    void Start()
    {
        grid= new char[GRID_WIDTH*GRID_HEIGHT];
        ResetGrid();
        Visit(1,1);
        PrintGrid();

    }
    void ResetGrid()
    {
        // Fills the grid with walls ('#' characters).
        for (int i=0; i<GRID_WIDTH*GRID_HEIGHT; ++i)
        {
            grid[i] = 'B';
        }
    }
    int XYToIndex( int x, int y )
    {
        // Converts the two-dimensional index pair (x,y) into a
        // single-dimensional index. The result is y * ROW_WIDTH + x.
        return y * GRID_WIDTH + x;
    }
    bool IsInBounds( int x, int y )
    {
        // Returns "true" if x and y are both in-bounds.
        if (x < 0 || x >= GRID_WIDTH) return false;
        if (y < 0 || y >= GRID_HEIGHT) return false;
        return true;
    }
// This is the recursive function we will code in the next project
    void Visit( int x, int y )
    {
        // Starting at the given index, recursively visits every direction in a
        // randomized order.
        // Set my current location to be an empty passage.
        grid[ XYToIndex(x,y) ] = 'O';
        // Create an local array containing the 4 directions and shuffle their order.
        int []dirs = new int[4];
        dirs[0] = NORTH;
        dirs[1] = EAST;
        dirs[2] = SOUTH;
        dirs[3] = WEST;
        for (int i=0; i<4; ++i)
        {
            int r = Random.Range(0, 4); 
            int temp = dirs[r];
            dirs[r] = dirs[i];
            dirs[i] = temp;
        }
        // Loop through every direction and attempt to Visit that direction.
        for (int i=0; i<4; ++i)
        {
            // dx,dy are offsets from current location. Set them based
            // on the next direction I wish to try.
            int dx=0, dy=0;
            switch (dirs[i])
            {
                case 0: dy = -1; break;
                case 1: dy = 1; break;
                case 2: dx = 1; break;
                case 3: dx = -1; break;
            }
            // Find the (x,y) coordinates of the grid cell 2 spots
            // away in the given direction.
            int x2 = x + (dx<<1);
            int y2 = y + (dy<<1);
            if (IsInBounds(x2,y2))
            {
                if (grid[ XYToIndex(x2,y2) ] == 'B')
                {
                    // (x2,y2) has not been visited yet... knock down the
                    // wall between my current position and that position
                    grid[ XYToIndex(x2-dx,y2-dy) ] = 'O';
                    // Recursively Visit (x2,y2)
                    Visit(x2,y2);
                }
            }
        }
    }
    void PrintGrid()
    {
        
        // Displays the finished maze to the screen.
        for (int y=0; y<GRID_HEIGHT; ++y)
        {
            for (int x=0; x<GRID_WIDTH; ++x)
            {
                if(grid[XYToIndex(x,y)] == 'B' && !(x==1 && y==0)){
                    //Instantiate inside the parent
                    //Debug.Log(transform.position);
                    GameObject cube = Instantiate(wall, transform.position + new Vector3(x*11, 0, -y*11), Quaternion.identity);
                    cube.GetComponentInParent<Transform>().SetParent(transform);
                }
                else{
                    GameObject cube = Instantiate(tunnel, transform.position + new Vector3(x*11, 0, -y*11), Quaternion.identity);
                    cube.GetComponentInParent<Transform>().SetParent(transform);
                }
                GameObject cubeup = Instantiate(wall, transform.position + new Vector3(x*11, 10, -y*11), Quaternion.identity);
                cubeup.GetComponentInParent<Transform>().SetParent(transform);
                GameObject cubedown = Instantiate(wall, transform.position + new Vector3(x*11, -10, -y*11), Quaternion.identity);
                cubedown.GetComponentInParent<Transform>().SetParent(transform);
            }

        }
        }
}
