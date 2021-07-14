using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastMobile.FXamarin.Core
{
    public class FMapService
    {
        public const string API_URL = "https://maps.googleapis.com/maps/api";
        public const string PLACE_URL = "place";
        public const string NEAR_BY_SEARCH_URL = "nearbysearch";
        public const string DETAIL_URL = "details";
        public const string GEOCODE = "geocode";
        public const string TYPES = "accounting,airport,amusement_park,aquarium,art_gallery,atm,bakery,bank,bar,beauty_salon,bicycle_store,book_store,bowling_alley,bus_station,cafe,campground,car_dealer,car_rental,car_repair,car_wash,casino,cemetery,church,city_hall,clothing_store,convenience_store,courthouse,dentist,department_store,doctor,drugstore,electrician,electronics_store,embassy,fire_station,florist,funeral_home,furniture_store,gas_station,gym,hair_care,hardware_store,hindu_temple,home_goods_store,hospital,insurance_agency,jewelry_store,laundry,lawyer,library,light_rail_station,liquor_store,local_government_office,locksmith,lodging,meal_delivery,meal_takeaway,mosque,movie_rental,movie_theater,moving_company,museum,night_club,painter,park,parking,pet_store,pharmacy,physiotherapist,plumber,police,post_office,primary_school,real_estate_agency,restaurant,roofing_contractor,rv_park,school,secondary_school,shoe_store,shopping_mall,spa,stadium,storage,store,subway_station,supermarket,synagogue,taxi_stand,tourist_attraction,train_station,transit_station,travel_agency,university,veterinary_care,zoo";
        public const string RESULT_TYPE = "json";

        public static FMapService Instance { get; }

        static FMapService()
        {
            Instance = new FMapService();
        }

        public async Task<FResult> PlaceDetail(string apiKey, double lat, double lng, string language, bool sensor, string result_type = "street_address|administrative_area_level_5|administrative_area_level_4|administrative_area_level_3|administrative_area_level_2|administrative_area_level_1")
        {
            var message = await GetAsync($"{Path.Combine(API_URL, GEOCODE, RESULT_TYPE)}?latlng={lat},{lng}&language={language}&sensor={sensor}&result_type={result_type}&key={apiKey}");
            if (message.Success != 1)
                return new FResult { OK = message };

            var result = FUtility.ToObject<FResult>(message.Message);
            result.OK = message;
            return result;
        }

        public async Task<FResult> SearchByNear(string apiKey, double lat, double lng, double radius, string language, string types = TYPES)
        {
            var message = await GetAsync($"{Path.Combine(API_URL, PLACE_URL, NEAR_BY_SEARCH_URL, RESULT_TYPE)}?location={lat},{lng}&radius={radius}&key={apiKey}&language={language}&types={types}");
            if (message.Success != 1)
                return new FResult { OK = message };

            var result = FUtility.ToObject<FResult>(message.Message);
            result.OK = message;
            return result;
        }

        public string IconFromType(string type)
        {
            return type switch
            {
                "accounting" => FIcons.AccountBoxOutline,
                "airport" => FIcons.Airplane,
                "amusement_park" => FIcons.MusicCircle,
                "aquarium" => FIcons.Fish,
                "art_gallery" => FIcons.ViewCarouselOutline,
                "atm" => FIcons.Atm,
                "bakery" => FIcons.Store,
                "bank" => FIcons.Bank,
                "bar" => FIcons.MusicBox,
                "beauty_salon" => FIcons.HairDryer,
                "bicycle_store" => FIcons.Bicycle,
                "book_store" => FIcons.Bookshelf,
                "bowling_alley" => FIcons.Bowling,
                "bus_station" => FIcons.Bus,
                "cafe" => FIcons.Coffee,
                "campground" => FIcons.Campfire,
                "car_dealer" => FIcons.CarHatchback,
                "car_rental" => FIcons.CarHatchback,
                "car_repair" => FIcons.CarHatchback,
                "car_wash" => FIcons.CarHatchback,
                "casino" => FIcons.CardsClub,
                "cemetery" => FIcons.Tree,
                "church" => FIcons.Church,
                "city_hall" => FIcons.CityVariant,
                "clothing_store" => FIcons.Store,
                "convenience_store" => FIcons.Store,
                "courthouse" => FIcons.HomeCity,
                "dentist" => FIcons.Doctor,
                "department_store" => FIcons.HomeCity,
                "doctor" => FIcons.Doctor,
                "drugstore" => FIcons.Store,
                "electrician" => FIcons.ElectricSwitch,
                "electronics_store" => FIcons.Store,
                "embassy" => FIcons.HomeCircle,
                "fire_station" => FIcons.Fireplace,
                "florist" => FIcons.Flower,
                "funeral_home" => FIcons.FlowerTulip,
                "furniture_store" => FIcons.Store,
                "gas_station" => FIcons.GasStation,
                "gym" => FIcons.WeightLifter,
                "hair_care" => FIcons.HairDryer,
                "hardware_store" => FIcons.Store,
                "hindu_temple" => FIcons.HomeFlood,
                "home_goods_store" => FIcons.Store,
                "hospital" => FIcons.HospitalBuilding,
                "insurance_agency" => FIcons.FileCertificate,
                "jewelry_store" => FIcons.Diamond,
                "laundry" => FIcons.TshirtCrew,
                "lawyer" => FIcons.ScaleBalance,
                "library" => FIcons.Library,
                "light_rail_station" => FIcons.TrainVariant,
                "liquor_store" => FIcons.Beer,
                "local_government_office" => FIcons.OfficeBuilding,
                "locksmith" => FIcons.HomeLock,
                "lodging" => FIcons.Bed,
                "meal_delivery" => FIcons.SilverwareForkKnife,
                "meal_takeaway" => FIcons.SilverwareForkKnife,
                "mosque" => FIcons.Church,
                "movie_rental" => FIcons.MovieRoll,
                "movie_theater" => FIcons.Movie,
                "moving_company" => FIcons.Movie,
                "museum" => FIcons.HomeCity,
                "night_club" => FIcons.WeatherNight,
                "painter" => FIcons.Draw,
                "park" => FIcons.PineTreeBox,
                "parking" => FIcons.CarBrakeParking,
                "pet_store" => FIcons.Cat,
                "pharmacy" => FIcons.Pharmacy,
                "physiotherapist" => FIcons.Marker,
                "plumber" => FIcons.WaterPump,
                "police" => FIcons.PoliceBadge,
                "post_office" => FIcons.PostageStamp,
                "primary_school" => FIcons.School,
                "real_estate_agency" => FIcons.HomeCity,
                "restaurant" => FIcons.SilverwareForkKnife,
                "roofing_contractor" => "",
                "rv_park" => FIcons.PineTreeBox,
                "school" => FIcons.School,
                "secondary_school" => FIcons.School,
                "shoe_store" => FIcons.ShoeFormal,
                "shopping_mall" => FIcons.Shopping,
                "spa" => FIcons.Spa,
                "stadium" => FIcons.Stadium,
                "storage" => FIcons.Harddisk,
                "store" => FIcons.Store,
                "subway_station" => FIcons.Subway,
                "supermarket" => FIcons.Cart,
                "synagogue" => FIcons.Church,
                "taxi_stand" => FIcons.Taxi,
                "tourist_attraction" => FIcons.HumanGreeting,
                "train_station" => FIcons.Train,
                "transit_station" => FIcons.TrainCar,
                "travel_agency" => FIcons.TrainVariant,
                "university" => FIcons.School,
                "veterinary_care" => FIcons.DogService,
                "zoo" => FIcons.Cat,
                "administrative_area_level_1" or "administrative_area_level_2" or "administrative_area_level_3" or "administrative_area_level_4" or "administrative_area_level_5" => FIcons.CityVariant,
                "archipelago" => FIcons.Island,
                "country" => FIcons.FlagVariant,
                "food" => FIcons.SilverwareForkKnife,
                "health" => FIcons.HandHeart,
                "intersection" => FIcons.PlusThick,
                "locality" or "political" => FIcons.CityVariant,
                "post_box" => FIcons.MailboxOpenUp,
                _ => FIcons.MapMarker
            };
        }

        public string HexFromType(string type)
        {
            return type switch
            {
                "bicycle_store" or "book_store" or "clothing_store" or "convenience_store" or "department_store" or "drugstore" or "electronics_store" or "furniture_store" or "hardware_store" or "home_goods_store" or "jewelry_store" or "liquor_store" or "pet_store" or "shoe_store" or "store" => "#177fd4",
                "food" or "archipelago" or "restaurant" or "meal_delivery" or "meal_takeaway" or "cafe" => "#fab146",
                "health" or "hospital" or "airport" or "bus_station" or "gas_station" or "fire_station" or "city_hall" or "church" or "library" or "movie_rental" or "movie_theater" or "post_office" or "supermarket" or "subway_station" or "train_station" or "transit_station" or "light_rail_station" or "local_government_office" or "lodging" => "#f05e8f",
                "accounting" or "amusement_park" or "aquarium" or "art_gallery" or "atm" or "bakery" or "bank" or "bar" or "beauty_salon" or "bowling_alley" or "campground" or "car_dealer" or "car_rental" or "car_repair" or "car_wash" or "casino" or "cemetery" or "courthouse" or "dentist" or "doctor" or "electrician" or "embassy" or "florist" or "funeral_home" or "gym" or "hair_care" or "hindu_temple" or "insurance_agency" or "laundry" or "lawyer" or "locksmith" => "#fab146",
                "mosque" or "moving_company" or "museum" or "night_club" or "painter" or "park" or "parking" or "pharmacy" or "physiotherapist" or "plumber" or "police" or "primary_school" or "real_estate_agency" or "roofing_contractor" or "rv_park" or "school" or "secondary_school" or "shopping_mall" or "spa" or "stadium" or "storage" or "synagogue" or "taxi_stand" or "tourist_attraction" or "travel_agency" or "university" or "veterinary_care" or "zoo" => "#78909c",
                _ => "#78909c"
            };
        }

        public string LanguageText(bool v)
        {
            return v ? "vi" : "en";
        }

        private async Task<FMessage> GetAsync(string url)
        {
            try
            {
                using var client = new HttpClient();
                var result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                return FMessage.FromSuccess(0, await result.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return FMessage.FromFail(100, ex.Message);
            }
        }

        public class FResult
        {
            public FMessage OK { get; set; }

            public FMapResults[] results { get; set; }
        }

        public class FMapResults
        {
            public string name { get; set; }
            public string icon { get; set; }
            public string place_id { get; set; }
            public string vicinity { get; set; }
            public string formatted_address { get; set; }
            public string[] types { get; set; }
            public FMapGeometry geometry { get; set; }
            public FMapPlusCode plus_code { get; set; }
            public FMapAddressComponent[] address_components { get; set; }
        }

        public class FMapAddressComponent
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public string[] types { get; set; }
        }

        public class FMapGeometry
        {
            public FMapPosition location { get; set; }
        }

        public class FMapPosition
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class FMapPlusCode
        {
            public string compound_code { get; set; }
            public string global_code { get; set; }
        }
    }
}