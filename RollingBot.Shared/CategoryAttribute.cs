﻿namespace RollingBot.Shared;

[AttributeUsage(AttributeTargets.All)]
public class CategoryAttribute : Attribute
{
    public string[] Category;

    public CategoryAttribute(params string[] thing)
    {
        Category = thing;
    }
}