﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retrofit.Net.Attributes.Parameters
{
    public class QueryAttribute : ValueAttribute
    {
        public QueryAttribute(string value)
        {
            this.Value = value;
        }
    }
}
