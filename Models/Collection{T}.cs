﻿namespace Models
{
    public class Collection<T> : Resource
    {
        public T[] Value { get; set; }
    }
}
