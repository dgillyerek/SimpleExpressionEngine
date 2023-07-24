using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleExpressionEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

namespace UnitTests
{
    [TestClass]
    public class StockTests
    {
        [TestMethod]
        public void Functions()
        {
            var ctx = new MyFunctionContext();
            //Assert.AreEqual(Parser.Parse("avgv(3)").Eval(ctx), 80000);
            //Assert.AreEqual(Parser.Parse("avgv(3,2)").Eval(ctx), 60000);
            Assert.AreEqual(Parser.Parse("maxc(5)").Eval(ctx), 118.4);
            Assert.AreEqual(Parser.Parse("maxc(3,2)").Eval(ctx), 113.5);
        }

        [TestMethod]
        public void FunctionsThatDontRequireArgs()
        {
            var ctx = new MyFunctionContext();
            Assert.AreEqual(Parser.Parse("c()").Eval(ctx), 118.4);
            Assert.AreEqual(Parser.Parse("c(5)").Eval(ctx), 113.5);
        }
    }

    public class FunctionProperties
    {
        public int MinArgs { get; set; }
        public int MaxArgs { get; set; }

    }

    public class Args
    {
        public int OffSet { get; set; }
        public int Length { get; set; }
    }


    //https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce
    class MyFunctionContext : IContext
    {
        private List<StockPrice> _data;
        public MyFunctionContext()
        {
            _data = SampleData.GetSampleData();
        }

        public double ResolveVariable(string name)
        {
            throw new InvalidDataException($"Unknown variable: '{name}'");
        }

        public Args ConvertArgs(double[] args)
        {
            var offset = 0;
            if (args.Length == 2)
            {
                //todo check type for cast?
                offset = (int)args[1];
            }

            return new Args() { OffSet = offset, Length = (int)args[0] };
        }

        public double CallFunction(string name, double[] arguments)
        {
            if (name.Length == 4)
            {
                var ele = name[3];
                var fun = name.Substring(0, 3);

                Func<StockPrice, double> propertySelector = f => f.ClosePrice;
                var args = ConvertArgs(arguments);

                if (ele == 'o')
                {
                    propertySelector = f => f.OpenPrice;
                }
                else if (ele == 'h')
                {
                    propertySelector = f => f.HighPrice;
                }
                else if (ele == 'l')
                {
                    propertySelector = f => f.LowPrice;
                }

                if (fun == "avg")
                {
                    return Avg(args, propertySelector);
                }
                else if (fun == "max")
                {
                    return Max(args, propertySelector);
                }
                else if (fun == "min")
                {
                    return Min(args, propertySelector);
                }
            }

            throw new InvalidDataException($"Unknown function: '{name}'");
        }

        private t Max<t>(Args args, Func<StockPrice, t> propertyDelegate)
        {
            var len = _data.Count - args.Length;
            return _data.Skip(len).Take(args.Length - args.OffSet).Max(propertyDelegate);
        }

        private t Min<t>(Args args, Func<StockPrice, t> propertyDelegate)
        {
            var len = _data.Count - args.Length;
            return _data.Skip(len).Take(args.Length - args.OffSet).Min(propertyDelegate);
        }

        private double Avg(Args args, Func<StockPrice, double> propertyDelegate)
        {
            var len = _data.Count - args.Length;
            return _data.Skip(len).Take(args.Length - args.OffSet).Average(propertyDelegate);
        }
    }
}
