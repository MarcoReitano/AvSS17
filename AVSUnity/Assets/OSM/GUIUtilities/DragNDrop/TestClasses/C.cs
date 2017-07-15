using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class C
{
    public string name = string.Empty;

    public C(string name)
    {
        this.name = name;
    }

    public void foo<T>(T bar)
    {
        //bar.GetType().IsSubclassOf()
    }  

    public override string ToString()
    {
        return this.name;
    }
}

