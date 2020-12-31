using UnityEngine;

[CreateAssetMenu(fileName = "MazeTiles", menuName = "Maze Tiles", order = 0)]
public class MazeTiles : ScriptableObject
{
    public MazeTile TileNoWalls;
    public MazeTile TileAllWalls;

    public MazeTile TileOneWallN;
    public MazeTile TileOneWallE;
    public MazeTile TileOneWallS;
    public MazeTile TileOneWallW;

    public MazeTile TileTwoWallCornerES;
    public MazeTile TileTwoWallCornerNE;
    public MazeTile TileTwoWallCornerSW;
    public MazeTile TileTwoWallCornerWN;

    public MazeTile TileTwoWallHallEW;
    public MazeTile TileTwoWallHallNS;

    public MazeTile TileThreeWallESW;
    public MazeTile TileThreeWallNES;
    public MazeTile TileThreeWallSWN;
    public MazeTile TileThreeWallWNE;
}