using SimpleExpressionEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine;
using NUnit.Framework;

namespace UnitTests
{

    public class TestCases
    {
        [Test]
        public void Tests()
        {
            var engine = new ExpressionEngine();
            Assert.AreEqual(1, engine.Evaluate("5 < 7"));
            Assert.AreEqual(0, engine.Evaluate("5 > 7"));

            Assert.AreEqual(1, engine.Evaluate("(5 < 7) && (3 > 2)"));
            Assert.AreEqual(0, engine.Evaluate("(5 < 7) && (3 < 2)"));
            Assert.AreEqual(0, engine.Evaluate("(5 > 7) && (3 > 2)"));
            Assert.AreEqual(0, engine.Evaluate("(5 > 7) && (3 < 2)"));

            Assert.AreEqual(1, engine.Evaluate("5 < 7 && 3 > 2"));
            Assert.AreEqual(0, engine.Evaluate("5 < 7 && 3 < 2"));
            Assert.AreEqual(0, engine.Evaluate("5 > 7 && 3 > 2"));
            Assert.AreEqual(0, engine.Evaluate("5 > 7 && 3 < 2"));

            Assert.AreEqual(1, engine.Evaluate("(5 < 7) || (3 > 2)"));
            Assert.AreEqual(1, engine.Evaluate("(5 < 7) || (3 < 2)"));
            Assert.AreEqual(1, engine.Evaluate("(5 > 7) || (3 > 2)"));
            Assert.AreEqual(0, engine.Evaluate("(5 > 7) || (3 < 2)"));

            Assert.AreEqual(3, engine.Evaluate("sqrt(9)"));
            Assert.AreEqual(2.302585092994046, engine.Evaluate("log(10)"));

            Assert.AreEqual(-27, engine.Evaluate("3 + 5 * ( 2 - 8 )"));
            Assert.AreEqual(1, engine.Evaluate("3 + 5 * ( 2 - 8 ) > sqrt(9)"));

            Assert.AreEqual(0, engine.Evaluate("(5 < 7) && (3 > 2) && 1 > 2"));
            Assert.AreEqual(1, engine.Evaluate("(5 < 7 && 3 < 2) || 2 > 1"));
            Assert.AreEqual(1, engine.Evaluate("((5 > 7) && (3 > 2)) || 2 > 1"));

            Assert.AreEqual(3, engine.Evaluate("3 + 5 * ( 2 - (1 + 1) )")); // 3
            Assert.AreEqual(1, engine.Evaluate("((5 < 7) || (3 > 2)) && (8 < (9 + 1))")); // 1
        }
    }

    public class StockTests
    {
        //[Test]
        //public void Functions()
        //{
        //    var ctx = new MyFunctionContext();
        //    Assert.AreEqual(Parser.Parse("avgv(3)").Eval(ctx), 80000);
        //    Assert.AreEqual(Parser.Parse("avgv(3,2)").Eval(ctx), 70000);
        //    Assert.AreEqual(Parser.Parse("maxc(5)").Eval(ctx), 118.4);
        //    Assert.AreEqual(Parser.Parse("maxc(3,2)").Eval(ctx), 113.5);
        //}

        //[Test]
        //public void FunctionsThatDontRequireArgs()
        //{
        //    var ctx = new MyFunctionContext();
        //    Assert.AreEqual(Parser.Parse("c").Eval(ctx), 118.4);
        //    Assert.AreEqual(Parser.Parse("c5").Eval(ctx), 103.2);
        //}

        //[Test]
        //public void CombinedFuction()
        //{
        //    var ctx = new MyFunctionContext();
        //    var res = Parser.Parse("avgv(3) > 70000");
        //    Assert.IsTrue(false);
        //}
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
            StockPrice val;
            if (name.Length == 1)
            {
                val = _data.Last();
            }
            else
            {
                var pos = _data.Count - int.Parse(name.Substring(1));
                name = name[0].ToString();
                val = _data[pos];
            }

            if (name == "c")
            {
                return val.ClosePrice;
            }
            else if (name == "o")
            {
                return val.OpenPrice;
            }
            else if (name == "h")
            {
                return val.HighPrice;
            }
            else if (name == "l")
            {
                return val.LowPrice;
            }
            else if (name == "v")
            {
                return val.Volume;
            }


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
                else if (ele == 'v')
                {
                    propertySelector = f => f.Volume;
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
