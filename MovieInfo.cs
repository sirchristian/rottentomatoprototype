using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace APIHelper
{
    [DataContract]
    public class MovieInfo
    {
        [DataMember]
        public MovieBasicList movie_list { get; set; }

        [DataMember]
        public Movie[] movie_details { get; set; }
    }

    [DataContract]
    public class MovieBasicList
    {
        [DataMember]
        public Movie[] movies { get; set; }
    }

    [DataContract]
    public class Movie
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public string[] genres { get; set; }

        [DataMember]
        public string mpaa_rating { get; set; }

        [DataMember]
        public Ratings ratings { get; set; }

        [DataMember]
        public Posters posters { get; set; }

        [DataMember]
        public Cast[] abridged_cast { get; set; }

        [DataMember(IsRequired=false)]
        public Director[] abridged_directors { get; set; }

        [DataMember]
        public AltIds alternate_ids { get; set; }
    }

    [DataContract]
    public class Ratings
    {
        [DataMember]
        public int critics_score { get; set; }
    }

    [DataContract]
    public class Posters
    {
        [DataMember]
        public string detailed { get; set; }
    }

    [DataContract]
    public class AltIds
    {
        [DataMember]
        public int imdb { get; set; }
    }

    [DataContract]
    public class Cast
    {
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string[] characters { get; set; }
    }

    [DataContract]
    public class Director
    {
        [DataMember]
        public string name { get; set; }
    }
}
