using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class A
{
    public int size = 0;

    public A(int size)
    {
        this.size = size;
    }

    public override string ToString()
    {
        return "A(" + size + ")";
    }
}
