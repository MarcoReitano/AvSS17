using System;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// Singleton for OSM-Data-Parse-Statistics
/// </summary>
public class OSMStatistics
{


    string filePath;

    public string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }


    #region statistics variables

    public static int
        nodeCount = 0,
        highwayTagCount = 0,

        wayCount = 0,
        wayHighwayTagCount = 0,
        wayNameTagCount = 0,
        laneInformation = 0,

        relationCount = 0,
        emptyElementCount = 0,
        nodeWithoutTags = 0,

        highway_motorway = 0,
        highway_motorway_link = 0,
        highway_trunk = 0,
        highway_trunk_link = 0,
        highway_primary = 0,
        highway_primary_link = 0,
        highway_secondary = 0,
        highway_secondary_link = 0,
        highway_tertiary = 0,
        highway_residential = 0,
        highway_unclassified = 0,


        highway_mini_roundabout = 0,
        highway_stop = 0,
        highway_give_way = 0,
        highway_traffic_signals = 0,
        highway_crossing = 0,
        highway_bus_stop = 0,
        highway_platform = 0,
        highway_turning_circle = 0,
        highway_motorway_junction = 0,

        railway_station = 0,
        railway_halt = 0,
        railway_tram_stop = 0,
        railway_crossing = 0,
        railway_subway_entrance = 0,
        railway_level_crossing = 0,

        amenity_bus_station = 0,
        amenity_taxi = 0,
        amenity_atm = 0,
        amenity_hospital = 0,
        amenity_police = 0,
        amenity_post_box = 0,
        amenity_post_office = 0,
        amenity_telephone = 0,
        amenity_toilets = 0,

        shop_bakery = 0,
        shop_beauty = 0,
        shop_beverages = 0,
        shop_books = 0,
        shop_boutique = 0,
        shop_clothes = 0,
        shop_dry_cleaning = 0,
        shop_electronics = 0,
        shop_florist = 0,
        shop_ice_cream = 0,
        shop_mall = 0,
        shop_supermarket = 0,
        shop_shoes = 0,

        tourism_hostel = 0,
        tourism_hotel = 0,
        tourism_information = 0,

        natural_bay = 0,
        natural_beach = 0,

        sport_swimming = 0,
        sport_soccer = 0,
        sport_volleyball = 0,
        sport_skateboard = 0,
        sport_basketball = 0,

        bridge = 0,

        crossing_no = 0,
        crossing_traffic_signals = 0,
        crossing_uncontrolled = 0;
    #endregion

    private OSMStatistics() { }
    private static OSMStatistics instance = null;

    public static OSMStatistics getInstance()
    {
        if (instance == null) instance = new OSMStatistics();
        return instance;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static string GetStatisticsString()
    {
        string tab = "   ";
        StringBuilder sb = new StringBuilder();
        sb.Append("* Print statistics... \n");
        sb.Append("\tDie Datei: " + OSMStatistics.getInstance().FilePath + " enthält:\n\n");
        sb.Append(tab).Append("Nodes:").Append("\t").Append(nodeCount).Append('\n');
        sb.Append(tab + tab).Append("EmptyElements:").Append("\t").Append(emptyElementCount).Append('\n');
        sb.Append(tab + tab).Append("Node without tags:").Append("\t").Append(nodeWithoutTags).Append('\n');

        sb.Append(tab + tab).Append("highwayTags:").Append("\t").Append(highwayTagCount).Append('\n');

        sb.Append(tab + tab + tab).Append("highway_motorway         ").Append(highway_motorway).Append('\n');
        sb.Append(tab + tab + tab).Append("highway_motorway_link    ").Append(highway_motorway_link).Append('\n');
        sb.Append(tab + tab + tab).Append("highway_trunk            ").Append(highway_trunk).Append('\n');
        sb.Append(tab + tab + tab).Append("highway_trunk_link       ").Append(highway_trunk_link).Append('\n');
        sb.Append(tab + tab + tab).Append("highway_primary          ").Append(highway_primary).Append('\n');
        sb.Append(tab + tab + tab).Append("highway_primary_link     ").Append(highway_primary_link).Append('\n');
        sb.Append(tab + tab + tab).Append("highway_secondary        ").Append(highway_secondary).Append('\n');
        sb.Append(tab + tab + tab).Append("highway_secondary_link   ").Append(highway_secondary_link).Append('\n');
        sb.Append(tab + tab + tab).Append("highway_tertiary         ").Append(highway_tertiary).Append('\n');
        sb.Append(tab + tab + tab).Append("highway_residential      ").Append(highway_residential).Append('\n');

        //sb.Append(tab + tab + tab).Append("highway_mini_roundabout  ").Append(highway_mini_roundabout).Append('\n');
        //sb.Append(tab + tab + tab).Append("highway_stop             ").Append(highway_stop).Append('\n');
        //sb.Append(tab + tab + tab).Append("highway_give_way         ").Append(highway_give_way).Append('\n');
        //sb.Append(tab + tab + tab).Append("highway_traffic_signals  ").Append(highway_traffic_signals).Append('\n');
        //sb.Append(tab + tab + tab).Append("highway_crossing         ").Append(highway_crossing).Append('\n');
        //sb.Append(tab + tab + tab).Append("highway_bus_stop         ").Append(highway_bus_stop).Append('\n');
        //sb.Append(tab + tab + tab).Append("highway_platform         ").Append(highway_platform).Append('\n');
        //sb.Append(tab + tab + tab).Append("highway_turning_circle   ").Append(highway_turning_circle).Append('\n');

        //sb.Append(tab + tab + tab).Append("railway_station          ").Append(railway_station).Append('\n');
        //sb.Append(tab + tab + tab).Append("railway_halt             ").Append(railway_halt).Append('\n');
        //sb.Append(tab + tab + tab).Append("railway_tram_stop        ").Append(railway_tram_stop).Append('\n');
        //sb.Append(tab + tab + tab).Append("railway_crossing         ").Append(railway_crossing).Append('\n');
        //sb.Append(tab + tab + tab).Append("railway_subway_entrance  ").Append(railway_subway_entrance).Append('\n');


        //sb.Append(tab + tab + tab).Append("amenity_bus_station      ").Append(amenity_bus_station).Append('\n');
        //sb.Append(tab + tab + tab).Append("amenity_taxi             ").Append(amenity_taxi).Append('\n');
        //sb.Append(tab + tab + tab).Append("amenity_atm              ").Append(amenity_atm).Append('\n');
        //sb.Append(tab + tab + tab).Append("amenity_hospital         ").Append(amenity_hospital).Append('\n');
        //sb.Append(tab + tab + tab).Append("amenity_police           ").Append(amenity_police).Append('\n');
        //sb.Append(tab + tab + tab).Append("amenity_post_box         ").Append(amenity_post_box).Append('\n');
        //sb.Append(tab + tab + tab).Append("amenity_post_office      ").Append(amenity_post_office).Append('\n');
        //sb.Append(tab + tab + tab).Append("amenity_telephone        ").Append(amenity_telephone).Append('\n');
        //sb.Append(tab + tab + tab).Append("amenity_toilets          ").Append(amenity_toilets).Append('\n');


        //sb.Append(tab + tab + tab).Append("shop_bakery              ").Append(shop_bakery).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_beauty              ").Append(shop_beauty).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_beverages           ").Append(shop_beverages).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_books               ").Append(shop_books).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_boutique            ").Append(shop_boutique).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_clothes             ").Append(shop_clothes).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_dry_cleaning        ").Append(shop_dry_cleaning).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_electronics         ").Append(shop_electronics).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_florist             ").Append(shop_florist).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_ice_cream           ").Append(shop_ice_cream).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_mall                ").Append(shop_mall).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_supermarket         ").Append(shop_supermarket).Append('\n');
        //sb.Append(tab + tab + tab).Append("shop_shoes               ").Append(shop_shoes).Append('\n');

        //sb.Append(tab + tab + tab).Append("tourism_hostel           ").Append(tourism_hostel).Append('\n');
        //sb.Append(tab + tab + tab).Append("tourism_hotel            ").Append(tourism_hotel).Append('\n');
        //sb.Append(tab + tab + tab).Append("tourism_information      ").Append(tourism_information).Append('\n');


        //sb.Append(tab + tab + tab).Append("natural_bay              ").Append(natural_bay).Append('\n');
        //sb.Append(tab + tab + tab).Append("natural_beach            ").Append(natural_beach).Append('\n');


        //sb.Append(tab + tab + tab).Append("sport_swimming           ").Append(sport_swimming).Append('\n');
        //sb.Append(tab + tab + tab).Append("sport_soccer             ").Append(sport_soccer).Append('\n');
        //sb.Append(tab + tab + tab).Append("sport_volleyball         ").Append(sport_volleyball).Append('\n');
        //sb.Append(tab + tab + tab).Append("sport_skateboard         ").Append(sport_skateboard).Append('\n');
        //sb.Append(tab + tab + tab).Append("sport_basketball         ").Append(sport_basketball).Append('\n');


        sb.Append(tab).Append("Ways:      ").Append("\t").Append(wayCount).Append('\n');
        sb.Append(tab + tab).Append("laneInformation:      ").Append("\t").Append(laneInformation).Append('\n');

        sb.Append(tab + tab).Append("wayHighwayTags:     ").Append("\t").Append(wayHighwayTagCount).Append('\n');
        sb.Append(tab + tab).Append("nameTags:     ").Append("\t").Append(wayNameTagCount).Append('\n');

        sb.Append(tab).Append("Relations: ").Append("\t").Append(relationCount).Append('\n');

        return sb.ToString();
    }


    /// <summary>
    /// 
    /// </summary>
    internal static void PrintStatistics()
    {
        //Console.WriteLine(OSMStatistics.GetStatisticsString());
        Debug.Log(OSMStatistics.GetStatisticsString());
    }

    internal void reset()
    {
        filePath = String.Empty;

        nodeCount = 0;
        highwayTagCount = 0;

        wayCount = 0;
        wayHighwayTagCount = 0;
        wayNameTagCount = 0;
        laneInformation = 0;

        relationCount = 0;
        emptyElementCount = 0;


        highway_motorway = 0;
        highway_motorway_link = 0;
        highway_trunk = 0;
        highway_trunk_link = 0;
        highway_primary = 0;
        highway_primary_link = 0;
        highway_secondary = 0;
        highway_secondary_link = 0;
        highway_tertiary = 0;
        highway_residential = 0;
        highway_unclassified = 0;

        highway_mini_roundabout = 0;
        highway_stop = 0;
        highway_give_way = 0;
        highway_traffic_signals = 0;
        highway_crossing = 0;
        highway_bus_stop = 0;
        highway_platform = 0;
        highway_turning_circle = 0;
        highway_motorway_junction = 0;

        railway_station = 0;
        railway_halt = 0;
        railway_tram_stop = 0;
        railway_crossing = 0;
        railway_subway_entrance = 0;
        railway_level_crossing = 0;

        amenity_bus_station = 0;
        amenity_taxi = 0;
        amenity_atm = 0;
        amenity_hospital = 0;
        amenity_police = 0;
        amenity_post_box = 0;
        amenity_post_office = 0;
        amenity_telephone = 0;
        amenity_toilets = 0;

        shop_bakery = 0;
        shop_beauty = 0;
        shop_beverages = 0;
        shop_books = 0;
        shop_boutique = 0;
        shop_clothes = 0;
        shop_dry_cleaning = 0;
        shop_electronics = 0;
        shop_florist = 0;
        shop_ice_cream = 0;
        shop_mall = 0;
        shop_supermarket = 0;
        shop_shoes = 0;

        tourism_hostel = 0;
        tourism_hotel = 0;
        tourism_information = 0;

        natural_bay = 0;
        natural_beach = 0;

        sport_swimming = 0;
        sport_soccer = 0;
        sport_volleyball = 0;
        sport_skateboard = 0;
        sport_basketball = 0;

        bridge = 0;

        crossing_no = 0;
        crossing_traffic_signals = 0;
        crossing_uncontrolled = 0;
    }
}

