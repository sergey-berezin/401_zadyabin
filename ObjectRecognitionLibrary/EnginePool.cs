using Microsoft.ML;
using Microsoft.ML.Transforms.Onnx;
using System.Collections.Concurrent;
using ObjectRecognitionLibrary.DataStructures;

namespace ObjectRecognitionLibrary
{
    public class EnginePool
    {
        private readonly ConcurrentBag<PredictionEngine<YoloV4BitmapData, YoloV4Prediction>> items = new ConcurrentBag<PredictionEngine<YoloV4BitmapData, YoloV4Prediction>>();
        private int counter = 0;
        private int MAX = 10;
        private MLContext mlContext;
        private Microsoft.ML.Data.TransformerChain<OnnxTransformer> model;

        public EnginePool(MLContext mlContext, Microsoft.ML.Data.TransformerChain<OnnxTransformer> model)
        {
            this.mlContext = mlContext;
            this.model = model;
        }

        public void Release(PredictionEngine<YoloV4BitmapData, YoloV4Prediction> item)
        {
            if (counter < MAX)
            {
                items.Add(item);
                counter++;
            }
        }
        public PredictionEngine<YoloV4BitmapData, YoloV4Prediction> Get()
        {
            PredictionEngine<YoloV4BitmapData, YoloV4Prediction> item;
            if (items.TryTake(out item))
            {
                counter--;
                return item;
            }
            else
            {
                var obj = mlContext.Model.CreatePredictionEngine<YoloV4BitmapData, YoloV4Prediction>(model);
                items.Add(obj);
                counter++;
                return obj;
            }
        }
    }
}
