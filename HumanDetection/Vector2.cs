using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanDetection
{
    class Vector2
    {
        [JsonProperty("@type")]
        public String type = "Vector2"; // Untuk menandai tipe data yang dikirim
        public float x;
        public float y;
    }
}
