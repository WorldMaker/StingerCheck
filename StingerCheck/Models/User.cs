﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StingerCheck.Models
{
    public class User
    {
        public long Id { get; set; }

        [Index(IsUnique=true)]
        [MaxLength(256)]
        public string Email { get; set; }
    }
}