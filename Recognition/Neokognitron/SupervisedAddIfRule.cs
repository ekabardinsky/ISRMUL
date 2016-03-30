using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISRMUL.Recognition.Neokognitron
{
    class SupervisedAddIfRule:AddIfRule
    {
        List<string> Labels { get; set; }
        public SupervisedAddIfRule(List<double[,]> trainData, int layer, double Dr, NeoKognitron neo, double LThresh, double RThresh, double[][] CPrevWeight, double[][] CWeight, double[][] DWeight, Logger logger,List<string> labels)
            :base(trainData,layer,Dr,neo,LThresh,RThresh,CPrevWeight,CWeight,DWeight,logger)
        {
            Labels = labels;
        }
        public override void Train()
        {
            List<S> tmp = new List<S>();
            stop = false;
            neo.U.Add(new U() { NeoKognitron = neo, Selectivity = LThresh });
            for (int p = 0; p < trainData.Count; p++)
            {
                Logger("pattern " + p, "newly " + newlySPlanes.Count);
                neo.clearOperation();
                clearOperation();
                neo.input(trainData[p]);

                for (int i = 0; i < prevC.Count; i++)
                {
                    for (int j = i; j < prevC.Count; j++)//look here, test i+1
                    {
                        if (stop)
                            goto end;
                        Logger("pattern " + p + "\nnewly " + newlySPlanes.Count + "(i=" + i + ",j=" + j + ")", "MaxT=" + maxT + "\nMaxO=" + maxO);
                        C one = prevC[i];
                        C two = prevC[j];
                        virtualPlane = getSplitedPlanes(one, two);
                        suppressToZero();
                        Point winner = getMaximun();
                        if (winner.X > 0)
                        {
                            newlySPlanes.Add(generateSPlane(i, j, winner.Y, winner.X));
                        }
                    }
                }
                tmp.AddRange(newlySPlanes);
                newlySPlanes.Clear();
            }
        end:
            neo.U[layer].S.AddRange(tmp);
            NeoKognitron.CConnectToS(neo.U[layer], DWeight);
            NeoKognitron.VConnectToC(neo.U[layer], CWeight);
        }
    }
}
