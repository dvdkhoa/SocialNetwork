﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DTO.Entities
{
    public class Detail
    {
        public string Text { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();
    }
}
