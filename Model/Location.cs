namespace Model {
    public class Location {

        public int LocationId { get; set; }
        public string LocationName { get; set; }

        public Location() { }

        public Location(int locationId, string locationName) {
            LocationId = locationId;
            LocationName = locationName;
        }
    }
}
