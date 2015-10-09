using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneTapCheckin
{
    class checkin
    {
        public class Meta
        {
            public int code { get; set; }
        }

        public class Item
        {
            public int unreadCount { get; set; }
        }

        public class Notification
        {
            public string type { get; set; }
            public Item item { get; set; }
        }

        public class DisplayGeo
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Photo
        {
            public string prefix { get; set; }
            public string suffix { get; set; }
        }

        public class User
        {
            public string id { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string gender { get; set; }
            public string relationship { get; set; }
            public Photo photo { get; set; }
        }

        public class Contact
        {
        }

        public class Location
        {
            public bool isFuzzed { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
            public string cc { get; set; }
            public string country { get; set; }
        }

        public class Icon
        {
            public string prefix { get; set; }
            public string suffix { get; set; }
        }

        public class Category
        {
            public string id { get; set; }
            public string name { get; set; }
            public string pluralName { get; set; }
            public string shortName { get; set; }
            public Icon icon { get; set; }
            public bool primary { get; set; }
        }

        public class Stats
        {
            public int checkinsCount { get; set; }
            public int usersCount { get; set; }
            public int tipCount { get; set; }
        }

        public class Specials
        {
            public int count { get; set; }
        }

        public class DisplayGeo2
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Photo2
        {
            public string prefix { get; set; }
            public string suffix { get; set; }
        }

        public class User2
        {
            public string id { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string gender { get; set; }
            public string relationship { get; set; }
            public Photo2 photo { get; set; }
        }

        public class Item2
        {
            public string id { get; set; }
            public int createdAt { get; set; }
            public string type { get; set; }
            public int timeZoneOffset { get; set; }
            public DisplayGeo2 displayGeo { get; set; }
            public string exactContextLine { get; set; }
            public User2 user { get; set; }
        }

        public class Group
        {
            public string type { get; set; }
            public string name { get; set; }
            public int count { get; set; }
            public List<Item2> items { get; set; }
        }

        public class HereNow
        {
            public int count { get; set; }
            public string summary { get; set; }
            public List<Group> groups { get; set; }
        }

        public class Target2
        {
            public string type { get; set; }
            public string url { get; set; }
        }

        public class Object
        {
            public string id { get; set; }
            public string type { get; set; }
            public Target2 target { get; set; }
            public bool ignorable { get; set; }
        }

        public class Target
        {
            public string type { get; set; }
            public Object @object { get; set; }
        }

        public class Item3
        {
            public string summary { get; set; }
            public string type { get; set; }
            public string reasonName { get; set; }
            public Target target { get; set; }
        }

        public class Reasons
        {
            public int count { get; set; }
            public List<Item3> items { get; set; }
        }

        public class Venue
        {
            public string id { get; set; }
            public string name { get; set; }
            public Contact contact { get; set; }
            public Location location { get; set; }
            public List<Category> categories { get; set; }
            public bool verified { get; set; }
            public Stats stats { get; set; }
            public Specials specials { get; set; }
            public HereNow hereNow { get; set; }
            public Reasons reasons { get; set; }
        }

        public class Source
        {
            public string name { get; set; }
            public string url { get; set; }
        }

        public class Photos
        {
            public int count { get; set; }
            public List<object> items { get; set; }
        }

        public class Posts
        {
            public int count { get; set; }
            public int textCount { get; set; }
        }

        public class Likes
        {
            public int count { get; set; }
            public List<object> groups { get; set; }
        }

        public class Comments
        {
            public int count { get; set; }
            public List<object> items { get; set; }
        }

        public class Score2
        {
            public string icon { get; set; }
            public string message { get; set; }
            public int points { get; set; }
        }

        public class Score
        {
            public int total { get; set; }
            public List<Score2> scores { get; set; }
        }

        public class Checkin
        {
            public string id { get; set; }
            public int createdAt { get; set; }
            public string type { get; set; }
            public int timeZoneOffset { get; set; }
            public DisplayGeo displayGeo { get; set; }
            public string exactContextLine { get; set; }
            public User user { get; set; }
            public Venue venue { get; set; }
            public Source source { get; set; }
            public Photos photos { get; set; }
            public Posts posts { get; set; }
            public Likes likes { get; set; }
            public bool like { get; set; }
            public Comments comments { get; set; }
            public Score score { get; set; }
            public bool hasVenueLeaderboard { get; set; }
            public List<string> objectLeaderboards { get; set; }
        }

        public class Response
        {
            public Checkin checkin { get; set; }
            public List<object> notifications { get; set; }
            public List<string> notificationsOrder { get; set; }
        }

        public class RootObject2
        {
            public Meta meta { get; set; }
            public List<Notification> notifications { get; set; }
            public Response response { get; set; }
        }
    }
}
