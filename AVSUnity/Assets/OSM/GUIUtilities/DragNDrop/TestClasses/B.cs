using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class B : A
{
    public B(int size) : base(size)
    {
        this.size = size * size;
    }

    public override string ToString()
    {
        return "B(" + size + ")";
    }
}

