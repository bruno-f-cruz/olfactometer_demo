using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenCV.Net;


[Combinator]
[Description("First and second arrays are the X and Y values of the calibration curve, respectively.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class InterpolateCalibration
{
    public float[][] CalibrationArray{ get; set; }

    public IObservable<float> Process(IObservable<float> source)
    {
        return source.Select(value => {
            var X = CalibrationArray[0];
            var Y = CalibrationArray[1];
            if (!(X.Length == Y.Length)){
                throw new DataMisalignedException("Size of the arrays does not match.");
            }
            int index = 0;
            while (index < X.Length){
                if (value < X[index]){
                    break;
                }
                index++;
            }

            if ((index == 0) || (index == (X.Length))){
                throw new ArgumentException("Input Value is outside the range to interpolate.");
            }
            return Interpolate(value, X[index-1], X[index], Y[index-1], Y[index], false);


        });
    }

    static float Interpolate(float inValue, float lower_x, float upper_x, float lower_y, float upper_y, bool clamp)
    {

        var slope = (upper_y - lower_y) / (upper_x - lower_x);
        var interpolated = (inValue-lower_x) * slope + lower_y;
        if (!clamp){return interpolated;}

        else{
            if (interpolated < lower_y){
                return lower_y;
            }
            if (interpolated > upper_y){
                return upper_y;
            }
            return interpolated;
        }
    }
}


