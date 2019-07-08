using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchableStringAttribute : SearchableAttribute
    {
        public SearchableStringAttribute()
        {
            ExpressionProvider = new StringSearchExpressionProvider();
        }
    }
}
