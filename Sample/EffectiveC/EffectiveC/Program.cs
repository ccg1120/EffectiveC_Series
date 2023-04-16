using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectiveC
{

    //class MyType
    //{
    //    public void DoStuff()
    //    {

    //    }

    //}


    //class Program
    //{
    //    public void test(IEnumerable theCollection)
    //    {
    //        foreach (MyType t in theCollection)
    //            t.DoStuff();

    //        IEnumerator it = theCollection.GetEnumerator();
    //        while (it.MoveNext())
    //        {
    //            MyType t = (MyType)it.Current;
    //            t.DoStuff();
    //        }

    //        IEnumerable collection = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
    //        var collection2 = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

    //    }
    class Test
    {
        static void Main(string[] args)
        {
            //int intValue = 10;

            //object o = intValue;

            //var intTest = o as int?;
            int test = 1 + EffectiveC_Library.Class1.ConstInt;
            int test2 = 2 + EffectiveC_Library.Class1.StaticReadOnlyInt;
            Console.WriteLine($"ConstInt = {EffectiveC_Library.Class1.ConstInt}");
            Console.WriteLine($"StaticReadOnlyInt = {EffectiveC_Library.Class1.StaticReadOnlyInt}");
            Console.ReadLine();
        }
    }
}
