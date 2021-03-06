
public enum NeighborType {
    LeftRight, TopBottom
}

/// <summary>
/// Neighboring relationship between two MapData objects.
/// </summary>
public class MapNeighborRelation { 
    // Class instead of struct because we want to point to the same relation object from both members
    public MapData firstMember, secondMember;
    public NeighborType neighborType;
    private DisplayNeighborRelation displayNeighborRelation;
    public MapNeighborRelation(MapData firstMember, MapData secondMember, NeighborType neighborType) {
        this.firstMember = firstMember;
        this.secondMember = secondMember;
        this.neighborType = neighborType;
    }

    public DisplayNeighborRelation AsDisplayNeighborRelation() {
        if(displayNeighborRelation == null) { // This makes sure that we are pointing at the same object from both members even after conversion
            displayNeighborRelation = new DisplayNeighborRelation(firstMember, secondMember, neighborType);
        }
        return displayNeighborRelation;
    }

    public MapData GetOther(MapData member) {
        if(member == firstMember) return secondMember;
        else if (member == secondMember) return firstMember;
        else throw new System.ArgumentException("Tried to GetOther for non-existing member!");
    }

    public int GetOtherWidth(MapData member) {
        return GetOther(member).GetWidth();
    }

    public int GetOtherHeight(MapData member) {
        return GetOther(member).GetHeight();
    }

    public bool IsFirstMember(MapData member) {
        if(member == firstMember) return true;
        else if (member == secondMember) return false;
        else throw new System.ArgumentException("Tried to IsFirstMember for non-existing member!");
    }
}
