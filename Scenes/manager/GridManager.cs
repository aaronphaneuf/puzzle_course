using Godot;
using System.Collections.Generic;
using System.Linq;
using Game.Component;

namespace Game.Manager;
public partial class GridManager : Node
{

	private HashSet<Vector2I> occupiedCells = new();

	[Export]
	private TileMapLayer highlightTilemapLayer;
	[Export]
	private TileMapLayer baseTerrainTilemapLayer;

	public bool IsTilePositionValid(Vector2I tilePosition)
	{
		
		var customData = baseTerrainTilemapLayer.GetCellTileData(tilePosition);
		if (customData == null) return false;
		if (!(bool)customData.GetCustomData("buildable")) return false;

		return !occupiedCells.Contains(tilePosition);
	}
	public void MarkTileAsOccupied(Vector2I tilePosition)
	{
		occupiedCells.Add(tilePosition);
	}
	
	public void HighlightBuildableTiles()
	{
		ClearHighlightedTiles();
		var buildingComponents = GetTree().GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>();
		foreach (var buildingComponent in buildingComponents)
		{ 
			HighlightValidTilesInRadius(buildingComponent.GetGridCellPosition(), buildingComponent.BuildableRadius);
		}
	}

	private void HighlightValidTilesInRadius(Vector2I rootCell, int radius) { 
		

		for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
		{
			for (var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++)
			{
				var tilePosition = new Vector2I(x, y);
				if (!IsTilePositionValid(tilePosition)) continue;
				highlightTilemapLayer.SetCell(tilePosition, 0, Vector2I.Zero);
			}
		}
	}

	public void ClearHighlightedTiles() { 
		highlightTilemapLayer.Clear();
	}

	public Vector2I GetMouseGridCellPosition()
	{
		var mousePosition = highlightTilemapLayer.GetGlobalMousePosition();
		var gridPosition = mousePosition / 64;
		gridPosition = gridPosition.Floor();
		return new Vector2I((int)gridPosition.X, (int)gridPosition.Y);
	}

}
