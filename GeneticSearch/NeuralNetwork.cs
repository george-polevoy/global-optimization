using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneticSearch
{
    public class NeuralNetwork
    {
        private List<List<Tuple<double, double>>> middleWeights;
        private List<List<Tuple<double,double>>> outputWeights;

        private List<Tuple<double,double>> CreateZeroList(int count)
        {
            return new List<Tuple<double,double>>(Enumerable.Repeat(Tuple.Create(0.0, 0.0), count));
        }

        public NeuralNetwork(int numberOfInputs, int numberOfOutputs, int numberOfHiddenNewrons)
        {
            middleWeights = new List<List<Tuple<double,double>>>(numberOfHiddenNewrons);
            middleWeights.AddRange(Enumerable.Range(0,numberOfHiddenNewrons).Select(i=>CreateZeroList(numberOfInputs)));

            outputWeights = new List<List<Tuple<double,double>>>(numberOfInputs+1);
            outputWeights.AddRange(Enumerable.Range(0,numberOfOutputs).Select(i=>CreateZeroList(numberOfHiddenNewrons)));
        }

        public NeuralNetwork Variate(Random sequence, double diameter)
        {
            var nn = new NeuralNetwork(middleWeights[0].Count, outputWeights.Count, middleWeights.Count);
            nn.ReadState(SerializeComponents().Select(c=>c+(sequence.NextDouble()-0.5) * diameter));
            return nn;
        }

        double Sigma(double x)
        {
            return Math.Atan(x);
        }

        public IEnumerable<double> Compute(IList<double> arguments)
        {
            var middles = new List<double>(middleWeights.Count);
            
            middles.AddRange(middleWeights.Select(t => t.Select((mw, inputIndex) => Sigma(arguments[inputIndex] *mw.Item1 + mw.Item2)).Sum()));

            return outputWeights.Select(t => t.Select((ow, middleIndex) => Sigma(middles[middleIndex]*ow.Item1 + ow.Item2)).Sum());
        }

        public int Size()
        {
            return (middleWeights.Count + outputWeights.Count)*2;
        }

        public IEnumerable<double> SerializeComponents()
        {
            return middleWeights.SelectMany(mw => mw.SelectMany(EncodeToSeparate)).Concat(outputWeights.SelectMany(mw => mw.SelectMany(EncodeToSeparate)));
        }

        public byte[] Serialize()
        {
            return middleWeights.SelectMany(mw=>mw.SelectMany(Encode)).Concat(outputWeights.SelectMany(mw=>mw.SelectMany(Encode))).ToArray();
        }

        IEnumerable<double> EncodeToSeparate(Tuple<double, double> i)
        {
            yield return i.Item1;
            yield return i.Item2;
        }

        IEnumerable<byte> Encode(Tuple<double, double> i)
        {
            return BitConverter.GetBytes(EncodeToShort(i.Item1)).Concat(BitConverter.GetBytes(EncodeToShort(i.Item2))).ToArray();
        }

        public byte Encode(double x)
        {
            var y = Math.Round(((Math.Atan(x / scale)) / Math.PI + 0.5) * 254);
            return (byte)y;
        }

        public byte EncodeToShort(double x)
        {
            return 0;
        }

        public double Decode(byte y)
        {
            var decode = ((double)y - 127) / 255;

            return decode * scale;
        }

        const double scale = 5;

        public void ReadState(IEnumerable<double> memory)
        {
            using (var iter = memory.GetEnumerator())
            {
                for (var i = 0; i < middleWeights.Count; i++)
                {
                    for (var j = 0; j < middleWeights[i].Count; j++)
                    {
                        var a = iter.Current;
                        iter.MoveNext();
                        var b = iter.Current;
                        middleWeights[i][j] = Tuple.Create(a, b);
                    }
                }

                for (var i = 0; i < outputWeights.Count; i++)
                {
                    for (var j = 0; j < outputWeights[i].Count; j++)
                    {
                        var a = iter.Current;
                        iter.MoveNext();
                        var b = iter.Current;
                        outputWeights[i][j] = Tuple.Create(a, b);
                    }
                }
            }
        }

        public void ReadState(byte[] memory)
        {
            var ms = new MemoryStream(memory);
            var r = new BinaryReader(ms);

            for (var i = 0; i < middleWeights.Count; i++)
            {
                for (var j = 0; j < middleWeights[i].Count; j++)
                {
                    middleWeights[i][j] = Tuple.Create(Decode(r.ReadByte()), Decode(r.ReadByte()));
                }
            }

            for (var i = 0; i < outputWeights.Count; i++)
            {
                for (var j = 0; j < outputWeights[i].Count; j++)
                {
                    outputWeights[i][j] = Tuple.Create(Decode(r.ReadByte()), Decode(r.ReadByte()));
                }
            }
        }
    }
}