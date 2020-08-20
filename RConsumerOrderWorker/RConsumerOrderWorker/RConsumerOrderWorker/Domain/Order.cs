﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RConsumerOrderWorker.Domain
{
    public sealed class Order
    {
        public int OrderNumber { get; set; }
        public string ItemName { get; set; }
        public float Price { get; set; }
        public string OrderGuid { get; set; }
    }
}
