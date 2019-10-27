public struct NoGrowthReason {
	public bool notEnoughNeighbourEnergy; // all neighbours have not collected enough energy to collectivly build the cell
	public bool waitingForRespawnCooldown; // this cell has existed and died, we need to wait a while before trying to respawn it
	public bool tooFarAwayFromNeighbours; // the spawn location is in the middle (average) of all neighbour positions. It can't be spawned if the gap to one or more of its neighbours is too big. This happens when the pose is wonky

	public bool spaceIsOccupied; // other creatures' animal cell (if attached not child origin or mother placenta) (Terrain still TODO)
	public bool spaceIsOccupiedByChildOrigin;
	public bool spaceIsOccupiedByMotherPlacenta;
	public bool fullyGrown; // creature has built all its cells, thus reached it's max size
}
