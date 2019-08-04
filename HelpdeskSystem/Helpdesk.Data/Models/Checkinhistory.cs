﻿using System;
using System.Collections.Generic;

namespace Helpdesk.Website.Models
{
    public partial class Checkinhistory
    {
        public Checkinhistory()
        {
            Checkinqueueitem = new HashSet<Checkinqueueitem>();
        }

        public int CheckInId { get; set; }
        public int UnitId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public byte? ForcedCheckout { get; set; }

        public virtual Unit Unit { get; set; }
        public virtual ICollection<Checkinqueueitem> Checkinqueueitem { get; set; }
    }
}