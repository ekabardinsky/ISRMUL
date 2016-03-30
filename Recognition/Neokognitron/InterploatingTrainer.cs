using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.Neokognitron
{
    class InterploatingTrainer : Trainer
    {
        Logger Logger { get; set; }
        List<double[,]> trainData { get; set; }
        NeoKognitron neo { get; set; }
        int Height { get { return prevC[0].Neurons.GetLength(0); } }
        int Width { get { return prevC[0].Neurons.GetLength(1); } }
        List<C> prevC { get { return neo.U[layer - 1].C; } }
        int layer { get; set; }
        List<string> Labels { get; set; }

        public InterploatingTrainer(NeoKognitron neokognitron, List<double[,]> trainSet,List<string> labelSet, int layer, Logger logger)
        {
            Labels = labelSet;
            neo = neokognitron;
            trainData = trainSet;
            this.layer = layer;
            Logger = logger;
        }

        public override void Train()
        {
            bool firstStep = true;
            neo.U.Add(new U() { NeoKognitron = neo}); 
            List<Clazz> clazzs = new List<Clazz>();
            bool training = true;
            int generated = 0;
            while (training)
            {
                training = false;
                for (int p = 0; p < trainData.Count; p++)
                {
                    //propagate
                    Logger("pattern " + p + ", class count " + clazzs.Count, "propagate\ngenerated vectors "+generated);
                    neo.clearOperation();
                    neo.input(trainData[p]);
                    Vector pattern = getPattern();

                    //get response
                    Logger("pattern " + p + ", class count " + clazzs.Count, "get response\ngenerated vectors " + generated);
                    double maxS = 0;
                    string classificationLabel = null;
                    for (int c = 0; c < clazzs.Count; c++)
                    {
                        double s = clazzs[c].Compute(pattern);
                        if (s > maxS)
                        {
                            maxS = s;
                            classificationLabel = clazzs[c].Name;
                        }
                    }
                    //first intertion
                    if (firstStep)
                    {
                        Logger("pattern " + p + ", class count " + clazzs.Count, "first iteration\ngenerated vectors " + generated);
                        Clazz clazz = new Clazz(Labels[p]);
                        clazz.AddReferenceVector(pattern);
                        clazzs.Add(clazz);
                        S plane = generateS(clazz);
                        neo.U[layer].S.Add(plane);
                        firstStep = false;
                        generated++;
                    }
                    else if (classificationLabel != null && !classificationLabel.Equals(Labels[p]))//wrong classify
                    {

                        Clazz clazz = clazzs.Where(x => x.Name.Equals(Labels[p])).FirstOrDefault();
                        if (clazz == null)//class missing
                        {
                            Logger("pattern " + p + ", class count " + clazzs.Count, "wrong missing\ngenerated vectors " + generated);
                            clazz = new Clazz(Labels[p]);
                            clazz.AddReferenceVector(pattern);
                            clazzs.Add(clazz);
                            S plane = generateS(clazz);
                            neo.U[layer].S.Add(plane);
                            training = true;
                        }
                        else // class exists
                        {
                            Logger("pattern " + p + ", class count " + clazzs.Count, "wrong exists");
                            clazz.AddReferenceVector(pattern);
                            training = true;
                            generated++;
                        }
                    }
                    else
                    {
                        //prepare not learned class
                        Clazz notLearned = clazzs.Where(x => x.Name.Equals(Labels[p]) && x.isNotLearned()).FirstOrDefault();
                        if (notLearned != null)
                        {
                            generated++;
                            notLearned.ReferenceVectors.Add(pattern);
                        }
                    }
                }
            }
            Logger("class count " + clazzs.Count, "end \ngenerated vectors " + generated);
        }
        SInterploating generateS(Clazz clazz)
        {
            SInterploating s = new SInterploating() { Clazz = clazz ,PrevC=this.prevC};
            s.Neurons = new Neuron[1, 1];//warning 1,1 situately

            SCellInteploating cell = new SCellInteploating() { Plane = s};
            s.Neurons[0, 0] = cell;

            return s;
        }
        private Vector getPattern()
        {
            double[] pattern = new double[prevC.Count * Width * Height];
            int dimension = 0;
            foreach (C c in prevC)
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        pattern[dimension++] = c.Neurons[y, x].getOut(1);

            return new Vector(pattern);
        }
    }
}
