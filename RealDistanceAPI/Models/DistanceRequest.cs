public class DistanceRequest
{
    public string Origin { get; set; }
    public string Destination { get; set; }

    //public enum Modes { Driving, Walking, Bicycling, Transit }
    public string Mode { get; set; }
}